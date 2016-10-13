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
        public Textures textures;
        public bool setFade = false;
        public bool Unloaded = false;
        public bool packetSent = false;
        UpdateMonsters updateMonsters;
        int mapMoveX, mapMoveY, prevDir, Combo, RogueCombo;
        bool Up, Down, Left, Right, pUp, pDown, pLeft, pRight, Close, newArea, aLeft, aUp, aRight, aDown, SentAttackState;
        public bool paused = false;
        bool releasedPause = true;
        Image ping = new Image();
        Image xy = new Image();
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
            NEWAREAMONSTERS,
            ATTACK,
            NEXTATTACK,
            ATTACKEND
        }
        enum MoveDirection
        {
            MOVE,
            NONE
        }
        public void GetReferences(Map realMap, Textures realTextures)
        {
            map = realMap;
            textures = realTextures;
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
            foreach (Sprite sprite in textures.Sprites)
            {
                if (sprite.HitBox.Left < PlayerList[PlayerID].PositionX + 54 && sprite.HitBox.Right > PlayerList[PlayerID].PositionX)
                {
                    sprite.Column = true;
                }
                else
                {
                    sprite.Column = false;
                }
                if (sprite.HitBox.Top < PlayerList[PlayerID].PositionY + 64 && sprite.HitBox.Bottom > PlayerList[PlayerID].PositionY)
                {
                    sprite.Row = true;
                }
                else
                {
                    sprite.Row = false;
                }
                if (sprite.Row == true)
                {
                    if (sprite.Left.Intersects(pHitBox))
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

                    if (sprite.Right.Intersects(pHitBox))
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
                if (sprite.Column == true)
                {
                    if (sprite.Down.Intersects(pHitBox))
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
                    if (sprite.Up.Intersects(pHitBox))
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
                    foreach (Sprite s in textures.Sprites)
                    {
                        s.image.UnloadContent();
                    }
                    XmlManager<Map> mapLoader = new XmlManager<Map>();
                    map = mapLoader.Load("Load/Gameplay/Maps/" + map.Area[X, Y] + "/Background.xml");
                    map.IsTransitioning = true;
                    map.LoadContent();
                    XmlManager<Textures> textureLoader = new XmlManager<Textures>();
                    textures = textureLoader.Load("Load/Gameplay/Maps/" + map.Area[X, Y] + "/Textures.xml");
                    textures.LoadContent();
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
                    outmsg.Write(map.Maps.Count);
                    client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
                    if (aUp == true)
                    {
                        PlayerList[PlayerID].Y = map.DeadZone.Bottom - 64;
                        PlayerList[PlayerID].pY = PlayerList[PlayerID].Y;
                    }
                    if (aDown == true)
                    {
                        PlayerList[PlayerID].Y = map.DeadZone.Top;
                        PlayerList[PlayerID].pY = PlayerList[PlayerID].Y;
                    }
                    if (aLeft == true)
                    {
                        PlayerList[PlayerID].X = map.DeadZone.Right - 54;
                        PlayerList[PlayerID].pX = PlayerList[PlayerID].X;
                    }
                    if (aRight == true)
                    {
                        PlayerList[PlayerID].X = map.DeadZone.Left;
                        PlayerList[PlayerID].pX = PlayerList[PlayerID].X;
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
                    foreach (Sprite s in textures.Sprites)
                    {
                        if (map.Left == false && map.Right == false)
                        {
                            s.image.Position.X = s.OriginalPosition.X - map.Moved.X + ScreenManager.instance.Dimensions.X / 2;
                        }
                        else if (map.Right == true)
                        {
                            s.image.Position.X = s.OriginalPosition.X - map.DeadZone.Width + ScreenManager.instance.Dimensions.X;
                        }
                        if (map.Down == false && map.Up == false)
                        {
                            s.image.Position.Y = s.OriginalPosition.Y - map.Moved.Y + ScreenManager.instance.Dimensions.Y / 2;
                        }
                        else if (map.Down == true)
                        {
                            s.image.Position.Y = s.OriginalPosition.Y - map.DeadZone.Height + ScreenManager.instance.Dimensions.Y;
                        }
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
        public void GetInput(bool paused)
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
            if (InputManager.Instance.KeyPressed(Keys.W))
            {
                Up = true;
            }
            if (InputManager.Instance.KeyPressed(Keys.S))
            {
                Down = true;
            }            
            if (InputManager.Instance.KeyPressed(Keys.A))
            {
                Left = true;
            }
            if (InputManager.Instance.KeyPressed(Keys.D))
            {
                Right = true;
            }
            if (InputManager.Instance.KeyPressed(Keys.Z))
            {
                if (PlayerList[PlayerID].Attacking == false)
                {
                    PlayerList[PlayerID].Attacking = true;
                    PlayerList[PlayerID].zPressed = true;
                }
            }
            if (PlayerList[PlayerID].Attacking == true)
            {
                if (PlayerList[PlayerID].PlayerImage.spriteSheetEffect.CurrentFrame.X >= PlayerList[PlayerID].PlayerImage.spriteSheetEffect.AmountOfFrames.X - 3)
                {
                    if (InputManager.Instance.KeyPressed(Keys.Z))
                    {
                        PlayerList[PlayerID].NextAttack = true;
                    }
                }
                if (PlayerList[PlayerID].PlayerImage.spriteSheetEffect.CurrentFrame.X == PlayerList[PlayerID].PlayerImage.spriteSheetEffect.AmountOfFrames.X - 1)
                {
                    if (PlayerList[PlayerID].NextAttack == true)
                    {
                        PlayerList[PlayerID].Combo++;
                        if (PlayerList[PlayerID].Combo > RogueCombo)
                        {
                            PlayerList[PlayerID].Combo = 0;
                        }
                    }
                    else
                    {
                        PlayerList[PlayerID].Attacking = false;
                        PlayerList[PlayerID].PlayerImage.spriteSheetEffect.CurrentFrame.Y = prevDir;
                    }
                }
            }
            WallCheck(ref Up, ref Down, ref Left, ref Right);
            AreaCheck(ref Up, ref Down, ref Left, ref Right);
            if (map.IsTransitioning == true || paused == true || PlayerList[PlayerID].Attacking == true)
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
        public void SendAttackState()
        {
            //if (PlayerList[PlayerID].Attacking == true)
            //{
            //    if (SentAttackState == false)
            //    {
                    //NetOutgoingMessage outmsg = client.CreateMessage();
                    //outmsg.Write((byte)PacketTypes.ATTACK);
                    //outmsg.Write(PlayerList[PlayerID].Attacking);
                    //client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
            //    }
            //}
            //else
            //{
            //    if (SentAttackState == true)
            //    {
            //        NetOutgoingMessage outmsg = client.CreateMessage();
            //        outmsg.Write((byte)PacketTypes.ATTACKEND);
            //        client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
            //    }
            //}
        }
        public Client()
        {

        }
        public void LoadContent()
        {
            NetPeerConfiguration ClientConfig = new NetPeerConfiguration("game");
            client = new NetClient(ClientConfig);
            ClientConfig.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
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
            Combo = 0;
            RogueCombo = 1;
            SentAttackState = false;
        }
        public void UnloadContent()
        {

        }
        public void Update(GameTime gameTime, bool paused)
        {
            if (PlayerList.Count > PlayerID)
            {
                GetInput(paused);
                SendAttackState();
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
                                //p.Attacking = ClientInc.ReadBoolean();
                                //p.NextAttack = ClientInc.ReadBoolean();
                                //p.Combo = ClientInc.ReadInt32();
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
                            for (int i = 0; i < count; i++)
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
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        break;
                }
            }
            if (PlayerList.Count > PlayerID)
            {
                map.Update(gameTime, PlayerList[PlayerID]);
                textures.Update(gameTime, PlayerList[PlayerID]);
                if (map.Left == true)
                {
                    if (PlayerList[PlayerID].X != PlayerList[PlayerID].pX)
                    {
                        if (PlayerList[PlayerID].X == PlayerList[PlayerID].pX + 10)
                        {
                            PlayerList[PlayerID].PositionX = PlayerList[PlayerID].X;
                            PlayerList[PlayerID].pX = PlayerList[PlayerID].X;
                        }
                        else if (PlayerList[PlayerID].X > PlayerList[PlayerID].pX + 10)
                        {
                            PlayerList[PlayerID].PositionX += 10;
                            PlayerList[PlayerID].pX += 10;
                        }
                        else if (PlayerList[PlayerID].X == PlayerList[PlayerID].pX - 10)
                        {
                            PlayerList[PlayerID].PositionX = PlayerList[PlayerID].X;
                            PlayerList[PlayerID].pX = PlayerList[PlayerID].X;
                        }
                        else
                        {
                            PlayerList[PlayerID].PositionX -= 10;
                            PlayerList[PlayerID].pX -= 10;
                        }
                    }
                    foreach (MapSprite m in map.Maps)
                    {
                        m.image.Position.X = m.OriginalPosition.X;
                    }
                    mapMoveX = 0;
                }
                if (map.Right == true)
                {
                    if (PlayerList[PlayerID].X != PlayerList[PlayerID].pX)
                    {
                        if (PlayerList[PlayerID].X == PlayerList[PlayerID].pX + 10)
                        {
                            PlayerList[PlayerID].PositionX = PlayerList[PlayerID].X - map.DeadZone.Right + (int)ScreenManager.instance.Dimensions.X;
                            PlayerList[PlayerID].pX = PlayerList[PlayerID].X;
                        }
                        else if (PlayerList[PlayerID].X > PlayerList[PlayerID].pX + 10)
                        {
                            PlayerList[PlayerID].PositionX += 10;
                            PlayerList[PlayerID].pX += 10;
                        }
                        else if (PlayerList[PlayerID].X == PlayerList[PlayerID].pX - 10)
                        {
                            PlayerList[PlayerID].PositionX = PlayerList[PlayerID].X - map.DeadZone.Right + (int)ScreenManager.instance.Dimensions.X;
                            PlayerList[PlayerID].pX = PlayerList[PlayerID].X;
                        }
                        else
                        {
                            PlayerList[PlayerID].PositionX -= 10;
                            PlayerList[PlayerID].pX -= 10;
                        }
                    }
                    foreach (MapSprite m in map.Maps)
                    {
                        m.image.Position.X = m.OriginalPosition.X + map.DeadZone.Right - (int)ScreenManager.instance.Dimensions.X;
                    }
                    mapMoveX = map.DeadZone.Right - (int)ScreenManager.instance.Dimensions.X;
                }
                if (map.Down == true)
                {
                    if (PlayerList[PlayerID].Y != PlayerList[PlayerID].pY)
                    {
                        if (PlayerList[PlayerID].Y == PlayerList[PlayerID].pY + 10)
                        {
                            PlayerList[PlayerID].PositionY = PlayerList[PlayerID].Y - map.DeadZone.Bottom + (int)ScreenManager.instance.Dimensions.Y;
                            PlayerList[PlayerID].pY = PlayerList[PlayerID].Y;
                        }
                        else if (PlayerList[PlayerID].Y > PlayerList[PlayerID].pY + 10)
                        {
                            PlayerList[PlayerID].PositionY += 10;
                            PlayerList[PlayerID].pY += 10;
                        }
                        else if (PlayerList[PlayerID].Y == PlayerList[PlayerID].pY - 10)
                        {
                            PlayerList[PlayerID].PositionY = PlayerList[PlayerID].Y - map.DeadZone.Bottom + (int)ScreenManager.instance.Dimensions.Y;
                            PlayerList[PlayerID].pY = PlayerList[PlayerID].Y;
                        }
                        else
                        {
                            PlayerList[PlayerID].PositionY -= 10;
                            PlayerList[PlayerID].pY -= 10;
                        }
                    }
                    foreach (MapSprite m in map.Maps)
                    {
                        m.image.Position.Y = m.OriginalPosition.Y - map.DeadZone.Bottom + (int)ScreenManager.instance.Dimensions.Y;
                    }
                    mapMoveY = map.DeadZone.Bottom - (int)ScreenManager.instance.Dimensions.Y;
                }
                if (map.Up == true)
                {
                    if (PlayerList[PlayerID].Y != PlayerList[PlayerID].pY)
                    {
                        if (PlayerList[PlayerID].Y == PlayerList[PlayerID].pY + 10)
                        {
                            PlayerList[PlayerID].PositionY = PlayerList[PlayerID].Y;
                            PlayerList[PlayerID].pY = PlayerList[PlayerID].Y;
                        }
                        else if (PlayerList[PlayerID].Y > PlayerList[PlayerID].pY + 10)
                        {
                            PlayerList[PlayerID].PositionY += 10;
                            PlayerList[PlayerID].pY += 10;
                        }
                        else if (PlayerList[PlayerID].Y == PlayerList[PlayerID].pY - 10)
                        {
                            PlayerList[PlayerID].PositionY = PlayerList[PlayerID].Y;
                            PlayerList[PlayerID].pY = PlayerList[PlayerID].Y;
                        }
                        else
                        {
                            PlayerList[PlayerID].PositionY -= 10;
                            PlayerList[PlayerID].pY -= 10;
                        }
                    }
                    foreach (MapSprite m in map.Maps)
                    {
                        m.image.Position.Y = m.OriginalPosition.Y;
                    }
                    mapMoveY = 0;
                }
                if (map.Left == false && map.Right == false)
                {
                    if (PlayerList[PlayerID].X != PlayerList[PlayerID].pX)
                    {
                        if (PlayerList[PlayerID].X == PlayerList[PlayerID].pX + 10)
                        {
                            PlayerList[PlayerID].pX = PlayerList[PlayerID].X;
                            mapMoveX = (PlayerList[PlayerID].X - (int)ScreenManager.instance.Dimensions.X / 2);
                            map.HorizontalMove();
                            textures.HorizontalMove();
                        }
                        else if (PlayerList[PlayerID].X > PlayerList[PlayerID].pX + 10)
                        {
                            PlayerList[PlayerID].pX += 10;
                            if (PlayerList[PlayerID].pX > ScreenManager.instance.Dimensions.X / 2)
                            {
                                map.Moved.X = PlayerList[PlayerID].pX;
                                mapMoveX = (PlayerList[PlayerID].pX - (int)ScreenManager.instance.Dimensions.X / 2);
                                map.HorizontalMove();
                                textures.HorizontalMove();
                            }
                            else
                            {
                                PlayerList[PlayerID].PositionX += 10;
                            }
                        }
                        else if (PlayerList[PlayerID].X == PlayerList[PlayerID].pX - 10)
                        {
                            PlayerList[PlayerID].pX = PlayerList[PlayerID].X;
                            mapMoveX = (PlayerList[PlayerID].X - (int)ScreenManager.instance.Dimensions.X / 2);
                            map.HorizontalMove();
                            textures.HorizontalMove();
                        }
                        else
                        {
                            PlayerList[PlayerID].pX -= 10;
                            if (PlayerList[PlayerID].pX < -(ScreenManager.instance.Dimensions.X / 2) + map.DeadZone.Right)
                            {
                                map.Moved.X = PlayerList[PlayerID].pX;
                                mapMoveX = (PlayerList[PlayerID].pX - (int)ScreenManager.instance.Dimensions.X / 2);
                                map.HorizontalMove();
                                textures.HorizontalMove();
                            }
                            else
                            {
                                PlayerList[PlayerID].PositionX -= 10;
                            }
                        }
                    }
                    if (PlayerList[PlayerID].pX > ScreenManager.instance.Dimensions.X / 2)
                    {
                        PlayerList[PlayerID].PositionX = (int)ScreenManager.instance.Dimensions.X / 2;
                    }
                    else
                    {
                        mapMoveX = (PlayerList[PlayerID].X - (int)ScreenManager.instance.Dimensions.X / 2);
                        map.HorizontalMove();
                        textures.HorizontalMove();
                    }
                }
                if (map.Up == false && map.Down == false)
                {
                    if (PlayerList[PlayerID].Y != PlayerList[PlayerID].pY)
                    {
                        if (PlayerList[PlayerID].Y == PlayerList[PlayerID].pY + 10)
                        {
                            PlayerList[PlayerID].pY = PlayerList[PlayerID].Y;
                            mapMoveY = PlayerList[PlayerID].Y - (int)ScreenManager.instance.Dimensions.Y / 2;
                            map.VerticalMove();
                            textures.VerticalMove();
                        }
                        else if (PlayerList[PlayerID].Y > PlayerList[PlayerID].pY + 10)
                        {
                            PlayerList[PlayerID].pY += 10;
                            if (PlayerList[PlayerID].pY < ScreenManager.instance.Dimensions.Y / 2)
                            {
                                PlayerList[PlayerID].PositionY += 10;
                            }
                            else
                            {
                                map.Moved.Y = PlayerList[PlayerID].pY;
                                mapMoveY = (PlayerList[PlayerID].pY - (int)ScreenManager.instance.Dimensions.Y / 2);
                                map.VerticalMove();
                                textures.VerticalMove();
                            }
                        }
                        else if (PlayerList[PlayerID].Y == PlayerList[PlayerID].pY - 10)
                        {
                            PlayerList[PlayerID].pY = PlayerList[PlayerID].Y;
                            mapMoveY = PlayerList[PlayerID].Y - (int)ScreenManager.instance.Dimensions.Y / 2;
                            map.VerticalMove();
                            textures.VerticalMove();
                        }
                        else
                        {
                            PlayerList[PlayerID].pY -= 10;
                            if (PlayerList[PlayerID].pY > (map.DeadZone.Bottom - ScreenManager.instance.Dimensions.Y / 2))
                            {
                                PlayerList[PlayerID].PositionY -= 10;
                            }
                            else
                            {
                                map.Moved.Y = PlayerList[PlayerID].pY;
                                mapMoveY = (PlayerList[PlayerID].pY - (int)ScreenManager.instance.Dimensions.Y / 2);
                                map.VerticalMove();
                                textures.VerticalMove();
                            }
                        }
                    }
                    if (PlayerList[PlayerID].pY > ScreenManager.instance.Dimensions.Y / 2)
                    {
                        PlayerList[PlayerID].PositionY = (int)ScreenManager.instance.Dimensions.Y / 2;
                    }
                    else
                    {
                        mapMoveY = PlayerList[PlayerID].Y - (int)ScreenManager.instance.Dimensions.Y / 2;
                        map.VerticalMove();
                        textures.VerticalMove();
                    }
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
                    if (p.X != p.pX)
                    {
                        if (p.X == p.pX + 10)
                        {
                            p.PositionX = p.X;
                            p.pX = p.X;
                        }
                        else if (p.X > p.pX + 10)
                        {
                            p.pX += 10;
                            p.PositionX += 10;
                        }
                        else if (p.X == p.pX - 10)
                        {
                            p.PositionX = p.X;
                            p.pX = p.X;
                        }
                        else
                        {
                            p.pX -= 10;
                            p.PositionX -= 10;
                        }
                    }
                    else
                    {
                        p.PositionX = p.X;
                    }
                    if (p.Y != p.pY)
                    {
                        if (p.Y == p.pY + 10)
                        {
                            p.PositionY = p.Y;
                            p.pY = p.Y;
                        }
                        else if (p.Y > p.pY + 10)
                        {
                            p.pY += 10;
                            p.PositionY += 10;
                        }
                        else if (p.Y == p.pY - 10)
                        {
                            p.PositionY = p.Y;
                            p.pY = p.Y;
                        }
                        else
                        {
                            p.pY -= 10;
                            p.PositionY -= 10;
                        }
                    }
                    else
                    {
                        p.PositionY = p.Y;
                    }
                    p.PlayerImage.Position.X = p.PositionX - mapMoveX;
                    p.PlayerImage.Position.Y = p.PositionY - mapMoveY;
                }
            }
            foreach (Monster m in MonsterList)
            {
                m.image.Update(gameTime);
                m.image.Position.X = m.X - mapMoveX;
                m.image.Position.Y = m.Y - mapMoveY;
                //    if (m.X != m.pX)
                //    {
                //        if (m.X == m.pX + 10)
                //        {
                //            m.image.Position.X = m.X - mapMoveX;
                //            m.pX = m.X;
                //        }
                //        else if (m.X > m.pX + 10)
                //        {
                //            m.pX += 10;
                //            m.image.Position.X = m.pX - mapMoveX;
                //        }
                //        else if (m.X == m.pX - 10)
                //        {
                //            m.image.Position.X = m.X - mapMoveX;
                //            m.pX = m.X;
                //        }
                //        else
                //        {
                //            m.pX -= 10;
                //            m.image.Position.X = m.pX - mapMoveX;
                //        }
                //    }
                //    if (m.Y != m.pY)
                //    {
                //        if (m.Y == m.pY + 10)
                //        {
                //            m.image.Position.Y = m.Y - mapMoveY;
                //            m.pY = m.Y;
                //        }
                //        else if (m.Y > m.pY + 10)
                //        {
                //            m.pY += 10;
                //            m.image.Position.Y = m.pY - mapMoveY;
                //        }
                //        else if (m.Y == m.pY - 10)
                //        {
                //            m.image.Position.Y = m.Y - mapMoveY;
                //            m.pY = m.Y;
                //        }
                //        else
                //        {
                //            m.pY -= 10;
                //            m.image.Position.Y = m.pY - mapMoveY;
                //        }
                //    }
                //    m.image.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (PlayerList.Count > 1)
            {
                PlayerList[1].PlayerImage.Draw(spriteBatch);
            }
            if (xy.texture != null)
            {
                xy.Draw(spriteBatch);
            }
            if (PlayerList.Count > PlayerID)
            {
                foreach (Monster m in MonsterList)
                {
                    if (m.AreaX == PlayerList[PlayerID].AreaX && m.AreaY == PlayerList[PlayerID].AreaY)
                    {
                        m.Draw(spriteBatch);
                    }
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
