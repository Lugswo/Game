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
        public Map map;
        bool Up, Down, Left, Right;
        UpdateMonsters updateMonsters;
        public void GetReferences(Map realMap)
        {
            map = realMap;
        }
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
            NEWAREAPLAYER
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
                if (server.ConnectionsCount > 0)
                {
                    server.SendMessage(outmsg, play.Connection, NetDeliveryMethod.ReliableOrdered, 0);
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
        }
        public void UnloadContent()
        {
            updateMonsters.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            foreach (Player p in GameState)
            {
                p.HitBox = new Rectangle(p.X, p.Y, 54, 64);
                updateMonsters.Update(gameTime, p, p.AreaX, p.AreaY);
                foreach (AreaMonsters area in updateMonsters.AreaList)
                {
                    if (p.AreaX != area.AreaX || p.AreaY != area.AreaY)
                    {
                        continue;
                    }
                    int count = area.DeadMonsters.Count;
                    for (int i = 0; i < count; i++)
                    {
                        NetOutgoingMessage outmsg = server.CreateMessage();
                        outmsg.Write((byte)PacketTypes.REMOVEMONSTER);
                        outmsg.Write(area.DeadMonsters[i]);
                        server.SendMessage(outmsg, p.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                        for (int j = 0; j < count; j++)
                        {
                            if (area.DeadMonsters[j] > area.DeadMonsters[i])
                            {
                                area.DeadMonsters[j] = area.DeadMonsters[j] - 1;
                            }
                        }
                    }
                    area.DeadMonsters.Clear();
                    foreach (Monster m in area.SpawnedMonsters)
                    {
                        m.Update(gameTime, p);
                    }
                }
                foreach (AreaMonsters area in updateMonsters.AreaList)
                {
                    if (area.MonsterAdded == true && p.AreaX == area.AreaX && p.AreaY == area.AreaY)
                    {
                        NetOutgoingMessage outmsg = server.CreateMessage();
                        outmsg.Write((byte)PacketTypes.ADDMONSTER);
                        foreach (Monster m in area.SpawnedMonsters)
                        {
                            outmsg.Write(m.X);
                            outmsg.Write(m.Y);
                            outmsg.Write(m.image.Path);
                            outmsg.Write(m.image.spriteSheetEffect.AmountOfFrames.X);
                            outmsg.Write(m.image.spriteSheetEffect.AmountOfFrames.Y);
                        }
                        if (server.ConnectionsCount > 0)
                        {
                            server.SendMessage(outmsg, p.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                        }
                        area.MonsterAdded = false;
                    }
                }
            }
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
                            }
                        }
                        else if (b == (byte)PacketTypes.SHUTDOWN)
                        {
                            server.Shutdown("bye");
                        }
                        else if (b == (byte)PacketTypes.NEWAREA)
                        {
                            foreach (Player p in GameState)
                            {
                                if (p.Connection != ServerInc.SenderConnection)
                                {
                                    continue;
                                }
                                Player temp = p;
                                temp.AreaX = ServerInc.ReadInt32();
                                temp.AreaY = ServerInc.ReadInt32();
                                bool Up = ServerInc.ReadBoolean();
                                bool Down = ServerInc.ReadBoolean();
                                bool Left = ServerInc.ReadBoolean();
                                bool Right = ServerInc.ReadBoolean();
                                p.DeadZone.X = ServerInc.ReadInt32();
                                p.DeadZone.Y = ServerInc.ReadInt32();
                                p.DeadZone.Width = ServerInc.ReadInt32();
                                p.DeadZone.Height = ServerInc.ReadInt32();
                                playerUpdate.NewArea(ref temp, Up, Down, Left, Right);
                                updateMonsters.NewArea(p.AreaX, p.AreaY);
                                NetOutgoingMessage outmsg = server.CreateMessage();
                                outmsg.Write((byte)PacketTypes.NEWAREAMONSTERS);
                                int count = 0;
                                foreach (AreaMonsters area in updateMonsters.AreaList)
                                {
                                    if (area.AreaX == p.AreaX && area.AreaY == p.AreaY)
                                    {
                                        count = area.SpawnedMonsters.Count();
                                    }
                                }
                                outmsg.Write(count);
                                foreach (AreaMonsters area in updateMonsters.AreaList)
                                {
                                    foreach (Monster m in area.SpawnedMonsters)
                                    {
                                        if (p.AreaX == m.AreaX && p.AreaY == m.AreaY)
                                        {
                                            outmsg.Write(m.X);
                                            outmsg.Write(m.Y);
                                            outmsg.Write(m.image.Path);
                                            outmsg.Write(m.image.spriteSheetEffect.AmountOfFrames.X);
                                            outmsg.Write(m.image.spriteSheetEffect.AmountOfFrames.Y);
                                        }
                                    }
                                }
                                server.SendMessage(outmsg, p.Connection, NetDeliveryMethod.ReliableOrdered);
                                int id = GameState.IndexOf(p);
                                NetOutgoingMessage outmsg2 = server.CreateMessage();
                                outmsg2.Write((byte)PacketTypes.NEWAREAPLAYER);
                                outmsg2.Write(id);
                                server.SendMessage(outmsg2, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
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
            foreach (Player p in GameState)
            {
                Player temp = p;
                p.UpdateHitBoxes();
                playerUpdate.Move(gameTime, ref temp, p.Up, p.Down, p.Left, p.Right);
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
