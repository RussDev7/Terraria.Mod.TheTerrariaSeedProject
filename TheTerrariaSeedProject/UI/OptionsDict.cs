using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TheTerrariaSeedProject.UI
{
    class OptionsDict : Dictionary<string, List<string>>
    {

        public const string title = "#### The Terraria Seed Project ####";

        public static class WorldInformation
        {
            public const string worldName = "World Name";
            public const string evilType = "Evil type";
            public const string difficulty = "Difficulty";
            public const string worldSize = "World size";
        }
        public static class Configuration
        {
            public const string configName = "Config file name";
            public const string startingSeed = "Starting seed search at";
            public const string searchSeedNum = "Search until starting seed +";
        }

        public static class Phase1
        {
            public const string title = "### Phase 1 ###";
            public const string pyramidsPossible = "Pyramids possible";
            public const string copperTin = "Copper or Tin";
            public const string ironLead = "Iron or Lead";
            public const string silverTungsten = "Silver or Tungsten";
            public const string goldPlatin = "Gold or Platin";
            public const string moonType = "Moon type";
        }

        public static class Phase2
        {            
            public const string title = "### Phase 2 ###";
            public const string positive = "Phase 2+";
            public const string negative = "Phase 2-";
        }

        public static class Phase3
        {
            public const string title = "### Phase 3 ###";
            public const string positive = "Phase 3+";
            public const string negative = "Phase 3-";
            public const string continueEvaluation = "Takeover positive";
            public const string continueEvaluationResetTag = "Start new";
            public const string continueEvaluatioTakeOverTag = "Takeover";
        }

        public static class GeneralOptions
        {
            public const string title = "### General Options ###";
            public const string naming = "Naming*";
            public const string omitRare = "OmitRare/";
            public const string omitRareAll = "Omit All";
        }

        public static class Paths
        {
            public const string statsPath = @"/TheTerrariaSeedProject/stats/";
            public const string configPath = @"/TheTerrariaSeedProject/config/";
            public const string debugPath = @"/TheTerrariaSeedProject/";
        }

        public static class Bug
        {
            public const string worldSizeBugMinMed = "World size min Med";
            public const string worldSizeBugMinLarge = "World size min Larg";

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
                "pass the current Phase the seed needs this amount of total points. Phase 1 only contains 1 condition, so if that is true the seed starts with 1 point in Phase 2 and needs at least one point there to get to Phase 3. " +
                "If you add multiple positive lists which can (if all conditions are true) lead to more than 1 point also a condition from the negative list can be true and the seed still have enough points get access to the next " +
                "Phase.  \n For some conditions you can only select 0 or 1. 1 means true, 0 false. E.g. “Dungeon in Snow Biome: 1” means in positive list you want a  Dungeon which is located in the Snow Biome. If you set it to 0 it " +
                "is means you want a seed which has at least no Dungeon in Snow Biome, but even better if it is located there. So 0 is true for each map for that condition.";

            public const string negativeDescription = "At this negative list you can add stuff you don't want in your world. Other than on positive list each condition placed here has a negative impact if true. To be true it need " +
                "to be greater than the selected value. So if it's equal it is not fulfilled. \n Same as in positive some conditions only have value 0 and 1. 1 means true, 0 not. But if you select something like “Dungeon in Snow " +
                "Biome: 1” it won't have any effect, because a negative condition is only true if it is grater than the selected value. And greater than true does not work. So if you don't want it you need to select 0. Furthermore " +
                "Dungeon in Snow Biome is also considered as rare. So in that case you also need to select it in omit rare.";

            public const string omitRareDescription = "Rares are as the name implies stuff which don’t happen that often during world generation, e.g. spawning at a floating island. If something rare got detected the map seed " +
                "qualifies for complete generation without fulfilling the condition you made in the different phase sections. \n If you are looking for many different rares it gets quite common to find any of those. " +
                "For that reason you can select rares here which you don’t want to grant access to full world generation anymore.  \n So even if the current seed has such a rare listed here, it still needs to fulfill the " +
                "other conditions you made. \n Some rares can only be detected during phase 3. So if a seed does not fulfill the conditions in phase 1 or 2 it won’t get detected. Same for phase 2 if phase 1 not fulfilled. " +
                "It can’t detect any rare in phase 1.";




            public const string nameDescription = "If a seed passes all Phases it gets saved in your world folder.Here you can select how it should be named.Those tags are concatenated. \n" +
                    ">> World name: the name you entered above or in at world creation. \n" +
                    ">> Seed: seed used to generate the map \n" +
                    ">> Content in short: Some short form of the map content.Each digit has some meaning. \n" +
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
                    "(i.) Chest Doub Glitch(D) \n" +
                    "(ii.) A Enchanted Sword which is very near to Spawn(#) \n" +
                    "(iii.) Spawn in Sky(Y) \n" +
                    "(iv.) All chest items you can't craft or fish (@) \n" +
                    "(v.) Near Enchanted Sword(N) but only if scaled score is less than 10  \n" +
                    "(vi.) Enchanted Sword near Pyramid or Tree(+) but only if scaled score is less than 10  \n" +
                    "(vii.) if the scaled score is less than 10 but the map has many pyramids for its size(P), 4 for small, 5 for medium, 6 for large \n" +
                    ">> Fantasy score: a total score of the seed is computed.It takes e.g.into account how many rare items or structures the map has, how many Pyramids, Trees, Clouds, Enchanted Swords, the location of the evil tiles, the placement of the Dungeon, how the beach looks and many many more. \n" +
                    ">> Rares: it add all kind of detected rares to the map name.e.g.SpawnSky, DungeonStrange, SpawnSnow... " +
                    "\n \n If you don't select anything it does: Seed_Content_Score_Rares";
        }

    



        public static readonly List<string> v0to10 = getPatern(0, 11);
        public static readonly List<string> v0to15 = getPatern(0, 16);
        public static readonly List<string> v0to20 = getPatern(0, 21);
        public static readonly List<string> v0to50 = getPatern(0, 51);
        
        public static readonly List<string> v0to120 = getPatern(0, 121);
        public static readonly List<string> v0to5 = getPatern(0, 6);        
        public static readonly List<string> v0to1 = getPatern(0, 2);
        public static readonly List<string> v0to2 = getPatern(0, 3);
        public static readonly List<string> vDungeonWalls = getPatern(10000, 19, 5000);
        public static readonly List<string> vDungeonALLWalls = getPatern(0, 4, 250).Concat(getPatern(1000, 9, 1000)).Concat(getPatern(10000, 21, 2000)).ToList();
        public static readonly List<string> vNearEvilOcean = getPatern(0, 8, 25).Concat(getPatern(400, 5, 100)).Concat(getPatern(1000, 10, 200)).ToList();
        public static readonly List<string> vBeachPenalty = getPatern(100, 9, 100).Concat(getPatern(1000, 4, 250)).Concat(getPatern(2000, 16, 500)).Concat(getPatern(10000, 11, 1000)).ToList();
        public static readonly List<string> vEvilTiles = getPatern(0, 10, 10).Concat(getPatern(100, 9, 100)).Concat(getPatern(1000, 9, 1000)).Concat(getPatern(10000, 9, 5000)).ToList();
        public static readonly List<string> vDistance = getPatern(100, 9, 100).Concat(getPatern(1000, 4, 250)).Concat(getPatern(2000, 5, 500)).ToList();
        public static readonly List<string> vDistanceShort = getPatern(25, 9, 25).Concat(getPatern(250, 4, 50)).Concat(getPatern(500, 11, 100)).ToList();
        public static readonly List<string> vDistanceLong = getPatern(200, 8, 100).Concat(getPatern(1000, 17, 200)).ToList();
        public static readonly List<string> vNumTiles = getPatern(0, 10, 10).Concat(getPatern(100, 9, 100)).Concat(getPatern(1000, 10, 1000)).ToList();
        public static readonly List<string> vNumSearch = getPatern(1, 3).Concat(getPatern(5, 15,0,2,5)).ToList();
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
        

        public static readonly List<string> vForUnknown = getPatern(0, 10).Concat(getPatern(10, 9, 10)).Concat(getPatern(100, 9, 100)).Concat(getPatern(1000, 9, 1000)).Concat(getPatern(10000, 10, 10000)).ToList();


        public OptionsDict()
        {

            Add(title, new List<string> {
                "The Terraria Seed Project(Beta) is a mod which allows you to search for a world with specific properties you are looking for. \n"+
                "First some quick reference for the buttons: \n \n" +
                UISearchSettings.IconNames.search +" Start the seed search. \n" +
                UISearchSettings.IconNames.options +" The search stops after finish generating the current seed. \n"+
                UISearchSettings.IconNames.configLoad +" Loads configuration file. Overrides current settings. \n"+
                UISearchSettings.IconNames.configSave +" Saves configuration file. Overrides config file with that name if exist. \n"+
                UISearchSettings.IconNames.positive +" Some configurations examples. Clicking again cycles through them.  \n"+
                UISearchSettings.IconNames.reset +" Resets all changes you have made since last load or search. Click again and it resets all settings. \n"+
                UISearchSettings.IconNames.stop +" Stops after finishing the current world gen step and throws you back to main menu. \n \n" +                
                "How it works: \n" +
                "During world generation this mod checks if the world fulfills the conditions you set. If not it takes the next seed and tries that. \n" +
                "Some conditions can already be checked before the full world is generated. With this you can save much search time. \n" +
                "At the left side panel you can select the conditions you are looking for and make some general options. The search is separated in 3 Phases. A seed need to "+
                "fulfill the conditions of one phase to enter the next. If it passes Phase 3 the world file gets created and the search continue with the next seed. "+                
                "For more details select those Phases at the left. \n \n" +

                "Here only some notes to the options on top. You can change them as well as you like. Only World Size has some issues in current vanilla version. Haven't fixed that bug. "+
                "If you generated a Large world and after this change it to Small then somehow the generated floating islands miss their chests sometimes. "+
                "So if you generated some Large worlds and decide to search Small now you need to restart Terraria (except you want clouds without chest houses). \n"+
                "“Config file name” is the name of your current configuration. If you click the load or save button it loads/saves that configuration file with that name."+
                "They are stored in Terraria \\ModLoader \\TheTerrariaSeedProject \\config close to your world files. Those are text files and you can also edit them by hand if you want. "+
                "You can interchange conditions from positive and negative list and set custom number values which you can't select. But e.g. adding a condition from Phase 3 in Phase 2 or 1" +
                "does not work. Also adding new conditions which aren't in any list won't work in most cases. If you edit by hand do some backup copy. It's not very bug safe at this time."+
                "The current unmoded vanilla version of world gen is also not free of bugs. Besides the Floating Islands bug mention above it can also happen that one seed generates slightly different worlds at "+
                "each time you generate the world. It could also throw some errors or freeze. "+
                "If you found a nice seed you would like to share try to create it with the vanilla version as well. More notes about this in Phase 3 description. \n"+
                "At last some handy note for the starting seed input. You can end that number also with M or m (million) to add 6 times 0, or '.' for 5 times 0 (',' works too). The final value need to be" +
                "between 0 and 2147483647. If you enter something else it sets it to a value in between. There is also some limited editing during world generation (it does not updated current search conditions). " +
                "Pressing the bin button there sets settings back to those you are searching for.\n \n"+

                "Don't be too harsh with the conditions you set. E.g. looking alone for a small map with 4 pyramids can take a day. And even if you only combine many common stuff all together can "+
                "become rare again. To speed up you can boost it in Phase 1 (only for Pyramids) or "+
                "if you have a newer PC with multiple CPU cores you can run Terraria multiple times. But don't run too many. It can slow down search again. And don't forget to start the search "+
                "at a different seed in each. At this time the numbers you can select do not change with map size in nearly all cases. So be carefull to not select impossibile values e.g a Small world " +
                "can never have 3 or more Living Trees. \n \n"+

                "If you found some good world seeds it would be nice if you share them in Terraria forum, e.g. at the linked mod page or the SHARE YOUR MAP SEED thread in PC General Talk section. "+
                "Can't wait to see them. " +
                "Especially looking for a small world with Carpet and Sandstorm bottle and some more stuff. Cycle the green +Button to VeryRare to view it."+
                "If you share your seed somewhere else and there is a chance someone might use this mod as well you could link/name it too. There are over 25 billion different maps (with current buggy vanilla world gen" +
                "even more^^). Nobody can search all of them alone. But together we can do this! \n If you plan to do some bigger search (e.g 1 million+) I can add you to a list at the mod page for reserved seeds. "+
                "So other don't need to search there again. "+
                "Also interested in your suggestions for further updates, bug reports and typos. What else is needed for a good world?\n"+
                "The mod source code is not shared at this point because it needs some refactoring first. If you want to help doing that or "+
                "implement some more features feel free writing me. If I have some time until 1.3.6. and vanilla world gen got fixed I will share it at this point.\n \n \n"+
                "Thanks to T-mod team for doing this mod loader.\n \n"+
                "Good luck in finding the seed of your dreams!"
            });

            //world information
            Add(WorldInformation.worldSize, new List<string>() { "Small", "Medium", "Large" });
            Add(Bug.worldSizeBugMinMed, new List<string>() { "# vanilla buggy for Small now", "Medium", "Large" });
            Add(Bug.worldSizeBugMinLarge, new List<string>() {"# vanilla buggy for smaller now", "Large" });

            Add(WorldInformation.evilType, new List<string>() { "Corruption", "Crimson", "Random" });
            Add(WorldInformation.difficulty, new List<string>() { "Normal", "Expert" });

            //config
            Add(Configuration.searchSeedNum, vNumSearch);



            //phase 1
            Add(Phase1.title, new List<string> { "In this phase it checks how many pyramids a seed can have.\n "+
                "E.g. if you are looking for a seed with at least 2 pyramids but at the current world generation step "+
                "it only computes a maximal number of 1, you don't need to generate further. This map seed can only have "+
                "zero or one pyramid in later world generation steps. So you can skip other steps and continue with next seed.\n "+
                "If a map seed has a higher value of maximal pyramid count, also the chance for pyramids it actually has increases.So you can tune your search here with a higher number than needed. E.g.If you enter 4 here, it will skip all seeds with a chance 3 or less.You might miss some worlds with 2 or 3 pyramids but will be faster in finding a seed which actually has 2 pyramids.\n "+
                "Out of experiments I recommend this value to not be higher than 6 for small, 7/8 for medium and 9 for large maps or two higher than the number of pyramids you are looking for. Because a higher value is also rarer in most cases (0(1) also rarer than 1(2)).\n "+
                "For a small map with value 6 you need to skip in mean close to 1000 seeds. At the right panel a distribution is shown. It shows a correlation between chance of a seed to have a number of Pyramids vs. the how many it actually has. To save space it's written in a short form. " +
                "E.g. 23 means at least 3 with 2 zeros behind, so over 300. At the end of a search it stores a file names lastStats.txt with normal number distribution in Terraria \\ModLoader \\TheTerrariaSeedProject folder close to your world files."+
                "\n\n So far I found small maps with 4 pyramids, medium and large with 5.\n \n"+
                "Challenge: Will you find more? Good luck!" +
                "\n \n \n"+
                "Update: You can now also select which ores and moon style the map should have. This is checked before pyramid chance and so very fast. To enter Phase 2 all conditions (ores, moon, pyramid chance) need to be true."
                
                });




            //phase 2
            Add(Phase2.title, new List<string> {"At this step world generation generated all pyramids this map actually has.Even if it had a chance to get 8 you can be unlucky and get zero of them placed. You can skip that seed if it has less than wanted.\n Besides that it also placed the pyramid chest, the living tree, including chests and the floating islands(but not the houses on top). Also Dungeon(with chests), evil, beach and some more."+
                "So you can save time in not generating all world generation steps if the seed can’t even fulfill those early conditions. \n"+
                "The conditions are separated in a list of positive and negative conditions.Positive could be e.g.the number of Pyramids and and the number of living trees, negative e.g.the amount of jungle grass which can get infected by evil biome or the distances to the Dungeon from mid (spawn point unknown in that phase). \n"+
                "All conditions you selected in a positive list need to be true.That means option1 need to be greater or equal than the selected value AND option2 need to be greater or equal AND option3 .. \n"+
                "If all is true the seed gets one point. \n"+
                "If any option of the negative list is greater (not equal) than the value selected the seed gets a point subtracted. So it gets - 1 point if negative list option1 is true OR option2 is true OR option3 … \n"+
                "To get qualified for Phase 3(full world generation) the seed needs to get at least one point here. \n"+
                "If you are not limited to one list of options you can add another positive list which benefit to the seed points.E.g.you are looking for a map which has at least two pyramids, a Sandstorm in a Bottle and a "+
                "Flying Carpet(in positive list 1) but you are also fine if the seed has 3 pyramids and only Sandstorm in a Bottle(in positive list 2). If you are lucky now and the seed generates a map which fulfills both, "+
                "so has 3 pyramids, a Sandstorm in a Bottle and a Flying Carpet the seed gets two points.Sadly it also fulfills two points of your negative list and 2 points gets subtracted, so the total points in this phase "+
                "are 0 and it gets not qualified for Phase 3.The seed search starts with next seed in phase 1. \n If you add nothing here this phase gets skipped and it starts Phase 3 with 2 points(one from first phase +one "+
                "it got for free in that phase). \n  \n All options should be self explained(at least to some degree).Only “Beach penalty” need some further notes. This scores both beaches if they contain gaps in sand or high "+
                "cliffs. The penalty is low if the beach is flat or slowly increases ground level(in ocean to world mid direction). \n A high beach penalty means that the beach don’t look very well, has gaps without sand or the "+
                "ocean is far above the bordering areas.Such a high cliff will result in a Beach penalty of over 9000 in most cases. It only considers the world structure, so e.g.evil biome tiles are not part of that penalty but "+
                "the deep holes of the corruption biome have a high negative impact. Near Dungeon also has negative impact, near living tree does not. Try around a find a value you like. "});



            //phase 3
            Add(Phase3.title, new List<string> { "In Phase 3 full world generation was done. You can make conditions for alls stuff left. All kind of chests are placed, Enchanted Sword Shrine are settled and the exact Spawn "+
                "location is now defined. As in Phase 2 you have again positive list which contain conditions which need to be true (equal or greater than selected value). For each list the seed can get a point if all containing "+
                "conditions of that specific list are fulfilled. The negative list reduces again the points by one for any condition listed there. For more details look Phase 2 description."+
                "The world qualifies to get accepted if it has a total of 3 points or more here. It already got 1 point in Phase 1, and at least one point in Phase 2.If you added more than one positive list in Phase 2 it can start "+
                "up with more than two points.If the negative options of this phase don’t reduce it again below 3 a seed can get accepted without fulfilling a positive list here. If you want to prohibit that " +
                "change “Takeover positive” to “Start new”. With this the points the evaluation starts with in Phase 3 can't be higher than 2 and the seed is forced to get at least one additional point here. \n \n"+
                "As some special kind of condition in this phase you have the option “Score”. That computes a total fantasy score of that seed.It takes e.g. into account how many rare items it has, how many pyramids, clouds, "+
                "living trees, enchanted swords(near spawn?), the beach structure, the location of evil tiles, were the Dungeon is placed, if it has some rare structures and many many more. It tries to be an objective rating, "+
                "so e.g.it does not care about the dungeon color. It’s neither mathematical correct nor containing all stuff nor right in all cases but the chances you get a nice map are higher with a high Score value.It’s "+
                "currently in beta state, haven’t updated for a while but it provides some meaningful values. \n If you don’t add anything in that Phase it gets skipped and the seed is accepted. \n \n After that the world file "+
                "is created.The name of the world file is like you selected in options on top.Currently the vanilla version of Terraria has a bugged world generation.One seed can generate slightly different maps for some seeds. "+
                "With this one variation might get accepted and another is not.This mod tries to spot that and if so generate the map multiple times until it fulfills the conditions again. It does not fix the vanilla version "+
                "bug! It can still happen that a valid variation gets skipped.Also a generated world can also depend on the world you created before.Some seeds have no variation at all, some only in vanilla and some only in the "+
                "Tmod-loader version (without any mods). But in most cases you can generate an equal map in vanilla and the Tmod-loader version + this mod.So if you find some amazing seed best practice before sharing with the world "+
                "is trying to generate it again with vanilla version. First, start up vanilla Terraria again and generate that seed and check if equal, if not, create it again (without restarting Terraria!or creating another world). "+
                "Most seeds are equal all the time or interchange in between variation 1 and variation 2 (1, 2, 1, 2, 1..) with some additional tiny changes sometimes. Those tiny changes are mostly Enchanted Swords Shrines, Mahogany " +
                "Trees with their chests and Minecart Tracks and some more not that important. So the condition of a seed having an active Enchanted Sword Shrine is not that reliable all times. But in that case you can generate " +
                "the seed again and might have one at another location. \n I hope that gets fixed in next update."});

            Add(Phase3.continueEvaluation, new List<string> { Phase3.continueEvaluatioTakeOverTag, Phase3.continueEvaluationResetTag });
            Add(Phase3.continueEvaluatioTakeOverTag, vEmpty);
            Add(Phase3.continueEvaluationResetTag, vEmpty);

            //General
            Add(GeneralOptions.title, new List<string> { "Here you can setup some addtional search settings. E.g. the world name of successfuly generated seeds. Besides the seed as number and the map name you given, you can also add "+
                "some short form of the content the map has and a computed fantasy score (in beta state, high score -> many rare/good stuff). Besides that it also adds, if it found some very rare stuff insdide, E.g. if you spawn in sky"+
                " or there is a Enchanted Sword very near to spawn. Besides the conditions you added, it always searches for rare stuff. If you don't want that, you can also disable it here."});

            Add(GeneralOptions.naming, new List<string> { "World name", "Seed", "Content in short", "Fantasy score", "Rares" });
            Add("World name", vEmpty);
            Add("Seed", vEmpty);
            Add("Content in short", vEmpty);
            Add("Fantasy score", vEmpty);
            Add("Rares", vEmpty);
            Add(GeneralOptions.omitRare, new List<string> {
                GeneralOptions.omitRareAll,
                "Omit Chest Doub Glitch",
                "Omit Dungeon has strange Pos",
                "Omit Dungeon in Snow Biome",
                "Omit Dungeon far above surface",
                "Omit Dungeon below ground",
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
                "Omit All chest items you can't craft or fish"
            });
                                    

            foreach (var rare in this[GeneralOptions.omitRare])            
                Add(rare, vEmpty);
            


            //phase 1 content
            Add(Phase1.pyramidsPossible, v0to15);
            Add(Phase1.copperTin, new List<string> { "Random", "Copper", "Tin"}); //foreach (var elem in this[Phase1.copperTin]) Add(elem, vEmpty);
            Add(Phase1.ironLead, new List<string> { "Random", "Iron", "Lead" });
            Add(Phase1.silverTungsten, new List<string> { "Random", "Silver", "Tungsten" }); //foreach (var elem in this[Phase1.silverTungsten]) Add(elem, vEmpty);
            Add(Phase1.goldPlatin, new List<string> { "Random", "Gold", "Platin" }); //foreach (var elem in this[Phase1.goldPlatin]) Add(elem, vEmpty);
            Add(Phase1.moonType, new List<string> { "Random", "White", "Orange", "Green" }); //foreach (var elem in this[Phase1.moonType]) Add(elem, vEmpty);


            //phase 2 content
            Add(Phase2.positive, new List<string> {
                "Number of Pyramids",
                "Number of Clouds",
                "Number of Living Trees",
                "Pyramid Bottle",
                "Pyramid Carpet",
                "Pyramid Mask",
                "Tree Chest",
                "Tree Chest Loom",
                "Dungeon has good Pos",
                "Dungeon has strange Pos",
                "Dungeon in Snow Biome",
                "Dungeon tiles above surface",
                "Pre Skeletron Dungeon Chest Risky",
                "Pre Skeletron Dungeon Chest Grab",
                "Water Bolt before Skeletron",
                "Water Bolt",
                "Muramasa",
                "Cobalt Shield",
                "Valor",
                "Bone Welder",
                "Alchemy Table",
                "Green Dungeon Walls",
                "Blue Dungeon Walls",
                "Pink Dungeon Walls",
                "All Dungeon Walls",
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
                "Jungle biome surface overlap mid"                
                });
            
            Add(Phase2.negative, new List<string> {                     
                "Evil Tiles for Jungle Grass",
                "Evil Tiles for Sand",
                "Evil Tiles for Ice",
                "Ice surface evil",
                "Ice surface more than half evil",
                "Distance Tree to mid",
                "Distance Cloud to mid",
                "Distance Pyramid to mid",
                "Distance Dungeon to mid",
                "Dungeon has strange Pos",
                "Dungeon tiles above surface",
                "Dungeon in Snow Biome",
                "Beach penalty mean",
                "Beach penalty max",
                "Has evil Ocean",
				"No Ocean",
                "Green Dungeon Walls",
                "Blue Dungeon Walls",
                "Pink Dungeon Walls",
                "Snow biome surface overlap mid",
                "Jungle biome surface overlap mid"
            });

            Add("Number of Pyramids", v0to10);
            Add("Number of Clouds", v0to10);
            Add("Number of Living Trees", v0to5);
            Add("Pyramid Bottle", v0to5);
            Add("Pyramid Carpet", v0to5);
            Add("Pyramid Mask", v0to5);
            Add("Tree Chest", v0to5);
            Add("Tree Chest Loom", v0to5);
            Add("Dungeon has good Pos", v0to1);
            Add("Dungeon has strange Pos", v0to1);
            Add("Dungeon in Snow Biome", v0to1);
            Add("Dungeon tiles above surface", v0to120);            
            Add("Pre Skeletron Dungeon Chest Risky", v0to10);
            Add("Pre Skeletron Dungeon Chest Grab", v0to10);
            Add("Water Bolt before Skeletron", v0to5);
            Add("Water Bolt", v0to10);
            Add("Muramasa", v0to10);
            Add("Cobalt Shield", v0to10);
            Add("Valor", v0to10);
            Add("Bone Welder", v0to10);
            Add("Alchemy Table", v0to10);
            Add("Green Dungeon Walls", vDungeonWalls);
            Add("Blue Dungeon Walls", vDungeonWalls);
            Add("Pink Dungeon Walls", vDungeonWalls);
            Add("All Dungeon Walls", vDungeonALLWalls);
            Add("All Dungeon Items", v0to1);
            Add("Nearest Evil left Ocean", vNearEvilOcean);
            Add("Nearest Evil right Ocean", vNearEvilOcean);
            Add("Evil only one side", v0to1);
            Add("Snow biome surface overlap mid", vNumTiles);
            Add("Jungle biome surface overlap mid", vNumTiles);
            
            

            Add("Beach penalty mean", vBeachPenalty);
            Add("Beach penalty max", vBeachPenalty);            
            Add("No Ocean", v0to2);
            Add("Evil Tiles for Jungle Grass", vEvilTiles);
            Add("Evil Tiles for Sand", vEvilTiles);
            Add("Evil Tiles for Ice", vEvilTiles);
            Add("Ice surface evil", vEvilTiles);
            Add("Ice surface more than half evil", v0to1);
            Add("Distance Tree to mid", vDistance);
            Add("Distance Cloud to mid", vDistance);
            Add("Distance Pyramid to mid", vDistance);
            Add("Distance Dungeon to mid", vDistance);
            Add("Has evil Ocean", v0to2);


            //phase 3 content
            Add(Phase3.positive, new List<string> {                
                "Enchanted Sword Shrine",
                "Enchanted Sword",
                "Near Enchanted Sword",                
                "Enchanted Sword near Pyramid or Tree",
                "Very Near Enchanted Sword",
                "Near Altar",
                "Near Spider Web count",
                "Near Mushroom Biome count",
                "Near Chest",
                "Near Tree",
                "Near Cloud",
                "Cloud Chest",
                "Cloud Ballon",
                "Cloud Starfury",
                "Cloud Horseshoe",
                "Cloud Sky Mill",
                "All Cloud Items",
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
                "All chest items you can't craft or fish",
                "Aglet",
                "Dart Trap",
                "Super Dart Trap",
                "Flame Trap",
                "Spiky Ball Trap",
                "Spear Trap",
                "Geyser",
                "Detonator",
                "Dungeon Distance",
                "Ground Distance",
                "Rock Distance",
                "Meteorite Bar unlocked",
                "Number different Paintings",
                "Number Paintings",
                "Different functional noncraf. Statues",
                "Number functional noncraf. Statues",
                "Different noncraf. Statues",
                "Number noncraf. Statues",
                "Floating Island without chest",
                "Score"
                });
            Add(Phase3.negative, new List<string> {
                "Hermes Flurry Boots Distance",             
                "Temple Distance",
                "Dungeon Distance",
                "Ground Distance",
                "Rock Distance",
                "Floating Island without chest",
                "Score"
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
            Add("Near Cloud", v0to5);
            Add("High Hive", v0to10);
            Add("Cloud Chest", v0to10);
            Add("Cloud Ballon", v0to10);
            Add("Cloud Starfury", v0to10);
            Add("Cloud Horseshoe", v0to10);
            Add("Cloud Sky Mill", v0to10);
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
            Add("All chest items you can't craft or fish", v0to1);
            Add("Aglet", v0to10);
            Add("Dart Trap", vDartTrap);
            Add("Super Dart Trap", v0to50);
            Add("Flame Trap", v0to20);
            Add("Spiky Ball Trap", vSpikyTrap);
            Add("Spear Trap", vSpearTrap);
            Add("Geyser", vGeyDetoTrap);
            Add("Detonator", vGeyDetoTrap);
            Add("Meteorite Bar unlocked", v0to10);
            Add("Dungeon Distance", vDistanceLong);
            Add("Ground Distance", vDistanceShort);
            Add("Rock Distance", vDistanceShort);
            Add("Number different Paintings", vPaintingsDiff);
            Add("Number Paintings", vPaintingsTotal);
            Add("Different functional noncraf. Statues", getPatern(0,27));
            Add("Number functional noncraf. Statues", vStatuesFuncTotal);
            Add("Different noncraf. Statues", getPatern(0, 30+26+1));
            Add("Number noncraf. Statues", vStatuesTotal);
            Add("Floating Island without chest", v0to1);

            Add("Hermes Flurry Boots Distance", vDistanceShort);
            Add("Temple Distance", vDistanceLong);
            Add("Score", vScore);


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
