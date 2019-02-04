using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TheTerrariaSeedProject.UI
{
    class OptionsDict : Dictionary<string, List<string>>
    {
        public Dictionary<string, string> HelpDict;


        public const string title = "#### The Terraria Seed Project ####";

        public static class WorldInformation
        {
            public const string worldName = "World Name";
            public const string evilType = "Evil type";
            public const string difficulty = "Difficulty";
            public const string worldSize = "World size";


            public const string randomInverse = "Random inverse";

        }
        public static class Configuration
        {
            public const string configName = "Config file name";
            public const string startingSeed = "Starting seed search at";
            public const string searchSeedNum = "Search until starting seed +";
            public const string stopSearchNum = "Stop search if found";
            public const string stepSize = "Seed cycle step size";
            public const string storeMMPic = "Store mini map as picture";
            public const string storeStats = "Store stats for each seed";
        }

        public static class Phase1
        {
            public const string title = "### Phase 1 ###";
            public const string pyramidsPossibleLegacy = "Pyramids possible";
            public const string pyramidsPossible = "Pyramids spots possible >=";

            public const string copperTin = "Copper or Tin";
            public const string ironLead = "Iron or Lead";
            public const string silverTungsten = "Silver or Tungsten";
            public const string goldPlatin = "Gold or Platinum";
            public const string moonType = "Moon type";            
            public const string hallowSide = "Hallow biome side";
            public const string dungeonWallColor = "Dungeon wall color";
            public const string dungeonSide = "Dungeon side";
            public const string boost = "Boost (experimental) >=";
        }

        public static class Phase2
        {
            public const string title = "### Phase 2 ###";
            public const string positive = "Phase 2+";
            public const string negative = "Phase 2-";


            public const string dungeonStrangePos = "Dungeon has strange Pos";
            public const string dungeonGoodPos = "Dungeon has good Pos";
            public const string maxPyrExitCavDist = "Max pyr. exit cav.-entr. distance";
            public const string maxTreeExitCavDist = "Max Tree exit cav.-entr. distance";
            public const string allDungeonWalls = "All Dungeon Walls";
            public const string dungeonFarmSpot = "Dungeon farm spot";
            public const string dungeonFarmSpotInters = "Dungeon farm spot 3Wall inters.";
            public const string dungeonFarmSpotLine = "Dungeon farm spot 3Wall in line";

      
           
        }

        public static class Phase3
        {
            public const string title = "### Phase 3 ###";
            public const string positive = "Phase 3+";
            public const string negative = "Phase 3-";
            public const string continueEvaluation = "Takeover positive";
            public const string continueEvaluationResetTag = "Start new";
            public const string continueEvaluatioTakeOverTag = "Takeover";



            public const string openTemple = "Open Temple";
            public const string greenPyramid = "Green Pyramid";
            public const string lonelyJungleTree = "Lonely jungle tree";
            public const string allChestItemsNoCraftFish = "All chest items you can't craft or fish";

            public const string pathlength = "Pathlength";



        }

        public static class GeneralOptions
        {
            public const string title = "### General Options ###";  //<--- outdated
            public const string naming = "Naming*";
            public const string omitRare = "OmitRare/";
            public const string omitRareAll = "Omit All";
            public const string omitBaCRare = "Omit bad and more common rare";
            public const string omitBadRare = "Omit bad rare";
            public const string omitCommonRare = "Omit more common rare";


            public const string fantasyScore = "Fantasy score";
            public const string contentShort = "Content in short";
            public const string sizeEvilDiff = "Size, evil type, difficulty";


            public const string distance = "Distance";
            public const string distanceS = "distance";



        }

        public static class Tools
        {
            public const string dummyPlus =          "-Tool- if all true, increase phase points by";
            public const string dummyNeg = "-Tool- decrease phase points by";
            public const string dummyNegEnhancer = "-Tool- below reduce by";

            public const string conditionConnector = "-Tool- Condition Connector"; //careful in naming even more containing this substring
            public const string conditionConnector1 = "-Tool- Condition Connector 1 is false";//carefull renaming those number bound to normal string length
            public const string conditionConnector2 = "-Tool- Condition Connector 2 is false";
            public const string conditionConnector3 = "-Tool- Condition Connector 3 is false";
            public const string conditionConnector4 = "-Tool- Condition Connector 4 is false";
            public const string conditionConnector5 = "-Tool- Condition Connector 5 is false";
        }


        public static class Paths
        {
            public const string statsPath = @"/TheTerrariaSeedProject/stats/"; // System.IO.Path.DirectorySeparatorChar
            public const string configPath = @"/TheTerrariaSeedProject/config/";
            public const string debugPath = @"/TheTerrariaSeedProject/";
        }


        public static class Bug
        {
            public const string worldSizeBugMinMed = "World size min Med";
            public const string worldSizeBugMinLarge = "World size min Larg";
        }

        public static class ModConfig
        {
            public const string dontShowItemIDs = "#dontShowItemIDs:";            
        }

        public static class Help
        {
            public const string helpBut = "Help!";
            public const string helpButHover = "Help!Hover";
        }


        public static class SelectableList
        {
            public const string positive = "+VVVVVVV positive if >= VVVVVVV+";
            public const string negative = "-VVVVVVV negative if > VVVVVVV-";
            public const string name = "*VVVVVVV naming order VVVVVVV*";
            public const string omitRare = "/VVVVVVV omit rare VVVVVVV/";
            public const string addingTag = "Add";
            public const string removeTag = "  X Remove  ";



            public const string positiveDescription = "Here you can add conditions you are looking for. Each list contains as many conditions you want. For each condition you can select a value. A condition is true if the map " +
                "seed generates a world which has at least the selected value or more for that property. E.g. “Number of Pyramids: 3” means the world need to have at least 3 Pyramids. If all conditions of a list are true the seed " +
                "gets one point. You can add multiple positive lists with multiple conditions in each. Each of those lists can add one point to the total points. An item in the negative list will reduce the points by one again. To " +
                "pass a Phase the seed need at least one point. " +
                "If you add multiple positive lists which can (if all conditions are true) lead to more than 1 point. Also a condition from the negative list can be true and the seed still have enough points to get access to the next " +
                "Phase.  \n For some conditions you can only select 0 or 1. 1 means true, 0 false. E.g. “Dungeon in Snow Biome: 1” means in positive list you want a  Dungeon which is located in the Snow Biome. If you set it to 0 it " +
                "is means you want a seed which has at least no Dungeon in Snow Biome, but even better if it is located there. So 0 is true for each map for that condition. \n" +
                "As advanced function you can use the tool '"+ Tools.dummyPlus+ "'. This allows you to change the amount of points a seeds gets if such a positive condition list is true. That can be used e.g. if you have multiple " +
                "condition lists which you value different or if you are OK if some negative list elements are true. \n If you add not a single condition the seed can only pass that phase if also the negative list is empty. If that's " +
                "the case this phase is skipped.";

            public const string negativeDescription = "At this negative list you can add stuff you don't want in your world. Other than on positive list each condition placed here has a negative impact if true. To be true it need " +
                "to be greater than the selected value. So if it's equal it is not fulfilled. \n Same as in positive some conditions only have value 0 and 1. 1 means true, 0 not. But if you select something like “Dungeon in Snow " +
                "Biome: 1” it won't have any effect, because a negative condition is only true if it is grater than the selected value. And greater than true does not work. So if you don't want it you need to select 0. Furthermore " +
                "Dungeon in Snow Biome is also considered as rare. So in that case you also need to select it in omit rare. \n" +
                "As advanced function you can use the tool '"+Tools.dummyNeg + "'. This allows you to reduce the points by a given value. That can be used e.g. with multiple positive condition lists.";

            public const string omitRareDescription = "Rares are as the name implies stuff which don’t happen that often during world generation, e.g. spawning at a floating island. If something rare got detected the map seed " +
                "qualifies for complete generation without fulfilling the condition you made in the different phase sections. \n If you are looking for many different rares it gets quite common to find any of those. " +
                "For that reason you can select rares here which you don’t want to grant access to full world generation anymore.\n So even if the current seed has such a rare you listed here, it still needs to fulfill the " +
                "other conditions you made. \nSome rares can only be detected during phase 3. So if a seed does not fulfill the conditions in phase 1 or 2 it won’t get detected. Same for phase 2 if phase 1 not fulfilled. " +
                "It can’t detect any rare in phase 1. \n \n>> Omit All: no rare counts, each seed need to fulfill all conditions you set up \n>> Omit bad rare: omits no ocean, Dungeon far above surface or below ground " +
                "(except there is also a living tree reaching to it), and also omits if a floating island got no chest \n>>Omit more common rare: omits Chest duplication Glitch, Pre Skeletron Dungeon Chest Risky/Grab " +
                "(except its a Muramasa for NE), Dungeon in Snow Biome, Near Enchanted Sword, Enchanted Sword near Tree/Pyramid, Spawn in Snow biome, Lonely jungle tree \n>>Omit bad and more common rare: combines both";




            public const string nameDescription = "If a seed passes all Phases it gets saved in your world folder.Here you can select how it should be named. Those tags are concatenated. \n" +
                    ">> World name: the name you entered above or in at world creation. \n" +
                    ">> Seed: seed used to generate the map \n" +
                    ">> Size, evil type, difficulty: (S)mall, (M)edium, (L)arge; crimson (r or R (for red)), corruption (b or B (for blue)), random evil biome is always the same for one seed, if the evil biome letter is written small (r or b) you can use random as well. e(x)pert, (n)ormal  \n" +
                    ">> Content in short: Some short form of the map content. Each digit has some meaning. \n" +
                    "1.) Number of Pyramids \n" +
                    "2.) If it has a Flying Carpet(C), a Sandstorm in a Bottle(B), or both(H), also Mask (O), or at least a Blizzard in a Bottle(Z) \n" +
                    "3.) If it has a Tree with Chest(T) , or even better a Tree with Living Loom(L) \n" +
                    "4.) Flower Boots(F), Staff of Regrowth or both growing items(G) \n" +
                    "5.) If it has a Sky Mill and all Cloud items(M), Sky Mill but not all other items(m) or if no Sky Mill it only displays the number of Floating islands with chests. \n" +
                    "6.) A valid Enchanted Sword Shrine(S), or Enchanted Swords somewhere else (E) \n" +
                    "7.) Lava Charm(U), Water Walking Boots(K) or both items needed for Lava Waders(V) \n" +
                    "8.) Shows some other rare or good stuff in following order: \n" +
                    "(i.) Water Bolt you can get before Skeletron(W) \n" +
                    "(ii.) a Tree near spawn(I) \n" +
                    "(iii.) a Cloud near spawn(Q) \n" +
                    "(iv.) a Fiberglass Fishing Pole(J) \n" +
                    "(v.) a Altar near spawn(A) \n" +
                    "(vi.) the number of chests near spawn \n" +
                    "9.) Fantasy score divided by 333 to scale it from 1 to 9+, if greater than 9 it shows(x), greater 10 (X), if it has less than 0 it shows(-). It can be overwritten if the map has some rare stuff, that could be: \n" +
                    "(i.) Chest duplication Glitch(D)\n" +
                    "(ii.) A Enchanted Sword which is very near to Spawn(#) \n" +
                    "(iii.) Spawn in Sky(Y) \n" +
                    "(iv.) "+Phase3.allChestItemsNoCraftFish+" (@) \n" +
                    "(v.) Near Enchanted Sword(N) but only if scaled score is less than 10  \n" +
                    "(vi.) Enchanted Sword near Pyramid or Tree(+) but only if scaled score is less than 10  \n" +
                    "(vii.) if the scaled score is less than 10 but the map has many pyramids for its size(P), 4 for small, 5 for medium, 6 for large \n" +
                    ">> Fantasy score: a total score of the seed is computed. It takes e.g. into account how many rare items or structures the map has, how many Pyramids, Trees, Clouds, Enchanted Swords, the location of the evil tiles, " +
                    "the placement of the Dungeon, how the beach looks and many many more. \n" +
                    ">> Rares: it add all kind of detected rares to the map name.e.g.SpawnSky, DungeonStrange, SpawnSnow... " +
                    "\n \n If you don't select anything it does: Seed_Content_Score_Rares";
        }




        public static readonly List<string> v0to6 = getPatern(0, 6);
        public static readonly List<string> v0to9 = getPatern(0, 10);
        public static readonly List<string> v0to10 = getPatern(0, 11);
        public static readonly List<string> v0to15 = getPatern(0, 16);
        public static readonly List<string> v0to20 = getPatern(0, 21);
        public static readonly List<string> v0to50 = getPatern(0, 51);
        public static readonly List<string> v0to80 = getPatern(0, 81);

        public static readonly List<string> v0to100 = getPatern(0, 101);
        public static readonly List<string> v0to120 = getPatern(0, 121);
        public static readonly List<string> v0to300_5 = getPatern(0, 61, 5);
        public static readonly List<string> v0to500_5 = getPatern(0, 101, 5);
        public static readonly List<string> v0to500_5_700_10 = getPatern(0, 100, 5).Concat(getPatern(500, 21, 10)).ToList();
        public static readonly List<string> v0to200_2 = getPatern(0, 101, 2);        
        public static readonly List<string> v0to5 = getPatern(0, 6);
        public static readonly List<string> v0to1 = getPatern(0, 2);
        public static readonly List<string> v0to2 = getPatern(0, 3);
        public static readonly List<string> vboost = getPatern(10, 40).Concat(getPatern(50, 11, 5).Concat(new string[] {"110","134","150","190","200","300","400" }.ToList())).ToList();
        public static readonly List<string> vDungeonWalls = getPatern(10000, 19, 5000);
        public static readonly List<string> vDungeonALLWalls = getPatern(0, 4, 250).Concat(getPatern(1000, 9, 1000)).Concat(getPatern(10000, 21, 2000)).ToList();
        public static readonly List<string> vNearEvilOcean = getPatern(0, 8, 25).Concat(getPatern(400, 5, 100)).Concat(getPatern(1000, 10, 200)).ToList();
        public static readonly List<string> vBeachPenalty = getPatern(100, 9, 100).Concat(getPatern(1000, 4, 250)).Concat(getPatern(2000, 16, 500)).Concat(getPatern(10000, 11, 1000)).ToList();
        public static readonly List<string> vEvilTiles = getPatern(0, 10, 10).Concat(getPatern(100, 9, 100)).Concat(getPatern(1000, 9, 1000)).Concat(getPatern(10000, 9, 5000)).ToList();
        public static readonly List<string> vEvilTilesNeg = getPatern(-50000, 8, 5000).Concat(getPatern(-10000, 9, 1000)).Concat(getPatern(-1000, 9, 100)).Concat(getPatern(-100, 11, 10)).ToList();
        public static readonly List<string> vDistance = getPatern(5, 4, 5).Concat(getPatern(25, 7, 25).Concat(getPatern(200, 4, 50).Concat(getPatern(400, 6, 100).Concat(getPatern(1000, 5, 200)).Concat(getPatern(2000, 9, 250))))).ToList();
        public static readonly List<string> vDistanceNeg = getPatern(-1000, 9, 100).Concat(getPatern(-100, 3, 25).Concat(getPatern(-25, 10, 5).Concat(getPatern(25, 3, 25).Concat(getPatern(100, 9, 100).Concat(getPatern(1000, 5, 200)).Concat(getPatern(2000, 9, 250)))))).ToList();
        public static readonly List<string> vDistanceShort = getPatern(25, 9, 25).Concat(getPatern(250, 4, 50)).Concat(getPatern(500, 16, 100)).ToList();
        public static readonly List<string> vDistanceShortNeg = getPatern(-25, 9, -25).Concat(getPatern(-250, 4, -50)).Concat(getPatern(-500, 16, -100)).ToList();
        public static readonly List<string> vDistanceLong = getPatern(200, 8, 100).Concat(getPatern(1000, 17, 200)).ToList();
        public static readonly List<string> vDistanceLongNeg = getPatern(-200, 8, -100).Concat(getPatern(-1000, 17, -200)).ToList();
        public static readonly List<string> vNumTiles = getPatern(0, 10, 10).Concat(getPatern(100, 9, 100)).Concat(getPatern(1000, 9, 1000).Concat(getPatern(10000, 9, 5000))).ToList();
        public static readonly List<string> vNumSearch = getPatern(1, 7).Concat(getPatern(10, 4, 5)).Concat(getPatern(50, 16, 0, 2, 5)).ToList();
        public static readonly List<string> vStepSize = (new string[] { "1000000000", "100000000", "10000000", "1000000", "100000", "10000", "1000", "100", "25", "10", "5", "3", "2", "1", "0", "-1", "-2", "-3", "-5", "-10", "-25", "-100", "-1000", "-10000", "-100000", "-1000000", "-10000000","-100000000", "-1000000000" }).ToList();
        public static readonly List<string> vScore = getPatern(-1000, 61, 100);
        public static readonly List<string> vEmpty = new List<string>();
        public static readonly List<string> vPaintingsDiff = getPatern(0, 52);
        public static readonly List<string> vPaintingsTotal = getPatern(0, 21, 5);
        public static readonly List<string> vStatuesFuncTotal = getPatern(50, 21, 10);
        public static readonly List<string> vStatuesTotal = getPatern(100, 31, 20);//84 240 , 173 536
        public static readonly List<string> vDartTrap = getPatern(0, 20, 5).Concat(getPatern(100, 9, 25)).ToList();
        public static readonly List<string> vSpikyTrap = getPatern(0, 31, 5);
        public static readonly List<string> vSpearTrap = getPatern(0, 21, 5);
        public static readonly List<string> vGeyDetoTrap = getPatern(0, 50).Concat(getPatern(50, 11, 10)).ToList();
        public static readonly List<string> vPathLength = getPatern(10, 9, 10).Concat(getPatern(100, 9, 100).Concat(getPatern(1000, 14, 250)).Concat(getPatern(5000, 6, 1000))).ToList();
        public static readonly List<string> vPathLengthNeg = getPatern(-10, 9, -10).Concat(getPatern(-100, 9, -100).Concat(getPatern(-1000, 14, -250)).Concat(getPatern(-5000, 6, -1000))).ToList();
        public static readonly List<string> vTilesToMine = getPatern(0, 50).Concat(getPatern(50, 11, 10)).ToList();
        public static readonly List<string> vAltarBeach = (new string[] { "0", "10", "25", "50", "75", "100", "150", "200", "250", "300" } ).Concat(getPatern(400, 12, 100)).ToList();
        public static readonly List<string> vdummy = getPatern(0, 21).Concat(new string[] { "50","100","200","500","1000"}).ToList();
        public static readonly List<string> vdummyNegEnhancer = (new string[] { "100", "50"}).Concat(getPatern(25, 31, -1).Concat(new string[] { "-10", "-15","-30" })).ToList();

        public static readonly List<string> vMaxDist = getPatern(0, 20, 10).Concat(getPatern(200, 17, 25)).ToList();
        public static readonly List<string> vMeanHeight = getPatern(0, 51, 5);
        public static readonly List<string> vTreePyrExistCavernDist = getPatern(-300, 61, 10);
        public static readonly List<string> vUnderground = getPatern(0, 50).Concat(getPatern(50, 10, 5)).Concat(getPatern(100, 10, 20)).Concat(getPatern(300, 29, 25)).ToList();

        public static readonly List<string> vForUnknown = getPatern(0, 10).Concat(getPatern(10, 9, 10)).Concat(getPatern(100, 9, 100)).Concat(getPatern(1000, 9, 1000)).Concat(getPatern(10000, 10, 10000)).ToList();


        public OptionsDict()
        {

            Add(title, new List<string> {
                "If you search for a seed the search starts at the entered. It will continue searching until it reached the selected max amount or found as many you like. After finishing one seed" +
                "it will increase it by the selected cycle step size and continue there. For some quick overview you can also store a picture of the world and a text file containg (nearly) all " +
                "information gathered about the world, the configuration and the current search stats. \n \n"+


                "Here only some notes to the options on top. You can change them as well as you like. Only World Size has some issues in current vanilla version. The mod does not fix that bug. "+
                "If you generated a Large world and after this change it to Small then somehow the generated floating islands miss their chests sometimes. "+
                "So if you generated some Large worlds and decide to search Small now you need to restart Terraria (except you want clouds without chest houses). \n \n"+
                "“Config file name” is the name of your current configuration. If you click the load or save button it loads/saves the configuration file with that name."+
                "They are stored in Terraria \\ModLoader \\TheTerrariaSeedProject \\config close to your world files. Those are text files and you can also edit them by hand if you want. "+
                "You can interchange conditions from positive and negative list and set custom number values which you can't select. But e.g. adding a condition from Phase 3 in Phase 2 or 1 " +
                "does not work. Also adding new conditions which aren't in any list won't work in most cases. If you edit by hand do some backup copy. It's not very bug safe at this time. \n \n"+
                "Some handy note for the starting seed input. You can end that number also with M or m (million) to add 6 times 0, or '.' for 5 times 0 (',' works too). The final value need to be " +
                "between 0 and 2147483647. If you enter something else it sets it to a value in between. There is also some limited editing during world generation (it does not updated current search conditions). " +
                "Pressing the bin button there sets settings back to those you are searching for.\n \n" +
            

                WorldInformation.randomInverse+": Random is not true random in vanilla Terraria. Each seed has his predefined evil type. No matter how often you generate the seed it will always be the same. If you select '" + WorldInformation.randomInverse +"' " +
                "it will use the opposite evil type of his normal evil type. \n \n" 
                +
                "Click at '"+SelectableList.omitRare+"' and '"+SelectableList.name+"' for more information."
                
            });

            //world information
            Add(WorldInformation.worldSize, new List<string>() { "Small", "Medium", "Large" });
            Add(Bug.worldSizeBugMinMed, new List<string>() { "# vanilla buggy for Small now", "Medium", "Large" });
            Add(Bug.worldSizeBugMinLarge, new List<string>() {"# vanilla buggy for smaller now", "Large" });

            Add(WorldInformation.evilType, new List<string>() { "Corruption", "Crimson", "Random", OptionsDict.WorldInformation.randomInverse });
            Add(WorldInformation.difficulty, new List<string>() { "Normal", "Expert" });

            //config
            Add(Configuration.searchSeedNum, vNumSearch);
            Add(Configuration.stopSearchNum, vNumSearch);
            Add(Configuration.stepSize, vStepSize);

            Add(Configuration.storeMMPic, new List<string>() { "Off", "For each in phase 3", "For each in phase 3 + item info", "only for stored worlds", "only for stored worlds + item info" });
            Add(Configuration.storeStats, new List<string>() { "Off", "For each in phase 3", "only for stored worlds"});


            //phase 1
            Add(Phase1.title, new List<string> { "In this phase it checks how many pyramids a seed can have, which ores it has, which dungeon color/side, which moon type and the location of" +
                "the hardmode hallow biome spread.\n " +
                "If you are looking for pyramids the option '"+Phase1.pyramidsPossible+"' is very important for speed up the search. The maximal number of possible pyramids is determined in this world " +
                "gen step. The seed value need to be at least the value you selected to pass that phase. \n" +
                "E.g. if you are looking for a seed with at least 2 pyramids but at the current world generation step "+
                "it only computes a maximal number of 1, you don't need to generate further. This map seed can only have "+
                "zero or one pyramid in later world generation steps. So you can skip other steps and continue with next seed.\n "+
                "If a map seed has a higher value of maximal pyramid count, also the chance for pyramids it actually has increases. So you can tune your search here with a higher number than needed. E.g. If you enter 4 here, it will skip all seeds with a chance 3 or less. You might miss some worlds with 2 or 3 pyramids but will be faster in finding a seed which actually has 2 pyramids.\n "+
                "Out of experiments it is recommended that this value should not be higher than 6 for small, 7/8 for medium and 8/9 for large maps or two higher than the number of pyramids you are looking for. Because a higher value is also rarer in most cases (0(1) also rarer than 1(2)).\n "+
                "For a small map with value 6 you need to skip in mean close to 1000 seeds. At the right panel a distribution is shown. It shows a correlation between chance of a seed to have a number of Pyramids vs. how many it actually has. To save space it's written in a short form. " +
                "E.g. 23 means at least 3 with 2 zeros behind, so over 300. At the end of a search it stores a file names lastStats.txt with normal number distribution in Terraria \\ModLoader \\TheTerrariaSeedProject folder close to your world files. Also stored in seed's stats file if option '"+Configuration.storeStats +"' active."+
                "\n\n So far small maps with 4 pyramids, medium with 5 and large with 6 are known.\n \n"+
                "Challenge: Can you find more? Good luck!" +
                "\n "+
                "Other phase 1 options are checked before pyramid chance and so very fast. To enter Phase 2 all conditions (ores, moon, pyramid chance, dungeon) need to be true. \n" +
                "In current vanilla version the hallow biome spread after defeating Wall of Flesh is always at the left side for pink dungeons and at the right side for blue dungeons. So some combinations are " +
                "impossible. \n \n" +
                "For advanced users: If you want to tune it even more you can use the option '"+Phase1.boost+"'. With this you can higher the chance to get a high number of possible pyramid spots and so also higher the" +
                "chance to get more pyramids. Furthermore the same value also has a direct impact at the number of living wood trees, Bee Hives, Granite and Marble biomes. It also has some minor influence " +
                "at lakes, dungeon position, sand generation and some more. This sand generation higher the chance for possible pyramid spots and so also for pyramids. A higher value will result in a " +
                "higher count of those mentioned above but too high value will limit the variation and can have negative impact as well. E.g. it seem to increase the chance of a dungeon placed in ocean (more testing needed). " +
                "Depending at world size and condition you are looking for there is some maximal value. Going higher than this will have no influence in this count. It is the internal amount of " +
                "possible values for that structure. For convenience it is multiplied by 10. E.g. a small world can have 0, 1 or 2 living trees, so 3 possibilities, times 10 = 30. If you select this value " +
                "the mod will divide 10 by this value and subtract it from one: 1-(10:30) = ratio (about 0.6667). For each seed with a value higher (or equal) than this ratio it is guaranteed that the small world which is" +
                "generates has two living trees. Or to be correct, at least world gen tries it. Sometimes it can't find a valid place. If you are also OK with one living tree, so you are OK with 2 out of those " +
                "3 possibilities divide 30 by this (30:2=15, ratio would be about 0.333). Each seed with" +
                "a value higher (or equal) than this will generate a small world with 1 or 2 living trees (or at least tries). \n" +
                "Here are those known possibilities counts (the real value may differ if e.g. multiplied by a value): \n \n" +
                "living trees : small 3, medium 3, large 6 \n" +
                "bee hives: small 3, medium 3, large 6 \n" +
                "marble: 5 \n" +
                "granite: 6 \n" +
                "lakes: small 19, medium 30, large 40 \n" +
                "sand spots: small 7, medium 11, large 15 \n \n" +
                "So in short: you are doing good selecting a value of 60, or if you are at pyramid hunting you can go for 70 (small), 110 (medium), 150 (large world). With this pyramids (should) have the best "+
                "start conditions. Only if you want more lakes or" +
                " do some tests about other effects of this value some greater values are needed. \n" +
                "As the name implies it's an experimental feature. It may not work anymore with futher Terraria update. Also there might be some mistakes in those values. Have fun testing."


                });




            //phase 2
            Add(Phase2.title, new List<string> {"At this step world gen generated all pyramids this map actually has. It also placed the pyramid chest, the living tree, including chests and the " +
                "floating islands(but not the houses on top). Also Dungeon(with chests), evil, beach and some more. \n" +
                "Even if it had a chance to get 8 pyramids you can be unlucky and get zero of them placed. You can skip that seed if it has less than wanted.\n"+
                "So you can save time in not generating all world generation steps if the seed can’t even fulfill those early conditions. \n"+
                "The conditions are separated in a list of positive and negative conditions. Positive could be e.g.the number of Pyramids and and the number of living trees, negative " +
                "e.g.the amount of jungle grass which can get infected by evil biome or the distances to the Dungeon from mid (spawn point unknown in that phase). \n"+
                "All conditions you selected in a positive list need to be true. That means option1 need to be greater or equal than the selected value AND option2 need to be greater or equal AND " +
                "option3 .. \n"+
                "If all is true the seed gets one point. \n"+
                "If any option of the negative list is greater (not equal) than the value selected the seed gets a point subtracted. So it gets - 1 point if negative list option1 is true OR " +
                "option2 is true OR option3 … (Each element can reduce points by 1. So with 3 elements it could be -3 in total)\n"+
                "To get qualified for Phase 3(full world generation) the seed needs to get at least one point here. \n"+
                "If you are not limited to one list of options you can add another positive list which benefit to the seed points. E.g. you are looking for a map which has at least two pyramids, " +
                "a Sandstorm in a Bottle and a "+
                "Flying Carpet(in positive list 1) but you are also fine if the seed has 3 pyramids and only Sandstorm in a Bottle(in positive list 2). If you are lucky now and the seed generates " +
                "a map which fulfills both, "+
                "so has 3 pyramids, a Sandstorm in a Bottle and a Flying Carpet the seed gets two points. Sadly it also fulfills two points of your negative list and 2 points gets subtracted, " +
                "so the total points in this phase "+
                "are 0 and it gets not qualified for Phase 3.The seed search starts with next seed in phase 1. \n If you add nothing here this phase gets skipped and it starts Phase 3 with 2 " +
                "points (one from first phase +one "+
                "it got for free in that phase). \n  " +
                "Some conditions contain 'mid'. At this sate the exact spawn position is unknown. But chances are high it is close to mid of world. For this reason distances are measured to mid of " +
                "world (in unit blocks) " +
                "Update: -Tool- dummies, they change the current phase points by selected value. You can use those e.g. if you selected 5 objects in the negative list (and nothing in positive) " +
                "but you are fine if only 4 of them are true (<). For this "+
                "add this tool to positive list and select 2 as value. This will give always 2 points. Or if you search for more than one at the same time and value each different (e.g. 3<5, " +
                "3+4>5, 6>5, 6+3+4>5). \n "             
                });

            //Only “Beach penalty” need some further notes. This scores both beaches if they contain gaps in sand or high "+
            //"cliffs. The penalty is low if the beach is flat or slowly increases ground level(in ocean to world mid direction). \n A high beach penalty means that the beach don’t look very well, has gaps without sand or the " +
            //"ocean is far above the bordering areas.Such a high cliff will result in a Beach penalty of over 9000 in most cases. It only considers the world structure, so e.g. evil biome tiles are not part of that penalty but " +
            //"the deep holes of the corruption biome have a high negative impact. Near Dungeon also has negative impact, near living tree does not. Try around a find a value you like. \n \n" +
            //"-Dungeon farm spot detection. You can search for the number of spot where all 3 Dungeon Wall intersect each other (cyan at mini map) or a line which contain each dungeon wall kind with at least 16 tiles width (dark cyan at mini map)."


            //phase 3
            Add(Phase3.title, new List<string> { "In Phase 3 full world generation was done. You can make conditions for alls stuff left. All kind of chests are placed, Enchanted Sword Shrine are settled and the exact Spawn "+
                "location is now defined. As in Phase 2 you have again positive list which contain conditions which need to be true (equal or greater than selected value). For each list the seed can get a point if all containing "+
                "conditions of that specific list are fulfilled. The negative list reduces again the points by one for any condition listed there. For more details look Phase 2 description."+
                "The world qualifies to get accepted if it has a total of 3 points or more here. It already got 1 point in Phase 1, and at least one point in Phase 2.If you added more than one positive list in Phase 2 it can start "+
                "up with more than two points. If the negative options of this phase don’t reduce it again below 3 a seed can get accepted without fulfilling a positive list here. If you want to prohibit that " +
                "change “Takeover positive” to “Start new”. With this the points the evaluation starts with in Phase 3 can't be higher than 2 and the seed is forced to get at least one additional point here. \n "+
                "\n If you don’t add anything in that Phase it gets skipped and the seed is accepted. \n \n After that the world file "+
                "is created. The name of the world file is like you selected in options on top. Currently the vanilla version of Terraria has a bugged world generation. One seed can generate slightly different maps for some seeds. "+
                "With this one variation might get accepted and another is not. This mod does not fix the vanilla version "+
                "bug! It can still happen that a valid variation gets skipped. Also a generated world can also depend at the world you created before. Some seeds have no variation at all, some only in vanilla and some only in the "+
                "Tmod-loader version (without any mods). But in most cases you can generate an equal map in vanilla and the Tmod-loader version + this mod. So if you find some amazing seed best practice before sharing with the world "+
                "is trying to generate it again with vanilla version. First, start up vanilla Terraria again and generate that seed and check if equal, if not, create it again (without restarting Terraria!or creating another world). "+
                "Most seeds are equal all the time or interchange in between variation 1 and variation 2 (1, 2, 1, 2, 1..) with some additional tiny changes sometimes. Those tiny changes are mostly Enchanted Swords Shrines, Mahogany " +
                "Trees with their chests and Minecart Tracks and some more not that important. So the condition of a seed having an active Enchanted Sword Shrine is not that reliable all times. But in that case you can generate " +
                "the seed again and might have one at another location. \n Some vanilla fix in next update would be nice."
                });

            //"As some special kind of condition in this phase you have the option “Score”. That computes a total fantasy score of that seed. It takes e.g. into account how many rare items it has, how many pyramids, clouds, "+
            //"living trees, enchanted swords(near spawn?), the beach structure, the location of evil tiles, were the Dungeon is placed, if it has some rare structures and many many more. It tries to be an objective rating, " +
            //"so e.g.it does not care about the dungeon color. It’s neither mathematical correct nor containing all stuff nor right in all cases but the chances you get a nice map are higher with a high Score value.It’s " +
            //"currently in beta state, haven’t updated for a while but it provides some meaningful values. 

            //This mod tries to spot that and if so generate the map multiple times until it fulfills the conditions again. 
            //"Update: Pathlengths to cavern entries: '40% cavern layer' means a cavern which reaches deep to 40% of the cavern layer and therefor the pathlength from spawn to the entrance of that layer. "


            Add(Phase3.continueEvaluation, new List<string> { Phase3.continueEvaluatioTakeOverTag, Phase3.continueEvaluationResetTag });
            Add(Phase3.continueEvaluatioTakeOverTag, vEmpty);
            Add(Phase3.continueEvaluationResetTag, vEmpty);

            //General

            //next is outdated
            Add(GeneralOptions.title, new List<string> { "Here you can setup some addtional search settings. E.g. the world name of successfuly generated seeds. Besides the seed as number and the map name you given, you can also add "+
                "some short form of the content the map has and a computed fantasy score (in beta state, high score -> many rare/good stuff). Besides that it also adds, if it found some very rare stuff insdide, E.g. if you spawn in sky"+
                " or there is a Enchanted Sword very near to spawn. Besides the conditions you added, it always searches for rare stuff. If you don't want that, you can also disable it here."});

            Add(GeneralOptions.naming, new List<string> { "World name", "Seed", "Size, evil type, difficulty", "Content in short", "Fantasy score", "Rares" });
            Add("World name", vEmpty);
            Add("Seed", vEmpty);
            Add(GeneralOptions.sizeEvilDiff, vEmpty);
            Add(GeneralOptions.contentShort, vEmpty);
            Add(GeneralOptions.fantasyScore, vEmpty);
            Add("Rares", vEmpty);
            Add(GeneralOptions.omitRare, new List<string> {
                GeneralOptions.omitRareAll,
                GeneralOptions.omitBaCRare,
                GeneralOptions.omitBadRare,
                GeneralOptions.omitCommonRare,
                "Omit Chest duplication Glitch",
                "Omit "+Phase2.dungeonStrangePos,
                "Omit Dungeon in Snow Biome",
                "Omit Dungeon far above surface",
                "Omit Dungeon below ground",
                "Omit Floating island cabin in Dungeon",
                "Omit Pre Skeletron Dungeon Chest Risky",
                "Omit Pre Skeletron Dungeon Chest Grab",
                "Omit Biome Item in normal Chest",
                "Omit No Ocean",
                "Omit Many Pyramids",
                "Omit Near Enchanted Sword",
                "Omit Spawn in Sky",
                "Omit Enchanted Sword near Tree",
                "Omit Near Enchanted Sword near Tree",
                "Omit Enchanted Sword near Pyramid",
                "Omit Near Enchanted Sword near Pyramid",
                "Omit Very Near Enchanted Sword",
                "Omit Spawn in Jungle biome",
                "Omit Spawn in Snow biome",
                "Omit Floating Island without chest",
                "Omit "+ Phase3.openTemple,
                "Omit Detonator at surface",
                "Omit "+ Phase3.greenPyramid,
                "Omit "+ Phase3.lonelyJungleTree,
                "Omit Shadow Chest item in normal chest",
                "Omit Mushroom Biome above surface",
                "Omit Spawn in Marble or Granite biome",
                "Omit All Paintings",
                "Omit "+ Phase3.allChestItemsNoCraftFish,
                
            });
                                    

            foreach (var rare in this[GeneralOptions.omitRare])            
                Add(rare, vEmpty);
            


            //phase 1 content
            Add(Phase1.pyramidsPossible, v0to15);
            Add(Phase1.copperTin, new List<string> { "Random", "Copper", "Tin"}); //foreach (var elem in this[Phase1.copperTin]) Add(elem, vEmpty);
            Add(Phase1.ironLead, new List<string> { "Random", "Iron", "Lead" });
            Add(Phase1.silverTungsten, new List<string> { "Random", "Silver", "Tungsten" }); //foreach (var elem in this[Phase1.silverTungsten]) Add(elem, vEmpty);
            Add(Phase1.goldPlatin, new List<string> { "Random", "Gold", "Platinum" }); //foreach (var elem in this[Phase1.goldPlatin]) Add(elem, vEmpty);
            Add(Phase1.moonType, new List<string> { "Random", "White", "Orange", "Green" }); //foreach (var elem in this[Phase1.moonType]) Add(elem, vEmpty);
            Add(Phase1.hallowSide, new List<string> { "Random", "Jungle side", "Snow/Dungeon side"});
            Add(Phase1.dungeonWallColor, new List<string> { "Random", "Blue", "Green", "Pink"});
            Add(Phase1.dungeonSide, new List<string> { "Random", "Left", "Right"});
            Add(Phase1.boost, vboost);

            //phase 2 content
            Add(Phase2.positive, new List<string> {
                "Number of Pyramids",
                "Number of Clouds",
                "Number of Living Trees",
                "Pyramid Bottle",
                "Pyramid Carpet",
                "Pyramid Mask",
                "Max open air pyramid surface",
                "Max pyramid height",
                "Max pyramid tunnel height",
                "Max pyramid total height",
                Phase2.maxPyrExitCavDist,
                "Ocean Pyramid",
                "Tree Chest",
                "Tree Chest Loom",
                "Trees near mid",
                "Tree chests near mid",
                "Tree to cavern layer",
                "Tree to cavern layer near mid",
                "Tree close to cavern layer",
                "Tree close to cavern layer near mid",
                "Max Living Tree Size",
                "Min Living Tree Size",
                "Max Living Tree root Size",
                "Max Living Tree total Size",
                Phase2.maxTreeExitCavDist,
                "Lake near mid (guess)",
                Phase2.dungeonGoodPos,
                Phase2.dungeonStrangePos,
                "Dungeon in Snow Biome",
                "Dungeon tiles above surface",
                "Dungeon below ground tree",
                "Pre Skeletron Dungeon Chest Risky",
                "Pre Skeletron Dungeon Chest Grab",
                "Pre Skeletron Dungeon Chest Any",
                "Pre Skeletron Muramasa Chest reachable",
                "Pre Skeletron Cobalt Shield Chest reachable",
                "Pre Skeletron Shadow Key Chest reachable",
                "Pre Skeletron Golden Key Grab",
                "Pre Skeletron Golden Key Risky",
                "Pre Skeletron Golden Key Any",
                "Water Bolt before Skeletron",                
                "Water Bolt",
                "Muramasa",
                "Cobalt Shield",
                "Valor",
                "Bone Welder",
                "Alchemy Table",
                "Dungeon Wall count",
                "All Dungeon Walls",
                "Dungeon farm spot",
                "Dungeon farm spot 3Wall inters.",
                "Dungeon farm spot 3Wall in line",
                "All Dungeon Items",                
                "Beach penalty mean",
                "Beach penalty max",
                "Nearest Evil left Ocean",
                "Nearest Evil right Ocean",                
                "Evil only one side",                               
                "Evil Tiles for Jungle Grass",
                "Evil Tiles for Sand",
                "Evil Tiles for Ice",
                "No Ocean",
                "Snow biome surface overlap mid",
                "Jungle biome surface overlap mid",
                "Jungle biome distance to mid",
                "Snow biome distance to mid",
                "Evil biome distance to mid",
                "Surface average height (aprox.)",
                "Surface height (sqrt) variance",
                "Surface max-min height",
                "Underground layer height",
                "neg. Distance Tree to mid", 
                "neg. Distance Tree Chest to mid",
                "neg. Distance Cloud to mid", 
                "neg. Distance Pyramid to mid", 
                "neg. Distance Dungeon to mid", 
                "neg. Distance ShadowOrb/Heart to mid",
                "neg. Distance Lake to mid (guess)",
                "neg. Evil Tiles for Jungle Grass",
                "neg. Evil Tiles for Sand",
                "neg. Evil Tiles for Ice",
                "Ice surface more than half not evil",
                OptionsDict.Tools.conditionConnector,
                OptionsDict.Tools.dummyPlus
                });
            //legacy
            //"Green Dungeon Walls",
             //   "Blue Dungeon Walls",
             //   "Pink Dungeon Walls",


            Add(Phase2.negative, new List<string> {                     
                "Evil Tiles for Jungle Grass",
                "Evil Tiles for Sand",
                "Evil Tiles for Ice",
                "Ice surface evil",
                "Ice surface more than half evil",
                "Distance Tree to mid",
                "Distance Tree Chest to mid",
                "Distance Cloud to mid",
                "Distance Pyramid to mid",
                "Distance Dungeon to mid",
                "Distance ShadowOrb/Heart to mid",
                "Distance Lake to mid (guess)",
                Phase2.dungeonStrangePos,
                "Dungeon tiles above surface",
                "Dungeon in Snow Biome",
                "Beach penalty mean",
                "Beach penalty max",
                "Has evil Ocean",
                "Has evil Dungeon Ocean",
                "Has evil Jungle Ocean",
                "No Ocean",
                "Dungeon Wall count",
                "Snow biome surface overlap mid",
                "Jungle biome surface overlap mid",
                "Jungle biome distance to mid",
                "Snow biome distance to mid",
                "Evil biome distance to mid",
                "Surface average height (aprox.)",
                "Surface height (sqrt) variance",
                "Surface max-min height",
                "Max Living Tree Size",
                "Min Living Tree Size",
                Phase2.maxTreeExitCavDist,
                Phase2.maxPyrExitCavDist,
                "Underground layer height",                
                OptionsDict.Tools.conditionConnector1,
                OptionsDict.Tools.conditionConnector2,
                OptionsDict.Tools.conditionConnector3,
                OptionsDict.Tools.conditionConnector4,
                OptionsDict.Tools.conditionConnector5,
                OptionsDict.Tools.dummyNegEnhancer,
                OptionsDict.Tools.dummyNeg
            });

            Add("Number of Pyramids", v0to10);
            Add("Number of Clouds", v0to9);
            Add("Number of Living Trees", v0to5);
            Add("Pyramid Bottle", v0to5);
            Add("Pyramid Carpet", v0to5);
            Add("Pyramid Mask", v0to5);
            Add("Max open air pyramid surface", v0to200_2);            
            Add("Max pyramid height", v0to200_2);
            Add("Max pyramid tunnel height", v0to500_5);
            Add("Max pyramid total height", v0to500_5);
            Add(Phase2.maxPyrExitCavDist, vTreePyrExistCavernDist);           
            Add("Ocean Pyramid", v0to2);
            Add("Tree Chest", v0to5);
            Add("Tree Chest Loom", v0to5);
            Add("Trees near mid", v0to5);
            Add("Tree chests near mid", v0to5);
            Add("Tree to cavern layer", v0to5);
            Add("Tree to cavern layer near mid", v0to5);
            Add("Tree close to cavern layer", v0to5);
            Add("Tree close to cavern layer near mid", v0to5);
            Add("Max Living Tree Size", v0to120);
            Add("Min Living Tree Size", v0to120);
            Add("Max Living Tree root Size", v0to500_5);
            Add("Max Living Tree total Size", v0to500_5_700_10);
            Add(Phase2.maxTreeExitCavDist, vTreePyrExistCavernDist);
            Add("Lake near mid (guess)", v0to1);
            Add(Phase2.dungeonGoodPos, v0to1);
            Add(Phase2.dungeonStrangePos, v0to1);
            Add("Dungeon in Snow Biome", v0to1);
            Add("Dungeon tiles above surface", v0to120);    
            Add("Dungeon below ground tree", v0to1);
            Add("Pre Skeletron Dungeon Chest Risky", v0to10);
            Add("Pre Skeletron Dungeon Chest Grab", v0to10);
            Add("Pre Skeletron Dungeon Chest Any", v0to10);
            Add("Pre Skeletron Muramasa Chest reachable", v0to5);
            Add("Pre Skeletron Cobalt Shield Chest reachable", v0to5);
            Add("Pre Skeletron Shadow Key Chest reachable", v0to5);
            Add("Pre Skeletron Golden Key Grab", v0to5);
            Add("Pre Skeletron Golden Key Risky", v0to5);
            Add("Pre Skeletron Golden Key Any", v0to5);
            Add("Water Bolt before Skeletron", v0to5);            
            Add("Water Bolt", v0to10);
            Add("Muramasa", v0to10);
            Add("Cobalt Shield", v0to10);
            Add("Valor", v0to10);
            Add("Bone Welder", v0to10);
            Add("Alchemy Table", v0to10);
            //Add("Green Dungeon Walls", vDungeonWalls);
            //Add("Blue Dungeon Walls", vDungeonWalls);
            //Add("Pink Dungeon Walls", vDungeonWalls);
            Add("Dungeon Wall count", vDungeonWalls);
            Add("All Dungeon Walls", vDungeonALLWalls);            
            Add("Dungeon farm spot", v0to10);
            Add("Dungeon farm spot 3Wall inters.", v0to10);
            Add("Dungeon farm spot 3Wall in line", v0to10);
            Add("All Dungeon Items", v0to1);            
            Add("Nearest Evil left Ocean", vNearEvilOcean);
            Add("Nearest Evil right Ocean", vNearEvilOcean);
            Add("Evil only one side", v0to1);
            Add("Snow biome surface overlap mid", vNumTiles);
            Add("Jungle biome surface overlap mid", vNumTiles);
            Add("Jungle biome distance to mid", vDistanceNeg);
            Add("Snow biome distance to mid", vDistanceNeg);
            Add("Evil biome distance to mid", vDistance);
            Add("Surface average height (aprox.)", vMeanHeight);
            Add("Surface height (sqrt) variance", v0to120);
            Add("Surface max-min height", vMaxDist);



            Add("Beach penalty mean", vBeachPenalty);
            Add("Beach penalty max", vBeachPenalty);            
            Add("No Ocean", v0to2);
            Add("Evil Tiles for Jungle Grass", vEvilTiles);
            Add("Evil Tiles for Sand", vEvilTiles);
            Add("Evil Tiles for Ice", vEvilTiles);
            Add("Ice surface evil", vEvilTiles);
            Add("Ice surface more than half evil", v0to1);
            Add("Ice surface more than half not evil", v0to1);
            Add("Distance Tree to mid", vDistance);
            Add("Distance Tree Chest to mid", vDistance);
            Add("Distance Cloud to mid", vDistance);
            Add("Distance Pyramid to mid", vDistance);
            Add("Distance Dungeon to mid", vDistance);
            Add("Distance ShadowOrb/Heart to mid", vDistance);
            Add("Distance Lake to mid (guess)", vDistance);

            Add("neg. Distance Tree to mid", vDistanceShortNeg);
            Add("neg. Distance Tree Chest to mid", vDistanceShortNeg);
            Add("neg. Distance Cloud to mid", vDistanceShortNeg);
            Add("neg. Distance Pyramid to mid", vDistanceShortNeg);
            Add("neg. Distance Dungeon to mid", vDistanceShortNeg);
            Add("neg. Distance ShadowOrb/Heart to mid", vDistanceShortNeg);
            Add("neg. Distance Lake to mid (guess)", vDistanceShortNeg);


            Add("neg. Evil Tiles for Jungle Grass", vEvilTilesNeg);
            Add("neg. Evil Tiles for Sand", vEvilTilesNeg);
            Add("neg. Evil Tiles for Ice", vEvilTilesNeg);
            

            Add("Has evil Ocean", v0to2);
            Add("Has evil Dungeon Ocean", v0to1);
            Add("Has evil Jungle Ocean", v0to1);
            Add("Underground layer height", vUnderground);


            Add(OptionsDict.Tools.dummyPlus, vdummy);
            Add(OptionsDict.Tools.dummyNeg, vdummy);
            Add(OptionsDict.Tools.dummyNegEnhancer, vdummyNegEnhancer);
            Add(OptionsDict.Tools.conditionConnector, v0to20);
            Add(OptionsDict.Tools.conditionConnector1, v0to50);
            Add(OptionsDict.Tools.conditionConnector2, v0to50);
            Add(OptionsDict.Tools.conditionConnector3, v0to50);
            Add(OptionsDict.Tools.conditionConnector4, v0to50);
            Add(OptionsDict.Tools.conditionConnector5, v0to50);


            //phase 3 content
            Add(Phase3.positive, new List<string> {
                "Enchanted Sword Shrine",
                "Enchanted Sword",
                "Near Enchanted Sword",
                "Enchanted Sword near Pyramid or Tree",
                "Very Near Enchanted Sword",
                "Near Sunflower",
                "Near Altar",
                "Near Spider Web count",
                "Near Mushroom Biome count",
                "Near Chest",
                "Near Tree",
                "Near Tree Chest",
                "Near Cloud",
                "Cloud Chest",
                "Cloud Ballon",
                "Cloud Starfury",
                "Cloud Horseshoe",
                "Cloud Sky Mill",
                "All Cloud Items",
                "Bee Hives",
                "High Hive",
                "Sharpening Station",
                "Flower Boots",
                "Staff of Regrowth",
                "Anklet of the Wind",
                "Feral Claws",
                "Fiberglass Fishing Pole",
                "Seaweed Pet",
                "Living Mahogany Wand",
                "Honey Dispenser",
                "Flurry Boots",
                "Blizzard in a Bottle",
                "Ice Machine",
                "Snowball Cannon",
                "Ice Boomerang",
                "Ice Blade",
                "Ice Skates",
                "Fish Pet",
                "Ice Mirror",
                "Magic Mirror",
                "Band of Regeneration",
                "Shoe Spikes",
                "Lava Charm",
                "Water Walking Boots",
                Phase3.allChestItemsNoCraftFish,
                "Aglet",
                "Dart Trap",
                "Super Dart Trap",
                "Flame Trap",
                "Spiky Ball Trap",
                "Spear Trap",
                "Geyser",
                "Detonator",
                "Detonator at surface",
                "Underground Distance to spawn",
                "Cavern Distance to spawn",
                "Meteorite Bar unlocked",
                "Number different Paintings",
                "Number Paintings",
                "Different functional noncraf. Statues",
                "Number functional noncraf. Statues",
                "Different noncraf. Statues",
                "Number noncraf. Statues",
                "Nearest Teleportation Potion count",
                "Free ShadowOrb/Heart",
                "Free cavern to mid Jungle",
                "Free cavern to deep Jungle",
                "Jungle cavern not blocked by structure",
                "Dungeon Distance",                
                "Temple door distance",
                "Temple door horizontal distance",
                "Temple Tile horizontal distance",
                "Temple Tile vertical distance",
                "neg. Temple door distance",
                "neg. Temple door horizontal distance",
                "neg. Temple Tile horizontal distance",
                "neg. Temple Tile vertical distance",
                "Temple at player side of jungle (%)",
                "Temple at ocean side of jungle (%)",
                "Temple at height (%)",
                "Temple at depth (%)",
                Phase3.openTemple,
                "Mushroom Biome above surface count",
                "neg. Pathlength to Temple Door",
                "neg. Pathlength to Boots",
                "neg. Pathlength to Iron/Lead Bar",
                "neg. Pathlength to 10 Iron/Lead Bar Chest",
                "neg. Pathlength to Gold/Platinum Bar",
                "neg. Pathlength to Bomb",
                "neg. Pathlength to Dynamite",
                "neg. Pathlength to 2nd Dynamite",
                "neg. Pathlength to Gravitation Potion",
                "neg. Pathlength to Crystal Heart",
                "neg. Pathlength to Jester's Arrow",
                "neg. Pathlength to Suspicious Looking Eye",
                "neg. Pathlength to Snowball Cannon",
                "neg. Pathlength to Teleport Potion",
                "neg. Pathlength to 2 Teleport Potion Chest",
                "neg. Pathlength to Meteorite Bar",
                "neg. Pathlength to Enchanted Sword",
                "neg. Pathlength to Altar",
                "neg. Pathlength to Bee Hive",
                "neg. Pathlength to Boomstick",
                "neg. Pathlength to Slime Staute",
                "neg. Pathlength to Shark Staute",
                "neg. Pathlength to Anvil",
                "neg. Pathlength to Ruby",
                "neg. Pathlength to Cloud in a Bottle",
                "neg. Pathlength to 2 Herb Bag Chest",
                "neg. Pathlength to Extractinator",
                "neg. Pathlength to Detonator",
                "neg. Pathlength to Magic/Ice Mirror",
                "neg. Pathlength to Chest",
                "neg. Pathlength to Tree Chest",
                "neg. Pathlength to free ShadowOrb/Heart",
                "neg. Pathlength into cavern layer",
                "neg. Pathlength into 40% cavern layer",
                "neg. Pathlength to 40% cavern entrance",
                "neg. Tiles to mine for 40% cavern",
                "neg. Pathlength to cavern entrance to mid of Jungle",
                "neg. Tiles to mine for mid Jungle cavern",
                "neg. Pathlength to cavern entrance to deep Jungle",
                "neg. Tiles to mine for deep Jungle cavern",
                "Floating Island without chest",
                "Nearest Altar Dungeon beach",
                "Nearest Altar Jungle beach",
                "Spawn in Snow biome",
                "Spawn in Jungle biome",
                "Spawn in Sky",
                "Spawn in Evil biome",
                "Wooden Chest",
                "Wooden Chest Dungeon",
                "Gold Chest",
                "Gold Chest locked",
                "Ice Chest", 
                "Ivy Chest", 
                "Water Chest", 
                "Skyware Chest", 
                "Web Covered Chest", 
                "Shadow Chest", 
                "Lihzahrd Chest", 
                "Living Wood Chest", 
                "Score",
                OptionsDict.Tools.conditionConnector,
                OptionsDict.Tools.dummyPlus
                });
            Add(Phase3.negative, new List<string> {
                "Hermes Flurry Boots Distance", 
                "Underground Distance to spawn",
                "Cavern Distance to spawn",
                "Dungeon Distance",
                "Temple door distance",
                "Temple door horizontal distance",
                "Temple Tile horizontal distance",
                "Temple Tile vertical distance",
                "neg. Temple door distance",
                "neg. Temple door horizontal distance",
                "neg. Temple Tile horizontal distance",
                "neg. Temple Tile vertical distance",
                "Temple at player side of jungle (%)",
                "Temple at ocean side of jungle (%)",
                "Temple at height (%)",
                "Temple at depth (%)",
                "Pathlength to Temple Door",
                "Pathlength to Boots",
                "Pathlength to Iron/Lead Bar",
                "Pathlength to 10 Iron/Lead Bar Chest",
                "Pathlength to Gold/Platinum Bar",
                "Pathlength to Bomb",
                "Pathlength to Dynamite",
                "Pathlength to 2nd Dynamite",
                "Pathlength to Gravitation Potion",
                "Pathlength to Crystal Heart",
                "Pathlength to Jester's Arrow",
                "Pathlength to Suspicious Looking Eye",
                "Pathlength to Snowball Cannon",
                "Pathlength to Teleport Potion",
                "Pathlength to 2 Teleport Potion Chest",
                "Pathlength to Meteorite Bar",
                "Pathlength to Enchanted Sword",
                "Pathlength to Altar",
                "Pathlength to Bee Hive",
                "Pathlength to Boomstick",
                "Pathlength to Slime Staute",
                "Pathlength to Shark Staute",
                "Pathlength to Anvil",
                "Pathlength to Ruby",
                "Pathlength to Cloud in a Bottle",
                "Pathlength to 2 Herb Bag Chest",
                "Pathlength to Extractinator",
                "Pathlength to Detonator",
                "Pathlength to Magic/Ice Mirror",
                "Pathlength to Chest",
                "Pathlength to Tree Chest",                
                "Pathlength to free ShadowOrb/Heart",
                "Pathlength into cavern layer",
                "Pathlength into 40% cavern layer",
                "Pathlength to 40% cavern entrance",
                "Tiles to mine for 40% cavern",
                "Pathlength to cavern entrance to mid of Jungle",
                "Tiles to mine for mid Jungle cavern",
                "Pathlength to cavern entrance to deep Jungle",
                "Tiles to mine for deep Jungle cavern",
                "Floating Island without chest",
                "Nearest Altar Dungeon beach",
                "Nearest Altar Jungle beach",
                "Score",
                OptionsDict.Tools.dummyNegEnhancer,
                OptionsDict.Tools.dummyNeg
            });

            Add("Enchanted Sword Shrine", v0to5);
            Add("Enchanted Sword", v0to10);
            Add("Near Enchanted Sword", v0to5);
            Add("Enchanted Sword near Pyramid or Tree", v0to5);
            Add("Very Near Enchanted Sword", v0to5);
            Add("Near Altar", v0to10);
            Add("Near Spider Web count", vNumTiles);
            Add("Near Mushroom Biome count", vNumTiles);            
            Add("Near Chest", v0to20);
            Add("Near Tree", v0to5);
            Add("Near Tree Chest", v0to5);
            Add("Near Cloud", v0to5);
            Add("Bee Hives", v0to20);
            Add("High Hive", v0to10);
            Add("Cloud Chest", v0to9);
            Add("Cloud Ballon", v0to9);
            Add("Cloud Starfury", v0to9);
            Add("Cloud Horseshoe", v0to9);
            Add("Cloud Sky Mill", v0to9);
            Add("All Cloud Items", v0to1);
            Add("Flower Boots", v0to10);
            Add("Staff of Regrowth", v0to10);
            Add("Anklet of the Wind", v0to10);
            Add("Feral Claws", v0to10);
            Add("Sharpening Station", v0to10);            
            Add("Fiberglass Fishing Pole", v0to10);
            Add("Seaweed Pet", v0to10);
            Add("Living Mahogany Wand", v0to10);
            Add("Honey Dispenser", v0to10);
            Add("Flurry Boots", v0to10);
            Add("Blizzard in a Bottle", v0to10);
            Add("Ice Machine", v0to10);
            Add("Snowball Cannon", v0to10);
            Add("Ice Boomerang", v0to10);
            Add("Ice Blade", v0to10);
            Add("Ice Skates", v0to10);
            Add("Fish Pet", v0to10);
            Add("Ice Mirror", v0to10);
            Add("Magic Mirror", v0to10);
            Add("Band of Regeneration", v0to10);
            Add("Shoe Spikes", v0to10);
            Add("Lava Charm", v0to10);
            Add("Water Walking Boots", v0to10);            
            Add(Phase3.allChestItemsNoCraftFish, v0to1);
            Add("Aglet", v0to10);
            Add("Dart Trap", vDartTrap);
            Add("Super Dart Trap", v0to50);
            Add("Flame Trap", v0to20);
            Add("Spiky Ball Trap", vSpikyTrap);
            Add("Spear Trap", vSpearTrap);
            Add("Geyser", vGeyDetoTrap);
            Add("Detonator", vGeyDetoTrap);
            Add("Detonator at surface", v0to5);
            Add("Meteorite Bar unlocked", v0to10);
            Add("Dungeon Distance", vDistanceLong);
            Add("Underground Distance to spawn", vDistanceShort);
            Add("Cavern Distance to spawn", vDistanceShort);
            Add("Number different Paintings", vPaintingsDiff);
            Add("Number Paintings", vPaintingsTotal);
            Add("Different functional noncraf. Statues", getPatern(0,27));
            Add("Number functional noncraf. Statues", vStatuesFuncTotal);
            Add("Different noncraf. Statues", getPatern(0, 30+26+1));
            Add("Number noncraf. Statues", vStatuesTotal);
            Add("Floating Island without chest", v0to1);
            Add("Near Sunflower", v0to20);
            Add("Nearest Teleportation Potion count", v0to5);            
            Add("Free ShadowOrb/Heart", v0to5);
            Add("Free cavern to mid Jungle", v0to1);
            Add("Free cavern to deep Jungle", v0to1);
            Add("Jungle cavern not blocked by structure", v0to1);
            Add(OptionsDict.Phase3.openTemple, v0to1);
            Add("Mushroom Biome above surface count", vNumTiles);

            Add("neg. Pathlength to Temple Door", vPathLengthNeg);
            Add("neg. Pathlength to Boots", vPathLengthNeg);
            Add("neg. Pathlength to Iron/Lead Bar", vPathLengthNeg);
            Add("neg. Pathlength to 10 Iron/Lead Bar Chest", vPathLengthNeg);
            Add("neg. Pathlength to Gold/Platinum Bar", vPathLengthNeg);
            Add("neg. Pathlength to Bomb", vPathLengthNeg);
            Add("neg. Pathlength to Dynamite", vPathLengthNeg);
            Add("neg. Pathlength to 2nd Dynamite", vPathLengthNeg);
            Add("neg. Pathlength to Gravitation Potion", vPathLengthNeg);
            Add("neg. Pathlength to Crystal Heart", vPathLengthNeg);
            Add("neg. Pathlength to Jester's Arrow", vPathLengthNeg);
            Add("neg. Pathlength to Suspicious Looking Eye", vPathLengthNeg);
            Add("neg. Pathlength to Snowball Cannon", vPathLengthNeg);
            Add("neg. Pathlength to Teleport Potion", vPathLengthNeg);
            Add("neg. Pathlength to 2 Teleport Potion Chest", vPathLengthNeg);
            Add("neg. Pathlength to Meteorite Bar", vPathLengthNeg);
            Add("neg. Pathlength to Enchanted Sword", vPathLengthNeg);
            Add("neg. Pathlength to Altar", vPathLengthNeg);
            Add("neg. Pathlength to Bee Hive", vPathLengthNeg);
            Add("neg. Pathlength to Boomstick", vPathLengthNeg);

            Add("neg. Pathlength to Slime Staute", vPathLengthNeg);
            Add("neg. Pathlength to Shark Staute", vPathLengthNeg);
            Add("neg. Pathlength to Anvil", vPathLengthNeg);
            Add("neg. Pathlength to Ruby", vPathLengthNeg);
            Add("neg. Pathlength to Cloud in a Bottle", vPathLengthNeg);
            Add("neg. Pathlength to 2 Herb Bag Chest", vPathLengthNeg);
            Add("neg. Pathlength to Extractinator", vPathLengthNeg);
            Add("neg. Pathlength to Detonator", vPathLengthNeg);
            Add("neg. Pathlength to Magic/Ice Mirror", vPathLengthNeg);
            Add("neg. Pathlength to Chest", vPathLengthNeg);
            Add("neg. Pathlength to Tree Chest", vPathLengthNeg);


            Add("neg. Pathlength to free ShadowOrb/Heart", vPathLengthNeg);
            Add("neg. Pathlength into cavern layer", vPathLengthNeg);
            Add("neg. Pathlength into 40% cavern layer", vPathLengthNeg);
            Add("neg. Pathlength to 40% cavern entrance", vPathLengthNeg);
            Add("neg. Tiles to mine for 40% cavern", vPathLengthNeg);
            Add("neg. Pathlength to cavern entrance to mid of Jungle", vPathLengthNeg);
            Add("neg. Tiles to mine for mid Jungle cavern", vPathLengthNeg);
            Add("neg. Pathlength to cavern entrance to deep Jungle", vPathLengthNeg);
            Add("neg. Tiles to mine for deep Jungle cavern", vPathLengthNeg);

            Add("Nearest Altar Dungeon beach", vAltarBeach);
            Add("Nearest Altar Jungle beach", vAltarBeach);


            Add("Hermes Flurry Boots Distance", vDistanceShort);
            Add("Pathlength to Boots", vPathLength);
            Add("Pathlength to Iron/Lead Bar", vPathLength);
            Add("Pathlength to 10 Iron/Lead Bar Chest", vPathLength);
            Add("Pathlength to Gold/Platinum Bar", vPathLength);
            Add("Pathlength to Bomb", vPathLength);
            Add("Pathlength to Dynamite", vPathLength);
            Add("Pathlength to 2nd Dynamite", vPathLength);
            Add("Pathlength to Gravitation Potion", vPathLength);
            Add("Pathlength to Crystal Heart", vPathLength);
            Add("Pathlength to Jester's Arrow", vPathLength);
            Add("Pathlength to Suspicious Looking Eye", vPathLength);
            Add("Pathlength to Snowball Cannon", vPathLength);
            Add("Pathlength to Teleport Potion", vPathLength);
            Add("Pathlength to 2 Teleport Potion Chest", vPathLength);
            Add("Pathlength to Meteorite Bar", vPathLength);
            Add("Pathlength to Enchanted Sword", vPathLength);
            Add("Pathlength to Altar", vPathLength);
            Add("Pathlength to Bee Hive", vPathLength);
            Add("Pathlength to Boomstick", vPathLength);
            Add("Pathlength to Slime Staute", vPathLength);
            Add("Pathlength to Shark Staute", vPathLength);
            Add("Pathlength to Anvil", vPathLength);
            Add("Pathlength to Ruby", vPathLength);
            Add("Pathlength to Cloud in a Bottle", vPathLength);
            Add("Pathlength to 2 Herb Bag Chest", vPathLength);
            Add("Pathlength to Extractinator", vPathLength);
            Add("Pathlength to Detonator", vPathLength);
            Add("Pathlength to Magic/Ice Mirror", vPathLength);
            Add("Pathlength to Chest", vPathLength);
            Add("Pathlength to Tree Chest", vPathLength);
            Add("Pathlength to Temple Door", vPathLength);
            Add("Pathlength to free ShadowOrb/Heart", vPathLength);  
            Add("Pathlength into cavern layer", vPathLength);  
            Add("Pathlength into 40% cavern layer", vPathLength);  
            Add("Pathlength to 40% cavern entrance", vPathLength);  
            Add("Tiles to mine for 40% cavern", vTilesToMine);
            Add("Pathlength to cavern entrance to mid of Jungle", vPathLength);
            Add("Tiles to mine for mid Jungle cavern", vTilesToMine);
            Add("Pathlength to cavern entrance to deep Jungle", vPathLength);
            Add("Tiles to mine for deep Jungle cavern", vTilesToMine);

            Add("Temple door distance", vDistanceLong);            
            Add("Temple door horizontal distance", vDistanceLong);
            Add("Temple Tile horizontal distance", vDistanceLong);
            Add("Temple Tile vertical distance", vDistanceShort);
            Add("neg. Temple door distance", vDistanceLongNeg);
            Add("neg. Temple door horizontal distance", vDistanceLongNeg);
            Add("neg. Temple Tile horizontal distance", vDistanceLongNeg);
            Add("neg. Temple Tile vertical distance", vDistanceShortNeg);

            Add("Temple at player side of jungle (%)", v0to100);
            Add("Temple at ocean side of jungle (%)", v0to100);
            Add("Temple at height (%)", v0to100);
            Add("Temple at depth (%)", v0to100);

            Add("Spawn in Snow biome", v0to1);
            Add("Spawn in Jungle biome", v0to1);            
            Add("Spawn in Sky", v0to1);
            Add("Spawn in Evil biome", v0to1);

            Add("Wooden Chest", v0to100);
            Add("Wooden Chest Dungeon", v0to20);
            Add("Gold Chest", v0to300_5);
            Add("Gold Chest locked", v0to50);
            Add("Ice Chest", v0to80);
            Add("Ivy Chest", v0to80);
            Add("Water Chest", v0to50);
            Add("Skyware Chest", v0to6);
            Add("Web Covered Chest", v0to20);
            Add("Shadow Chest", v0to20);
            Add("Lihzahrd Chest", v0to20);
            Add("Living Wood Chest", v0to5);

            Add("Score", vScore);

            AddHelpDict();
        }

        private void AddHelpDict()
        {
            HelpDict = new Dictionary<string, string>();

            HelpDict.Add(Help.helpBut, "The Terraria Seed Project is a mod which allows you to search for a world with specific properties you are looking for. \n" +
                "First some quick reference for the buttons: \n \n" +
                UISearchSettings.IconNames.search + " Start the seed search. \n" +
                UISearchSettings.IconNames.options + " The search stops after finishing the current world gen step. \n" +
                UISearchSettings.IconNames.configLoad + " Loads configuration file. Overrides current settings. \n" +
                UISearchSettings.IconNames.configSave + " Saves configuration file. Overrides config file with that name if exist. \n" +
                UISearchSettings.IconNames.positive + " Some configurations examples. Clicking again cycles through them.  \n" +
                UISearchSettings.IconNames.reset + " Resets all changes you have made since last load or search. Click again and it resets all settings. \n" +
                UISearchSettings.IconNames.stop + " Stops after finishing the current world gen step and exit the mod. \n \n \n" +
                "You can left click at colored (sub) section headers or hover your mouse over many other for more information. Here only some general information. \n \n" +
                "This mod was made for vanilla Terraria. Seeds you found here will work there too *(more details later). However also most mods tested can be used with this. Some " +
                "exception is 'WorldGen Previewer' and for some user also 'Overhaul' don't work. Best results if you disable all other and restart Terraria. \n \n" +
                "How the search works: \n" +
                "During world generation this mod checks if the world fulfills the conditions you set up. If not it takes the next seed and tries that. \n" +
                "Some conditions can already be checked before the full world is generated. With this you can save much search time. \n" +
                "At the left side panel you can select the conditions you are looking for and make some general options. The search is separated in 3 Phases. A seed need to " +
                "fulfill the conditions of one phase to enter the next. If it passes Phase 3 the world file gets created and the search continue with the next seed. " +
                "For more details select those Phases at the left. \n \n" +
                "Besides that you can also activate rare world gen phenomena search. If active it also detect rare stuff besides the conditions you are looking for. E.g. spawn on top of" +
                " a floating island. To activate this remove '" + GeneralOptions.omitRareAll + "' and have look in '" + SelectableList.omitRare + "' section. " +
                "The examples which you can cycles through with \n " + UISearchSettings.IconNames.positive + "  also have '" + GeneralOptions.omitRareAll + "' in their list. If you remove it " +
                "they will also search for (not omitted) rares. \n \n " +
                "If you want to look for many pyramids have look at '" + Phase1.pyramidsPossible + "'. \n \n" +
                "*The current unmoded vanilla version of world gen is not free of bugs. One seed can generate slightly different worlds at " +
                "each time you generate the world. It could also throw some errors or freeze. " +
                "\n \nIf you found a nice seed you would like to share try to create it with the vanilla version as well. More notes about this in Phase 3 description. \n \n" +
                
                
                "Don't be too harsh with the conditions you set. E.g. looking alone for a small map with 4 pyramids can take a day. And even if you only combine many common stuff all together can "+
                "become rare again. To speed up you can boost it in Phase 1 ('" + Phase1.pyramidsPossible + "' and '" + Phase1.boost + "') or " +
                "if you have a newer PC with multiple CPU cores you can run Terraria multiple times. But don't run too many. It can slow down search again. And don't forget to start the search " +
                "at a different seed in each. At this time the numbers you can select do not change with map size in nearly all cases. So be careful to not select impossible values e.g a Small world " +
                "can never have 3 or more Living Trees. \n \n" +

                "If you found some good world seeds it would be nice if you share them in Terraria forum, e.g. at the linked mod page or the SHARE YOUR MAP SEED thread in PC General Talk section or " +
                "at the discord channel (link on linked mod page). " +
                "Can't wait to see them. " +
                "Especially looking for a small world with Carpet and Sandstorm bottle and some more stuff. Cycle the green +Button to VeryRare to view it." +
                "If you share your seed somewhere else and there is a chance someone might use this mod as well you could link/name it too. There are over 25 billion different maps (with current buggy vanilla world gen" +
                "even more^^). Nobody can search all of them alone. But together we can do this! \n If you plan to do some bigger search (e.g 1 million+) I can add you to a list at the mod page for reserved seeds. " +
                "So other don't need to search there again. " +
                "Also interested in your suggestions for further updates, bug reports and typos. What else is needed for a good world?\n" +

                "Thanks to T-mod team for doing this mod loader.\n \n" +
                "Good luck in finding the seed of your dreams!"
                );

            HelpDict.Add(Help.helpButHover, "click for help");

            //headers
            HelpDict.Add(title, "Main section for some general search options. Left click for more information.");
            HelpDict.Add(SelectableList.omitRare, "Rare world gen phenomena can be nice to watch. If such is detected it overwrites all conditions you made up, it will always generate it. " +
                "Some rare stuff is more appealing than others. Here you can list all stuff you don't like to show up. By default all gets omitted. If you want to change it remove that " +
                "element first before adding new rules. Left click this header for more information.");
            HelpDict.Add(SelectableList.name, "Here you can change how your files are named. Left click for more information.");

            HelpDict.Add(Phase1.title, "First part of world generation. Filter out seeds here will be very fast but if you add too much conditions it can get rare as well. All conditions need to be " +
                "true to pass that phase. \n" +
                "If you are looking for many pyramids have a look at option '" +Phase1.pyramidsPossible +"' and the advanced option '"+Phase1.boost +"'. \n" +
                "If dungeon should be at a given side check out hover text. \n" +
                " Left click header for more information");

            HelpDict.Add(Phase2.title, "2nd Phase of world gen. Here dungeon, trees and pyramids already generated and can be checked for conditions. You can set up positive conditions lists. " +
                "Each of those give the seed one point by default if all conditions in that list are true (greater or equal to selected value). The single negative list reduce those points " +
                "again for each entry which is true (greater as selected value). A seed nee at least one point to pass that phase and continue to next. Left click for more information.");

            HelpDict.Add(Phase3.title, "Final world gen steps. If a seed enter this phase it gets fully generated. You can check for all conditions left. Besides positive and negative list " +
                " as in Phase 2 this Phase also has the option to take over points from last phase. If active the points of phase 2 and 3 count together (only in phase 3, in phase 2 only those points " +
                "count). So if the seed got many points in phase 2 no more points in phase 3 needed. Left click for more information.");
                        

            HelpDict.Add("Takeover positive", "With this you can combine point calculation for phase 2 and 3. A seed need to pass phase 2 first where only those points count.");


            HelpDict.Add("Start new", "With this points calculated starts again (same as in phase 1 and 2). To pass that phase 3 a seed need at least one point in that phase.");
            HelpDict.Add("Takeover", "With this the point calculation continue with those points which left over from phase 2. The seed is not forced to get a point in phase 3 if it already " +
                "got many in phase 2.");

            HelpDict.Add(SelectableList.positive, "Positive condition list. Here you can add multiple positive conditions with their values. A condition is true if the seed has at least the value you selected. " +
                "This condition list is true if all conditions it contains are true. If so the seed gets one point (by default) for that. The points are sum up among all positive lists and a single " +
                "negative list. To pass the phase the seed need at least one point. Left click for more details.");

            HelpDict.Add(SelectableList.negative, "Negative condition list. Here you can add muliple negative conditions with their values. A condition is true if the seed has a higher value than " +
                "selected. If thats the case the seed points are reduced by one for each condition which is true. To pass the phase a seed need at least one point. Left click for more details. ");

            //public const string continueEvaluatioTakeOverTag = "Takeover";


            //general

            HelpDict.Add(WorldInformation.worldSize, "Careful: Vanilla world gen is bugged if you generate a small world after a large one. Floating island have a high chance missing their cabins.");

            HelpDict.Add("Store mini map as picture", "Stores a revealed image of the generated world. If you like you can also spoil some item info. E.g. location of rare or close items, chests, altars, Life Crystals and many more." +
                " You can do this for all worlds which were fully generated (at phase 3) or only for those which also fulfilled that final phase. \n The file is located at your normal world folder. " +
                "Unless you don't want to get spoiled storing that image is suggested. It gives you a quick overview.");

            HelpDict.Add("Store stats for each seed", "Stores a text file which contains nearly all information you can ask for in that mod. It also stores information about the configuration you searched for" +
                " and some statistics about your current search. \n The file is stored at your normal world folder. This file can be used for record (what I have searched for this seed?) and to figure out" +
                " good values for future searches. E.g. you found a world you like, just look up the values in that text file and add them to your search conditions. Storing that stats file is suggested." +
                " \n Some advanced experimental feature (beta): You can analzse an already existing world (with some content you like). For this enter the full world file name including '.wld' as world name " +
                "(outside mod) and a seed starting with '?'. It will generate the stats file and a minimap image.");
            

            HelpDict.Add("For each in phase 3", "Store it for each seed which was fully generated (all seeds in Phase 3). Phase 3 conditions don't need to be true.");
            HelpDict.Add("For each in phase 3 + item info", "Same as 'For each in phase 3' but also adds some item icons, circles and other enhancements which gives you more information about the world.");

            HelpDict.Add("only for stored worlds", "Store it for each seed which passed all Phases.");
            HelpDict.Add("only for stored worlds + item info", "Same as 'only for stored worlds' but also adds some item icons, circles and other enhancements which gives you more information about the world.");
                       


            HelpDict.Add("neg. ", "neg. means negative value. Same as positive but different sign. With this e.g. pathlength or distance can also be used in positive list. If you are looking for "+
                "a distance less than 100 you can insert the positive value in negative list or the negative value in positive list. If seed has value 42, negative would be -42 which is greater than -100.");

            HelpDict.Add(WorldInformation.randomInverse, "Evil type is bound on seed. If you do random you always get the same evil type for one seed. '" + WorldInformation.randomInverse +"' changes it to the opposite evil type.");

            HelpDict.Add(GeneralOptions.omitRareAll, "Omits all rares. With this in list rare detection is deactivated.");

            HelpDict.Add(GeneralOptions.omitBadRare, "Omits \n no ocean,\n Dungeon far above surface \n Dungeon below ground " +
                "(except there is also a living tree reaching to it),\n Floating Island without chest");

            HelpDict.Add(GeneralOptions.omitCommonRare, "Omits \nChest duplication Glitch,\n Pre Skeletron Dungeon Chest Risky/Grab " +
                "(except its a Muramasa for NE),\n Dungeon in Snow Biome,\n Near Enchanted Sword,\n Enchanted Sword near Tree/Pyramid,\n Spawn in Snow biome,\n "+Phase3.lonelyJungleTree );

            HelpDict.Add(GeneralOptions.omitBaCRare, "Combines omitting bad and more common rare. Look there for me details.");

            HelpDict.Add(GeneralOptions.fantasyScore, "A score computed for the seed. It contains e.g. how many Pyramids or rare items. No absolute value. Only the chance for having a nice world" +
                " getting higher with a higher score. For more details left click " + SelectableList.name);

            HelpDict.Add(GeneralOptions.contentShort, "Summarizes the content of the world for a quick overview. Can also be used for searching among generated worlds in your world folder. " +
                "For more details left click " + SelectableList.name);

            HelpDict.Add("Size, evil type, difficulty", "(S)mall, (M)edium, (L)arge; crimson (r for red), corruption (b blue); e(x)pert, (n)ormal \n" +
                "Random evil biome is always the same for one seed. If the evil biome letter is written small (r or b) you can use random as well");
            

            HelpDict.Add(GeneralOptions.distance, "(distance measured in unit tile block count)");
            HelpDict.Add(GeneralOptions.distanceS, "(distance measured in unit tile block count)");

            //tools
            HelpDict.Add(Tools.dummyPlus, "If all conditions in a positive list are true (the same list where this tool is located) the seed will get the selected amount of points. Also the case " +
                "if no other conditions in that list. That can be used e.g. if you search for multiple condition list at the same time with different personal value, e.g. one which give 2 points " +
                "one give 3 and one 6. With multiple negative list elements you can reduce those points again. E.g. You have 5 of them. If only the 2 point condition list is true only one of those " +
                "5 negative list elements can be true to still pass the phase. If also the 3 points list is true then 4 negative list elements can be true. If only the 6 points list is true all " +
                "negative list elements can be true and the seed still pass the phase. With tool '"+Tools.dummyNeg+"' (as a negative list element) you can reduce those points by a given value.");

            HelpDict.Add(Tools.dummyNeg, "Decreases the phase points by a given value. To pass a phase a seed need at least one point in that phase. That tool can be used e.g. " +
                "if you have multiple positive condition lists and a given amount of those should be true. By default each of those positive lists give the seed one point. You can change this with " +
                "'"+Tools.dummyPlus+"'. A normal negative list element reduces the points by one if the condition is true. E.g. you have 5 positive condition lists which give 1 point each. If you " +
                "select a value of 3 for this tool then 4 or 5 of those positive lists need to be true to pass that phase.");

            HelpDict.Add(Tools.dummyNegEnhancer, "This tool will change the default negative value (=1) to the selected value for all negative list elements below this element (only current phase). So" +
                " if such a condition is true it will reduce the points by the give value instead of one. This tool also changes the impact of '"+Tools.conditionConnector1+"' (same for 2 to 5) to the given " +
                "value. '"+Tools.dummyNeg+"' is not effected by this. ");

            HelpDict.Add(Tools.conditionConnector, "This tool allows to connect different search criteria. Main usage is multiple condition search among phase 2 and 3. So you can search for many " +
                "stuff at the same time. \n E.g. "+
                " In Phase 2 you can add it to a condition list and select a value as label. In phase 3 you can add it to a condition list which should only be used if also the condition list in " +
                "Phase 2 was true (else it give no points). Both need to have the same label. You can also add it to multiple condition list. It will be true if any condition list in phase 2 containing " +
                "the same label was true. For label 1 to 5 there is some additional tool in negative list, e.g. '" + Tools.conditionConnector1+"'");

            string condConFalse = "This tool will reduce the phase points by one if the amount of positive condition lists with the same label are not fulfilled is higher than the selected value. " +
                "Have a look at '" + Tools.conditionConnector + "' in positive list first. For label 1 to 5 there is some additional functionality. You can use those if you have " +
                "multiple condition lists with the same label in Phase 2. Or if you want to give a negative point if a positive condition list you like is not fulfilled.";

            HelpDict.Add(Tools.conditionConnector1, condConFalse);
            HelpDict.Add(Tools.conditionConnector2, condConFalse);
            HelpDict.Add(Tools.conditionConnector3, condConFalse);
            HelpDict.Add(Tools.conditionConnector4, condConFalse);
            HelpDict.Add(Tools.conditionConnector5, condConFalse);


           //phase 1
           HelpDict.Add(Phase1.pyramidsPossible, "At begin of world gen some possible pyramids spots are set up. Only those have the chance to get a pyramid in later world gen steps. " +
               "E.g. a world with only 2 possible pyramid spots can never have 3 or more pyramids. If you select a value here only those seeds which have the chance to get that many " +
               "or more pyramids will pass that phase. If you are looking for pyramids in phase 2 then this value should be at least the amount you are looking for. You can higher the value " +
               "to increase the chance getting many pyramids in later world gen. But be careful, high value also become more rare. Values not higher than 6 for small, " +
               "7/8 for medium, 8/9 for large, or 2 higher than wished pyramid count is suggested for most cases. But feel free to try around yourself. Some stats distribution is displayed at " +
               "the right side of the GUI during search or in a seeds stats file if active. To higher the chances getting many possible pyramid spots you can use option '"+ Phase1.boost+ "'");


            HelpDict.Add(Phase1.boost, "An advanced feature which gives you some control about a very important value during world gen. This has (known) direct impact at the number of living trees," +
                " Bee Hives, Granite and Marble biomes. Some minor influence at lakes, dungeon position, sand generation and some more. This sand generation again has some influence at the value for " +
                "option '"+Phase1.pyramidsPossible+ "' which has again some influence at the number of pyramids. \n" +
                " Most times a higher values gives you a beter world. The seed need to have a higher (or equal) value than you selected here to pass that initial very fast test. Too high values can also have " +
                "some negative effect. For more details left click at header '"+ Phase1.title +"'.\n" +
                " Or in short: you are doing good with value 60 or if you hunt for pyramids 70 for small worlds, 110 for medium, 150 for large");


            string wtext = "Be careful setting dungeon side together with dungeon color and hardmode evil spread side. In current version hardmode evil spread is always at the left side on worlds with blue dungeons and always to the right on worlds with pink dungeon";
            HelpDict.Add(Phase1.dungeonSide, wtext);
            HelpDict.Add(Phase1.dungeonWallColor, wtext);
            HelpDict.Add(Phase1.hallowSide, wtext);

            
            //phase 2
            HelpDict.Add("Number of Clouds", "That includes floating lakes as well. Unless something goes really wrong small world has one floating lake, medium 2, large 3. Total max number is " +
                "4, 7 and 9.");

            HelpDict.Add("Number of Living Trees", "Max number is 2 for small and medium, 5 for large worlds");

            HelpDict.Add("Number of Pyramids", "Max number known is 4 for small, 5 for medium, 6 for large worlds."); //upper bound 10,16,20



            HelpDict.Add(Phase2.dungeonStrangePos, "Combines omitting \n'Dungeon below ground',\n 'Dungeon far above surface',\n 'Dungeon in Snow Biome'");

            HelpDict.Add(Phase2.dungeonGoodPos, "Dungeon placed at surface, no tiles above and not intersection with ocean.");


            HelpDict.Add(Phase2.maxPyrExitCavDist, "Distance from Pyramid exit to cavern layer entrance. Positive if exist is below cavern layer entrance. Negative if exit not as deep as cavern layer entrance.");
            HelpDict.Add(Phase2.maxTreeExitCavDist, "Distance from Tree root exit to cavern layer entrance. Positive if exist is below cavern layer entrance. Negative if exit not as deep as cavern layer entrance.");


            HelpDict.Add("Dungeon below ground tree", "The dungeon can be located below surface. Sometimes a tree is above this and leads down to the dungeon. If that rare case happen this value will be 1.");

            string dungeonChestText = "Sometimes another (micro-)biome overlapping the dungeon. If that case happen a dungeon chest can be nearby. It's called Risky if you need to enter the " +
                "the dungeon to reach it(Auto-pause can be helpful). It's called Grab if you don't need to enter dungeon and can loot the chest without summon the dungeon guardian.";
            string dungeonChestTextChest = " It only increases count if you can get a chest with a golden key as well.";

            HelpDict.Add("Pre Skeletron Dungeon Chest Risky", dungeonChestText+ dungeonChestTextChest);
            HelpDict.Add("Pre Skeletron Dungeon Chest Grab", dungeonChestText+ dungeonChestTextChest);
            HelpDict.Add("Pre Skeletron Dungeon Chest Any", dungeonChestText+ dungeonChestTextChest);

            string dungeonChestTextReach = "Reachable means you can reach a dungeon chest containing this item before killing Skeletron. In some cases you get attacked by dungeon guardian and need to be quick (auto-pause may help). " +
                " You also need to get a Golden key to open it. You can get this e.g. from destroying dungeon pots or you can also insert the option 'Pre Skeletron Golden Key Grab/Risky/Any'. With this option only seeds " +
                "which also have a golden key in a chest you can reach will pass the phase.";
            HelpDict.Add("Pre Skeletron Muramasa Chest reachable", dungeonChestTextReach);
            HelpDict.Add("Pre Skeletron Cobalt Shield Chest reachable", dungeonChestTextReach);
            HelpDict.Add("Pre Skeletron Shadow Key Chest reachable", dungeonChestTextReach);

            string dungeonChestTextGold = " In this case the chest will contain a golden key. You can use this together with reachable chest options to be sure also obtaining a key. 'Dungeon Chest Grab/Risky/Any' checks by itself if there is also a golden key in reach.";

            HelpDict.Add("Pre Skeletron Golden Key Grab", dungeonChestText+ dungeonChestTextGold);
            HelpDict.Add("Pre Skeletron Golden Key Risky", dungeonChestText+ dungeonChestTextGold);
            HelpDict.Add("Pre Skeletron Golden Key Any", dungeonChestText+ dungeonChestTextGold);

            HelpDict.Add(Phase2.allDungeonWalls, "There exists three kinds of dungeon walls. Each spawns different enemies. This value is the smallest value among all walls.");

            string dungeonFarmSpot = "Each dungeon wall spawns different kind of enemies. Can you farm all at once? '3Wall inters.' (intersection) means all 3 wall types are located at one spot. " +
                "'3Wall in line' means all 3 wall types are in one line of screen width. Staying a little above those points allows you to farm all dungeon items. Sometimes they are hidden beneath " +
                "dungeon blocks. Those spots are displayed at the minimap image with light blue circles in dungeon area (line a little darker). This option counts how many such spots exist.";
            HelpDict.Add(Phase2.dungeonFarmSpot, dungeonFarmSpot);
            HelpDict.Add(Phase2.dungeonFarmSpotLine, dungeonFarmSpot);
            HelpDict.Add(Phase2.dungeonFarmSpotInters, dungeonFarmSpot);


            string beachPen = "This condition should give you the ability to filter for different beach environment structures. If a beach looks like a common real life beach that value should be low. " +
                  "If it has some gaps, cliffs or rising instead of falling the beach gets some penalty value. 'max' is the max penalty of both, 'mean' the mean of both oceans. Beach looks OK for values " +
                  "less than 2000 and odd for values above 3000 in most cases. Try around yourself, if you store the stats file you will find the beach penalty values in that file. It only takes the surface" +
                  "structure into account (only living trees are OK too). For even better beach you can use e.g. 'Has evil Ocean = 0' in negative list.";

            HelpDict.Add("Beach penalty mean", beachPen);
            HelpDict.Add("Beach penalty max", beachPen);


            HelpDict.Add("Dungeon tiles above surface", "Is dungeon placed at surface or do you need some rope to climb it? This counts the tiles/blocks between surface and dungeon. Normal dungeon" +
                " has a value of 0.");



            
            HelpDict.Add("Tree close to cavern layer", "A tree with deep roots which leads you close to cavern layer entrance.");
            HelpDict.Add("Tree close to cavern layer near mid", "A tree located near mid (<300 blocks) with deep roots which leads you close to cavern layer entrance.");

            HelpDict.Add("Tree to cavern layer", "A tree with deep roots which leads you beyond cavern layer entrance.");
            HelpDict.Add("Tree to cavern layer near mid", "A tree located near mid (<300 blocks) with deep roots which leads you beyond cavern layer entrance.");


            HelpDict.Add("Lake near mid (guess)", "Checks if there is a lake with at least 200 block size in less than 250 blocks distance from mid. It is marked with 'guess' because " +
                " world gen does some small changes on liquids after Phase 2. It can happen that this value is different in phase 3.");

            HelpDict.Add("Distance Lake to mid (guess)", "Distance to closest lake with at least 200 blocks in size. It is marked with 'guess' because " +
                " world gen does some small changes on liquids after Phase 2. It can happen that this value is different in phase 3.");

            
            HelpDict.Add("Evil Tiles for Jungle Grass", "How many evil biome tiles exists which can convert a jungle grass tile to evil biome tile");
            HelpDict.Add("Evil Tiles for Sand", "How many evil biome tiles exists which can convert a Sand tile to evil biome tile");
            HelpDict.Add("Evil Tiles for Ice", "How many evil biome tiles exists which can convert a Ice tile to evil biome tile");


            HelpDict.Add("Ice surface more than half evil", "True if more than half of the ice tiles above surface are evil ice tiles or can get converted to.");
            HelpDict.Add("Ice surface more than half not evil", "Opposite of 'Ice surface more than half evil'");




            //phase 3

            HelpDict.Add(Phase3.greenPyramid, "A pyramid with green interiority.");

            


            HelpDict.Add(Phase3.allChestItemsNoCraftFish, "All Pyramid Items \n Valor, Bone Welder \n Flower Boots, Living Loom, Sky Mill, Blizzard in a Bottle \n" +
                "Ice Blade, Ice Boomerang, Ice Skates, Ice Machine, Snowball Cannon, Fish Pet\n Seaweed Pet, Honey Dispenser, Living Mahogany Wand (and Leaf Wand) \n Shoe Spikes" +
                ", Band of Regeneration, Lava Charm, Water Walking Boots, Magic Mirror, Ice Mirror, Flurry Boots, Hermes Boots \n Shadow Chest items \n Web Slinger, Angel Statue \n" +
                "Write me if I miss any. \n Be careful with that option. This is very very very very rare.");

            
            HelpDict.Add("High Hive", "Counts hives which are located close to the surface (<200 tiles).");

            HelpDict.Add("Meteorite Bar unlocked", "If meteorite bar is located in a chest which don't need a shadow key.");

            HelpDict.Add("All Paintings", "No seed found yet. Can you find it? (May take some years searching)");
            
            HelpDict.Add("Number different Paintings", "Be careful with setting high values. Not a small number of large worlds don't even have a total number of 51 paintings. " +
                " No seed known yet which had them all. ");
            
            HelpDict.Add("Free ShadowOrb/Heart", "Seed contains a Shadow Orb or Crimson Heart which you can destroy without the need to mine or bomb evil biome tiles around.");

            HelpDict.Add("Free cavern to mid Jungle", "A common Jungle biome has some cave entrance after you went by the half of it at surface. This cave leads down to underground and cavern layer jungle. " +
                "This conditions checks if this cave leads down to mid of Jungle cavern biome without the need of mining (many) tiles. Many stuff can happen during world gen. This value might not be" +
                " 100% accurate. At minimap image the shortest way is displayed by a red line in jungle biome. The pathlength to this cavern entrance can be checked with option " +
                "'Pathlength to cavern entrance to mid of Jungle'. The tiles you need to mine with 'Tiles to mine for mid Jungle cavern'");

            HelpDict.Add("Free cavern to deep Jungle", "A common Jungle biome has some cave entrance after you went by the half of it at surface. This cave leads down to underground and cavern layer jungle. " +
                "This conditions checks if this cave even leads down close to underworld (95%) without the need of mining (many) tiles. Many stuff can happen during world gen. This value might not be" +
                " 100% accurate. At minimap image the shortest way is displayed by a blue line in jungle biome. The pathlength to this cavern entrance can be checked with option " +
                "'Pathlength to cavern entrance to deep Jungle'. The tiles you need to mine with 'Tiles to mine for deep Jungle cavern'.");

            HelpDict.Add("Jungle cavern not blocked by structure", "This combines the conditions 'Free cavern to mid Jungle' and 'Free cavern to deep Jungle' and the entrances of both located very close to each other. " +
                "You can use this e.g. for avoiding a Bee Hive being placed right at the way.");

            HelpDict.Add("Dungeon Distance", "Horizontal distance between spawn and dungeon location.");

            HelpDict.Add("Temple door distance", "Euclidean distance between spawn and Jungle Temple door location. In some cases the temple door does miss after world gen. If that happen this value " +
                "is replaced by the euclidean distance from spawn to the mid of Jungle Temple. (mid of smallest surrounding square to be correct)");

            HelpDict.Add("Temple door horizontal distance", "Horizontal distance between spawn and Jungle Temple door location. In some cases the temple door does miss after world gen. In that case " +
                "this value is the horizontal distance from spawn to the mid of Jungle Temple. (mid of smallest surrounding square to be correct)");

            HelpDict.Add("Temple Tile horizontal distance", "Distance between spawn and closest Jungle Temple tile in horizontal direction.");

            HelpDict.Add("Temple Tile vertical distance", "Distance between spawn and closest Jungle Temple tile in vertical direction. Means how many tiles you need to go down from spawn location to " +
                "be the same height as the top of the Jungle Temple.");

            HelpDict.Add("Temple at player side of jungle (%)", "With this you can filter the location of the jungle regarded to the (cavern) jungle biome. 100% means the mid of jungle temple is at" +
                " the same horizontal coordinate (x) as the most inner jungle tile (in cavern layer). 0% if temple is at the same as the most ocean side jungle biome tile. Other way around is" +
                " option 'Temple at ocean side of jungle (%)'");

            HelpDict.Add("Temple at ocean side of jungle (%)", "With this you can filter the location of the jungle regarded to the (cavern) jungle biome. 100% means the mid of jungle temple is at" +
               " the same horizontal coordinate (x) as the most outer jungle tile (in cavern layer). 0% if temple is at the same as the most inner jungle biome tile.  Other way around is" +
               " option 'Temple at player side of jungle (%)'");

            
            HelpDict.Add("Temple at height (%)", "Location of the jungle temple regarding to the cavern layer. 100% means the mid of jungle temple is at the same height as the entrance to the " +
                "cavern layer. 0% at the same height as the entrance to the underworld. Other way around is option 'Temple at depth (%)'");

            HelpDict.Add("Temple at depth (%)", "Location of the jungle temple regarding to the cavern layer. 100% means the mid of jungle temple is at the same depth as the entrance to the " +
               "underworld. 0% at the same depth as the entrance to the cavern layer. Other way around is option 'Temple at height (%)'");

            HelpDict.Add(Phase3.openTemple, "If you can access the Jungle Temple without key. Current implementation does not take overlapping evil biome into account (but trees, pyramids, cabins,..)." +
                " Often this feature is also not stable between multiple world generations and modded/vanilla Terraria.");

            HelpDict.Add("Mushroom Biome above surface count", "Counts mushroom biome tiles in surface layer. May not exist in current vanilla version.");
            
            HelpDict.Add(Phase3.pathlength, "Pathlength tries to calculate the amount of blocks you need to walk from spawn to reach the given point. It introduces some extra costs if you need to mine " +
                "some tiles or pass webs, liquids, more dangerous areas and many more. Tiles you can't mine from start (evil biome, dungeon, temple, ..) count as impassable. It's not a perfect " +
                "measure but good in most cases. Try around to find good values. They are listed in stats file.");

            
            HelpDict.Add("Pathlength into cavern layer", "Measures the pathlength of the closest way from spawn to cavern layer entrance");

            HelpDict.Add("Pathlength into 40% cavern layer", "Measures the pathlength of the closest way from spawn to be 40% into the cavern layer. 0% would be cavern layer entrance, 100% would be " +
                "underworld entrance. If that entrance should be close to spawn as well you can use also 'Pathlength to 40% cavern entrance'");

            HelpDict.Add("Pathlength to 40% cavern entrance", "Measures the pathlength of the closest way from spawn to a cavern entrance which leads down to 40% of cavern layer. See option " +
                " 'Pathlength into 40% cavern layer' for more details.");

            
            HelpDict.Add("Pathlength to cavern entrance to mid of Jungle", "Measures the pathlength of the closest way from spawn to a cavern entrance which leads down to mid of Jungle biome. 'mid' means mid of " +
                "cavern layer entrance and underworld entrance. It is displayed at minimap image as a red line terminating in jungle biome. If that cavern should also start in Jungle biome you can use " +
                "option 'Free cavern to mid Jungle'. With 'Tiles to mine for mid Jungle cavern' you can check how many tiles you need to mine to get there.");

            HelpDict.Add("Pathlength to cavern entrance to deep Jungle", "Measures the pathlength of the closest way from spawn to a cavern entrance which leads down to deep Jungle biome. 'deep' means " +
                " 95% of the way from cavern layer entrance to underworld entrance. It is displayed at minimap image as a blue line terminating in jungle biome. If that cavern should also start in Jungle biome you can use " +
                "option 'Free cavern to deep Jungle'. With 'Tiles to mine for deep Jungle cavern' you can check how many tiles you need to mine to get there.");


            HelpDict.Add("Tiles to mine for mid Jungle cavern", "'mid' means mid of cavern layer entrance and underworld entrance located in jungle biome. This counts the tiles you need to mine to get there. " +
                "You can also use option 'Free cavern to mid Jungle' which is true (= 1) if you don't need to mine many tiles and the cavern entrance is located on surface in Jungle biome.");

            HelpDict.Add("Tiles to mine for deep Jungle cavern", "'deep' 95% of the way from cavern layer entrance to underworld entrance. This counts the tiles you need to mine to get there. " +
                "You can also use option 'Free cavern to deep Jungle' which is true (= 1) if you don't need to mine many tiles and the cavern entrance is located on surface in Jungle biome.");

            HelpDict.Add("Nearest Altar Dungeon beach", "Smallest horizontal distance between an altar above surface layer and beach at dungeon side");
            HelpDict.Add("Nearest Altar Jungle beach", "Smallest horizontal distance between an altar above surface layer and beach at dungeon side");

            HelpDict.Add("Score", "It's a fantasy score of the world (seed). It takes e.g. into account how many rare items or structures the map has, how many Pyramids, Trees, " +
                "Clouds, Enchanted Swords, the location of the evil tiles, the placement of the Dungeon, how the beach looks and many many more. If one world was generated the score " +
                "computation is viewed at the bottom of the right GUI and also in the stats file if stored. It's not perfect in all cases. Only the chance of getting a nice world is " +
                "higher with a higher score.");

            HelpDict.Add("Underground Distance to spawn", "How many tiles downwards until end of world");
            HelpDict.Add("Cavern Distance to spawn", "How many tiles downwards until start of cavern layer");

            

        }


        public static List<string> getPatern(int from, int steps, int stepSize = 1, int factor1 = 1, int factor2 = 0)
        {

            List<string> values = new List<string>();
            int n = from;
            int cfactor = 1;
            int factor = 1;
            for (int i = 0; i < steps; i++)
            {
                
                factor *= cfactor;
                n = (from + i * stepSize) * factor;
                cfactor = cfactor == factor1 && factor2 != 0 ? factor2 : factor1;
                values.Add(n.ToString());
            }

            return values;

        }


    }
}
