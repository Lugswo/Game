using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        ClientServer clientServer = new ClientServer();
        public override void LoadContent()
        {
            base.LoadContent();
            clientServer.LoadContent();
            //XmlManager<Map> mapLoader = new XmlManager<Map>();
            //map = mapLoader.Load("Load/Gameplay/Maps/" + map.Area[map.AreaX, map.AreaY] + "/Background.xml");
            //map.LoadContent();
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
            clientServer.UnloadContent();
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
            clientServer.Update(gameTime);
            //map.Update(gameTime);
            //textures.Update(gameTime);
            //updateMonsters.Update(gameTime);
            //controls.Update(gameTime);
            //gameMenu.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            clientServer.Draw(spriteBatch);
            //map.Draw(spriteBatch);
            //updateMonsters.Draw(spriteBatch);
            //textures.Draw(spriteBatch);
            //gameMenu.Draw(spriteBatch);
        }
    }
}
