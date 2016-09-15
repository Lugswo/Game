using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace The_Dream.Classes
{
    public class Server
    {
        public string hostip;
        static NetIncomingMessage ServerInc;
        static NetPeerConfiguration ServerConfig;
        static NetServer server;
        public List<Player> GameState;
        public bool host;
        bool addedNewPlayer = true;
        PlayerUpdate playerUpdate;
        public Map map;
        bool Up, Down, Left, Right;
        public void GetReferences(Map realMap)
        {
            map = realMap;
            playerUpdate.GetReferences(map);
        }
        enum PacketTypes
        {
            LOGIN,
            MOVE,
            WORLDSTATE,
            ADDPLAYER,
            REMOVEPLAYER,
            JOINED,
            NEWAREA,
            SHUTDOWN
        }
        public void SendGameState()
        {
            NetOutgoingMessage outmsg = server.CreateMessage();
            outmsg.Write((byte)PacketTypes.WORLDSTATE);
            outmsg.Write(GameState.Count);
            foreach (Player p in GameState)
            {
                outmsg.Write(p.X);
                outmsg.Write(p.Y);
                outmsg.Write(p.VelocityX);
                outmsg.Write(p.VelocityY);
                outmsg.Write(p.AreaX);
                outmsg.Write(p.AreaY);
            }
            if (server.ConnectionsCount > 0)
            {
                server.SendMessage(outmsg, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }
        public void LoadContent()
        {
            ServerConfig = new NetPeerConfiguration("game");
            ServerConfig.Port = 25565;
            ServerConfig.MaximumConnections = 4;
            ServerConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            server = new NetServer(ServerConfig);
            if (host == true)
            {
                server.Start();
                hostip = "localhost";
            }
            GameState = new List<Player>();
            playerUpdate = new PlayerUpdate();
            playerUpdate.map = map;
        }
        public void UnloadContent()
        {

        }
        public void Update(GameTime gameTime)
        {
            while ((ServerInc = server.ReadMessage()) != null)
            {
                switch (ServerInc.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        if (ServerInc.ReadByte() == (byte)PacketTypes.LOGIN)
                        {
                            ServerInc.SenderConnection.Approve();
                            Player player = new Player();
                            ServerInc.ReadAllProperties(player);
                            player.Connection = ServerInc.SenderConnection;
                            GameState.Add(player);
                            addedNewPlayer = false;
                            while (addedNewPlayer == false)
                            {
                                if (server.ConnectionsCount == GameState.Count)
                                {
                                    NetOutgoingMessage joinMsg = server.CreateMessage();
                                    joinMsg.Write((byte)PacketTypes.JOINED);
                                    int count = GameState.Count - 1;
                                    joinMsg.Write(count);
                                    for (int i = 0; i < count; i++)
                                    {
                                        Player temp = new Player();
                                        temp = GameState[i];
                                        joinMsg.WriteAllProperties(temp);
                                    }
                                    Thread.Sleep(100);
                                    server.SendMessage(joinMsg, ServerInc.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                                    NetOutgoingMessage outmsg = server.CreateMessage();
                                    outmsg.Write((byte)PacketTypes.ADDPLAYER);
                                    outmsg.WriteAllProperties(player);
                                    server.SendMessage(outmsg, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                                    addedNewPlayer = true;
                                }
                            }
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        byte b = ServerInc.ReadByte();
                        if (b == (byte)PacketTypes.MOVE)
                        {
                            foreach (Player p in GameState)
                            {
                                if (p.Connection != ServerInc.SenderConnection)
                                {
                                    continue;
                                }
                                p.Up = ServerInc.ReadBoolean();
                                p.Down = ServerInc.ReadBoolean();
                                p.Left = ServerInc.ReadBoolean();
                                p.Right = ServerInc.ReadBoolean();
                                p.PositionX = ServerInc.ReadInt32();
                                p.PositionY = ServerInc.ReadInt32();
                                break;
                            }
                        }
                        else if (b == (byte)PacketTypes.SHUTDOWN)
                        {
                            server.Shutdown("bye");
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        if (ServerInc.SenderConnection.Status == NetConnectionStatus.Disconnected || ServerInc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                        {
                            foreach (Player p in GameState)
                            {
                                if (p.Connection == ServerInc.SenderConnection)
                                {
                                    NetOutgoingMessage outmsg = server.CreateMessage();
                                    outmsg.Write((byte)PacketTypes.REMOVEPLAYER);
                                    outmsg.Write(GameState.IndexOf(p));
                                    GameState.Remove(p);
                                    if (server.ConnectionsCount > 0)
                                    {
                                        server.SendMessage(outmsg, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
            foreach (Player p in GameState)
            {
                if (p.newArea == true)
                {
                    NetOutgoingMessage outmsg = server.CreateMessage();
                    outmsg.Write((byte)PacketTypes.NEWAREA);
                    server.SendMessage(outmsg, p.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                    p.newArea = false;
                }
                Player temp = p;
                playerUpdate.Move(gameTime, ref temp, p.Up, p.Down, p.Left, p.Right);
            }
            SendGameState();
            foreach (Player pl in GameState)
            {
                pl.VelocityX = 0;
                pl.VelocityY = 0;
            }
        }
    }
}
