﻿using System;
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
        int mapMoveX, mapMoveY, prevDir, Combo, RogueCombo, newAreaX, newAreaY;
        bool Up, Down, Left, Right, pUp, pDown, pLeft, pRight, Close, newArea, aLeft, aUp, aRight, aDown;
        public bool paused = false;
        bool releasedPause = true;
        Image text = new Image();
        Image levelUpImage;
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
            ATTACKEND,
            REMOVEMONSTER,
            NEWAREAPLAYER,
            WALLCOLLISION
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
                    newArea = false;
                    PlayerList[PlayerID].X = newAreaX;
                    PlayerList[PlayerID].Y = newAreaY;
                    foreach (MapSprite m in map.Maps)
                    {
                        m.image.UnloadContent();
                    }
                    foreach (Sprite s in map.Sprites)
                    {
                        s.image.UnloadContent();
                    }
                    XmlManager<Map> mapLoader = new XmlManager<Map>();
                    map = mapLoader.Load("Load/Gameplay/Maps/" + map.Area[X, Y] + "/Background.xml");
                    map.IsTransitioning = true;
                    map.LoadContent();
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
                    foreach (Sprite s in map.Sprites)
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
                    if (PlayerList[PlayerID].NextAttack == false)
                    {
                        if (InputManager.Instance.KeyPressed(Keys.Z))
                        {
                            PlayerList[PlayerID].NextAttack = true;
                            PlayerList[PlayerID].Combo++;
                            if (PlayerList[PlayerID].Combo > 1)
                            {
                                PlayerList[PlayerID].Combo = 0;
                            }
                        }
                    }
                }
            }
            if (map.IsTransitioning == true || paused == true || PlayerList[PlayerID].Attacking == true)
            {
                Up = Down = Left = Right = false;
            }
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
        }
        public void SendAttackState()
        {
            if (PlayerList[PlayerID].Attacking == true)
            {
                NetOutgoingMessage outmsg = client.CreateMessage();
                outmsg.Write((byte)PacketTypes.ATTACK);
                outmsg.Write(PlayerList[PlayerID].Attacking);
                outmsg.Write(PlayerList[PlayerID].NextAttack);
                outmsg.Write(PlayerList[PlayerID].Combo);
                outmsg.Write(PlayerList[PlayerID].aUp);
                outmsg.Write(PlayerList[PlayerID].aDown);
                outmsg.Write(PlayerList[PlayerID].aLeft);
                outmsg.Write(PlayerList[PlayerID].aRight);
                client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
            }
            if (PlayerList[PlayerID].Attacking == false)
            {
                NetOutgoingMessage outmsg = client.CreateMessage();
                outmsg.Write((byte)PacketTypes.ATTACKEND);
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
            levelUpImage = new Image();
            levelUpImage.Path = "Gameplay/Effects/Level Up";
            levelUpImage.Effects = "SpriteSheetEffect";
            levelUpImage.spriteSheetEffect.AmountOfFrames.X = 15;
            levelUpImage.spriteSheetEffect.AmountOfFrames.Y = 1;
            levelUpImage.IsActive = false;
            levelUpImage.LoadContent();
            newArea = false;
        }
        public void UnloadContent()
        {

        }
        public void Update(GameTime gameTime, bool paused)
        {
            if (PlayerList.Count > PlayerID)
            {
                //text = new Image();
                //text.Text = PlayerList[0].EXP.ToString();
                //text.LoadContent();
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
                                if (newArea == true)
                                {
                                    if (PlayerList.IndexOf(p) == PlayerID)
                                    {
                                        ClientInc.ReadInt32();
                                        ClientInc.ReadInt32();
                                        p.VelocityX = ClientInc.ReadInt32();
                                        p.VelocityY = ClientInc.ReadInt32();
                                        p.AreaX = ClientInc.ReadInt32();
                                        p.AreaY = ClientInc.ReadInt32();
                                        p.EXP = ClientInc.ReadInt32();
                                    }
                                }
                                else
                                {
                                    p.X = ClientInc.ReadInt32();
                                    p.Y = ClientInc.ReadInt32();
                                    p.VelocityX = ClientInc.ReadInt32();
                                    p.VelocityY = ClientInc.ReadInt32();
                                    p.AreaX = ClientInc.ReadInt32();
                                    p.AreaY = ClientInc.ReadInt32();
                                    p.EXP = ClientInc.ReadInt32();
                                }
                            }
                            foreach (Player p in PlayerList)
                            {
                                if (PlayerList.IndexOf(p) == PlayerID)
                                {
                                    ClientInc.ReadBoolean();
                                    ClientInc.ReadBoolean();
                                    ClientInc.ReadInt32();
                                }
                                else
                                {
                                    p.Attacking = ClientInc.ReadBoolean();
                                    p.NextAttack = ClientInc.ReadBoolean();
                                    p.Combo = ClientInc.ReadInt32();
                                }
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
                        else if (b == (byte)PacketTypes.NEWAREAPLAYER)
                        {
                            int id = ClientInc.ReadInt32();
                            PlayerList[id].pX = PlayerList[id].X;
                            PlayerList[id].pY = PlayerList[id].Y;
                        }
                        else if (b == (byte)PacketTypes.NEWAREA)
                        {
                            PlayerList[PlayerID].AreaX = ClientInc.ReadInt32();
                            PlayerList[PlayerID].AreaY = ClientInc.ReadInt32();
                            newAreaX = ClientInc.ReadInt32();
                            newAreaY = ClientInc.ReadInt32();
                            PlayerList[PlayerID].pX = newAreaX;
                            PlayerList[PlayerID].pY = newAreaY;
                            newArea = true;
                            map.NewArea(PlayerList[PlayerID].AreaX, PlayerList[PlayerID].AreaY);
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
                                temp.pX = temp.X;
                                temp.pY = temp.Y;
                                PlayerList.Add(temp);
                            }
                            PlayerID = count;
                        }
                        else if (b == (byte)PacketTypes.REMOVEPLAYER)
                        {
                            int toRemove = ClientInc.ReadInt32();
                            PlayerList.Remove(PlayerList[toRemove]);
                        }
                        else if (b == (byte)PacketTypes.REMOVEMONSTER)
                        {
                            int toRemove = ClientInc.ReadInt32();
                            MonsterList.Remove(MonsterList[toRemove]);
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
                        m.image.Position.X = m.OriginalPosition.X - map.DeadZone.Right + (int)ScreenManager.instance.Dimensions.X;
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
                        }
                        else if (PlayerList[PlayerID].X > PlayerList[PlayerID].pX + 10)
                        {
                            PlayerList[PlayerID].pX += 10;
                            if (PlayerList[PlayerID].pX > ScreenManager.instance.Dimensions.X / 2)
                            {
                                map.Moved.X = PlayerList[PlayerID].pX;
                                mapMoveX = (PlayerList[PlayerID].pX - (int)ScreenManager.instance.Dimensions.X / 2);
                                map.HorizontalMove();
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
                        }
                        else
                        {
                            PlayerList[PlayerID].pX -= 10;
                            if (PlayerList[PlayerID].pX < -(ScreenManager.instance.Dimensions.X / 2) + map.DeadZone.Right)
                            {
                                map.Moved.X = PlayerList[PlayerID].pX;
                                mapMoveX = (PlayerList[PlayerID].pX - (int)ScreenManager.instance.Dimensions.X / 2);
                                map.HorizontalMove();
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
                            }
                        }
                        else if (PlayerList[PlayerID].Y == PlayerList[PlayerID].pY - 10)
                        {
                            PlayerList[PlayerID].pY = PlayerList[PlayerID].Y;
                            mapMoveY = PlayerList[PlayerID].Y - (int)ScreenManager.instance.Dimensions.Y / 2;
                            map.VerticalMove();
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
                    }
                }
                PlayerList[PlayerID].PlayerImage.Position.X = PlayerList[PlayerID].PositionX;
                PlayerList[PlayerID].PlayerImage.Position.Y = PlayerList[PlayerID].PositionY;
            }
            foreach (Player p in PlayerList)
            {
                p.Update(gameTime);
                if (p.levelUp == true)
                {
                    levelUpImage.IsActive = true;
                }
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
            levelUpImage.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (text.texture != null)
            {
                text.Draw(spriteBatch);
            }
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
                        if (p.levelUp == true)
                        {
                            if (PlayerList.IndexOf(p) == PlayerID)
                            {
                                levelUpImage.Position.X = p.PositionX - ((levelUpImage.spriteSheetEffect.FrameWidth - p.PlayerImage.spriteSheetEffect.FrameWidth) / 2);
                                levelUpImage.Position.Y = p.PositionY - p.PlayerImage.spriteSheetEffect.FrameHeight;
                            }
                            else
                            {
                                levelUpImage.Position.X = p.X - ((levelUpImage.spriteSheetEffect.FrameWidth - p.PlayerImage.spriteSheetEffect.FrameWidth / 2));
                                levelUpImage.Position.Y = p.Y - p.PlayerImage.spriteSheetEffect.FrameHeight;
                            }
                            if (levelUpImage.spriteSheetEffect.CurrentFrame.X == levelUpImage.spriteSheetEffect.AmountOfFrames.X - 1)
                            {
                                p.levelUp = false;
                                levelUpImage.IsActive = false;
                            }
                            levelUpImage.Draw(spriteBatch);
                        }
                    }
                }
                PlayerList[PlayerID].PlayerImage.Draw(spriteBatch);
            }
        }
    }
}
