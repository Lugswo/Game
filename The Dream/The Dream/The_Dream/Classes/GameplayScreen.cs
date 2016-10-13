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
            XmlManager<Textures> textureLoader = new XmlManager<Textures>();
            textures = textureLoader.Load("Load/Gameplay/Maps/" + map.Area[player.AreaX, player.AreaY] + "/Textures.xml");
            textures.LoadContent();
            map.GetReferences(textures);
            client.GetReferences(map, textures);
            if (server.host == true)
            {
                server.GetReferences(map);
            }
            //XmlManager<Player> playerLoader = new XmlManager<Player>();
            //player = playerLoader.Load("Load/Gameplay/SaveFile.xml");
            //player.LoadContent();
            XmlManager<UpdateGameMenu> menuLoader = new XmlManager<UpdateGameMenu>();
            gameMenu = menuLoader.Load("Load/Gameplay/GameMenu.xml");
            gameMenu.LoadContent(player);

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
            if (server.host == true)
            {
                server.UnloadContent();
            }
            gameMenu.UnloadContent();
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
            if (client.PlayerList.Count > client.PlayerID)
            {
                gameMenu.Update(gameTime, client.PlayerList[client.PlayerID]);
            }
            client.Update(gameTime, gameMenu.paused);
            if (server.host == true)
            {
                server.Update(gameTime);
            }
            client.AreaTransition(gameTime, client.map.AreaX, client.map.AreaY, ref client.map, ref fadeImage);
            //map.Update(gameTime);
            //textures.Update(gameTime);
            //updateMonsters.Update(gameTime);
            //controls.Update(gameTime);
            //gameMenu.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            client.map.Draw(spriteBatch);
            client.textures.Draw(spriteBatch);
            client.Draw(spriteBatch);
            gameMenu.Draw(spriteBatch);
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
