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
        public Image dialogueImage;
        public int PlayerID;
        public Map map;
        public bool setFade = false;
        public bool Unloaded = false;
        public bool packetSent = false;
        UpdateMonsters updateMonsters;
        int mapMoveX, mapMoveY, prevDir, Combo, RogueCombo, newAreaX, newAreaY, nAreaX, nAreaY;
        double endLagTimer;
        bool Up, Down, Left, Right, pUp, pDown, pLeft, pRight, Close, newArea, aLeft, aUp, aRight, aDown, Talking;
        public bool paused = false;
        bool releasedPause = true;
        Image text = new Image();
        Image health;
        List<Item> itemsDropped;
        Dictionary<int, Item> Items;
        Items.TestItem testItem;
        List<Skills.Skill> skillList;
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
            LEVELUP,
            PLAYERSTATE,
            ADDITEM,
            REMOVEITEM,
            SKILL,
            REMOVESKILL,
            DEBUG
        }
        enum MoveDirection
        {
            MOVE,
            NONE
        }
        void SetItem<T>(ref T item, int ID)
        {
            if (item == null)
            {
                item = (T)Activator.CreateInstance(typeof(T));
            }
            Items.Add(ID, (item as Item));
        }
        public void RecievePacket()
        {
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
                            temp.PlayerImage.Layer = .5f;
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
                                        p.Health = ClientInc.ReadInt32();
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
                                else
                                {
                                    p.X = ClientInc.ReadInt32();
                                    p.Y = ClientInc.ReadInt32();
                                    p.VelocityX = ClientInc.ReadInt32();
                                    p.VelocityY = ClientInc.ReadInt32();
                                    p.AreaX = ClientInc.ReadInt32();
                                    p.AreaY = ClientInc.ReadInt32();
                                    p.EXP = ClientInc.ReadInt32();
                                    p.Health = ClientInc.ReadInt32();
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
                            foreach (Skills.Skill skill in skillList)
                            {
                                skill.X = ClientInc.ReadInt32();
                                skill.Y = ClientInc.ReadInt32();
                            }
                        }
                        else if (b == (byte)PacketTypes.SKILL)
                        {
                            Skills.Skill temp = new Skills.Skill();
                            temp.image.Path = ClientInc.ReadString();
                            temp.X = ClientInc.ReadInt32();
                            temp.Y = ClientInc.ReadInt32();
                            temp.pX = temp.X;
                            temp.pY = temp.Y;
                            temp.projSpeed = ClientInc.ReadInt32();
                            temp.image.LoadContent();
                            skillList.Add(temp);
                            endLagTimer += ClientInc.ReadInt32();
                        }
                        else if (b == (byte)PacketTypes.REMOVESKILL)
                        {
                            int count = ClientInc.ReadInt32();
                            for (int i = 0; i < count; i++)
                            {
                                skillList.Remove(skillList[ClientInc.ReadInt32()]);
                            }
                        }
                        else if (b == (byte)PacketTypes.LEVELUP)
                        {
                            int player = ClientInc.ReadInt32();
                            if (player != PlayerID)
                            {
                                PlayerList[player].levelUp = true;
                                PlayerList[player].levelUpImage.IsActive = true;
                            }
                        }
                        else if (b == (byte)PacketTypes.ADDMONSTER)
                        {
                            Type type;
                            type = updateMonsters.MonsterList[ClientInc.ReadInt32()].GetType();
                            Monster temp = new Monster();
                            temp = (Monster)Activator.CreateInstance(type);
                            temp.LoadContent();
                            temp.X = ClientInc.ReadInt32();
                            temp.Y = ClientInc.ReadInt32();
                            temp.pX = temp.X;
                            temp.pY = temp.Y;
                            MonsterList.Add(temp);
                        }
                        else if (b == (byte)PacketTypes.ADDITEM)
                        {
                            int count = ClientInc.ReadInt32();
                            for (int i = 0; i < count; i++)
                            {
                                Type type;
                                type = Items[ClientInc.ReadInt32()].GetType();
                                Item temp = new Item();
                                temp = (Item)Activator.CreateInstance(type);
                                temp.X = ClientInc.ReadInt32();
                                temp.Y = ClientInc.ReadInt32();
                                temp.LoadContent();
                                itemsDropped.Add(temp);
                            }
                        }
                        else if (b == (byte)PacketTypes.NEWAREAPLAYER)
                        {
                            int id = ClientInc.ReadInt32();
                            PlayerList[id].pX = ClientInc.ReadInt32();
                            PlayerList[id].pY = ClientInc.ReadInt32();
                        }
                        else if (b == (byte)PacketTypes.NEWAREA)
                        {
                            nAreaX = ClientInc.ReadInt32();
                            nAreaY = ClientInc.ReadInt32();
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
                                temp.pX = temp.X;
                                temp.pY = temp.Y;
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
                                temp.PlayerImage.Layer = .5f;
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
                            int count = ClientInc.ReadInt32();
                            for (int i = 0; i < count; i++)
                            {
                                int j = ClientInc.ReadInt32();
                                j = j - i;
                                MonsterList.Remove(MonsterList[j]);
                            }
                        }
                        else if (b == (byte)PacketTypes.REMOVEITEM)
                        {
                            int count = ClientInc.ReadInt32();
                            for (int i = 0; i < count; i++)
                            {
                                int j = ClientInc.ReadInt32();
                                j = j - i;
                                PlayerList[PlayerID].inventory.Add(itemsDropped[j]);
                                itemsDropped.Remove(itemsDropped[j]);
                            }
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
        }
        public void GetReferences(Map realMap)
        {
            map = realMap;
        }
        public void ClientInterpolate(GameTime gameTime)
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
                else
                {
                    PlayerList[PlayerID].PositionX = PlayerList[PlayerID].X;
                }
                foreach (MapSprite m in map.Maps)
                {
                    m.image.Position.X = m.OriginalPosition.X;
                }
                foreach (Sprite s in map.Sprites)
                {
                    s.image.Position.X = s.OriginalPosition.X;
                }
                foreach (Item item in itemsDropped)
                {
                    item.image.Position.X = item.X;
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
                else
                {
                    PlayerList[PlayerID].PositionX = PlayerList[PlayerID].X - map.DeadZone.Right + (int)ScreenManager.instance.Dimensions.X;
                }
                foreach (MapSprite m in map.Maps)
                {
                    m.image.Position.X = m.OriginalPosition.X - map.DeadZone.Right + (int)ScreenManager.instance.Dimensions.X;
                }
                foreach (Sprite m in map.Sprites)
                {
                    m.image.Position.X = m.OriginalPosition.X - map.DeadZone.Right + (int)ScreenManager.instance.Dimensions.X;
                }
                foreach (Item item in itemsDropped)
                {
                    item.image.Position.X = item.X - map.DeadZone.Right + (int)ScreenManager.instance.Dimensions.X;
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
                else
                {
                    PlayerList[PlayerID].PositionY = PlayerList[PlayerID].Y - map.DeadZone.Bottom + (int)ScreenManager.instance.Dimensions.Y;
                }
                foreach (MapSprite m in map.Maps)
                {
                    m.image.Position.Y = m.OriginalPosition.Y - map.DeadZone.Bottom + (int)ScreenManager.instance.Dimensions.Y;
                }
                foreach (Sprite m in map.Sprites)
                {
                    m.image.Position.Y = m.OriginalPosition.Y - map.DeadZone.Bottom + (int)ScreenManager.instance.Dimensions.Y;
                }
                foreach (Item item in itemsDropped)
                {
                    item.image.Position.Y = item.Y - map.DeadZone.Bottom + (int)ScreenManager.instance.Dimensions.Y;
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
                else
                {
                    PlayerList[PlayerID].PositionY = PlayerList[PlayerID].Y;
                }
                foreach (MapSprite m in map.Maps)
                {
                    m.image.Position.Y = m.OriginalPosition.Y;
                }
                foreach (Sprite m in map.Sprites)
                {
                    m.image.Position.Y = m.OriginalPosition.Y;
                }
                foreach (Item item in itemsDropped)
                {
                    item.image.Position.Y = item.Y;
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
                        foreach (Item item in itemsDropped)
                        {
                            item.HorizontalMove(mapMoveX);
                        }
                    }
                    else if (PlayerList[PlayerID].X > PlayerList[PlayerID].pX + 10)
                    {
                        PlayerList[PlayerID].pX += 10;
                        if (PlayerList[PlayerID].pX > ScreenManager.instance.Dimensions.X / 2)
                        {
                            map.Moved.X = PlayerList[PlayerID].pX;
                            mapMoveX = (PlayerList[PlayerID].pX - (int)ScreenManager.instance.Dimensions.X / 2);
                            map.HorizontalMove();
                            foreach (Item item in itemsDropped)
                            {
                                item.HorizontalMove(mapMoveX);
                            }
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
                        foreach (Item item in itemsDropped)
                        {
                            item.HorizontalMove(mapMoveX);
                        }
                    }
                    else
                    {
                        PlayerList[PlayerID].pX -= 10;
                        if (PlayerList[PlayerID].pX < -(ScreenManager.instance.Dimensions.X / 2) + map.DeadZone.Right)
                        {
                            map.Moved.X = PlayerList[PlayerID].pX;
                            mapMoveX = (PlayerList[PlayerID].pX - (int)ScreenManager.instance.Dimensions.X / 2);
                            map.HorizontalMove();
                            foreach (Item item in itemsDropped)
                            {
                                item.HorizontalMove(mapMoveX);
                            }
                        }
                        else
                        {
                            PlayerList[PlayerID].PositionX -= 10;
                        }
                    }
                }
                else
                {
                    mapMoveX = (PlayerList[PlayerID].X - (int)ScreenManager.instance.Dimensions.X / 2);
                    map.HorizontalMove();
                    foreach (Item item in itemsDropped)
                    {
                        item.HorizontalMove(mapMoveX);
                    }
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
                        foreach (Item item in itemsDropped)
                        {
                            item.VerticalMove(mapMoveY);
                        }
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
                            foreach (Item item in itemsDropped)
                            {
                                item.VerticalMove(mapMoveY);
                            }
                        }
                    }
                    else if (PlayerList[PlayerID].Y == PlayerList[PlayerID].pY - 10)
                    {
                        PlayerList[PlayerID].pY = PlayerList[PlayerID].Y;
                        mapMoveY = PlayerList[PlayerID].Y - (int)ScreenManager.instance.Dimensions.Y / 2;
                        map.VerticalMove();
                        foreach (Item item in itemsDropped)
                        {
                            item.VerticalMove(mapMoveY);
                        }
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
                            foreach (Item item in itemsDropped)
                            {
                                item.VerticalMove(mapMoveY);
                            }
                        }
                    }
                }
                else
                {
                    mapMoveY = PlayerList[PlayerID].Y - (int)ScreenManager.instance.Dimensions.Y / 2;
                    map.VerticalMove();
                    foreach (Item item in itemsDropped)
                    {
                        item.VerticalMove(mapMoveY);
                    }
                }
            }
            PlayerList[PlayerID].PlayerImage.Position.X = PlayerList[PlayerID].PositionX;
            PlayerList[PlayerID].PlayerImage.Position.Y = PlayerList[PlayerID].PositionY;
        }
        public int Interpolate(int pos, int pPos, int moveSpeed)
        {
            if (pos != pPos)
            {
                if (pos == pPos + moveSpeed)
                {
                    return 1;
                }
                else if (pos > pPos + moveSpeed)
                {
                    return 2;
                }
                else if (pos == pPos - moveSpeed)
                {
                    return 1;
                }
                else if (pos < pPos - moveSpeed)
                {
                    return 3;
                }
                else
                {
                    return 4;
                }
            }
            else
            {
                return 1;
            }
        }
        public void AreaTransition(GameTime gameTime, ref Map map, ref Image fadeImage)
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
                    PlayerList[PlayerID].pX = newAreaX;
                    PlayerList[PlayerID].pY = newAreaY;
                    PlayerList[PlayerID].AreaX = nAreaX;
                    PlayerList[PlayerID].AreaY = nAreaY;
                    foreach (MapSprite m in map.Maps)
                    {
                        m.image.UnloadContent();
                    }
                    foreach (Sprite s in map.Sprites)
                    {
                        s.image.UnloadContent();
                    }
                    XmlManager<Map> mapLoader = new XmlManager<Map>();
                    map = mapLoader.Load("Load/Gameplay/Maps/" + map.Area[PlayerList[PlayerID].AreaX, PlayerList[PlayerID].AreaY] + "/Background.xml");
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
                    foreach (NPC npc in map.NPCs)
                    {
                        if (map.Left == false && map.Right == false)
                        {
                            npc.image.Position.X = npc.OriginalPosition.X - map.Moved.X + ScreenManager.instance.Dimensions.X / 2;
                        }
                        else if (map.Right == true)
                        {
                            npc.image.Position.X = npc.OriginalPosition.X - map.DeadZone.Width + ScreenManager.instance.Dimensions.X;
                        }
                        if (map.Down == false && map.Up == false)
                        {
                            npc.image.Position.Y = npc.OriginalPosition.Y - map.Moved.Y + ScreenManager.instance.Dimensions.Y / 2;
                        }
                        else if (map.Down == true)
                        {
                            npc.image.Position.Y = npc.OriginalPosition.Y - map.DeadZone.Height + ScreenManager.instance.Dimensions.Y;
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
                    if (Talking == true)
                    {
                        foreach (NPC npc in map.NPCs)
                        {
                            if (npc.bFarewell == true)
                            {
                                npc.dialogueEnded = true;
                            }
                            else if (npc.Talking == true)
                            {
                                if (npc.text.IsActive == true)
                                {
                                    npc.text.IsActive = false;
                                }
                                else
                                {
                                    npc.ContinueDialogue();
                                }
                                break;
                            }
                        }
                    }
                    foreach (NPC npc in map.NPCs)
                    {
                        if (npc.interactable == true && paused != true)
                        {
                            Talking = true;
                            npc.Talking = true;
                            break;
                        }
                    }
                    if (Talking == false)
                    {
                        PlayerList[PlayerID].Attacking = true;
                        PlayerList[PlayerID].zPressed = true;
                    }
                }
            }
            if (InputManager.Instance.KeyPressed(Keys.LeftShift))
            {
                if (endLagTimer <= 0 && PlayerList[PlayerID].Attacking == false)
                {
                    if (PlayerList[PlayerID].shiftSkill.SkillID != 0)
                    {
                        NetOutgoingMessage outmsg = client.CreateMessage();
                        outmsg.Write((byte)PacketTypes.SKILL);
                        int skillNum = PlayerList[PlayerID].shiftSkill.SkillID;
                        outmsg.Write(skillNum);
                        outmsg.Write(PlayerList[PlayerID].Direction());
                        client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                    }
                }
            }
            if (PlayerList[PlayerID].Attacking == true)
            {
                if (PlayerList[PlayerID].AttackCounter >= 100)
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
            if (map.IsTransitioning == true || paused == true || PlayerList[PlayerID].Attacking == true || Talking == true || endLagTimer > 0)
            {
                Up = Down = Left = Right = false;
            }
            if (paused == true || endLagTimer > 0)
            {
                PlayerList[PlayerID].Attacking = false;
                Talking = false;
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
            if (InputManager.Instance.KeyPressed(Keys.B))
            {
                NetOutgoingMessage outmsg = client.CreateMessage();
                outmsg.Write((byte)PacketTypes.DEBUG);
                client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
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
        public void SendPlayerState()
        {
            NetOutgoingMessage outmsg = client.CreateMessage();
            outmsg.Write((byte)PacketTypes.PLAYERSTATE);
            outmsg.Write(PlayerList[PlayerID].maxHealth);
            outmsg.Write(PlayerList[PlayerID].HealthRegen);
            client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
        }
        public Client()
        {
            dialogueImage = new Image();
            health = new Image();
            updateMonsters = new UpdateMonsters();
            itemsDropped = new List<Item>();
            Items = new Dictionary<int, Item>();
            testItem = new Items.TestItem();
            endLagTimer = 0;
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
            image.Path = "Gameplay/Characters/Player/Player";
            image.Position = new Vector2(100, 100);
            image.LoadContent();
            ConnectionTimer = 0;
            MonsterList = new List<Monster>();
            Combo = 0;
            RogueCombo = 1;
            newArea = false;
            dialogueImage.Path = "Gameplay/GUI/Dialogue";
            dialogueImage.Layer = .6f;
            dialogueImage.LoadContent();
            Talking = false;
            health.Layer = .9f;
            health.color = new Color(255, 0, 0, 255);
            health.LoadContent();
            updateMonsters.LoadContent();
            SetItem<Items.TestItem>(ref testItem, testItem.ItemID);
            skillList = new List<Skills.Skill>();
        }
        public void UnloadContent()
        {

        }
        public void Update(GameTime gameTime, bool paused)
        {
            if (PlayerList.Count > PlayerID)
            {
                if (endLagTimer > 0)
                {
                    endLagTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
                }
                foreach (NPC npc in map.NPCs)
                {
                    if (npc.dialogueEnded == true)
                    {
                        Talking = npc.Talking = npc.dialogueEnded = npc.bGreeting = npc.bDialogue = npc.bFarewell = npc.initiated = false;
                        npc.UnloadText();
                    }
                }
                health.Text = "HP: " + PlayerList[PlayerID].Health.ToString() + "/" + PlayerList[PlayerID].maxHealth;
                //text = new Image();
                //text.Text = PlayerList[PlayerID].Level.ToString();
                //text.LoadContent();
                GetInput(paused);
                SendPlayerState();
                if (Talking == false)
                {
                    SendAttackState();
                }
            }
            RecievePacket();
            if (PlayerList.Count > PlayerID)
            {
                ClientInterpolate(gameTime);
                if (PlayerList[PlayerID].levelUp == true)
                {
                    PlayerList[PlayerID].levelUpImage.IsActive = true;
                    NetOutgoingMessage outmsg = client.CreateMessage();
                    outmsg.Write((byte)PacketTypes.LEVELUP);
                    client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
                }
                PlayerList[PlayerID].UpdateHitBoxes();
                PlayerList[PlayerID].Update(gameTime);
                map.UpdateNPCs(gameTime, PlayerList[PlayerID]);
                foreach (NPC npc in map.NPCs)
                {
                    if (PlayerList[PlayerID].Y + PlayerList[PlayerID].PlayerImage.spriteSheetEffect.FrameHeight < npc.HitBox.Bottom)
                    {
                        npc.image.Layer = .6f;
                    }
                    else
                    {
                        npc.image.Layer = .4f;
                    }
                }
                foreach (Monster m in MonsterList)
                {
                    if (PlayerList[PlayerID].Y + PlayerList[PlayerID].PlayerImage.spriteSheetEffect.FrameHeight < m.Y + m.image.spriteSheetEffect.FrameHeight)
                    {
                        m.image.Layer = .6f;
                    }
                    else
                    {
                        m.image.Layer = .4f;
                    }
                }
            }
            foreach (Player p in PlayerList)
            {
                if (PlayerList.IndexOf(p) != PlayerID)
                {
                    p.Update(gameTime);
                    int data = Interpolate(p.X, p.pX, p.moveSpeed);
                    if (data == 1)
                    {
                        p.pX = p.X;
                        p.PositionX = p.X;
                    }
                    else if (data == 2)
                    {
                        p.pX += p.moveSpeed;
                        p.PositionX += p.moveSpeed;
                    }
                    else if (data == 3)
                    {
                        p.pX -= p.moveSpeed;
                        p.PositionX -= p.moveSpeed;
                    }
                    data = Interpolate(p.Y, p.pY, p.moveSpeed);
                    if (data == 1)
                    {
                        p.pY = p.Y;
                        p.PositionY = p.Y;
                    }
                    else if (data == 2)
                    {
                        p.pY += p.moveSpeed;
                        p.PositionY += p.moveSpeed;
                    }
                    else if (data == 3)
                    {
                        p.pY -= p.moveSpeed;
                        p.PositionY -= p.moveSpeed;
                    }
                    p.PlayerImage.Position.X = p.PositionX - mapMoveX;
                    p.PlayerImage.Position.Y = p.PositionY - mapMoveY;
                }
            }
            foreach (Monster m in MonsterList)
            {
                m.image.Update(gameTime);
                int data = Interpolate(m.X, m.pX, m.moveSpeed);
                if (data == 1)
                {
                    m.pX = m.X;
                    m.PositionX = m.X;
                }
                else if (data == 2)
                {
                    m.pX += m.moveSpeed;
                    m.PositionX += m.moveSpeed;
                }
                else if (data == 3)
                {
                    m.pX -= m.moveSpeed;
                    m.PositionX -= m.moveSpeed;
                }
                data = Interpolate(m.Y, m.pY, m.moveSpeed);
                if (data == 1)
                {
                    m.pY = m.Y;
                    m.PositionY = m.Y;
                }
                else if (data == 2)
                {
                    m.pY += m.moveSpeed;
                    m.PositionY += m.moveSpeed;
                }
                else if (data == 3)
                {
                    m.pY -= m.moveSpeed;
                    m.PositionY -= m.moveSpeed;
                }
                m.image.Position.X = m.PositionX - mapMoveX;
                m.image.Position.Y = m.PositionY - mapMoveY;
            }
            foreach (Skills.Skill skill in skillList)
            {
                skill.image.Update(gameTime);
                int data = Interpolate(skill.X, skill.pX, skill.projSpeed);
                if (data == 1)
                {
                    skill.pX = skill.X;
                    skill.PositionX = skill.X;
                }
                else if (data == 2)
                {
                    skill.pX += skill.projSpeed;
                    skill.PositionX += skill.projSpeed;
                }
                else if (data == 3)
                {
                    skill.pX -= skill.projSpeed;
                    skill.PositionX -= skill.projSpeed;
                }
                data = Interpolate(skill.Y, skill.pY, skill.projSpeed);
                if (data == 1)
                {
                    skill.pY = skill.Y;
                    skill.PositionY = skill.Y;
                }
                else if (data == 2)
                {
                    skill.pY += skill.projSpeed;
                    skill.PositionY += skill.projSpeed;
                }
                else if (data == 3)
                {
                    skill.pY -= skill.projSpeed;
                    skill.PositionY -= skill.projSpeed;
                }
                skill.image.Position.X = skill.PositionX - mapMoveX;
                skill.image.Position.Y = skill.PositionY - mapMoveY;
            }
            foreach (Player p in PlayerList)
            {
                p.levelUpImage.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (text.texture != null)
            {
                text.Draw(spriteBatch);
            }
            if (PlayerList.Count > PlayerID)
            {
                health.DrawString(spriteBatch);
                foreach (Monster m in MonsterList)
                {
                    m.Draw(spriteBatch);
                }
                foreach (Item item in itemsDropped)
                {
                    item.Draw(spriteBatch);
                }
                foreach (Player p in PlayerList)
                {
                    if (PlayerList[PlayerID].AreaX == p.AreaX && PlayerList[PlayerID].AreaY == p.AreaY)
                    {
                        p.PlayerImage.Draw(spriteBatch);
                        if (p.levelUp == true)
                        {
                            p.levelUpImage.Position.X = p.PlayerImage.Position.X - ((p.levelUpImage.spriteSheetEffect.FrameWidth - p.PlayerImage.spriteSheetEffect.FrameWidth) / 2);
                            p.levelUpImage.Position.Y = p.PlayerImage.Position.Y - p.PlayerImage.spriteSheetEffect.FrameHeight;
                            if (p.levelUpImage.spriteSheetEffect.CurrentFrame.X == p.levelUpImage.spriteSheetEffect.AmountOfFrames.X - 1)
                            {
                                p.levelUp = false;
                                p.levelUpImage.IsActive = false;
                            }
                            p.levelUpImage.Draw(spriteBatch);
                        }
                    }
                }
                if (Talking == true)
                {
                    dialogueImage.Draw(spriteBatch);
                }
                foreach (Skills.Skill skill in skillList)
                {
                    skill.Draw(spriteBatch);
                }
            }
        }
    }
}
