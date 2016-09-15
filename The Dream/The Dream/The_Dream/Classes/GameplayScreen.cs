using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace The_Dream.Classes
{
    public class GameplayScreen : GameScreen
    {
        PlayerUpdate playerUpdate;
        Map map = new Map();
        Textures textures;
        UpdateGameMenu gameMenu;
        UpdateMonsters updateMonsters = new UpdateMonsters();
        Controls controls = new Controls();
        CharacterCreationScreen characterCreate = new CharacterCreationScreen();
        SoundManager soundManager = new SoundManager();
        Client client = new Client();
        Player player = new Player();
        Server server = new Server();
        bool setFade = false;
        Image fadeImage;
        void AreaTransition(GameTime gameTime, int X, int Y, ref Map map, ref Image fadeImage)
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
                    map.Update(gameTime, client.PlayerList[client.PlayerID]);
                    map.LoadContent();
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
                    client.GetReferences(map);
                }
                else if (fadeImage.Alpha == 0.0f)
                {
                    fadeImage.IsActive = false;
                    map.IsTransitioning = false;
                    setFade = false;
                }
            }
        }
        public GameplayScreen()
        {
            fadeImage = new Image();
            fadeImage.Alpha = 0;
            fadeImage.Path = "ScreenManager/FadeImage";
            fadeImage.Effects = "FadeEffect";
            fadeImage.Scale = new Vector2(10, 10);
            fadeImage.LoadContent();
        }
        public override void LoadContent()
        {
            base.LoadContent();
            XmlManager<Client> clientLoader = new XmlManager<Client>();
            XmlManager<Server> serverLoader = new XmlManager<Server>();
            if (File.Exists("Load/ServerSavedIPandHost.xml"))
            {
                server = serverLoader.Load("Load/ServerSavedIPandHost.xml");
            }
            if (File.Exists("Load/ClientSavedIPandHost.xml"))
            {
                client = clientLoader.Load("Load/ClientSavedIPandHost.xml");
                server = serverLoader.Load("Load/ServerSavedIPandHost.xml");
            }
            if (server.host == true)
            {
                server.LoadContent();
            }
            client.LoadContent();
            XmlManager<Player> playerLoader = new XmlManager<Player>();
            player = playerLoader.Load("Load/Gameplay/Savefile.xml");
            XmlManager<Map> mapLoader = new XmlManager<Map>();
            map = mapLoader.Load("Load/Gameplay/Maps/" + map.Area[player.AreaX, player.AreaY] + "/Background.xml");
            map.LoadContent();
            client.GetReferences(map);
            if (server.host == true)
            {
                server.GetReferences(map);
            }
            //XmlManager<Player> playerLoader = new XmlManager<Player>();
            //player = playerLoader.Load("Load/Gameplay/SaveFile.xml");
            //player.LoadContent();
            //XmlManager<UpdateGameMenu> menuLoader = new XmlManager<UpdateGameMenu>();
            //gameMenu = menuLoader.Load("Load/Gameplay/GameMenu.xml");
            //gameMenu.SetReferences(map, playerUpdate);
            //gameMenu.LoadContent();
            //XmlManager<Textures> textureLoader = new XmlManager<Textures>();
            //textures = textureLoader.Load("Load/Gameplay/Maps/" + map.Area[map.AreaX, map.AreaY] + "/Textures.xml");
            //textures.LoadContent();
            //XmlManager<SoundManager> soundLoader = new XmlManager<SoundManager>();
            //soundManager = soundLoader.Load("Load/Gameplay/Sound.xml");
            //soundManager.LoadContent();
            //updateMonsters.GetMap(map, textures);
            //updateMonsters.LoadContent();
            //playerUpdate.GetReferences(map, soundManager);
            //map.GetReferences(playerUpdate, soundManager);
            //textures.GetPlayerMap(playerUpdate, map);
            //controls.GetReferences(playerUpdate, map, gameMenu, soundManager);
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            client.UnloadContent();
            map.UnloadContent();
            server.UnloadContent();
            //map.UnloadContent();
            //player.UnloadContent();
            //gameMenu.UnloadContent();
            //textures.UnloadContent();
            //updateMonsters.UnloadContent();
            //soundManager.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            client.Update(gameTime);
            if (server.host == true)
            {
                server.Update(gameTime);
            }
            AreaTransition(gameTime, map.AreaX, map.AreaY, ref map, ref fadeImage);
            //map.Update(gameTime);
            //textures.Update(gameTime);
            //updateMonsters.Update(gameTime);
            //controls.Update(gameTime);
            //gameMenu.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            map.Draw(spriteBatch);
            client.Draw(spriteBatch);
            if (map.IsTransitioning == true)
            {
                fadeImage.Draw(spriteBatch);
            }
            //map.Draw(spriteBatch);
            //updateMonsters.Draw(spriteBatch);
            //textures.Draw(spriteBatch);
            //gameMenu.Draw(spriteBatch);
        }
    }
}
