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
        List<Monster> MonsterList;
        float ConnectionTimer;
        public Image image = new Image();
        public int PlayerID;
        public Map map;
        public bool setFade = false;
        public bool Unloaded = false;
        public bool packetSent = false;
        UpdateMonsters updateMonsters;
        int mapMoveX, mapMoveY;
        bool Up, Down, Left, Right, pUp, pDown, pLeft, pRight, Close, newArea, aLeft, aUp, aRight, aDown;
        enum PacketTypes
        {
            LOGIN,
            MOVE,
            WORLDSTATE,
            ADDPLAYER,
            REMOVEPLAYER,
            JOINED,
            NEWAREA,
            SHUTDOWN,
            ADDMONSTER,
            NEWAREAMONSTERS
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
        public void WallCheck(ref bool Up, ref bool Down, ref bool Left, ref bool Right)
        {
            Rectangle pHitBox = new Rectangle(PlayerList[PlayerID].PositionX, PlayerList[PlayerID].PositionY, 54, 64);
            foreach (MapSprite blank in map.Blanks)
            {
                if (blank.HitBox.Left < PlayerList[PlayerID].PositionX + 54 && blank.HitBox.Right > PlayerList[PlayerID].PositionX)
                {
                    blank.Column = true;
                }
                else
                {
                    blank.Column = false;
                }
                if (blank.HitBox.Top < PlayerList[PlayerID].PositionY + 64 && blank.HitBox.Bottom > PlayerList[PlayerID].PositionY)
                {
                    blank.Row = true;
                }
                else
                {
                    blank.Row = false;
                }
                if (blank.Row == true)
                {
                    if (blank.Left.Intersects(pHitBox))
                    {
                        if (Left == true)
                        {
                            Left = true;
                        }
                        else
                        {
                            Right = false;
                        }
                    }

                    if (blank.Right.Intersects(pHitBox))
                    {
                        if (Right == true)
                        {
                            Right = true;
                        }
                        else
                        {
                            Left = false;
                        }
                    }
                }
                if (blank.Column == true)
                {
                    if (blank.Down.Intersects(pHitBox))
                    {
                        if (Down == true)
                        {
                            Down = true;
                        }
                        else
                        {
                            Up = false;
                        }
                    }
                    if (blank.Up.Intersects(pHitBox))
                    {
                        if (Up == true)
                        {
                            Up = true;
                        }
                        else
                        {
                            Down = false;
                        }
                    }
                }
            }
        }
        public void AreaCheck(ref bool Up, ref bool Down, ref bool Left, ref bool Right)
        {
            if (PlayerList[PlayerID].Y > map.DeadZone.Bottom - 64)
            {
                if (map.Area[PlayerList[PlayerID].AreaX, PlayerList[PlayerID].AreaY + 1] != null)
                {
                    PlayerList[PlayerID].AreaY++;
                    aDown = true;
                    newArea = true;
                }
                else
                {
                    if (Up == true)
                    {
                        Up = true;
                    }
                    else
                    {
                        Down = false;
                    }
                }
            }
            if (PlayerList[PlayerID].Y < map.DeadZone.Top)
            {
                if (map.Area[PlayerList[PlayerID].AreaX, PlayerList[PlayerID].AreaY - 1] != null)
                {
                    PlayerList[PlayerID].AreaY--;
                    aUp = true;
                    newArea = true;
                }
                else
                {
                    if (Down == true)
                    {
                        Down = true;
                    }
                    else
                    {
                        Up = false;
                    }
                }
            }
            if (PlayerList[PlayerID].X > map.DeadZone.Right - 54)
            {
                if (map.Area[PlayerList[PlayerID].AreaX + 1, PlayerList[PlayerID].AreaY] != null)
                {
                    PlayerList[PlayerID].AreaX++;
                    aRight = true;
                    newArea = true;
                }
                else
                {
                    if (Left == true)
                    {
                        Left = true;
                    }
                    else
                    {
                        Right = false;
                    }
                }
            }
            if (PlayerList[PlayerID].X < map.DeadZone.Left)
            {
                if (map.Area[PlayerList[PlayerID].AreaX - 1, PlayerList[PlayerID].AreaY] != null)
                {
                    PlayerList[PlayerID].AreaX--;
                    aLeft = true;
                    newArea = true;
                }
                else
                {
                    if (Right == true)
                    {
                        Right = true;
                    }
                    else
                    {
                        Left = false;
                    }
                }
            }
        }
        public void AreaTransition(GameTime gameTime, int X, int Y, ref Map map, ref Image fadeImage)
        {
            if (map.IsTransitioning == true)
            {
                if (setFade == false)
                {
                    fadeImage.IsActive = true;
                    fadeImage.fadeEffect.Increase = true;
                    setFade = true;
                }
                fadeImage.Update(gameTime);
                if (fadeImage.Alpha == 1.0f)
                {
                    foreach (MapSprite m in map.Maps)
                    {
                        m.image.UnloadContent();
                    }
                    XmlManager<Map> mapLoader = new XmlManager<Map>();
                    map = mapLoader.Load("Load/Gameplay/Maps/" + map.Area[X, Y] + "/Background.xml");
                    map.IsTransitioning = true;
                    map.LoadContent();
                    NetOutgoingMessage outmsg = client.CreateMessage();
                    outmsg.Write((byte)PacketTypes.NEWAREA);
                    outmsg.Write(PlayerList[PlayerID].AreaX);
                    outmsg.Write(PlayerList[PlayerID].AreaY);
                    outmsg.Write(aUp);
                    outmsg.Write(aDown);
                    outmsg.Write(aLeft);
                    outmsg.Write(aRight);
                    outmsg.Write(map.DeadZone.X);
                    outmsg.Write(map.DeadZone.Y);
                    outmsg.Write(map.DeadZone.Width);
                    outmsg.Write(map.DeadZone.Height);
                    client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
                    if (aUp == true)
                    {
                        PlayerList[PlayerID].Y = map.DeadZone.Bottom - 64;
                    }
                    if (aDown == true)
                    {
                        PlayerList[PlayerID].Y = map.DeadZone.Top;
                    }
                    if (aLeft == true)
                    {
                        PlayerList[PlayerID].X = map.DeadZone.Right - 54;
                    }
                    if (aRight == true)
                    {
                        PlayerList[PlayerID].X = map.DeadZone.Left;
                    }
                    map.Update(gameTime, PlayerList[PlayerID]);
                    foreach (MapSprite m in map.Maps)
                    {
                        if (map.Left == false && map.Right == false)
                        {
                            m.image.Position.X = m.OriginalPosition.X - map.Moved.X + ScreenManager.instance.Dimensions.X / 2;
                        }
                        else if (map.Right == true)
                        {
                            m.image.Position.X = m.OriginalPosition.X - map.DeadZone.Width + ScreenManager.instance.Dimensions.X;
                        }
                        if (map.Down == false && map.Up == false)
                        {
                            m.image.Position.Y = m.OriginalPosition.Y - map.Moved.Y + ScreenManager.instance.Dimensions.Y / 2;
                        }
                        else if (map.Down == true)
                        {
                            m.image.Position.Y = m.OriginalPosition.Y - map.DeadZone.Height + ScreenManager.instance.Dimensions.Y;
                        }
                    }
                    foreach (MapSprite b in map.Blanks)
                    {
                        b.image.Position.X = b.OriginalPosition.X - map.Moved.X;
                        b.image.Position.Y = b.OriginalPosition.Y - map.Moved.Y;
                    }
                }
                else if (fadeImage.Alpha == 0.0f)
                {
                    fadeImage.IsActive = false;
                    map.IsTransitioning = false;
                    setFade = false;
                }
            }
            else
            {
                aUp = aDown = aLeft = aRight = false;
            }
        }
        public void GetInput()
        {
            MoveDirection dir = new MoveDirection();
            dir = MoveDirection.MOVE;
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
            WallCheck(ref Up, ref Down, ref Left, ref Right);
            AreaCheck(ref Up, ref Down, ref Left, ref Right);
            if (map.IsTransitioning == true)
            {
                Up = Down = Left = Right = false;
            }
            if (pUp == Up && pDown == Down && pLeft == Left && pRight == Right)
            {
                dir = MoveDirection.NONE;
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
            if (dir == MoveDirection.MOVE)
            {
                NetOutgoingMessage outmsg = client.CreateMessage();
                outmsg.Write((byte)PacketTypes.MOVE);
                outmsg.Write(Up);
                outmsg.Write(Down);
                outmsg.Write(Left);
                outmsg.Write(Right);
                outmsg.Write(PlayerList[PlayerID].PositionX);
                outmsg.Write(PlayerList[PlayerID].PositionY);
                client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
            }
            if (newArea == true)
            {
                map.NewArea(PlayerList[PlayerID].AreaX, PlayerList[PlayerID].AreaY);
                newArea = false;
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
            MonsterList = new List<Monster>();
        }
        public void UnloadContent()
        {

        }
        public void Update(GameTime gameTime)
        {
            if (PlayerList.Count > PlayerID)
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
                            foreach (Player p in PlayerList)
                            {
                                p.X = ClientInc.ReadInt32();
                                p.Y = ClientInc.ReadInt32();
                                p.VelocityX = ClientInc.ReadInt32();
                                p.VelocityY = ClientInc.ReadInt32();
                                p.AreaX = ClientInc.ReadInt32();
                                p.AreaY = ClientInc.ReadInt32();
                            }
                            foreach (Monster m in MonsterList)
                            {
                                m.X = ClientInc.ReadInt32();
                                m.Y = ClientInc.ReadInt32();
                            }
                        }
                        else if (b == (byte)PacketTypes.ADDMONSTER)
                        {
                            Monster temp = new Monster();
                            temp.X = ClientInc.ReadInt32();
                            temp.Y = ClientInc.ReadInt32();
                            temp.image.Path = ClientInc.ReadString();
                            temp.image.spriteSheetEffect.AmountOfFrames.X = ClientInc.ReadFloat();
                            temp.image.spriteSheetEffect.AmountOfFrames.Y = ClientInc.ReadFloat();
                            temp.LoadContent();
                            MonsterList.Add(temp);
                        }
                        else if (b == (byte)PacketTypes.NEWAREAMONSTERS)
                        {
                            MonsterList.Clear();
                            int count = ClientInc.ReadInt32();
                            for (int i = 0; i < count; i++)
                            {
                                Monster temp = new Monster();
                                temp.X = ClientInc.ReadInt32();
                                temp.Y = ClientInc.ReadInt32();
                                temp.image.Path = ClientInc.ReadString();
                                temp.image.spriteSheetEffect.AmountOfFrames.X = ClientInc.ReadFloat();
                                temp.image.spriteSheetEffect.AmountOfFrames.Y = ClientInc.ReadFloat();
                                temp.LoadContent();
                                MonsterList.Add(temp);
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
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        if (ClientInc.SenderConnection.Status == NetConnectionStatus.Disconnected || ClientInc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                        {
                            PlayerList.Clear();
                        }
                        break;
                }
            }
            if (PlayerList.Count > PlayerID)
            {
                map.Update(gameTime, PlayerList[PlayerID]);
                if (map.Left == true)
                {
                    PlayerList[PlayerID].PositionX = PlayerList[PlayerID].X;
                    mapMoveX = 0;
                }
                if (map.Right == true)
                {
                    PlayerList[PlayerID].PositionX = PlayerList[PlayerID].X - map.DeadZone.Right + (int)ScreenManager.instance.Dimensions.X;
                    mapMoveX = map.DeadZone.Right;
                }
                if (map.Down == true)
                {
                    PlayerList[PlayerID].PositionY = PlayerList[PlayerID].Y - map.DeadZone.Bottom + (int)ScreenManager.instance.Dimensions.Y;
                }
                if (map.Up == true)
                {
                    PlayerList[PlayerID].PositionY = PlayerList[PlayerID].Y;
                    mapMoveY = 0;
                }
                if (map.Left == false && map.Right == false)
                {
                    map.HorizontalMove();
                    mapMoveX = (PlayerList[PlayerID].X - (int)ScreenManager.instance.Dimensions.X / 2) + 10;
                }
                if (map.Up == false && map.Down == false)
                {
                    map.VerticalMove();
                    mapMoveY = (PlayerList[PlayerID].Y - (int)ScreenManager.instance.Dimensions.Y / 2) + 10;
                }
                PlayerList[PlayerID].PlayerImage.Position.X = PlayerList[PlayerID].PositionX;
                PlayerList[PlayerID].PlayerImage.Position.Y = PlayerList[PlayerID].PositionY;
            }
            foreach (Player p in PlayerList)
            {
                p.Update(gameTime);
            }
            foreach (Player p in PlayerList)
            {
                if (PlayerList.IndexOf(p) != PlayerID)
                {
                    p.PlayerImage.Position.X = p.X - mapMoveX;
                    p.PlayerImage.Position.Y = p.Y - mapMoveY;
                }
            }
            foreach (Monster m in MonsterList)
            {
                m.image.Position.X = m.X - mapMoveX;
                m.image.Position.Y = m.Y - mapMoveY;
                m.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (PlayerList.Count > PlayerID)
            {
                foreach (Monster m in MonsterList)
                {
                    m.Draw(spriteBatch);
                }
                foreach (Player p in PlayerList)
                {
                    if (PlayerList[PlayerID].AreaX == p.AreaX && PlayerList[PlayerID].AreaY == p.AreaY)
                    {
                        p.PlayerImage.Draw(spriteBatch);
                    }
                }
                PlayerList[PlayerID].PlayerImage.Draw(spriteBatch);
            }
        }
    }
}
