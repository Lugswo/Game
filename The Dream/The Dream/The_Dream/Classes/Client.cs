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
    public class Client
    {
        public string hostip;
        public bool host;
        NetIncomingMessage ClientInc;
        static NetClient client;
        NetOutgoingMessage ClientOut;
        public List<Player> PlayerList;
        float ConnectionTimer;
        public Image image = new Image();
        public int PlayerID;
        public Map map;
        int mapMoveX, mapMoveY;
        bool Up, Down, Left, Right, pUp, pDown, pLeft, pRight, Close;
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
        enum MoveDirection
        {
            MOVE,
            NONE
        }
        public void GetReferences(Map realMap)
        {
            map = realMap;
        }
        public void GetInput()
        {
            MoveDirection MoveDir = new MoveDirection();
            MoveDir = MoveDirection.MOVE;
            if (InputManager.Instance.KeyDown(Keys.Down) && InputManager.Instance.KeyDown(Keys.Up))
            {
                Up = false;
                Down = false;
            }
            else if (InputManager.Instance.KeyDown(Keys.Down))
            {
                Down = true;
            }
            else if (InputManager.Instance.KeyDown(Keys.Up))
            {
                Up = true;
            }
            else
            {
                Up = false;
                Down = false;
            }
            if (InputManager.Instance.KeyDown(Keys.Right) && InputManager.Instance.KeyDown(Keys.Left))
            {
                Left = false;
                Right = false;
            }
            else if (InputManager.Instance.KeyDown(Keys.Right))
            {
                Right = true;
            }
            else if (InputManager.Instance.KeyDown(Keys.Left))
            {
                Left = true;
            }
            else
            {
                Left = false;
                Right = false;
            }
            if (Up == pUp && Down == pDown && Left == pLeft && Right == pRight)
            {
                MoveDir = MoveDirection.NONE;
            }
            pUp = Up;
            pDown = Down;
            pLeft = Left;
            pRight = Right;
            if (InputManager.Instance.KeyDown(Keys.Q))
            {
                if (host == true)
                {
                    NetOutgoingMessage outmsg2 = client.CreateMessage();
                    outmsg2.Write((byte)PacketTypes.SHUTDOWN);
                    client.SendMessage(outmsg2, NetDeliveryMethod.ReliableOrdered, 0);
                }
                client.Disconnect("bye bye");
                if (ScreenManager.Instance.IsTransitioning == false)
                {
                    ScreenManager.Instance.ChangeScreens("TitleScreen");
                }
            }
            if (MoveDir == MoveDirection.MOVE)
            {
                NetOutgoingMessage outmsg = client.CreateMessage();
                outmsg.Write((byte)PacketTypes.MOVE);
                outmsg.Write(Up);
                outmsg.Write(Down);
                outmsg.Write(Left);
                outmsg.Write(Right);
                client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }
        public Client()
        {

        }
        public void LoadContent()
        {
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
            if (map.IsTransitioning == false)
            {
                GetInput();
            }
            while ((ClientInc = client.ReadMessage()) != null)
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
                                p.X = ClientInc.ReadInt32();
                                p.Y = ClientInc.ReadInt32();
                                p.VelocityX = ClientInc.ReadInt32();
                                p.VelocityY = ClientInc.ReadInt32();
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
                            PlayerID = count;
                        }
                        else if (b == (byte)PacketTypes.REMOVEPLAYER)
                        {
                            int toRemove = ClientInc.ReadInt32();
                            PlayerList.Remove(PlayerList[toRemove]);
                        }
                        else if (b == (byte)PacketTypes.NEWAREA)
                        {
                            map.NewArea(PlayerList[PlayerID].AreaX, PlayerList[PlayerID].AreaY);
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
                p.PlayerImage.Position.X = p.X - mapMoveX;
                p.PlayerImage.Position.Y = p.Y - mapMoveY;
            }
            if (PlayerList.Count > PlayerID)
            {
                map.Update(gameTime, PlayerList[PlayerID]);
                if (map.Left == true)
                {
                    PlayerList[PlayerID].PositionX = PlayerList[PlayerID].X;
                }
                if (map.Right == true)
                {
                    PlayerList[PlayerID].PositionX = PlayerList[PlayerID].X - map.DeadZone.Right + (int)ScreenManager.instance.Dimensions.X;
                }
                if (map.Down == true)
                {
                    PlayerList[PlayerID].PositionY = PlayerList[PlayerID].Y - map.DeadZone.Bottom + (int)ScreenManager.instance.Dimensions.Y;
                }
                if (map.Up == true)
                {
                    PlayerList[PlayerID].PositionY = PlayerList[PlayerID].Y;
                }
                if (map.Left == false && map.Right == false)
                {
                    map.HorizontalMove(PlayerList[PlayerID].VelocityX);
                    mapMoveX = PlayerList[PlayerID].X - (int)ScreenManager.instance.Dimensions.X / 2;
                }
                if (map.Up == false && map.Down == false)
                {
                    map.VerticalMove(PlayerList[PlayerID].VelocityY);
                    mapMoveY = PlayerList[PlayerID].Y - (int)ScreenManager.instance.Dimensions.Y / 2;
                }
                PlayerList[PlayerID].PlayerImage.Position.X = PlayerList[PlayerID].PositionX;
                PlayerList[PlayerID].PlayerImage.Position.Y = PlayerList[PlayerID].PositionY;
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
                if (PlayerList[PlayerID].AreaX == p.AreaX && PlayerList[PlayerID].AreaY == p.AreaY)
                {
                    p.PlayerImage.Draw(spriteBatch);
                }
            }
            if (PlayerList.Count > PlayerID)
            {
                PlayerList[PlayerID].PlayerImage.Draw(spriteBatch);
            }
        }
    }
}
