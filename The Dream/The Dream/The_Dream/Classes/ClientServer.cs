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
            UP,
            DOWN,
            LEFT,
            RIGHT,
            UPLEFT,
            UPRIGHT,
            DOWNLEFT,
            DOWNRIGHT,
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
            server.SendMessage(outmsg, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }
        private static void GetInputAndSendItToServer()
        {
            MoveDirection MoveDir = new MoveDirection();
            MoveDir = MoveDirection.NONE;
            if (InputManager.Instance.KeyDown(Keys.Up))
            {
                MoveDir = MoveDirection.UP;
            }
            if (InputManager.Instance.KeyDown(Keys.Down))
            {
                MoveDir = MoveDirection.DOWN;
            }
            if (InputManager.Instance.KeyDown(Keys.Left))
            {
                MoveDir = MoveDirection.LEFT;
            }
            if (InputManager.Instance.KeyDown(Keys.Right))
            {
                MoveDir = MoveDirection.RIGHT;
            }
            if (InputManager.Instance.KeyDown(Keys.Up) && InputManager.Instance.KeyDown(Keys.Left))
            {
                MoveDir = MoveDirection.UPLEFT;
            }
            if (InputManager.Instance.KeyDown(Keys.Up) && InputManager.Instance.KeyDown(Keys.Right))
            {
                MoveDir = MoveDirection.UPRIGHT;
            }
            if (InputManager.Instance.KeyDown(Keys.Down) && InputManager.Instance.KeyDown(Keys.Left))
            {
                MoveDir = MoveDirection.DOWNLEFT;
            }
            if (InputManager.Instance.KeyDown(Keys.Down) && InputManager.Instance.KeyDown(Keys.Right))
            {
                MoveDir = MoveDirection.DOWNRIGHT;
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
            hostip = "10.65.30.213";
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
                                Player temp = p;
                                temp.VelocityX = 0;
                                temp.VelocityY = 0;
                                byte b = ServerInc.ReadByte();
                                if (b == (byte)MoveDirection.UP)
                                {
                                    temp.VelocityY = -10;
                                }
                                if (b == (byte)MoveDirection.DOWN)
                                {
                                    temp.VelocityY = 10;
                                }
                                if (b == (byte)MoveDirection.LEFT)
                                {
                                    temp.VelocityX = -10;
                                }
                                if (b == (byte)MoveDirection.RIGHT)
                                {
                                    temp.VelocityX = 10;
                                }
                                if (b == (byte)MoveDirection.UPLEFT)
                                {
                                    temp.VelocityX = -10;
                                    temp.VelocityY = -10;
                                }
                                if (b == (byte)MoveDirection.UPRIGHT)
                                {
                                    temp.VelocityX = 10;
                                    temp.VelocityY = -10;
                                }
                                if (b == (byte)MoveDirection.DOWNLEFT)
                                {
                                    temp.VelocityX = -10;
                                    temp.VelocityY = 10;
                                }
                                if (b == (byte)MoveDirection.DOWNRIGHT)
                                {
                                    temp.VelocityX = 10;
                                    temp.VelocityY = 10;
                                }
                                temp.X += temp.VelocityX;
                                temp.Y += temp.VelocityY;
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
