using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;

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
        public Image image = new Image();
        enum PacketTypes
        {
            LOGIN,
            MOVE,
            WORLDSTATE
        }
        enum MoveDirection
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            NONE
        }
        private static void GetInputAndSendItToServer()
        {
            MoveDirection MoveDir = new MoveDirection();
            MoveDir = MoveDirection.NONE;
            KeyboardState currentstate = Keyboard.GetState();
            if (currentstate.IsKeyDown(Keys.W) == true)
            {
                MoveDir = MoveDirection.UP;
            }
            if (currentstate.IsKeyDown(Keys.S) == true)
            {
                MoveDir = MoveDirection.DOWN;
            }
            if (currentstate.IsKeyDown(Keys.A) == true)
            {
                MoveDir = MoveDirection.LEFT;
            }
            if (currentstate.IsKeyDown(Keys.D) == true)
            {
                MoveDir = MoveDirection.RIGHT;
            }
            if (currentstate.IsKeyDown(Keys.Q) == true)
            {
                client.Disconnect("bye bye");
                server.Shutdown("bye bye");
                ScreenManager.Instance.ChangeScreens("TitleScreen");
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
            ServerConfig.Port = 12345;
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
            client.Connect(hostip, 12345, ClientOut);
            GameState = new List<Player>();
            PlayerList = new List<Player>();
            image.Path = "Gameplay/Characters/Sprites/Player/Player";
            image.Position = new Vector2(100, 100);
            image.LoadContent();
            ConnectionTimer = 0;
        }
        public void UnloadContent()
        {

        }
        public void Update(GameTime gameTime)
        {
            GetInputAndSendItToServer();
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
                            NetOutgoingMessage outmsg = server.CreateMessage();
                            outmsg.Write((byte)PacketTypes.WORLDSTATE);
                            outmsg.Write(GameState.Count);
                            foreach (Player p in GameState)
                            {
                                outmsg.WriteAllProperties(p);
                            }
                            server.SendMessage(outmsg, ServerInc.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
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
                                byte b = ServerInc.ReadByte();
                                if ((byte)MoveDirection.UP == b)
                                    p.Y -= 10;
                                if ((byte)MoveDirection.DOWN == b)
                                    p.Y += 10;
                                if ((byte)MoveDirection.LEFT == b)
                                    p.X -= 10;
                                if ((byte)MoveDirection.RIGHT == b)
                                    p.X += 10;
                                NetOutgoingMessage outmsg = server.CreateMessage();
                                outmsg.Write((byte)PacketTypes.WORLDSTATE);
                                outmsg.Write(GameState.Count);
                                foreach (Player ch2 in GameState)
                                {
                                    outmsg.WriteAllProperties(ch2);
                                }
                                server.SendMessage(outmsg, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
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
                                    PlayerList.Remove(p);
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
                        if (ClientInc.ReadByte() == (byte)PacketTypes.WORLDSTATE)
                        {
                            PlayerList.Clear();
                            int count = 0;
                            count = ClientInc.ReadInt32();
                            for (int i = 0; i < count; i++)
                            {
                                Player p = new Player();
                                ClientInc.ReadAllProperties(p);
                                PlayerList.Add(p);
                                PlayerList[PlayerList.Count - 1].LoadContent();
                                PlayerList[PlayerList.Count - 1].PlayerImage.Position.X = p.X;
                                PlayerList[PlayerList.Count - 1].PlayerImage.Position.Y = p.Y;
                            }
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        switch (ClientInc.SenderConnection.Status)
                        {
                            /**/
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
