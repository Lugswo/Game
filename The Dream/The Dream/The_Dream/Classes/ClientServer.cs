using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;
using Lidgren.Network.Xna;

namespace The_Dream.Classes
{
    public class ClientServer
    {
        string hostip;
        NetIncomingMessage ClientInc;
        static NetIncomingMessage ServerInc;
        static NetPeerConfiguration ServerConfig;
        static NetServer server;
        static NetClient client;
        NetOutgoingMessage ClientOut;
        public List<Player> PlayerList, GameState;
        float ConnectionTimer;
        PlayerUpdate playerUpdate;
        public Image image = new Image();
        bool addedNewPlayer = true;
        public int PlayerID;
        enum PacketTypes
        {
            LOGIN,
            MOVE,
            WORLDSTATE,
            ADDPLAYER,
            JOINED
        }
        enum MoveDirection
        {
            MOVE,
            NONE
        }
        public void SendGameState()
        {
            NetOutgoingMessage outmsg = server.CreateMessage();
            outmsg.Write((byte)PacketTypes.WORLDSTATE);
            outmsg.Write(GameState.Count);
            foreach (Player p in GameState)
            {
                outmsg.WriteAllProperties(p);
            }
            server.SendMessage(outmsg, server.Connections, NetDeliveryMethod.Unreliable, 0);
        }
        public void GetInput()
        {
            MoveDirection MoveDir = new MoveDirection();
            MoveDir = MoveDirection.NONE;
            int X, Y;
            if (InputManager.Instance.KeyDown(Keys.Down) && InputManager.Instance.KeyDown(Keys.Up))
            {
                Y = 0;
            }
            else if (InputManager.Instance.KeyDown(Keys.Down))
            {
                Y = 10;
            }
            else if (InputManager.Instance.KeyDown(Keys.Up))
            {
                Y = -10;
            }
            else
            {
                Y = 0;
            }
            if (InputManager.Instance.KeyDown(Keys.Right) && InputManager.Instance.KeyDown(Keys.Left))
            {
                X = 0;
            }
            else if (InputManager.Instance.KeyDown(Keys.Right))
            {
                X = 10;
            }
            else if (InputManager.Instance.KeyDown(Keys.Left))
            {
                X = -10;
            }
            else
            {
                X = 0;
            }
            if (InputManager.Instance.KeyUp(Keys.Down) && InputManager.Instance.KeyUp(Keys.Up))
            {
                Y = 0;
            }
            if (InputManager.Instance.KeyUp(Keys.Left) && InputManager.Instance.KeyUp(Keys.Right))
            {
                X = 0;
            }
            if (X != 0 || Y != 0)
            {
                MoveDir = MoveDirection.MOVE;
            }
            if (InputManager.Instance.KeyDown(Keys.Q))
            {
                client.Disconnect("bye bye");
                server.Shutdown("bye bye");
                if (ScreenManager.Instance.IsTransitioning == false)
                {
                    ScreenManager.Instance.ChangeScreens("TitleScreen");
                }
            }
            if (MoveDir == MoveDirection.MOVE)
            {
                NetOutgoingMessage outmsg = client.CreateMessage();
                outmsg.Write((byte)PacketTypes.MOVE);
                outmsg.Write(X);
                outmsg.Write(Y);
                client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }
        public ClientServer()
        {

        }
        public void LoadContent()
        {
            ServerConfig = new NetPeerConfiguration("game");
            ServerConfig.Port = 25565;
            ServerConfig.MaximumConnections = 4;
            ServerConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            server = new NetServer(ServerConfig);
            server.Start();
            hostip = "localhost";
            NetPeerConfiguration ClientConfig = new NetPeerConfiguration("game");
            client = new NetClient(ClientConfig);
            ClientOut = client.CreateMessage();
            client.Start();
            ClientOut.Write((byte)PacketTypes.LOGIN);
            Player player = new Player();
            XmlManager<Player> playerLoader;
            playerLoader = new XmlManager<Player>();
            player = playerLoader.Load("Load/Gameplay/SaveFile.xml");
            ClientOut.WriteAllProperties(player);
            client.Connect(hostip, 25565, ClientOut);
            GameState = new List<Player>();
            PlayerList = new List<Player>();
            image.Path = "Gameplay/Characters/Sprites/Player/Player";
            image.Position = new Vector2(100, 100);
            image.LoadContent();
            ConnectionTimer = 0;
            playerUpdate = new PlayerUpdate();
        }
        public void UnloadContent()
        {

        }
        public void Update(GameTime gameTime)
        {
            GetInput();
            if (PlayerList.Count == 0)
            {
                ConnectionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (ConnectionTimer >= 5)
                {
                    if (ScreenManager.Instance.IsTransitioning == false)
                    {
                        server.Shutdown("bye bye");
                        ScreenManager.Instance.ChangeScreens("TitleScreen");
                    }
                }
            }
            if ((ServerInc = server.ReadMessage()) != null)
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
                        if (ServerInc.ReadByte() == (byte)PacketTypes.MOVE)
                        {
                            foreach (Player p in GameState)
                            {
                                if (p.Connection != ServerInc.SenderConnection)
                                {
                                    continue;
                                }
                                int X = ServerInc.ReadInt32();
                                int Y = ServerInc.ReadInt32();
                                p.X += X;
                                p.Y += Y;
                                SendGameState();
                                break;
                            }
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        if (ServerInc.SenderConnection.Status == NetConnectionStatus.Disconnected || ServerInc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                        {
                            foreach (Player p in GameState)
                            {
                                if (p.Connection == ServerInc.SenderConnection)
                                {
                                    GameState.Remove(p);
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
            if ((ClientInc = client.ReadMessage()) != null)
            {
                switch (ClientInc.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        byte b = ClientInc.ReadByte();
                        if (b == (byte)PacketTypes.ADDPLAYER)
                        {
                            Player temp = new Player();
                            ClientInc.ReadAllProperties(temp);
                            temp.LoadContent();
                            PlayerList.Add(temp);
                        }
                        else if (b == (byte)PacketTypes.WORLDSTATE)
                        {
                            int count = 0;
                            count = ClientInc.ReadInt32();
                            foreach (Player p in PlayerList)
                            {
                                ClientInc.ReadAllProperties(p);
                            }
                        }
                        else if (b == (byte)PacketTypes.JOINED)
                        {
                            PlayerList.Clear();
                            int count = ClientInc.ReadInt32();
                            for (int i = 0; i < count; i ++)
                            {
                                Player temp = new Player();
                                ClientInc.ReadAllProperties(temp);
                                temp.LoadContent();
                                PlayerList.Add(temp);
                            }
                            PlayerID = count + 1;
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        if (ClientInc.SenderConnection.Status == NetConnectionStatus.Disconnected || ClientInc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                        {
                            PlayerList.Clear();
                        }
                        break;
                }
            }
            foreach (Player p in PlayerList)
            {
                p.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Player p in PlayerList)
            {
                p.PlayerImage.Draw(spriteBatch);
            }
        }
    }
}
