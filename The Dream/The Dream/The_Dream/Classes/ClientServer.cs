using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        static List<Player> PlayerList, GameState;
        float ConnectionTimer;
        PlayerUpdate playerUpdate;
        public Image image = new Image();
        bool addedNewPlayer = true;
        enum PacketTypes
        {
            LOGIN,
            MOVE,
            WORLDSTATE,
            ADDPLAYER
        }
        enum MoveDirection
        {
            MOVE,
            NONE
        }
        private static void GetInputAndSendItToServer()
        {
            MoveDirection MoveDir = new MoveDirection();
            if (InputManager.Instance.KeyDown(Keys.Up) == true || InputManager.Instance.KeyDown(Keys.Down) == true || InputManager.Instance.KeyDown(Keys.Left) || InputManager.Instance.KeyDown(Keys.Right))
            {
                MoveDir = MoveDirection.MOVE;
            }
            if (InputManager.Instance.KeyDown(Keys.Q))
            {
                client.Disconnect("bye bye");
                server.Shutdown("bye bye");
                MoveDir = MoveDirection.NONE;
                if (ScreenManager.Instance.IsTransitioning == false)
                {
                    ScreenManager.Instance.ChangeScreens("TitleScreen");
                }
            }
            if (MoveDir != MoveDirection.NONE)
            {
                NetOutgoingMessage outmsg = client.CreateMessage();
                outmsg.Write((byte)PacketTypes.MOVE);
                outmsg.Write((byte)MoveDir);
                client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                MoveDir = MoveDirection.NONE;
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
            GetInputAndSendItToServer();
            if (PlayerList.Count == 0)
            {
                ConnectionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (ConnectionTimer >= 5)
                {
                    if (ScreenManager.Instance.IsTransitioning == false)
                    {
                        ScreenManager.Instance.ChangeScreens("TitleScreen");
                    }
                }
            }
            if (server.ConnectionsCount > 0 && addedNewPlayer == true)
            {
                NetOutgoingMessage outmsg = server.CreateMessage();
                outmsg.Write((byte)PacketTypes.WORLDSTATE);
                outmsg.Write(GameState.Count);
                foreach (Player p in GameState)
                {
                    outmsg.WriteAllProperties(p);
                }
                server.SendMessage(outmsg, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
            }
            if (addedNewPlayer == false && server.ConnectionsCount > 0)
            {
                NetOutgoingMessage outmsg = server.CreateMessage();
                Player temp = GameState[GameState.Count - 1];
                outmsg.Write((byte)PacketTypes.ADDPLAYER);
                outmsg.WriteAllProperties(temp);
                server.SendMessage(outmsg, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                addedNewPlayer = true;
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
                                Player temp = p;
                                playerUpdate.Update(gameTime, ref temp);
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
                            Player newPlayer = new Player();
                            ClientInc.ReadAllProperties(newPlayer);
                            newPlayer.LoadContent();
                            PlayerList.Add(newPlayer);
                            addedNewPlayer = true;
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
                p.PlayerImage.Position = new Vector2(p.X, p.Y);
                p.PlayerImage.Update(gameTime);
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
