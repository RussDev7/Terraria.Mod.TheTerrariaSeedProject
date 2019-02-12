using Terraria.ModLoader;
using Terraria;
using Terraria.UI;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Localization;
using Terraria.GameContent.UI.States;
using System;
using Microsoft.Xna.Framework;




using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI.Gamepad;
using Terraria.World.Generation;
using Terraria.ModLoader;
using Terraria.Map;

using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheTerrariaSeedProject;
using TheTerrariaSeedProject.UI;
using ReLogic.Graphics;


namespace TheTerrariaSeedProject
{
	class TheTerrariaSeedProject : Mod
	{
        //the code was copied und merged from former projects,
        //has many quick coding fixes, repetitions, unused and unnecessary content
        //the code hardly need some refactoring
        //you should not use it as reference


        public TheTerrariaSeedProject instance;
        public TheTerrariaSeedProject()
		{
			Properties = new ModProperties()
			{
				Autoload = true
			};
            instance = this;

        }
        public override void Load()
        {

            Tuple<List<int>, bool, string> conf = WorldGenSeedSearch.readConfigFile();

            if (conf.Item2 == true)
            {
                Main.menuMode = 10; //does not work            
                WorldGen.CreateNewWorld(null);
            }



        }
        public override void Unload()
        {

            string olddbf = Main.SavePath + OptionsDict.Paths.debugPath + @".\debug.txt";
            if (System.IO.File.Exists(olddbf))
            {
                System.IO.File.Delete(olddbf);
            }
            WorldGen.saveLock = false;
        }


    }
}
