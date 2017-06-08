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
        UpdateGameMenu gameMenu;
        UpdateMonsters updateMonsters = new UpdateMonsters();
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
            fadeImage.Layer = 1.0f;
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
            map = mapLoader.Load("Load/Gameplay/Maps/" + map.Area[player.areaXSpawn, player.areaYSpawn] + "/Background.xml");
            map.LoadContent();
            client.GetReferences(map);
            XmlManager<UpdateGameMenu> menuLoader = new XmlManager<UpdateGameMenu>();
            gameMenu = new UpdateGameMenu();
            gameMenu.LoadContent(client.player);

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
            client.AreaTransition(gameTime, ref client.map, ref fadeImage);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            client.map.Draw(spriteBatch);
            client.map.DrawNPCs(spriteBatch);
            client.Draw(spriteBatch);
            if (client.PlayerList.Count > 0)
            {
                gameMenu.Draw(spriteBatch, client.PlayerList[client.PlayerID]);
            }
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
