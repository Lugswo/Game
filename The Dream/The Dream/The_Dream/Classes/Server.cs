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
        bool Up, Down, Left, Right, AreaAdded;
        UpdateMonsters updateMonsters;
        List<Map> maps;
        List<Vector2> AreasAdded;
        Map map;
        List<Skills.Skill> skillList;
        Dictionary<int, Skills.Skill> dSkills;
        Skills.TestSkill testSkill;
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
        public void SendGameState()
        {
            foreach (Player play in GameState)
            {
                NetOutgoingMessage outmsg = server.CreateMessage();
                outmsg.Write((byte)PacketTypes.WORLDSTATE);
                foreach (Player p in GameState)
                {
                    outmsg.Write(p.X);
                    outmsg.Write(p.Y);
                    outmsg.Write(p.VelocityX);
                    outmsg.Write(p.VelocityY);
                    outmsg.Write(p.AreaX);
                    outmsg.Write(p.AreaY);
                    outmsg.Write(p.EXP);
                    outmsg.Write(p.Health);
                }
                foreach (Player p in GameState)
                {
                    outmsg.Write(p.Attacking);
                    outmsg.Write(p.NextAttack);
                    outmsg.Write(p.Combo);
                }
                foreach (AreaMonsters area in updateMonsters.AreaList)
                {
                    if (play.AreaX != area.AreaX || play.AreaY != area.AreaY)
                    {
                        continue;
                    }
                    foreach (Monster m in area.SpawnedMonsters)
                    {
                        outmsg.Write(m.X);
                        outmsg.Write(m.Y);
                    }
                }
                foreach (Skills.Skill skill in skillList)
                {
                    outmsg.Write(skill.X);
                    outmsg.Write(skill.Y);
                }
                if (server.ConnectionsCount > 0)
                {
                    server.SendMessage(outmsg, play.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                }
            }
        }
        public void LoadMap(int X, int Y)
        {
            Vector2 area = new Vector2(X, Y);
            if (AreasAdded.Count == 0)
            {
                AreasAdded.Add(area);
                XmlManager<Map> mapLoader = new XmlManager<Map>();
                Map temp = mapLoader.Load("Load/Gameplay/Maps/" + map.Area[X, Y] + "/Background.xml");
                temp.AreaX = X;
                temp.AreaY = Y;
                temp.LoadContent();
                maps.Add(temp);
            }
            foreach (Vector2 v in AreasAdded)
            {
                if (v == area)
                {
                    AreaAdded = true;
                }
                else
                {
                    AreaAdded = false;
                }
            }
            if (AreaAdded == false)
            {
                AreasAdded.Add(area);
                XmlManager<Map> mapLoader = new XmlManager<Map>();
                Map temp = mapLoader.Load("Load/Gameplay/Maps/" + map.Area[X, Y] + "/Background.xml");
                temp.AreaX = X;
                temp.AreaY = Y;
                temp.LoadContent();
                maps.Add(temp);
            }
        }
        public void SetSkill<T>(ref T skill, int ID)
        {
            skill = (T)Activator.CreateInstance(typeof(T));
            dSkills.Add(ID, (skill as Skills.Skill));
        }
        public void ReceivePacket()
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
                            player.hair.Path = ServerInc.ReadString();
                            player.eyes.Path = ServerInc.ReadString();
                            player.Connection = ServerInc.SenderConnection;
                            GameState.Add(player);
                            LoadMap(player.AreaX, player.AreaY);
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
                                        joinMsg.Write(temp.hair.Path);
                                        joinMsg.Write(temp.eyes.Path);
                                    }
                                    Thread.Sleep(100);
                                    server.SendMessage(joinMsg, ServerInc.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                                    NetOutgoingMessage outmsg = server.CreateMessage();
                                    outmsg.Write((byte)PacketTypes.ADDPLAYER);
                                    outmsg.WriteAllProperties(player);
                                    outmsg.Write(player.hair.Path);
                                    outmsg.Write(player.eyes.Path);
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
                        else if (b == (byte)PacketTypes.PLAYERSTATE)
                        {
                            foreach (Player p in GameState)
                            {
                                if (p.Connection != ServerInc.SenderConnection)
                                {
                                    continue;
                                }
                                p.maxHealth = ServerInc.ReadInt32();
                                p.HealthRegen = ServerInc.ReadInt32();
                            }
                        }
                        else if (b == (byte)PacketTypes.ATTACK)
                        {
                            foreach (Player p in GameState)
                            {
                                if (p.Connection != ServerInc.SenderConnection)
                                {
                                    continue;
                                }
                                p.Attacking = ServerInc.ReadBoolean();
                                p.NextAttack = ServerInc.ReadBoolean();
                                p.Combo = ServerInc.ReadInt32();
                                p.aUp = ServerInc.ReadBoolean();
                                p.aDown = ServerInc.ReadBoolean();
                                p.aLeft = ServerInc.ReadBoolean();
                                p.aRight = ServerInc.ReadBoolean();
                                p.inCombat = true;
                                p.combatTimer = 0;
                                foreach (AreaMonsters area in updateMonsters.AreaList)
                                {
                                    if (area.AreaX != p.AreaX || area.AreaY != p.AreaY)
                                    {
                                        continue;
                                    }
                                    foreach (Monster m in area.SpawnedMonsters)
                                    {
                                        if (p.aUp == true)
                                        {
                                            if (p.upAttackHitBox.Intersects(m.Hitbox))
                                            {
                                                m.Health = m.Health - p.Strength;
                                            }
                                        }
                                        else if (p.aDown == true)
                                        {
                                            if (p.downAttackHitBox.Intersects(m.Hitbox))
                                            {
                                                m.Health = m.Health - p.Strength;
                                            }
                                        }
                                        else if (p.aLeft == true)
                                        {
                                            if (p.leftAttackHitBox.Intersects(m.Hitbox))
                                            {
                                                m.Health = m.Health - p.Strength;
                                            }
                                        }
                                        else if (p.aRight == true)
                                        {
                                            if (p.rightAttackHitBox.Intersects(m.Hitbox))
                                            {
                                                m.Health = m.Health - p.Strength;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                        else if (b == (byte)PacketTypes.SKILL)
                        {
                            foreach (Player p in GameState)
                            {
                                if (p.Connection != ServerInc.SenderConnection)
                                {
                                    continue;
                                }
                                int ID = ServerInc.ReadInt32();
                                int direction = ServerInc.ReadInt32();
                                Skills.Skill skill;
                                skill = (Skills.Skill)Activator.CreateInstance(dSkills[ID].GetType());
                                skill.LoadContent(p.X, p.Y);
                                if (direction == 0)
                                {
                                    skill.down = true;
                                }
                                else if (direction == 1)
                                {
                                    skill.up = true;
                                }
                                else if (direction == 2)
                                {
                                    skill.right = true;
                                }
                                else if (direction == 3)
                                {
                                    skill.left = true;
                                }
                                skill.areaX = ServerInc.ReadInt32();
                                skill.areaY = ServerInc.ReadInt32();
                                skillList.Add(skill);
                                NetOutgoingMessage outmsg = server.CreateMessage();
                                outmsg.Write((byte)PacketTypes.SKILL);
                                outmsg.Write(skill.image.Path);
                                outmsg.Write(skill.X);
                                outmsg.Write(skill.Y);
                                outmsg.Write(skill.projSpeed);
                                //outmsg.Write(skill.startLag);
                                outmsg.Write(skill.endLag);
                                //outmsg.Write(skill.cooldown);
                                outmsg.Write(skill.areaX);
                                outmsg.Write(skill.areaY);
                                server.SendMessage(outmsg, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                        }
                        else if (b == (byte)PacketTypes.LEVELUP)
                        {
                            foreach (Player p in GameState)
                            {
                                if (p.Connection != ServerInc.SenderConnection)
                                {
                                    continue;
                                }
                                p.EXP = 0;
                                NetOutgoingMessage outmsg = server.CreateMessage();
                                outmsg.Write((byte)PacketTypes.LEVELUP);
                                outmsg.Write(GameState.IndexOf(p));
                                server.SendMessage(outmsg, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                                break;
                            }
                        }
                        else if (b == (byte)PacketTypes.NEXTATTACK)
                        {
                            foreach (Player p in GameState)
                            {
                                if (p.Connection != ServerInc.SenderConnection)
                                {
                                    continue;
                                }
                                p.Attacking = true;
                                p.NextAttack = true;
                                p.Combo = ServerInc.ReadInt32();
                                break;
                            }
                        }
                        else if (b == (byte)PacketTypes.ATTACKEND)
                        {
                            foreach (Player p in GameState)
                            {
                                if (p.Connection != ServerInc.SenderConnection)
                                {
                                    continue;
                                }
                                p.Attacking = false;
                                p.NextAttack = false;
                                p.Combo = 0;
                                break;
                            }
                        }
                        else if (b == (byte)PacketTypes.DEBUG)
                        {
                            foreach (Player p in GameState)
                            {
                                p.EXP += 1000;
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
        }
        public void LoadContent()
        {
            ServerConfig = new NetPeerConfiguration("game");
            ServerConfig.Port = 25565;
            ServerConfig.MaximumConnections = 4;
            ServerConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            ServerConfig.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            server = new NetServer(ServerConfig);
            if (host == true)
            {
                server.Start();
                hostip = "localhost";
            }
            GameState = new List<Player>();
            playerUpdate = new PlayerUpdate();
            updateMonsters = new UpdateMonsters();
            updateMonsters.LoadContent();
            maps = new List<Map>();
            AreaAdded = false;
            map = new Map();
            AreasAdded = new List<Vector2>();
            skillList = new List<Skills.Skill>();
            dSkills = new Dictionary<int, Skills.Skill>();
            testSkill = new Skills.TestSkill();
            SetSkill(ref testSkill, testSkill.SkillID);
        }
        public void UnloadContent()
        {
            updateMonsters.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            ReceivePacket();
            List<int> temp2 = new List<int>();
            foreach (Skills.Skill skill in skillList)
            {
                skill.Update(gameTime);
                if (skill.despawn == true)
                {
                    skill.UnloadContent();
                    temp2.Add(skillList.IndexOf(skill));
                }
            }
            NetOutgoingMessage outmsg3 = server.CreateMessage();
            outmsg3.Write((byte)PacketTypes.REMOVESKILL);
            outmsg3.Write(temp2.Count);
            foreach (int i in temp2)
            {
                outmsg3.Write(i);
                skillList.Remove(skillList[i]);
            }
            if (server.Connections.Count > 0)
            {
                server.SendMessage(outmsg3, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
            }
            foreach (Player p in GameState)
            {
                p.HitBox = new Rectangle(p.X, p.Y, 60, 60);
                updateMonsters.Update(gameTime, p, p.AreaX, p.AreaY);
                foreach (AreaMonsters area in updateMonsters.AreaList)
                {
                    if (p.AreaX != area.AreaX || p.AreaY != area.AreaY)
                    {
                        continue;
                    }
                    if (area.DeadMonsters.Count > 0)
                    {
                        p.EXP += area.EXP;
                        NetOutgoingMessage outmsg = server.CreateMessage();
                        outmsg.Write((byte)PacketTypes.REMOVEMONSTER);
                        outmsg.Write(area.DeadMonsters.Count);
                        foreach (int i in area.DeadMonsters)
                        {
                            outmsg.Write(i);
                        }
                        server.SendMessage(outmsg, p.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                    }
                    foreach (Monster m in area.SpawnedMonsters)
                    {
                        m.Update(gameTime, p);
                    }
                }
                foreach (AreaMonsters area in updateMonsters.AreaList)
                {
                    int count = 0;
                    foreach (Item item in area.Drops)
                    {
                        if (item.itemAdded == true)
                        {
                            count++;
                        }
                    }
                    if (count > 0)
                    {
                        NetOutgoingMessage outmsg2 = server.CreateMessage();
                        outmsg2.Write((byte)PacketTypes.ADDITEM);
                        outmsg2.Write(count);
                        foreach (Item item in area.Drops)
                        {
                            if (item.itemAdded == true)
                            {
                                outmsg2.Write(item.ItemID);
                                outmsg2.Write(item.X);
                                outmsg2.Write(item.Y);
                                item.itemAdded = false;
                            }
                        }
                        server.SendMessage(outmsg2, p.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                    }
                }
                if (p.inCombat == true)
                {
                    p.combatTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (p.combatTimer >= 5)
                    {
                        p.inCombat = false;
                        p.combatTimer = 0;
                    }
                }
                if (p.inCombat == false)
                {
                    if (p.Health < p.maxHealth)
                    {
                        p.OneSecond += gameTime.ElapsedGameTime.TotalSeconds;
                        if (p.OneSecond >= 1)
                        {
                            p.Health += p.HealthRegen;
                            p.OneSecond = 0;
                        }
                    }
                }
            }
            foreach (AreaMonsters area in updateMonsters.AreaList)
            {
                foreach (Player p in GameState)
                {
                    if (area.MonsterAdded == true && p.AreaX == area.AreaX && p.AreaY == area.AreaY)
                    {
                        NetOutgoingMessage outmsg = server.CreateMessage();
                        outmsg.Write((byte)PacketTypes.ADDMONSTER);
                        outmsg.Write(area.SpawnedMonsters[area.SpawnedMonsters.Count - 1].MonsterID);
                        outmsg.Write(area.SpawnedMonsters[area.SpawnedMonsters.Count - 1].X);
                        outmsg.Write(area.SpawnedMonsters[area.SpawnedMonsters.Count - 1].Y);
                        if (server.ConnectionsCount > 0)
                        {
                            server.SendMessage(outmsg, p.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                        }
                    }
                    if (p.AreaX == area.AreaX && p.AreaY == area.AreaY)
                    {
                        NetOutgoingMessage outmsg2 = server.CreateMessage();
                        outmsg2.Write((byte)PacketTypes.REMOVEITEM);
                        int count = 0;
                        foreach (Item item in area.Drops)
                        {
                            item.Update(gameTime, p);
                            if (item.pickedUp == true)
                            {
                                count++;
                            }
                        }
                        outmsg2.Write(count);
                        List<int> temp = new List<int>();
                        foreach (Item item in area.Drops)
                        {
                            if (item.pickedUp == true)
                            {
                                temp.Add(area.Drops.IndexOf(item));
                                outmsg2.Write(area.Drops.IndexOf(item));
                            }
                        }
                        for (int i = 0; i < temp.Count; i++)
                        {
                            area.Drops.Remove(area.Drops[temp[i]]);
                            for (int ii = 0; ii < temp.Count; ii++)
                            {
                                temp[ii]--;
                            }
                        }
                        if (server.ConnectionsCount > 0)
                        {
                            server.SendMessage(outmsg2, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                        }
                    }
                }
                foreach (Monster monster in area.SpawnedMonsters)
                {
                    foreach (Skills.Skill skill in skillList)
                    {
                        if (skill.areaX == monster.AreaX && skill.areaY == monster.AreaY)
                        {
                            if (skill.hitBox.Intersects(monster.Hitbox))
                            {
                                monster.Health -= skill.skillDamage;
                                skill.despawn = true;
                            }
                        }
                    }
                }
                area.MonsterAdded = false;
            }
            foreach (AreaMonsters area in updateMonsters.AreaList)
            {
                area.DeadMonsters.Clear();
                area.EXP = 0;
            }
            foreach (AreaMonsters area in updateMonsters.AreaList)
            {
                foreach (Player p in GameState)
                {
                    if (area.AreaX == p.AreaX && area.AreaY == p.AreaY)
                    {
                        area.playersInside = true;
                        break;
                    }
                    else
                    {
                        area.playersInside = false;
                    }
                }
            }
            foreach (Player p in GameState)
            {
                foreach (AreaMonsters area in updateMonsters.AreaList)
                {
                    if (area.playersInside == false)
                    {
                        area.UnloadContent();
                    }
                }
                Player temp = p;
                p.UpdateHitBoxes();
                foreach (Map map in maps)
                {
                    if (map.AreaX == p.AreaX && map.AreaY == p.AreaY)
                    {
                        playerUpdate.Move(gameTime, ref temp, map);
                        if (playerUpdate.newArea == true)
                        {
                            LoadMap(p.AreaX, p.AreaY);
                        }
                        break;
                    }
                }
                if (playerUpdate.newArea == true)
                {
                    foreach (Map map in maps)
                    {
                        if (map.AreaX == p.AreaX && map.AreaY == p.AreaY)
                        {
                            updateMonsters.NewArea(p.AreaX, p.AreaY, map);
                            if (playerUpdate.aUp == true)
                            {
                                p.Y = map.DeadZone.Bottom - 60;
                            }
                            else if (playerUpdate.aDown == true)
                            {
                                p.Y = map.DeadZone.Top;
                            }
                            else if (playerUpdate.aLeft == true)
                            {
                                p.X = map.DeadZone.Right - 60;
                            }
                            else if (playerUpdate.aRight == true)
                            {
                                p.X = map.DeadZone.Left;
                            }
                            NetOutgoingMessage outmsg = server.CreateMessage();
                            outmsg.Write((byte)PacketTypes.NEWAREA);
                            outmsg.Write(p.AreaX);
                            outmsg.Write(p.AreaY);
                            outmsg.Write(p.X);
                            outmsg.Write(p.Y);
                            foreach (AreaMonsters area in updateMonsters.AreaList)
                            {
                                if (area.AreaX == p.AreaX && area.AreaY == p.AreaY)
                                {
                                    outmsg.Write(area.SpawnedMonsters.Count);
                                    foreach (Monster m in area.SpawnedMonsters)
                                    {
                                        outmsg.Write(m.X);
                                        outmsg.Write(m.Y);
                                        outmsg.Write(m.image.Path);
                                        outmsg.Write(m.image.spriteSheetEffect.AmountOfFrames.X);
                                        outmsg.Write(m.image.spriteSheetEffect.AmountOfFrames.Y);
                                    }
                                }
                            }
                            server.SendMessage(outmsg, p.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                            NetOutgoingMessage outmsg2 = server.CreateMessage();
                            outmsg2.Write((byte)PacketTypes.NEWAREAPLAYER);
                            outmsg2.Write(GameState.IndexOf(p));
                            outmsg2.Write(p.X);
                            outmsg2.Write(p.Y);
                            server.SendMessage(outmsg2, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                            playerUpdate.newArea = playerUpdate.aUp = playerUpdate.aDown = playerUpdate.aLeft = playerUpdate.aRight = false;
                            break;
                        }
                    }
                }
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
