using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
//using Terraria.Utilities;
using Terraria.IO;
using System.Reflection;
using System.Threading;

using Terraria.GameContent.UI.States;
using TheTerrariaSeedProject.UI;


using ReLogic.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.UI;

using Terraria.Map;

using System.Drawing.Imaging;





#if WINDOWS
using System.Runtime.InteropServices;
#endif

using Terraria.Localization;

#if !WINDOWS
using Terraria.Utilities;
#endif


namespace TheTerrariaSeedProject
{

    public class WorldGenSeedSearch : ModWorld
    {



        public const bool isPubRel = true;
        public const int PlanBulbQuicTime = 20;

        public int stage = 0;
        public bool ended = false;
        public bool searchForSeed = false;
        public bool inSearch = false;
        public bool isInCreation = false;
        public bool gotoCreation = false;
        //int numPyr;

        //public int dungeonSide;
        //ushort jungleHut;
        int numPyrChance;

        //int seed = 200369066;
        public int seed;
        public int lastSeedPhase2 = -1;
        public int lastSeedPhase3 = -1;
        public int lastSeed = 0;
        public int lastStage = 0;

        generatorInfo genInfo;
        ScoreWorld score;
        AcceptConditons acond;


        string startDate;
        int numSearched;
        public bool didNotfinishLast = false;
        int numPyramidChanceTrue, numPyramidChanceTrueComputed;
        bool pyramidPruning; //beta, predict max amount of placed pyramids
        bool cloudPruning; //beta, predict max amount of placed pyramids
        int[] passedStage;
        int[] couldNotGenerateStage;
        int rareGenOnly;
        int rareGen;
        

        int paterCondsTrue;
        int paterScore;

        //string seedPath;

        int condsTrue;
        int rareMax;

        const int chanceSize = 22;
        int[] chanceCount;
        int[,] hasCount;

        Dictionary<string, int> nameToID;

        UISearchSettings uiss;
        GenerationProgress genProg;

        public Stopwatch stopWatchWorldGen;
        public Stopwatch runTime;
        bool gotToMain = false;

        Configuration currentConfiguration = null;

        public bool mediumWasDone = false;
        public bool largeWasDone = false;

        bool statsUpdated = false;
        int numSeedSearch = 0;
        string worldName = "";

        //todo somethime better
        string looking4copperTin = "";
        string looking4ironLead = "";
        string looking4silverTung = "";
        string looking4goldPlation = "";
        string looking4moonType = "";
        string looking4hallowSide = "";
        string looking4dungeonWallColor = "";
        string looking4dungeonSide = "";

        bool randomInverse = false;

        string mapSize = "0";

        double boostValue = 0;
        double boostValueSeed = 0;        
        int boostPyramidValue = 0;
        int boostCloudValue = 0;
        int boostCloudInner2Value = 0;
        int boostCloudNumValue = 0;
        int boostCloudNum1stLakeValue = 0;
        int boostPyramidValueSeed = 0;
        int boostESValue = 0;
        int boostESGraniteValue = 0;
        int boostESValueSeed = 0;
        int boostESGraniteValueSeed = 0;
        int boostESValueSeedP2 = 0;
        int boostESGraniteValueSeedP2 = 0;
        int boostESValueSeedP2good = 0;

        int boostHeightValueMin = 0;
        int boostHeightValueMax = 300;
        int boostUGHeightValueMin = 0;
        int boostUGHeightValueMax = 1000;
        int boostUGHeightValueSeed = 0;
        int boostCavernLayeroffsetMin = 0;
        int boostCavernLayeroffsetMax = 300;
        int boostCavernLayeroffsetSeed = 0;
        int boostHeightValueSeed = 0;
        int boostRockLayerOffset = 0;
        int boostRockLayerOffsetSeed = 0;
        int boostSurfRockLayerOffset = 0;
        int boostSurfRockLayerOffsetSeed = 0;
        int boostSpawnRockSeedOffsetMin = -1000;
        int boostSpawnRockSeedOffsetMax =  100;
        int boostSpawnRockSeedOffsetSeed = 0;
        int boostMidTreeValue = 0;
        int boostMidTreeValueSeed = 0;
        int boostPyramidMidValue = 0;       
        bool doApproxRun = false;
        bool inApproxRun = false;

        bool search4MoonOres = false;
        
        int localnumPyr = 0;
        int[] localPyrX = null;
        int[] localPyrY = null;
        int localDungeonSide = 0;
        int storeMMImg = -1;
        int storeStats = -1;
        int stepsize = 1;
        bool evilHMIsLeft = false;
        int dungeonColor = 0;

        bool continueEval = false;
        bool saveLogSet = false;
        int  useOtherStackFrame = 0;

        List<int> itemIDdoNotWant = null;

        bool loadWorld = false;

        public override void Initialize()
        {
            //TODO all in one, else endless stacks, edit: ????????


            if (saveLogSet)
                WorldGen.saveLock = false;

            /// debug lines start
            //if (!System.IO.Directory.Exists(Main.SavePath + OptionsDict.Paths.debugPath))
            //    System.IO.Directory.CreateDirectory(Main.SavePath + OptionsDict.Paths.debugPath);            
            //writeDebugFile("menu mode: " + Main.menuMode );

            //writeDebugFile("calling func name0: " + (new StackFrame(0, true).GetMethod().Name) );
            //writeDebugFile("calling func name1: " + (new StackFrame(1, true).GetMethod().Name));
            //writeDebugFile("calling func name2: " + (new StackFrame(2, true).GetMethod().Name));
            //writeDebugFile("calling func name3: " + (new StackFrame(3, true).GetMethod().Name));
            //writeDebugFile("calling func name4: " + (new StackFrame(4, true).GetMethod().Name));
            //writeDebugFile("calling func name5: " + (new StackFrame(5, true).GetMethod().Name));
            //writeDebugFile("calling func name6: " + (new StackFrame(6, true).GetMethod().Name));
            //writeDebugFile("calling func name7: " + (new StackFrame(7, true).GetMethod().Name));
            /// debug lines end

            if (useOtherStackFrame == 0)
            {
                Mod overhaul = ModLoader.GetMod("TerrariaOverhaul");
                if (overhaul != null)
                {
                    useOtherStackFrame = 7;
                }
                else
                    useOtherStackFrame = 3;
            }


            bool quickStart = false;
            string quickStartConf = "";
            if (Main.menuMode == 0 && stage == 0 && !ended)
            {
                quickStart = true;
                               
                writeDebugFile("starting in quick mode ");
            }

            if (Main.menuMode != 10 && !quickStart) { stage = 0; return; }
            saveLogSet = false;

            if (stage == 0 && !ended)
            {

                gotToMain = false;
                searchForSeed = false;
                isInCreation = false;
                inSearch = false;
                gotoCreation = false;

                //do_worldGenCallBack 
                //doing new world`?
                string callingFun = new StackFrame(3, true).GetMethod().Name;
                string callingFun7 = new StackFrame(useOtherStackFrame, true).GetMethod().Name; //some people have different exe overhaul

                


                if (!callingFun.Equals("do_worldGenCallBack") && !callingFun7.Equals("do_worldGenCallBack")) return;

                if (!System.IO.Directory.Exists(Main.SavePath + OptionsDict.Paths.configPath))
                    System.IO.Directory.CreateDirectory(Main.SavePath + OptionsDict.Paths.configPath);

                               

                worldName = Main.worldName;
                string seedText = Main.ActiveWorldFileData.SeedText;

                //do seed search?
                if (!((worldName.Length > 0 && worldName[0] == '?') || (seedText.Length > 0 && seedText[0] == '?' || quickStart)))
                    return;
                if (worldName.Length > 0 && worldName[0] == '?')
                    worldName = worldName.Substring(1, worldName.Length - 1);
                Main.worldName = worldName.Length == 0 ? "SeedSearch" : worldName;


                double boostValueMin=0;
                double boostValueMax=0;

                Tuple<List<int>,bool, string> conf= readConfigFile();
                itemIDdoNotWant = conf.Item1;
                quickStartConf = conf.Item3;

                //stuff only need to done once
                acond = new AcceptConditons();


                gotToMain = false;

                nameToID = new Dictionary<string, int>();

                seed = ParseAndSetSeed(Main.ActiveWorldFileData.SeedText);
                

                InitSearch();



                score = new ScoreWorld(); score.init();
                genProg = new GenerationProgress();
                uiss = new UISearchSettings(genProg, mod, this); //also inits config

                if (quickStart && quickStartConf.Length > 0)
                {
                    
                    Configuration cconf = Configuration.LoadConfiguration(Main.SavePath + OptionsDict.Paths.configPath + "config" + quickStartConf + ".txt");
                    if (cconf != null)
                    {
                        Random rnd = new Random();
                        int qseed = rnd.Next(0, Int32.MaxValue);
                        cconf.startSeed = qseed.ToString(); //set this dont set seed, should be read only, or?
                        cconf.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.SelectableText, OptionsDict.Configuration.startingSeed, qseed.ToString());

                        Selectable seedIbox = uiss.infopanel.Search4ElementWithHeaderName(OptionsDict.Configuration.startingSeed);
                        seedIbox.SetValue(qseed.ToString());
                        Main.ActiveWorldFileData.SetSeed(cconf.startSeed);
                        seed = qseed;

                        rnd = null;                        
                        uiss.currentConfig = cconf;
                        currentConfiguration = cconf;                      
                        uiss.SetToConfiguration(currentConfiguration);
                        writeDebugFile("reading quickstart config file :" + quickStartConf + " seed:" + currentConfiguration.FindConfigItemValue(OptionsDict.Configuration.startingSeed, 0));

                    }
                }

                stage++;



                statsUpdated = false;
                numSeedSearch = 1000 * 1000;
                int numStopSearch = numSeedSearch;
                stepsize = 1;
                numSearched = 0;
               


                loadWorld = false;
                
                if (worldName.Length > 4 )
                {
                    //load world instead of search
                    //only works if seed is ?
                  
                    string[] lines = new string[0];
                    if (worldName.Substring(worldName.Length - 4).Equals(".txt"))
                    {
                        string pathTxt = Main.WorldPath + '\\' + worldName;
                        if (System.IO.File.Exists(pathTxt))
                            lines = System.IO.File.ReadAllLines(pathTxt);
                    }else 
                    if (worldName.Substring(worldName.Length - 4).Equals(".wld"))
                        lines = new string[1] { worldName };

                    for (int l = 0; l < lines.Length; l++)
                    {
                        string path = Main.WorldPath + '\\' + lines[l];
                        
                        if (System.IO.File.Exists(path))
                        {
                            writeDebugFile(path);
                            //writeDebugFile(path);
                            loadWorld = true;

                            Main.worldName = lines[l].Length>4 ?lines[l].Substring(0, lines[l].Length - 4) : lines[l]; 
                            worldName = Main.worldName;

                            Main.ActiveWorldFileData = new WorldFileData(path, false);

                            WorldFile.loadWorld(false);
                            seed = lastSeed = Main.ActiveWorldFileData.Seed;
                            
                            stage = 42;
                            lastStage = 42;
                            ended = false;
                            gotToMain = false;
                            numPyrChance = -1;
                            currentConfiguration = uiss.currentConfig;
                            PostWorldGen();
                            setHights(true);
                            checkAndSetIfESmidPossible(false);
                            checkAndSetIfESmidPossible(true);
                            
                            analyzeWorld(score, genInfo, true, lines.Length==1);

                            computeScore(score);
                            var name = createMapName(score, true, currentConfiguration, worldName);

                            worldName = name.worldNameByUser + "_" + name.content + "_" + name.sscore + "_" + name.strares;
                            Main.worldName = worldName;

                            Main.ActiveWorldFileData = WorldFile.CreateMetadata(Main.worldName, false, Main.expertMode);
                            Main.ActiveWorldFileData.SetSeed(name.cseed);
                            
                            StoreMapAsPNG(true);
                            StoreLastStats(true);
                            stage = 0;
                        }
                    }
                }


                while (stage > 0 && !ended && !gotToMain && !loadWorld)
                {
                    {   // <-- without this linux\mono need more memory

                        checkButtons();
                        if (gotToMain) { Exit(); break; };

                        if (numSeedSearch-- <= 0) { goToOptions(); }
                        else if (numStopSearch <= 0) { goToOptions(); }

                        if (gotoCreation) stage = 1;
                        if (gotoCreation) { StoreLastStats(); }

                        
                        if (stage == 1 && !ended && searchForSeed == false)
                        {


                            //setup world
                            Main.worldName = uiss.infopanel.Search4ElementWithHeaderName(OptionsDict.WorldInformation.worldName).GetValue();

                            Selectable seedIbox = uiss.infopanel.Search4ElementWithHeaderName(OptionsDict.Configuration.startingSeed);
                            seedIbox.SetValue(seed.ToString());

                            //avoid cureent vanilla bug
                            BugAvoidCloudChest(true);

                            if (quickStart)
                            {
                                searchForSeed = true;
                                quickStart = false;
                            }

                            if (!TryToGenerate()) continue;
                            didNotfinishLast = true;
                            if (ended || searchForSeed == false || gotToMain || stage < 0) continue;

                            //TODO set up config values, still needed?

                            seed = ParseAndSetSeed(currentConfiguration.FindConfigItemValue(OptionsDict.Configuration.startingSeed, 0));
                           


                            worldName = currentConfiguration.FindConfigItemValue(OptionsDict.WorldInformation.worldName, 0);
                            Main.worldName = worldName;
                            mapSize = currentConfiguration.FindConfigItemValue(OptionsDict.WorldInformation.worldSize, 0); //todo custom sizes

                            if (mapSize.Contains("x"))
                            {
                                int xsi, ysi;
                                Int32.TryParse(mapSize.Substring(0, mapSize.IndexOf('x')), out xsi);
                                Int32.TryParse(mapSize.Substring(mapSize.IndexOf('x') + 1, mapSize.Length - mapSize.IndexOf('x') - 1), out ysi);

                                Main.maxTilesX = xsi < 600 ? Main.maxTilesX : xsi;
                                Main.maxTilesY = ysi < 600 ? Main.maxTilesY : ysi;
                                writeDebugFile(" custom world size " + Main.maxTilesX + "x" + Main.maxTilesY);
                            }
                            else
                            {
                                Main.maxTilesX = mapSize.Equals("Small") ? 4200 : mapSize.Equals("Medium") ? 6400 : mapSize.Equals("Large") ? 8400 : Main.maxTilesX;
                                Main.maxTilesY = mapSize.Equals("Small") ? 1200 : mapSize.Equals("Medium") ? 1800 : mapSize.Equals("Large") ? 2400 : Main.maxTilesY;
                                
                            }

                            string storeMM = currentConfiguration.FindConfigItemValue(OptionsDict.Configuration.storeMMPic, 0);
                            if (!storeMM.Equals("Off"))
                            {
                                storeMMImg = storeMM.Normalize().Contains("phase") ? 0 : 2;
                                storeMMImg += storeMM.Normalize().Contains("info") ? 1 : 0;
                            }
                            else
                                storeMMImg = -1;

                            string storeSt = currentConfiguration.FindConfigItemValue(OptionsDict.Configuration.storeStats, 0);
                            if (!storeSt.Equals("Off"))
                                storeStats = (storeSt.Normalize().Contains("phase") ? 0 : 1);
                            else
                                storeStats = -1;



                            looking4copperTin = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.copperTin, 1);
                            looking4ironLead = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.ironLead, 1);
                            looking4silverTung = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.silverTungsten, 1);
                            looking4goldPlation = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.goldPlatin, 1);
                            looking4moonType = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.moonType, 1);

                            looking4hallowSide = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.hallowSide, 1);
                            looking4dungeonWallColor = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.dungeonWallColor, 1);
                            looking4dungeonSide = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.dungeonSide, 1);

                            int boostValueInt = 10;
                            string boostString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boost, 1);
                            if (boostString.Length > 0)
                                if(!Int32.TryParse(boostString, out boostValueInt)) boostValueInt = 10;

                            int boostValueIntMax = 10;
                            string boostStringMax = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostMax, 1);
                            if (boostStringMax.Length > 0)
                                if (!Int32.TryParse(boostStringMax, out boostValueIntMax)) boostValueIntMax = 10;


                            if (boostValueInt == -1)
                            {
                                //special case value
                                //-1 => skylake shoud be at mid
                                boostValue = -1;
                                boostValueMin = 0.5 + 100.0 / ((double)Main.maxTilesX);
                                boostValueMax = 0.5 + 225.0 / ((double)Main.maxTilesX);

                            }
                            else
                            {
                                boostValue = 1.0 - (1.0 / (0.1 * ((double)boostValueInt)));
                                //e.g. living trees can have 3 values for small if boost val =30 it only accept values greater than 0.6666
                                // if one tree is ok too go for val = 20 and all values greater 0.3333 are fine
                                
                                boostValueMin = boostValue;
                                boostValueMax = (1.0 / (0.1 * ((double)boostValueIntMax)));
                            }
                                                     

                            boostPyramidValue = 0;
                            string boostPyramidString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostPyr, 1);
                            if (boostPyramidString.Length > 0)
                                if (!Int32.TryParse(boostPyramidString, out boostPyramidValue)) boostPyramidValue = 0;

                            boostCloudValue = -1;
                            string boostCloudString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostMidCloud, 1);
                            if (boostCloudString.Length > 0)
                                if (!Int32.TryParse(boostCloudString, out boostCloudValue)) boostCloudValue = -1;

                            boostCloudInner2Value = -1;
                            boostCloudString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostMidCloudInner2, 1);
                            if (boostCloudString.Length > 0)
                                if (!Int32.TryParse(boostCloudString, out boostCloudInner2Value)) boostCloudInner2Value = -1;

                            boostCloudNumValue = 0;
                            boostCloudString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostMidCloudNum, 1);
                            if (boostCloudString.Length > 0)
                                if (!Int32.TryParse(boostCloudString, out boostCloudNumValue)) boostCloudNumValue = 0;

                            boostCloudNum1stLakeValue = 0;
                            boostCloudString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostMidCloud1stLake, 1);
                            if (boostCloudString.Length > 0)
                                if (!Int32.TryParse(boostCloudString, out boostCloudNum1stLakeValue)) boostCloudNum1stLakeValue = 0;



                            boostESValue = 0;
                            string boostESString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostES, 1);
                            if (boostESString.Length > 0)
                                if (!Int32.TryParse(boostESString, out boostESValue)) boostESValue = 0;

                            boostESGraniteValue = 0;
                            string boostESGraniteString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostESgran, 1);
                            if (boostESGraniteString.Length > 0)
                                if (!Int32.TryParse(boostESGraniteString, out boostESGraniteValue)) boostESGraniteValue = 0;

                            boostHeightValueMin = 0;
                            boostHeightValueMax = 300;
                            string boostHeightString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostHeightMin, 1);
                            if (boostHeightString.Length > 0)
                                if (!Int32.TryParse(boostHeightString, out boostHeightValueMin)) boostHeightValueMin = 0;
                            boostHeightString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostHeightMax, 1);
                            if (boostHeightString.Length > 0)
                                if (!Int32.TryParse(boostHeightString, out boostHeightValueMax)) boostHeightValueMax = 300;

                            boostUGHeightValueMin = 0;
                            string boostUGHeightString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostUGheightMin, 1);
                            if (boostUGHeightString.Length > 0)
                                if (!Int32.TryParse(boostUGHeightString, out boostUGHeightValueMin)) boostUGHeightValueMin = 0;

                            boostUGHeightValueMax = 1000;
                            boostUGHeightString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostUGheightMax, 1);
                            if (boostUGHeightString.Length > 0)
                                if (!Int32.TryParse(boostUGHeightString, out boostUGHeightValueMax)) boostUGHeightValueMax = 1000;

                            boostCavernLayeroffsetMin = 0;
                            string boostCavernLayeroffsetString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostCavernLayeroffsetMin, 1);
                            if (boostCavernLayeroffsetString.Length > 0)
                                if (!Int32.TryParse(boostCavernLayeroffsetString, out boostCavernLayeroffsetMin)) boostCavernLayeroffsetMin = 0;

                            boostCavernLayeroffsetMax = 300;
                            boostCavernLayeroffsetString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostCavernLayeroffsetMax, 1);
                            if (boostCavernLayeroffsetString.Length > 0)
                                if (!Int32.TryParse(boostCavernLayeroffsetString, out boostCavernLayeroffsetMax)) boostCavernLayeroffsetMax = 300;

                            boostRockLayerOffset = 0;
                            string boostRockLayerOffsetString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostRockLayerOffset, 1);
                            if (boostRockLayerOffsetString.Length > 0)
                                if (!Int32.TryParse(boostRockLayerOffsetString, out boostRockLayerOffset)) boostRockLayerOffset = 0;

                            boostSurfRockLayerOffset = 0;
                            string boostSurfRockLayerOffsetString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostSurfRockLayerOffset, 1);
                            if (boostSurfRockLayerOffsetString.Length > 0)
                                if (!Int32.TryParse(boostSurfRockLayerOffsetString, out boostSurfRockLayerOffset)) boostSurfRockLayerOffset = 0;

                            boostSpawnRockSeedOffsetMin = -1000;
                            string boostSpawnRockSeedOffsetString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostSpawnRockSeedOffsetMin, 1);
                            if (boostSpawnRockSeedOffsetString.Length > 0)
                                if (!Int32.TryParse(boostSpawnRockSeedOffsetString, out boostSpawnRockSeedOffsetMin)) boostSpawnRockSeedOffsetMin = -1000;

                            boostSpawnRockSeedOffsetMax = 100;
                             boostSpawnRockSeedOffsetString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostSpawnRockSeedOffsetMax, 1);
                            if (boostSpawnRockSeedOffsetString.Length > 0)
                                if (!Int32.TryParse(boostSpawnRockSeedOffsetString, out boostSpawnRockSeedOffsetMax)) boostSpawnRockSeedOffsetMax = -1000;


                            doApproxRun = false;
                            if (boostPyramidValue > 0 || boostCloudValue > 0 || boostCloudInner2Value > 0 )
                                doApproxRun = true;

                            boostMidTreeValue = 0;
                            string boostMidTreeString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostMidTree, 1);
                            if (boostMidTreeString.Length > 0)
                                if (!Int32.TryParse(boostMidTreeString, out boostMidTreeValue)) boostMidTreeValue = 0;

                            boostPyramidMidValue = 0;
                            string boostPyramidMidString = currentConfiguration.FindConfigItemValue(OptionsDict.Phase1.boostMidPyramid, 1);
                            if (boostPyramidMidString.Length > 0)
                                if (!Int32.TryParse(boostPyramidMidString, out boostPyramidMidValue)) boostPyramidMidValue = 0;



                            search4MoonOres = true;
                            if (looking4copperTin.Equals("Random") && looking4ironLead.Equals("Random") && looking4silverTung.Equals("Random")
                                && looking4goldPlation.Equals("Random") && looking4moonType.Equals("Random") && looking4hallowSide.Equals("Random")
                                && looking4dungeonSide.Equals("Random") && looking4dungeonWallColor.Equals("Random"))
                                search4MoonOres = false;

                            //avoid cureent vanilla bug
                            BugAvoidCloudChest();

                            WorldGen.setWorldSize();
                            string evilType = currentConfiguration.FindConfigItemValue(OptionsDict.WorldInformation.evilType, 0);
                            WorldGen.WorldGenParam_Evil = evilType.Equals("Corruption") ? 0 : evilType.Equals("Crimson") ? 1 : -1;
                            randomInverse = evilType.Equals("Random inverse");
                            string diffi = currentConfiguration.FindConfigItemValue(OptionsDict.WorldInformation.difficulty, 0);
                            Main.expertMode = diffi.Equals("Normal") ? false : true;
                            continueEval = !currentConfiguration.FindConfigItemValue(OptionsDict.Phase3.continueEvaluation, 3).Equals(OptionsDict.Phase3.continueEvaluationResetTag);

                            if(seedFile==null)
                                numSeedSearch = currentConfiguration.numSeedSearch;
                            Int32.TryParse(currentConfiguration.FindConfigItemValue(OptionsDict.Configuration.stopSearchNum, 0), out numStopSearch);
                            bool cp = Int32.TryParse(currentConfiguration.FindConfigItemValue(OptionsDict.Configuration.stepSize, 0), out stepsize);
                            if (!cp) stepsize = 1;


                            InitSearch();
                            Main.ActiveWorldFileData.SetSeed(seed.ToString());

                            clearWorld(stage);
                            pyramidPruning = false;
                            cloudPruning = false;
                            continue;
                            

                        }


                        if (stage > 0 && !ended && searchForSeed)
                        {

                            //InitWG();
                        }

                        if (stage == 1 && !ended && searchForSeed && !gotoCreation)
                        {
                            paterCondsTrue = 0;
                            paterScore = -10000000;
                            statsUpdated = false;
                            score.clear(); score.init();
                            condsTrue = 0; //added here

                            boostESValueSeed = -1;
                            boostESGraniteValueSeed = -1;
                            boostESValueSeedP2 = -1;
                            boostESGraniteValueSeedP2 = -1;
                            boostESValueSeedP2good = -1;
                            boostRockLayerOffsetSeed = -1;
                            boostSurfRockLayerOffsetSeed = -1;
                            boostSpawnRockSeedOffsetSeed = -10000;
                            boostHeightValueSeed = -1;
                            boostUGHeightValueSeed = -1;
                            pyramidPruning = false;
                            cloudPruning = false;
                            midESspots = null;
                            midESspotsGood = null;
                            midGraniteESspots = null;

                            double ratio = numPyramidChanceTrue == 0 ? 0 : ((float)numSearched) / ((float)numPyramidChanceTrue);

                            
                            Tuple<double,double,double, double> randVals = setDirectComputeValues(); //### boost values and set dungeon color and hallow side
                            

                            bool look4dungCol = false;
                            bool dungColTrue = false;
                            if (!looking4dungeonWallColor.Equals("Random"))
                            {
                                look4dungCol = true;
                                if ((looking4dungeonWallColor.Equals("Blue") && dungeonColor == 0) ||
                                    (looking4dungeonWallColor.Equals("Green") && dungeonColor == 1) ||
                                    (looking4dungeonWallColor.Equals("Pink") && dungeonColor == 2))
                                    dungColTrue = true;

                            }

                            bool midtree = true;
                            const int treeDist = 105;
                            
                            if (boostMidTreeValue > 0)
                            {
                                midtree = false;
                                if (boostValueSeed > 1.0 / (3 * (Main.maxTilesX / 4200))) //at least one tree
                                    if (boostMidTreeValueSeed > Main.maxTilesX / 2 - treeDist && boostMidTreeValueSeed < Main.maxTilesX / 2 + treeDist)
                                        if (boostMidTreeValueSeed <= Main.maxTilesX / 2 - 100 || boostMidTreeValueSeed >= Main.maxTilesX / 2 + 100)                                    
                                            midtree = true;

                            }

                            
                            //if(randVals.Item1 >95 && randVals.Item2 < 5 && randVals.Item3>90)
                            //if (randVals.Item1 < 2 && randVals.Item2 < 2)
                            //if (randVals.Item1 > 98 && randVals.Item2 < 1 && randVals.Item3 > 98)
                            //if (randVals.Item1 > 86 && randVals.Item2 < 20 && randVals.Item3 > 80)
                            //if (randVals.Item1 < 13 && randVals.Item2 > 80 && randVals.Item3 < 20)                               
                            //if (randVals.Item1 < 13 && randVals.Item2 > 80 && randVals.Item3 < 20)
                            //if (randVals.Item1 > 1e9-50)    
                            bool doit = false;


                           

                            if (((boostValue>=0 && boostValueSeed >= boostValueMin && boostValueSeed <= boostValueMax) ||  
                                (boostValue < 0 && boostValueSeed > boostValueMin && boostValueSeed < boostValueMax) ||
                                (boostValue < 0 && boostValueSeed < 1.0-boostValueMin && boostValueSeed > 1.0-boostValueMax) ) //1st cloud in mid
                                && midtree) //the inverted boostValue can be a little smaller than original value >= --> > ?
                                if (!look4dungCol || dungColTrue)
                                {
                                    doit = true;
                                    if (boostESValue != 0 || boostESGraniteValue != 0)
                                    {
                                        var res = checkAndSetIfESmidPossible(stage != 1);
                                        //writeDebugFile("" + seed + " " + res.Item1 + " " + res.Item2 +" " + res.Item4);
                                        if (boostESValue != 0 && res.Item1 < boostESValue ||
                                            boostESGraniteValue != 0 && res.Item2 < boostESGraniteValue)
                                            doit = false;

                                        //writeDebugFile(""+res.Item4);
                                        //if(res.Item4<8)
                                        //    doit = false;
                                    }

                                }
                                    

                            if(doit)
                            if (!ended && searchForSeed && !gotoCreation)
                                    if (!TryToGenerate()) continue; //--> genInfo ores


                            if (condsTrue > 0)
                            {
                                //rocklayer explosives min, marble/granite max hight  -> small good
                                //rockLayerHigh+ caves -> small good
                                //WorldGen.worldSurfaceLow - 50 , lowest clouds, big good?, -20 highest lakes, -10 evil start, jungle upper lakes, min surface chest
                                //worldSurfaceHigh small holes start, surface caves, min webs, surface altar height, life crystal, statues,buried chest min hight --> small good
                                //worldSurface fulldesert hight, ES min hight  --> small good

                                //Main.surface = WorldGen.worldSurfaceHigh +25;
                                //marble granite : small spawn high , high main.surf-wg.rock value
                                //if ( (Main.worldSurface - WorldGen.rockLayer - boostHeightValueSeed > 30) && !isPubRel && seedFile == null )
                                //    writeDebugFile($"{seed} (m:wlh) {(int)Main.worldSurface}: {(int)WorldGen.worldSurface}|{(int)WorldGen.worldSurfaceLow}|{(int)WorldGen.worldSurfaceHigh} # " +
                                //        $"{(int)Main.rockLayer}: {(int)WorldGen.rockLayer}|{(int)WorldGen.rockLayerLow}|{(int)WorldGen.rockLayerHigh} # {boostHeightValueSeed} # {Main.maxTilesY}");
                            }


                            int legacyPyrNum = 0; ///define inside again#########################################
                            int numPyramidChanceAdv = 0;
                            int numPyramidChanceAdvUber = 0;
                            if (condsTrue == 1 && stage == 1 && !ended && searchForSeed && !gotoCreation)
                            {
                                numPyramidChanceTrueComputed++;

                                genInfo.numPyrChance = localnumPyr;
                                numPyrChance = localnumPyr;
                                legacyPyrNum = localnumPyr;

                                var red= removeInvalidPyrPos();
                                numPyramidChanceAdv = red.Item1;
                                numPyramidChanceAdvUber = red.Item2;
                                int minDist = red.Item3;


                                localnumPyr = numPyramidChanceAdvUber;
                                genInfo.numPyrChance = numPyramidChanceAdvUber;
                                numPyrChance = numPyramidChanceAdvUber;

                                //if (legacyPyrNum - 3 > numPyramidChanceAdvUber && numPyramidChanceAdv > numPyramidChanceAdvUber && numPyramidChanceAdvUber < 2)
                                //if ( ((legacyPyrNum > 4 && numPyramidChanceAdv > numPyramidChanceAdvUber) && numPyramidChanceAdvUber == 1) || (legacyPyrNum > 4 && numPyramidChanceAdv==0) )
                                //    writeDebugFile("seed "+seed+ " has pyspot " + legacyPyrNum + " adv " + numPyramidChanceAdv + " uber " + numPyramidChanceAdvUber);
                                if (minDist < 0.058*Main.maxTilesX && !isPubRel)
                                    writeDebugFile("seed " + seed + " has min pyramid dist " + minDist);
                                                                

                                //chanceCount[numPyr] += 1;
                                chanceCount[localnumPyr] += 1;
                                uiss.ChangeCountText(hasCount, chanceCount, numPyrChance);

                                score.insertGenInfo(genInfo);
                                condsTrue = acond.checkConditions(score, currentConfiguration, stage);
                                                              

                                if (boostMidTreeValue > 0)
                                {
                                    midtree = false;   
                                    if (condsTrue > 0 )
                                    if (boostMidTreeValueSeed > Main.maxTilesX / 2 - treeDist && boostMidTreeValueSeed < Main.maxTilesX / 2 + treeDist)
                                        if (boostMidTreeValueSeed <= Main.maxTilesX / 2 - 100 || boostMidTreeValueSeed >= Main.maxTilesX / 2 + 100)
                                        {
                                            int y = 151;
                                            midtree = true;
                                            while (!Main.tile[boostMidTreeValueSeed, y].active() && y > 0) y--;                                                
                                            if (y > 0) midtree = false;
                                                
                                        }
                                    condsTrue = midtree ? condsTrue : 0;
                                }
                                                                
                                if (boostPyramidMidValue!=0 && minDist > 0.1 * Main.maxTilesX+1)
                                    condsTrue = 0;                                                               

                            }
                            else
                                condsTrue = 0;

                            rareMax = 0;
                            //writeDebugFile(" geninfo " + genInfo.numPyrChance + " " + genInfo.numPyramids + " for seed " + Main.ActiveWorldFileData.Seed + " cond tru " + condsTrue );

                            //clearWorld(stage);

                            lastSeed = seed; lastStage = 1;
                                                        
                            boostPyramidValueSeed = -1;


                            if (condsTrue > 0 && doApproxRun && !ended && searchForSeed && !gotoCreation)
                            {
                                //pyramid apporx
                                inApproxRun = true;
                                if ( boostPyramidValue > 0) pyramidPruning = true;
                                if (boostCloudValue > 0 || boostCloudInner2Value > 0) cloudPruning = true;
                                if (!TryToGenerate()) continue; // -- > boostHeightValueSeed
                                pyramidPruning = false;
                                cloudPruning = false;
                                inApproxRun = false;
                                //writeDebugFile("seed " + seed + " has pyspot " + legacyPyrNum + " adv " + numPyramidChanceAdv + " uber " + numPyramidChanceAdvUber + " placed " + boostPyramidValueSeed);


                            }
                            if (condsTrue > 0 && !ended && searchForSeed && !gotoCreation)
                            {

                                passedStage[stage]++;
                                stage = (condsTrue == 3 ? 42 : condsTrue + 1); // goto 2 if no conditions set in phase 1, or got 3 if also no set in 2
                                if (stage > 2) passedStage[2]++;
                                if (stage > 3) passedStage[3]++;



                            }
                            else
                                startNextSeed();
                        }
                                                

                        if (stage > 1 && !ended && searchForSeed && !gotoCreation)
                        {
                            lastSeedPhase2 = seed;
                            
                            writeToDescList(GenerateSeedStateText(), 1);


                            //check how many Pyramids world actually has and how many living trees, and which items they contain and how far away from mid map
                            if (!TryToGenerate()) continue; //--> genInfo ores
                            if(stage!=2) PostWorldGen(); //--> geninfo

                            //writeToDescList(seed+" left stage 2 next stage " + stage + " construe " + condsTrue);
                            //writeStatsOld();
                        }



                        bool skipStage3 = true;
                        if (stage == 3 && !ended && searchForSeed && !gotoCreation)
                        {
                            skipStage3 = false;
                            writeToDescList(GenerateSeedStateText(), 1);

                            analyzeWorld(score, genInfo, true);
                            paterScore = computeScore(score);
                            paterCondsTrue = acond.checkConditions(score, currentConfiguration, stage);

                            rareMax = Math.Max(score.rare, rareMax);


                            condsTrue = paterCondsTrue;

                            lastSeed = seed; lastStage = 3;
                            lastSeedPhase3 = seed;

                            if (!statsUpdated)
                            {
                                numPyramidChanceTrue++;
                                hasCount[numPyrChance, score.pyramids] += 1;
                                chanceCount[numPyrChance] -= 1;
                                uiss.ChangeCountText(hasCount, chanceCount, numPyrChance, score.pyramids);
                                statsUpdated = true;

                                if (boostPyramidValue > 0 && score.pyramids > boostPyramidValueSeed && !isPubRel)
                                    writeDebugFile("seed " + seed + " has more pyramids than expected (" + boostPyramidValueSeed + " vs " + score.pyramids + ") out of " + numPyramidChanceTrue);
                            }

                            

                            //if (condsTrue > 1) { Main.PlaySound(8, -1, -1, 1, 1f, 0f); }
                            if (condsTrue > 2 || score.rare > 0) { passedStage[stage]++; writeToDescList(GenerateStatsText(), -2); stage = 42; }
                            else
                            {
                                writeToDescList(GenerateStatsText(), -2);
                                if (storeMMImg == 0 || storeMMImg == 1)
                                {
                                    bool valid = true; // !tryAgain && trials < 2, legacy
                                    createMapName(score, valid, currentConfiguration, worldName);
                                    StoreMapAsPNG(storeMMImg % 2 > 0);
                                }
                                if (storeStats == 0)
                                    StoreLastStats(true);
                                
                                startNextSeed();
                            }


                            //writeToDescList(seed+" left stage 3 next stage " + stage +" construe " + condsTrue);

                        }


                        if (stage == 42 && !ended && searchForSeed && !gotoCreation)
                        {


                            //generate and write map file
                            writeToDescList(GenerateSeedStateText(), 1);

                            //due to vanilla world generation is buged and sometimes generates various maps with same seed, generate more than one until good

                            paterCondsTrue = condsTrue;
                            //###check if good map, else redo generation again   
                            int scoreVal;


                            if (skipStage3)
                            {                                
                                analyzeWorld(score, genInfo, true);
                                scoreVal = computeScore(score);
                                condsTrue = acond.checkConditions(score, currentConfiguration, 3);
                            }

                            if (!statsUpdated)
                            {
                                numPyramidChanceTrue++;
                                hasCount[numPyrChance, score.pyramids] += 1;
                                chanceCount[numPyrChance] -= 1;
                                uiss.ChangeCountText(hasCount, chanceCount, numPyrChance, score.pyramids);
                                statsUpdated = true;

                                if (boostPyramidValue > 0 && score.pyramids > boostPyramidValueSeed && !isPubRel)
                                    writeDebugFile("seed " + seed + " has more pyramids than expected (" + boostPyramidValueSeed + " vs " + score.pyramids + ") out of " + numPyramidChanceTrue);
                            }
                            lastSeed = seed; lastStage = 42;
                            if ((score.rare > 0 || condsTrue > 2))
                            {
                                bool valid = true; // !tryAgain && runs < 2, legacy
                                createMapName(score, valid, currentConfiguration, worldName);
                                                               
                                WorldFile.saveWorld(false, true);//Main.ActiveWorldFileData.IsCloudSave = false

                                bool generated = true;

                                if (score.hasOBjectOrParam["Chest duplication Glitch"] > 0)  //debug correct term removed, working now without? Not for modded
                                {
                                    //sometimes a chest does not get saved

                                    WorldFile.loadWorld(false);
                                    PostWorldGen();


                                    analyzeWorld(score, genInfo, true);
                                    computeScore(score);
                                    condsTrue = acond.checkConditions(score, currentConfiguration, 3);



                                    if ((score.rare > 0 || (condsTrue > 2)))
                                    {
                                        //delete old write new,
                                        FileUtilities.Delete(Main.worldPathName, false);
                                        string name = Main.worldPathName.Substring(0, Main.worldPathName.Length - 3) + "twld";
                                        FileUtilities.Delete(name, false);
                                        createMapName(score, valid, currentConfiguration, worldName);
                                        WorldFile.saveWorld(false, true);
                                    }
                                    else
                                    {
                                        generated = false;
                                        FileUtilities.Delete(Main.worldPathName, false);
                                        string name = Main.worldPathName.Substring(0, Main.worldPathName.Length - 3) + "twld";
                                        FileUtilities.Delete(name, false);
                                    }
                                    //delete wrong map sed?
                                }

                                if (generated)
                                {
                                    didNotfinishLast = false;

                                    Main.PlaySound(41, -1, -1, 1, 0.7f, 0f);

                                    passedStage[4]++;

                                    if (condsTrue > 2)
                                        numStopSearch--;

                                    if (score.rare > 0) rareGen++;
                                    if (score.rare > 0 && condsTrue < 3) rareGenOnly++;


                                    //foreach (var elem in score.hasOBjectOrParam)
                                    //    wrtei += elem.Key + ": " + elem.Value + Environment.NewLine;
                                    if (storeMMImg >= 0) StoreMapAsPNG(storeMMImg % 2 > 0);

                                    if (storeStats >= 0) StoreLastStats(true);

                                    writeToDescList(GenerateStatsText(), -2);
                                }
                            }


                            //start again with next seed
                            startNextSeed();
                        }



                    }//without this linux needs more memory

                }

                WorldGen.saveLock = true; saveLogSet = true;
                passedStage = null;
                couldNotGenerateStage = null;
                chanceCount = null;
                hasCount = null;
                itemIDdoNotWant = null;

                nameToID.Clear(); nameToID = null;


            }


            if (stage < 0 && ended)
            {
                stage = 0;
                if (stage == 0) ended = false;

            }
        }



        private void InitSearch()
        {

            chanceCount = new int[chanceSize];
            hasCount = new int[chanceCount.Length, chanceSize / 2];
            passedStage = new int[5];
            couldNotGenerateStage = new int[5];
            rareGenOnly = 0;
            rareGen = 0;

            numSearched = 0;
            numPyramidChanceTrue = 0;
            numPyramidChanceTrueComputed = 0;


            startDate = DateTime.Now.ToString(@"yyyy\/MM\/dd HH\:mm\:ss");

            runTime = new Stopwatch();
            runTime.Start();
        }


        private void BugAvoidCloudChest(bool before = false)
        {
            string mapSize = "";
            if (before)
                mapSize = uiss.infopanel.Search4ElementWithHeaderName(OptionsDict.WorldInformation.worldSize).GetValue();
            else
                mapSize = currentConfiguration.FindConfigItemValue(OptionsDict.WorldInformation.worldSize, 0); //todo custom sizes
            mediumWasDone = mapSize.Equals("Medium") ? true : mediumWasDone;
            largeWasDone = mapSize.Equals("Large") ? true : largeWasDone;
            if (mediumWasDone && !largeWasDone)
            {
                Selectable worldSizeSelect = uiss.infopanel.Search4ElementWithHeaderName(OptionsDict.WorldInformation.worldSize);
                uiss.opdict[OptionsDict.WorldInformation.worldSize] = uiss.opdict[OptionsDict.Bug.worldSizeBugMinMed];
                uiss.infopanel.opdict[OptionsDict.WorldInformation.worldSize] = uiss.opdict[OptionsDict.Bug.worldSizeBugMinMed];
                worldSizeSelect.SetValuesSelText(new List<string>() { "# " + OptionsDict.WorldInformation.worldSize }.Concat(uiss.opdict[OptionsDict.Bug.worldSizeBugMinMed]).ToList());
                //if (before) uiss.infopanel.Search4ElementWithHeaderName(OptionsDict.WorldInformation.worldSize).SetValuesSelText(uiss.opdict[OptionsDict.Bug.worldSizeBugMinMed]);
                if (mapSize.Equals("Small"))
                {
                    Main.maxTilesX = Math.Max(6400, Main.maxTilesX);
                    Main.maxTilesY = Math.Max(1800, Main.maxTilesY);

                    worldSizeSelect.SetValue("Medium");
                }
            }
            if (largeWasDone)
            {
                Selectable worldSizeSelect = uiss.infopanel.Search4ElementWithHeaderName(OptionsDict.WorldInformation.worldSize);
                uiss.opdict[OptionsDict.WorldInformation.worldSize] = uiss.opdict[OptionsDict.Bug.worldSizeBugMinLarge];
                uiss.infopanel.opdict[OptionsDict.WorldInformation.worldSize] = uiss.opdict[OptionsDict.Bug.worldSizeBugMinLarge];
                worldSizeSelect.SetValuesSelText(new List<string>() { "# " + OptionsDict.WorldInformation.worldSize }.Concat(uiss.opdict[OptionsDict.Bug.worldSizeBugMinLarge]).ToList());
                //if (before) uiss.infopanel.Search4ElementWithHeaderName(OptionsDict.WorldInformation.worldSize).SetValuesSelText(uiss.opdict[OptionsDict.Bug.worldSizeBugMinLarge]);
                if (mapSize.Equals("Small") || mapSize.Equals("Medium"))
                {
                    Main.maxTilesX = Math.Max(8400, Main.maxTilesX);
                    Main.maxTilesY = Math.Max(2400, Main.maxTilesY);
                    worldSizeSelect.SetValue("Large");

                }
            }

        }

        private void writeToDescList(string wrtei, int stats1text2bothNeg = -1)
        {
            if (uiss.writeText && stats1text2bothNeg != 1) return;
            //uiss.writeTextUpdating = true;

            if (stats1text2bothNeg == 1 || stats1text2bothNeg == -1)
                uiss.writtenStats = wrtei;
            if (stats1text2bothNeg == 2 || stats1text2bothNeg == -2)
                uiss.writtenText = wrtei;

            if (stats1text2bothNeg == 1 || stats1text2bothNeg < 0)
                uiss.writeStats = true;
            if (stats1text2bothNeg == 2 || stats1text2bothNeg < 0)
                uiss.writeText = true;


        }

        string[] seedFile = null;
        private int ParseAndSetSeed(string seedText)
        {


            //is million given?
            //string st = Main.ActiveWorldFileData.SeedText;

            if (seedText.Length > 0 && seedText[0] == '?')
                seedText = seedText.Substring(1, seedText.Length - 1);
            
            if (seedText.Equals("seeds.txt"))
            {
                string pathTxt = Main.WorldPath + '\\' + seedText;
                if (System.IO.File.Exists(pathTxt))
                {
                    seedFile = System.IO.File.ReadAllLines(pathTxt);
                    if (seedFile.Length > 0)
                    {
                        seedText = seedFile[0];
                        numSeedSearch = seedFile.Length;
                    }
                }
            }
            else
            {
                seedFile = null;
            }
            Main.ActiveWorldFileData.SetSeed(seedText);
            int seed = Main.ActiveWorldFileData.Seed;

            int tseed = 0;
            if (seedText.Length < 6 && (seedText.EndsWith("m") || seedText.EndsWith("M") || seedText.EndsWith(",") || seedText.EndsWith(".")))
            {
                if (Int32.TryParse(seedText.Substring(0, seedText.Length - 1), out tseed))
                {
                    if (seedText.EndsWith(",") || seedText.EndsWith("."))
                        seed = tseed * 100 * 1000;
                    else
                        seed = tseed * 1000 * 1000;

                }
            }
            


            if (seed == Int32.MinValue)
                seed++;

            Main.ActiveWorldFileData.SetSeed(seed.ToString());
            seed = Main.ActiveWorldFileData.Seed;

            return seed;
        }


        int gotoptSeed = 0;
        int gotoptStage = 0;
        public void goToOptions(bool wait = false)
        {
            if (!ended && stage >= 0 && !gotoCreation)
            {

                writeToDescList(GenerateStatsText(false, false, didNotfinishLast), -2);

                gotoptSeed = seed;
                gotoptStage = stage;
                gotoCreation = true;
                searchForSeed = false;
                uiss.writeText = true;
                uiss.writeStats = true;
                inSearch = false; //added 


            }
        }

        //trial to avoid freezing worlds, do not work
        private void StartWGT()
        {

            try
            {

                WorldGen.generateWorld(Main.ActiveWorldFileData.Seed, null); //--> genInfo   
            }
            catch (ThreadAbortException abortException)
            {
                writeDebugFile(" freezing seed thread aborted");
            }

        }


        private bool TryToGenerate()
        {
            //stopWatchWorldGen = new Stopwatch();
            //stopWatchWorldGen.Start();
            clearWorld(stage);

            //writeDebugFile("in clycle " + uiss.writeTextUpdating + " " + uiss.rephrasing + " " + uiss.detailsList.isUpdating + " " + uiss.writeStats + " " + uiss.writeText);            
            while (uiss.writeTextUpdating == true || uiss.rephrasing || uiss.detailsList.isUpdating || uiss.writeStats || uiss.writeText) { Thread.Sleep(10); };
            inSearch = true;

            try
            {
               
                    

                WorldGen.generateWorld(Main.ActiveWorldFileData.Seed, null); //--> genInfo              

            }
            catch (Exception ex)
            {
                if (gotToMain)
                {
                    Exit();
                    return false;
                }
                else
                {
                    writeDebugFile(" could not build seed " + seed + " with size " + Main.maxTilesX + " and evil type " + WorldGen.WorldGenParam_Evil + " expert mode " + Main.expertMode + " in stage " + stage);
                    writeDebugFile(ex.Message);
                    writeDebugFile(ex.ToString());
                    writeDebugFile("stage " + stage);

                    var st = new StackTrace(ex, true);                    
                    var frame = st.GetFrame(0);                    
                    var line = frame.GetFileLineNumber();
                    writeDebugFile(frame + " line " + line);


                    writeDebugFile($"dungeon pos: { WorldGen.dungeonX}.{WorldGen.dungeonY}");
                    

                    //todod remove from stats
                    couldNotGenerateStage[stage == 42 ? 4 : stage]++;

                    if (statsUpdated)
                    {
                        hasCount[numPyrChance, score.pyramids] -= 1;

                    }
                    else
                    {
                        chanceCount[numPyrChance] -= 1;
                    }

                    uiss.SetCountText(hasCount, chanceCount);

                    if (!isPubRel)
                    {
                        //debug
                        Main.worldName = "failed " + seed;

                        Main.ActiveWorldFileData = WorldFile.CreateMetadata(Main.worldName, false, Main.expertMode);
                        Main.ActiveWorldFileData.SetSeed(seed.ToString());
                        StoreMapAsPNG(stage > 2);
                    }

                    //debug end


                    startNextSeed();
                }
                inSearch = false;
                return false;
            }

            inSearch = false;
            return true;

        }


        private void checkButtons()
        {
            Vector2 mousPos = new Vector2(Main.mouseX, Main.mouseY);

            if (Main.mouseLeftRelease)
            {
                uiss.optionListScrollbar.MouseUp(null);
                uiss.detailsListScrollbar.MouseUp(null);
            }
            else if (uiss.optionListScrollbar.ContainsPoint(mousPos) && Main.mouseLeft)
            {
                UIMouseEvent evt = new UIMouseEvent(uiss.optionListScrollbar, mousPos);
                uiss.optionListScrollbar.MouseDown(evt);
            }
            else if (uiss.detailsListScrollbar.ContainsPoint(mousPos) && Main.mouseLeft)
            {
                UIMouseEvent evt = new UIMouseEvent(uiss.detailsListScrollbar, mousPos);
                uiss.detailsListScrollbar.MouseDown(evt);
            }
            else
            if (uiss.stopButton.ContainsPoint(mousPos))
            {
                if (Main.mouseLeft)
                {
                    //stop seed search
                    Exit();
                }
            } else
            if (uiss.optionsButton.ContainsPoint(mousPos) && Main.mouseLeft)
            {
                //change settings                
                goToOptions();
            }
            //else
            //if (uiss.helpButton.ContainsPoint(mousPos) && Main.mouseLeft)
            //{
            //    //help                
            //    uiss.helpClick(null, null);
            ///}



        }

        //int seedCount = 0;
        private void startNextSeed()
        {
           
            stage = 1;
            seed += stepsize;
            numSearched++;
            if(seedFile!=null && seedFile.Length > numSearched)
            {
                Main.ActiveWorldFileData.SetSeed(seedFile[numSearched]);               
                seed = Main.ActiveWorldFileData.Seed;
            }


            if (seed == Int32.MinValue) seed = 0; //else vanilla crashes here            
            if (seed < 0) seed = Int32.MaxValue + seed + 1;

            if (randomInverse)
            {
                WorldGen.WorldGenParam_Evil = -1; //reset evil type
            }

            Main.ActiveWorldFileData.SetSeed(seed.ToString());
        }
        private void clearWorld(int cstage)
        {
            stage = -1;
            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();


            //clear with selected size

            WorldGen.clearWorld();
            stage = stage == -42 ? 0 : cstage;

            //TimeSpan ts = stopWatch.Elapsed;
            //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            //ts.Hours, ts.Minutes, ts.Seconds,
            //ts.Milliseconds / 10);
            //writeDebugFile(doFull+" clear world took " + elapsedTime);

        }

        private string getFullDistrString()
        {
            string stats = "";


            int[] sumI = new int[hasCount.GetLength(1)];
            int all = 0;
            for (int i = 0; i < hasCount.GetLength(0); i++)
            {
                stats = stats + ((i.ToString()).PadLeft(2, '_')) + ":";
                int sum = 0;
                for (int j = 0; j < hasCount.GetLength(1); j++)
                {
                    sumI[j] += hasCount[i, j];
                    sum += hasCount[i, j];
                    stats = stats + "   " + ((hasCount[i, j].ToString()).PadLeft(6, '_'));
                }
                all += sum;
                stats = stats + "   sum: " + (sum + chanceCount[i]).ToString().PadLeft(6, '_') + System.Environment.NewLine;
            }
            stats += "su ";
            for (int j = 0; j < hasCount.GetLength(1); j++)
            {
                stats = stats + "   " + ((sumI[j].ToString()).PadLeft(6, '_'));
            }

            stats = stats + "   all:    " + all.ToString().PadLeft(6, '_') + " with chance of at least " + score.maxPyramidCountChance + " pyramids";


            int ng = 0; for (int i = 0; i < couldNotGenerateStage.Length; i++) ng += couldNotGenerateStage[i];
            stats += System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + "out of " + (numSearched - ng).ToString().PadLeft(6, '_') + " total";

            //System.IO.File.WriteAllText(OptionsDict.Paths.debugPath + "_stats.txt", stats);//////////////////// TODO
            return stats;
        }

        private string GenerateSeedStateText()
        {
            TimeSpan ts = runTime.Elapsed;
            string times = "";
            if (ts.TotalDays > 1 || ts.TotalHours > 1)
                times = Math.Round(ts.TotalHours) + "h";
            else if (ts.TotalMinutes > 1)
                times = Math.Round(ts.TotalMinutes) + "min";
            else if (ts.TotalSeconds > 1)
                times = Math.Round(ts.TotalSeconds) + "s";
            else
                times = Math.Round(ts.TotalMilliseconds) + "ms";

            string sss = "seed " + seed + (stage == 42 ? " builing phase" : (" phase " + stage)) + " time: " + times;
            return sss;
        }

        string lastScoreText = "";
        private string GenerateStatsText(bool doFull = false, bool doLog = false, bool didNotfinishLast = false)
        {
                       
            

            int seed_ = seed;
            int stage_ = stage;

            if (gotoCreation || numSearched == 0)
            {
                seed_ = lastSeed;//todo test if also work for gotCreation
                stage_ = lastStage;
            }
            

            string stats = "";
            int didNotfinished = doFull == true || didNotfinishLast ? 0 : 1;

            stats += "Last eval. seed: " + seed_ + (stage_ > 1 && acond.rares > 0 ? (" Rares: " + acond.rares) : "") + (stage_ > 2 ? (" Score: " + score.score) : "") + Environment.NewLine;
            TimeSpan rt = runTime.Elapsed;
            stats += "Runtime: " + ((int)rt.TotalHours).ToString().PadLeft(2, '0') + ":" + rt.Minutes.ToString().PadLeft(2, '0') + ":" + rt.Seconds.ToString().PadLeft(2, '0') + Environment.NewLine;

            stats += Environment.NewLine;
            stats += "So far searched: " + (numSearched + didNotfinished) + Environment.NewLine;
            stats += "Seeds passed Phases 1: " + passedStage[1] + Environment.NewLine;
            stats += "Seeds passed Phases 2: " + passedStage[2] + Environment.NewLine;
            stats += "Seeds passed Phases 3: " + passedStage[3] + Environment.NewLine;
            stats += "Seeds stored as world: " + passedStage[4] + Environment.NewLine;
            stats += "Rare worlds stored: " + rareGen + Environment.NewLine;
            stats += "..not matching other conditions: " + rareGenOnly + Environment.NewLine;
            int numErr = 0;
            for (int i = 0; i < couldNotGenerateStage.Length; i++) numErr += couldNotGenerateStage[i];
            stats += "Failed to generate: " + numErr + " (" + couldNotGenerateStage[1] + ", " + couldNotGenerateStage[2] + ", " + couldNotGenerateStage[3] + ", " + couldNotGenerateStage[4] + ")" + Environment.NewLine + Environment.NewLine;

            stats += "Distribution: Pyramid chance with how many it has (out of " + (numPyramidChanceTrueComputed - numErr) + "). " + Environment.NewLine;
            stats += uiss.countText;
            stats += Environment.NewLine + "(in short form: e.g. '42' means at least 2 with 4 times zero, so over 20000 times.)" + Environment.NewLine;

            if (doFull)
            {
                stats += Environment.NewLine + Environment.NewLine + "Extended Distribution, normal numbers:";
                stats += Environment.NewLine + getFullDistrString();
            }


            if (stage_ == 42 || doLog)
            {



                string wrtei = "Content last stored Seed: " + seed_.ToString() + Environment.NewLine;
                                
                //wrtei += acond.conditionCheck;
                lastScoreText = wrtei + score.scoreAsText;

                if (acond.rares > 0)
                {
                    lastScoreText += Environment.NewLine + Environment.NewLine + "Rares:" + Environment.NewLine;
                    lastScoreText += acond.rareText;
                }

            }
            if (lastScoreText.Length > 0)
                stats += Environment.NewLine + Environment.NewLine;
            stats += lastScoreText;



            return stats;
        }


        public void StoreLastStats(bool storeLog = false)
        {
            string wso = "";

            Tuple<string, string> conf = currentConfiguration.GetConfigAsString();

            string config = "Config name: config" + conf.Item1 + Environment.NewLine;
            config += conf.Item2 + Environment.NewLine;

            wso += config;
            wso += Environment.NewLine + "Last stats: " + Environment.NewLine;

            string fullStats = GenerateStatsText(true, storeLog);

            wso += Environment.NewLine + fullStats;
            if (!storeLog)
                System.IO.File.WriteAllText(Main.SavePath + OptionsDict.Paths.debugPath + "lastStats.txt", wso);
            else
            {
                string filename = Main.worldPathName.Substring(0, Main.worldPathName.Length - 4) + ".txt";
                System.IO.File.WriteAllText(filename, wso);
            }

        }

        public void Exit()
        {
            ended = false;
            stage = 0;
            gotToMain = true;
            //throw new Exception("TerrariaSeadSearch stopped by user (don't care about this)\n");
        }



        public void ClearPasses(List<GenPass> tasks, int start, int end = -1)
        {
            if (end == -1) end = tasks.Count;
            for (int passid = start; passid < end; passid++)
            {
                
                //thanks to jopojelly again
                PassLegacy passLegacy = tasks[passid] as PassLegacy;
                
                if (passLegacy != null)
                {
                    //private WorldGenLegacyMethod _method;
                    FieldInfo methodFieldInfo = typeof(PassLegacy).GetField("_method", BindingFlags.Instance | BindingFlags.NonPublic);
                    methodFieldInfo.SetValue(passLegacy, (WorldGenLegacyMethod)delegate (GenerationProgress progress) { });
                }
            }
        }


        List<Tuple<int, int>> midESspots = null;
        List<Tuple<int, int>> midGraniteESspots = null;
        List<Tuple<int, int>> midESspotsGood = null;
        //highly experimental, low chance of prediction is true
        public Tuple<int,int, int, int> checkAndSetIfESmidPossible(bool checkIfValid = false)
        {
            //TODO depend on stage better structure, not vaild for stage 3,42
            int inStage = checkIfValid?2:1 ;
            if (checkIfValid )
            {
                midESspots = new List<Tuple<int, int>>();
                midESspotsGood = new List<Tuple<int, int>>();
                midGraniteESspots = new List<Tuple<int, int>>();                
            }

            int exMax = 50000; // only for small worlds!^!!!!!!!!!!!!!!!!!!
            if (Main.maxTilesX >= 6400)
                exMax += 40000;
            if (Main.maxTilesX >= 8400)
                exMax += 30000;
            int ex = 0;

            //other way, counts sometimes some mmore
            if (false)
            {
                UnifiedRandom dummy2 = new UnifiedRandom(seed);
                bool lg = false;
                bool lg2 = false;
                bool lg22 = false;
                bool lg3 = false;


                int lv = 0;
                int pESm = 0;
                int count = 0;
                while (ex++ < exMax)
                {
                    double val = dummy2.NextDouble();


                    if (count >= 0)
                        count++;
                    if (count > 2) // 2
                    {
                        count = -1;
                        lg3 = false;
                    }

                    if (lg3 == true && ((int)(val * 75)) == 0)
                    {
                        pESm++;
                        //writeDebugFile("pesm hit at " + lv );
                    }

                    int type = ((int)(val * 22));

                    if ((lg22 == true))
                    {
                        if (type >= 7 && type <= 15)
                        {
                            lg3 = true;
                            count = 0;
                        }
                        lg22 = false;
                    }

                    if (lg2 == true)
                    {
                        lg2 = false;
                        if (type >= 7 && type <= 15)
                        {
                            lg3 = true;
                            count = 0;
                        }
                        else
                        if (type >= 16 && type <= 22)     //>=16               
                            lg22 = true;
                    }

                    if (lg && val < 0.025) //0.0333
                    {
                        lg2 = true;
                        lg = false;
                    }

                    if (Math.Abs(0.5 - val) < 0.035)//0.04
                    {
                        lg = true;
                        lv = (int)(val * Main.maxTilesX);
                    }
                    else
                        lg = false;
                }
                //writeDebugFile("chance Es mis for " + seed + " :" + pESm + " out of " + exMax);
            }



            UnifiedRandom dummy3 = new UnifiedRandom(seed);

            ex = 0;
            List<double> vals = new List<double>(6) { 0, 0, 0, 0, 0, 0 };
            int li = -1;
            int ces = 0;
            int cesGood = 0;
            int cges = 0;

            Func<Tile, bool> isActiveAndNoDeco = (Tile tile) => {
                return (tile.active() && tile.type != TileID.Containers
                        && tile.type != TileID.Pots && tile.type != TileID.SmallPiles && tile.type != TileID.LargePiles && tile.type != TileID.LargePiles2);
            };


            Func<int, bool> isActive = (int xo) =>
            {
                if (!checkIfValid)
                    return false;

                return isActiveAndNoDeco( Main.tile[25 + (int)(vals[(li + 6 - xo) % 6] * (Main.maxTilesX - 50)),
                    (int)(Main.worldSurface + (vals[(li + 6 - xo +1) % 6] * (Main.maxTilesY - 300 - Main.worldSurface)))]);

                

            };

            //double xmin = 95.0/(Main.maxTilesX-50);//70 95
            //tree min is +/- 100
            double xmin = (inStage <= 1 ? 84.0: 0)/(Main.maxTilesX-50);//70 95
            double xmax = 118.0 / (Main.maxTilesX - 50);//150 145
            double ymax = 20.0/ ((double)Main.maxTilesY - 300.0  -(inStage <= 1? (double)Main.maxTilesY * 0.17 -25: Main.worldSurface )); // 20  15

            int xgmaxi = 60;
            int ygmaxi = 35;
            int xoff = 0;
            if (checkIfValid)
            {
                int x = Main.maxTilesX/2;
               
                int sign = 0;
                int j;
                int i = 0; ;
                for (j = (int)Main.worldSurface; j <= (int)Main.worldSurface + ygmaxi && sign == 0; j++)
                {
                    for (i = 0; i <= xgmaxi && sign==0 ; i++)                    {                       

                        if ( (Main.tile[x-i,j].wall == WallID.MarbleUnsafe || Main.tile[x-i, j].wall == WallID.GraniteUnsafe) && !Main.tile[x-i, j].active())
                        {                          
                            sign = -1;
                        }
                        if ((Main.tile[x + i, j].wall == WallID.MarbleUnsafe || Main.tile[x + i, j].wall == WallID.GraniteUnsafe) && !Main.tile[x + i, j].active())
                        {
                            sign = 1;
                        }
                    }
                }
                if (sign != 0)
                {
                    
                    int xi = x + sign * i;
                    int xstart = xi;
                    const int sb = 7;
                    const int ss = 5;
                    while (true){
                        //Main.tile[xi, j].wall = WallID.AmberGemspark;
                        //Main.tile[xi, j].type = TileID.AmberGemspark;

                        if (xi <= x)//or xstart better here?
                        {
                            if ((Main.tile[xi+1, j + 1].wall == WallID.MarbleUnsafe || Main.tile[xi+1, j + 1].wall == WallID.GraniteUnsafe) && Math.Abs(xstart - xi - i) < xgmaxi)
                            {                              
                                j++;
                                xi++;
                                continue;
                            }
                            if( (Main.tile[xi+sb, j + 1].wall == WallID.MarbleUnsafe || Main.tile[xi+sb, j + 1].wall == WallID.GraniteUnsafe) && Math.Abs(xstart - xi - sb) < xgmaxi)
                            {
                                xi += sb; j++;
                                continue;
                            }
                            if ((Main.tile[xi - ss, j + 1].wall == WallID.MarbleUnsafe || Main.tile[xi - ss, j + 1].wall == WallID.GraniteUnsafe) && Math.Abs(xstart - xi + ss) < xgmaxi)
                            {
                                xi += ss; j++;
                                continue;
                            }
                        }
                        if (xi >= x)
                        {
                            if ((Main.tile[xi - 1, j + 1].wall == WallID.MarbleUnsafe || Main.tile[xi - 1, j + 1].wall == WallID.GraniteUnsafe) && Math.Abs(xstart - xi + 1) < xgmaxi)
                            {
                                j++;
                                xi--;
                                continue;
                            }
                            if ((Main.tile[xi - sb, j + 1].wall == WallID.MarbleUnsafe || Main.tile[xi - sb, j + 1].wall == WallID.GraniteUnsafe) && Math.Abs(xstart - xi + sb) < xgmaxi)
                            {
                                xi -= sb; j++;
                                continue;
                            }
                            if ((Main.tile[xi + ss, j + 1].wall == WallID.MarbleUnsafe || Main.tile[xi + ss, j + 1].wall == WallID.GraniteUnsafe) && Math.Abs(xstart - xi - ss) < xgmaxi)
                            {
                                xi += ss; j++;
                                continue;
                            }
                        }
                        for (i = 0; i <= xgmaxi ; i++)
                        {
                            
                            if ((Main.tile[xi - i, j+1].wall == WallID.MarbleUnsafe || Main.tile[xi - i, j+1].wall == WallID.GraniteUnsafe) && !Main.tile[xi - i, j+1].active() && Math.Abs(xstart - xi + i) < xgmaxi)
                            {
                                xi -= i;j++;
                                break;
                            }
                            if ((Main.tile[xi + i, j+1].wall == WallID.MarbleUnsafe || Main.tile[xi + i, j+1].wall == WallID.GraniteUnsafe) && !Main.tile[xi + i, j+1].active() && Math.Abs(xstart - xi - i) < xgmaxi)
                            {
                                xi += i;j++;
                                break;
                            }
                        }
                        if (i <= xgmaxi)
                            continue;
                        break;

                    }

                    xoff = xi - x;                    
                    ygmaxi = j - (int)Main.worldSurface;
                }
            }



            double xgmax = (double)xgmaxi / (Main.maxTilesX - 50);
            double xgoff = (double)xoff / (Main.maxTilesX - 50);
            double ygmax = (double)ygmaxi / ((double)Main.maxTilesY - 300.0 - (inStage <= 1 ? (double)Main.maxTilesY * 0.17 - 25 : Main.worldSurface));

            int mbes = 0; //mid top, maybe explosive? if together with high wg rocklayer over surf
            while (ex++ < exMax)
            {
                li = (++li) % 6;
                vals[li] = dummy3.NextDouble();

                bool xo3 = false;
                bool xo4 = false;
                bool xo5 = false;

                bool xo3g = false;
                bool xo4g = false;
                bool xo5g = false;
                if ((int)(vals[li] * 75) == 0)
                {
                    //for trees
                    if ((Math.Abs(0.5 - vals[(li + 6 - 3) % 6]) < xmax && Math.Abs(0.5 - vals[(li + 6 - 3) % 6]) > xmin && vals[(li + 6 - 2) % 6] < ymax && (int)(vals[(li + 6 + -1) % 6] * 22) >= 7 && (int)(vals[(li + 6 - 1) % 6] * 22) <= 15  && !isActive(3)))
                    {
                        ces++;
                        xo3 = true;                        
                    }
                    if ((Math.Abs(0.5 - vals[(li + 6 - 4) % 6]) < xmax && Math.Abs(0.5 - vals[(li + 6 - 4) % 6]) > xmin && vals[(li + 6 - 3) % 6] < ymax && (int)(vals[(li + 6 + -2) % 6] * 22) >= 7 && (int)(vals[(li + 6 - 2) % 6] * 22) <= 15 && (int)(vals[(li + 6 - 1) % 6] * 5) != 0 && !isActive(4)) 
                        || (Math.Abs(0.5 - vals[(li + 6 - 4) % 6]) < xmax && Math.Abs(0.5 - vals[(li + 6 - 4) % 6]) > xmin && vals[(li + 6 - 3) % 6] < ymax && (int)(vals[(li + 6 + -2) % 6] * 22) >= 16 && (int)(vals[(li + 6 - 2) % 6] * 22) <= 22 && (int)(vals[(li + 6 + -1) % 6] * 22) >= 7 && (int)(vals[(li + 6 - 1) % 6] * 22) <= 15 && !isActive(4)))
                    {
                        ces++;
                        xo4 = true;
                    }
                    if ((Math.Abs(0.5 - vals[(li + 6 - 5) % 6]) < xmax && Math.Abs(0.5 - vals[(li + 6 - 5) % 6]) > xmin && vals[(li + 6 - 4) % 6] < ymax && (int)(vals[(li + 6 + -3) % 6] * 22) >= 16 && (int)(vals[(li + 6 - 3) % 6] * 22) <= 22 && (int)(vals[(li + 6 + -2) % 6] * 22) >= 7 && (int)(vals[(li + 6 - 2) % 6] * 22) <= 15 && (int)(vals[(li + 6 - 1) % 6] * 5) != 0  && !isActive(5)))                        
                    {
                        ces++;
                        xo5 = true;                        
                    }

                    //for granite /marble mid
                    if ((Math.Abs(0.5 - vals[(li + 6 - 3) % 6] - xgoff) < xgmax && vals[(li + 6 - 2) % 6] < ygmax && (int)(vals[(li + 6 + -1) % 6] * 22) >= 7 && (int)(vals[(li + 6 - 1) % 6] * 22) <= 15 && !isActive(3)))
                    {
                        cges++;
                        xo3g = true;
                    }
                    if ((Math.Abs(0.5 - vals[(li + 6 - 4) % 6] - xgoff) < xgmax  && vals[(li + 6 - 3) % 6] < ygmax && (int)(vals[(li + 6 + -2) % 6] * 22) >= 7 && (int)(vals[(li + 6 - 2) % 6] * 22) <= 15 && (int)(vals[(li + 6 - 1) % 6] * 5) != 0 && !isActive(4))
                        || (Math.Abs(0.5 - vals[(li + 6 - 4) % 6] - xgoff) < xgmax && vals[(li + 6 - 3) % 6] < ygmax && (int)(vals[(li + 6 + -2) % 6] * 22) >= 16 && (int)(vals[(li + 6 - 2) % 6] * 22) <= 22 && (int)(vals[(li + 6 + -1) % 6] * 22) >= 7 && (int)(vals[(li + 6 - 1) % 6] * 22) <= 15 && !isActive(4)))
                    {
                        cges++;
                        xo4g = true;
                    }
                    if ((Math.Abs(0.5 - vals[(li + 6 - 5) % 6] - xgoff) < xgmax && vals[(li + 6 - 4) % 6] < ygmax && (int)(vals[(li + 6 + -3) % 6] * 22) >= 16 && (int)(vals[(li + 6 - 3) % 6] * 22) <= 22 && (int)(vals[(li + 6 + -2) % 6] * 22) >= 7 && (int)(vals[(li + 6 - 2) % 6] * 22) <= 15 && (int)(vals[(li + 6 - 1) % 6] * 5) != 0 && !isActive(5)))
                    {
                        cges++;
                        xo5g = true;
                    }

                    if (checkIfValid)
                    {
                        /*writeDebugFile("seed " + seed +" cesm hit at " + (25 + (int)(vals[(li + 6 - 3) % 6] * (Main.maxTilesX - 50))) + "." + (int)(Main.worldSurface + (vals[(li + 6 - 3 + 1) % 6] * (Main.maxTilesY - 300 - Main.worldSurface))) + " or " +
                            (25 + (int)(vals[(li + 6 - 4) % 6] * (Main.maxTilesX - 50))) + "." + (int)(Main.worldSurface + (vals[(li + 6 - 4 + 1) % 6] * (Main.maxTilesY - 300 - Main.worldSurface))) + " or " +
                            (25 + (int)(vals[(li + 6 - 5) % 6] * (Main.maxTilesX - 50)))  + "." + (int)(Main.worldSurface + (vals[(li + 6 - 5 + 1) % 6] * (Main.maxTilesY - 300 - Main.worldSurface))) +
                            " isac " + isActive(3) +"."+ isActive(4)+ "." + isActive(5) );*/

                        if (inStage == 2)
                            for (int i = 3; i < 6; i++)
                            {
                                if (i == 3 && xo3 || i == 4 && xo4 || i == 5 && xo5)
                                    if (!isActive(i))
                                    {
                                        if (Math.Abs(0.5 - vals[(li + 6 - i) % 6]) < xmax && Math.Abs(0.5 - vals[(li + 6 - i) % 6]) > xmin && vals[(li + 6 - i + 1) % 6] < ymax)
                                            midESspots.Add(new Tuple<int, int>((25 + (int)(vals[(li + 6 - i) % 6] * (Main.maxTilesX - 50))), (int)(Main.worldSurface + (vals[(li + 6 - i + 1) % 6] * (Main.maxTilesY - 300 - Main.worldSurface)))));
                                    }
                                if (i == 3 && xo3g || i == 4 && xo4g || i == 5 && xo5g)
                                    if (!isActive(i))
                                    {
                                        if (Math.Abs(0.5 - vals[(li + 6 - i) % 6] - xgoff) < xgmax && vals[(li + 6 - i + 1) % 6] < ygmax)
                                            midGraniteESspots.Add(new Tuple<int, int>((25 + (int)(vals[(li + 6 - i) % 6] * (Main.maxTilesX - 50))), (int)(Main.worldSurface + (vals[(li + 6 - i + 1) % 6] * (Main.maxTilesY - 300 - Main.worldSurface)))));
                                    }

                            }

                    }


                    if (checkIfValid)
                        while (xo3 || xo4 || xo5 || xo3g || xo4g || xo5g)
                        {
                            int xo = -1;
                            if (xo3) { xo = 3; xo3 = false; }                            
                            else if (xo4) { xo = 4; xo4 = false; }
                            else if (xo5) { xo = 5; xo5 = false; }
                            else if (xo3g) { xo = 3; xo3g = false; }
                            else if (xo4g) { xo = 4; xo4g = false; }
                            else if (xo5g) { xo = 5; xo5g = false; }

                            int x = (25 + (int)(vals[(li + 6 - xo) % 6] * (Main.maxTilesX - 50)));
                            int y = ((int)(Main.worldSurface + (vals[(li + 6 - xo + 1) % 6] * (Main.maxTilesY - 300 - Main.worldSurface))));
                                                
                            while (!isActiveAndNoDeco(Main.tile[x, y + 1]) && y < Main.rockLayer+18)
                            {
                                y++; 
                            }
                            if (y < Main.rockLayer+18 && isActiveAndNoDeco(Main.tile[x, y + 1]))
                            {
                               
                                bool good = true;
                                for (int xi = x - 1; xi < x + 2; xi++)
                                {
                                    for (int yi = y; yi < y + 1; yi++)
                                    {
                                        if (Main.tile[xi, yi]!=null && isActiveAndNoDeco(Main.tile[xi, yi])) 
                                            good = false;
                                    }
                                    if (!WorldGen.SolidTile2(xi, y + 1))
                                    {
                                        good = false;
                                    }
                                }

                                if (good)
                                {
                                    if (isActiveAndNoDeco(Main.tile[x, y + 1]) && (Main.tile[x, y + 1].type == TileID.LivingWood                                      
                                        || Main.tile[x, y + 1].type == TileID.Granite
                                        || Main.tile[x, y + 1].type == TileID.Marble
                                        || Main.tile[x, y + 1].type == TileID.MarbleBlock
                                        || Main.tile[x, y + 1].type == TileID.GraniteBlock
                                        ))
                                    {
                                        //writeDebugFile("seed " + seed + " cesm hit at " + x + "." + y + "  ("+xo+") 2ndlv "  + vals[(li + 6 - 1) % 6] );
                                        if(inStage == 2)
                                            midESspotsGood.Add(new Tuple<int, int>(x,y));
                                        cesGood++;
                                    }
                                }
                            }

                        }

                }

                if ((int)(vals[li] * (Main.maxTilesY - 200) * 0.5) < 5 &&
                       (int)(vals[(li+6-1)%6] * (Main.maxTilesX - 100) * 0.5) < 15)
                    mbes++;

            }


            //writeDebugFile("chance EsMis2 for " + seed + " :" + ces + " out of " + exMax);

            //if (!isPubRel)
            //    writeDebugFile(""+seed+ " :" + ces + " . " + cges + " " + cesGood + " off " + xoff + " ymax " + ygmaxi);



            if (ces > 10 && !isPubRel)
                writeDebugFile("chance for " + seed + " :" + ces);

            if (inStage == 1)
            {
                boostESValueSeed = ces;
                boostESGraniteValueSeed = cges;
            }
            if (inStage == 2)
            {
                boostESValueSeedP2 = ces;
                boostESGraniteValueSeedP2 = cges;
                boostESValueSeedP2good = cesGood;
            }
            //writeDebugFile($"{xoff} and {ygmaxi}");
            //StoreMapAsPNG(false);

            return new Tuple<int, int, int, int>(ces, cges, cesGood, mbes);
        }



        public Tuple<double, double, double, double > setDirectComputeValues()
        {
            //check hardmode evil biome side and dungeon wall color
            //that might not work in later terraria versions!!!! !!!!!!!

            UnifiedRandom dummy = new UnifiedRandom(seed);
            //int sandSpotDensity = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.0008), (int)((double)Main.maxTilesX * 0.0025)); 

            //random var 1
            //+sand -> pyramids spots possible -> pyramids for small 7 (3 to 10[.5] +2),mid 11 (5 to 16 +2) large 15 (6 to 21 +2)
            //+living trees : small 3
            //+lakes small 19, mid 30, large 40
            //+hive count small 3 (1+(5 to 8))
            //+thin ice biome 3 (..)
            //+marble count 5
            //+granite count 6
            //position of first floating istland lake, except if it is mid

            //maybe
            //dungeon distance something (only inital)
            //+surface 20
            //+jungel distance + rand2 value
            //+full desert biome distance, first trial
            //frist trial temple depth
            double sandSpotDensity = dummy.NextDouble();
            //int sandSpotDensity = dummy.Next(0, 100);

            //2nd random
            //first tree
            //maybe 
            //+cavern depth 20 ( cavern low to get small underground layer)
            //first trial temple distance
            //jungel distance
            double a = dummy.NextDouble(); //dummy
            //int a = dummy.Next(0, 100); //dummy

            //3rd rand
            //dungeon color
            //jungle chest count small 5 (7 to 12) , large 10
            //maybe jungle depth
            double randVar = dummy.NextDouble();
            evilHMIsLeft = (int)(randVar * (double)2) == 0 && a > -1.0;
            dungeonColor = (int)(randVar * (double)3); // 0: blue, 1:green, 2: pink ===> evil hm side and dcolor correlated 


            boostValueSeed = sandSpotDensity;
            boostMidTreeValueSeed = 300 + (int)(a * (Main.maxTilesX - 600));




            return new Tuple<double, double, double, double >(sandSpotDensity, a, randVar, dummy.NextDouble());
        }

        public int guessSpawnHight(bool reduce)
        {

            //height approx
            bool isSky = false;
            const int csi = 15;
            int from = Main.maxTilesX / 2 - csi;
            int to = Main.maxTilesX / 2 + csi + 1;
            int y = (int)Main.worldSurface - 1;
            while (!isSky && --y > 200)
            {
                isSky = true;
                for (int x = from; x < to; ++x)
                {
                    //if (Main.tile[x, y].wall != WallID.None)
                    if (Main.tile[x, y].active() || Main.tile[x, y].wall != WallID.None)
                    {
                        isSky = false;
                        break;
                    }                    
                }
            }
            isSky = true;
            if(reduce)
            while (isSky && ++y < Main.worldSurface)
            {                
                for (int x = from; x < to; ++x)
                {
                    //if (Main.tile[x, y].wall != WallID.None)
                    if ( (Main.tile[x, y].active() && Main.tile[x, y].wall == WallID.None)  
                        || (!Main.tile[x, y].active()  )) //&& (Main.tile[x, y].wall == WallID.None  || Main.tile[x, y].wall == WallID.GraniteUnsafe  || Main.tile[x, y].wall == WallID.MarbleUnsafe))
                        {

                    }
                    else
                    {
                        isSky = false;
                        break;
                    }
                }
            }



            int height = (int)Main.worldSurface - y;
            if(reduce && Main.tile[(int)Main.worldSurface/2, y-1].wall == WallID.None)
                height++;
            return height;

        }

        public void setHights(bool reduce)
        {
            boostHeightValueSeed = guessSpawnHight(reduce);
            boostUGHeightValueSeed = (int)(Main.rockLayer - Main.worldSurface);

            boostRockLayerOffsetSeed = (int)(Main.rockLayer - WorldGen.rockLayer);
            boostSurfRockLayerOffsetSeed = loadWorld ? -100000 :(int)(Main.worldSurface - WorldGen.rockLayer);

            boostSpawnRockSeedOffsetSeed = boostSurfRockLayerOffsetSeed - boostHeightValueSeed;

            //writeDebugFile(" seed " + seed + "  " + WorldGen.rockLayerLow+ "  " + WorldGen.rockLayerHigh + "  " + WorldGen.rockLayer + "  " + WorldGen.worldSurface +"  " + WorldGen.worldSurfaceHigh+ "  " + WorldGen.worldSurfaceLow);


            boostCavernLayeroffsetSeed = 0;
            if (boostRockLayerOffsetSeed < boostUGHeightValueSeed) boostCavernLayeroffsetSeed = (int)(((float)boostRockLayerOffsetSeed) /((float)boostUGHeightValueSeed)*100);
            else if (boostRockLayerOffsetSeed < boostUGHeightValueSeed+ boostHeightValueSeed) boostCavernLayeroffsetSeed = 100+(int)(((float)(boostRockLayerOffsetSeed- boostUGHeightValueSeed)) / ((float)boostHeightValueSeed) * 100);
            else boostCavernLayeroffsetSeed = 200 + (int)(((float)(boostRockLayerOffsetSeed - boostUGHeightValueSeed- boostHeightValueSeed)) / ((float)boostHeightValueSeed) * 100);

        }

        private int startIndex = 0;
        
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            //writeDebugFile(" save lokck is " + WorldGen.saveLock + " gtm " + gotToMain + " ended " + ended + " stage " + stage + " seed " + seed);
            if (ended || gotToMain || WorldGen.saveLock || stage == -1)
            {
                int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Reset"));
                if (genIndex != -1)
                {
                    tasks.Insert(genIndex, new PassLegacy("Stop Sead search", delegate (GenerationProgress progress)
                    {

                        Exit();


                    }));
                    tasks.RemoveRange(genIndex + 1, tasks.Count - genIndex - 1);
                }
            }
            else
             if (stage > 0 || gotoCreation || isInCreation)
            {
                int resetIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Reset"));   /// beter set to 0 or search for setup seed search
                startIndex = resetIndex;// should be 0 for unmodedd
                if (resetIndex != -1)
                {
                                        
                    
                    tasks.Insert(resetIndex, new PassLegacy("Setup seed options", delegate (GenerationProgress progress)
                    {
                        if (!searchForSeed)
                            progress.Message = "Setup Seed Search Options";
                        //var a = new UISearchSettings(progress, mod, this);

                        uiss.UpdateProgress(progress);

                        //while (uiss.writeTextUpdating == true || uiss.rephrasing || uiss.detailsList.isUpdating) { Thread.Sleep(60); };

                        uiss.writeTextUpdating = true;

                        Main.MenuUI.SetState(uiss);

                        uiss.writeTextUpdating = false;



                        if (!ended && !searchForSeed)
                        {
                            gotoCreation = false;
                            isInCreation = true;

                            uiss.HideUnhideProgressBar();
                            while (!ended && !searchForSeed) { clearWorld(stage); };
                            uiss.HideUnhideProgressBar();
                            isInCreation = false;

                            while (uiss.writeTextUpdating == true || uiss.rephrasing || uiss.detailsList.isUpdating || uiss.writeStats || uiss.writeText) { Thread.Sleep(10); };

                        }
                        if (ended)
                        {
                            //writeDebugFile("left loop with stage " + stage + " end?" + ended + " tasks.Count " + tasks.Count + " genInde " + genIndex);
                            stage = -1;
                        }
                        else
                        {
                            progress.Message = "Setup Configuration";
                            currentConfiguration = uiss.currentConfig;
                        }

                    }));

                    tasks.Insert(resetIndex, new PassLegacy("Continue or not ", delegate (GenerationProgress progress)
                    {
                        if (ended || !searchForSeed || gotToMain || stage < 0)
                            ClearPasses(tasks, resetIndex + 1);
                    }));


                    if (stage == 1 && !inApproxRun)
                    {
                        //other tasks not needed now
                        int sandIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sand"));
                        if (sandIndex != -1)
                            tasks.RemoveRange(sandIndex + 1, tasks.Count - sandIndex - 1);
                    }
                    else if (tasks.Count > 0 && !inApproxRun)
                    {
                        tasks.Add(new PassLegacy("Analyze World", delegate (GenerationProgress progress)
                        {
                            progress.Message = "Analyze World for Jungle chest items and rare stuff";
                        }));


                        for (int pi = tasks.Count - 2; pi > 1; pi--)
                        {
                            int val = pi;
                            tasks.Insert(pi, new PassLegacy("Continue or not ", delegate (GenerationProgress progress)
                            {
                                if (ended || !searchForSeed || gotToMain || stage < 0)
                                    ClearPasses(tasks, val + 1);
                                   

                            }));
                        }

                    }

                    
                    int terrind = tasks.FindIndex(genpass => genpass.Name.Equals("Terrain"));
                    if (terrind != -1)
                    {
                        tasks.Insert(terrind + 1, new PassLegacy("Check height ", delegate (GenerationProgress progress)
                        {
                            progress.Message = "Estimate spawn height above surface layer " + seed;

                            setHights(false);

                        }));

                        tasks.Insert(terrind + 2, new PassLegacy("Continue or not ", delegate (GenerationProgress progress)
                        {
                            if ((boostHeightValueMax != 300 && boostHeightValueSeed > boostHeightValueMax) ||
                                (boostHeightValueMin != 0 && boostHeightValueSeed < boostHeightValueMin) ||
                                (boostUGHeightValueMax != 1000 && boostUGHeightValueSeed > boostUGHeightValueMax) ||
                                (boostUGHeightValueMin != 0 && boostUGHeightValueSeed < boostUGHeightValueMin) ||
                                (boostCavernLayeroffsetMin != 0 && boostCavernLayeroffsetSeed < boostCavernLayeroffsetMin) ||
                                (boostCavernLayeroffsetMax != 300 && boostCavernLayeroffsetSeed > boostCavernLayeroffsetMax) ||
                                (boostRockLayerOffset != 0 && boostRockLayerOffsetSeed < boostRockLayerOffset) ||
                                (boostSurfRockLayerOffset != 0 && boostSurfRockLayerOffsetSeed < boostSurfRockLayerOffset) ||
                                (boostSpawnRockSeedOffsetMin != -1000 && boostSpawnRockSeedOffsetSeed < boostSpawnRockSeedOffsetMin) ||
                                (boostSpawnRockSeedOffsetMax != 100 && boostSpawnRockSeedOffsetSeed > boostSpawnRockSeedOffsetMax)
                                )
                            {
                                condsTrue = 0;
                                ClearPasses(tasks, terrind + 3);
                            }

                        }));
                    }

                    if (stage > 1)
                    {

                        int sandIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Wood Tree Walls"));
                        if (sandIndex != -1)
                            tasks.Insert(sandIndex, new PassLegacy("Check near ES", delegate (GenerationProgress progress)
                            {
                                checkAndSetIfESmidPossible(true);


                            }));
                    }

                    if (stage == 1 && !inApproxRun)
                    {
                        //other tasks not needed now


                        resetIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Reset"));
                        //credits to jopojelly added some modified version:
                        tasks.Insert(resetIndex + 1, new PassLegacy("Lookup ore and moon type and dungeon side ", (progress) => DetectDungeonSide(progress, tasks[resetIndex] as PassLegacy)));

                        tasks.Insert(resetIndex + 2, new PassLegacy("Checking ore and moon type and dungeon side ", delegate (GenerationProgress progress)
                        {
                            bool oreMoon = true;

                            if (randomInverse == true)
                            {                               
                                WorldGen.WorldGenParam_Evil = WorldGen.crimson ? 0 : 1;  //setup inverse
                                WorldGen.crimson = WorldGen.crimson? false: true;
                            }

                           //setDirectComputeValues(); //todo can be done earlier and faster for dungeon color
                            
                            if (search4MoonOres)
                            {
                                progress.Message = "Checking ore and moon type and dungeon side " + seed;

                                //stage = 23;
                                PostWorldGen(); //--> geninfo
                                oreMoon = CheckOresMoon(genInfo);

                                if (oreMoon)
                                {

                                    if ((localDungeonSide < 0 && evilHMIsLeft) || (localDungeonSide > 0 && !evilHMIsLeft))
                                    {
                                        //jungle side
                                        if (looking4hallowSide.Equals("Snow/Dungeon side"))
                                            oreMoon = false;
                                    }
                                    else
                                    {
                                        //snow side
                                        if (looking4hallowSide.Equals("Jungle side"))
                                            oreMoon = false;
                                    }
                                    if (oreMoon)
                                    {
                                        //if (!looking4dungeonWallColor.Equals("Random"))
                                        //{
                                        //    if ((looking4dungeonWallColor.Equals("Blue") && dungeonColor != 0) || (looking4dungeonWallColor.Equals("Green") && dungeonColor != 1) || (looking4dungeonWallColor.Equals("Pink") && dungeonColor != 2))
                                        //        oreMoon = false;  
                                        //}

                                        //if (oreMoon)
                                        //{
                                            if ((looking4dungeonSide.Equals("Left") && localDungeonSide > 0) || (looking4dungeonSide.Equals("Right") && localDungeonSide < 0))
                                                oreMoon = false;

                                        //}
                                    }


                                }

                            }
                            condsTrue = oreMoon ? 1 : 0;

                        }));

                        tasks.Insert(resetIndex + 3, new PassLegacy("Continue or not ", delegate (GenerationProgress progress)
                        {
                            if (condsTrue < 1)
                                ClearPasses(tasks, resetIndex + 4);

                        }));

                        
                        int sandIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sand"));
                        
                        if (sandIndex != -1)
                        {
                            tasks.Insert(sandIndex - 1, new PassLegacy("Reduce maxTilesY to skip tile Runner in Sand pass ", delegate (GenerationProgress progress)
                            {
                                if (stage == 1)
                                {
                                    progress.Message = "Speed up Sand Pass";
                                    Main.maxTilesY = -Math.Abs(Main.maxTilesY);
                                }
                            }));
                            //credits to jopojelly for replacing vanilla source code with this:
                            sandIndex++;//increased by insert                            
                            tasks.Insert(sandIndex + 1, new PassLegacy("Lookup chance of Pyramids for seed ", (progress) => DetectNumPyr(progress, tasks[sandIndex] as PassLegacy)));
                                          

                        }
                       

                    }
                    else if (stage == 1 && inApproxRun)
                    {
                        //only do reset, terrain , sand, full dester dungeon, Gravitating Sand and pyramids



                        int skipind = tasks.FindIndex(genpass => genpass.Name.Equals("Pyramids"));
                        
                        if (skipind != -1)
                        {
                            tasks.Insert(skipind - 1, new PassLegacy("Approximate Pyramid count ", delegate (GenerationProgress progress)
                            {
                                int y = (int)(Main.rockLayer + 0.5 * (Main.maxTilesY - Main.rockLayer - 200));
                                progress.Message = "Approximate Pyramid count " + seed;
                                
                                for (int x = 0; x < Main.maxTilesX; x++)
                                {
                                    //Main.tile[x, y].type = TileID.LihzahrdBrick;
                                    Main.tile[x, y].active(false);
                                }

                            }));
                            if (tasks.Count > skipind + 2)
                                ClearPasses(tasks, skipind + 2);
                            tasks.Add( new PassLegacy("Checkout Pyramid count ", delegate (GenerationProgress progress)
                            {
                                
                                if (boostPyramidValue > 0)
                                {
                                    
                                    progress.Message = "Checkout Pyramid count " + seed;
                                    int pyrPlaced = 0;
                                    for (int i = 0; i < Main.maxChests; i++)
                                    {
                                        if (Main.chest[i] != null)
                                        {
                                            Chest chest = Main.chest[i];
                                            //check if doubl chest
                                            int cx = chest.x;
                                            int cy = chest.y;
                                            int iid = chest.item[0].type;
                                            if (iid == ItemID.PharaohsMask || iid == ItemID.PharaohsRobe || iid == ItemID.FlyingCarpet || iid == ItemID.SandstorminaBottle)
                                                pyrPlaced++;

                                        }
                                    }

                                    if (pyrPlaced < boostPyramidValue)
                                        condsTrue = 0;

                                    boostPyramidValueSeed = pyrPlaced;
                                    
                                }

                            }));

                            int cloudindex = tasks.FindIndex(genpass => genpass.Name.Equals("Floating Islands"));
                            if (cloudindex > 0)
                            {
                                tasks.Insert(cloudindex - 1, new PassLegacy("search for good clouds ", delegate (GenerationProgress progress)
                                {
                                    if (condsTrue == 0 || (boostCloudValue <= 0 && boostCloudInner2Value <= 0))
                                    {
                                        ClearPasses(tasks, cloudindex + 1, cloudindex + 2);
                                    }

                                }));


                                tasks.Insert(cloudindex + 2, new PassLegacy("search for good clouds ", delegate (GenerationProgress progress)
                                {
                                    if ( (boostCloudValue > 0 || boostCloudInner2Value > 0))
                                    {
                                        int turn = -1;


                                        int index = cloudindex;

                                        while (true)
                                        {

                                            turn++;
                                            var cloudPos = new HashSet<int>(); //change to list

                                        //todo: no border
                                        for (int x = 0; x < Main.maxTilesX; x++)
                                            {
                                                for (int y = 0; y < Main.worldSurface - 50; y++) // changed to rocklayer
                                            {
                                                    var tile = Main.tile[x, y];

                                                    if (tile.active())
                                                    {
                                                    //Clouds
                                                    if (tile.type == TileID.Cloud)
                                                        {

                                                            int area = 120;
                                                            bool found = false;
                                                            for (int i = -area; i < area; i++)
                                                            {
                                                                if (cloudPos.Contains(x + i))
                                                                {
                                                                    found = true;
                                                                    if (Math.Abs(i) > 50)
                                                                    {
                                                                        cloudPos.Remove(x + i);
                                                                        int newPos = x + i - Math.Sign(i);
                                                                        cloudPos.Add(newPos);

                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                            if (!found) cloudPos.Add(x);
                                                        }
                                                    }
                                                }
                                            }

                                            int closeToMid = 0;
                                            int maxc = 0;
                                            int minL = Main.maxTilesX;
                                            int minR = Main.maxTilesX;
                                            foreach (var cloudX in cloudPos)
                                            {
                                                int midDist = Math.Abs(Main.maxTilesX / 2 - cloudX);
                                                if (midDist < boostCloudValue)
                                                    closeToMid++;
                                                if (cloudX < Main.maxTilesX / 2)
                                                    minL = Math.Min(minL, midDist);
                                                else
                                                    minR = Math.Min(minR, midDist);
                                                if (midDist > maxc)
                                                {
                                                    maxc = midDist;
                                                }
                                            }
                                            int distInner2 = (minL + minR) / 2;
                                            string pm = "Seed " + seed + " has " + cloudPos.Count + " clouds out of those in mid " + closeToMid;
                                            progress.Message = pm;
                                        //writeDebugFile("Checked clouds for seed " + seed + " :" + closeToMid + " of " + cloudPos.Count);
                                        if ((boostCloudNumValue > 0 && closeToMid < boostCloudNumValue) || (boostCloudInner2Value > 0 && distInner2 > boostCloudInner2Value))
                                            {
                                                for (int x = 0; x < Main.maxTilesX; x++)
                                                {
                                                    for (int y = 0; y < Main.worldSurface - 50; y++)
                                                    {
                                                        Main.tile[x, y].ClearEverything();
                                                    }
                                                }

                                                bool good = false;
                                                int normFac = Main.maxTilesX;
                                                if (boostCloudValue > 0 && boostCloudNum1stLakeValue == 1) normFac = boostCloudValue;
                                                else if (boostCloudInner2Value > 0 && (boostCloudNum1stLakeValue == 2 || (boostCloudValue <= 0 && boostCloudNum1stLakeValue == 1))) normFac = boostCloudInner2Value * 2 - 80;//inner can be shifted

                                            while (!good)
                                                {
                                                    double val = (new UnifiedRandom(++seed)).NextDouble();
                                                    if (boostCloudNum1stLakeValue == 0 || (val - 0.5 > (80.0) / (0.8 * Main.maxTilesX) && (val - 0.5) <= ((double)normFac) / (0.8 * Main.maxTilesX)))  //small also works but less pyramids Math.Abs(val-0.5)
                                                    good = true;
                                                }

                                                Main.ActiveWorldFileData.SetSeed((seed).ToString());
                                                seed = Main.ActiveWorldFileData.Seed;
                                                WorldGen._lastSeed = seed;
                                                Main.rand = new UnifiedRandom(seed);


                                                index = tasks.FindIndex(genpass => genpass.Name.Equals("Floating Islands"));
                                                if (index > 0)
                                                {
                                                    tasks[index].Apply(progress);

                                                }



                                            }
                                            else if (turn != 0)
                                            {
                                                Main.ActiveWorldFileData.SetSeed((seed - stepsize).ToString()); // TODO may be error if overflow
                                                seed = Main.ActiveWorldFileData.Seed;
                                                stage = 1;
                                                ClearPasses(tasks, cloudindex + 2);
                                                condsTrue = 0;
                                                break;
                                            }
                                            else
                                            {
                                                if (boostCloudValue > maxc && !isPubRel)
                                                {
                                                    writeDebugFile("min cloud dist " + maxc + " from seed " + seed);
                                                }
                                                break;
                                            }
                                            if (stage > 0 && !ended && !gotToMain && !loadWorld && !gotoCreation)
                                            { }
                                            else
                                            {
                                                ClearPasses(tasks, cloudindex + 2);
                                                stage = 1;
                                                condsTrue = 0;
                                                break;
                                            }

                                        }
                                    }
                                }));

                            }



                        }


                        //only do reset, terrain , sand, full dester dungeon, Gravitating Sand and pyramids

                        int ind = tasks.FindIndex(genpass => genpass.Name.Equals("Tunnels")); if (ind != -1) ClearPasses(tasks, ind, ind + 1);

                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Mount Caves")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Dirt Wall Backgrounds")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Rocks In Dirt")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Dirt In Rocks")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Clay")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Small Holes")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Dirt Layer Caves")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Rock Layer Caves")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Surface Caves")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Slush Check")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Grass")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Marble")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Granite")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Mud Caves To Grass")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        //ind = tasks.FindIndex(genpass => genpass.Name.Equals("Floating Islands")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Mushroom Patches")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Mud To Dirt")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Silt")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Webs")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Underworld")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Corruption")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Slush")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Mud Caves To Grass")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Beaches")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Gems")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                        ind = tasks.FindIndex(genpass => genpass.Name.Equals("Clean Up Dirt")); if (ind != -1)  ClearPasses(tasks, ind, ind + 1);
                                                                        

                    }
                    else if (stage == 2) ////////// or only else
                    {
                        int woodIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Check near ES"));//==Wood Tree Walls +1
                        if (woodIndex != -1)
                        {
                            tasks.Insert(woodIndex + 1, new PassLegacy("Analyze World basic", delegate (GenerationProgress progress)
                            {
                                progress.Message = "Analyze World for Pyramids and Living Trees";
                                PostWorldGen(); //--> geninfo

                                analyzeWorld(score, genInfo, false);
                                


                                condsTrue = acond.checkConditions(score, currentConfiguration, stage);  //-near tree, -cloud, -pyramid, +capet & sandbottle or +3Py C | SB
                                rareMax = score.rare;
                                //writeDebugFile(seed + " left check conditoin sage 2 points " + score.points + " rare " + score.rare + " condsTrue " + condsTrue);

                                //WorldFile.saveWorld(Main.ActiveWorldFileData.IsCloudSave, true); //DEEEEEEEEEEEEEEEEEEEEBUGGGGGGGGGGGGGG

                                //clearWorld(stage);

                                if (!statsUpdated)
                                {
                                    numPyramidChanceTrue++;
                                    hasCount[numPyrChance, score.pyramids] += 1;
                                    chanceCount[numPyrChance] -= 1;
                                    uiss.ChangeCountText(hasCount, chanceCount, numPyrChance, score.pyramids);
                                    statsUpdated = true;

                                    if (boostPyramidValue > 0 && score.pyramids > boostPyramidValueSeed && !isPubRel)
                                        writeDebugFile("seed " + seed + " has more pyramids than expected (" + boostPyramidValueSeed + " vs " + score.pyramids + ") out of " + numPyramidChanceTrue);
                                }

                                lastSeed = seed; lastStage = 2;
                                //writeToDescList(uiss.countText, -2);
                                writeToDescList(GenerateStatsText(), -2);


                                // writeDebugFile(" seed " + seed + " chance " + numPyrChance + "("+ genInfo.numPyrChance+") has pyramid " + score.pyramids + "("+genInfo.numPyramids+") cond true " + condsTrue + " with point " + score.points );

                                if (!continueEval && !score.phase3Empty) condsTrue = Math.Min(condsTrue, stage);


                                //writeDebugFile("2run " + seed + " cond " + condsTrue +  " rare " + score.rare + "(" + rareMax + ")");

                                //has it pyramids, trees, near to mid? 
                                if (condsTrue > 1 || score.rare > 0)
                                {
                                    //if (condsTrue > 2 || score.rare > 0)
                                    if (score.rare > 0)
                                    {
                                        passedStage[stage]++;
                                        //writeDebugFile(seed + "in take all 2 points " + score.points + " rare " + score.rare + " condsTrue " + condsTrue);
                                        //writeDebugFile("build seed " + seed + " chance " + numPyrChance + " has pyramid " + score.pyramids + " cond true " + condsTrue);
                                        stage = 42; //it has the number of pyramids you are looked for
                                        passedStage[3]++;
                                    }
                                    else
                                    {
                                        passedStage[stage]++;
                                        stage++;
                                    }
                                }
                                else
                                    startNextSeed();




                            }));
                            tasks.Insert(woodIndex + 2, new PassLegacy("Continue or not ", delegate (GenerationProgress progress)
                            {
                                if (stage < 2)
                                    ClearPasses(tasks, resetIndex + 3);

                            }));
                        }



                       
                        


                    }



                }
            }



        }



        //from jopojelly to replace vanilla code
        private void DetectNumPyr(GenerationProgress progress, PassLegacy SandPass)
        {

            Main.maxTilesY = Math.Abs(-Main.maxTilesY);
            progress.Message = "Looking chance of Pyramids for seed " + seed;
            FieldInfo generationMethod = typeof(PassLegacy).GetField("_method", BindingFlags.Instance | BindingFlags.NonPublic);
            WorldGenLegacyMethod method = (WorldGenLegacyMethod)generationMethod.GetValue(SandPass);

            var numPyrField = method.Method.DeclaringType?.GetFields
            (
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.Static
            )
            .Single(x => x.Name == "numPyr");
            localnumPyr = (int)numPyrField.GetValue(method.Target);



            var pyrXField = method.Method.DeclaringType?.GetFields
            (
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.Static
            )
            .Single(x => x.Name == "PyrX");
            localPyrX = (int[])pyrXField.GetValue(method.Target);

            var pyrYField = method.Method.DeclaringType?.GetFields
            (
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.Static
            )
            .Single(x => x.Name == "PyrY");
            localPyrY = (int[])pyrYField.GetValue(method.Target);

            

        }
             

        //from jopojelly modified version
        private void DetectDungeonSide(GenerationProgress progress, PassLegacy ResetPass)
        {
            progress.Message = "Lookup ore and moon type and dungeon side " + seed;
            FieldInfo generationMethod = typeof(PassLegacy).GetField("_method", BindingFlags.Instance | BindingFlags.NonPublic);
            WorldGenLegacyMethod method = (WorldGenLegacyMethod)generationMethod.GetValue(ResetPass);
            var dungeonSideField = method.Method.DeclaringType?.GetFields
            (
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.Static
            )
            .Single(x => x.Name == "dungeonSide");
            localDungeonSide = (int)dungeonSideField.GetValue(method.Target);

        }


        public override void PostWorldGen()
        {
            if (ended || gotToMain)
            {
                WorldGen.saveLock = true;
                saveLogSet = true;
            }
            else
            {
                if (stage > 1 && stage != 23)
                {
                    genInfo = reviewWhatWasDone(); //checks pyramid, lving tree, clouds and their distance
                    genInfo.numPyrChance = numPyrChance;
                }
                if (stage > 0)
                {
                    if(stage==1) genInfo = new generatorInfo();
                    
                    genInfo.copperOrTin = WorldGen.CopperTierOre == 7 ? "Copper" : "Tin";
                    genInfo.ironOrLead = WorldGen.IronTierOre == 6 ? "Iron" : "Lead";
                    genInfo.silverOrTung = WorldGen.SilverTierOre == 9 ? "Silver" : "Tungsten";
                    genInfo.goldOrPlat = WorldGen.GoldTierOre == 8 ? "Gold" : "Platinum";
                    genInfo.moonType = Main.moonType == 0 ? "White" : Main.moonType == 1 ? "Orange" : Main.moonType == 2 ? "Green" : "Random";
                }
                
            }

        }

        
        private void ComputePathlength(ref int[,] pathLength)
        {
            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime;

            int maxsize = (Main.maxTilesX * 6 + Main.maxTilesY * 4); // values can be greater!
            List<Tuple<int, int>>[] waypoints = new List<Tuple<int, int>>[maxsize];
            short[,] travelCost = new short[Main.maxTilesX, Main.maxTilesY];

            pathLength = new int[Main.maxTilesX, Main.maxTilesY];



            for (int x = 0; x < Main.maxTilesX; x++)
                for (int y = 0; y < Main.maxTilesY; y++)
                    pathLength[x, y] = Int32.MaxValue;           

            for (int x = 0; x < Main.maxTilesX; x++)
                for (int y = 0; y < Main.maxTilesY; y++)
                    travelCost[x, y] = (short)TravelCost(x, y);

            

            pathLength[Main.spawnTileX, Main.spawnTileY - 1] = 0;
            waypoints[0] = new List<Tuple<int, int>> { new Tuple<int, int>(Main.spawnTileX, Main.spawnTileY - 1) };

            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            //writeDebugFile(" analyze time after init pathfinding " + elapsedTime);
            
            
          
            for (int l = 0; l < maxsize; l++)
            {
                if (waypoints[l] != null)
                {
                    foreach (Tuple<int, int> p in waypoints[l])
                        AddWayPoints(ref pathLength, ref waypoints, ref travelCost, p.Item1, p.Item2, l);
                    waypoints[l].Clear();
                }
            }
                        

            //norm to ~tiles num --> pathlengthNormFac
            //for (int x = 0; x < Main.maxTilesX; x++)
            //    for (int y = 0; y < Main.maxTilesY; y++)
            //        if (pathLength[x, y] != Int32.MaxValue)
            //            pathLength[x, y] = (int)(0.2 * pathLength[x, y]);

            travelCost = null;
            waypoints = null;

            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            //writeDebugFile(" analyze time after pathfinding " + elapsedTime);

            //only for debug
            if (1 == 42)
            {
                int dimX = Main.maxTilesX;
                int dimY = Main.maxTilesY;
                int scale = 1;
                /*while (dimX > 6200)
                {
                    dimX /= 2;
                    dimY /= 2;
                    scale *= 2;
                }*/

                //https://msdn.microsoft.com/en-us/library/system.drawing.imaging.bitmapdata(v=vs.110).aspx
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(dimX, dimY, PixelFormat.Format32bppArgb);


                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);


                // Get the address of the first line.
                IntPtr ptr = bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                byte[] rgbValues = new byte[bytes];

                // Copy the RGB values into the array.
                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                int indx = 0;
                for (int y = 0; y < Main.maxTilesY; y += scale)
                    for (int x = 0; x < Main.maxTilesX; x += scale)
                    {
                        //MapTile cur = MapHelper.CreateMapTile(x, y, 255);
                        //Color cc = MapHelper.GetMapTileXnaColor(ref cur);
                        //int cv = (int)(((float)pathLength[x, y]) / Main.maxTilesX/7 * 255.0);                            
                        //int cv = (int)(((float)pathLength[x, y]) / (0.2 * maxsize) * 255.0);
                        int cv = (int)(((float)pathLength[x, y]) / (maxsize) * 255.0);
                        cv = cv > 255 ? 255 : cv;
                        rgbValues[indx++] = (byte)(255 - cv);
                        rgbValues[indx++] = (byte)(255 - cv);
                        rgbValues[indx++] = (byte)(255 - cv);
                        rgbValues[indx++] = 255;

                    }

                //draw Spawm
                int aw = 0;

                for (int y = Main.spawnTileY - 1; y > Main.spawnTileY - 36; y--)
                {
                    int x = Main.spawnTileX;
                    int off = y * 4 * Main.maxTilesX + x * 4;

                    rgbValues[off + 0] = 0;
                    rgbValues[off + 1] = 80;
                    rgbValues[off + 2] = 50;
                    rgbValues[off + 3] = 255;

                    for (int awi = 0; awi < (aw < 18 ? aw : 4); awi++)
                    {
                        rgbValues[off + 0 + 4 * (awi / 3)] = 0;
                        rgbValues[off + 1 + 4 * (awi / 3)] = 80;
                        rgbValues[off + 2 + 4 * (awi / 3)] = 50;
                        rgbValues[off + 3 + 4 * (awi / 3)] = 255;

                        rgbValues[off + 0 - 4 * (awi / 3)] = 0;
                        rgbValues[off + 1 - 4 * (awi / 3)] = 80;
                        rgbValues[off + 2 - 4 * (awi / 3)] = 50;
                        rgbValues[off + 3 - 4 * (awi / 3)] = 255;
                    }
                    aw++;
                }


                // Copy the RGB values back to the bitmap
                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

                // Unlock the bits.
                bmp.UnlockBits(bmpData);
                bmp.Save(Main.WorldPath + @"\" + seed + "_paths.png");


            }


        }



        private void analyzeWorld(ScoreWorld score, generatorInfo genInfo, bool doFull, bool doBulbExtra = false)
        {


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            score.clear(); score.init();

            Dictionary<string, int> hasOBjectOrParam = score.hasOBjectOrParam; //Todo only ref
            hasOBjectOrParam.Clear();

            score.insertGenInfo(genInfo);

            //TODo better inclusion
            localDungeonSide = Main.dungeonX < Main.maxTilesX / 2 ? -1 : 1; // changed from spawnx
            setDirectComputeValues();
            
            const int oceanBiomeSizeX = 380; //from vanilla

            hasOBjectOrParam.Add("Max open air pyramid surface", 0);
            hasOBjectOrParam.Add("Max pyramid height", 0);
            hasOBjectOrParam.Add("Max pyramid tunnel height", 0);
            hasOBjectOrParam.Add("Max pyramid total height", 0);
            hasOBjectOrParam.Add(OptionsDict.Phase2.maxPyrExitCavDist, -100000);
            hasOBjectOrParam.Add("Ocean Pyramid", 0);

            hasOBjectOrParam.Add("Enchanted Sword Shrine", 0);
            hasOBjectOrParam.Add("Enchanted Sword", 0);

            hasOBjectOrParam.Add("Near Enchanted Sword", 0); // at least world generation depth 2
            hasOBjectOrParam.Add("Very Near Enchanted Sword", 0);
            hasOBjectOrParam.Add("Enchanted Sword near Tree", 0);
            hasOBjectOrParam.Add("Near Enchanted Sword near Tree", 0);
            hasOBjectOrParam.Add("Enchanted Sword near Pyramid", 0);
            hasOBjectOrParam.Add("Near Enchanted Sword near Pyramid", 0);


            hasOBjectOrParam.Add("Meteorite Bar unlocked", 0);
            hasOBjectOrParam.Add("Biome Item in normal Chest", 0);



            hasOBjectOrParam.Add("Cloud Chest", 0);
            hasOBjectOrParam.Add("Tree", 0);
            hasOBjectOrParam.Add("Tree Chest", 0);
            hasOBjectOrParam.Add("Tree near mid open to mid", 0);
            hasOBjectOrParam.Add("Max Living Tree Size", 0);
            hasOBjectOrParam.Add("Min Living Tree Size", 0);
            hasOBjectOrParam.Add("Max Living Tree root Size", 0);
            hasOBjectOrParam.Add("Max Living Tree total Size", 0);
            hasOBjectOrParam.Add(OptionsDict.Phase2.maxTreeExitCavDist, -100000);
            hasOBjectOrParam.Add("Tree to cavern layer", 0);
            hasOBjectOrParam.Add("Tree to cavern layer near mid", 0);
            hasOBjectOrParam.Add("Tree close to cavern layer", 0);
            hasOBjectOrParam.Add("Tree close to cavern layer near mid", 0);

            hasOBjectOrParam.Add("Trees near mid", 0);
            hasOBjectOrParam.Add("Tree chests near mid", 0);            
            hasOBjectOrParam.Add("Near Tree", 0);
            hasOBjectOrParam.Add("Near Tree Chest", 0);            
            hasOBjectOrParam.Add("Distance Tree Chest to mid", 100000);

            hasOBjectOrParam.Add("Lake near mid (guess)", 0);
            hasOBjectOrParam.Add("Water / Duck Score (guess)", 0);

            hasOBjectOrParam.Add("Water Bolt before Skeletron", 0);
            hasOBjectOrParam.Add("Water Bolt", 0);

            hasOBjectOrParam.Add("Temple door distance", 0);
            hasOBjectOrParam.Add("Temple door horizontal distance", 0);
            hasOBjectOrParam.Add("Temple Tile horizontal distance", 0);
            hasOBjectOrParam.Add("Temple Tile vertical distance", 0);
            hasOBjectOrParam.Add("neg. Temple door distance", 0);
            hasOBjectOrParam.Add("neg. Temple door horizontal distance", 0);
            hasOBjectOrParam.Add("neg. Temple Tile horizontal distance", 0);
            hasOBjectOrParam.Add("neg. Temple Tile vertical distance", 0);
            

            hasOBjectOrParam.Add("Temple at player side of jungle (%)", 0);
            hasOBjectOrParam.Add("Temple at ocean side of jungle (%)", 0);
            hasOBjectOrParam.Add("Temple at height (%)", 0);
            hasOBjectOrParam.Add("Temple at depth (%)", 0);



            hasOBjectOrParam.Add("Near Sunflower", 0);
            hasOBjectOrParam.Add("Near Altar", 0);
            hasOBjectOrParam.Add("Near Spider Web count", 0);
            hasOBjectOrParam.Add("Near Mushroom Biome count", 0);            
            hasOBjectOrParam.Add("Near Chest", 0);

            hasOBjectOrParam.Add("Near Cloud", 0);

            hasOBjectOrParam.Add("Nearest Teleportation Potion count", 0);

            hasOBjectOrParam.Add("Pathlength to Teleport Potion", 1000000);
            hasOBjectOrParam.Add("Pathlength to 2 Teleport Potion Chest", 1000000);

            hasOBjectOrParam.Add("Pathlength to 2nd Teleport Potion", 1000000);
            hasOBjectOrParam.Add("Pathlength to Lava Charm", 1000000);
            hasOBjectOrParam.Add("Pathlength to Water Walking Boots", 1000000);
            hasOBjectOrParam.Add("Pathlength to Fish Pet", 1000000);
            hasOBjectOrParam.Add("Pathlength to Seaweed Pet", 1000000);

            hasOBjectOrParam.Add("Pathlength to Meteorite Bar", 1000000);
            hasOBjectOrParam.Add("Pathlength to Obsidian Skin Potion", 1000000);
            hasOBjectOrParam.Add("Pathlength to Battle Potion", 1000000);
            hasOBjectOrParam.Add("Pathlength to Lifeforce Potion", 1000000);
            hasOBjectOrParam.Add("Pathlength to Recall Potion", 1000000);
            hasOBjectOrParam.Add("Pathlength to Builder Potion", 1000000);
            hasOBjectOrParam.Add("Pathlength to Rope", 1000000);
            hasOBjectOrParam.Add("Pathlength to Copper/Tin Bar", 1000000);
            hasOBjectOrParam.Add("Pathlength to Iron/Lead Bar", 1000000);
            hasOBjectOrParam.Add("Pathlength to 10 Iron/Lead Bar Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to 12 Iron/Lead Bar Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to Silver/Tungsten Bar", 1000000);
            hasOBjectOrParam.Add("Pathlength to Gold/Platinum Bar", 1000000);

            hasOBjectOrParam.Add("Pathlength to Bomb", 1000000);
            hasOBjectOrParam.Add("Pathlength to Dynamite", 1000000);
            hasOBjectOrParam.Add("Pathlength to 2nd Dynamite", 1000000);
            hasOBjectOrParam.Add("Pathlength to Gravitation Potion", 1000000);
            hasOBjectOrParam.Add("Pathlength to Crystal Heart", 1000000);
            hasOBjectOrParam.Add("Pathlength to 2nd Crystal Heart", 1000000);
            hasOBjectOrParam.Add("Pathlength to Jester's Arrow", 1000000);

            hasOBjectOrParam.Add("Pathlength to Boots", 1000000);
            hasOBjectOrParam.Add("Pathlength to Enchanted Sword", 1000000);
            hasOBjectOrParam.Add("Pathlength to Altar", 1000000);
            hasOBjectOrParam.Add("Pathlength to Bee Hive", 1000000);
            hasOBjectOrParam.Add("Pathlength to Temple Door", 1000000);
            hasOBjectOrParam.Add("Pathlength to Temple Tile", 1000000);
            hasOBjectOrParam.Add("Pathlength to free ShadowOrb/Heart", 1000000);
            hasOBjectOrParam.Add("Pathlength to Chest duplication Glitch", 1000000);
            hasOBjectOrParam.Add("Pathlength to Pot dupl. Glitch", 1000000);
            hasOBjectOrParam.Add("Pathlength to Pot dupl. Glitch Single", 1000000);
            hasOBjectOrParam.Add("Pathlength to Life Crystal dupl. Glitch", 1000000);
            hasOBjectOrParam.Add("Pathlength to Life Crystal dupl. Glitch Single", 1000000);
            hasOBjectOrParam.Add("Pathlength to Floating dupl. Glitch structure", 1000000);
            hasOBjectOrParam.Add("Pathlength to Boomstick", 1000000);
            hasOBjectOrParam.Add("Pathlength to Flower Boots", 1000000);
            hasOBjectOrParam.Add("Pathlength to Suspicious Looking Eye", 1000000);
            hasOBjectOrParam.Add("Pathlength to Snowball Cannon", 1000000);

            hasOBjectOrParam.Add("Pathlength to Slime Staute", 1000000);
            hasOBjectOrParam.Add("Pathlength to Shark Staute", 1000000);
            hasOBjectOrParam.Add("Pathlength to Heart Staute", 1000000);
            hasOBjectOrParam.Add("Pathlength to Star Staute", 1000000);
            hasOBjectOrParam.Add("Pathlength to Anvil", 1000000);
            hasOBjectOrParam.Add("Pathlength to Ruby", 1000000);
            hasOBjectOrParam.Add("Pathlength to Diamond", 1000000);
            hasOBjectOrParam.Add("Pathlength to Cloud in a Bottle", 1000000);
            hasOBjectOrParam.Add("Pathlength to 2 Herb Bag Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to Grenades", 1000000);
            hasOBjectOrParam.Add("Pathlength to Extractinator", 1000000);
            hasOBjectOrParam.Add("Pathlength to Detonator", 1000000);
            hasOBjectOrParam.Add("Pathlength to Explosives", 1000000);
            hasOBjectOrParam.Add("Pathlength to 2nd Explosives", 1000000);
            hasOBjectOrParam.Add("Pathlength to Magic/Ice Mirror", 1000000);
            hasOBjectOrParam.Add("Pathlength to Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to 2nd Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to 3rd Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to 4th Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to 5th Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to underground Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to 2nd underground Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to 3rd underground Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to 4th underground Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to 5th underground Chest", 1000000);



            hasOBjectOrParam.Add("Pathlength to Wooden Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to Golden Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to Water Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to Tree Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to Pyramid Chest", 1000000);
            hasOBjectOrParam.Add("Pathlength to cabin", 1000000);
            hasOBjectOrParam.Add("Pathlength to Minecart Track", 1000000);

            hasOBjectOrParam.Add("Pathlength to underground MarbleGranite", 1000000);
            hasOBjectOrParam.Add("Pathlength into cavern layer", 1000000);
            hasOBjectOrParam.Add("Pathlength into 40% cavern layer", 1000000);
            hasOBjectOrParam.Add("Pathlength to 40% cavern entrance", 1000000);
            hasOBjectOrParam.Add("Tiles to mine for 40% cavern", 1000000);

            hasOBjectOrParam.Add("Pathlength to cavern entrance to mid of Jungle", 1000000);
            hasOBjectOrParam.Add("Tiles to mine for mid Jungle cavern", 1000000);
            hasOBjectOrParam.Add("Pathlength to cavern entrance to deep Jungle", 1000000);
            hasOBjectOrParam.Add("Tiles to mine for deep Jungle cavern", 1000000);





            hasOBjectOrParam.Add("neg. Pathlength to Teleport Potion", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 2 Teleport Potion Chest", -1000000);

            hasOBjectOrParam.Add("neg. Pathlength to 2nd Teleport Potion", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Lava Charm", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Water Walking Boots", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Fish Pet", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Seaweed Pet", -1000000);

            hasOBjectOrParam.Add("neg. Pathlength to Meteorite Bar", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Obsidian Skin Potion", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Battle Potion", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Lifeforce Potion", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Recall Potion", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Builder Potion", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Rope", -1000000);

            hasOBjectOrParam.Add("neg. Pathlength to Copper/Tin Bar", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Iron/Lead Bar", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 10 Iron/Lead Bar Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 12 Iron/Lead Bar Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Silver/Tungsten Bar", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Gold/Platinum Bar", -1000000);

            hasOBjectOrParam.Add("neg. Pathlength to Bomb", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Dynamite", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 2nd Dynamite", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Gravitation Potion", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Crystal Heart", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 2nd Crystal Heart", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Jester's Arrow", -1000000);

            hasOBjectOrParam.Add("neg. Pathlength to Boots", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Enchanted Sword", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Altar", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Bee Hive", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Temple Door", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Temple Tile", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to free ShadowOrb/Heart", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Chest duplication Glitch", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Pot dupl. Glitch", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Pot dupl. Glitch Single", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Life Crystal dupl. Glitch", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Life Crystal dupl. Glitch Single", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Floating dupl. Glitch structure", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Boomstick", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Flower Boots", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Suspicious Looking Eye", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Snowball Cannon", -1000000);

            hasOBjectOrParam.Add("neg. Pathlength to Slime Staute", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Shark Staute", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Heart Staute", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Star Staute", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Anvil", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Ruby", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Diamond", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Cloud in a Bottle", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 2 Herb Bag Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Grenades", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Extractinator", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Detonator", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Explosives", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 2nd Explosives", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Magic/Ice Mirror", 1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 2nd Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 3rd Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 4th Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 5th Chest", -1000000);

            hasOBjectOrParam.Add("neg. Pathlength to underground Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 2nd underground Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 3rd underground Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 4th underground Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 5th underground Chest", -1000000);

      
            hasOBjectOrParam.Add("neg. Pathlength to Wooden Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Golden Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Water Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Tree Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Pyramid Chest", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to cabin", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Minecart Track", -1000000);

            hasOBjectOrParam.Add("neg. Pathlength to underground MarbleGranite", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength into cavern layer", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength into 40% cavern layer", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to 40% cavern entrance", -1000000);
            hasOBjectOrParam.Add("neg. Tiles to mine for 40% cavern", -1000000);

            hasOBjectOrParam.Add("neg. Pathlength to cavern entrance to mid of Jungle", -1000000);
            hasOBjectOrParam.Add("neg. Tiles to mine for mid Jungle cavern", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to cavern entrance to deep Jungle", -1000000);
            hasOBjectOrParam.Add("neg. Tiles to mine for deep Jungle cavern", -1000000);




            hasOBjectOrParam.Add("Free cavern to mid Jungle", 0);
            hasOBjectOrParam.Add("Free cavern to deep Jungle", 0);
            hasOBjectOrParam.Add("Jungle cavern not blocked by structure", 0);


            hasOBjectOrParam.Add("Free ShadowOrb/Heart", 0);
            hasOBjectOrParam.Add("Distance ShadowOrb/Heart to mid", 1000000);
            hasOBjectOrParam.Add("Distance Lake to mid (guess)", 1000000);

            hasOBjectOrParam.Add("neg. Distance Tree to mid", -1000000);
            hasOBjectOrParam.Add("neg. Distance Tree Chest to mid", -1000000);
            hasOBjectOrParam.Add("neg. Distance Cloud to mid", -1000000);
            hasOBjectOrParam.Add("neg. Distance Pyramid to mid", -1000000);
            hasOBjectOrParam.Add("neg. Distance Dungeon to mid", -1000000);
            hasOBjectOrParam.Add("neg. Distance ShadowOrb/Heart to mid", -1000000);
            hasOBjectOrParam.Add("neg. Distance Lake to mid (guess)", -1000000);

            hasOBjectOrParam.Add("neg. Jungle biome distance to mid", -1000000);
            hasOBjectOrParam.Add("neg. Snow biome distance to mid", -1000000);
            hasOBjectOrParam.Add("neg. Evil biome distance to mid", -1000000);
            hasOBjectOrParam.Add("neg. MarbleGranite at surf dist. to mid", -1000000);
            
            hasOBjectOrParam.Add("neg. Top MarbleGranite dist. to spawn (guess)", -1000000);
            hasOBjectOrParam.Add("neg. UG MarbleGranite dist. to spawn (guess)", -1000000);
           


            hasOBjectOrParam.Add("Bee Hives", 0);
            hasOBjectOrParam.Add("High Hive", 0);
            hasOBjectOrParam.Add("Open Bee Hive below lava", 0);


            hasOBjectOrParam.Add("Spawn in Sky", 0);

            hasOBjectOrParam.Add(OptionsDict.Phase3.openTemple, 0);
            hasOBjectOrParam.Add("Mushroom Biome above surface count", 0);
            

            hasOBjectOrParam.Add("Evil Tiles for Mud", 0);
            hasOBjectOrParam.Add("Evil Tiles for Jungle Grass", 0);
            hasOBjectOrParam.Add("Evil Tiles for Sand", 0);
            hasOBjectOrParam.Add("Evil Tiles for Ice", 0);

            hasOBjectOrParam.Add("Ice surface evil", 0);
            hasOBjectOrParam.Add("Ice Surface", 0);
            hasOBjectOrParam.Add("Snow biome surface overlap mid", 0);
            hasOBjectOrParam.Add("Jungle biome surface overlap mid", 0);
            hasOBjectOrParam.Add("Jungle biome distance to mid", 100000);
            hasOBjectOrParam.Add("Snow biome distance to mid", 100000);
            hasOBjectOrParam.Add("Evil biome distance to mid", 100000);
            hasOBjectOrParam.Add("MarbleGranite at surf dist. to mid", 100000);
            hasOBjectOrParam.Add("Top MarbleGranite dist. to spawn (guess)", 100000* 10000);
            hasOBjectOrParam.Add("UG MarbleGranite dist. to spawn (guess)", 100000* 10000);



            hasOBjectOrParam.Add("Nearest Evil left Ocean", Main.maxTilesX);
            hasOBjectOrParam.Add("Nearest Evil right Ocean", 0);
            hasOBjectOrParam.Add("Has evil Ocean", 0);
            hasOBjectOrParam.Add("Has evil Dungeon Ocean", 0);
            hasOBjectOrParam.Add("Has evil Jungle Ocean", 0);


            hasOBjectOrParam.Add("Hermes Flurry Boots Distance", 100000);


            hasOBjectOrParam.Add("Nearest Altar left beach", 100000);
            hasOBjectOrParam.Add("Nearest Altar right beach", 100000);
            hasOBjectOrParam.Add("Nearest Altar Dungeon beach", 100000);
            hasOBjectOrParam.Add("Nearest Altar Jungle beach", 100000);
            hasOBjectOrParam.Add("Beach left", 0);
            hasOBjectOrParam.Add("Beach right", Main.maxTilesX - 1);
            hasOBjectOrParam.Add("Dungeon beach", -1);
            hasOBjectOrParam.Add("Jungle beach", -1);
            hasOBjectOrParam.Add("Beach penalty left", 0);
            hasOBjectOrParam.Add("Beach penalty right", 0);
            hasOBjectOrParam.Add("Beach penalty mean", 0);
            hasOBjectOrParam.Add("Beach penalty max", 0);
            hasOBjectOrParam.Add("No Ocean", 0);

            hasOBjectOrParam.Add("Surface average height (aprox.)", 0);
            hasOBjectOrParam.Add("Surface height (sqrt) variance", 0);
            hasOBjectOrParam.Add("Surface max-min height", 0);
            hasOBjectOrParam.Add("Underground layer height", (int)Math.Abs(Main.rockLayer - Main.worldSurface));
            hasOBjectOrParam.Add("neg. Underground layer height", (int)-Math.Abs(Main.rockLayer - Main.worldSurface ));

            int spawnHightGuess = guessSpawnHight(true);
            hasOBjectOrParam.Add("Underground Distance to spawn (guess)", spawnHightGuess);
            hasOBjectOrParam.Add("neg. Underground Distance to spawn (guess)", -spawnHightGuess);






            hasOBjectOrParam.Add("Chest duplication Glitch", 0);
            hasOBjectOrParam.Add("Pot duplication Glitch", 0);
            hasOBjectOrParam.Add("Pot duplication Glitch Single", 0);
            hasOBjectOrParam.Add("Pot duplication Glitch Single Cavern", 0);
            hasOBjectOrParam.Add("Life Crystal duplication Glitch", 0);
            hasOBjectOrParam.Add("Life Crystal duplication Glitch Single", 0);
            hasOBjectOrParam.Add("Enchanted Sword duplication Glitch", 0);
            hasOBjectOrParam.Add("Floating duplication Glitch structure", 0);
            hasOBjectOrParam.Add("Game breaker", 0);
            hasOBjectOrParam.Add("Quick Plantera bulb prediction (beta)", 0);



            hasOBjectOrParam.Add("Dungeon tiles above surface", 0);
            hasOBjectOrParam.Add("Dungeon far above surface", 0);
            hasOBjectOrParam.Add("Dungeon below ground", 0);
            hasOBjectOrParam.Add("Dungeon below ground tree", 0);
            hasOBjectOrParam.Add(OptionsDict.Phase2.dungeonGoodPos, 0);
            hasOBjectOrParam.Add("Dungeon in Snow Biome", 0);
            hasOBjectOrParam.Add(OptionsDict.Phase2.dungeonStrangePos, 0);
            hasOBjectOrParam.Add("Floating island cabin in Dungeon", 0);
            hasOBjectOrParam.Add("Pre Skeletron Dungeon Chest Risky", 0);
            hasOBjectOrParam.Add("Pre Skeletron Dungeon Chest Grab", 0);
            hasOBjectOrParam.Add("Pre Skeletron Dungeon Chest Any", 0);
            hasOBjectOrParam.Add("Pre Skeletron Golden Key Risky", 0);
            hasOBjectOrParam.Add("Pre Skeletron Golden Key Grab", 0);
            hasOBjectOrParam.Add("Pre Skeletron Golden Key Any", 0);
           
            hasOBjectOrParam.Add("Pre Skeletron Muramasa good positon", 0);
            hasOBjectOrParam.Add("Pre Skeletron Muramasa Chest reachable", 0);
            hasOBjectOrParam.Add("Pre Skeletron Cobalt Shield Chest reachable", 0);
            hasOBjectOrParam.Add("Pre Skeletron Handgun Chest reachable", 0);
            hasOBjectOrParam.Add("Pre Skeletron Shadow Key Chest reachable", 0);


            hasOBjectOrParam.Add("Alchemy Table", 0);
            hasOBjectOrParam.Add("Sharpening Station", 0);
            hasOBjectOrParam.Add("Dungeon farm spot", 0);
            hasOBjectOrParam.Add("Dungeon farm spot 3Wall inters.", 0);
            hasOBjectOrParam.Add("Dungeon farm spot 3Wall in line", 0);
            hasOBjectOrParam.Add("Dungeon wall color", dungeonColor);
            hasOBjectOrParam.Add("Dungeon side", localDungeonSide);
            hasOBjectOrParam.Add("Hardmode evil side", evilHMIsLeft ? -1 : 1);

            hasOBjectOrParam.Add("Copper or Tin", score.copperOrTin.Equals("Copper") ? -1 : 1);
            hasOBjectOrParam.Add("Iron or Lead", score.ironOrLead.Equals("Iron") ? -1 : 1);
            hasOBjectOrParam.Add("Silver or Tungsten", score.silverOrTung.Equals("Silver") ? -1 : 1);
            hasOBjectOrParam.Add("Gold or Platinum", score.goldOrPlat.Equals("Gold") ? -1 : 1);


            hasOBjectOrParam.Add("Floating Island without chest", 0);//not scored
            hasOBjectOrParam.Add("Many Pyramids", 0);


            hasOBjectOrParam.Add("Number different Paintings", 0);
            hasOBjectOrParam.Add("Number Paintings", 0);

            hasOBjectOrParam.Add("Different functional noncraf. Statues", 0);
            hasOBjectOrParam.Add("Number functional noncraf. Statues", 0);
            hasOBjectOrParam.Add("Different noncraf. Statues", 0);
            hasOBjectOrParam.Add("Number noncraf. Statues", 0);
            hasOBjectOrParam.Add("Angel Statue chest", 0);
            hasOBjectOrParam.Add("Angel Statue placed", 0);

            hasOBjectOrParam.Add("Dart Trap", 0);
            hasOBjectOrParam.Add("Super Dart Trap", 0);
            hasOBjectOrParam.Add("Flame Trap", 0);
            hasOBjectOrParam.Add("Spiky Ball Trap", 0);
            hasOBjectOrParam.Add("Spear Trap", 0);
            hasOBjectOrParam.Add("Geyser", 0);
            hasOBjectOrParam.Add("Detonator", 0);
            hasOBjectOrParam.Add("Detonator at surface", 0);
            hasOBjectOrParam.Add(OptionsDict.Phase3.greenPyramid, 0);
            hasOBjectOrParam.Add(OptionsDict.Phase3.lonelyJungleTree, 0);
            hasOBjectOrParam.Add("Shadow Chest item in normal chest", 0);
            hasOBjectOrParam.Add(OptionsDict.Phase3.frozenTemple, 0);


            hasOBjectOrParam.Add("Wooden Chest", 0);
            hasOBjectOrParam.Add("Wooden Chest Dungeon", 0);
            hasOBjectOrParam.Add("Gold Chest", 0);
            hasOBjectOrParam.Add("Gold Chest locked", 0);            
            hasOBjectOrParam.Add("Ice Chest", 0);
            hasOBjectOrParam.Add("Ivy Chest", 0);            
            hasOBjectOrParam.Add("Water Chest", 0);
            hasOBjectOrParam.Add("Skyware Chest", 0);
            hasOBjectOrParam.Add("Web Covered Chest", 0);
            hasOBjectOrParam.Add("Shadow Chest", 0);            
            hasOBjectOrParam.Add("Lihzahrd Chest", 0);
            hasOBjectOrParam.Add("Living Wood Chest", 0);

            checkAndSetIfESmidPossible(false);// already compuited in some cases
            hasOBjectOrParam.Add("Boost ES Tree mid value seed", boostESValueSeed);
            hasOBjectOrParam.Add("Enchanted Sword mid may possible (guess)", boostESValueSeedP2);
            hasOBjectOrParam.Add("Boost ES granite mid value seed", boostESGraniteValueSeed);
            hasOBjectOrParam.Add("Enchanted Sword mid granite may possible (guess)", boostESGraniteValueSeedP2);
            hasOBjectOrParam.Add("Enchanted Sword mid good may possible (guess)", boostESValueSeedP2good);

            

            if (!isPubRel)
            {
                //todo something better
                if (midESspots != null)
                {
                    int i = 0;
                    foreach (var pspot in midESspots)
                    {
                        hasOBjectOrParam.Add("Boost ES pos. mid location X" + i, pspot.Item1);
                        hasOBjectOrParam.Add("Boost ES pos. mid location Y" + i++, pspot.Item2);
                    }
                }

                if (midGraniteESspots != null)
                {
                    int i = 0;
                    foreach (var pspot in midGraniteESspots)
                    {
                        hasOBjectOrParam.Add("Boost ES pos. mid granite location X" + i, pspot.Item1);
                        hasOBjectOrParam.Add("Boost ES pos. mid granite location Y" + i++, pspot.Item2);
                    }
                }

                if (midESspotsGood != null)
                {
                    int i = 0;
                    foreach (var pspot in midESspotsGood)
                    {
                        hasOBjectOrParam.Add("Boost ES good pos. mid location X" + i, pspot.Item1);
                        hasOBjectOrParam.Add("Boost ES good pos. mid location Y" + i++, pspot.Item2);
                    }
                }
            } 

            hasOBjectOrParam.Add("Boost height value seed", boostHeightValueSeed);
            hasOBjectOrParam.Add("Boost rock layer offset", boostRockLayerOffsetSeed);
            hasOBjectOrParam.Add("Boost cavern layer offset (%)", boostCavernLayeroffsetSeed);
            hasOBjectOrParam.Add("Boost surfRock layer offset", boostSurfRockLayerOffsetSeed);
            hasOBjectOrParam.Add("Spawn rock layer offset (guess)", boostSpawnRockSeedOffsetSeed);
            hasOBjectOrParam.Add("neg. Spawn rock layer offset (guess)", boostSpawnRockSeedOffsetSeed);

            hasOBjectOrParam.Add("random value seed *10000", (int)(10000.0 * boostValueSeed));
            hasOBjectOrParam.Add("Boost random value seed", (int)(10.0/(1.0-boostValueSeed)));
            hasOBjectOrParam.Add("Boost pyramid value seed", boostPyramidValueSeed);
            hasOBjectOrParam.Add("Last seed in Phase 2", lastSeedPhase2);
            hasOBjectOrParam.Add("Last seed in Phase 3", lastSeedPhase3);


            hasOBjectOrParam.Add("Score", 0);

            //Beach detection
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime;
            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            //writeDebugFile(" analyze time before beach detection " + elapsedTime);


            int dungSide = Main.dungeonX > Main.maxTilesX / 2 ? 1 : 0;


            int posX = 25;
            int posY = 100;
            int direct = 1;
            int xOSL = findOceanToSurface(true, ref posX, ref posY, ref direct);
            posX = xOSL < 0 ? 0 : xOSL;
            hasOBjectOrParam["Beach left"] = posX;
            int blsY = posY;



            int xOSR = findOceanToSurface(false, ref posX, ref posY, ref direct);
            posX = xOSR < 0 ? Main.maxTilesX - 1 : xOSR;
            hasOBjectOrParam["Beach right"] = posX;
            int brsY = posY;

            if (dungSide == 0)
            {
                hasOBjectOrParam["Dungeon beach"] = hasOBjectOrParam["Beach left"];
                hasOBjectOrParam["Jungle beach"] = hasOBjectOrParam["Beach right"];
            }
            else
            {
                hasOBjectOrParam["Jungle beach"] = hasOBjectOrParam["Beach left"];
                hasOBjectOrParam["Dungeon beach"] = hasOBjectOrParam["Beach right"];
            }

            //aprox avrg height             
            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            //writeDebugFile(" analyze time before avrg height " + elapsedTime);



            //TODO ext function
            Tuple<int, int, int> mheiStdDiff = CheckSurface(blsY, hasOBjectOrParam["Beach left"], hasOBjectOrParam["Beach right"]);

            hasOBjectOrParam["Surface average height (aprox.)"] = mheiStdDiff.Item1;
            hasOBjectOrParam["Surface height (sqrt) variance"] = mheiStdDiff.Item2;
            hasOBjectOrParam["Surface max-min height"] = mheiStdDiff.Item3;

            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            //writeDebugFile(" after surface average height " + elapsedTime);


            int[,] pathLength = null;
            int mostSnowSideIvyChest = localDungeonSide < 0 ? Main.maxTilesX : 0;

            if (doFull)
            {                
                ComputePathlength(ref pathLength);
            }
            List<Tuple<int, int>> goldenKeyReach = new List<Tuple<int, int>>();
            List<Tuple<int, int>> muramasaKeyReach = new List<Tuple<int, int>>();


            //check chests
            for (int i = 0; i < 1000; i++)
            {
                Chest chest = Main.chest[i];
                if (chest != null)
                {

                    //check if doubl chest
                    int cx = chest.x;
                    int cy = chest.y;

                    if (chest.x < Main.offLimitBorderTiles || chest.x > Main.maxTilesX - Main.offLimitBorderTiles || chest.y < Main.offLimitBorderTiles || chest.y > Main.maxTilesX - Main.offLimitBorderTiles)
                        continue;

                    if (!((Main.tile[cx, cy].type == TileID.Containers || Main.tile[cx + 1, cy].type == TileID.Containers || Main.tile[cx, cy + 1].type == TileID.Containers || Main.tile[cx + 1, cy + 1].type == TileID.Containers)
                         &&
                         (Main.tile[cx, cy].active() || Main.tile[cx + 1, cy].active() || Main.tile[cx, cy + 1].active() || Main.tile[cx + 1, cy + 1].active()))
                       )
                        continue;


                    if (chest.item[0] != null)
                    {
                        int iid = chest.item[0].type;
                        //only store good one
                        if (iid == ItemID.PharaohsMask || iid == ItemID.SandstorminaBottle || iid == ItemID.FlyingCarpet || iid == ItemID.StaffofRegrowth || iid == ItemID.IceSkates || iid == ItemID.LivingLoom ||
                            iid == ItemID.FlowerBoots || iid == ItemID.Aglet || iid == ItemID.AnkletoftheWind || iid == ItemID.FiberglassFishingPole || iid == ItemID.Fish || iid == ItemID.Seaweed || iid == ItemID.BlizzardinaBottle
                            || iid == ItemID.SnowballCannon || iid == ItemID.WaterWalkingBoots || iid == ItemID.LavaCharm)
                            if (score.itemLocation.ContainsKey(chest.item[0].type))
                                score.itemLocation[chest.item[0].type].Add(new Tuple<int, int>(cx, cy));
                            else
                                score.itemLocation.Add(chest.item[0].type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                    }


                    //double glitch detector might not work
                    if (chest.item[0] != null &&
                        ((Main.tile[cx, cy - 1].active() && Main.tile[cx, cy - 1].type == TileID.ClosedDoor) ||
                        (Main.tile[cx + 1, cy - 1].active() && Main.tile[cx + 1, cy - 1].type == TileID.ClosedDoor) ||
                        !Main.tile[cx, cy + 2].active() || !Main.tile[cx + 1, cy + 2].active()
                        ))
                    {

                        hasOBjectOrParam["Chest duplication Glitch"] += 1; //writeDebugFile("maybe found doubglitch (buggy) at " + cx + " " + cy + " in seed " + Main.ActiveWorldFileData.Seed + " can get overridden: " + (!doFull));
                        if (score.itemLocation.ContainsKey(ItemID.DynastyChest))
                            score.itemLocation[ItemID.DynastyChest].Add(new Tuple<int, int>(cx, cy));
                        else
                            score.itemLocation.Add(ItemID.DynastyChest, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });

                        if (doFull)
                        {
                            int pl = FindShortestPathInRange(ref pathLength, cx, cy);
                            hasOBjectOrParam["Pathlength to Chest duplication Glitch"] = Math.Min(hasOBjectOrParam["Pathlength to Chest duplication Glitch"], pl);
                        }



                    }




                    //TODO Grab distance trough walls not included yet,, edit: still not?
                    ushort chestWall = Main.tile[cx, cy].wall;
                    if ((Main.tile[cx, cy].frameX == 72 || (chest.item[0].type == ItemID.GoldenKey && Main.tile[cx, cy].frameX == 0)) && Main.tile[cx, cy].frameY == 0 && Main.tile[cx, cy].type == 21)
                    {
                        //&& (chestWall == 7 || chestWall == 8 || chestWall == 9)
                        //  ||
                        // (chestWall == 94 || chestWall == 98 || chestWall == 96)
                        //   ||
                        //  (chestWall == 95 || chestWall == 99 || chestWall == 97)))
                        //Item item = chest.item[0];
                        //Todo update with chest ids
                        //if (!(item.Name.Equals("Piranha Gun") || item.Name.Equals("Rainbow Gun") || item.Name.Equals("Scourge of the Corruptor") || item.Name.Equals("Vampire Knives") || item.Name.Equals("Staff of the Frost Hydra")))

                        bool canget = false;

                        //TODO rearange code key/item surface, golden key counted at other location in case it is not item[0]

                        //chest in dungeon   
                        if (chest.y <= Main.worldSurface + 2 || canGrabDungeonItem(cx, cy))
                        {
                            if (chest.y > Main.worldSurface + 2 || chest.item[0].type != ItemID.GoldenKey)
                            {
                                //dont show common golden keys above surface
                                if (score.itemLocation.ContainsKey(chest.item[0].type))
                                    score.itemLocation[chest.item[0].type].Add(new Tuple<int, int>(cx, cy));
                                else
                                    score.itemLocation.Add(chest.item[0].type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                            }

                            if (Main.tile[cx, cy].frameX == 72)
                            {
                                hasOBjectOrParam["Pre Skeletron Dungeon Chest Grab"] += 1;
                                //writeDebugFile("########### can grab dungeon item at " + cx + " " + cy + " " + seed);
                                
                                if(chest.item[0].type == ItemID.Muramasa)
                                {                                    
                                    hasOBjectOrParam["Pre Skeletron Muramasa good positon"]++;
                                }

                            }
                            else if (Main.tile[cx, cy].frameX == 0 && chest.item[0].type == ItemID.GoldenKey && chest.y > Main.worldSurface + 2)
                            {
                                //excluding golden keys above surface, they are counted at other place
                                hasOBjectOrParam["Pre Skeletron Golden Key Grab"] += 1;
                                //writeDebugFile("########### can grab golden key at " + cx + " " + cy + " " + seed);
                                goldenKeyReach.Add(new Tuple<int,int> (cx,cy));
                            }
                            canget = true;
                        }
                        else
                        if (canGetDungeonItem(cx, cy - 1, 15, 15))
                        {
                            if (score.itemLocation.ContainsKey(chest.item[0].type))
                                score.itemLocation[chest.item[0].type].Add(new Tuple<int, int>(cx, cy));
                            else
                                score.itemLocation.Add(chest.item[0].type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });

                            if (Main.tile[cx, cy].frameX == 72)
                            {
                                hasOBjectOrParam["Pre Skeletron Dungeon Chest Risky"] += 1;
                                //writeDebugFile("########### can get dungeon item at " + cx + " " + cy + " " + seed);
                                if (chest.item[0].type == ItemID.Muramasa)
                                {
                                    muramasaKeyReach.Add(new Tuple<int, int>(cx, cy));
                                }
                            }
                            else if (Main.tile[cx, cy].frameX == 0 && chest.item[0].type == ItemID.GoldenKey)
                            {
                                hasOBjectOrParam["Pre Skeletron Golden Key Risky"] += 1;
                                //writeDebugFile("########### can get golden key at " + cx + " " + cy + " " + seed);
                            }
                            canget = true;
                        }
                        if (canget)
                        {
                            if (chest.item[0].type == ItemID.Muramasa)
                                hasOBjectOrParam["Pre Skeletron Muramasa Chest reachable"] += 1;
                            else if (chest.item[0].type == ItemID.CobaltShield)
                                hasOBjectOrParam["Pre Skeletron Cobalt Shield Chest reachable"] += 1;
                            else if (chest.item[0].type == ItemID.Handgun)
                                hasOBjectOrParam["Pre Skeletron Handgun Chest reachable"] += 1;
                            else if (chest.item[0].type == ItemID.ShadowKey)
                                hasOBjectOrParam["Pre Skeletron Shadow Key Chest reachable"] += 1;
                        }


                    }

                    int pathl = doFull ? FindShortestPathInRange(ref pathLength, cx, cy) : 1000000;

                    if (chest.item[0] != null && doFull)                        
                    {                        
                        if (pathl < hasOBjectOrParam["Pathlength to 5th Chest"])                        
                        {
                            List<int> pl = new List<int>
                            {
                                hasOBjectOrParam["Pathlength to Chest"],
                                hasOBjectOrParam["Pathlength to 2nd Chest"],
                                hasOBjectOrParam["Pathlength to 3rd Chest"],
                                hasOBjectOrParam["Pathlength to 4th Chest"],
                                pathl
                            };
                            pl.Sort();
                            hasOBjectOrParam["Pathlength to Chest"] = pl[0];
                            hasOBjectOrParam["Pathlength to 2nd Chest"] = pl[1];
                            hasOBjectOrParam["Pathlength to 3rd Chest"] = pl[2];
                            hasOBjectOrParam["Pathlength to 4th Chest"] = pl[3];
                            hasOBjectOrParam["Pathlength to 5th Chest"] = pl[4];
                        }

                        if (cy > Main.worldSurface-1 && pathl < hasOBjectOrParam["Pathlength to 5th underground Chest"])
                        {
                            List<int> pl = new List<int>
                            {
                                hasOBjectOrParam["Pathlength to underground Chest"],
                                hasOBjectOrParam["Pathlength to 2nd underground Chest"],
                                hasOBjectOrParam["Pathlength to 3rd underground Chest"],
                                hasOBjectOrParam["Pathlength to 4th underground Chest"],
                                pathl
                            };
                            pl.Sort();
                            hasOBjectOrParam["Pathlength to underground Chest"] = pl[0];
                            hasOBjectOrParam["Pathlength to 2nd underground Chest"] = pl[1];
                            hasOBjectOrParam["Pathlength to 3rd underground Chest"] = pl[2];
                            hasOBjectOrParam["Pathlength to 4th underground Chest"] = pl[3];
                            hasOBjectOrParam["Pathlength to 5th underground Chest"] = pl[4];
                        }



                        int iid = chest.item[0].type;
                        if (iid == ItemID.PharaohsMask || iid == ItemID.PharaohsRobe || iid == ItemID.FlyingCarpet || iid == ItemID.SandstorminaBottle)
                        {
                            if(pathl < hasOBjectOrParam["Pathlength to Pyramid Chest"])
                            {
                                hasOBjectOrParam["Pathlength to Pyramid Chest"] = pathl;
                            }
                        }
                    }

                    if (doFull && chest.item[0] != null && Main.tile[cx, cy].frameY == 0)
                    {
                        
                        if (Main.tile[cx, cy].frameX == 360)
                        {
                            hasOBjectOrParam["Ivy Chest"]++;
                            if (localDungeonSide < 0)
                                mostSnowSideIvyChest = Math.Min(mostSnowSideIvyChest, cx);
                            else
                                mostSnowSideIvyChest = Math.Max(mostSnowSideIvyChest, cx);
                        }else if (Main.tile[cx, cy].frameX == 0)
                        {
                            if (chest.item[0].type == ItemID.GoldenKey)
                                hasOBjectOrParam["Wooden Chest Dungeon"]++;
                            else
                            {
                                hasOBjectOrParam["Wooden Chest"]++;
                                if (pathl < hasOBjectOrParam["Pathlength to Wooden Chest"])
                                    hasOBjectOrParam["Pathlength to Wooden Chest"] = pathl;

                            }
                        }
                        else if (Main.tile[cx, cy].frameX == 36)
                        {
                            hasOBjectOrParam["Gold Chest"]++;
                            if (pathl < hasOBjectOrParam["Pathlength to Golden Chest"])
                                hasOBjectOrParam["Pathlength to Golden Chest"] = pathl;
                        }
                        else if (Main.tile[cx, cy].frameX == 72)
                        {
                            hasOBjectOrParam["Gold Chest locked"]++;
                        }
                        else if (Main.tile[cx, cy].frameX == 396)
                        {
                            hasOBjectOrParam["Ice Chest"]++;
                        }
                        else if (Main.tile[cx, cy].frameX == 144)
                        {
                            hasOBjectOrParam["Shadow Chest"]++;
                        }
                        else if (Main.tile[cx, cy].frameX == 432)
                        {
                            hasOBjectOrParam["Living Wood Chest"]++;
                        }
                        else if (Main.tile[cx, cy].frameX == 468)
                        {
                            hasOBjectOrParam["Skyware Chest"]++;
                        }
                        else if (Main.tile[cx, cy].frameX == 540)
                        {
                            hasOBjectOrParam["Web Covered Chest"]++;
                        }
                        else if (Main.tile[cx, cy].frameX == 576)
                        {
                            hasOBjectOrParam["Lihzahrd Chest"]++;
                        }
                        else if (Main.tile[cx, cy].frameX == 612)
                        {
                            hasOBjectOrParam["Water Chest"]++;
                            if (pathl < hasOBjectOrParam["Pathlength to Water Chest"])
                                hasOBjectOrParam["Pathlength to Water Chest"] = pathl;
                        }


                    }
                    

                    for (int l = 0; l < 40; l++)
                    {
                        Item item = chest.item[l];

                        if (item == null) { break; } // might not work

                        //used for cond conf
                        if (!nameToID.ContainsKey(item.Name)) { nameToID.Add(item.Name, item.type); }


                        if (!hasOBjectOrParam.ContainsKey(item.type.ToString())) { hasOBjectOrParam.Add(item.type.ToString(), 0); }

                        hasOBjectOrParam[item.type.ToString()] += 1;


                        if (doFull && (item.type == ItemID.HermesBoots || item.type == ItemID.FlurryBoots))
                        {
                            if (pathl < hasOBjectOrParam["Pathlength to Boots"])
                            {
                                hasOBjectOrParam["Pathlength to Boots"] = pathl;

                                if (score.itemLocation.ContainsKey(ItemID.HermesBoots))
                                    score.itemLocation[ItemID.HermesBoots] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };//flurry shortest path
                                else
                                    score.itemLocation.Add(ItemID.HermesBoots, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                            }


                            int bootsDist = getDistanceToSpawn(chest.x, chest.y);
                            if (bootsDist < hasOBjectOrParam["Hermes Flurry Boots Distance"])
                            {
                                hasOBjectOrParam["Hermes Flurry Boots Distance"] = bootsDist;

                                if (score.itemLocation.ContainsKey(ItemID.FlurryBoots))
                                    score.itemLocation[ItemID.FlurryBoots] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };//only one elemenet, hermes shortes distance
                                else
                                    score.itemLocation.Add(ItemID.FlurryBoots, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                            }



                        }
                        else if (item.type == ItemID.MeteoriteBar)
                        {

                            //frameX look in text file in terrariao folder
                            if (Main.tile[chest.x, chest.y].frameX != 144)
                            {
                                hasOBjectOrParam["Meteorite Bar unlocked"] += 1;

                                if (score.itemLocation.ContainsKey(ItemID.MeteoriteBar))
                                    score.itemLocation[ItemID.MeteoriteBar].Add(new Tuple<int, int>(cx, cy));
                                else
                                    score.itemLocation.Add(ItemID.MeteoriteBar, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });

                                if (pathl < hasOBjectOrParam["Pathlength to Meteorite Bar"])
                                {
                                    hasOBjectOrParam["Pathlength to Meteorite Bar"] = pathl;
                                }

                            }


                        }
                        else if (item.type == ItemID.PiranhaGun || item.type == ItemID.RainbowGun || item.type == ItemID.ScourgeoftheCorruptor || item.type == ItemID.VampireKnives || item.type == ItemID.StaffoftheFrostHydra)
                        {
                            if (Main.tile[chest.x, chest.y].frameX != 864 && Main.tile[chest.x, chest.y].frameX != 900 && Main.tile[chest.x, chest.y].frameX != 936 && Main.tile[chest.x, chest.y].frameX != 972 && Main.tile[chest.x, chest.y].frameX != 828 && Main.tile[chest.x, chest.y].type == 21)
                            {
                                hasOBjectOrParam["Biome Item in normal Chest"] += 1;
                                //without tileid == 21(chest), 798807821 small expert random 1353 gets true here but has no chest there, destroyed by evil biome
                            }
                        }
                        else if (item.type == ItemID.AngelStatue)
                        {
                            hasOBjectOrParam["Angel Statue chest"] += 1;

                        }
                        else if (cy <= Main.worldSurface + 2 && item.type == ItemID.GoldenKey)
                        {
                            hasOBjectOrParam["Pre Skeletron Golden Key Grab"] += 1;
                        }
                        else if (item.type == ItemID.LivingWoodWand)
                        {
                            hasOBjectOrParam["Distance Tree Chest to mid"] = Math.Min(hasOBjectOrParam["Distance Tree Chest to mid"] , Math.Abs(Main.maxTilesX / 2 - cx));

                            if(Math.Abs(Main.maxTilesX/2-cx) < 300)
                                hasOBjectOrParam["Tree chests near mid"]++;

                            if (doFull)
                            {
                                if (checkIfNearSpawn(cx, cy, 275, 8000))
                                    hasOBjectOrParam["Near Tree Chest"]++;

                                if (pathl < hasOBjectOrParam["Pathlength to Tree Chest"])
                                {
                                    hasOBjectOrParam["Pathlength to Tree Chest"] = pathl;
                                }
                            }
                        }


                        if (doFull)
                        {

                            if (item.type == ItemID.TeleportationPotion && (!isInDungeon(cx, cy) || cy < Main.worldSurface + 2))
                            {                                
                                if (pathl < hasOBjectOrParam["Pathlength to 2 Teleport Potion Chest"] && item.stack > 1)
                                {

                                    hasOBjectOrParam["Pathlength to 2 Teleport Potion Chest"] = pathl;

                                    if (pathl <= hasOBjectOrParam["Pathlength to 2nd Teleport Potion"]) hasOBjectOrParam["Pathlength to 2nd Teleport Potion"] = pathl;

                                    if (score.itemLocation.ContainsKey(ItemID.ChaosFish))
                                        score.itemLocation[ItemID.ChaosFish] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(ItemID.ChaosFish, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }

                                if (pathl < hasOBjectOrParam["Pathlength to Teleport Potion"])
                                {
                                    hasOBjectOrParam["Pathlength to 2nd Teleport Potion"] = item.stack > 1? pathl: hasOBjectOrParam["Pathlength to Teleport Potion"];
                                    hasOBjectOrParam["Pathlength to Teleport Potion"] = pathl;

                                    if (score.itemLocation.ContainsKey(item.type))
                                    {
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy), new Tuple<int, int>(score.itemLocation[item.type].ElementAt(0).Item1, score.itemLocation[item.type].ElementAt(0).Item2) };
                                    }
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });


                                }
                                else if (pathl < hasOBjectOrParam["Pathlength to 2nd Teleport Potion"])
                                {
                                    //gets removed if equal to 2 teleport chest
                                    hasOBjectOrParam["Pathlength to 2nd Teleport Potion"] = pathl;
                                    score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(score.itemLocation[item.type].ElementAt(0).Item1, score.itemLocation[item.type].ElementAt(0).Item2), new Tuple<int, int>(cx, cy) };
                                }
                                if (hasOBjectOrParam["Pathlength to Teleport Potion"] < hasOBjectOrParam["Pathlength to 2 Teleport Potion Chest"])
                                    hasOBjectOrParam["Nearest Teleportation Potion count"] = 1;
                                else
                                    hasOBjectOrParam["Nearest Teleportation Potion count"] = 2;

                            }

                            else if ((item.type == ItemID.CopperBar || item.type == ItemID.TinBar))
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Copper/Tin Bar"])
                                {
                                    hasOBjectOrParam["Pathlength to Copper/Tin Bar"] = pathl;
                                    if (score.itemLocation.ContainsKey(ItemID.CopperBar))
                                        score.itemLocation[ItemID.CopperBar] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(ItemID.CopperBar, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }

                            }                            
                            else if ((item.type == ItemID.IronBar || item.type == ItemID.LeadBar))
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Iron/Lead Bar"])
                                {

                                    hasOBjectOrParam["Pathlength to Iron/Lead Bar"] = pathl;
                                    if (score.itemLocation.ContainsKey(ItemID.IronBar))
                                        score.itemLocation[ItemID.IronBar] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(ItemID.IronBar, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }
                                //neg. Pathlength to 10 Iron/Lead Bar Chest
                                if (pathl < hasOBjectOrParam["Pathlength to 10 Iron/Lead Bar Chest"] && item.stack > 9)
                                {

                                    hasOBjectOrParam["Pathlength to 10 Iron/Lead Bar Chest"] = pathl;
                                    if (score.itemLocation.ContainsKey(ItemID.CobaltBar))
                                        score.itemLocation[ItemID.CobaltBar] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(ItemID.CobaltBar, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }
                                if (pathl < hasOBjectOrParam["Pathlength to 12 Iron/Lead Bar Chest"] && item.stack > 11)
                                {

                                    hasOBjectOrParam["Pathlength to 12 Iron/Lead Bar Chest"] = pathl;
                                    if (score.itemLocation.ContainsKey(ItemID.MythrilBar))
                                        score.itemLocation[ItemID.MythrilBar] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(ItemID.MythrilBar, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }




                            }                            
                            else if ((item.type == ItemID.SilverBar || item.type == ItemID.TungstenBar))
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Silver/Tungsten Bar"])
                                {
                                    hasOBjectOrParam["Pathlength to Silver/Tungsten Bar"] = pathl;
                                    if (score.itemLocation.ContainsKey(ItemID.SilverBar))
                                        score.itemLocation[ItemID.SilverBar] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(ItemID.SilverBar, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }

                            }
                             else if ((item.type == ItemID.GoldBar || item.type == ItemID.PlatinumBar))
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Gold/Platinum Bar"])
                                {
                                    hasOBjectOrParam["Pathlength to Gold/Platinum Bar"] = pathl;
                                    if (score.itemLocation.ContainsKey(ItemID.GoldBar))
                                        score.itemLocation[ItemID.GoldBar] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(ItemID.GoldBar, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }

                            }

                            else if ((item.type == ItemID.SuspiciousLookingEye))
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Suspicious Looking Eye"])
                                {
                                    hasOBjectOrParam["Pathlength to Suspicious Looking Eye"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }

                            }
                            else if ((item.type == ItemID.SnowballCannon))
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Snowball Cannon"])
                                {
                                    hasOBjectOrParam["Pathlength to Snowball Cannon"] = pathl;
                                }

                            }
                            else if (item.type == ItemID.Boomstick)
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Boomstick"])
                                {
                                    hasOBjectOrParam["Pathlength to Boomstick"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }


                            }
                            else if (item.type == ItemID.FlowerBoots)
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Flower Boots"])
                                {
                                    hasOBjectOrParam["Pathlength to Flower Boots"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }


                            }
                            else if (item.type == ItemID.Bomb)
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Bomb"])
                                {
                                    hasOBjectOrParam["Pathlength to Bomb"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }


                            }
                            else if (item.type == ItemID.JestersArrow)
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Jester's Arrow"] && (!isInDungeon(cx, cy) || cy > Main.worldSurface + 2))
                                {
                                    hasOBjectOrParam["Pathlength to Jester's Arrow"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }


                            }
                            else if (item.type == ItemID.Dynamite)
                            {
                                if ((!isInDungeon(cx, cy) || cy > Main.worldSurface + 2) && Main.tile[chest.x, chest.y].frameX != 144)
                                {
                                    //Pathlength to 2nd Dynamite

                                    if (pathl < hasOBjectOrParam["Pathlength to Dynamite"])
                                    {
                                        hasOBjectOrParam["Pathlength to 2nd Dynamite"] = hasOBjectOrParam["Pathlength to Dynamite"];
                                        hasOBjectOrParam["Pathlength to Dynamite"] = pathl;

                                        if (score.itemLocation.ContainsKey(item.type))
                                        {
                                            score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy), new Tuple<int, int>(score.itemLocation[item.type].ElementAt(0).Item1, score.itemLocation[item.type].ElementAt(0).Item2) };
                                        }
                                        else
                                            score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });


                                    }
                                    else if (pathl < hasOBjectOrParam["Pathlength to 2nd Dynamite"])
                                    {
                                        hasOBjectOrParam["Pathlength to 2nd Dynamite"] = pathl;
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(score.itemLocation[item.type].ElementAt(0).Item1, score.itemLocation[item.type].ElementAt(0).Item2), new Tuple<int, int>(cx, cy) };
                                    }


                                    
                                }
                            }
                            else if (item.type == ItemID.GravitationPotion)
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Gravitation Potion"] && (!isInDungeon(cx, cy) || cy > Main.worldSurface + 2) && Main.tile[chest.x, chest.y].frameX != 144)
                                {
                                    hasOBjectOrParam["Pathlength to Gravitation Potion"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type].Add(new Tuple<int, int>(cx, cy));
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }
                            }
                            else if (item.type == ItemID.CloudinaBottle)
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Cloud in a Bottle"] )
                                {
                                    hasOBjectOrParam["Pathlength to Cloud in a Bottle"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }
                            }
                            else if (item.type == ItemID.HerbBag && item.stack > 1)
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to 2 Herb Bag Chest"] )
                                {
                                    hasOBjectOrParam["Pathlength to 2 Herb Bag Chest"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }
                            }
                            else if (item.type == ItemID.Grenade)
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Grenades"])
                                {
                                    hasOBjectOrParam["Pathlength to Grenades"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }
                            }
                             else if (item.type == ItemID.Extractinator)
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Extractinator"])
                                {
                                    hasOBjectOrParam["Pathlength to Extractinator"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }
                            }
                            else if (item.type == ItemID.MagicMirror || item.type == ItemID.IceMirror)
                            {
                                
                                if (pathl < hasOBjectOrParam["Pathlength to Magic/Ice Mirror"])
                                {
                                    hasOBjectOrParam["Pathlength to Magic/Ice Mirror"] = pathl;
                                    if (score.itemLocation.ContainsKey(ItemID.MagicMirror))
                                        score.itemLocation[ItemID.MagicMirror] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(ItemID.MagicMirror, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }
                            }
                            else if ( (item.type == ItemID.DarkLance || item.type == ItemID.Flamelash || item.type == ItemID.FlowerofFire 
                                || item.type == ItemID.Sunfury || item.type == ItemID.HellwingBow                                 
                                ) && Main.tile[cx, cy].frameX != 144 )
                            {
                                if (score.itemLocation.ContainsKey(item.type))
                                    score.itemLocation[item.type].Add(new Tuple<int, int>(cx, cy));
                                else
                                    score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });

                                
                                hasOBjectOrParam["Shadow Chest item in normal chest"]++;

                                
                            }else if(item.type == ItemID.LavaCharm)
                            {
                                hasOBjectOrParam["Pathlength to Lava Charm"] = Math.Min(pathl, hasOBjectOrParam["Pathlength to Lava Charm"]);
                            }
                            else if (item.type == ItemID.WaterWalkingBoots)
                            {
                                hasOBjectOrParam["Pathlength to Water Walking Boots"] = Math.Min(pathl, hasOBjectOrParam["Pathlength to Water Walking Boots"]);
                            }
                            else if (item.type == ItemID.Fish)
                            {
                                hasOBjectOrParam["Pathlength to Fish Pet"] = Math.Min(pathl, hasOBjectOrParam["Pathlength to Fish Pet"]);
                            }
                            else if (item.type == ItemID.Seaweed)
                            {
                                hasOBjectOrParam["Pathlength to Seaweed Pet"] = Math.Min(pathl, hasOBjectOrParam["Pathlength to Seaweed Pet"]);
                            }
                            else if (item.type == ItemID.Rope )
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Rope"])
                                {
                                    hasOBjectOrParam["Pathlength to Rope"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }
                            }
                            else if (item.type == ItemID.RecallPotion )
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Recall Potion"])
                                {
                                    hasOBjectOrParam["Pathlength to Recall Potion"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }
                            }
                            else if (item.type == ItemID.BuilderPotion)
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Builder Potion"])
                                {
                                    hasOBjectOrParam["Pathlength to Builder Potion"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }
                            }

                            else if (item.type == ItemID.LifeforcePotion && Main.tile[cx, cy].frameX != 144 )
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Lifeforce Potion"])
                                {
                                    hasOBjectOrParam["Pathlength to Lifeforce Potion"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }
                            }




                            else if (item.type == ItemID.BattlePotion && Main.tile[cx, cy].frameX != 144)
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Battle Potion"])
                                {
                                    hasOBjectOrParam["Pathlength to Battle Potion"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }
                            }
                            else if (item.type == ItemID.ObsidianSkinPotion && Main.tile[cx, cy].frameX != 144)
                            {
                                if (pathl < hasOBjectOrParam["Pathlength to Obsidian Skin Potion"])
                                {
                                    hasOBjectOrParam["Pathlength to Obsidian Skin Potion"] = pathl;
                                    if (score.itemLocation.ContainsKey(item.type))
                                        score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                    else
                                        score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                                }
                            }

                            


                        }



                    }
                }
            }

            if(score.itemLocation.ContainsKey(ItemID.TeleportationPotion) && score.itemLocation[ItemID.TeleportationPotion].Count > 1 && score.itemLocation.ContainsKey(ItemID.ChaosFish))
            {
                if(score.itemLocation[ItemID.TeleportationPotion][1].Item1 == score.itemLocation[ItemID.ChaosFish][0].Item1 &&
                    score.itemLocation[ItemID.TeleportationPotion][1].Item2 == score.itemLocation[ItemID.ChaosFish][0].Item2)
                {
                    //remove 2nd tp potion if equal to 2tp chest
                    score.itemLocation[ItemID.TeleportationPotion] = new List<Tuple<int, int>> { new Tuple<int, int>(score.itemLocation[ItemID.TeleportationPotion][0].Item1, score.itemLocation[ItemID.TeleportationPotion][0].Item2) };
                }
            }

            if(goldenKeyReach.Count > 0 && muramasaKeyReach.Count > 0)
            {
                foreach(var gp in goldenKeyReach)
                {
                    foreach (var mp in muramasaKeyReach)
                    {                        
                        if ( (gp.Item1-mp.Item1)* (gp.Item1 - mp.Item1) + (gp.Item2 - mp.Item2) * (gp.Item2 - mp.Item2) < 22500) //dist < 150 tiles, x,y 106 each
                        {
                            hasOBjectOrParam["Pre Skeletron Muramasa good positon"]++; 
                        }
                    }
                }
            }
            goldenKeyReach.Clear();
            goldenKeyReach = null;
            muramasaKeyReach.Clear();
            muramasaKeyReach = null;


            int goldenKeys = hasOBjectOrParam["Pre Skeletron Golden Key Grab"] + hasOBjectOrParam["Pre Skeletron Golden Key Risky"];
            hasOBjectOrParam["Pre Skeletron Golden Key Any"] = goldenKeys;

            hasOBjectOrParam["Pre Skeletron Dungeon Chest Any"] = Math.Min(hasOBjectOrParam["Pre Skeletron Dungeon Chest Grab"] + hasOBjectOrParam["Pre Skeletron Dungeon Chest Risky"], hasOBjectOrParam["Pre Skeletron Golden Key Any"]);

            hasOBjectOrParam["Pre Skeletron Dungeon Chest Grab"] = Math.Min(hasOBjectOrParam["Pre Skeletron Dungeon Chest Grab"], hasOBjectOrParam["Pre Skeletron Golden Key Grab"]);


            if (hasOBjectOrParam["Pre Skeletron Golden Key Grab"] == 0)
                hasOBjectOrParam["Pre Skeletron Dungeon Chest Risky"] = Math.Min(hasOBjectOrParam["Pre Skeletron Dungeon Chest Risky"] + hasOBjectOrParam["Pre Skeletron Dungeon Chest Grab"], goldenKeys);
            else
                hasOBjectOrParam["Pre Skeletron Dungeon Chest Risky"] = Math.Min(hasOBjectOrParam["Pre Skeletron Dungeon Chest Risky"], goldenKeys);




            int pyramids = has(ref hasOBjectOrParam, ItemID.SandstorminaBottle) + has(ref hasOBjectOrParam, ItemID.PharaohsMask) + has(ref hasOBjectOrParam, ItemID.FlyingCarpet);
            //maps with aat least 4 for small, 5 for mid, 6  for large
            if (pyramids > (Main.maxTilesY / 600 + 1))
                hasOBjectOrParam["Many Pyramids"] += (pyramids - (Main.maxTilesY / 600 + 1));


            if (score.itemLocation.ContainsKey(ItemID.HermesBoots) && score.itemLocation.ContainsKey(ItemID.FlurryBoots) && score.itemLocation[ItemID.HermesBoots][0].Item1 == score.itemLocation[ItemID.FlurryBoots][0].Item1 && score.itemLocation[ItemID.HermesBoots][0].Item2 == score.itemLocation[ItemID.FlurryBoots][0].Item2)
            {
                score.itemLocation.Remove(ItemID.FlurryBoots);
            }



            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            //writeDebugFile(" analyze basice took " + elapsedTime);



            //if (!doFull)   return;



            //Todo sprite search



            //tile saerch
            var treesPos = new HashSet<int>();

            var cloudPos = new HashSet<int>();

            var tileCounts = new Dictionary<int, int>();

            var paintingsCount = new Dictionary<string, int>();
            var statuesCount = new Dictionary<string, int>();
            var statuesFunctionalCount = new Dictionary<string, int>();

            const int Max_wall_count = 256;
            int[] wallCounts = new int[Max_wall_count];

            int activeTiles = 0;
            int evilTiles = 0;


            int leftmostSurfaceSnowTilex = Main.maxTilesX;
            int rightmostSurfaceSnowTilex = 0;

            int leftmostCavernJungleTilex = Main.maxTilesX;
            int rightmostCavernJungleTilex = 0;

            int leftmostSurfaceJungleTilex = Main.maxTilesX;
            int rightmostSurfaceJungleTilex = 0;

            int leftmostTempleTilex = Main.maxTilesX;
            int rightmostTempleTilex = 0;
            int topmostTempleTiley = Main.maxTilesY;
            int botmostTempleTiley = 0;

            int highestFreeLava = Main.maxTilesY;
            List<Tuple<int, int>> openHives = new List<Tuple<int, int>>();
            List<Tuple<int, int>> larveInDanger = new List<Tuple<int, int>>();

            List<Tuple<int, int, int, int>> dungeonFarmSpotCandidates = new List<Tuple<int, int, int, int>>(); // xmin xmax of inner wall, ymin ymax of detection

            bool treeToUGDung = false;

            int liqf = 0;
            int liqt = 0;

            int[] waterDuckSpawn = new int[601];
                        

            //todo: no border
            for (int x = 10; x < Main.maxTilesX - 10; x++)
           {
                for (int y = 10; y < Main.maxTilesY - 10; y++)
                { 
                    var tile = Main.tile[x, y];
                    if (tile == null && !isPubRel)
                    {
                        writeDebugFile("tile null at " + x + " " + y);
                    }

                    if (tile != null && (tile.wall == WallID.GraniteUnsafe || tile.wall == WallID.MarbleUnsafe))
                    {
                        if (doFull)
                            if (y > Main.worldSurface && (tile.wall == WallID.GraniteUnsafe || tile.wall == WallID.MarbleUnsafe)
                                        && pathLength[x, y] < hasOBjectOrParam["Pathlength to underground MarbleGranite"])
                            {
                                bool isFree = true;
                                for (int xi = x - 1; xi < x + 2; xi++)
                                    for (int yi = y - 1; yi < y + 1; yi++)
                                    {
                                        Tile tilegm = Main.tile[xi, yi];
                                        //if(tilegm!=null)
                                        if ((tilegm.wall != WallID.GraniteUnsafe && tilegm.wall != WallID.MarbleUnsafe))
                                            isFree = false;
                                        else if (tilegm.type == TileID.Granite || tilegm.type == TileID.GraniteBlock || tilegm.type == TileID.Marble || tilegm.type == TileID.MarbleBlock)
                                            isFree = false;

                                    }

                                if (isFree)
                                {
                                    hasOBjectOrParam["Pathlength to underground MarbleGranite"] = pathLength[x, y];

                                }


                            }


                        if (y > Main.worldSurface || y <= Main.worldSurface - spawnHightGuess + 4)
                        {
                            int distSq = (x - Main.maxTilesX / 2) * (x - Main.maxTilesX / 2) + ((int)Main.worldSurface - spawnHightGuess - y) * ((int)Main.worldSurface - spawnHightGuess - y);

                            //granite marble UG near mid
                            if (y > Main.worldSurface)
                                hasOBjectOrParam["UG MarbleGranite dist. to spawn (guess)"] = Math.Min(distSq, hasOBjectOrParam["UG MarbleGranite dist. to spawn (guess)"]);
                            //Top MarbleGranite dist. to spawn (guess)
                            if (y <= Main.worldSurface - spawnHightGuess + 4 && (tile.type != TileID.Marble && tile.type != TileID.MarbleBlock && tile.type != TileID.GraniteBlock && tile.type != TileID.Granite))
                                hasOBjectOrParam["Top MarbleGranite dist. to spawn (guess)"] = Math.Min(distSq, hasOBjectOrParam["Top MarbleGranite dist. to spawn (guess)"]);

                        }
               
                    }

                    if(tile!=null)
                    {
                        

                        if (tile.wall == WallID.LivingWood && (!tile.active() || tile.type!= TileID.LivingWood) )
                        {              
                            if (y < Main.worldSurface && Main.tile[x, y - 1].wall == WallID.LivingWood
                                && Math.Abs(Main.maxTilesX / 2 - x) < 275)
                            {
                                int off = -1;
                                if (x < Main.maxTilesX / 2)
                                    off = +1;
                                

                                bool opening = true;
                                //opening
                                for (int i = 0; i < 4; i++)
                                {
                                    if ((Main.tile[x + off, y + i].active() && Main.tile[x + off, y + i].type == TileID.LivingWood) || Main.tile[x + off, y + i].wall == WallID.LivingWood)
                                    {
                                        opening = false;

                                    }
                                }
                                if (opening)
                                {
                                    hasOBjectOrParam["Tree near mid open to mid"] += 1;
                                    //Main.tile[x , y ].wall = WallID.AmberGemspark;
                                }


                            }
                        }

                        if (doFull)
                        {
                            if (tile.liquid == 255 && tile.lava() == true && y < highestFreeLava && (
                                 ((!Main.tile[x + 1, y].active() || !Main.tileSolid[(int)Main.tile[x + 1, y].type]) && Main.tile[x + 1, y].liquid == 0) ||
                                ((!Main.tile[x - 1, y].active() || !Main.tileSolid[(int)Main.tile[x - 1, y].type]) && Main.tile[x - 1, y].liquid == 0) ||
                                ((!Main.tile[x, y + 1].active() || !Main.tileSolid[(int)Main.tile[x, y + 1].type]) && Main.tile[x, y + 1].liquid == 0))                                
                                )
                            {
                                highestFreeLava = y;
                            }
                            if(tile.type == TileID.Larva && tile.liquid>0 && tile.honey()== true && Main.tile[x, y - 1].liquid == 0)
                            {
                                larveInDanger.Add(new Tuple<int, int>(x, y));
                            }

                          
                          

                            
                            //check for open hive, TODO improve code
                            if (tile.wall == WallID.HiveUnsafe && (!tile.active() ||!Main.tileSolid[(int)tile.type]) &&  (
                                ((!Main.tile[x + 1, y].active() || !Main.tileSolid[(int)Main.tile[x + 1, y].type]) && Main.tile[x + 1, y].wall != WallID.HiveUnsafe) ||
                                ((!Main.tile[x - 1, y].active() || !Main.tileSolid[(int)Main.tile[x - 1, y].type]) && Main.tile[x - 1, y].wall != WallID.HiveUnsafe) ||                                
                                ((!Main.tile[x, y - 1].active() || !Main.tileSolid[(int)Main.tile[x , y-1].type]) && Main.tile[x, y - 1].wall != WallID.HiveUnsafe))
                                )
                            {
                                List<Tuple<int,int>> waya = new List < Tuple<int, int> >{ };
                                waya.Add( new Tuple<int,int>(x, y) );
                                

                                List<Tuple<int, int>> wayn = new List<Tuple<int, int>> { };
                                wayn.Add(new Tuple<int, int>(x, y));
                                bool valid = true;
                                bool lavaFound = false;
                                for (int i = 0; i < 100 && !lavaFound; i++)
                                {
                                    List<Tuple<int, int>> wayn2 = new List<Tuple<int, int>> { };
                                    foreach (var wp in wayn)
                                    {
                                        int xt = wp.Item1;
                                        int yt = wp.Item2;

                                        if (Main.tile[xt, yt].lava() && Main.tile[xt, yt].liquid == 255)
                                        {
                                            lavaFound = true;
                                            break;
                                        }

                                        for (int z = -1; z < 4; z += 2)
                                        {
                                            int xa, ya;
                                            if (z == 3)
                                            {
                                                xa = 0;
                                                ya = -1;
                                            }
                                            else
                                            {
                                                xa = z;
                                                ya = 0;
                                            }
                                            if ((!Main.tile[xt + xa, yt + ya].active() || !Main.tileSolid[(int)Main.tile[xt + xa, yt + ya].type]) && Main.tile[xt + xa, yt + ya].wall != WallID.HiveUnsafe)
                                            {
                                                bool contains = false;
                                                foreach (var wa in waya) { if (wa.Item1 == xt + xa && wa.Item2 == yt + ya) contains = true; }
                                                if (!contains)
                                                {
                                                    wayn2.Add(new Tuple<int, int>(xt + xa, yt + ya));
                                                    waya.Add(new Tuple<int, int>(xt + xa, yt + ya));
                                                }
                                            }
                                        }


                                    }
                                    wayn = wayn2;
                                    if ( (waya.Count > 300 && wayn.Count > 300) || waya.Count > 1000  || lavaFound)
                                    {
                                        break;
                                    }
                                    else if (wayn2.Count == 0 )
                                    {
                                        valid = false;
                                        break;
                                    }
                                }

                                if (valid)
                                {
                                    openHives.Add(new Tuple<int, int>(x, y));
                                    

                                }

                            }

                            


                        }


                        if ( y < Main.worldSurface && y > 150 && x > Main.maxTilesX / 2 - 300 && x < Main.maxTilesX / 2 + 300 && waterDuckSpawn[x - Main.maxTilesX / 2 + 300] == 0 && Main.tile[x, y].liquid > 0 && Main.tile[x, y - 1].liquid > 0)
                        {
                            
                            bool duckCanSpawn = false;
                            int yi = y - 2;
                            for (; yi > y - 50; yi--)
                            {
                                if (Main.tile[x, yi].liquid == 0 && !WorldGen.SolidTile(x, yi) && !WorldGen.SolidTile(x, yi + 1) && !WorldGen.SolidTile(x, yi + 2))
                                {
                                    duckCanSpawn = true;
                                    break;
                                }
                            }
                            if (duckCanSpawn)
                            {
                                
                                int badTiles = 0;
                                for (int dd = yi; dd > yi - 6; dd--)
                                {
                                    //dummy to avoid below surface
                                    if (Main.tile[x, dd].wall != 0 || WorldGen.SolidTile(x, dd))
                                        badTiles++;
                                }
                                if (badTiles > 1)
                                    duckCanSpawn = false;
                                if (duckCanSpawn)
                                {

                                    int count = 0;
                                    int maxv = Math.Min((int)Main.worldSurface, yi + 50);
                                    badTiles = 0;
                                    for (; yi < maxv; yi++)
                                    {
                                        if (Main.tile[x, yi - 1].liquid > 0 && Main.tile[x, yi - 2].liquid > 0)
                                            count++;
                                        if (Main.tile[x, yi].wall != 0)
                                            badTiles++;
                                        if (WorldGen.SolidTile(x, yi) || Main.tile[x, yi].liquid == 0)
                                            badTiles += 2;
                                        if (badTiles > 6)
                                            break;
                                    }
                                    waterDuckSpawn[x - Main.maxTilesX / 2 + 300] = count;
                                }
                            }
                        }

                    }

                    if (!tile.active())
                    {

                        // if used with mod or new update there might be more
                        if (tile.wall < Max_wall_count && tile.wall >= 0)
                            wallCounts[tile.wall] += 1;

                        

                        if (tile.wall == WallID.LivingWood)
                        {
                            if (!treeToUGDung && checkIfNearDungeon(x, y, 60, 40))
                                treeToUGDung = true;

                        }

                        //find lake close to spawn (guessing)
                        if (tile.wall == 0 && y < Main.worldSurface )
                        {
                            
                            if (tile.liquid == 255 && x > liqt && x < 2 * Main.maxTilesX / 2 - liqf && x > 500 && x < Main.maxTilesX - 500)
                            {
                                int size = 0;
                                int xl = x;
                                int yl = y;
                                while (Main.tile[xl, yl].liquid == 255 && xl < Main.maxTilesX - 500)
                                {
                                    int yli = yl;
                                    while (Main.tile[xl, yli++].liquid == 255 && yli < Main.rockLayer) size++;
                                    xl++;
                                }
                                if (size > 200)
                                {
                                    liqf = x;
                                    liqt = xl;
                                }
                            }

                        }

                        if (doFull)
                        {
                            //Floating duplication Glitch structure
                            if (Main.tile[x, y - 1].active() && (Main.tile[x, y - 1].type == TileID.Containers || Main.tile[x, y - 1].type == TileID.DemonAltar))
                            {

                                int pathl = FindShortestPathInRange(ref pathLength, x, y, 3, 3, 4, 4);
                                
                                if (pathl < hasOBjectOrParam["Pathlength to Floating dupl. Glitch structure"])
                                {
                                    hasOBjectOrParam["Pathlength to Floating dupl. Glitch structure"] = pathl;
                                }



                                hasOBjectOrParam["Floating duplication Glitch structure"] += 1;
                                if (score.itemLocation.ContainsKey(ItemID.Glass))
                                    score.itemLocation[ItemID.Glass].Add(new Tuple<int, int>(x, y));
                                else
                                    score.itemLocation.Add(ItemID.Glass, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });
                            }
                        }

                    }
                    else
                    if (tile.active())
                    {
                        activeTiles++;

                        if (tileCounts.ContainsKey(tile.type))
                        {
                            tileCounts[tile.type] += 1;
                        }
                        else
                        {
                            tileCounts.Add(tile.type, 1);
                        }


                        //Tree
                        if (tile.type == TileID.LivingWood)
                        {
                            //unique
                            int count = 0;
                            bool isTree = false;
                            for (int i = -2; i < 3; i++)
                                count += (Main.tile[x + i, y].type == TileID.LivingWood) ? 1 : 0;
                            isTree = count > 4;
                            count = 0;
                            for (int i = -1; i < 11; i++)
                                count += (Main.tile[x, y + i].type == TileID.LivingWood) ? 1 : 0;
                            isTree = isTree && (count > 11);

                            //already exists?
                            if (isTree == true)
                            {
                                int newPos = 0;
                                //TODO check map border
                                for (int i = -25; i < 26; i++)
                                {
                                    if (treesPos.Contains(x + i) && i != 0)
                                    {
                                        newPos = (x + i) - (int)Math.Ceiling(((float)i) / 4.0);
                                        treesPos.Remove(x + i);
                                        treesPos.Add(newPos);
                                    }
                                }
                                if (newPos == 0)
                                {
                                    treesPos.Add(x);
                                }
                            }
                        }

                        //water bolt
                        else if ((tile.type == TileID.Books) && (tile.frameX == (short)90) && (tile.frameY == (short)0))
                        {
                            if (y <= Main.worldSurface + 2)
                            {
                                hasOBjectOrParam["Water Bolt before Skeletron"] += 1;

                                if (score.itemLocation.ContainsKey(ItemID.WaterBolt))
                                    score.itemLocation[ItemID.WaterBolt].Add(new Tuple<int, int>(x, y));
                                else
                                    score.itemLocation.Add(ItemID.WaterBolt, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });
                            }

                            hasOBjectOrParam["Water Bolt"] += 1;
                        }


                        //else if (tile.type == TileID.Cloud && checkIfNearSpawn(x, y, 200, 1000))
                        else if (tile.type == TileID.Cloud)
                        {

                            int area = 120;
                            bool found = false;
                            for (int i = -area; i < area; i++)
                            {
                                if (cloudPos.Contains(x + i))
                                {
                                    found = true;
                                    if (Math.Abs(i) > 50)
                                    {
                                        cloudPos.Remove(x + i);
                                        int newPos = x + i - Math.Sign(i);
                                        cloudPos.Add(newPos);

                                        //Console.Write(newPos.ToString() + " ");


                                    }
                                    break;
                                }
                            }

                            if (!found) cloudPos.Add(x);

                            if (doFull && checkIfNearSpawn(x, y, 100, 50))
                            {
                                hasOBjectOrParam["Spawn in Sky"] = 1;
                            }

                        }


                        //dungeon in snow biome
                        else if (tile.type == TileID.IceBlock || tile.type == TileID.SnowBlock || tile.type == TileID.CorruptIce || tile.type == TileID.FleshIce)
                        {
                            int ydiff = (int)Math.Abs(Main.dungeonY - y) * 2;
                            if (((dungSide == 0 && x < Main.dungeonX - 50 - ydiff) || (dungSide == 1 && x > Main.dungeonX + 50 + ydiff)))
                            {
                                hasOBjectOrParam["Dungeon in Snow Biome"] += 1;
                            }
                            if (tile.type == TileID.IceBlock && y < Main.worldSurface)
                            {
                                hasOBjectOrParam["Ice Surface"] += 1;
                            }

                            if (y < Main.rockLayer && x < leftmostSurfaceSnowTilex) leftmostSurfaceSnowTilex = x;
                            if (y < Main.rockLayer && x > rightmostSurfaceSnowTilex) rightmostSurfaceSnowTilex = x;

                            if (y < Main.worldSurface && ((x > Main.maxTilesX / 2 && dungSide == 0) || (x < Main.maxTilesX / 2 && dungSide == 1)))
                            {
                                hasOBjectOrParam["Snow biome surface overlap mid"] += 1;
                            }


                        }
                        //paintings
                        else if (tile.type == TileID.Painting2X3 || tile.type == TileID.Painting3X2 || tile.type == TileID.Painting3X3 || tile.type == TileID.Painting6X4)
                        {
                            //  || tile.type == TileID.Painting4X3 is Catacomb , no real painting 
                            int px = x;
                            int py = y;
                            ushort pt = tile.type;

                            while (Main.tile[px, py].type == pt) py--; py++;
                            while (Main.tile[px, py].type == pt) px--; px++;

                            string picIdent = pt + "|" + Main.tile[px, py].frameX + "|" + Main.tile[px, py].frameY;

                            //it counts each tile of each picture, divide by 72 to get the real number TODO only count top left frame
                            int count = (tile.type == TileID.Painting2X3 || tile.type == TileID.Painting3X2 ? 12 : tile.type == TileID.Painting3X3 ? 8 : tile.type == TileID.Painting6X4 ? 3 : -100000);

                            if (pt == TileID.Painting3X3 && Main.tile[px, py].frameY == 0 && (Main.tile[px, py].frameX == 864 || Main.tile[px, py].frameX == 918))
                            {
                                //do not count Hanging Skeleton
                            }
                            else
                            {
                                if (paintingsCount.ContainsKey(picIdent))
                                    paintingsCount[picIdent] += count;
                                else
                                {
                                    paintingsCount.Add(picIdent, count);
                                    //writeDebugFile("pos:" + x + " " + y + " is painting " + picIdent);
                                }

                            }

                        }

                        //alchemy table
                        else if (tile.type == TileID.AlchemyTable && tile.frameX == 0 && tile.frameY == 0)
                        {
                            hasOBjectOrParam["Alchemy Table"]++;
                        }

                        //Sharpening Station
                        else if (tile.type == TileID.SharpeningStation && tile.frameX == 0 && tile.frameY == 0)
                        {
                            hasOBjectOrParam["Sharpening Station"]++;
                        }

                        else if (y < Main.worldSurface)
                        {

                            if (((x > Main.maxTilesX / 2 && dungSide == 1) || (x < Main.maxTilesX / 2 && dungSide == 0)) &&
                            (Main.tile[x, y].type == TileID.JungleGrass || Main.tile[x, y].type == TileID.JunglePlants || Main.tile[x, y].type == TileID.JunglePlants2 || Main.tile[x, y].type == TileID.JungleThorns || Main.tile[x, y].type == TileID.JungleVines))
                            {

                                hasOBjectOrParam["Jungle biome surface overlap mid"] += 1;
                            }

                        }
                        //new if
                        //corruption2 crimson2 sandCor sandCrim iceCor iceCrim 
                        if (tile.type == TileID.Ebonstone || tile.type == TileID.CorruptGrass || tile.type == TileID.Crimstone || tile.type == TileID.FleshGrass
                            || tile.type == TileID.Ebonsand || tile.type == TileID.Crimsand || tile.type == TileID.CorruptIce || tile.type == TileID.FleshIce)
                        {

                            if (y < Main.rockLayer && Math.Abs(x - Main.maxTilesX / 2) < hasOBjectOrParam["Evil biome distance to mid"]) hasOBjectOrParam["Evil biome distance to mid"] = Math.Abs(x - Main.maxTilesX / 2);


                            hasOBjectOrParam["Nearest Evil left Ocean"] = Math.Min(x, hasOBjectOrParam["Nearest Evil left Ocean"]);
                            hasOBjectOrParam["Nearest Evil right Ocean"] = Math.Max(x, hasOBjectOrParam["Nearest Evil right Ocean"]);


                            bool canDirtMud = false;
                            bool canEvilGrass = false;
                            bool canEvilSand = false;
                            bool canEvilIce = false;

                            int lookUpDim = (tile.type == TileID.CorruptGrass || tile.type == TileID.FleshGrass) ? 6 : 3; //search radius 6 for gras ............................ TODO not used
                                                                                                                          //check if mud or jungel grass around
                            for (int xi = -3; xi < 4; xi++)
                            {
                                for (int yi = -3; yi < 4; yi++)
                                {
                                    if (x + xi < 0 || y + yi < 0 || x + xi >= Main.maxTilesX || y + yi >= Main.maxTilesY)
                                        continue;

                                    if (!Main.tile[x + xi, y + yi].active()) continue;

                                    var tileType = Main.tile[x + xi, y + yi].type;
                                    evilTiles += tileType;
                                    if (tileType == TileID.Mud)
                                    {
                                        canDirtMud = true;
                                    }
                                    else if (tileType == TileID.JungleGrass)
                                    {
                                        canEvilGrass = true;
                                    }
                                    else if (tileType == TileID.Sand)
                                    {
                                        canEvilSand = true;
                                    }
                                    else if (tileType == TileID.IceBlock)
                                    {
                                        canEvilIce = true;
                                    }
                                }
                            }
                            if (canDirtMud)
                            {
                                hasOBjectOrParam["Evil Tiles for Mud"] += 1;
                            }
                            if (canEvilGrass)
                                hasOBjectOrParam["Evil Tiles for Jungle Grass"] += 1;
                            if (canEvilSand)
                                hasOBjectOrParam["Evil Tiles for Sand"] += 1;
                            hasOBjectOrParam["Evil Tiles for Sand"] += ((tile.type == TileID.Ebonsand || tile.type == TileID.Crimsand) ? 1 : 0);//adds itself ..... da ist selbst evil, gibt aber auch karten ohne
                            if (canEvilIce)
                                hasOBjectOrParam["Evil Tiles for Ice"] += 1;
                            hasOBjectOrParam["Evil Tiles for Ice"] += ((tile.type == TileID.CorruptIce || tile.type == TileID.FleshIce) ? 1 : 0);//adds itself

                            //to check if there is also snow biome wihtout evil, not 100% correct TODo
                            if ((tile.type == TileID.CorruptIce || tile.type == TileID.FleshIce || canEvilIce) && y < Main.worldSurface)
                                hasOBjectOrParam["Ice surface evil"] += (canEvilIce ? 1 : 0) + ((tile.type == TileID.CorruptIce || tile.type == TileID.FleshIce) ? 1 : 0);

                        }
                        else if (tile.type == TileID.ShadowOrbs && (tile.frameX == 0 || tile.frameX == 36) && tile.frameY == 0)
                        {
                            int dist = Math.Abs(x - Main.maxTilesX / 2);

                            if (dist < hasOBjectOrParam["Distance ShadowOrb/Heart to mid"])
                            {
                                hasOBjectOrParam["Distance ShadowOrb/Heart to mid"] = dist;
                            }
                        }

                        //check jungle loc
                        else if (tile.type == TileID.JungleGrass)
                        {
                            if (y > Main.rockLayer)
                            {
                                if (x < leftmostCavernJungleTilex) leftmostCavernJungleTilex = x;
                                if (x > rightmostCavernJungleTilex) rightmostCavernJungleTilex = x;
                            }
                            if (y < Main.rockLayer)
                            {
                                if (x < leftmostSurfaceJungleTilex) leftmostSurfaceJungleTilex = x;
                                if (x > rightmostSurfaceJungleTilex) rightmostSurfaceJungleTilex = x;
                            }
                        }
                        //granite marble near mid
                        else if ((tile.type == TileID.Marble || tile.type == TileID.Granite) )
                        {

                            if(y < Main.worldSurface + 14)
                                hasOBjectOrParam["MarbleGranite at surf dist. to mid"] = Math.Min(hasOBjectOrParam["MarbleGranite at surf dist. to mid"],
                                    Math.Abs(Main.maxTilesX/2-x));
                                                       
                        }


                        
                       



                        //dungeon farm spot detection
                        // all 3 wall types in one area
                        // +34, so player can stay on top 
                        if (y > Main.rockLayer + 34 + 1 && isInDungeon(x, y))
                        {
                            int wallCount = 1;
                            ushort tw1 = tile.wall;
                            ushort tw2 = tw1;
                            ushort tw3 = tw1;

                            for (int xi = x - 1; xi < x + 2; xi++)
                                for (int yi = y - 1; yi < y + 2; yi++)
                                {
                                    if (isInDungeon(xi, yi) && Main.tile[xi, yi].wall != tw1 && Main.tile[xi, yi].wall != tw2  )
                                    {
                                        if (wallCount == 1)
                                            tw2 = Main.tile[xi, yi].wall;
                                        else if (wallCount == 2)
                                            tw3 = Main.tile[xi, yi].wall;

                                        wallCount++;
                                    }
                                }

                            if (wallCount == 3)
                            {
                                //3 Wall intersection detected
                                bool add = true;
                                if (score.itemLocation.ContainsKey(ItemID.PaladinsHammer))
                                    foreach (var spot in score.itemLocation[ItemID.PaladinsHammer])
                                    {
                                        if (Math.Abs(spot.Item1 - x) < 42 && Math.Abs(spot.Item2 - y) < 42)
                                            add = false;
                                    }


                                if (add)
                                {
                                    const int checkSize = 6;
                                    const int minWallNum = 25;

                                    int countW1 = 0;
                                    int countW2 = 0;
                                    int countW3 = 0;

                                    for (int xi = x - checkSize; xi < x + checkSize + 1; xi++)
                                        for (int yi = y - checkSize; yi < y + checkSize + 1; yi++)
                                        {
                                            if (Main.tile[xi, yi].wall == tw1) countW1++;
                                            else if (Main.tile[xi, yi].wall == tw2) countW2++;
                                            else if (Main.tile[xi, yi].wall == tw3) countW3++;
                                        }
                                    if (countW1 < minWallNum || countW2 < minWallNum || countW3 < minWallNum)
                                        add = false;

                                    if (add)
                                        if (score.itemLocation.ContainsKey(ItemID.PaladinsHammer))
                                            score.itemLocation[ItemID.PaladinsHammer].Add(new Tuple<int, int>(x, y));
                                        else
                                            score.itemLocation.Add(ItemID.PaladinsHammer, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });

                                    hasOBjectOrParam["Dungeon farm spot"]++;
                                    hasOBjectOrParam["Dungeon farm spot 3Wall inters."]++;

                                }

                            }
                            if (wallCount > 1 && isInDungeon(x - 1, y) && isInDungeon(x + 1, y) && Main.tile[x - 1, y].wall != Main.tile[x + 1, y].wall)
                            {
                                //dungeon farm spots
                                //farm spot line from above
                                const int rangeMin = 16; //16
                                const int rangeMax = 56;//56 ,42

                                bool doSearch = true;
                                const int ignoreX = rangeMin;
                                const int ignoreY = 1;//4  TODO: recode, atm only works for 1, if greater regions cant connect anymore with 100%
                                for (int i = dungeonFarmSpotCandidates.Count - 1; i >= 0 && doSearch; i--)
                                {
                                    if (x - dungeonFarmSpotCandidates[i].Item1 > ignoreX)
                                        break;
                                    if (y - dungeonFarmSpotCandidates[i].Item4 < ignoreY && x - dungeonFarmSpotCandidates[i].Item1 < ignoreX)
                                        doSearch = false;
                                }

                                if (doSearch)
                                {



                                    ushort twl = Main.tile[x - 1, y].wall;
                                    ushort twr = Main.tile[x + 1, y].wall;

                                    bool isGood = true;
                                    for (int i = 2; i < rangeMin; i++)
                                        if (x-i<0 || Main.tile[x - i, y].wall != twl) { isGood = false; break; }
                                    if (isGood)
                                        for (int i = 2; i < rangeMin; i++)
                                            if (x + i >= Main.maxTilesX || Main.tile[x + i, y].wall != twr) { isGood = false; break; }
                                    if (isGood)
                                    {
                                        //search for 3rd wall type in line
                                        int start3 = -1;
                                        int ndw = 0;
                                        const int maxNotDung = 5;
                                        for (int i = rangeMin + 1; i < rangeMax; i++)
                                        {
                                            bool isGood3 = false;
                                            if (x + i + rangeMin >= Main.maxTilesX || isInDungeon(x + i, +y))
                                            {
                                                if ((Main.tile[x + i, y].wall != twl && Main.tile[x + i, y].wall != twr))
                                                { //check if is 3rd wall type, todo: can be done more efficent
                                                    isGood3 = true;
                                                    ushort twrr = Main.tile[x + i, y].wall;
                                                    int ii = i;
                                                    for (; ii < i + rangeMin; ii++)
                                                        if (Main.tile[x + ii, y].wall != twrr) { isGood3 = false; break; }
                                                    if (!isGood3)
                                                        i = ii; //continue afterwards
                                                    else
                                                    {
                                                        start3 = i;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                                ndw++;
                                        }
                                        if (start3 > 0 && ndw < maxNotDung)
                                        {

                                            //check if place above
                                            bool validSpawn = false;
                                            const int npcSpawnRange = 35;
                                            if ( (x- rangeMin > 0 && y > npcSpawnRange+2 && x + start3 + rangeMin < Main.maxTilesX)
                                                &&
                                                ((isInDungeon(x - rangeMin, y - npcSpawnRange) && isInDungeon(x - rangeMin, y - npcSpawnRange - 1) && isInDungeon(x - rangeMin, y - npcSpawnRange - 2))
                                                || (isInDungeon(x + start3 + rangeMin, y - npcSpawnRange) && isInDungeon(x + start3 + rangeMin, y - npcSpawnRange - 1) && isInDungeon(x + start3 + rangeMin, y - npcSpawnRange - 2)))
                                                && (isInDungeon(x + start3 / 2, y - npcSpawnRange) && isInDungeon(x + start3 / 2, y - npcSpawnRange - 1) && isInDungeon(x + start3 / 2, y - npcSpawnRange - 2))
                                                )
                                            {
                                                validSpawn = true;
                                            }

                                            if (validSpawn)
                                            {
                                                bool found = false;
                                                int foundi = -1;
                                                const int maxAllSpace = 5;
                                                for (int i = dungeonFarmSpotCandidates.Count - 1; i >= 0 && doSearch; i--)
                                                {
                                                    if (x - dungeonFarmSpotCandidates[i].Item1 > ignoreX)
                                                        break;
                                                    if ((y - dungeonFarmSpotCandidates[i].Item4) == ignoreY && x - dungeonFarmSpotCandidates[i].Item1 < maxAllSpace)
                                                    {
                                                        dungeonFarmSpotCandidates[i] = new Tuple<int, int, int, int>(x, x + start3, dungeonFarmSpotCandidates[i].Item3, y);
                                                        found = true;
                                                        foundi = i;
                                                        break;
                                                    }
                                                }
                                                if (!found)
                                                    dungeonFarmSpotCandidates.Add(new Tuple<int, int, int, int>(x, x + start3, y, y));

                                                //connect to below TODO write code I'm able to read later on
                                                for (int i = dungeonFarmSpotCandidates.Count - 1; i >= 0 && doSearch; i--)
                                                {
                                                    if (x - dungeonFarmSpotCandidates[i].Item1 > ignoreX)
                                                        break;
                                                    if ((dungeonFarmSpotCandidates[i].Item4 - y) == ignoreY && x - dungeonFarmSpotCandidates[i].Item1 < maxAllSpace)
                                                    {
                                                        if (foundi < 0)
                                                            dungeonFarmSpotCandidates[i] = new Tuple<int, int, int, int>(dungeonFarmSpotCandidates[i].Item1, dungeonFarmSpotCandidates[i].Item2, y, dungeonFarmSpotCandidates[i].Item4);
                                                        else
                                                        {
                                                            dungeonFarmSpotCandidates[foundi] = new Tuple<int, int, int, int>(dungeonFarmSpotCandidates[i].Item1, dungeonFarmSpotCandidates[i].Item2, dungeonFarmSpotCandidates[foundi].Item3, dungeonFarmSpotCandidates[i].Item4);
                                                        }
                                                        break;
                                                    }
                                                }

                                            }
                                        }

                                    }

                                }//if doSearch

                            }




                        }



                        if (doFull)
                        {


                            //ESS                    
                            if ((tile.type == TileID.LargePiles2) && ((tile.frameX == (short)918 || (Main.tile[x - 1, y].frameX != 918 && tile.frameX == (short)936) ) && (tile.frameY == (short)0)
                                || (tile.frameY == (short)18 && (tile.frameX == (short)954)  && Main.tile[x - 2, y-1].frameX != (short)918 && Main.tile[x - 1, y - 1].frameX != (short)936)
                                ) 
                                ) // it may miss some which only partly exist, if added more also change duplication
                            {

                                if (score.itemLocation.ContainsKey(ItemID.EnchantedSword))
                                    score.itemLocation[ItemID.EnchantedSword].Add(new Tuple<int, int>(x, y));
                                else
                                    score.itemLocation.Add(ItemID.EnchantedSword, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });

                                int pathl = FindShortestPathInRange(ref pathLength, x, y, 2, 4, 1, 3);

                                //Enchanted sword                             
                                if (tile.wall == 68)
                                {
                                    //..shrine
                                    hasOBjectOrParam["Enchanted Sword Shrine"] += 1;

                                }
                                hasOBjectOrParam["Enchanted Sword"] += 1;
                                


                                if (checkIfNearSpawn(x, y, 165, 100) && pathl < 330)
                                {
                                    hasOBjectOrParam["Very Near Enchanted Sword"] += 1;
                                }

                                //is it in Sand?
                                bool inSand = CheckIfInSand(x + 1, y);
                                bool counted = false;
                                if (!inSand && checkIfNearSpawn(x, y, 350, 200) && pathl < 700)
                                {
                                    hasOBjectOrParam["Near Enchanted Sword"] += 1;
                                    counted = true;
                                }
                                if (checkIfNearTree(x, y, 80, 50) && tile.wall != 68)
                                {
                                    hasOBjectOrParam["Enchanted Sword near Tree"] += 1;

                                    if (!inSand && checkIfNearSpawn(x, y, 350, 2000) && pathl < 1050)
                                    {
                                        hasOBjectOrParam["Near Enchanted Sword near Tree"] += 1;
                                        hasOBjectOrParam["Near Enchanted Sword"] += counted ? 0 : 1;
                                    }
                                }
                                if (checkIfNearPyramid(x, y, 100, 50) && tile.wall != 68)
                                {
                                    hasOBjectOrParam["Enchanted Sword near Pyramid"] += 1;
                                    if (!inSand && checkIfNearSpawn(x, y, 400, 2000) && pathl < 1200)
                                    {
                                        hasOBjectOrParam["Near Enchanted Sword near Pyramid"] += 1;
                                        hasOBjectOrParam["Near Enchanted Sword"] += counted ? 0 : 1;
                                    }

                                }



                                if (pathl < hasOBjectOrParam["Pathlength to Enchanted Sword"])
                                {
                                    hasOBjectOrParam["Pathlength to Enchanted Sword"] = pathl;
                                }

                                //able to duplicate? unkown if any seed has it
                                {
                                    int bx = x;
                                    int by = y;

                                    if (tile.frameX == (short)936) bx--;
                                    if (tile.frameX == (short)954) { bx -= 2; by--; }

                                    int invalid = 0;
                                    for (int bxi = bx; bxi < bx + 3; bxi++)
                                    {
                                        for (int byi = by; byi < by + 2; byi++)
                                            if (!Main.tile[bxi, byi].active() ||  Main.tile[bxi, byi].type != TileID.LargePiles2 || Main.tile[bxi,byi].frameX != 918+(bxi-bx)*18 || Main.tile[bxi, byi].frameY != (byi - by) * 18)
                                                invalid++;
                                        if (!Main.tile[bxi, by+2].active() || (Main.tile[bxi, by + 2].active() && (Main.tile[bxi, by + 2].type == TileID.ClosedDoor || Main.tile[bxi, by + 2].type == TileID.Cobweb || Main.tile[bxi, by + 2].slope()>0 )))
                                            invalid++;
                                    }
                                    if (invalid > 0)
                                    {
                                        hasOBjectOrParam["Enchanted Sword duplication Glitch"] += 1;
                                        if (score.itemLocation.ContainsKey(ItemID.Arkhalis))
                                            score.itemLocation[ItemID.Arkhalis].Add(new Tuple<int, int>(x, y));
                                        else
                                            score.itemLocation.Add(ItemID.Arkhalis, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });

                                    }


                                }




                            }

                            //pot duplication
                            if(tile.type == TileID.Pots)
                            {
                                bool m36x = tile.frameX % 36 == 0;
                                bool m36y = tile.frameY % 36 == 0;

                                int bx = x;
                                int by = y;
                                int bfx = tile.frameX;
                                int bfy = tile.frameY;
                                //dont account overlapping pots
                                // (Main.tile[x - 1, y].type != TileID.Pots || Main.tile[x - 1, y].frameX%36 == 0 )
                                if (!m36x) { bx--; bfx -= 18; }
                                if (!m36y) { by--; bfy -= 18; }
                                                              

                                int invalid = 0;
                                int invalidSingle = 0;
                                int[] invalidTile = new int[4];
                                int gameBreaker = 0;
                                
                                for (int bxi = bx; bxi < bx + 2; bxi++)
                                {
                                    for (int byi = by; byi < by + 2; byi++)
                                    {                                        
                                        if (!Main.tile[bxi, byi].active() || Main.tile[bxi, byi].type != TileID.Pots || Main.tile[bxi, byi].frameX != bfx + (bxi - bx) * 18 || Main.tile[bxi, byi].frameY != bfy + (byi - by) * 18)
                                        {                                            
                                            invalidTile[(bxi - bx) * 2 + byi - by] = 1;
                                        }
                                    }
                                    if (!Main.tile[bxi, by + 2].active() || (Main.tile[bxi, by + 2].active() && (Main.tile[bxi, by + 2].type == TileID.ClosedDoor || Main.tile[bxi, by + 2].type == TileID.Cobweb || Main.tile[bxi, by + 2].slope() > 0)))
                                        invalid++;
                                    if ((!Main.tile[bxi, by].active() || Main.tile[bxi, by].type == TileID.Pots) && Main.tile[bxi, by - 1].active() && (Main.tile[bxi, by - 1].type == TileID.DemonAltar || Main.tile[bxi, by - 1].type == TileID.Containers))
                                        invalidSingle++;
                                    if ( (Main.tile[bxi, by+2].active() && Main.tile[bxi, by + 2].type == TileID.ClosedDoor && Main.tile[bxi, by + 3].active() && Main.tile[bxi, by + 3].type == TileID.ClosedDoor && ( (Main.tile[bxi, by + 4].active() && Main.tile[bxi, by + 4].type != TileID.ClosedDoor) || !Main.tile[bxi, by + 4].active()) ) ||
                                         (Main.tile[bxi, by - 1].active() && Main.tile[bxi, by - 1].type == TileID.ClosedDoor && Main.tile[bxi, by - 2].active() && Main.tile[bxi, by - 2].type == TileID.ClosedDoor && ((Main.tile[bxi, by - 3].active() && Main.tile[bxi, by - 3].type != TileID.ClosedDoor) || !Main.tile[bxi, by - 3].active())) ||
                                         (Main.tile[bxi, by - 1].active() && Main.tile[bxi, by - 1].type == TileID.ClosedDoor && ((Main.tile[bxi, by - 2].active() && Main.tile[bxi, by - 2].type != TileID.ClosedDoor) || !Main.tile[bxi, by - 2].active())) 
                                        )
                                        gameBreaker++;

                                }
                                invalid += invalidTile[0] + invalidTile[1] + invalidTile[2] + invalidTile[3] + invalidSingle;


                                // only count once, 00 10 01 11
                                int countIt = 1;
                                if (!m36x && !m36y && (invalidTile[0] != 1 || invalidTile[1] != 1 || invalidTile[2] != 1)) { invalid = 0; countIt = 0; }
                                else if (m36x && !m36y && (invalidTile[0] != 1 || invalidTile[2] != 1)) { invalid = 0; ; countIt = 0; }
                                else if (!m36x && m36y && (invalidTile[0] != 1)) { invalid = 0; ; countIt = 0; }

                                if (gameBreaker > 0)
                                {
                                    hasOBjectOrParam["Game breaker"] += 1;
                                    if (score.itemLocation.ContainsKey(ItemID.AvengerEmblem))
                                    {
                                        bool insert = true;
                                        foreach(var gb in score.itemLocation[ItemID.AvengerEmblem])
                                        {
                                            if (Math.Abs(gb.Item1 - x) < 2 && Math.Abs(gb.Item2 - y) < 2)
                                            {
                                                insert = false;
                                                break;
                                            }
                                        }
                                        if(insert)
                                            score.itemLocation[ItemID.AvengerEmblem].Add(new Tuple<int, int>(x, y));

                                    }
                                    else
                                        score.itemLocation.Add(ItemID.AvengerEmblem, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });

                                }

                                if (invalid > 0)
                                {
                                    
                                    hasOBjectOrParam["Pot duplication Glitch"] += 1;
                                    if(invalidSingle>0)
                                        hasOBjectOrParam["Pot duplication Glitch Single"] += 1;
                                    if(y> Main.rockLayer && invalidSingle>0)
                                        hasOBjectOrParam["Pot duplication Glitch Single Cavern"] += 1;

                                    int pathl = FindShortestPathInRange(ref pathLength, x, y, 3, 3, 3, 3);
                                    if (pathl < hasOBjectOrParam["Pathlength to Pot dupl. Glitch"])
                                        hasOBjectOrParam["Pathlength to Pot dupl. Glitch"] = pathl;

                                    


                                    if (invalidSingle > 0)
                                    {
                                        if (pathl < hasOBjectOrParam["Pathlength to Pot dupl. Glitch Single"])
                                            hasOBjectOrParam["Pathlength to Pot dupl. Glitch Single"] = pathl;

                                        if (score.itemLocation.ContainsKey(ItemID.PotStatue))
                                            score.itemLocation[ItemID.PotStatue].Add(new Tuple<int, int>(x, y));
                                        else
                                            score.itemLocation.Add(ItemID.PotStatue, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });
                                    }
                                    else
                                    {
                                        if (score.itemLocation.ContainsKey(ItemID.PotionStatue))
                                            score.itemLocation[ItemID.PotionStatue].Add(new Tuple<int, int>(x, y));
                                        else
                                            score.itemLocation.Add(ItemID.PotionStatue, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });
                                    }




                                }
                            }

                            //cabin finder
                            if ( (tile.type == TileID.ClosedDoor && tile.frameY != 612) ||
                                (tile.type == TileID.Platforms && Main.tile[x, y].frameY != 414) ||
                                (tile.type == TileID.Torches) ||
                                (tile.type == TileID.Chandeliers) ||
                                (tile.type == TileID.Chairs) ||
                                (tile.type == TileID.Tables)
                                )
                            {
                                int pathl = FindShortestPathInRange(ref pathLength, x, y, 2, 2, 2, 2);
                                if (pathl < hasOBjectOrParam["Pathlength to cabin"])
                                    hasOBjectOrParam["Pathlength to cabin"] = pathl;

                            }


                            //todo sort by frequenzy or switch

                            //Temple Door
                            if (tile.type == TileID.ClosedDoor && tile.frameY == 612)
                            {
                                hasOBjectOrParam["Temple door distance"] = getDistanceToSpawn(x, y);
                                hasOBjectOrParam["Temple door horizontal distance"] = Math.Abs(x - Main.spawnTileX);

                                int pathl = FindShortestPathInRange(ref pathLength, x, y, 1, 1, 1, 1);
                                if (pathl < hasOBjectOrParam["Pathlength to Temple Door"])
                                {
                                    hasOBjectOrParam["Pathlength to Temple Door"] = pathl;
                                    //writeDebugFile(" temple door at " + x + " " + y + " " + pathl);

                                }
                            }
                            else if (tile.type == TileID.LihzahrdAltar && tile.frameX == 0 && tile.frameY == 0)
                            {
                                if (pathLength[x, y] < Int32.MaxValue)
                                {
                                    hasOBjectOrParam[OptionsDict.Phase3.openTemple] = 1;
                                }

                            }

                            //Altar
                            else if (tile.type == TileID.DemonAltar && (tile.frameY == (short)0) && (tile.frameX == (short)18 || tile.frameX == (short)72))
                            {
                                int pathl = FindShortestPathInRange(ref pathLength, x, y, 3, 3, 3, 3);

                                if (pathl < hasOBjectOrParam["Pathlength to Altar"])
                                    hasOBjectOrParam["Pathlength to Altar"] = pathl;

                                if (checkIfNearSpawn(x, y, 300, 200))
                                {
                                    hasOBjectOrParam["Near Altar"] += 1;
                                }
                                else if (y < Main.worldSurface)
                                {
                                    if (x < Main.maxTilesX / 2 && x > hasOBjectOrParam["Beach left"])
                                    {
                                        hasOBjectOrParam["Nearest Altar left beach"] = Math.Min(x - hasOBjectOrParam["Beach left"], hasOBjectOrParam["Nearest Altar left beach"]);
                                    }
                                    else if (x > Main.maxTilesX / 2 && x < hasOBjectOrParam["Beach right"])
                                    {
                                        hasOBjectOrParam["Nearest Altar right beach"] = Math.Min(hasOBjectOrParam["Beach right"] - x, hasOBjectOrParam["Nearest Altar right beach"]);
                                    }
                                }
                            }
                            //high hives
                            else if (tile.type == TileID.Larva && tile.frameX == 18 && tile.frameY == 18)
                            {


                                int pathl = FindShortestPathInRange(ref pathLength, x, y, 2, 2, 2, 2);
                                if (pathl < hasOBjectOrParam["Pathlength to Bee Hive"])
                                    hasOBjectOrParam["Pathlength to Bee Hive"] = pathl;

                                hasOBjectOrParam["Bee Hives"]++;

                                //TODO do better
                                if ((y - Main.worldSurface) < 200)
                                {
                                    int yoff = 0;
                                    for (yoff = 0; yoff < 200; yoff++)
                                    {
                                        if (Main.tile[x, y - yoff].wall != 86 && ((Main.tile[x, y - yoff].active() && Main.tile[x, y - yoff].type != TileID.Hive) || !Main.tile[x, y - yoff].active()))
                                            break;

                                    }

                                    int yi = y - yoff;
                                    for (; y - yi < 200; yi--)
                                    {
                                        int countTiles = 0;
                                        int i = 0;
                                        for (; i < 100; i++)
                                        {
                                            if (x - i < 0 || x + i >= Main.maxTilesX)
                                                continue;
                                            //191 && 192 exclude living tree
                                            if ((Main.tile[x + i, yi].active() || Main.tile[x + i, yi].wall > 0) && Main.tile[x + i, yi].type != TileID.LivingWood && Main.tile[x + i, yi].type != TileID.LeafBlock)
                                                countTiles++;
                                            if ((Main.tile[x - i, yi].active() || Main.tile[x - i, yi].wall > 0) && Main.tile[x + i, yi].type != TileID.LivingWood && Main.tile[x + i, yi].type != TileID.LeafBlock)
                                                countTiles++;
                                            if (countTiles > 20)
                                                break;
                                            //check if tiles above
                                        }
                                        if (countTiles <= 20)
                                            break;
                                    }


                                    var digDist = y - yi;
                                    if (digDist < 200)
                                    {
                                        hasOBjectOrParam["High Hive"] += 1;
                                    }

                                }

                            }

                            //Sunflower
                            else if (tile.type == TileID.Sunflower && tile.frameY == (short)0 && (tile.frameX == (short)0 || tile.frameX == (short)36 || tile.frameX == (short)72))
                            {
                                if (checkIfNearSpawn(x, y, 200, 100))
                                {
                                    hasOBjectOrParam["Near Sunflower"] += 1;
                                }
                            }
                            //Statues
                            else if (tile.type == TileID.Statues || tile.type == TileID.MushroomStatue)
                            {
                                int sx = x;
                                int sy = y;
                                ushort st = tile.type;

                                //todo might not be true alle the time
                                //while (Main.tile[sx, sy].type == st && (Main.tile[sx, sy   ].frameY - Main.tile[sx, sy -1].frameY <20) && (Main.tile[sx, sy-1].frameX - Main.tile[sx, sy].frameX == 0))  sy--; sy++;
                                //while (Main.tile[sx, sy].type == st && (Main.tile[sx -1, sy].frameX - Main.tile[sx , sy].frameX < 20) && (Main.tile[sx-1, sy].frameY - Main.tile[sx, sy].frameY == 0)) sx--; sx++;

                                // if (x - sx > 2 || y - sy > 3)
                                //writeDebugFile(" something weird happened at " + x + " " + y + " statue lefttop got " + sx + " " + sy);

                                //care only about top left trame
                                short fx = Main.tile[sx, sy].frameX;
                                short fy = Main.tile[sx, sy].frameY;

                                string statIdent = st + "|" + fx + "|" + fy;
                                //mushroom is 349 | 0 0
                                //writeDebugFile("pos:" + x + " " + y + " is statue " + statIdent);

                                //extra check bc good for speed run
                                if (fx == 144 && fy == 0 && pathLength[sx, sy] < hasOBjectOrParam["Pathlength to Slime Staute"])
                                {
                                    hasOBjectOrParam["Pathlength to Slime Staute"] = pathLength[sx, sy];
                                    if (score.itemLocation.ContainsKey(ItemID.SlimeStatue))
                                        score.itemLocation[ItemID.SlimeStatue] = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) };
                                    else
                                        score.itemLocation.Add(ItemID.SlimeStatue, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });
                                }
                                else if (fx == 1800 && fy == 0 && pathLength[sx, sy] < hasOBjectOrParam["Pathlength to Shark Staute"])
                                {
                                    hasOBjectOrParam["Pathlength to Shark Staute"] = pathLength[sx, sy];
                                    if (score.itemLocation.ContainsKey(ItemID.SharkStatue))
                                        score.itemLocation[ItemID.SharkStatue] = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) };
                                    else
                                        score.itemLocation.Add(ItemID.SharkStatue, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });
                                }
                                else if (fx == 1332 && fy == 0 && pathLength[sx, sy] < hasOBjectOrParam["Pathlength to Heart Staute"])
                                {
                                    hasOBjectOrParam["Pathlength to Heart Staute"] = pathLength[sx, sy];
                                    if (score.itemLocation.ContainsKey(ItemID.HeartStatue))
                                        score.itemLocation[ItemID.HeartStatue].Add(new Tuple<int, int>(x, y) );
                                    else
                                        score.itemLocation.Add(ItemID.HeartStatue, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });
                                }
                                else if (fx == 72 && fy == 0 && pathLength[sx, sy] < hasOBjectOrParam["Pathlength to Star Staute"])
                                {
                                    hasOBjectOrParam["Pathlength to Star Staute"] = pathLength[sx, sy];
                                    if (score.itemLocation.ContainsKey(ItemID.StarStatue))
                                        score.itemLocation[ItemID.StarStatue].Add(new Tuple<int, int>(x, y));
                                    else
                                        score.itemLocation.Add(ItemID.StarStatue, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });
                                }



                                //it counts each tile of each statue, divide by 6 to get the real number == 26 max
                                if ((fy == 0 && (fx == 72 || fx == 144 || fx == 252 || fx == 360 || fx == 612 || fx == 648 || fx == 828 || fx == 972 || fx == 1332 ||
                                    fx == 1440 || fx == 1476 || fx == 1512 || fx == 1800))
                                    ||
                                    (fy == 54 && (fx == 288 || fx == 324 || fx == 360 || fx == 396 || fx == 432 || fx == 468 || fx == 504 || fx == 540 || fx == 576 || fx == 612 || fx == 648 ||
                                    fx == 684 || fx == 720))
                                    )
                                {
                                    if (statuesFunctionalCount.ContainsKey(statIdent))
                                        statuesFunctionalCount[statIdent] += 1;
                                    else
                                    {
                                        statuesFunctionalCount.Add(statIdent, 1);
                                    }
                                }

                                if ((
                                    (fx >= 1656 && fx <= 1764) ||
                                    (fy == 0 && (fx == 0 || fx == 288 || fx == 324 || fx == 1008 || fx == 1836 || fx == 1872 || fx == 1908 || fx == 1944)) ||
                                    (fy == 54 && (fx >= 0 && fx <= 252))
                                    ) && st == TileID.Statues
                                    )
                                {
                                    //omit 4 vases, and statues you can build like critters and armor statue

                                }
                                else if (fx % 36 == 0 && fy % 54 == 0)
                                {
                                    //max 30 including lihzahrd, angle might be in chest, or only 29? cant get Mushroom statue -->! mushroom has extra tile id, working now

                                    if (fx == 36 && fy == 0 && st == TileID.Statues)
                                    {
                                        hasOBjectOrParam["Angel Statue placed"]++;
                                    }

                                    //together with funcitonal 30+26 = 56
                                    if (statuesCount.ContainsKey(statIdent))
                                        statuesCount[statIdent] += 1;
                                    else
                                    {
                                        statuesCount.Add(statIdent, 1);
                                        //writeDebugFile("pos:" + x + " " + y + " is statue " + statIdent);
                                    }
                                }


                            }

                            //Traps
                            else if (tile.type == TileID.Traps)
                            {
                                switch (tile.frameY)
                                {
                                    case 0: hasOBjectOrParam["Dart Trap"]++; break;
                                    case 18: hasOBjectOrParam["Super Dart Trap"]++; break;
                                    case 36: hasOBjectOrParam["Flame Trap"]++; break;
                                    case 54: hasOBjectOrParam["Spiky Ball Trap"]++; break;
                                    case 72: hasOBjectOrParam["Spear Trap"]++; break;
                                }
                            }

                            //Geyser 
                            else if (tile.type == TileID.GeyserTrap && tile.frameX == 0 && tile.frameX == 0)
                            {
                                hasOBjectOrParam["Geyser"]++;
                            }
                            //Detonator
                            else if (tile.type == TileID.Detonator && tile.frameX == 0 && tile.frameY == 0)
                            {                                
                                hasOBjectOrParam["Detonator"]++;
                                if (pathLength[x+1,y] < hasOBjectOrParam["Pathlength to Detonator"])
                                {
                                    hasOBjectOrParam["Pathlength to Detonator"] = pathLength[x + 1, y];
                                }
                                if (y+12 < Main.worldSurface)
                                {
                                    int numfree = 0;
                                    int numfreeL = 0;
                                    int numfreeR = 0;
                                    int total = 0;
                                    int liquid = 0;
                                    for (int xi=x-10; xi<x+12; xi++)
                                    {
                                        for (int yi = y - 9; yi < y + 2; yi++)
                                        {
                                            total++;
                                            if(Main.tile[xi,yi].wall == WallID.None && !Main.tile[xi, yi].active())
                                            {
                                                numfree++;
                                                if (xi < x)
                                                    numfreeL++;
                                                else if (xi > x)
                                                    numfreeR++;
                                            }
                                            if (Main.tile[xi, yi].liquid == 255)
                                                liquid++;
                                        }
                                    }
                                    
                                    if( (((float)numfree)/ total > 0.55 || ((float)numfreeL)/ total > 0.45 || ((float)numfreeR) / total > 0.45) && ((float)liquid)/total <0.65)
                                    {
                                        hasOBjectOrParam["Detonator at surface"]++;
                                    
                                        if(score.itemLocation.ContainsKey(ItemID.Detonator))
                                            score.itemLocation[ItemID.Detonator].Add(new Tuple<int, int>(x, y) ); 
                                        else
                                            score.itemLocation.Add(ItemID.Detonator, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });
                                    }
                                }

                            }
                            else if(tile.type == TileID.Explosives)
                            {
                                int pathl = FindShortestPathInRange(ref pathLength, x, y, 2, 2, 2, 2);

                                if (pathl < hasOBjectOrParam["Pathlength to Explosives"])
                                {
                                    hasOBjectOrParam["Pathlength to 2nd Explosives"] = hasOBjectOrParam["Pathlength to Explosives"];
                                    hasOBjectOrParam["Pathlength to Explosives"] = pathl;
                                }
                                else if (pathl < hasOBjectOrParam["Pathlength to 2nd Explosives"])
                                {
                                    hasOBjectOrParam["Pathlength to 2nd Explosives"] = pathl;
                                }



                            }
                            //Mushroom biome
                            else if (tile.type == TileID.MushroomGrass || tile.type == TileID.MushroomPlants || tile.type == TileID.MushroomTrees)
                            {
                                if (checkIfNearSpawn(x, y, 150, 400))
                                {
                                    hasOBjectOrParam["Near Mushroom Biome count"] += 1;
                                }
                                if (y < Main.worldSurface)
                                {
                                    hasOBjectOrParam["Mushroom Biome above surface count"] += 1;
                                }

                            }

                            //Chest
                            else if (tile.type == TileID.Containers)
                            {

                                if (checkIfNearSpawn(x, y, 300, 200) && (tile.frameY == (short)0))
                                {
                                    hasOBjectOrParam["Near Chest"] += 1;
                                    //sb.WriteLine("chest at {0} {1}", x.ToString(),y.ToString());
                                }

                                //Skyware Chest
                                /*if ((tile.frameX == (short)468) && (tile.frameY == (short)0))
                                {
                                    if (checkIfNearSpawn(x, y, 100, 50))
                                    {
                                        hasOBjectOrParam["Spawn in Sky"] = 1; //todo for water cloud and more than one    
                                        points += 1000;
                                    }

                                }*/

                            }
                            else if (tile.type == TileID.ShadowOrbs && (tile.frameX == 0 || tile.frameX == 36) && tile.frameY == 0)
                            {
                                int pathL = FindShortestPathInRange(ref pathLength, x, y);

                                if (pathL < Main.maxTilesX)
                                {
                                    hasOBjectOrParam["Free ShadowOrb/Heart"] += 1;

                                    if (pathL < hasOBjectOrParam["Pathlength to free ShadowOrb/Heart"])
                                        hasOBjectOrParam["Pathlength to free ShadowOrb/Heart"] = pathL;

                                    int itemid = WorldGen.crimson ? ItemID.TheUndertaker : ItemID.Musket;

                                    if (score.itemLocation.ContainsKey(itemid))
                                        score.itemLocation[itemid].Add(new Tuple<int, int>(x, y));
                                    else
                                        score.itemLocation.Add(itemid, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });

                                }
                            }

                            //floating island dungeon
                            else if (tile.type == TileID.Sunplate)
                            {
                                if (IsDungeonBrick(x + 1, y) || IsDungeonBrick(x - 1, y) || IsDungeonBrick(x, y + 1) || IsDungeonBrick(x, y - 1))
                                    hasOBjectOrParam["Floating island cabin in Dungeon"] = 1;
                            }
                            //Temple location
                            else if (tile.type == TileID.LihzahrdBrick || tile.wall == WallID.LihzahrdBrickUnsafe)
                            {
                               
                                if (x < leftmostTempleTilex) leftmostTempleTilex = x;
                                if (x > rightmostTempleTilex) rightmostTempleTilex = x;
                                if (y < topmostTempleTiley) topmostTempleTiley = y;
                                if (y > botmostTempleTiley) botmostTempleTiley = y;

                                if(tile.wall != WallID.LihzahrdBrickUnsafe)
                                {
                                    if(checkIfNearTiles(x, y, new List<ushort> {TileID.SnowBlock, TileID.IceBlock, TileID.FleshIce, TileID.CorruptIce, TileID.BreakableIce }))
                                    {
                                        hasOBjectOrParam[OptionsDict.Phase3.frozenTemple]++;                                        
                                    }
                                    int bestPL = FindShortestPathInRange(ref pathLength, x, y, 1, 1, 1, 1);
                                    if (bestPL < hasOBjectOrParam["Pathlength to Temple Tile"])
                                        hasOBjectOrParam["Pathlength to Temple Tile"] = bestPL;

                                }


                            }
                            else if (tile.type == TileID.Anvils && tile.frameY == 0 && (tile.frameX == 0 || tile.frameX == 36))
                            {
                                if (hasOBjectOrParam["Pathlength to Anvil"] > pathLength[x, y])
                                {
                                    hasOBjectOrParam["Pathlength to Anvil"] = pathLength[x, y];

                                    if (score.itemLocation.ContainsKey(ItemID.IronAnvil))
                                        score.itemLocation[ItemID.IronAnvil] = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) };
                                    else
                                        score.itemLocation.Add(ItemID.IronAnvil, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });
                                }
                            }
                            else if(Main.tile[x, y].type == TileID.Heart )
                            {
                                
                                //check for duplication
                                bool m36x = tile.frameX % 36 == 0;
                                bool m36y = tile.frameY % 36 == 0;

                                int bx = x;
                                int by = y;
                                int bfx = tile.frameX;
                                int bfy = tile.frameY;
                                //dont account all overlapping
                                
                                if (!m36x) { bx--; bfx -= 18; }
                                if (!m36y) { by--; bfy -= 18; }

                                int invalid = 0;
                                int invalidSingle = 0;
                                int[] invalidTile = new int[4];

                                for (int bxi = bx; bxi < bx + 2; bxi++)
                                {
                                    for (int byi = by; byi < by + 2; byi++)
                                    {
                                        if (!Main.tile[bxi, byi].active() || Main.tile[bxi, byi].type != TileID.Heart || Main.tile[bxi, byi].frameX != bfx + (bxi - bx) * 18 || Main.tile[bxi, byi].frameY != bfy + (byi - by) * 18)
                                        {
                                            invalidTile[(bxi - bx) * 2 + byi - by] = 1;
                                        }
                                    }
                                    if (!Main.tile[bxi, by + 2].active() || (Main.tile[bxi, by + 2].active() && (Main.tile[bxi, by + 2].type == TileID.ClosedDoor || Main.tile[bxi, by + 2].type == TileID.Cobweb || Main.tile[bxi, by + 2].slope() > 0)))
                                        invalid++;
                                    if ((!Main.tile[bxi, by].active() || Main.tile[bxi, by].type == TileID.Heart) && Main.tile[bxi, by - 1].active() && Main.tile[bxi, by - 1].active() && (Main.tile[bxi, by - 1].type == TileID.DemonAltar || Main.tile[bxi, by - 1].type == TileID.Containers))
                                        invalidSingle++;

                                }
                                invalid += invalidTile[0] + invalidTile[1] + invalidTile[2] + invalidTile[3] + invalidSingle;


                                // only count once, 00 10 01 11
                                int countIt = 1;
                                if (!m36x && !m36y && (invalidTile[0] != 1 || invalidTile[1] != 1 || invalidTile[2] != 1)) { invalid = 0; countIt = 0; }
                                else if (m36x && !m36y && (invalidTile[0] != 1 || invalidTile[2] != 1)) { invalid = 0; ; countIt = 0; }
                                else if (!m36x && m36y && (invalidTile[0] != 1)) { invalid = 0; ; countIt = 0; }

                                
                                if (countIt == 1)
                                {
                                    int pathl = FindShortestPathInRange(ref pathLength, x, y, 2, 3, 2, 3);

                                    if (invalid > 0)
                                    {
                                        hasOBjectOrParam["Life Crystal duplication Glitch"] += 1;

                                        if (invalidSingle > 0)
                                        {
                                            hasOBjectOrParam["Life Crystal duplication Glitch Single"] += 1;

                                            if (pathl < hasOBjectOrParam["Pathlength to Life Crystal dupl. Glitch Single"])
                                            {
                                                hasOBjectOrParam["Pathlength to Life Crystal dupl. Glitch Single"] = pathl;
                                            }
                                        }
                                        if (pathl < hasOBjectOrParam["Pathlength to Life Crystal dupl. Glitch"])
                                        {
                                            hasOBjectOrParam["Pathlength to Life Crystal dupl. Glitch"] = pathl;
                                        }

                                        if (score.itemLocation.ContainsKey(ItemID.HeartLantern))
                                            score.itemLocation[ItemID.HeartLantern].Add(new Tuple<int, int>(x, y));
                                        else
                                            score.itemLocation.Add(ItemID.HeartLantern, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });

                                    }

                                    

                                    if (pathl < hasOBjectOrParam["Pathlength to Crystal Heart"])
                                    {
                                        hasOBjectOrParam["Pathlength to 2nd Crystal Heart"] = hasOBjectOrParam["Pathlength to Crystal Heart"];
                                        hasOBjectOrParam["Pathlength to Crystal Heart"] = pathl;
                                    }
                                    else if (pathl < hasOBjectOrParam["Pathlength to 2nd Crystal Heart"])
                                    {

                                        hasOBjectOrParam["Pathlength to 2nd Crystal Heart"] = pathl;
                                    }
                                }



                            }
                            else if (Main.tile[x, y].active() && Main.tile[x, y].type == TileID.Ruby || (Main.tile[x, y].type == TileID.ExposedGems && Main.tile[x, y].frameX == 72) || (Main.tile[x, y].type == TileID.SmallPiles && Main.tile[x, y].frameX == 828 && Main.tile[x, y].frameY == 18))
                            {
                                int pathl = FindShortestPathInRange(ref pathLength, x, y, 2, 2, 2, 2);

                                if (pathl < hasOBjectOrParam["Pathlength to Ruby"])
                                {
                                    hasOBjectOrParam["Pathlength to Ruby"] = pathl;
                                    if (score.itemLocation.ContainsKey(ItemID.Ruby))
                                        score.itemLocation[ItemID.Ruby] = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) };
                                    else
                                        score.itemLocation.Add(ItemID.Ruby, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });


                                }

                            }
                            else if (Main.tile[x, y].active() && Main.tile[x, y].type == TileID.Diamond || (Main.tile[x, y].type == TileID.ExposedGems && Main.tile[x, y].frameX == 90) || (Main.tile[x, y].type == TileID.SmallPiles && Main.tile[x, y].frameX == 864 && Main.tile[x, y].frameY == 18) )
                            {
                                int pathl = FindShortestPathInRange(ref pathLength, x, y, 2, 2, 2, 2);

                                if (pathl < hasOBjectOrParam["Pathlength to Diamond"])
                                {
                                    hasOBjectOrParam["Pathlength to Diamond"] = pathl;
                                    if (score.itemLocation.ContainsKey(ItemID.Diamond))
                                        score.itemLocation[ItemID.Diamond] = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) };
                                    else
                                        score.itemLocation.Add(ItemID.Diamond, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });
                                }

                            }


                            else if (Main.tile[x, y].type == TileID.Extractinator && Main.tile[x, y].frameX == 0 && Main.tile[x, y].frameY == 0)
                            {
                                int pathl = FindShortestPathInRange(ref pathLength, x, y, 2, 4, 2, 4);

                                if (pathl < hasOBjectOrParam["Pathlength to Extractinator"])
                                {
                                    hasOBjectOrParam["Pathlength to Extractinator"] = pathl;
                                    
                                    if (score.itemLocation.ContainsKey(ItemID.Extractinator))
                                        score.itemLocation[ItemID.Extractinator] = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) };
                                    else
                                        score.itemLocation.Add(ItemID.Extractinator, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });

                                }
                            }
                            //green pyramid
                            else if (Main.tile[x, y].type == TileID.Banners && Main.tile[x, y].frameY == 0 && Main.tile[x, y].wall == WallID.GrassUnsafe && (Main.tile[x, y].frameX == 72 || Main.tile[x, y].frameX == 90 || Main.tile[x, y].frameX == 108))
                            {
                                hasOBjectOrParam[OptionsDict.Phase3.greenPyramid ]= 1;

                            }
                            else if( tile.type == TileID.MinecartTrack)
                            {                                
                                    hasOBjectOrParam["Pathlength to Minecart Track"] = Math.Min(pathLength[x,y], hasOBjectOrParam["Pathlength to Minecart Track"]);
                            }


                            //Web
                            if (tile.wall == 62)
                            {
                                if (checkIfNearSpawn(x, y, 150, 500))
                                {
                                    hasOBjectOrParam["Near Spider Web count"] += 1;
                                }

                            }

                            


                                



                        }



                    }//end active





                }//end for y

            }//end for x





            score.activeTiles = activeTiles;
            score.evilTiles = evilTiles;

            hasOBjectOrParam[OptionsDict.Phase3.frozenTemple] = hasOBjectOrParam[OptionsDict.Phase3.frozenTemple] > 100 ? 1 : 0;

            //pictures
            hasOBjectOrParam["Number different Paintings"] = paintingsCount.Count;
            int totalP = 0;
            foreach (var pic in paintingsCount) totalP += pic.Value / 72;
            hasOBjectOrParam["Number Paintings"] = totalP;

            //open hive
            
            List<Tuple<int, int>> openHivesBelowLava = new List<Tuple<int, int>>();
            if (openHives.Count > 0)
            {
                
                List<Tuple<int, int>> openHivesBelowLavaTmp = new List<Tuple<int, int>>();
                foreach (var openh in openHives)
                {
                    if (openh.Item2 > highestFreeLava)
                    {
                        
                        bool near = false;
                        foreach (var larve in larveInDanger)
                        {
                            if (Math.Abs(larve.Item1 - openh.Item1) < 110 && (larve.Item2 - openh.Item2) < 120 && (larve.Item2 - openh.Item2) > -8)
                            {
                                near = true;
                                break;
                            }
                        }
                        if(near)
                            openHivesBelowLavaTmp.Add(openh);

                    }
                }

                if (openHivesBelowLavaTmp.Count > 0)
                {
                     
                    foreach (var openh in openHivesBelowLavaTmp)
                    {
                        bool nearIn = false;
                        foreach (var fin in openHivesBelowLava)
                        {
                            if (Math.Abs(fin.Item1 - openh.Item1) < 15 && Math.Abs(fin.Item2 - openh.Item2) < 15)
                            {
                                nearIn = true;
                                break;
                            }
                        }
                        if (!nearIn)
                        {
                            openHivesBelowLava.Add(openh);
                            if (score.itemLocation.ContainsKey(ItemID.HiveWall))
                                score.itemLocation[ItemID.HiveWall].Add(openh);
                            else
                                score.itemLocation.Add(ItemID.HiveWall, new List<Tuple<int, int>> { openh });

                        }

                    }
                    hasOBjectOrParam["Open Bee Hive below lava"] = openHivesBelowLava.Count;
                }

            }
               



            //statues

            //hasOBjectOrParam.Add("Angel Statue chest", 0) ToDO?

            hasOBjectOrParam["Different functional noncraf. Statues"] = statuesFunctionalCount.Count;
            int totalS = 0;
            foreach (var sta in statuesFunctionalCount) totalS += sta.Value;
            hasOBjectOrParam["Number functional noncraf. Statues"] = totalS;

            totalS = 0;
            hasOBjectOrParam["Different noncraf. Statues"] = statuesCount.Count;
            foreach (var sta in statuesCount) totalS += sta.Value;
            hasOBjectOrParam["Number noncraf. Statues"] = totalS;

            //sqrt dist
            hasOBjectOrParam["UG MarbleGranite dist. to spawn (guess)"] = (int)Math.Sqrt((float)hasOBjectOrParam["UG MarbleGranite dist. to spawn (guess)"]);
            hasOBjectOrParam["Top MarbleGranite dist. to spawn (guess)"] = (int)Math.Sqrt((float)hasOBjectOrParam["Top MarbleGranite dist. to spawn (guess)"]);


            //half double count
            hasOBjectOrParam["Near Chest"] = hasOBjectOrParam["Near Chest"] / 2; // todo refin

            //count total clouds
            hasOBjectOrParam["Cloud Chest"] = has(ref hasOBjectOrParam, ItemID.ShinyRedBalloon) + has(ref hasOBjectOrParam, ItemID.Starfury) + has(ref hasOBjectOrParam, ItemID.LuckyHorseshoe);

            //good trees
            hasOBjectOrParam["Tree Chest"] = has(ref hasOBjectOrParam, ItemID.LivingWoodWand); //same 

            hasOBjectOrParam["Tree"] = treesPos.Count;

            int offp = 0;
            int offpc = 0;
           
            while (true)
            {
                foreach (var treeX in treesPos)
                {
                    int midDist = Math.Abs(Main.maxTilesX / 2 - treeX);

                    if (midDist <= 300+offp)
                    {
                        hasOBjectOrParam["Trees near mid"] += 1;
                    }
                }
                if (hasOBjectOrParam["Trees near mid"] > offpc)
                {
                    offpc++;
                    offp += 75;
                    hasOBjectOrParam["Trees near mid"] = 0;
                }
                else
                    break;
            }


            //lake

            hasOBjectOrParam["Distance Lake to mid (guess)"] = liqf< Main.maxTilesX / 2 && liqt> Main.maxTilesX / 2? 0: Math.Min(Math.Abs(Main.maxTilesX / 2 - liqf), Math.Abs(Main.maxTilesX / 2 - liqt));

            if (hasOBjectOrParam["Distance Lake to mid (guess)"] < 250) {

                hasOBjectOrParam["Lake near mid (guess)"] = 1;                
            }

            //Duckspawn
            //
            {
                int dim = waterDuckSpawn.Length / 2;
                double dwscore = 0;
                for(int i=-dim; i<dim;i++)
                {
                    int val =waterDuckSpawn[i + dim];
                    if (Math.Abs(i) < 63)
                    {
                        dwscore += val * (Math.Abs(i) / 62 / 2 + 0.5);
                    } else if (Math.Abs(i) < 169)
                    {
                        dwscore += val ;
                    }
                    else if (Math.Abs(i) < 231)
                    {
                        dwscore += val * ((230-Math.Abs(i)) / 62 / 2 +0.5);
                    }
                    else if (Math.Abs(i) < 301)
                    {
                        dwscore += val * ((300 - Math.Abs(i)) / 70 / 2);
                    }
                }
                double contl=0, contr = 0;
                int newLakeV = -2 * dim;
                int sl = newLakeV, sr = newLakeV;
                for (int i = 0; i < dim-1; i++)
                {
                    if (sl == -i+1 && sr == -i+1 && i != 0 && waterDuckSpawn[dim - i] > 0 && waterDuckSpawn[dim + i] > 0)
                    {                        
                        contl += 0.5 * (2.0 + 0.1 * (2*i) - 0.3);
                        contr += 0.5 * (2.0 + 0.1 * (2*i) - 0.3);
                        sl = -i;
                        sr = -i;
                    }
                    else
                    {
                        if (waterDuckSpawn[dim - i] > 0)
                        {
                            if (sl == newLakeV) sl = i;
                            contl += 1.0 * (2.0 + 0.1 * (i - sl) - 0.3 - (i < 63 && waterDuckSpawn[dim - i] > 3 ? 0.2 : 0)  -(i<63&& waterDuckSpawn[dim - i]>6?0.2:0) - (i < 63 && waterDuckSpawn[dim - i] > 9 ? 0.4 : 0));
                        }
                        else
                        {
                            contl--;
                            sl = newLakeV;
                        }
                        if (waterDuckSpawn[dim + i] > 0)
                        {
                            if (sr == newLakeV) sr = i;
                            contr += 1.0 * (2.0 + 0.1 * (i - sr) - 0.3 - (i < 63 && waterDuckSpawn[dim + i] > 3 ? 0.2 : 0) - (i < 63 && waterDuckSpawn[dim + i] > 6 ? 0.2 : 0) - (i < 63 && waterDuckSpawn[dim + i] > 9 ? 0.4 : 0));
                        }
                        else
                        {
                            contr--;
                            sr = newLakeV;
                        }
                    }
                }               
                
                dwscore = dwscore + Math.Max(contl , contr) + Math.Max(Math.Min(contl, contr), 0)  + (waterDuckSpawn[dim]>0?-1.7:0 );
                hasOBjectOrParam["Water/Duck Score (guess)"] = (int)(dwscore+0.5+dim); //dim ... >=0,   //about 22k max

            }

            //writeDebugFile(" analyze all took " + elapsedTime);
            //tallest tree finder TODO ext fun
            CheckAndSetTreeSizes(ref hasOBjectOrParam, treesPos, stage);


            //pyramid surface
            foreach (var pyrm in genInfo.pyramidPos)
            {
                int os = CheckPyramidOpenAirSurface(pyrm.Item1, pyrm.Item2);
                
                hasOBjectOrParam["Max open air pyramid surface"] = Math.Max(hasOBjectOrParam["Max open air pyramid surface"], os);

                Tuple<int,int,int> pinfo = CheckPyramidHeight(pyrm.Item1, pyrm.Item2);

                hasOBjectOrParam["Max pyramid height"] = Math.Max(hasOBjectOrParam["Max pyramid height"], pinfo.Item1);
                hasOBjectOrParam["Max pyramid tunnel height"] = Math.Max(hasOBjectOrParam["Max pyramid tunnel height"], pinfo.Item2);
                hasOBjectOrParam["Max pyramid total height"] = Math.Max(hasOBjectOrParam["Max pyramid total height"], pinfo.Item1 + pinfo.Item2);
                hasOBjectOrParam[OptionsDict.Phase2.maxPyrExitCavDist] = Math.Max(hasOBjectOrParam[OptionsDict.Phase2.maxPyrExitCavDist], pinfo.Item3);

                //writeDebugFile(seed + " " + stage + " p stats height:" + pinfo.Item1 + " tunnel:" + pinfo.Item2 + " total:" + (pinfo.Item1 + pinfo.Item2) + " dist:" + pinfo.Item3);

                //check if pyramid is near ocean
                if (pyrm.Item1 < oceanBiomeSizeX || pyrm.Item1 > Main.maxTilesX - oceanBiomeSizeX)
                    hasOBjectOrParam["Ocean Pyramid"]++;

            }

            //good beach?
            int BeachPenaltyL = computeBeachPenalty(true);
            int BeachPenaltyR = computeBeachPenalty(false);

            if (BeachPenaltyL < 0 || BeachPenaltyR < 0)
            {
                hasOBjectOrParam["No Ocean"] = ((BeachPenaltyL < 0) ? 1 : 0) + ((BeachPenaltyR < 0) ? 1 : 0);

            }

            hasOBjectOrParam["Beach penalty left"] = BeachPenaltyL;
            hasOBjectOrParam["Beach penalty right"] = BeachPenaltyR;
            hasOBjectOrParam["Beach penalty mean"] = (BeachPenaltyR + BeachPenaltyL) / 2;
            hasOBjectOrParam["Beach penalty max"] = Math.Max(BeachPenaltyR, BeachPenaltyL);

            //evil beach?
            //int posX = 25;
            //int posY = 100;
            //int direct = 1;
            //int xOSL = findOceanToSurface(true, ref posX, ref posY, ref direct); // done on top
            //posX = xOSL < 0 ? 0 : xOSL;


            posY = (int)Main.rockLayer;
            int distance = hasOBjectOrParam["Nearest Evil left Ocean"] - hasOBjectOrParam["Beach left"];
            bool isSaveL = true;
            if (distance < 300)
            {
                isSaveL = false;
                for (int x = hasOBjectOrParam["Nearest Evil left Ocean"]; x > hasOBjectOrParam["Beach left"]; x--)
                {
                    Tile tile = Main.tile[x, posY];

                    if (tile.type == TileID.BlueDungeonBrick || tile.type == TileID.GreenDungeonBrick || tile.type == TileID.PinkDungeonBrick || tile.type == TileID.LivingWood || tile.type == TileID.SandstoneBrick)
                        isSaveL = true;

                }

            }
            hasOBjectOrParam["Nearest Evil left Ocean"] = distance;

            //int xOSR = findOceanToSurface(false, ref posX, ref posY, ref direct);
            //posX = xOSR < 0 ? Main.maxTilesX - 1 : xOSR;


            posY = (int)Main.rockLayer;
            bool isSaveR = true;
            distance = (hasOBjectOrParam["Beach right"] - hasOBjectOrParam["Nearest Evil right Ocean"]);
            if (distance < 300)
            {
                isSaveR = false;
                for (int x = hasOBjectOrParam["Nearest Evil right Ocean"]; x < Main.maxTilesX; x++)
                {
                    Tile tile = Main.tile[x, posY];

                    if (tile.type == TileID.BlueDungeonBrick || tile.type == TileID.GreenDungeonBrick || tile.type == TileID.PinkDungeonBrick || tile.type == TileID.LivingWood || tile.type == TileID.SandstoneBrick)
                        isSaveR = true;

                }

            }
            hasOBjectOrParam["Nearest Evil right Ocean"] = distance;

            hasOBjectOrParam.Add("Nearest Evil Dungeon Ocean", localDungeonSide>0? distance: hasOBjectOrParam["Nearest Evil left Ocean"]);
            hasOBjectOrParam.Add("Nearest Evil Jungle Ocean", localDungeonSide < 0 ? distance : hasOBjectOrParam["Nearest Evil left Ocean"] );

            hasOBjectOrParam["Has evil Ocean"] = (!isSaveL ? 1 : 0) + (!isSaveR ? 1 : 0); // set it to neareast form ocean

            if (dungSide == 0)
            {
                hasOBjectOrParam["Has evil Dungeon Ocean"] = (isSaveL ? 0 : 1);
                hasOBjectOrParam["Has evil Jungle Ocean"] = (isSaveR ? 0 : 1);
            }
            else
            {
                hasOBjectOrParam["Has evil Dungeon Ocean"] = (isSaveR ? 0 : 1);
                hasOBjectOrParam["Has evil Jungle Ocean"] = (isSaveL ? 0 : 1);
            }

            //writeDebugFile(" dist left" + hasOBjectOrParam["Nearest Evil left Ocean"] + " right " + hasOBjectOrParam["Nearest Evil right Ocean"] + " sl " + isSaveL + " sr " + isSaveR + " eo" + hasOBjectOrParam["Has evil Ocean"] + " bsl" + BeachPenaltyL + " bsr"+ BeachPenaltyR);


            if (dungSide == 0)
            {
                hasOBjectOrParam["Nearest Altar Dungeon beach"] = hasOBjectOrParam["Nearest Altar left beach"];
                hasOBjectOrParam["Nearest Altar Jungle beach"] = hasOBjectOrParam["Nearest Altar right beach"];
            }
            else
            {
                hasOBjectOrParam["Nearest Altar Jungle beach"] = hasOBjectOrParam["Nearest Altar left beach"];
                hasOBjectOrParam["Nearest Altar Dungeon beach"] = hasOBjectOrParam["Nearest Altar right beach"];
            }



            //dungeon penalty
            hasOBjectOrParam["Dungeon tiles above surface"] = checkDungAboveSurface();

            //below ground ? tofo refine, might not work sometimes
            int blockCountR = 0, blockCountL = 0;
            for (int x = Main.dungeonX - 50; x < Main.dungeonX && x < Main.maxTilesX; x++)
                for (int y = Main.dungeonY - 50; y < Main.dungeonY - 40 && y < Main.maxTilesY; y++)
                    blockCountL += (Main.tile[x, y].active() && Main.tile[x, y].type != 189) ? 1 : 0; // dont count clouds
            for (int x = Main.dungeonX; x < Main.dungeonX + 50 && x < Main.maxTilesX; x++)
                for (int y = Main.dungeonY - 50; y < Main.dungeonY - 40 && y < Main.maxTilesY; y++)
                    blockCountR += (Main.tile[x, y].active() && Main.tile[x, y].type != 189) ? 1 : 0; // dont count clouds


            hasOBjectOrParam["Dungeon below ground"] = (blockCountL > 100 && blockCountR > 100) || (blockCountR + blockCountL > 500) ? 1 : 0;
            if (hasOBjectOrParam["Dungeon below ground"] == 1 && treeToUGDung)
                hasOBjectOrParam["Dungeon below ground tree"] = 1; 


            int xOSD = Main.dungeonX < Main.maxTilesX / 2 ? xOSL : xOSR;
            if (Math.Abs(Main.maxTilesX / 2 - xOSD) - Math.Abs(Main.maxTilesX / 2 - Main.dungeonX) > 200 && hasOBjectOrParam["Dungeon below ground"] == 0 && hasOBjectOrParam["Dungeon tiles above surface"] == 0)
            {
                hasOBjectOrParam[OptionsDict.Phase2.dungeonGoodPos] = 1;
            }

            hasOBjectOrParam["Dungeon far above surface"] = hasOBjectOrParam["Dungeon tiles above surface"] > 95 ? 1 : 0;

            if (hasOBjectOrParam["Dungeon below ground"] > 0 || hasOBjectOrParam["Dungeon far above surface"] > 0 || hasOBjectOrParam["Dungeon in Snow Biome"] > 0)
                hasOBjectOrParam[OptionsDict.Phase2.dungeonStrangePos] = 1;
            
            //test
            if(doFull )
            {
                //Stopwatch ss = new Stopwatch();
                //ss.Start();

                //at night start you can't trust anymore -> firefly spawns use rand 1/9 chance +1
                UnifiedRandom genRand = new UnifiedRandom(0);

                int recm = 11;
                double[] randrec = new double[recm];

                int above = 2 * (int)Math.Ceiling(3.0 * 1e-5 * Main.maxTilesX * Main.maxTilesY);//some more can happen
                int below = 2 * (int)Math.Ceiling(1.5 * 1e-5 * Main.maxTilesX * Main.maxTilesY);
                int timesPerUpdate = above + below;


                //if weather changes and more/less clouds, can't trust anymore
                
                int rvc = 0;
                int rvcw = 0;
                int rvcu = 0;
              
                int fx = ((Main.spawnTileX-34)/200);
                int tx = ((Main.spawnTileX+34)/200);
                int fy = ((Main.spawnTileY-27)/150);
                int ty = ((Main.spawnTileY+27)/150);

                //writeDebugFile("pos" + fx + " " + fy + " " + tx + " " + ty + " sw" + Main.screenWidth);

                
                for (int fxi = fx; fxi <= tx; fxi++)
                    for (int fyi = fy; fyi <= ty; fyi++)
                    {
                        
                        for (int x = (fxi) * 200 - 1; x < (fxi + 1) * 200 + 1; x++)
                        {
                            for (int y = Math.Max((fyi) * 150 - 1,0); y < (fyi + 1) * 150 + 1; y++)
                            {

                                Tile tile = Main.tile[x, y];
                                if (tile != null)
                                {
                                    if (tile.wall != 0 && Main.wallLargeFrames[(int)tile.wall] != 1 && Main.wallLargeFrames[(int)tile.wall] != 2)
                                        rvcw++;

                                    if (tile.active())
                                    {
                                        ushort type = tile.type;
                                        bool a = true; bool b = true;
                                        if (!TileLoader.TileFrame(x, y, tile.type, ref a, ref b))
                                        {
                                            continue;
                                        }
                                        if (Main.tileFrameImportant[(int)tile.type] && !TileLoader.IsTorch(tile.type))
                                        {
                                            continue;
                                        }
                                        if (!Main.tileFrameImportant[(int)tile.type])
                                        {
                                            rvcu++;
                                        }

                                    }

                                }
                            }
                        }
                    }
                
                int w = 0;
                int nc = Main.numClouds;
                int o = (int)(1.05*timesPerUpdate +3+1000); //1000  to be above for sure
                //134,000 possible (tested for large worlds), 44k for small
                for (int c = 0; c < (rvc + rvcw + rvcu+nc+o); c++)
                {
                    w += (int)genRand.NextDouble();
                }
                if (w > 0 && !isPubRel) writeDebugFile("something wrong with seed " + seed);
                //writeDebugFile("" + rvc + " " + rvcw+  " " + rvcu +" " + nc+" " +o + " random values for seed " + seed + " total" + (rvc+ rvcw+ rvcu+nc+o));


                //for (int f = recm; f < 1e9; f++)//start from recm to avoid negative index in randrec
                
                for (int f = recm; f < timesPerUpdate*60*PlanBulbQuicTime * (doBulbExtra?100:1); f++)//strat from recm to avoid negative index in randrec
                {
                    randrec[f%recm]=genRand.NextDouble();

                    if((int)(randrec[f % recm]*60.0)==0 && (int)(randrec[(f-1) % recm]*25.0)==0)
                    {
                        
                        for (int s = 3; s < 8; s++)
                        {
                            int off = 0;
                            int x = (int)((randrec[(f - s) % recm] * (Main.maxTilesX - 20)) + 10);
                            int y = (int)((randrec[(f - s+ (++off)) % recm] * (Main.maxTilesY - 20 - ((int)Main.worldSurface - 1))) + (int)Main.worldSurface - 1);
                            
                            
                            if (Main.tile[x, y] == null)
                                continue;

                            ushort type = Main.tile[x, y].type;

                            if (!Main.tile[x, y].active() || type != (ushort)60)
                                continue;
                                                        
                            if ((double)y > (Main.worldSurface + Main.rockLayer) / 2.0)
                            {
                                if (   (int)(randrec[(f - s+ (++off) ) % recm]*300)  == 0)
                                {
                                    ++off; ++off;                                        
                                }
                            }   

                            if (!Main.tile[x, y].active() || type != (ushort)60)
                                continue;

                            if (!Main.tile[x, y-1].active() && (int)(randrec[(f - s + (++off)) % recm] * 10.0) == 0)
                            {
                                continue;
                            }

                            if ((int)(randrec[(f - s + (++off)) % recm] * 25.0) == 0 && Main.tile[x, y-1].liquid == 0)
                            {
                        
                                if ((int)(randrec[(f - s + (++off)) % recm] * 60.0) == 0 && off==s  )
                                {

                                    int xi = x;
                                    int yi = y-1;
                                    bool bulb = true;

                                    for (int i = xi - 1; i < xi + 1; i++)
                                    {
                                        if (Main.tile[i, yi + 1] == null)
                                        {
                                            continue;
                                        }
                                        for (int j = yi - 1; j < yi + 1; j++)
                                        {
                                            if (Main.tile[i, j] == null)
                                            {
                                                continue;
                                            }                                            
                                            if (Main.tile[i, j].active() &&
                                                Main.tile[i, j].type != TileID.JunglePlants && 
                                                Main.tile[i, j].type != TileID.JungleVines && 
                                                Main.tile[i, j].type != TileID.JungleThorns && 
                                                Main.tile[i, j].type != TileID.JunglePlants2 &&
                                                (Main.tile[i, j].type != TileID.SmallPiles || Main.tile[i, j].frameY != 0) &&
                                                Main.tile[i, j].type != TileID.PlantDetritus                  
                                                )
                                            {
                                                bulb = false;
                                            }
                                        }                                        
                                        //placed on solid jungle grass
                                        if (!WorldGen.SolidTile(i, yi + 1) || Main.tile[i, yi + 1].type != 60)
                                        {
                                            bulb = false;
                                        }
                                    }

                                    if (bulb)
                                    {
                                        int min = (f-recm-s) / timesPerUpdate / 60 / 60;
                                        int sec = (f-recm-s) / timesPerUpdate / 60 - min*60;

                                        
                                        //writeDebugFile("" + seed + " bulb at " + x + " " + y + " upd:" + (f-recm-s) + " max time: " + min + "min "+ sec + "sec" + " off:" + s );
                                        if (f < timesPerUpdate * 60 * PlanBulbQuicTime)
                                        {
                                            hasOBjectOrParam["Quick Plantera bulb prediction (beta)"]++;
                                            int yt = (-y * 10000) - (min * 100) - (sec );
                                            if (score.itemLocation.ContainsKey(ItemID.PlanteraTrophy))
                                                score.itemLocation[ItemID.PlanteraTrophy].Add(new Tuple<int, int>(x, yt));
                                            else
                                                score.itemLocation.Add(ItemID.PlanteraTrophy, new List<Tuple<int, int>> { new Tuple<int, int>(x, yt) });
                                        }
                                        else
                                        {
                                            int yt = (-y * 10000)- (min*10)-(sec/10);
                                            if (score.itemLocation.ContainsKey(ItemID.PlanteraMask))
                                                score.itemLocation[ItemID.PlanteraMask].Add( new Tuple<int, int>(x, yt) );
                                            else
                                                score.itemLocation.Add(ItemID.PlanteraMask, new List<Tuple<int, int>> { new Tuple<int, int>(x, yt) });



                                        }

                                    }

                                }


                            }


                        }


                    }
                }
                //ss.Stop();
                //writeDebugFile("quick bulb took "+ss.ElapsedMilliseconds);
            }


            //dungeon Farm spot line
            const int minDim = 6;

            /*{
                // not needed anymore, not correct, TODO: true ?
                //connect
                bool found = true;
                while (found)
                {
                    writeDebugFile("connect ");
                    found = false;
                    for (int i = 0; i < dungeonFarmSpotCandidates.Count; i++)
                    {
                        for (int j = 0; j < dungeonFarmSpotCandidates.Count; j++)
                        {
                            if ((dungeonFarmSpotCandidates[j].Item3 - dungeonFarmSpotCandidates[i].Item4) == 1 && Math.Abs(dungeonFarmSpotCandidates[j].Item1 - dungeonFarmSpotCandidates[i].Item1) < 5)
                            {
                                dungeonFarmSpotCandidates[i] = new Tuple<int, int, int, int>(dungeonFarmSpotCandidates[j].Item1, dungeonFarmSpotCandidates[j].Item2, dungeonFarmSpotCandidates[i].Item3, dungeonFarmSpotCandidates[j].Item4);
                                dungeonFarmSpotCandidates.RemoveAt(j);

                                found = true;
                                writeDebugFile("found " + i + " " + j);
                            }
                        }
                    }
                }
            }*/



            foreach (var spot in dungeonFarmSpotCandidates)
            {
                if (spot.Item4 - spot.Item3 > minDim)
                {
                    if (score.itemLocation.ContainsKey(ItemID.Ectoplasm))
                        score.itemLocation[ItemID.Ectoplasm].Add(new Tuple<int, int>(spot.Item1 + (spot.Item2 - spot.Item1) / 2, spot.Item4));
                    else
                        score.itemLocation.Add(ItemID.Ectoplasm, new List<Tuple<int, int>> { new Tuple<int, int>(spot.Item1 + (spot.Item2 - spot.Item1) / 2, spot.Item4) });
                    //writeDebugFile("found line spot " + spot.Item1 + "  " + spot.Item2 + " " + spot.Item3 + "  " + spot.Item4);

                    hasOBjectOrParam["Dungeon farm spot"]++;
                    hasOBjectOrParam["Dungeon farm spot 3Wall in line"]++;
                }
            }


            //set unset for score           

            hasOBjectOrParam.Add("Pyramid Bottle", has(ref hasOBjectOrParam, ItemID.SandstorminaBottle));
            hasOBjectOrParam.Add("Pyramid Carpet", has(ref hasOBjectOrParam, ItemID.FlyingCarpet));
            hasOBjectOrParam.Add("Pyramid Mask", has(ref hasOBjectOrParam, ItemID.PharaohsMask));
            hasOBjectOrParam.Add("All Pyramid Items", has(ref hasOBjectOrParam, ItemID.PharaohsMask) > 0 && has(ref hasOBjectOrParam, ItemID.FlyingCarpet) > 0 && has(ref hasOBjectOrParam, ItemID.SandstorminaBottle) > 0 ? 1 : 0);

            hasOBjectOrParam.Add("Muramasa", has(ref hasOBjectOrParam, ItemID.Muramasa));
            hasOBjectOrParam.Add("Cobalt Shield", has(ref hasOBjectOrParam, ItemID.CobaltShield));
            hasOBjectOrParam.Add("Bone Welder", has(ref hasOBjectOrParam, ItemID.BoneWelder));
            hasOBjectOrParam.Add("Valor", has(ref hasOBjectOrParam, ItemID.Valor));

            hasOBjectOrParam.Add("Tree Chest Loom", has(ref hasOBjectOrParam, ItemID.LivingLoom));


            hasOBjectOrParam.Add("Green Dungeon Walls", wallCounts[WallID.GreenDungeonUnsafe] + wallCounts[WallID.GreenDungeonSlabUnsafe] + wallCounts[WallID.GreenDungeonTileUnsafe]);
            hasOBjectOrParam.Add("Blue Dungeon Walls", wallCounts[WallID.BlueDungeonUnsafe] + wallCounts[WallID.BlueDungeonSlabUnsafe] + wallCounts[WallID.BlueDungeonTileUnsafe]);
            hasOBjectOrParam.Add("Pink Dungeon Walls", wallCounts[WallID.PinkDungeonUnsafe] + wallCounts[WallID.PinkDungeonSlabUnsafe] + wallCounts[WallID.PinkDungeonTileUnsafe]);

            hasOBjectOrParam.Add("Dungeon Wall count", hasOBjectOrParam["Green Dungeon Walls"] + hasOBjectOrParam["Blue Dungeon Walls"] + hasOBjectOrParam["Pink Dungeon Walls"]);

            int type1 = wallCounts[WallID.PinkDungeonUnsafe] + wallCounts[WallID.BlueDungeonUnsafe] + wallCounts[WallID.GreenDungeonUnsafe];
            int type2 = wallCounts[WallID.GreenDungeonSlabUnsafe] + wallCounts[WallID.BlueDungeonSlabUnsafe] + wallCounts[WallID.PinkDungeonSlabUnsafe];
            int type3 = wallCounts[WallID.PinkDungeonTileUnsafe] + wallCounts[WallID.BlueDungeonTileUnsafe] + wallCounts[WallID.GreenDungeonTileUnsafe];

            hasOBjectOrParam.Add("All Dungeon Walls", Math.Min(Math.Min(type1, type2), type3));

            hasOBjectOrParam.Add("All Dungeon Items", has(ref hasOBjectOrParam, ItemID.Valor) > 0 &&
                                                      has(ref hasOBjectOrParam, ItemID.BoneWelder) > 0 &&
                                                      has(ref hasOBjectOrParam, ItemID.Muramasa) > 0 &&
                                                      has(ref hasOBjectOrParam, ItemID.CobaltShield) > 0 &&
                                                      has(ref hasOBjectOrParam, ItemID.Handgun) > 0 &&
                                                      has(ref hasOBjectOrParam, ItemID.AquaScepter) > 0 &&
                                                      has(ref hasOBjectOrParam, ItemID.MagicMissile) > 0 &&
                                                      has(ref hasOBjectOrParam, ItemID.BlueMoon) > 0 &&
                                                      has(ref hasOBjectOrParam, ItemID.ShadowKey) > 0 &&
                                                      hasOBjectOrParam["Water Bolt"] > 0 &&
                                                      hasOBjectOrParam["Alchemy Table"] > 0
                                                      ? 1 : 0);

            hasOBjectOrParam.Add("Evil only one side", (hasOBjectOrParam["Nearest Evil left Ocean"] < Main.maxTilesX / 2 && hasOBjectOrParam["Nearest Evil right Ocean"] < Main.maxTilesX / 2) ? 0 : 1);

            hasOBjectOrParam.Add("Distance Dungeon to mid", Math.Abs(Main.dungeonX - Main.maxTilesX / 2));
            hasOBjectOrParam.Add("Ice surface more than half evil", hasOBjectOrParam["Ice surface evil"] > (hasOBjectOrParam["Ice Surface"] + 50) ? 1 : 0);
            hasOBjectOrParam.Add("Ice surface more than half not evil", 1- hasOBjectOrParam["Ice surface more than half evil"]);


            hasOBjectOrParam["Jungle biome distance to mid"] = localDungeonSide < 0 ? (leftmostSurfaceJungleTilex - Main.maxTilesX / 2) : (Main.maxTilesX / 2 - rightmostSurfaceJungleTilex);
            hasOBjectOrParam["Snow biome distance to mid"] = localDungeonSide > 0 ? (leftmostSurfaceSnowTilex - Main.maxTilesX / 2) : (Main.maxTilesX / 2 - rightmostSurfaceSnowTilex);



            hasOBjectOrParam["neg. Distance Tree to mid"] = -hasOBjectOrParam["Distance Tree to mid"];
            hasOBjectOrParam["neg. Distance Tree Chest to mid"] = -hasOBjectOrParam["Distance Tree Chest to mid"];
            hasOBjectOrParam["neg. Distance Cloud to mid"] = -hasOBjectOrParam["Distance Cloud to mid"];
            hasOBjectOrParam["neg. Distance Pyramid to mid"] = -hasOBjectOrParam["Distance Pyramid to mid"];
            hasOBjectOrParam["neg. Distance Dungeon to mid"] = -hasOBjectOrParam["Distance Dungeon to mid"];
            hasOBjectOrParam["neg. Distance ShadowOrb/Heart to mid"] = -hasOBjectOrParam["Distance ShadowOrb/Heart to mid"];
            hasOBjectOrParam["neg. Distance Lake to mid (guess)"] = -hasOBjectOrParam["Distance Lake to mid (guess)"];

            hasOBjectOrParam["neg. Jungle biome distance to mid"] = -hasOBjectOrParam["Jungle biome distance to mid"];
            hasOBjectOrParam["neg. Snow biome distance to mid"] = -hasOBjectOrParam["Snow biome distance to mid"];
            hasOBjectOrParam["neg. Evil biome distance to mid"] = -hasOBjectOrParam["Evil biome distance to mid"];
            hasOBjectOrParam["neg. MarbleGranite at surf dist. to mid"] = -hasOBjectOrParam["MarbleGranite at surf dist. to mid"];
            hasOBjectOrParam["neg. Top MarbleGranite dist. to spawn (guess)"] = -hasOBjectOrParam["Top MarbleGranite dist. to spawn (guess)"];
            hasOBjectOrParam["neg. UG MarbleGranite dist. to spawn (guess)"] = -hasOBjectOrParam["UG MarbleGranite dist. to spawn (guess)"];

            

            hasOBjectOrParam["neg. Evil Tiles for Jungle Grass"] = -hasOBjectOrParam["Evil Tiles for Jungle Grass"];
            hasOBjectOrParam["neg. Evil Tiles for Sand"] = -hasOBjectOrParam["Evil Tiles for Sand"];
            hasOBjectOrParam["neg. Evil Tiles for Ice"] = -hasOBjectOrParam["Evil Tiles for Ice"];

            hasOBjectOrParam["neg. Nearest Evil Dungeon Ocean"] = -hasOBjectOrParam["Nearest Evil Dungeon Ocean"];
            hasOBjectOrParam["neg. Nearest Evil Jungle Ocean"] = -hasOBjectOrParam["Nearest Evil Jungle Ocean"];
            


            if (doFull)
            {


                if (hasOBjectOrParam["Cloud Chest"] + 1 + ((Main.maxTilesX > 6000) ? 1 : 0) + ((Main.maxTilesX > 8000) ? 1 : 0) < hasOBjectOrParam["Number of Clouds"])
                    hasOBjectOrParam["Floating Island without chest"] = 1;

                hasOBjectOrParam["Pathlength to Detonator"] /= pathNormFac;
                hasOBjectOrParam["Pathlength to underground MarbleGranite"] /= pathNormFac;
                hasOBjectOrParam["Pathlength to Minecart Track"] /= pathNormFac;

                hasOBjectOrParam["Minecart Track close to spawn"] = hasOBjectOrParam["Pathlength to Minecart Track"] < 150 ? 1 : 0;
                hasOBjectOrParam["ExplosiveDetonator close to spawn"] = (hasOBjectOrParam["Pathlength to Detonator"] < 20 && hasOBjectOrParam["Pathlength to Explosives"] < 35)
                                                                     || (hasOBjectOrParam["Pathlength to Explosives"] < 20 && hasOBjectOrParam["Pathlength to Detonator"] < 35) ? 1 : 0;
                

                //find path
                Stopwatch stopWatchway = new Stopwatch();
                stopWatchway.Start();

                int startx = 100;
                int endx = Main.maxTilesX - 100;
                int cy = (int)(Main.rockLayer + 0.4 * ((Main.maxTilesY - Main.rockLayer) - 200));
                int cyEnter = (int)(Main.rockLayer + 10);
                int cmp = Int32.MaxValue;
                int cmpEnter = Int32.MaxValue;
                int cmxi = 0;

                for (int cxi = startx; cxi <= endx; cxi++)
                {
                    if (pathLength[cxi, cy] < cmp && !isInDungeon(cxi, cy))
                    {
                        cmp = pathLength[cxi, cy];
                        cmxi = cxi;
                    }
                    if (pathLength[cxi, cyEnter] < cmpEnter && !isInDungeon(cxi, cyEnter))
                    {
                        cmpEnter = pathLength[cxi, cyEnter];                        
                    }
                }
                if (cmp < Int32.MaxValue)
                {
                    Tuple<List<Tuple<int, int>>, int, int> entrpath = FindCaveEntrance(ref pathLength, cmxi, cy);
                    Tuple<int, int> entr = entrpath.Item1.Last();
                    int tilesToMine = entrpath.Item2;

                    score.itemLocation.Add(ItemID.StoneBlock, entrpath.Item1);

                    score.itemLocation[ItemID.StoneBlock] = entrpath.Item1;
                    hasOBjectOrParam["Pathlength into 40% cavern layer"] = cmp / pathNormFac;
                    hasOBjectOrParam["Pathlength to 40% cavern entrance"] = pathLength[entr.Item1, entr.Item2] / pathNormFac;
                    hasOBjectOrParam["Tiles to mine for 40% cavern"] = tilesToMine;

                    hasOBjectOrParam["Pathlength into cavern layer"] = cmpEnter / pathNormFac;
                }

                //into jungle
                int jy = (int)(Main.rockLayer + 0.5 * ((Main.maxTilesY - Main.rockLayer) - 200));
                int jmp = Int32.MaxValue;
                int jmxi = leftmostCavernJungleTilex;
                int jungleWide = (rightmostCavernJungleTilex - leftmostCavernJungleTilex);
                int jungleWideSurf = (rightmostSurfaceJungleTilex - leftmostSurfaceJungleTilex);

                int borderOff = jungleWide / 4;

                startx = leftmostCavernJungleTilex + borderOff;
                endx = rightmostCavernJungleTilex - borderOff;
                if (localDungeonSide > 0) 
                    endx -= (int)(0.15 * jungleWide);
                else
                    startx += (int)(0.15 * jungleWide);


                for (int jxi = startx; jxi <= endx; jxi++)
                {
                    if (pathLength[jxi, jy] < jmp)
                    {
                        jmp = pathLength[jxi, jy];
                        jmxi = jxi;
                    }
                }
                int jmex = 0;
                int jmey = 0;
                if (jmp < Int32.MaxValue)
                {
                    Tuple<List<Tuple<int, int>>, int, int> entrpathJ = FindCaveEntrance(ref pathLength, jmxi, jy, true, leftmostCavernJungleTilex + jungleWide / 10, rightmostCavernJungleTilex - jungleWide / 10); // changed from + to -
                    score.itemLocation.Add(ItemID.JungleShirt, entrpathJ.Item1);

                    Tuple<int, int> entrJ = entrpathJ.Item1.Last();
                    jmex = entrJ.Item1;
                    jmey = entrJ.Item2;

                    hasOBjectOrParam["Pathlength to cavern entrance to mid of Jungle"] = pathLength[entrJ.Item1, entrJ.Item2] / pathNormFac;
                    hasOBjectOrParam["Tiles to mine for mid Jungle cavern"] = entrpathJ.Item2;

                    if (jmex > leftmostSurfaceJungleTilex + jungleWideSurf / 10 && jmex < rightmostSurfaceJungleTilex - jungleWideSurf / 10 && entrpathJ.Item2 < Main.maxTilesY / 120 && entrpathJ.Item3 == 0)
                        hasOBjectOrParam["Free cavern to mid Jungle"] = 1; //changed rightmostSurfaceJungleTilex + jungleWideSurf /10 -> rightmostSurfaceJungleTilex - jungleWideSurf /10
                }


                int jymi = (int)(Main.rockLayer + 0.95 * ((Main.maxTilesY - Main.rockLayer) - 200));
                int jyma = (int)(Main.rockLayer + 0.99 * ((Main.maxTilesY - Main.rockLayer) - 200));

                jmp = Int32.MaxValue;
                int jmyi = Main.maxTilesY;

                startx = leftmostCavernJungleTilex;
                endx = rightmostCavernJungleTilex;
                if (localDungeonSide > 0)
                    endx -= (int)(0.50 * jungleWide);
                else
                    startx += (int)(0.50 * jungleWide);

                //for (int jyi = jymi; jyi <= jyma; jyi++)
                //for (int jxi = leftmostCavernJungleTilex + borderOff; jxi <= rightmostCavernJungleTilex - borderOff; jxi++)

                for (int jyi = jymi; jyi <= jyma; jyi++)
                    for (int jxi = startx; jxi < endx; jxi++)
                    {
                        if (pathLength[jxi, jyi] < jmp)
                        {
                            jmp = pathLength[jxi, jyi];
                            jmxi = jxi;
                            jmyi = jyi;
                        }
                    }


                int jmexd = 100;
                int jmeyd = 100;
                if (jmp < Int32.MaxValue)
                {
                    Tuple<List<Tuple<int, int>>, int, int> entrpathJB = FindCaveEntrance(ref pathLength, jmxi, jmyi, true, leftmostCavernJungleTilex + jungleWide / 10, rightmostCavernJungleTilex - jungleWide / 10); //changed + to -
                    score.itemLocation.Add(ItemID.JunglePants, entrpathJB.Item1);

                    Tuple<int, int> entrJB = entrpathJB.Item1.Last();
                    jmexd = entrJB.Item1;
                    jmeyd = entrJB.Item2;
                    hasOBjectOrParam["Pathlength to cavern entrance to deep Jungle"] = pathLength[jmexd, jmeyd] / pathNormFac;
                    hasOBjectOrParam["Tiles to mine for deep Jungle cavern"] = entrpathJB.Item2;

                    if (jmexd > leftmostSurfaceJungleTilex + jungleWideSurf / 10 && jmexd < rightmostSurfaceJungleTilex - jungleWideSurf / 10 && entrpathJB.Item2 < Main.maxTilesY / 60 && entrpathJB.Item3 == 0)
                        hasOBjectOrParam["Free cavern to deep Jungle"] = 1; //changed + to -
                }
                if (Math.Abs(jmexd - jmex) < 5 && Math.Abs(jmeyd - jmey) < 5 && hasOBjectOrParam["Free cavern to deep Jungle"] == 1 && hasOBjectOrParam["Free cavern to mid Jungle"] == 1)
                    hasOBjectOrParam["Jungle cavern not blocked by structure"] = 1;


                ts = stopWatchway.Elapsed;
                elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

                if (localDungeonSide < 0)
                {
                    int val = Math.Min(leftmostCavernJungleTilex, leftmostSurfaceJungleTilex);
                    if (val - mostSnowSideIvyChest > 100)
                        hasOBjectOrParam[OptionsDict.Phase3.lonelyJungleTree] = 1;
                }
                else
                {
                    int val = Math.Max(rightmostCavernJungleTilex, rightmostSurfaceJungleTilex);
                    if (mostSnowSideIvyChest - val > 100)
                        hasOBjectOrParam[OptionsDict.Phase3.lonelyJungleTree] = 1;
                }
                    




                int templeMidx = leftmostTempleTilex + ((rightmostTempleTilex - leftmostTempleTilex) / 2);

                int templeMidy = topmostTempleTiley + ((botmostTempleTiley - topmostTempleTiley) / 2);



                float diff = (localDungeonSide < 0) ? rightmostCavernJungleTilex - templeMidx : templeMidx - leftmostCavernJungleTilex;
                int ratio = (int)(diff / jungleWide * 100.0f);

                hasOBjectOrParam["Temple at player side of jungle (%)"] = ratio;
                hasOBjectOrParam["Temple at ocean side of jungle (%)"] = 100 - ratio;

                diff = (float)(templeMidy - Main.rockLayer);
                ratio = (int)(diff / (Main.maxTilesY - Main.rockLayer - 200) * 100.0f);
                hasOBjectOrParam["Temple at height (%)"] = 100 - ratio;
                hasOBjectOrParam["Temple at depth (%)"] = ratio;
                

                hasOBjectOrParam["Temple Tile horizontal distance"] = (localDungeonSide < 0) ? leftmostTempleTilex - Main.spawnTileX : Main.spawnTileX - rightmostTempleTilex;
                hasOBjectOrParam["Temple Tile vertical distance"] = Math.Abs(topmostTempleTiley - Main.spawnTileY);

                hasOBjectOrParam["neg. Temple Tile horizontal distance"] = -hasOBjectOrParam["Temple Tile horizontal distance"];
                hasOBjectOrParam["neg. Temple Tile vertical distance"] = -hasOBjectOrParam["Temple Tile vertical distance"];
                
                if (hasOBjectOrParam["Temple door distance"] == 0)
                {
                    //no temple door
                    hasOBjectOrParam["Temple door distance"] = getDistanceToSpawn(templeMidx, templeMidy);
                    hasOBjectOrParam["Temple door horizontal distance"] = Math.Abs(templeMidx - Main.spawnTileX);
                    hasOBjectOrParam[OptionsDict.Phase3.openTemple] = 1;
                }
                hasOBjectOrParam["neg. Temple door distance"] = -hasOBjectOrParam["Temple door distance"];
                hasOBjectOrParam["neg. Temple door horizontal distance"] = -hasOBjectOrParam["Temple door horizontal distance"];


                //################################################## use geninfo?
                score.closestTreeToSpawn = 13370;                
                foreach (var treeX in treesPos)
                {
                    int spawnDist = Math.Abs(Main.spawnTileX - treeX);
                    score.closestTreeToSpawn = Math.Min(score.closestTreeToSpawn, spawnDist);

                    if (spawnDist <= 275)
                    {
                        hasOBjectOrParam["Near Tree"] += 1;
                    }                    
                }




                //TODO if 0 clouds
                foreach (var cloudX in cloudPos)
                {
                    if (checkIfNearSpawn(cloudX, Main.spawnTileY, 275, 1000))
                    {
                        hasOBjectOrParam["Near Cloud"] += 1;
                    }
                }

                int numJ = 0;
                int numS = 0;
                int numE = 0;
                int numGM = 0;
                for (int xi = Main.spawnTileX - 6; xi < Main.spawnTileX + 7; xi++)
                    for (int yi = Main.spawnTileY - 6; yi < Main.spawnTileY + 7; yi++)
                    {
                        if (Main.tile[xi, yi].active())
                        {

                            if (Main.tile[xi, yi].type == TileID.SnowBlock || Main.tile[xi, yi].type == TileID.IceBlock || Main.tile[xi, yi].type == TileID.FleshIce || Main.tile[xi, yi].type == TileID.CorruptIce)
                                numS++;
                            if (Main.tile[xi, yi].type == TileID.JungleGrass || Main.tile[xi, yi].type == TileID.JunglePlants || Main.tile[xi, yi].type == TileID.JunglePlants2 || Main.tile[xi, yi].type == TileID.JungleThorns || Main.tile[xi, yi].type == TileID.JungleVines)
                                numJ++;
                            if (Main.tile[xi, yi].type == TileID.CorruptGrass || Main.tile[xi, yi].type == TileID.CorruptThorns || Main.tile[xi, yi].type == TileID.CorruptSandstone
                                || Main.tile[xi, yi].type == TileID.CorruptPlants || Main.tile[xi, yi].type == TileID.CorruptIce || Main.tile[xi, yi].type == TileID.CorruptHardenedSand
                                || Main.tile[xi, yi].type == TileID.Ebonsand || Main.tile[xi, yi].type == TileID.Ebonstone || Main.tile[xi, yi].type == TileID.Ebonwood
                                || Main.tile[xi, yi].type == TileID.Crimsand || Main.tile[xi, yi].type == TileID.CrimsonHardenedSand || Main.tile[xi, yi].type == TileID.CrimsonSandstone
                                || Main.tile[xi, yi].type == TileID.CrimsonVines || Main.tile[xi, yi].type == TileID.Crimstone || Main.tile[xi, yi].type == TileID.CrimtaneThorns
                                || Main.tile[xi, yi].type == TileID.FleshGrass || Main.tile[xi, yi].type == TileID.FleshIce || Main.tile[xi, yi].type == TileID.FleshWeeds || Main.tile[xi, yi].type == TileID.Shadewood)
                                numE++; //maybe something missing here
                            if (Main.tile[xi, yi].type == TileID.Granite || Main.tile[xi, yi].type == TileID.Marble || Main.tile[xi, yi].wall == WallID.GraniteUnsafe || Main.tile[xi, yi].wall == WallID.MarbleUnsafe)
                                numGM++;
                        }
                    }
                hasOBjectOrParam.Add("Spawn in Snow biome", numS > 10 ? 1 : 0);
                hasOBjectOrParam.Add("Spawn in Jungle biome", numJ > 10 ? 1 : 0);
                hasOBjectOrParam.Add("Spawn in Evil biome", numE > 10 ? 1 : 0);
                hasOBjectOrParam.Add("Spawn in Marble or Granite biome", numGM > 4 ? 1 : 0);

                hasOBjectOrParam.Add("Mushroom Biome above surface", hasOBjectOrParam["Mushroom Biome above surface count"] > 50 ? 1 : 0); //todo higher value if exist

                //set unset for score
                hasOBjectOrParam.Add("Cloud Ballon", has(ref hasOBjectOrParam, ItemID.ShinyRedBalloon));
                hasOBjectOrParam.Add("Cloud Starfury", has(ref hasOBjectOrParam, ItemID.Starfury));
                hasOBjectOrParam.Add("Cloud Horseshoe", has(ref hasOBjectOrParam, ItemID.LuckyHorseshoe));
                hasOBjectOrParam.Add("Cloud Sky Mill", has(ref hasOBjectOrParam, ItemID.SkyMill));
                hasOBjectOrParam.Add("All Cloud Items", hasOBjectOrParam["Cloud Ballon"] > 0 && hasOBjectOrParam["Cloud Starfury"] > 0 && hasOBjectOrParam["Cloud Horseshoe"] > 0 && hasOBjectOrParam["Cloud Sky Mill"] > 0 ? 1 : 0);


                hasOBjectOrParam.Add("Flower Boots", has(ref hasOBjectOrParam, ItemID.FlowerBoots));
                hasOBjectOrParam.Add("Staff of Regrowth", has(ref hasOBjectOrParam, ItemID.StaffofRegrowth));
                hasOBjectOrParam.Add("Fiberglass Fishing Pole", has(ref hasOBjectOrParam, ItemID.FiberglassFishingPole));
                hasOBjectOrParam.Add("Feral Claws", has(ref hasOBjectOrParam, ItemID.FeralClaws));
                hasOBjectOrParam.Add("Anklet of the Wind", has(ref hasOBjectOrParam, ItemID.AnkletoftheWind));



                hasOBjectOrParam.Add("Lava Charm", has(ref hasOBjectOrParam, ItemID.LavaCharm));
                hasOBjectOrParam.Add("Water Walking Boots", has(ref hasOBjectOrParam, ItemID.WaterWalkingBoots));

                hasOBjectOrParam.Add("Dungeon Distance", Math.Abs(Main.dungeonX - Main.spawnTileX));
                
                hasOBjectOrParam.Add("Underground Distance to spawn", (int)Math.Abs(Main.worldSurface - Main.spawnTileY));
                hasOBjectOrParam.Add("Cavern Distance to spawn", (int)Math.Abs(Main.rockLayer - Main.spawnTileY));

                hasOBjectOrParam.Add("Seaweed Pet", has(ref hasOBjectOrParam, ItemID.Seaweed));
                hasOBjectOrParam.Add("Fish Pet", has(ref hasOBjectOrParam, ItemID.Fish));



                hasOBjectOrParam.Add("Enchanted Sword near Pyramid or Tree", hasOBjectOrParam["Enchanted Sword near Pyramid"] + hasOBjectOrParam["Enchanted Sword near Tree"]);

                //hasOBjectOrParam["Very Near Enchanted Sword"] += hasOBjectOrParam["Near Enchanted Sword near Pyramid"] + hasOBjectOrParam["Near Enchanted Sword near Tree"];





                //other rare items
                hasOBjectOrParam.Add("Aglet", has(ref hasOBjectOrParam, ItemID.Aglet));
                hasOBjectOrParam.Add("Hermes Boots", has(ref hasOBjectOrParam, ItemID.HermesBoots));

                hasOBjectOrParam.Add("Blizzard in a Bottle", has(ref hasOBjectOrParam, ItemID.BlizzardinaBottle));
                hasOBjectOrParam.Add("Ice Machine", has(ref hasOBjectOrParam, ItemID.IceMachine));
                hasOBjectOrParam.Add("Snowball Cannon", has(ref hasOBjectOrParam, ItemID.SnowballCannon));
                hasOBjectOrParam.Add("Ice Boomerang", has(ref hasOBjectOrParam, ItemID.IceBoomerang));
                hasOBjectOrParam.Add("Ice Blade", has(ref hasOBjectOrParam, ItemID.IceBlade));
                hasOBjectOrParam.Add("Ice Skates", has(ref hasOBjectOrParam, ItemID.IceSkates));
                hasOBjectOrParam.Add("Living Mahogany Wand", has(ref hasOBjectOrParam, ItemID.LivingMahoganyWand));
                hasOBjectOrParam.Add("Honey Dispenser", has(ref hasOBjectOrParam, ItemID.HoneyDispenser));
                hasOBjectOrParam.Add("Band of Regeneration", has(ref hasOBjectOrParam, ItemID.BandofRegeneration));
                hasOBjectOrParam.Add("Shoe Spikes", has(ref hasOBjectOrParam, ItemID.ShoeSpikes));
                hasOBjectOrParam.Add("Magic Mirror", has(ref hasOBjectOrParam, ItemID.MagicMirror));
                hasOBjectOrParam.Add("Ice Mirror", has(ref hasOBjectOrParam, ItemID.IceMirror));
                hasOBjectOrParam.Add("Flurry Boots", has(ref hasOBjectOrParam, ItemID.FlurryBoots));




                hasOBjectOrParam.Add(OptionsDict.Phase3.allChestItemsNoCraftFish, hasOBjectOrParam["All Pyramid Items"] > 0 &&
                                                                                hasOBjectOrParam["Valor"] > 0 &&
                                                                                hasOBjectOrParam["Bone Welder"] > 0 &&
                                                                                hasOBjectOrParam["Flower Boots"] > 0 &&
                                                                                hasOBjectOrParam["Tree Chest Loom"] > 0 &&
                                                                                hasOBjectOrParam["Cloud Sky Mill"] > 0 &&
                                                                                hasOBjectOrParam["Blizzard in a Bottle"] > 0 &&
                                                                                hasOBjectOrParam["Ice Blade"] > 0 &&
                                                                                hasOBjectOrParam["Ice Boomerang"] > 0 &&
                                                                                hasOBjectOrParam["Ice Skates"] > 0 &&
                                                                                hasOBjectOrParam["Ice Machine"] > 0 &&
                                                                                hasOBjectOrParam["Snowball Cannon"] > 0 &&
                                                                                hasOBjectOrParam["Fish Pet"] > 0 &&
                                                                                hasOBjectOrParam["Seaweed Pet"] > 0 &&
                                                                                hasOBjectOrParam["Honey Dispenser"] > 0 &&
                                                                                hasOBjectOrParam["Living Mahogany Wand"] > 0 &&
                                                                                hasOBjectOrParam["Shoe Spikes"] > 0 &&
                                                                                hasOBjectOrParam["Band of Regeneration"] > 0 &&
                                                                                hasOBjectOrParam["Lava Charm"] > 0 &&
                                                                                hasOBjectOrParam["Water Walking Boots"] > 0 &&
                                                                                hasOBjectOrParam["Magic Mirror"] > 0 &&
                                                                                hasOBjectOrParam["Ice Mirror"] > 0 &&
                                                                                hasOBjectOrParam["Flurry Boots"] > 0 &&
                                                                                hasOBjectOrParam["Hermes Boots"] > 0 &&
                                                                                hasOBjectOrParam["Shadow Chest"] > 4 &&  //ToDO more detailed
                                                                                hasOBjectOrParam["Web Covered Chest"] > 0 &&  //ToDO more detailed
                                                                                hasOBjectOrParam["Angel Statue placed"] + hasOBjectOrParam["Angel Statue chest"] > 0
                                                                                ? 1 : 0);
                //hasOBjectOrParam.Add("Has Alchemy, Sharpening, Enchanted",  );

                hasOBjectOrParam.Add("All Paintings", hasOBjectOrParam["Number different Paintings"] > 50? 1 : 0 );

                

                //negative value of pathlength for postive list
                hasOBjectOrParam["neg. Pathlength to Temple Door"] = -hasOBjectOrParam["Pathlength to Temple Door"];
                hasOBjectOrParam["neg. Pathlength to Temple Tile"] = -hasOBjectOrParam["Pathlength to Temple Tile"];
                hasOBjectOrParam["neg. Pathlength to Boots"] = -hasOBjectOrParam["Pathlength to Boots"];
                hasOBjectOrParam["neg. Pathlength to Copper/Tin Bar"] = -hasOBjectOrParam["Pathlength to Copper/Tin Bar"];
                hasOBjectOrParam["neg. Pathlength to Iron/Lead Bar"] = -hasOBjectOrParam["Pathlength to Iron/Lead Bar"];
                hasOBjectOrParam["neg. Pathlength to 10 Iron/Lead Bar Chest"] = -hasOBjectOrParam["Pathlength to 10 Iron/Lead Bar Chest"];
                hasOBjectOrParam["neg. Pathlength to 12 Iron/Lead Bar Chest"] = -hasOBjectOrParam["Pathlength to 12 Iron/Lead Bar Chest"];
                hasOBjectOrParam["neg. Pathlength to Silver/Tungsten Bar"] = -hasOBjectOrParam["Pathlength to Silver/Tungsten Bar"];                
                hasOBjectOrParam["neg. Pathlength to Gold/Platinum Bar"] = -hasOBjectOrParam["Pathlength to Gold/Platinum Bar"];                
                hasOBjectOrParam["neg. Pathlength to Bomb"] = -hasOBjectOrParam["Pathlength to Bomb"];
                hasOBjectOrParam["neg. Pathlength to Dynamite"] = -hasOBjectOrParam["Pathlength to Dynamite"];
                hasOBjectOrParam["neg. Pathlength to 2nd Dynamite"] = -hasOBjectOrParam["Pathlength to 2nd Dynamite"];
                hasOBjectOrParam["neg. Pathlength to Gravitation Potion"] = -hasOBjectOrParam["Pathlength to Gravitation Potion"];
                hasOBjectOrParam["neg. Pathlength to Crystal Heart"] = -hasOBjectOrParam["Pathlength to Crystal Heart"];
                hasOBjectOrParam["neg. Pathlength to 2nd Crystal Heart"] = -hasOBjectOrParam["Pathlength to 2nd Crystal Heart"];
                hasOBjectOrParam["neg. Pathlength to Jester's Arrow"] = -hasOBjectOrParam["Pathlength to Jester's Arrow"];

                hasOBjectOrParam["neg. Pathlength to Suspicious Looking Eye"] = -hasOBjectOrParam["Pathlength to Suspicious Looking Eye"];
                hasOBjectOrParam["neg. Pathlength to Snowball Cannon"] = -hasOBjectOrParam["Pathlength to Snowball Cannon"];
                hasOBjectOrParam["neg. Pathlength to Teleport Potion"] = -hasOBjectOrParam["Pathlength to Teleport Potion"];
                hasOBjectOrParam["neg. Pathlength to 2 Teleport Potion Chest"] = -hasOBjectOrParam["Pathlength to 2 Teleport Potion Chest"];

                hasOBjectOrParam["neg. Pathlength to 2nd Teleport Potion"] = -hasOBjectOrParam["Pathlength to 2nd Teleport Potion"];
                hasOBjectOrParam["neg. Pathlength to Lava Charm"] = -hasOBjectOrParam["Pathlength to Lava Charm"];
                hasOBjectOrParam["neg. Pathlength to Water Walking Boots"] = -hasOBjectOrParam["Pathlength to Water Walking Boots"];
                hasOBjectOrParam["neg. Pathlength to Fish Pet"] = -hasOBjectOrParam["Pathlength to Fish Pet"];
                hasOBjectOrParam["neg. Pathlength to Seaweed Pet"] = -hasOBjectOrParam["Pathlength to Seaweed Pet"];
              
                hasOBjectOrParam["neg. Pathlength to Meteorite Bar"] = -hasOBjectOrParam["Pathlength to Meteorite Bar"];
                hasOBjectOrParam["neg. Pathlength to Obsidian Skin Potion"] = -hasOBjectOrParam["Pathlength to Obsidian Skin Potion"];
                hasOBjectOrParam["neg. Pathlength to Battle Potion"] = -hasOBjectOrParam["Pathlength to Battle Potion"];
                hasOBjectOrParam["neg. Pathlength to Lifeforce Potion"] = -hasOBjectOrParam["Pathlength to Lifeforce Potion"];
                hasOBjectOrParam["neg. Pathlength to Recall Potion"] = -hasOBjectOrParam["Pathlength to Recall Potion"];
                hasOBjectOrParam["neg. Pathlength to Builder Potion"] = -hasOBjectOrParam["Pathlength to Builder Potion"];
                hasOBjectOrParam["neg. Pathlength to Rope"] = -hasOBjectOrParam["Pathlength to Rope"];

                

                hasOBjectOrParam["neg. Pathlength to Enchanted Sword"] = -hasOBjectOrParam["Pathlength to Enchanted Sword"];
                hasOBjectOrParam["neg. Pathlength to Altar"] = -hasOBjectOrParam["Pathlength to Altar"];
                hasOBjectOrParam["neg. Pathlength to Bee Hive"] = -hasOBjectOrParam["Pathlength to Bee Hive"];
                hasOBjectOrParam["neg. Pathlength to Boomstick"] = -hasOBjectOrParam["Pathlength to Boomstick"];
                hasOBjectOrParam["neg. Pathlength to Flower Boots"] = -hasOBjectOrParam["Pathlength to Flower Boots"];

                hasOBjectOrParam["neg. Pathlength to Slime Staute"] = -hasOBjectOrParam["Pathlength to Slime Staute"];
                hasOBjectOrParam["neg. Pathlength to Shark Staute"] = -hasOBjectOrParam["Pathlength to Shark Staute"];
                hasOBjectOrParam["neg. Pathlength to Heart Staute"] = -hasOBjectOrParam["Pathlength to Heart Staute"];
                hasOBjectOrParam["neg. Pathlength to Star Staute"] = -hasOBjectOrParam["Pathlength to Star Staute"];
                hasOBjectOrParam["neg. Pathlength to Anvil"] = -hasOBjectOrParam["Pathlength to Anvil"];
                hasOBjectOrParam["neg. Pathlength to Ruby"] = -hasOBjectOrParam["Pathlength to Ruby"];
                hasOBjectOrParam["neg. Pathlength to Diamond"] = -hasOBjectOrParam["Pathlength to Diamond"];
                hasOBjectOrParam["neg. Pathlength to Cloud in a Bottle"] = -hasOBjectOrParam["Pathlength to Cloud in a Bottle"];
                hasOBjectOrParam["neg. Pathlength to 2 Herb Bag Chest"] = -hasOBjectOrParam["Pathlength to 2 Herb Bag Chest"];
                hasOBjectOrParam["neg. Pathlength to Grenades"] = -hasOBjectOrParam["Pathlength to Grenades"];
                hasOBjectOrParam["neg. Pathlength to Extractinator"] = -hasOBjectOrParam["Pathlength to Extractinator"];
                hasOBjectOrParam["neg. Pathlength to Detonator"] = -hasOBjectOrParam["Pathlength to Detonator"];
                hasOBjectOrParam["neg. Pathlength to Explosives"] = -hasOBjectOrParam["Pathlength to Explosives"];
                hasOBjectOrParam["neg. Pathlength to 2nd Explosives"] = -hasOBjectOrParam["Pathlength to 2nd Explosives"];
                hasOBjectOrParam["neg. Pathlength to Magic/Ice Mirror"] = -hasOBjectOrParam["Pathlength to Magic/Ice Mirror"];
                hasOBjectOrParam["neg. Pathlength to Chest"] = -hasOBjectOrParam["Pathlength to Chest"];
                hasOBjectOrParam["neg. Pathlength to 2nd Chest"] = -hasOBjectOrParam["Pathlength to 2nd Chest"];
                hasOBjectOrParam["neg. Pathlength to 3rd Chest"] = -hasOBjectOrParam["Pathlength to 3rd Chest"];
                hasOBjectOrParam["neg. Pathlength to 4th Chest"] = -hasOBjectOrParam["Pathlength to 4th Chest"];
                hasOBjectOrParam["neg. Pathlength to 5th Chest"] = -hasOBjectOrParam["Pathlength to 5th Chest"];
                hasOBjectOrParam["neg. Pathlength to underground Chest"] = -hasOBjectOrParam["Pathlength to underground Chest"];
                hasOBjectOrParam["neg. Pathlength to 2nd underground Chest"] = -hasOBjectOrParam["Pathlength to 2nd underground Chest"];
                hasOBjectOrParam["neg. Pathlength to 3rd underground Chest"] = -hasOBjectOrParam["Pathlength to 3rd underground Chest"];
                hasOBjectOrParam["neg. Pathlength to 4th underground Chest"] = -hasOBjectOrParam["Pathlength to 4th underground Chest"];
                hasOBjectOrParam["neg. Pathlength to 5th underground Chest"] = -hasOBjectOrParam["Pathlength to 5th underground Chest"];


                

                hasOBjectOrParam["neg. Pathlength to Wooden Chest"] = -hasOBjectOrParam["Pathlength to Wooden Chest"]; 
                hasOBjectOrParam["neg. Pathlength to Golden Chest"] = -hasOBjectOrParam["Pathlength to Golden Chest"]; 
                hasOBjectOrParam["neg. Pathlength to Water Chest"] = -hasOBjectOrParam["Pathlength to Water Chest"]; 
                hasOBjectOrParam["neg. Pathlength to Tree Chest"] = -hasOBjectOrParam["Pathlength to Tree Chest"]; 
                hasOBjectOrParam["neg. Pathlength to Pyramid Chest"] = -hasOBjectOrParam["Pathlength to Pyramid Chest"];
                hasOBjectOrParam["neg. Pathlength to cabin"] = -hasOBjectOrParam["Pathlength to cabin"];
                hasOBjectOrParam["neg. Pathlength to Minecart Track"] = -hasOBjectOrParam["Pathlength to Minecart Track"];

                hasOBjectOrParam["neg. Pathlength to free ShadowOrb/Heart"] = -hasOBjectOrParam["Pathlength to free ShadowOrb/Heart"];
                hasOBjectOrParam["neg. Pathlength to Chest duplication Glitch"] = -hasOBjectOrParam["Pathlength to Chest duplication Glitch"];
                hasOBjectOrParam["neg. Pathlength to Pot dupl. Glitch"] = -hasOBjectOrParam["Pathlength to Pot dupl. Glitch"];
                hasOBjectOrParam["neg. Pathlength to Pot dupl. Glitch Single"] = -hasOBjectOrParam["Pathlength to Pot dupl. Glitch Single"];
                hasOBjectOrParam["neg. Pathlength to Life Crystal dupl. Glitch"] = -hasOBjectOrParam["Pathlength to Life Crystal dupl. Glitch"];
                hasOBjectOrParam["neg. Pathlength to Life Crystal dupl. Glitch Single"] = -hasOBjectOrParam["Pathlength to Life Crystal dupl. Glitch Single"];                
                hasOBjectOrParam["neg. Pathlength to Floating dupl. Glitch structure"] = -hasOBjectOrParam["Pathlength to Floating dupl. Glitch structure"];                
                hasOBjectOrParam["neg. Pathlength to underground MarbleGranite"] = -hasOBjectOrParam["Pathlength to underground MarbleGranite"];
                hasOBjectOrParam["neg. Pathlength into cavern layer"] = -hasOBjectOrParam["Pathlength into cavern layer"];
                hasOBjectOrParam["neg. Pathlength into 40% cavern layer"] = -hasOBjectOrParam["Pathlength into 40% cavern layer"];
                hasOBjectOrParam["neg. Pathlength to 40% cavern entrance"] = -hasOBjectOrParam["Pathlength to 40% cavern entrance"];
                hasOBjectOrParam["neg. Tiles to mine for 40% cavern"] = -hasOBjectOrParam["Tiles to mine for 40% cavern"];
                hasOBjectOrParam["neg. Pathlength to cavern entrance to mid of Jungle"] = -hasOBjectOrParam["Pathlength to cavern entrance to mid of Jungle"];
                hasOBjectOrParam["neg. Tiles to mine for mid Jungle cavern"] = -hasOBjectOrParam["Tiles to mine for mid Jungle cavern"];
                hasOBjectOrParam["neg. Pathlength to cavern entrance to deep Jungle"] = -hasOBjectOrParam["Pathlength to cavern entrance to deep Jungle"];
                hasOBjectOrParam["neg. Tiles to mine for deep Jungle cavern"] = -hasOBjectOrParam["Tiles to mine for deep Jungle cavern"];


            }



            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            //writeDebugFile(" analyze all took " + elapsedTime);

            pathLength = null; //someone got oom, test if that helps
            treesPos = null;
            cloudPos = null;
            tileCounts = null;
            paintingsCount = null;
            statuesCount = null;
            statuesFunctionalCount = null;
            wallCounts = null;
            dungeonFarmSpotCandidates = null;
            stopWatch = null;



            if (!ModLoader.windows)
            {
                //some problems with linux garbage collector, large worlds throw out of memory exception
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
            }

        }

        /*
        Tuple<int, int> FindCaveEntrance(ref int[,] pathlength, int x, int y, int cplval, int deep)
        {
            Tuple<int, int>[] ways = new Tuple<int, int>[] {
                    new Tuple<int, int> (x+1 , y),
                    new Tuple<int, int> (x+1 , y+1),
                    new Tuple<int, int> (x , y+1),
                    new Tuple<int, int> (x-1 , y+1),
                    new Tuple<int, int> (x-1 , y),
                    new Tuple<int, int> (x-1 , y-1),
                    new Tuple<int, int> (x , y-1),
                    new Tuple<int, int> (x+1 , y-1),
                };
            List<int> n = new List<int> { pathlength[x + 1, y], pathlength[x + 1, y + 1], pathlength[x, y + 1], pathlength[x - 1, y + 1], pathlength[x - 1, y], pathlength[x - 1, y - 1], pathlength[x, y - 1], pathlength[x + 1, y - 1] };
            int minv = n.Min();
            int mini = n.IndexOf(minv);

            x = ways[mini].Item1;
            y = ways[mini].Item2;

            deep--;
            if (deep == 0 || (Main.tile[x, y].wall == WallID.None && Main.tile[x, y].active() == false && (y < Main.worldSurface || y < Main.spawnTileY - 10)))
                return ways[mini];
            else
                return FindCaveEntrance(ref pathlength, x, y, minv, deep);
        }*/

        private Tuple<int, int, int> CheckSurface(int startY, int beachLx, int beachRx)
        {
            int cury = startY;
            int size = beachRx - beachLx;
            if (size == Main.maxTilesX) size--; //in case bot oceans are invalid

            int[] diff = new int[size];
            bool[] trueval = new bool[size];
            int startX = beachLx;
            const int maxv = 3;
            for (int countx = 0; countx < size; countx++)
            {
                int curx = startX + countx + 1;

                ushort ttype = Main.tile[curx, cury].type;

                if (ttype != TileID.LivingWood && Main.tile[curx, cury].wall != WallID.LivingWood && ttype != TileID.LeafBlock && !isInDungeon(curx, cury) && ttype != TileID.PinkDungeonBrick && ttype != TileID.GreenDungeonBrick && ttype != TileID.BlueDungeonBrick)
                {
                    int i;
                    for (i = -maxv; i < maxv; i++)
                        if (CheckIfSolidAndNoTree(curx, cury + i) || Main.tile[curx, cury + i].wall != WallID.None) { cury += i; break; }
                    if (i == maxv) cury += maxv;

                }
                if (cury == Main.maxTilesY)
                {
                    //should only happen if wg changed and e.g. empty map
                    cury -= maxv;
                    trueval[countx] = false;
                }
                else
                {
                    if (CheckIfSolidAndNoTree(curx, cury) || Main.tile[curx, cury].type == TileID.LivingWood || Main.tile[curx, cury].wall == WallID.LivingWood || Main.tile[curx, cury].type == TileID.LeafBlock || isInDungeon(curx, cury))
                        trueval[countx] = true;
                    else
                        trueval[countx] = false;
                }

                diff[countx] = (int)Main.worldSurface - cury;

                //debug
                //Main.tile[curx, cury].active(true);
                //Main.tile[curx, cury].type = (ushort)(196 + (trueval[countx]==true? 1: 0));
            }
            //TODO merge to one function



            startX = beachRx;
            int[] diff2 = new int[size];
            bool[] trueval2 = new bool[size];

            for (int countx = 0; countx < size; countx++)
            {
                int curx = startX - countx - 1;

                ushort ttype = Main.tile[curx, cury].type;
                if (ttype != TileID.LivingWood && Main.tile[curx, cury].wall != WallID.LivingWood && ttype != TileID.LeafBlock && !isInDungeon(curx, cury) && ttype != TileID.PinkDungeonBrick && ttype != TileID.GreenDungeonBrick && ttype != TileID.BlueDungeonBrick)
                {
                    int i;
                    for (i = -maxv; i < maxv; i++)
                        if (CheckIfSolidAndNoTree(curx, cury + i) || Main.tile[curx, cury + i].wall != WallID.None) { cury += i; break; }
                    if (i == maxv) cury += maxv;

                }
                if (cury == Main.maxTilesY)
                {
                    //should only happen if wg changed and e.g. empty map
                    cury -= maxv;
                    trueval2[size - countx - 1] = false;
                }
                else
                {
                    if (CheckIfSolidAndNoTree(curx, cury) || Main.tile[curx, cury].type == TileID.LivingWood || Main.tile[curx, cury].wall == WallID.LivingWood || Main.tile[curx, cury].type == TileID.LeafBlock || isInDungeon(curx, cury))
                        trueval2[size - countx - 1] = true;
                    else
                        trueval2[size - countx - 1] = false;
                }

                diff2[size - countx - 1] = (int)Main.worldSurface - cury;

                //debug
                //Main.tile[curx, cury].active(true);
                //Main.tile[curx, cury].type = (ushort)(196 + (trueval2[size - countx - 1] ==true? 1: 0));
            }


            double mean = 0;
            double meanSq = 0;
            int min = 9000;
            int max = 0;
            for (int i = 0; i < size; i++)
            {
                diff[i] = (trueval[i] && trueval2[i]) ? Math.Max(diff[i], diff2[i]) : (!trueval[i] && !trueval2[i]) ? Math.Min(diff[i], diff2[i]) : ((trueval[i] ? diff[i] : 0) + (trueval2[i] ? diff2[i] : 0));

                if (diff[i] > max) max = diff[i];
                if (diff[i] < min) min = diff[i];

                mean += diff[i];
                meanSq += diff[i] * diff[i];

                //debug
                //Main.tile[beachLx + i, (int)Main.worldSurface - diff[i]].active(true);
                //Main.tile[beachLx + i, (int)Main.worldSurface - diff[i]].type = (ushort)(196 + (trueval[i] == true ? 1 : 0)+(trueval2[ i] == true ? 1 : 0));
            }

            //diff = null;
            //diff2 = null;
            //trueval = null;
            //trueval2 = null;

            mean /= (double)size;
            meanSq /= (double)size;

            Tuple<int, int, int> mheiStdDiff = new Tuple<int, int, int>((int)Math.Round(mean), (int)Math.Round(Math.Sqrt(meanSq - mean * mean)), (max - min));


            //smoothing //not done yet, needed? hill detection?
            int siMax = 0;
            for (int si = 0; si < siMax; si++)
            {

                //writeDebugFile(seed + " mean " + mean + " meanSq " + meanSq + " msqd " + ((int)Math.Round(Math.Sqrt(meanSq - mean * mean))) + " min " + min + " max " + max);

                mean = 0;
                meanSq = 0;
                min = 9000;
                max = 0;
                for (int i = si + 1; i < size - si - 1; i++)
                {


                    diff[i] = (diff[i - 1] + +diff[i + 1]) / 2;

                    if (diff[i] > max) max = diff[i];
                    if (diff[i] < min) min = diff[i];

                    mean += diff[i];
                    meanSq += diff[i] * diff[i];

                    //debug
                    if (si == siMax - 1)
                    {
                        //Main.tile[beachLx + i, (int)Main.worldSurface - diff[i]].active(true);
                        //Main.tile[beachLx + i, (int)Main.worldSurface - diff[i]].type = (ushort)(196 + (trueval[i] == true ? 1 : 0) + (trueval2[i] == true ? 1 : 0));
                    }
                }



                mean /= (double)size;
                meanSq /= (double)size;

                mheiStdDiff = new Tuple<int, int, int>((int)Math.Round(mean), (int)Math.Round(Math.Sqrt(meanSq - mean * mean)), (max - min));

                //writeDebugFile(seed + " mean " + mean + " meanSq " + meanSq + " msqd " + ((int)Math.Round(Math.Sqrt(meanSq - mean * mean))) + " min " + min + " max " + max);

            }


            diff = null;
            diff2 = null;
            trueval = null;
            trueval2 = null;


            return mheiStdDiff;

        }


        Tuple<List<Tuple<int, int>>, int, int> FindCaveEntrance(ref int[,] pathlength, int x, int y, bool excludeLastMiningTiles = true, int borderLeftX = -1, int borderRightX = 100000)
        {
            List<Tuple<int, int>> path = new List<Tuple<int, int>>();
            path.Add(new Tuple<int, int>(x, y));

            int ox = -1;
            int oy = -1;

            int ominv = pathlength[x + 0, y + 0];
            int tilesToMine = 0;
            int ctilesToMine = 0;
            int tilesOutside = 0;

            while ((y > Main.worldSurface && y > Main.spawnTileY + 10) || Main.tile[x, y].wall != WallID.None || Main.tile[x, y].active() == true)
            {
                ox = x;
                oy = y;

                //writeDebugFile("at " + x +"," + y + " :" +pathlength[x + 0, y + 0] + " " + pathlength[x + 1, y + 0] + " " + pathlength[x + 1, y + 1] + " " + pathlength[x - 1, y + 1] +" " + pathlength[x + 0, y + 1]  + " " + pathlength[x - 1, y + 0] + " " + pathlength[x - 1, y - 1] + " " + pathlength[x + 0, y - 1] + " " + pathlength[x + 1, y - 1]);


                int minv = pathlength[x + 0, y + 0];

                if (x < borderLeftX || x > borderRightX)
                    tilesOutside++;

                ominv = minv;



                if (minv > pathlength[x + 1, y + 0]) minv = pathlength[x + 1, y + 0];
                if (minv > pathlength[x + 1, y + 1]) minv = pathlength[x + 1, y + 1];
                if (minv > pathlength[x + 0, y + 1]) minv = pathlength[x + 0, y + 1];
                if (minv > pathlength[x - 1, y + 1]) minv = pathlength[x - 1, y + 1];
                if (minv > pathlength[x - 1, y + 0]) minv = pathlength[x - 1, y + 0];
                if (minv > pathlength[x - 1, y - 1]) minv = pathlength[x - 1, y - 1];
                if (minv > pathlength[x + 0, y - 1]) minv = pathlength[x + 0, y - 1];
                if (minv > pathlength[x + 1, y - 1]) minv = pathlength[x + 1, y - 1];

                if (minv == pathlength[x + 1, y - 1]) { x++; y--; }
                else if (minv == pathlength[x, y - 1]) { y--; }
                else if (minv == pathlength[x - 1, y - 1]) { x--; y--; }
                else if (minv == pathlength[x - 1, y]) { x--; }
                else if (minv == pathlength[x - 1, y + 1]) { x--; y++; }
                else if (minv == pathlength[x, y + 1]) { y++; }
                else if (minv == pathlength[x + 1, y + 1]) { x++; y++; }
                else if (minv == pathlength[x + 1, y]) { x++; y++; }

                if (minv < ominv - 60 && !(Main.tile[x, y].liquid > 0))
                    ctilesToMine++;
                else
                {
                    //exclude last tiles 
                    tilesToMine += ctilesToMine;
                    ctilesToMine = 0;
                }

                if (ox == x && oy == y)
                {
                    if (x != Main.spawnTileX && y != Main.spawnTileY)
                        writeDebugFile("entrance finding failed for seed " + seed);
    
                    //should not happen  , only if start is in spawn like in seed 170170245
                    break;
                }
                path.Add(new Tuple<int, int>(x, y));
            }

            if (!excludeLastMiningTiles) tilesToMine += ctilesToMine;
            return new Tuple<List<Tuple<int, int>>, int, int>(path, tilesToMine, tilesOutside);
        }


        const int pathNormFac = 5;
        private void AddWayPoints(ref int[,] pathLength, ref List<Tuple<int, int>>[] waypoints, ref short[,] travelCost, int x, int y, int l)
        {
            

            if (pathLength[x, y] < l || x < 42 || y < 10 || x > Main.maxTilesX - 43 || y > Main.maxTilesY - 11) return;


            const int costLR = pathNormFac;
            const int costU = 5;
            const int costD = 3;
            const int costDLR = 6;
            const int costULR = 7;

            //careful: tracelCost in short with value max 10,000, 3 times close to max short value
            int tcl = travelCost[x - 1, y] + travelCost[x - 1, y + 1] + travelCost[x - 1, y - 1];
            int tcr = travelCost[x + 1, y] + travelCost[x + 1, y + 1] + travelCost[x + 1, y - 1];
            int tcu = travelCost[x, y - 1] + travelCost[x + 1, y - 1];
            int tcd = travelCost[x, y + 1] + travelCost[x + 1, y + 1];

            int mpl = l + MIN_PATH_LENGTH;
            if (pathLength[x - 1, y] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x - 1, y, l, costLR, tcl);
            if (pathLength[x + 1, y] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x + 1, y, l, costLR, tcr);
            if (pathLength[x, y + 1] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x, y + 1, l, costD, tcd);
            if (pathLength[x, y - 1] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x, y - 1, l, costU, tcu, true);
            if (pathLength[x - 1, y - 1] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x - 1, y - 1, l, costULR, tcu + tcl, true);
            if (pathLength[x + 1, y - 1] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x + 1, y - 1, l, costULR, tcu + tcr, true);
            if (pathLength[x - 1, y + 1] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x - 1, y + 1, l, costDLR, tcd + tcl);
            if (pathLength[x + 1, y + 1] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x + 1, y + 1, l, costDLR, tcd + tcr);

        }



        private int TravelCost(int x, int y)
        {
            //ignores stuff not generated in vanilla world gen

            const short costWater = 20;
            const short costHoneyBon = 15;
            const short costLavaBon = 800;
            const short costFullLiqBoni = 15;

            const short costPlatform = 0;

            const short costUnderWBoni = 15;
            const short costCavernLavaBoni = 3;
            const short costCavernBoni = 2;
            const short costUnderGBoni = 1;
            const short costDungeonBoni = 1;
            const short costDungeonBelowSurfBoni = 200;

            const short costWebThorn = 25; //maybe too high e..g 2 dynamite 2127639275 , pathlegnth of 1st and 2nd dynamite
            const short costMineCart = -17; //maybe too good
            const short costDirt = 50; //maybe too high, changed from 60 to 50
            const short costStone = 100; //maybe too high, changed from 120 to 100

            const short costEvilWall = 2;


            int bon = 0;
            if (y > Main.maxTilesY - 190)
                bon += costUnderWBoni;
            else if (y > (int)(Main.rockLayer + 0.55 * ((Main.maxTilesY - Main.rockLayer) - 200)))
                bon += costCavernLavaBoni;
            else if (y > Main.rockLayer)
                bon += costCavernBoni;
            else if (y > Main.worldSurface)
                bon += costUnderGBoni;

            ushort type = Main.tile[x, y].type;
            ushort wall = Main.tile[x, y].wall;

            if (wall == WallID.EbonstoneUnsafe || wall == WallID.CorruptGrassUnsafe || wall == WallID.CrimstoneUnsafe || wall == WallID.CrimsonGrassUnsafe)
                bon += costEvilWall;

            bool inDungeon = isInDungeon(x, y);
            if (inDungeon)
            {
                if (y < Main.worldSurface + 3)
                {
                    bon += costDungeonBoni;
                }
                else
                {
                    bon += costDungeonBelowSurfBoni;
                }

            }

            if (Main.tile[x, y].liquid > 0)
            {
                if (x == 0 || x == Main.maxTilesX - 1 || y == 0 || y == Main.maxTilesY - 1)
                    bon += costFullLiqBoni;
                else if (Main.tile[x + 1, y].liquid > 0 && Main.tile[x - 1, y].liquid > 0 && Main.tile[x + 1, y - 1].liquid > 0 && Main.tile[x - 1, y - 1].liquid > 0 && Main.tile[x, y - 1].liquid > 0 && Main.tile[x + 1, y + 1].liquid > 0 && Main.tile[x - 1, y + 1].liquid > 0 && Main.tile[x, y + 1].liquid > 0)
                    bon += costFullLiqBoni;

                return costWater + bon + (Main.tile[x, y].honey() ? costHoneyBon : 0) + (Main.tile[x, y].lava() ? costLavaBon : 0);
            }



            bool val = ((!Main.tile[x, y].active()
                || type == TileID.Plants
                || type == TileID.Plants2
                || type == TileID.Trees
                || type == TileID.PalmTree
                || type == TileID.CorruptPlants
                || type == TileID.WaterCandle
                || type == TileID.Bottles
                || type == TileID.Books
                || type == TileID.Books
                || type == TileID.JunglePlants
                || type == TileID.JunglePlants2
                || type == TileID.JungleVines
                || type == TileID.MushroomPlants
                || type == TileID.MushroomTrees
                || type == TileID.SmallPiles
                || type == TileID.LargePiles
                || type == TileID.LargePiles2
                || type == TileID.Coral
                || type == TileID.BloomingHerbs
                || type == TileID.ImmatureHerbs
                || type == TileID.MatureHerbs
                || type == TileID.Vines
                || type == TileID.VineFlowers
                || type == TileID.Cactus
                || type == TileID.Banners
                || type == TileID.Lamps
                || type == TileID.PressurePlates
                || type == TileID.Stalactite
                || type == TileID.Explosives
                || type == TileID.Detonator
                || type == TileID.FleshWeeds
                || type == TileID.CrimsonVines
                || type == TileID.DyePlants
                || type == TileID.DyePlants
                || type == TileID.LongMoss
                || type == TileID.Pots
                || type == TileID.WorkBenches
                || type == TileID.Tables
                || type == TileID.Tables2
                || type == TileID.Bookcases
                || type == TileID.Anvils
                || type == TileID.Statues
                || type == TileID.Chairs                
                || type == TileID.Containers
                || type == TileID.Containers2
                || type == TileID.Sunflower
                || type == TileID.ShadowOrbs
                || type == TileID.DemonAltar
                || type == TileID.LihzahrdAltar
                || type == TileID.SharpeningStation
                || type == TileID.AlchemyTable
                || type == TileID.Larva
                || (type == TileID.ClosedDoor && Main.tile[x, y].frameY != 594 && Main.tile[x, y].frameY != 612 && Main.tile[x, y].frameY != 620)  //temple door not allowed
                ));
            if (val)
                return 0 + bon;

            if (type == TileID.Cobweb || type == TileID.CorruptThorns || type == TileID.CrimtaneThorns || type == TileID.JungleThorns)
                return costWebThorn + bon;

            if (type == TileID.Platforms)
                return costPlatform + bon;

            if (type == TileID.MinecartTrack)
                return costMineCart + bon;

            if (type == TileID.Dirt || type == TileID.Grass || type == TileID.FleshGrass || type == TileID.CorruptGrass || type == TileID.JungleGrass || type == TileID.MushroomGrass || type == TileID.Sand || type == TileID.ClayBlock || type == TileID.Mud || type == TileID.Silt || type == TileID.Ash || type == TileID.SnowBlock || type == TileID.Slush || type == TileID.HardenedSand || type == TileID.CrimsonHardenedSand || type == TileID.CorruptHardenedSand || type == TileID.Hive)
                return costDirt + bon;

            if (type == TileID.BlueDungeonBrick || type == TileID.GreenDungeonBrick || type == TileID.PinkDungeonBrick || type == TileID.Obsidian || type == TileID.Ebonstone || type == TileID.Crimstone || type == TileID.Hellstone || type == TileID.LihzahrdBrick || type == TileID.Demonite || type == TileID.Crimtane || (type == TileID.ClosedDoor && (Main.tile[x, y].frameY == 594 || Main.tile[x, y].frameY == 612 || Main.tile[x, y].frameY == 620)))
                return 10000;


            //stone, ores, mossstone, gemstone
            return costStone + bon;
        }

        private int checkInAir(ref short[,] travelCost, int x, int y)
        {
            int air = 0;
            const int inAirCost = 25;
            const int noWallCost = 30;

            air = (travelCost[x, y + 2] > 50 && Main.tile[x, y].liquid == 0) || (travelCost[x + 1, y + 2] > 50 && Main.tile[x, y].liquid == 0) ? 0 : inAirCost;
            air = Main.tile[x, y + 2].type == TileID.Platforms || Main.tile[x + 1, y + 2].type == TileID.Platforms ? 0 : air;

            if (air > 0 && Main.tile[x, y + 2].wall == 0 && Main.tile[x + 1, y + 2].wall == 0)
                air += noWallCost;

            return air;
        }


        const int MIN_PATH_LENGTH = 2;
        private void AddPoint(ref int[,] pathLength, ref List<Tuple<int, int>>[] waypoints, ref short[,] travelCost, int x, int y, int l, int newLengthAdd, int tileCost, bool countInAir = false)
        {
            const int CAN_NOT_PASS = 1000;

            //if (tileCost > CAN_NOT_PASS)
            //   return;

            int air = 0;
            if (countInAir)
            {
                air = checkInAir(ref travelCost, x, y);
                /*int val = checkInAir(ref travelCost, x, y);
                if (val > 0)
                {
                    bool inAir = true;
                    //sloooooow ==> write without for =>& break
                    int xi = 0;
                    for (xi = -6; xi <= 6 && inAir; xi+=2)
                        for (int yi = -6; yi <= 6 && inAir; yi++)
                            inAir &= (checkInAir(ref travelCost, x + xi, y + yi) > 0);

                    if (inAir)
                        air = val;

                }*/


            }



            int add = Math.Max(MIN_PATH_LENGTH, newLengthAdd + tileCost + air);


            if (add > CAN_NOT_PASS)
                return;

            int npl = l + add;

            if (pathLength[x, y] > npl)
            {
                pathLength[x, y] = npl;
                if (npl < waypoints.Length)
                {
                    if (waypoints[npl] == null)
                        waypoints[npl] = new List<Tuple<int, int>>();
                    waypoints[npl].Add(new Tuple<int, int>(x, y)); //can be in more than one list, remove from old?                    
                }
            }

        }

        //e.g. you can also access stuff through walls
        private int FindShortestPathInRange(ref int[,] pathLength, int x, int y, int mx = 2, int px = 3, int my = 2, int py = 3)
        {
            int minVal = Int32.MaxValue;

            for (int xi = x - mx; xi <= x + px; xi++)
                for (int yi = y - my; yi <= y + py; yi++)
                    minVal = pathLength[xi, yi] < minVal ? pathLength[xi, yi] : minVal;

            return minVal / pathNormFac;
        }

        


        private class ScoreWorld
        {
            public Dictionary<string, int> hasOBjectOrParam;
            public Dictionary<int, List<Tuple<int, int>>> itemLocation;
            public int points;
            public int maxPyramidCountChance;


            public int score;

            public int closestTreeToSpawn;

            public string scoreAsText;

            //from geninfo
            public int pyramids;
            public int trees;
            public int floatingIslands;
            public int closestPyramidToMid;
            public int closestTreeToMid;
            public int closestCloudToMid;

            public int rare;

            public int activeTiles;
            public int evilTiles;

            public int missingCount = 0;
            public int missingCountNot = 0;

            public bool phase3Empty = false;

            /*
            public int sandstromBottle;
            public int pharoMask;
            public int sarpet;
            public int livingLoom;
            public int flowerBoots;

            public int tree_LeafWand;
            public int blizzardBottle;
            public int fiberFishPole;*/

            public void init()
            {
                points = 0;
                pyramids = 0;
                score = 0;
                closestTreeToSpawn = 13370;
                closestTreeToMid = closestTreeToSpawn;
                closestPyramidToMid = 13370;
                closestCloudToMid = 13370;
                rare = 0;
                maxPyramidCountChance = 0;
                floatingIslands = 0;
                trees = 0;
                scoreAsText = "";
                hasOBjectOrParam = new Dictionary<string, int>();
                itemLocation = new Dictionary<int, List<Tuple<int, int>>>();
                missingCount = 0;
                missingCountNot = 0;
                phase3Empty = false;

                copperOrTin = "Random";
                ironOrLead = "Random";
                silverOrTung = "Random";
                goldOrPlat = "Random";

            }

            public void clear()
            {
                hasOBjectOrParam.Clear();
                hasOBjectOrParam = null;
            }

            public string copperOrTin = "Random";
            public string ironOrLead = "Random";
            public string silverOrTung = "Random";
            public string goldOrPlat = "Random";
            

            public void insertGenInfo(generatorInfo genInfo)
            {

                
                copperOrTin = genInfo.copperOrTin;
                ironOrLead = genInfo.ironOrLead;
                silverOrTung = genInfo.silverOrTung;
                goldOrPlat = genInfo.goldOrPlat;
                
                maxPyramidCountChance = genInfo.numPyrChance;
                pyramids = genInfo.numPyramids;
                floatingIslands = genInfo.numIsland; //todo do funtion for score which sets geninfo
                closestTreeToMid = genInfo.minTreeToMapMidDist;
                closestCloudToMid = genInfo.minCloudToMapMidDist;
                closestPyramidToMid = genInfo.minPyramidToMapMidDist;
                trees = genInfo.numTree;

                hasOBjectOrParam.Add(OptionsDict.Phase1.pyramidsPossible, maxPyramidCountChance);
                hasOBjectOrParam.Add("Number of Pyramids", pyramids);
                hasOBjectOrParam.Add("Number of Clouds", floatingIslands);
                hasOBjectOrParam.Add("Number of Living Trees", trees);

                hasOBjectOrParam.Add("Distance Tree to mid", closestTreeToMid);
                hasOBjectOrParam.Add("Distance Cloud to mid", closestCloudToMid);
                hasOBjectOrParam.Add("Distance Pyramid to mid", closestPyramidToMid);


            }

        }

        private static bool CheckIfInSand(int x, int y, int dim = 50)
        {
            int notInSand = 0;
            bool inSand = false;
            if (Main.tile[x, y].wall == WallID.HardenedSand || Main.tile[x, y].wall == WallID.Sandstone ||
                Main.tile[x + 1, y].wall == WallID.HardenedSand || Main.tile[x + 1, y].wall == WallID.Sandstone ||
                Main.tile[x - 1, y].wall == WallID.HardenedSand || Main.tile[x - 1, y].wall == WallID.Sandstone)
                inSand = true;


            if (inSand)
            {
                for (int tx = x; tx < x + dim; tx++)
                {
                    if (tx == Main.maxTilesX) break;
                    if ((Main.tile[tx, y].wall == WallID.HardenedSand || Main.tile[tx, y].wall == WallID.Sandstone) && !inSand)
                    {
                        inSand = true;
                        notInSand = 0;
                    }
                    else if ((Main.tile[tx, y].wall != WallID.HardenedSand && Main.tile[tx, y].wall != WallID.Sandstone))
                        inSand = false;

                    if (!inSand)
                    {
                        notInSand++;
                    }
                }
                if (notInSand > 0) inSand = false;
            }
            if (inSand)
            {
                for (int tx = x; tx > x - dim; tx--)
                {
                    if (tx == -1) break;
                    if ((Main.tile[tx, y].wall == WallID.HardenedSand || Main.tile[tx, y].wall == WallID.Sandstone) && !inSand)
                    {
                        inSand = true;
                        notInSand = 0;
                    }
                    else if ((Main.tile[tx, y].wall != WallID.HardenedSand && Main.tile[tx, y].wall != WallID.Sandstone))
                        inSand = false;

                    if (!inSand)
                    {
                        notInSand++;
                    }
                }
                if (notInSand > 0) inSand = false;
            }
            if (inSand)
            {
                for (int ty = y; ty < y + dim; ty++)
                {
                    if (ty == Main.maxTilesY) break;
                    if ((Main.tile[x, ty].wall == WallID.HardenedSand || Main.tile[x, ty].wall == WallID.Sandstone) && !inSand)
                    {
                        inSand = true;
                        notInSand = 0;
                    }
                    else if ((Main.tile[x, ty].wall != WallID.HardenedSand && Main.tile[x, ty].wall != WallID.Sandstone))
                        inSand = false;

                    if (!inSand)
                    {
                        notInSand++;
                    }
                }
                if (notInSand > 0) inSand = false;
            }
            if (inSand)
            {
                for (int ty = y; ty > y - dim; ty--)
                {
                    if (ty == 0) break;
                    if ((Main.tile[x, ty].wall == WallID.HardenedSand || Main.tile[x, ty].wall == WallID.Sandstone) && !inSand)
                    {
                        inSand = true;
                        notInSand = 0;
                    }
                    else if ((Main.tile[x, ty].wall != WallID.HardenedSand && Main.tile[x, ty].wall != WallID.Sandstone))
                        inSand = false;

                    if (!inSand)
                    {
                        notInSand++;
                    }
                }
                if (notInSand > 0) inSand = false;
            }

            return inSand;
        }

        private Tuple<int,int,int> removeInvalidPyrPos()
        {
            int numPyramidChanceAdv;
            int numPyramidChanceAdvUber;

            //more accurate pyramid number
            List<int> pyrPosX = new List<int>();
            if (localPyrX != null)
            {
                for (int p = 0; p < localPyrX.Length; p++)
                {
                    if (localPyrX[p] != 0 && localPyrX[p] > 300 && localPyrX[p] < Main.maxTilesX - 300
                        && ((localDungeonSide < 0 && localPyrX[p] > 240 + (double)Main.maxTilesX * 0.15)
                        || (localDungeonSide > 0 && localPyrX[p] < Main.maxTilesX - 240 - (double)Main.maxTilesX * 0.15))
                        )
                        pyrPosX.Add(localPyrX[p]);
                    //writeDebugFile("seed " + seed + " might have pyramid at " + localPyrX[p] + "," + localPyrY[p]);
                }
            }
            numPyramidChanceAdv = pyrPosX.Count;

            //even more accurate pyramid number
            pyrPosX.Sort();
            int lastx = -133700;
            List<int> conflict = new List<int>();
            int subp = 0;
            int minDist = Main.maxTilesX;
            for (int pi = 0; pi < pyrPosX.Count; pi++)
            {
                minDist = Math.Min(minDist, Math.Abs(pyrPosX[pi] - Main.maxTilesX / 2));

                if (pyrPosX[pi] - lastx < 250)
                {
                    if (conflict.Count == 0)
                    {
                        conflict.Add(lastx);
                    }
                    conflict.Add(pyrPosX[pi]);
                }
                else
                {
                    if (conflict.Count > 0)
                    {
                        if (conflict.Count == 2) subp++; //reduce count by one
                        else
                        {
                            int fx = conflict[0];
                            int lx = conflict[0];
                            int maxp = (lx - fx) / 250 + 1;

                            if (maxp == 1)
                            {
                                subp += conflict.Count - 1;
                            }
                            else
                            {
                                int l = conflict.Count;
                                bool[] used = new bool[l];
                                const int maxVal = 8; // else too long

                                int remove = l - maxp;
                                for (; remove < maxVal; remove++)
                                {
                                    int[] which = new int[remove];
                                    for (int i = 0; i < remove; i++) which[i] = i;
                                    bool works = false;

                                    while (!works)
                                    {
                                        for (int i = 0; i < l; i++) used[i] = true;
                                        for (int i = 0; i < remove; i++) used[which[i]] = false;

                                        works = true;
                                        for (int i = 0; i < l && works; i++)
                                            for (int j = i + 1; j < l && works; j++)
                                                if (used[i] && used[j])
                                                    if (conflict[j] - conflict[i] < 250) works = false;

                                        which[0]++;
                                        for (int i = 0; i < remove - 1; i++) if (which[i] == l) { which[i] = 0; which[i + 1]++; }
                                        if (which[remove - 1] == l)
                                            break;
                                    }
                                    if (works)
                                    {
                                        subp += remove;
                                        break;
                                    }

                                }
                                if (remove == maxVal)
                                {
                                    subp += remove;
                                }



                            }

                        }
                        conflict.Clear();
                    }
                }
                lastx = pyrPosX[pi];
            }
            numPyramidChanceAdvUber = numPyramidChanceAdv - subp;

            return new Tuple<int, int, int>(numPyramidChanceAdv, numPyramidChanceAdvUber, minDist);
        }

        private static bool isPosValid(int xheadLeft, int yheadLeft, int xsize = 1, int ysize = 1, bool checkWallAsWell = false)
        {

            for (int x = xheadLeft; x < xheadLeft + xsize; x++)
            {
                for (int y = yheadLeft; y < yheadLeft + ysize; y++)
                {
                    if (Main.tile[x, y].active())
                    {
                        if (IsDungeonBrick(x, y))
                            return false;
                    }
                    if (checkWallAsWell)
                    {
                        if (isInDungeon(x, y))
                            return false;
                    }

                }
            }

            return true;
        }

        private static bool IsDungeonBrick(int x, int y)
        {
            ushort type = Main.tile[x, y].type;
            if (type == 41 || type == 43 || type == 44)
                return true;
            else
                return false;
        }


        private static bool isInDungeon(int x, int y)
        {
            //sb.WriteLine("##### isindungeonpassed at {0} {1}  ", x, y);
            //dont check if border form map, do it?
            //if (!Main.Tiles[x, y].IsActive )
            //    return false;
            //sb.WriteLine("##### isindungeonpassed active {0} {1}  ", x, y);
            ushort chestWall = Main.tile[x, y].wall;
            if ((chestWall == 7 || chestWall == 8 || chestWall == 9)
                          ||
                        (chestWall == 94 || chestWall == 98 || chestWall == 96)
                          ||
                        (chestWall == 95 || chestWall == 99 || chestWall == 97))
            {
                //sb.WriteLine("##### isindungeonpassed at {0} {1}  true", x, y);
                return true;
            }
            //sb.WriteLine("##### isindungeonpassed at {0} {1}  false", x, y);
            return false;
        }

        //todo beta version, not tested in editor
        private static bool canGrabDungeonItem(int x, int y)
        {
            const int distX = 4;
            const int distY = 2;

            bool cangrab = false;


            for (int cx = x - distX - 2; cx < x + distX + 3; cx++)
            {
                for (int cy = y - distY - 3; cy < y + distY + 3; cy++)
                {
                    cangrab = cangrab || isPosValid(cx, cy, 2, 3, true);


                    if (cangrab) return true; ;
                }
            }
            return cangrab;
        }

        //todo refine code for faster versoin
        private static bool canGetDungeonItem(int x, int y, int distX, int distY)
        {

            bool canPass = isPosValid(x, y, 2, 3);
            //sb.WriteLine("####### ^start at {0} {1}  {2}", x, y, canPass);
            //check outerRim

            if (canPass)
            {
                int cx = x - distX;
                int cy = y - distY;
                canPass = false;
                for (; cx <= x + distX; cx++) canPass = canPass || !isInDungeon(cx, cy); cx--;
                for (; cy <= y + distY; cy++) canPass = canPass || !isInDungeon(cx, cy); cy--;
                for (; cx >= x - distX; cx--) canPass = canPass || !isInDungeon(cx, cy); cx++;
                for (; cy >= y - distY; cy--) canPass = canPass || !isInDungeon(cx, cy); cy++;
                //sb.WriteLine("####### passed cxy at {0} {1}  {2}", cx, cy, canPass);
            }
            //sb.WriteLine("####### passed at {0} {1}  {2}", x, y, canPass);
            if (canPass)
            {
                int cx;
                int cy;

                //check if way outside
                int[,] canGoThere = new int[distX * 2 + 1, distY * 2 + 1];
                int xOff = x - distX;
                int yOff = y - distY;

                for (cx = 0; cx < distX * 2 + 1; cx++)
                    for (cy = 0; cy < distY * 2 + 1; cy++)
                    {
                        canGoThere[cx, cy] = isPosValid(cx + xOff, cy + yOff, 2, 3) == false ? -1 : 0;
                    }

                int numChanges;
                canGoThere[distX, distY] = 1;
                int offsetX = Math.Max(distX - 1, 0);
                int offsetY = Math.Max(distY - 1, 0);
                int steps = 0;
                //TODO do something better here, very slow, with dist 15 up about 2000 steps in if per chest, some over 4000
                do
                {
                    numChanges = 0;
                    for (cx = 0 + offsetX; cx < distX * 2 + 1 - offsetX; cx++)
                        for (cy = 0 + offsetY; cy < distY * 2 + 1 - offsetX; cy++)
                            if (canGoThere[cx, cy] == 0)
                            {
                                bool canGo = false;
                                if (cy - 1 >= 0 && !canGo) canGo = canGo || (canGoThere[cx, cy - 1] > 0);
                                if (cx + 1 < 2 * distX + 1 && !canGo) canGo = canGo || (canGoThere[cx + 1, cy] > 0);
                                if (cy + 1 < 2 * distY + 1 && !canGo) canGo = canGo || (canGoThere[cx, cy + 1] > 0);
                                if (cx - 1 >= 0 && !canGo) canGo = canGo || (canGoThere[cx - 1, cy] > 0);

                                if (canGo)
                                {
                                    canGoThere[cx, cy] = 1;
                                    numChanges++;
                                }
                                steps++;
                            }
                    if (offsetX > 0) offsetX--;
                    if (offsetY > 0) offsetY--;

                } while (numChanges > 0);
                //sb.WriteLine("####### steps {0} ", steps);
                //search if somewhere can exit
                cy = 0;
                canPass = false;
                for (cx = 0; cx < 2 * distX + 1; cx++) canPass = canPass || (canGoThere[cx, cy] > 0 && !isInDungeon(cx + xOff, cy + yOff)); cx--;
                for (cy = 0; cy < 2 * distY + 1; cy++) canPass = canPass || (canGoThere[cx, cy] > 0 && !isInDungeon(cx + xOff, cy + yOff)); cy--;
                for (; cx >= 0; cx--) canPass = canPass || (canGoThere[cx, cy] > 0 && !isInDungeon(cx + xOff, cy + yOff)); cx++;
                for (; cy >= 0; cy--) canPass = canPass || (canGoThere[cx, cy] > 0 && !isInDungeon(cx + xOff, cy + yOff)); cy++;
                canGoThere = null;

            }

            return canPass;
        }



        private static int has(ref Dictionary<string, int> hasOBjectOrParam, int id)
        {
            if (!hasOBjectOrParam.ContainsKey(id.ToString())) return 0;

            return hasOBjectOrParam[id.ToString()];
        }

        private static int has(ref Dictionary<string, int> hasOBjectOrParam, string name)
        {
            if (!hasOBjectOrParam.ContainsKey(name)) return 0;

            return hasOBjectOrParam[name];
        }

        private static bool checkIfNearTiles(int tileX, int tileY, List<ushort> tilesToCheck, int radiusAround = 1)
        {
            for(int x = tileX-radiusAround; x < tileX + radiusAround +1; x++)
            {
                for (int y = tileY - radiusAround; y < tileY + radiusAround + 1; y++)
                {
                    ushort type = Main.tile[x, y].type;
                    foreach(ushort id in tilesToCheck)
                    {
                        if (type == id)
                            return true;

                    }

                }

            }
            return false;

        }


        private static bool checkIfNearTree(int curX, int curY, int xlookLR, int yaboveLook)
        {
            bool near = false;
            xlookLR = Math.Abs(xlookLR);
            int y = curY - yaboveLook;


            for (int x = curX - xlookLR; x < curX + xlookLR; x++)
            {
                if (x >= 0 && x < Main.maxTilesX && Main.tile[x, y].active() && (Main.tile[x, y].type == TileID.LivingWood))
                {
                    near = true;
                    break;
                }

            }
            return near;
        }

        private static bool checkIfNearPyramid(int curX, int curY, int xlookLR, int yaboveLook)
        {
            bool near = false;
            xlookLR = Math.Abs(xlookLR);
            int y = curY - yaboveLook;


            for (int x = curX - xlookLR; x < curX + xlookLR; x++)
            {
                if (x >= 0 && x < Main.maxTilesX && Main.tile[x, y].active() && ((Main.tile[x, y].type == TileID.SandstoneBrick) || (Main.tile[x, y].wall == WallID.SandstoneBrick)))
                {
                    near = true;
                    break;
                }

            }


            return near;
        }


        private static bool checkIfNearDungeon(int curX, int curY, int xdiff, int ydiff)
        {
            return Math.Abs(curX - Main.dungeonX) <= xdiff && Math.Abs(curY - Main.dungeonY) <= ydiff;
        }


        private static bool checkIfNearSpawn(int curX, int curY, int xdiff, int ydiff)
        {
            return Math.Abs(curX - Main.spawnTileX) <= xdiff && Math.Abs(curY - Main.spawnTileY) <= ydiff;
        }

        private static int getDistanceToSpawn(int curX, int curY)
        {
            return (int)Math.Sqrt((Main.spawnTileX - curX) * (Main.spawnTileX - curX) + (Main.spawnTileY - curY) * (Main.spawnTileY - curY));
        }

        public static void writeDebugFile(string content, bool newFile = false)
        {
            using (System.IO.StreamWriter file =
             new System.IO.StreamWriter(Main.SavePath + OptionsDict.Paths.debugPath + @"debug.txt", !newFile))
            {
                file.WriteLine(content);
            }
        }

        public static Tuple<List<int>, bool, string> readConfigFile()
        {
            string path = Main.SavePath + OptionsDict.Paths.debugPath + @"modconfig.txt";


            if (!System.IO.File.Exists(path))
                return (new Tuple<List<int>, bool, string>(null , false, ""));

            string content = "";
            using (System.IO.StreamReader file =
             new System.IO.StreamReader(path))
            {
                content = file.ReadToEnd();
            }

            if (content.Length == 0)
                return (new Tuple<List<int>, bool, string>(null , false, ""));

            content = content.Normalize();

            string[] lines = content.Split(System.Environment.NewLine.ToCharArray());
            List<int> idn = new List<int>();
            bool quickStart = false;
            string configName = "";

            for (int l = 0; l < lines.Length; l++)
            {
                
                if (lines[l].Length == 0)
                    continue;
               
                int ind = lines[l].IndexOf(OptionsDict.ModConfig.dontShowItemIDs);
                if (ind > -1)
                {
                    string[] ids = (lines[l].Substring(ind + OptionsDict.ModConfig.dontShowItemIDs.Length)).Split(',');

                    for (int i = 0; i < ids.Length; i++)
                    {
                        int idt = -1;

                        if (Int32.TryParse(ids[i], out idt))
                            idn.Add(idt);
                    }
                    continue;
                }
                ind = lines[l].IndexOf(OptionsDict.ModConfig.quickStart);
                
                if (ind > -1)
                {
                    string doOrNot = lines[l].Substring(ind + OptionsDict.ModConfig.quickStart.Length);
                    quickStart = doOrNot.Trim().Length > 0;
                    configName = doOrNot.Trim();
                    continue;
                }
            }

            return (new Tuple<List<int>, bool, string> ( (idn.Count == 0 ? null : idn), quickStart, configName));
        }


        private static int checkDungAboveSurface()
        {
            //todo refine code
            int dir = Main.dungeonX < Main.maxTilesX / 2 ? 1 : -1;

            int maxSteps = 1337;
            int dx = Main.dungeonX;
            int dy = Main.dungeonY;

            int stepsDone = 0;
            bool goodD = false;
            int wentDown = 0;


            //find stair
            while (maxSteps-- > 0)
            {
                if (dx >= Main.maxTilesX || dx < 0) break;
                if (dy >= Main.maxTilesY || dy < 0) break;

                while ((Main.tile[dx, dy].active() && (Main.tile[dx, dy].type == 41 || Main.tile[dx, dy].type == 43 || Main.tile[dx, dy].type == 44)))
                {
                    //Main.tile[dx, dy].active(true); if us debug dont forget 45 in exception in whilses
                    //Main.tile[dx, dy].type = 45;

                    dx += dir;
                    if (dx == Main.maxTilesX) break; if (dx == -1) break;
                }
                if (dx == Main.maxTilesX) break; if (dx == -1) break;

                if (stepsDone > 10 && Main.tile[dx, dy].active())
                {
                    goodD = true;
                    break;
                }
                wentDown = 0;
                if (dx > Main.maxTilesX - 2 || dx < 1) break;
                while (!Main.tile[dx, dy].active() || (Main.tile[dx, dy].active() && Main.tile[dx, dy].active() && (Main.tile[dx, dy].type != 41 && Main.tile[dx, dy].type != 43 && Main.tile[dx, dy].type != 44)) && (Main.tile[dx - dir, dy].active() &&
                    (Main.tile[dx - dir, dy].type == 41 || Main.tile[dx - dir, dy].type == 43 || Main.tile[dx - dir, dy].type == 44)))
                {
                    //Main.tile[dx, dy].active(true);
                    // Main.tile[dx, dy].type = 45;
                    dy += 1;
                    wentDown++;
                    if (dy == Main.maxTilesY) break;
                }
                if (dx == Main.maxTilesX - 1) break; if (dx == 0) break; if (dy == Main.maxTilesY) break;

                if (!(Main.tile[dx - dir, dy].active() &&
                    (Main.tile[dx, dy].type == 41 || Main.tile[dx, dy].type == 43 || Main.tile[dx, dy].type == 44)))
                {
                    //end of stairs reached
                    while (!Main.tile[dx, dy].active())
                    {
                        //Main.tile[dx, dy].active(true);
                        //Main.tile[dx, dy].type = 45;
                        dy += 1;
                        wentDown++;
                        if (dy == Main.maxTilesY)
                            break;
                    }
                    break;
                }

                stepsDone++;
            }

            wentDown--;

            if (maxSteps == 0 || dy == Main.maxTilesY || dx == Main.maxTilesX)
                return -1;//something strange might happen here

            return goodD ? 0 : wentDown;
        }



        private static int getTilesToSolid(int posX, int posY, int startYdist, bool printTile)
        {
            //start from top surface
            int dist = startYdist < 0 ? 0 : startYdist;




            //sb.Write(" type {0} ", Main.tile[posX, posY - dist].IsActive + " ");

            //52 = vines
            //62 = jungel vines
            //51 = cobweb
            //205 = crimson vines
            //191 == living wood, 78 wall, 192 leaf
            //32 = corruption thorns
            //352 = crimtane thorns
            //fffffffffff   in Terrarai maybe not solid, but they are in Tedit


            //here vines not solid but bad if you go up there?
            //if (tile.IsActive && World.TileProperties[Main.tile[posX, posY - dist].Type].IsSolid &&
            //    Main.tile[posX, posY - dist].Type != 51 && Main.tile[posX, posY - dist].Type != 52 &&
            //        Main.tile[posX, posY - dist].Type != 62)

            //liquid &&  Main.tile[posX, posY - dist].liquid != 255

            if (Main.tile[posX, posY - dist].type == TileID.LivingWood || Main.tile[posX, posY - dist].wall == WallID.LivingWood || Main.tile[posX, posY - dist].type == TileID.LeafBlock || Main.tile[posX, posY - dist].liquid == 255)
                return 0;

            bool up = false;



            while (CheckIfSolidAndNoTree(posX, posY - dist)) {

                up = true;

                if (printTile)
                {
                    Main.tile[posX, posY - dist].active(true);
                    Main.tile[posX, posY - dist].type = 47; //debug
                }
                dist++;
            }

            if (up == false)
                while (!Main.tile[posX, posY - dist].active() ||
                        (Main.tile[posX, posY - dist].active() &&
                        !Main.tileSolid[Main.tile[posX, posY - dist].type]) ||
                        (Main.tile[posX, posY - dist].active() &&
                        Main.tileSolid[Main.tile[posX, posY - dist].type] && (
                        Main.tile[posX, posY - dist].type == TileID.Cobweb ||
                        Main.tile[posX, posY - dist].type == TileID.Vines ||
                        Main.tile[posX, posY - dist].type == TileID.JungleVines ||
                        Main.tile[posX, posY - dist].type == TileID.CorruptThorns ||
                        Main.tile[posX, posY - dist].type == TileID.CrimtaneThorns ||
                        Main.tile[posX, posY - dist].type == TileID.CrimsonVines ||
                        Main.tile[posX, posY - dist].type == TileID.LivingWood ||
                        Main.tile[posX, posY - dist].type == TileID.LeafBlock
                        )
                        ))
                {
                    //if (Main.tile[posX, posY - dist].liquid == 255)
                    //    break;
                    if (Main.tile[posX, posY - dist].active() && dist <= 0 &&
                        (
                        Main.tile[posX, posY - dist].type == TileID.LivingWood ||
                        Main.tile[posX, posY - dist].type == TileID.LeafBlock ||
                        Main.tile[posX, posY - dist].wall == WallID.LivingWood))
                        break;

                    if (printTile && dist < 0)
                    {
                        Main.tile[posX, posY - dist].active(true);
                        Main.tile[posX, posY - dist].type = 48; //debug
                    }
                    dist--;
                }

            //sb.WriteLine("dist {0}", (dist > 0 ? dist - 1 : dist));
            return dist;
        }


        private static int findOceanToSurface(bool leftNotRight, ref int posX, ref int posY, ref int direct)
        {

            posX = 25;
            posY = 100;
            direct = 1;


            if (!leftNotRight)
            {
                posX = Main.maxTilesX - posX;
                direct = -1;
            }


            int SEARCH_TO = Main.maxTilesY / 2;

            //find liquid
            for (; posY < SEARCH_TO; posY++)
            {

                if (Main.tile[posX, posY].liquid == 255)
                {

                    break;
                }

            }

            if (posY == SEARCH_TO)
            {
                //no ocean
                return -1;
            }

            const int OCEAON_CHECK_SIZE = 10;


            bool noOcean = false;
            for (int tempY = posY; tempY < posY + OCEAON_CHECK_SIZE; tempY++)
            {
                if (Main.tile[posX, tempY].liquid != 255)
                {
                    if (Main.tile[posX, tempY].active())
                    {
                        noOcean = true;
                        break;
                    }
                    else
                        posY = tempY;
                }

            }

            if (noOcean)
            {

                return -1;
            }

            //at water surface

            for (int trans = 0; trans < Main.maxTilesX / 2; trans++)
            {
                posX += direct;
                if (Main.tile[posX, posY].liquid == 00)
                {

                    break;
                }
            }

            return posX;
        }


        //excluding living tree
        private static bool CheckIfSolidAndNoTree(int posX, int posY)
        {

            return (Main.tile[posX, posY].active() && Main.tileSolid[Main.tile[posX, posY].type] &&
                Main.tile[posX, posY].type != TileID.LivingWood &&
                Main.tile[posX, posY].type != TileID.LeafBlock &&
                Main.tile[posX, posY].type != TileID.Cobweb &&
                Main.tile[posX, posY].type != TileID.CorruptThorns &&
                Main.tile[posX, posY].type != TileID.CrimtaneThorns &&
                Main.tile[posX, posY].type != TileID.Vines &&
                Main.tile[posX, posY].type != TileID.JungleVines &&
                Main.tile[posX, posY].type != TileID.CrimsonVines) ||
                    (Main.tile[posX, posY].active() && !Main.tileSolid[Main.tile[posX, posY].type] &&
                        Main.tile[posX, posY].wall != WallID.LivingWood &&
                        Main.tile[posX, posY].wall != WallID.None
                    );



        }


        private static int computeBeachPenalty(bool leftNotRight)
        {
            int posX = 25;
            int posY = 100;
            int direct = 1;

            int noOcean = findOceanToSurface(leftNotRight, ref posX, ref posY, ref direct);

            if (noOcean == -1)
            {
                //sb.WriteLine("############ NO ocean found ");
                return -1;
            }

            //beach start
            //int bend = posX + 400;
            const float BEACH_SEARCH_RANGE = 400;
            const int BEACH_MAX_VAR = 4;
            const int BEACH_LOCAL_MAX_FAC = 2;
            const float BEACH_MAX_VAR_TOTAL_FACTOR = 10;
            const int CHECK_ABOVE_RANGE = 52; //40 before, but dungeon detection did not work there, min 50, too high? //TODO ignore cloud tiles

            //int bend = posX + (int)BEACH_SEARCH_RANGE;
            int bend = (int)BEACH_SEARCH_RANGE;
            float bscore = 0;
            int lastDist = 0;
            int surfaceY = posY;
            int maxSurfaceHeight = posY;
            int maxDist = 0;
            bool printTile = false;

            //for (int posX0 = posX; posX < bend; posX++)
            for (int trans = 0; trans < bend; trans++)
            {
                posX += direct;

                //check tile above
                int dist = getTilesToSolid(posX, surfaceY, CHECK_ABOVE_RANGE, printTile);
                //surfaceY += tileOnTop - CHECK_ABOVE_RANGE;
                //sb.WriteLine("tileOnTop {0}   {1}", posX, tileOnTop);

                //int dist = getTilesToSolid(world, posX, surfaceY, lastDist, 0, sb, true);

                //int dist = getTilesToSolid(world, posX, surfaceY, lastDist, sb);

                if (printTile) { Main.tile[posX, surfaceY].active(true); Main.tile[posX, surfaceY].type = 46; } //debug##########################}

                float fac = ((float)(BEACH_SEARCH_RANGE - trans)) / ((float)BEACH_SEARCH_RANGE);

                int distToLast = Math.Abs(lastDist - dist);
                if (dist < 0)
                    bscore += fac * Math.Max(-dist, distToLast);
                else if (distToLast > 2)
                    bscore += fac * (distToLast - 1) * (distToLast - 2) * 0.5f;

                //add bscore ######################################## if last dist very != dist


                //sb.WriteLine("bscoreX {0} {1} {2}", posX, dist, bscore);

                lastDist = dist;

                if (dist < 0)
                {
                    if (surfaceY < maxSurfaceHeight)
                    {
                        maxSurfaceHeight = surfaceY;
                        maxDist = 0;
                    }
                    else
                    {
                        maxDist++;
                    }

                    int allowed = (int)((((float)trans) * BEACH_MAX_VAR * BEACH_MAX_VAR_TOTAL_FACTOR) / ((float)BEACH_SEARCH_RANGE));
                    int allowedMax = (int)((((float)maxDist) * BEACH_MAX_VAR * BEACH_MAX_VAR_TOTAL_FACTOR * BEACH_LOCAL_MAX_FAC) / ((float)BEACH_SEARCH_RANGE));

                    if (surfaceY < posY + allowed && surfaceY < maxSurfaceHeight + BEACH_MAX_VAR + allowedMax) { surfaceY++; dist++; }
                }
                else if (dist > 0)
                {

                    int val = surfaceY - maxSurfaceHeight - dist;
                    maxDist = (int)(((float)BEACH_SEARCH_RANGE) * val / (BEACH_MAX_VAR * BEACH_MAX_VAR_TOTAL_FACTOR * BEACH_LOCAL_MAX_FAC));

                    //surfaceY -= dist;
                    surfaceY--;
                    dist = 0;
                }


            }



            return (int)bscore;
        }

        private static double sumUp(double howMany, double start, double fac)
        {
            //if (howMany <= 0) return 0;
            if (fac == 1) return start * howMany;

            return start * (Math.Pow(fac, howMany) - 1) / (fac - 1);

            /*
            double val = 0;
            for (int i = 0; i < howMany; i++)
            {
                val += start * Math.Pow(fac, i);
            }
            return val;
            
            */


        }

        static bool writeScoreToFile = false;
        private static void writeScore(string allScoreText, bool append = false)
        {
            if (writeScoreToFile)
            {
                using (System.IO.StreamWriter file =
                 new System.IO.StreamWriter(@".\lastScore.txt", append))
                {
                    file.WriteLine(allScoreText);
                }
            }
        }



        private static int computeScore(ScoreWorld scoreW)
        {


            Dictionary<string, int> hasOBjectOrParam = scoreW.hasOBjectOrParam;

            int mapScale = Main.maxTilesY / 600; // small 2, medium 3, large 4
            double score = 0;
            //bool showDebug = false;
            string allScoreText;

            string seed = (Main.ActiveWorldFileData.Seed.ToString()).PadLeft(10, '0');
            allScoreText = seed;
                      

            score += 100.0 * hasOBjectOrParam["Number of Pyramids"] * Math.Pow(1.49517, (double)hasOBjectOrParam["Number of Pyramids"] - 1.0);//100, 299, 670, 1337
            score += (hasOBjectOrParam["Pyramid Carpet"] > 0 && hasOBjectOrParam["Pyramid Bottle"] > 0) ? 150 : 0;
            score += (hasOBjectOrParam["Pyramid Mask"] > 0 && hasOBjectOrParam["Pyramid Carpet"] > 0 && hasOBjectOrParam["Pyramid Bottle"] > 0) ? 350 : 0;
            score += (hasOBjectOrParam["Many Pyramids"] > 0 && hasOBjectOrParam["Pyramid Carpet"] > 0 && hasOBjectOrParam["Pyramid Bottle"] > 0 &&
                hasOBjectOrParam["Pyramid Bottle"] >= hasOBjectOrParam["Pyramid Carpet"] &&
                hasOBjectOrParam["Pyramid Carpet"] >= hasOBjectOrParam["Pyramid Mask"] && hasOBjectOrParam["Pyramid Mask"] > 0 &&
                hasOBjectOrParam["Pyramid Bottle"] <= hasOBjectOrParam["Pyramid Carpet"] * 2
                ) ? 420 : 0;

            allScoreText += System.Environment.NewLine + "Score Pyramid number and CB " + (int)score;

            score -= hasOBjectOrParam["Pyramid Mask"] == 0 ? 25 : 0;
            score -= hasOBjectOrParam["Pyramid Carpet"] == 0 ? 60 : 0;
            score -= hasOBjectOrParam["Pyramid Bottle"] == 0 ? 60 : 0;

            allScoreText += System.Environment.NewLine + "Score Pyramid -missing " + (int)score;

            score += sumUp(hasOBjectOrParam["Pyramid Mask"], 25, 0.5);
            score += sumUp(hasOBjectOrParam["Pyramid Carpet"], 60, 0.7);
            score += sumUp(hasOBjectOrParam["Pyramid Bottle"], 60, 0.8);

            allScoreText += System.Environment.NewLine + "Score Pyramid amount boni " + (int)score;

            score += hasOBjectOrParam["Cloud Chest"] > 2 ? 100 : 0;//todo sumup bei larger map

            allScoreText += System.Environment.NewLine + "Score Cloud Chest number " + (int)score;

            score -= hasOBjectOrParam["Cloud Starfury"] == 0 ? 60 : 0;
            score -= hasOBjectOrParam["Cloud Horseshoe"] == 0 ? 40 : 0;
            score -= hasOBjectOrParam["Cloud Sky Mill"] == 0 ? 25 : 0;
            score -= hasOBjectOrParam["Cloud Ballon"] == 0 ? 30 : 0;

            allScoreText += System.Environment.NewLine + "Score Cloud items " + (int)score;

            score += sumUp(hasOBjectOrParam["Tree"], 25, 1.0);
            score += sumUp(hasOBjectOrParam["Tree Chest"], 50, 0.6);
            score += sumUp(hasOBjectOrParam["Tree Chest Loom"], 50, 0.2);

            allScoreText += System.Environment.NewLine + "Score Tree " + (int)score;

            score += hasOBjectOrParam["Enchanted Sword Shrine"] > 0 ? 50 : 0;
            score += sumUp(hasOBjectOrParam["Enchanted Sword"], 25, 1.1);
            score += sumUp(hasOBjectOrParam["Enchanted Sword Shrine"], 15, 1.1);
            score -= hasOBjectOrParam["Enchanted Sword"] == 0 ? 100 : 0;
            score -= hasOBjectOrParam["Enchanted Sword Shrine"] == 0 ? 20 : 0;

            allScoreText += System.Environment.NewLine + "Score ESS " + (int)score;




            score += hasOBjectOrParam["Water Bolt before Skeletron"] > 0 ? 70 : -10;
            score += sumUp(hasOBjectOrParam["Water Bolt before Skeletron"], 20, 1.4);
            score -= hasOBjectOrParam["Water Bolt"] == 0 ? -40 : 0;

            allScoreText += System.Environment.NewLine + "Score Water Bolt " + (int)score;

            score -= hasOBjectOrParam["Meteorite Bar unlocked"] == 0 ? 20 : -30;
            score += sumUp(hasOBjectOrParam["Meteorite Bar unlocked"], 20, 0.9);

            allScoreText += System.Environment.NewLine + "Score Meteorite " + (int)score;


            score -= hasOBjectOrParam["Lava Charm"] == 0 ? 50 : 0;
            score += sumUp(hasOBjectOrParam["Lava Charm"], 30, 0.3);

            score -= hasOBjectOrParam["Water Walking Boots"] == 0 ? 50 : 0;
            score += sumUp(hasOBjectOrParam["Water Walking Boots"], 30, 0.3);

            allScoreText += System.Environment.NewLine + "Score Lava Waders " + (int)score;




            score += hasOBjectOrParam["Aglet"] > 0 ? sumUp(hasOBjectOrParam["Aglet"], 25, 0.4) : -50;
            allScoreText += System.Environment.NewLine + "Score Aglet " + (int)score;

            score += hasOBjectOrParam["Magic Mirror"] > 0 ? sumUp(hasOBjectOrParam["Magic Mirror"], 30, 0.4) : (hasOBjectOrParam["Ice Mirror"] == 0 ? -250 : -100);//together with ice -500 if both 0
            score += hasOBjectOrParam["Band of Regeneration"] > 0 ? sumUp(hasOBjectOrParam["Band of Regeneration"], 30, 0.4) - 20 : -10;
            score += hasOBjectOrParam["Band of Regeneration"] < 4 ? -40 : 0;
            score += hasOBjectOrParam["Shoe Spikes"] > 0 ? sumUp(hasOBjectOrParam["Shoe Spikes"], 30, 0.4) - 20 : -50;
            score += hasOBjectOrParam["Hermes Boots"] > 0 ? sumUp(hasOBjectOrParam["Hermes Boots"], 20, 0.4) - 20 : (hasOBjectOrParam["Flurry Boots"] == 0 ? -50 : -5);
            score -= 140; //penalty because they are not rare
            allScoreText += System.Environment.NewLine + "Score Forest Chest items " + (int)score;








            score += hasOBjectOrParam["Ice Skates"] > 0 ? 50 + sumUp(hasOBjectOrParam["Ice Skates"], 35, 0.4) : -60;
            allScoreText += System.Environment.NewLine + "Score Ice Skates " + (int)score;

            score += hasOBjectOrParam["Blizzard in a Bottle"] > 0 ? sumUp(hasOBjectOrParam["Blizzard in a Bottle"], 30, 0.4) : (hasOBjectOrParam["Pyramid Bottle"] > 0 ? -5 : -30);
            score += hasOBjectOrParam["Ice Machine"] > 0 ? sumUp(hasOBjectOrParam["Ice Machine"], 15, 0.4) : -5;
            score += hasOBjectOrParam["Snowball Cannon"] > 0 ? sumUp(hasOBjectOrParam["Snowball Cannon"], 25, 0.4) : -25;
            score += hasOBjectOrParam["Ice Boomerang"] > 0 ? sumUp(hasOBjectOrParam["Ice Boomerang"], 10, 0.4) : -5;
            score += hasOBjectOrParam["Ice Blade"] > 0 ? sumUp(hasOBjectOrParam["Ice Blade"], 10, 0.4) : -5;
            score += hasOBjectOrParam["Ice Mirror"] > 0 ? sumUp(hasOBjectOrParam["Ice Mirror"], 20, 0.4) : (hasOBjectOrParam["Magic Mirror"] == 0 ? -250 : -5);
            score += hasOBjectOrParam["Flurry Boots"] > 0 ? sumUp(hasOBjectOrParam["Flurry Boots"], 20, 0.4) : (hasOBjectOrParam["Hermes Boots"] == 0 ? -50 : -5);
            score -= 50; //penalty because they are not that rare
            allScoreText += System.Environment.NewLine + "Score Snow Chest Items " + (int)score;



            score -= hasOBjectOrParam["Flower Boots"] == 0 ? 120 : 0;
            score += sumUp(hasOBjectOrParam["Flower Boots"], 80, 0.7);

            allScoreText += System.Environment.NewLine + "Score Flower " + (int)score;

            score -= hasOBjectOrParam["Fiberglass Fishing Pole"] == 0 ? 10 : 0;
            score += sumUp(hasOBjectOrParam["Fiberglass Fishing Pole"], 10, 0.7);

            allScoreText += System.Environment.NewLine + "Score FiberPole " + (int)score;

            score -= hasOBjectOrParam["Staff of Regrowth"] == 0 ? 100 : 0;
            score += sumUp(hasOBjectOrParam["Staff of Regrowth"], 50, 0.4);

            allScoreText += System.Environment.NewLine + "Score Staff " + (int)score;

            score -= hasOBjectOrParam["Anklet of the Wind"] == 0 ? 60 : 0;
            score += sumUp(hasOBjectOrParam["Anklet of the Wind"], 25, 0.4);

            allScoreText += System.Environment.NewLine + "Score Anklet " + (int)score;

            score -= hasOBjectOrParam["Feral Claws"] == 0 ? 30 : 0;
            score += sumUp(hasOBjectOrParam["Feral Claws"], 15, 0.4);

            allScoreText += System.Environment.NewLine + "Score Feral " + (int)score;

            score += hasOBjectOrParam["Seaweed Pet"] > 0 ? sumUp(hasOBjectOrParam["Seaweed Pet"], 30, 0.4) : -10;
            score += hasOBjectOrParam["Fish Pet"] > 0 ? sumUp(hasOBjectOrParam["Fish Pet"], 30, 0.4) : -10;
            allScoreText += System.Environment.NewLine + "Score Seaweed Fish Pet " + (int)score;

            score += hasOBjectOrParam["Living Mahogany Wand"] > 0 ? sumUp(hasOBjectOrParam["Living Mahogany Wand"], 25, 0.4) : -45;
            score += hasOBjectOrParam["Honey Dispenser"] > 0 ? sumUp(hasOBjectOrParam["Honey Dispenser"], 15, 0.4) : -45;
            score -= 60; //penalty because they are not rare
            allScoreText += System.Environment.NewLine + "Score Dispenser Mahogany Wand " + (int)score;

            score -= hasOBjectOrParam["High Hive"] == 0 ? 25 : 0;
            score += sumUp(hasOBjectOrParam["High Hive"], 23, 1.337);
            score -= hasOBjectOrParam["Bee Hives"] < mapScale*3 ? 10 : 0;
            score += hasOBjectOrParam["Bee Hives"]- (mapScale * 3);

            allScoreText += System.Environment.NewLine + "Score Hives " + (int)score;

            score -= hasOBjectOrParam["Evil Tiles for Jungle Grass"] > 5 ? 150 : 0;
            score -= hasOBjectOrParam["Evil Tiles for Jungle Grass"] > 3000 ? 150 : 0.05 * hasOBjectOrParam["Evil Tiles for Jungle Grass"];
            score -= hasOBjectOrParam["Evil Tiles for Mud"] > 250 * 50 ? 250 : 0.02 * hasOBjectOrParam["Evil Tiles for Mud"];

            allScoreText += System.Environment.NewLine + "Score Evil Jungle " + (int)score;

            score -= hasOBjectOrParam["Evil Tiles for Sand"] > 300 * 100 ? 300 : (hasOBjectOrParam["Evil Tiles for Sand"] > 4000 ? 0.01 * hasOBjectOrParam["Evil Tiles for Sand"] : 0);
            score -= hasOBjectOrParam["Evil Tiles for Ice"] > 100 * 200 ? 100 : 0.005 * hasOBjectOrParam["Evil Tiles for Ice"];
            score += hasOBjectOrParam["Ice surface more than half evil"] > 0 ? 50 : -100;

            allScoreText += System.Environment.NewLine + "Score Evil SandIce " + (int)score;

            score += hasOBjectOrParam["Evil only one side"] == 1 ? 100 : -50;
            allScoreText += System.Environment.NewLine + "Score Evil one side " + (int)score;



            

            score += hasOBjectOrParam["Near Enchanted Sword"] > 0 ? sumUp(hasOBjectOrParam["Near Enchanted Sword"] - hasOBjectOrParam["Very Near Enchanted Sword"], 250, 1.0) : 0;
            score += hasOBjectOrParam["Near Enchanted Sword near Pyramid"]+ hasOBjectOrParam["Near Enchanted Sword near Tree"] > hasOBjectOrParam["Near Enchanted Sword"] ?
                sumUp(hasOBjectOrParam["Near Enchanted Sword near Pyramid"] + hasOBjectOrParam["Near Enchanted Sword near Tree"] - hasOBjectOrParam["Near Enchanted Sword"], 150, 1.0) : 0;
            score += hasOBjectOrParam["Very Near Enchanted Sword"] > 0 ? (sumUp(hasOBjectOrParam["Very Near Enchanted Sword"], 820, 1.0)) + 3*Math.Max(330-hasOBjectOrParam["Pathlength to Enchanted Sword"],0) : 0 ;            
            score += hasOBjectOrParam["Enchanted Sword near Pyramid"] > 0 ? sumUp(hasOBjectOrParam["Enchanted Sword near Pyramid"] - hasOBjectOrParam["Near Enchanted Sword near Pyramid"], 50, 1.0) : 0;
            score += hasOBjectOrParam["Enchanted Sword near Tree"] > 0 ? sumUp(hasOBjectOrParam["Enchanted Sword near Tree"] - hasOBjectOrParam["Near Enchanted Sword near Tree"], 50, 1.0) : 0;

            allScoreText += System.Environment.NewLine + "Score Near ES " + (int)score;

            score += hasOBjectOrParam["Near Tree"] > 0 ? sumUp(hasOBjectOrParam["Near Tree"], 90, 1.0) : 0;
            score += hasOBjectOrParam["Near Tree Chest"] > 0 ? sumUp(hasOBjectOrParam["Near Tree Chest"], 75, 1.25) : 0;

            allScoreText += System.Environment.NewLine + "Score Near Tree " + (int)score;

            score += hasOBjectOrParam["Near Altar"] > 0 ? sumUp(hasOBjectOrParam["Near Altar"], 50, 0.2) : 0;

            allScoreText += System.Environment.NewLine + "Score Near Altar " + (int)score;

            double webCount = sumUp(hasOBjectOrParam["Near Spider Web count"], 0.09, 0.9999);
            score += hasOBjectOrParam["Near Spider Web count"] > 0 ? (webCount > 50 ? 50 : webCount) : -50;

            allScoreText += System.Environment.NewLine + "Score Near Spider Web " + (int)score;

            double musCount = sumUp(hasOBjectOrParam["Near Mushroom Biome count"], 0.09, 0.9999);
            score += hasOBjectOrParam["Near Mushroom Biome count"] > 0 ? (musCount > 50 ? 50 : musCount) : -25;
            musCount = sumUp(hasOBjectOrParam["Mushroom Biome above surface count"], 1.0, 0.999999);
            score += hasOBjectOrParam["Mushroom Biome above surface count"] > 0 ? (musCount > 100 ? 100 : musCount) : 0;
            

            allScoreText += System.Environment.NewLine + "Score Near Mushroom Biome count " + (int)score;

            score += hasOBjectOrParam["Near Chest"] > 0 ? sumUp(hasOBjectOrParam["Near Chest"], 5, 1.05) : -300;

            allScoreText += System.Environment.NewLine + "Score Near Chest " + (int)score;

            score += hasOBjectOrParam["Near Cloud"] > 0 ? 75 * hasOBjectOrParam["Near Cloud"] : 0;

            allScoreText += System.Environment.NewLine + "Score Near Cloud " + (int)score;



            score += hasOBjectOrParam["Dungeon Distance"] < 1200 ? 100 : 0;
            score += hasOBjectOrParam["Dungeon Distance"] > 1700 ? -50 : 0;

            allScoreText += System.Environment.NewLine + "Score DungDist " + (int)score;

            score += hasOBjectOrParam["Temple door distance"] < 600 ? 0.5 * (600 - hasOBjectOrParam["Temple door distance"]) : 0;//todo dist to open entrance not door
            score += hasOBjectOrParam["Temple door distance"] < 750 ? 100 : 0;
            score += hasOBjectOrParam["Temple door distance"] < 850 ? 40 : 0;
            score += hasOBjectOrParam["Temple door distance"] < 900 ? 10 : 0;
            score += hasOBjectOrParam["Temple door distance"] > 1350 ? -80 : 0;

            int dpf = (hasOBjectOrParam["Temple door distance"] * 2 * 100) / hasOBjectOrParam["Pathlength to Temple Door"];
            score += (dpf - 100)/2;            
            dpf = (int)(( Math.Sqrt((double)hasOBjectOrParam["Temple Tile horizontal distance"] * hasOBjectOrParam["Temple Tile horizontal distance"] +
                (double)hasOBjectOrParam["Temple Tile vertical distance"] * hasOBjectOrParam["Temple Tile vertical distance"]) * 100) / hasOBjectOrParam["Pathlength to Temple Tile"]);
            score += (dpf - 100) / 2;
            allScoreText += System.Environment.NewLine + "Score TempleDistPath " + (int)score;


            score += hasOBjectOrParam["Underground Distance to spawn"] < 50 ? 40 : 0;
            score += hasOBjectOrParam["Underground Distance to spawn"] > 120 ? -60 : 0;

            allScoreText += System.Environment.NewLine + "Score GroundDist " + (int)score;

            score += hasOBjectOrParam["Cavern Distance to spawn"] < 150 ? 40 : 0;
            score += hasOBjectOrParam["Cavern Distance to spawn"] > 250 ? -60 : 0;

            allScoreText += System.Environment.NewLine + "Score RockDist " + (int)score;

            score += hasOBjectOrParam["Hermes Flurry Boots Distance"] < 150 && hasOBjectOrParam["Pathlength to Boots"] < 450 ? 60 : 0;
            score -= 0.05 * hasOBjectOrParam["Hermes Flurry Boots Distance"];
            score -= 0.015 * hasOBjectOrParam["Pathlength to Boots"];

            allScoreText += System.Environment.NewLine + "Score BootsDistPath " + (int)score;


            //near objects
            double tval = 0.05 * (1000 - hasOBjectOrParam["Pathlength to Teleport Potion"]);
            score += tval > -5 ? tval : -5;
            allScoreText += System.Environment.NewLine + "Score Path Teleport Potion " + (int)score;

            tval = 0.05 * (1000 - hasOBjectOrParam["Pathlength to Gravitation Potion"]);
            score += tval > -5 ? tval : -5;
            allScoreText += System.Environment.NewLine + "Score Path Gravitation Potion " + (int)score;

            //near objects
            tval = 0.1 * (500 - hasOBjectOrParam["Pathlength to Iron/Lead Bar"]);
            tval += hasOBjectOrParam["Pathlength to 10 Iron/Lead Bar Chest"] < 500 ? 0.1 * (500 - hasOBjectOrParam["Pathlength to 10 Iron/Lead Bar Chest"]) : 0;
            tval += hasOBjectOrParam["Pathlength to 12 Iron/Lead Bar Chest"] < 500 ? 0.05 * (500 - hasOBjectOrParam["Pathlength to 12 Iron/Lead Bar Chest"]) : 0;
            score += tval > -10 ? tval : -10;
            allScoreText += System.Environment.NewLine + "Score Path Iron/Lead Bar " + (int)score;

            tval = 0.1 * (800 - hasOBjectOrParam["Pathlength to Gold/Platinum Bar"]);
            score += tval > -7 ? tval : -7;
            allScoreText += System.Environment.NewLine + "Score Path Gold/Platinum Bar " + (int)score;

            tval = 0.05 * (5000 - hasOBjectOrParam["Pathlength to Meteorite Bar"]);
            score += tval > -5 ? tval : -5;
            allScoreText += System.Environment.NewLine + "Score Path Meteorite Bar " + (int)score;

            tval = 0.1 * (500 - hasOBjectOrParam["Pathlength to Bomb"]);
            score += tval > -3 ? tval : -3;
            allScoreText += System.Environment.NewLine + "Score Path Bomb " + (int)score;

            tval = 0.05*(1000 - hasOBjectOrParam["Pathlength to Dynamite"]);
            score += tval > -5 ? tval : -5;
            tval = 0.05*(1000 - hasOBjectOrParam["Pathlength to 2nd Dynamite"]);
            score += tval > -5 ? tval : -5;
            allScoreText += System.Environment.NewLine + "Score Path Dynamite " + (int)score;

            tval = 0.02 * (1000 - hasOBjectOrParam["Pathlength to Ruby"]);
            tval = 0.05 * (1000 - hasOBjectOrParam["Pathlength to Diamond"]);
            score += tval > -10 ? tval : -10;
            allScoreText += System.Environment.NewLine + "Score Path Ruby/Diamond " + (int)score;

            tval = 0.02 * (600 - hasOBjectOrParam["Pathlength to Jester's Arrow"]);
            score += tval > -3 ? tval : -3;
            allScoreText += System.Environment.NewLine + "Score Path Jester's Arrow " + (int)score;

            tval = 0.05 * (700 - hasOBjectOrParam["Pathlength to Suspicious Looking Eye"]);
            score += tval > -1 ? tval : -1;
            allScoreText += System.Environment.NewLine + "Score Path Looking Eye " + (int)score;

            tval = 0.03 * (1500 - hasOBjectOrParam["Pathlength to Bee Hive"]);
            score += tval > -15 ? tval : -15;
            allScoreText += System.Environment.NewLine + "Score Path Bee Hive " + (int)score;

            tval = 0.1 * (2000 - hasOBjectOrParam["Pathlength to free ShadowOrb/Heart"]);
            score += tval > 0 ? tval : 0;
            score += hasOBjectOrParam["Pathlength to free ShadowOrb/Heart"] < 2500 && hasOBjectOrParam["Free ShadowOrb/Heart"] > 0 ? 30 + sumUp(hasOBjectOrParam["Free ShadowOrb/Heart"], 30, 0.525) : 0;
            allScoreText += System.Environment.NewLine + "Score (Path) Exposed Orbs/Heart " + (int)score;





            //beach
            score -= hasOBjectOrParam["Beach penalty left"] < 0 ? 0 : hasOBjectOrParam["Beach penalty left"] > 5000 ? 100 : 0.02 * hasOBjectOrParam["Beach penalty left"];
            score -= hasOBjectOrParam["Beach penalty right"] < 0 ? 0 : hasOBjectOrParam["Beach penalty right"] > 5000 ? 100 : 0.02 * hasOBjectOrParam["Beach penalty right"];

            score += hasOBjectOrParam["Beach penalty left"] < 0 ? 0 : hasOBjectOrParam["Beach penalty left"] > 1000 ? 0 : 0.04 * (1000 - hasOBjectOrParam["Beach penalty left"]);
            score += hasOBjectOrParam["Beach penalty right"] < 0 ? 0 : hasOBjectOrParam["Beach penalty right"] > 1000 ? 0 : 0.04 * (1000 - hasOBjectOrParam["Beach penalty right"]);

            allScoreText += System.Environment.NewLine + "Score Beach " + (int)score;



            //dungeon pos            
            score += hasOBjectOrParam[OptionsDict.Phase2.dungeonGoodPos] > 0 ? 15 : -55;
            allScoreText += System.Environment.NewLine + "Score Dungeon Pos " + (int)score;

            float wallNum = hasOBjectOrParam["Green Dungeon Walls"] + hasOBjectOrParam["Blue Dungeon Walls"] + hasOBjectOrParam["Pink Dungeon Walls"];
            score += hasOBjectOrParam["All Dungeon Walls"] > 1337 ? 100.0 * hasOBjectOrParam["All Dungeon Walls"] / wallNum : -135;
            allScoreText += System.Environment.NewLine + "Score All Dungeon Walls " + (int)score;

            score += hasOBjectOrParam["All Dungeon Items"] > 0 ? 40 : -55;
            allScoreText += System.Environment.NewLine + "Score All Dungeon Items " + (int)score;



            score += hasOBjectOrParam["Flame Trap"] > 0 ? 15 + hasOBjectOrParam["Flame Trap"] * 5 : -42;
            allScoreText += System.Environment.NewLine + "Score Flame Traps " + (int)score;

            score += sumUp(hasOBjectOrParam["Number different Paintings"], 5, 1.1) * 0.05 - 15;
            score += hasOBjectOrParam["Number Paintings"] * (0.5f / mapScale) - 2;
            allScoreText += System.Environment.NewLine + "Score Paintings " + (int)score;


            score += hasOBjectOrParam["Different functional noncraf. Statues"] == 26 - (Main.expertMode ? 0 : 2) ? hasOBjectOrParam["Number functional noncraf. Statues"] * (0.25f / (mapScale - 1)) : -100;
            score += hasOBjectOrParam["Different noncraf. Statues"] == 56 ? hasOBjectOrParam["Number noncraf. Statues"] * (0.05f / (mapScale - 1)) : -50;
            allScoreText += System.Environment.NewLine + "Score Statues " + (int)score;

            score += hasOBjectOrParam["Alchemy Table"] > 0 ? sumUp(hasOBjectOrParam["Alchemy Table"], 30, 0.72) - 30 : -150;
            allScoreText += System.Environment.NewLine + "Score Alchemy Table " + (int)score;

            score += hasOBjectOrParam["Sharpening Station"] > 0 ? sumUp(hasOBjectOrParam["Sharpening Station"], 23, 0.65) - 23 : -150;
            allScoreText += System.Environment.NewLine + "Score Sharpening Station " + (int)score;


            


            //other rare stuff
            if (hasOBjectOrParam["No Ocean"] > 0)
            {
                score += hasOBjectOrParam["No Ocean"] > 0 ? 420 * hasOBjectOrParam["No Ocean"] / 2 : 0;
                allScoreText += System.Environment.NewLine + "Score no Ocean " + (int)score;
            }

            if (hasOBjectOrParam["Spawn in Sky"] > 0)
            {
                score += 1337;
                allScoreText += System.Environment.NewLine + "Score Spawn in Sky " + (int)score;
            }

            if (hasOBjectOrParam[OptionsDict.Phase3.openTemple] > 0)
            {
                score += 420;
                allScoreText += System.Environment.NewLine + "Score Open Temple " + (int)score;
            }

            if (hasOBjectOrParam["Snow biome surface overlap mid"] > 5 && hasOBjectOrParam["Jungle biome surface overlap mid"] > 5)
            {
                score += 1337;
                allScoreText += System.Environment.NewLine + "Score Spawn in Frosty Jungle biome " + (int)score;
            }
            else
            if (hasOBjectOrParam["Spawn in Jungle biome"] > 0)
            {
                score += 1337;
                allScoreText += System.Environment.NewLine + "Score Spawn in Jungle biome " + (int)score;
            } else
            if (hasOBjectOrParam["Spawn in Snow biome"] > 0)
            {
                score += 420;
                allScoreText += System.Environment.NewLine + "Score Spawn in Snow biome " + (int)score;
            }
            if (hasOBjectOrParam["Spawn in Marble or Granite biome"] > 0)
            {
                score += 840;
                allScoreText += System.Environment.NewLine + "Score Spawn Marble or Granite biome " + (int)score;
            }


            score += hasOBjectOrParam["Shadow Chest item in normal chest"] > 0 ? 420 * hasOBjectOrParam["Shadow Chest item in normal chest"] : 0;
            if (hasOBjectOrParam["Shadow Chest item in normal chest"] > 0) allScoreText += System.Environment.NewLine + "Score ShadowChestItemInNormal " + (int)score;

            score += hasOBjectOrParam["Biome Item in normal Chest"] > 0 ? 1337 * hasOBjectOrParam["Biome Item in normal Chest"] : 0; //should be patched
            if (hasOBjectOrParam["Biome Item in normal Chest"] > 0) allScoreText += System.Environment.NewLine + "Score BiomeNormalChest " + (int)score;

            score += hasOBjectOrParam["Dungeon in Snow Biome"] > 0 ? 420 : 0;
            score += hasOBjectOrParam["Dungeon far above surface"] > 0 ? 42 : 0;
            score += hasOBjectOrParam["Dungeon below ground"] > 0 ? 420 : 0;
            score += hasOBjectOrParam["Dungeon below ground tree"] > 0 ? 420 : 0;
            if (hasOBjectOrParam[OptionsDict.Phase2.dungeonStrangePos] > 0) allScoreText += System.Environment.NewLine + "Score Dungeon has strange Pos " + (int)score;

            score += hasOBjectOrParam["Floating island cabin in Dungeon"] > 0 ? 420 : 0;
            if (hasOBjectOrParam["Floating island cabin in Dungeon"] > 0) allScoreText += System.Environment.NewLine + "Score Floating cabin in Dungeon " + (int)score;

            score += hasOBjectOrParam["Detonator at surface"] > 0 ? 420* hasOBjectOrParam["Detonator at surface"] : 0;
            if (hasOBjectOrParam["Detonator at surface"] > 0) allScoreText += System.Environment.NewLine + "Score Detonator at surface " + (int)score;

            score += hasOBjectOrParam[OptionsDict.Phase3.greenPyramid] > 0 ? 420* hasOBjectOrParam[OptionsDict.Phase3.greenPyramid] : 0;
            if (hasOBjectOrParam[OptionsDict.Phase3.greenPyramid] > 0) allScoreText += System.Environment.NewLine + "Score Green Pyramid " + (int)score;

            score += hasOBjectOrParam[OptionsDict.Phase3.frozenTemple] > 0 ? 420 * hasOBjectOrParam[OptionsDict.Phase3.frozenTemple] : 0;
            if (hasOBjectOrParam[OptionsDict.Phase3.frozenTemple] > 0) allScoreText += System.Environment.NewLine + "Score Frozen Temple " + (int)score;

            score += hasOBjectOrParam[OptionsDict.Phase3.lonelyJungleTree] > 0 ? 420: 0;
            if (hasOBjectOrParam[OptionsDict.Phase3.lonelyJungleTree] > 0) allScoreText += System.Environment.NewLine + "Score Lonely jungle tree " + (int)score;
            
            score += hasOBjectOrParam["Minecart Track close to spawn"] > 0 ? 420: 0;
            if (hasOBjectOrParam["Minecart Track close to spawn"] > 0) allScoreText += System.Environment.NewLine + "Score Minecart spawn " + (int)score;

            score += hasOBjectOrParam["ExplosiveDetonator close to spawn"] > 0 ? 1337: 0;
            if (hasOBjectOrParam["ExplosiveDetonator close to spawn"] > 0) allScoreText += System.Environment.NewLine + "Score ExplosiveDetonator spawn " + (int)score;
            


            score += hasOBjectOrParam["Pre Skeletron Dungeon Chest Risky"] > 0 ? 100 * hasOBjectOrParam["Pre Skeletron Dungeon Chest Risky"] : 0;
            score += hasOBjectOrParam["Pre Skeletron Dungeon Chest Grab"] > 0 ? 200 * hasOBjectOrParam["Pre Skeletron Dungeon Chest Grab"] : 0;
            score += hasOBjectOrParam["Pre Skeletron Muramasa good positon"] > 0 ? 50 * hasOBjectOrParam["Pre Skeletron Muramasa good positon"] : 0;
            if (hasOBjectOrParam["Pre Skeletron Dungeon Chest Grab"] > 0 || hasOBjectOrParam["Pre Skeletron Dungeon Chest Risky"] > 0)
                allScoreText += System.Environment.NewLine + "Score Pre Skeletron Dungeon Chest " + (int)score;

            if (hasOBjectOrParam["Chest duplication Glitch"] > 0)
            {
                score += hasOBjectOrParam["Chest duplication Glitch"] > 0 ? 80 * hasOBjectOrParam["Chest duplication Glitch"] : 0;
                score += hasOBjectOrParam["Pathlength to Chest duplication Glitch"] < 1000 ? (1000 - hasOBjectOrParam["Pathlength to Chest duplication Glitch"]) / 20 : 0;
                allScoreText += System.Environment.NewLine + "Score Chest duplication Glitch " + (int)score;
            }
            if (hasOBjectOrParam["Pot duplication Glitch"] > 0)
            {
                score += hasOBjectOrParam["Pot duplication Glitch"] > 0 ? 10 * hasOBjectOrParam["Pot duplication Glitch"] : 0;
                score += hasOBjectOrParam["Pot duplication Glitch Single"] > 0 ? 80 * hasOBjectOrParam["Pot duplication Glitch Single"] : 0;
                score += hasOBjectOrParam["Pot duplication Glitch Single Cavern"] > 0 ? 90 * hasOBjectOrParam["Pot duplication Glitch Single Cavern"] : 0;
                score += hasOBjectOrParam["Pathlength to Pot dupl. Glitch"] < 1000 ? (1000-hasOBjectOrParam["Pathlength to Pot dupl. Glitch"])/20 : 0;                
                score += hasOBjectOrParam["Pathlength to Pot dupl. Glitch Single"] < 1000 ? (1000-hasOBjectOrParam["Pathlength to Pot dupl. Glitch Single"])/20 : 0;
                
                allScoreText += System.Environment.NewLine + "Score Pot duplication Glitch " + (int)score;
            }
            if (hasOBjectOrParam["Life Crystal duplication Glitch"] > 0)
            {
                score += hasOBjectOrParam["Life Crystal duplication Glitch"] > 0 ? 70 * hasOBjectOrParam["Life Crystal duplication Glitch"] : 0;                
                score += hasOBjectOrParam["Life Crystal duplication Glitch Single"] > 0 ? 80 * hasOBjectOrParam["Life Crystal duplication Glitch Single"] : 0;
                score += hasOBjectOrParam["Pathlength to Life Crystal dupl. Glitch"] < 1000 ? (1000 - hasOBjectOrParam["Pathlength to Life Crystal dupl. Glitch"]) / 10 : 0;
                score += hasOBjectOrParam["Pathlength to Life Crystal dupl. Glitch Single"] < 1000 ? (1000 - hasOBjectOrParam["Pathlength to Life Crystal dupl. Glitch Single"]) / 5 : 0;
                allScoreText += System.Environment.NewLine + "Score Life Crystal duplication Glitch " + (int)score;
            }
            if (hasOBjectOrParam["Enchanted Sword duplication Glitch"] > 0)
            {
                score += hasOBjectOrParam["Enchanted Sword duplication Glitch"] > 0 ? 250 * hasOBjectOrParam["Enchanted Sword duplication Glitch"] : 0;
                allScoreText += System.Environment.NewLine + "Score Enchanted Sword duplication Glitch " + (int)score;
            }
            if (hasOBjectOrParam["Floating duplication Glitch structure"] > 0)
            {
                score += hasOBjectOrParam["Floating duplication Glitch structure"] > 0 ? 100 * hasOBjectOrParam["Floating duplication Glitch structure"] : 0;
                allScoreText += System.Environment.NewLine + "Score Floating duplication Glitch structure " + (int)score;
            }

            if (hasOBjectOrParam[OptionsDict.Phase3.allChestItemsNoCraftFish] > 0)
            {
                score += 1337 * (5-mapScale);
                allScoreText += System.Environment.NewLine + "Score all chest items you can't craft or fish! " + (int)score;
            }

            if (hasOBjectOrParam["All Paintings"] > 0)
            {
                score += 1337 * (6 - mapScale);
                allScoreText += System.Environment.NewLine + "Score all paintings! " + (int)score;
            }


            allScoreText += System.Environment.NewLine + "Bonus Score:";
            float multiAppearance = 0;
            if (hasOBjectOrParam["Pyramid Mask"] > 0) multiAppearance += 3;
            if (hasOBjectOrParam["Tree Chest"] > 0) multiAppearance += 11;
            if (hasOBjectOrParam["Tree Chest Loom"] > 0) multiAppearance += 22;
            if (hasOBjectOrParam["Living Mahogany Wand"] > 0) multiAppearance += 4;
            if (hasOBjectOrParam["Honey Dispenser"] > 0) multiAppearance += 2;
            if (hasOBjectOrParam["Ice Machine"] > 0) multiAppearance += 7;
            if (hasOBjectOrParam["Bone Welder"] > 0) multiAppearance += 5;
            if (hasOBjectOrParam["Cloud Sky Mill"] > 0) multiAppearance += 10;
            if (hasOBjectOrParam["Fish Pet"] > 0) multiAppearance += 18;
            if (hasOBjectOrParam["Seaweed Pet"] > 0) multiAppearance += 18;
            allScoreText += System.Environment.NewLine + "Appearance chest items (" + multiAppearance + "/100)";//extra point if > 54

            float multiUsefullRare = 0;
            if (hasOBjectOrParam["Pyramid Carpet"] > 0) multiUsefullRare += 1;
            if (hasOBjectOrParam["Pyramid Bottle"] > 0) multiUsefullRare += 1;
            if (hasOBjectOrParam["Flower Boots"] > 0) multiUsefullRare += 1;
            if (hasOBjectOrParam["Ice Skates"] > 0) multiUsefullRare += 1;
            if (hasOBjectOrParam["Lava Charm"] > 0 && hasOBjectOrParam["Water Walking Boots"] > 0) multiUsefullRare += 1;
            allScoreText += System.Environment.NewLine + "Rare unmakeable chest items (" + (int)(multiUsefullRare / 5.0 * 100) + "/100)";

            float multiOtherRareItems = 0;
            if (hasOBjectOrParam["Blizzard in a Bottle"] > 0) multiOtherRareItems += 17;
            if (hasOBjectOrParam["Snowball Cannon"] > 0) multiOtherRareItems += 10;
            if (hasOBjectOrParam["Shoe Spikes"] > 0) multiOtherRareItems += 10;
            if (hasOBjectOrParam["Band of Regeneration"] > 0) multiOtherRareItems += 10;
            if (hasOBjectOrParam["Magic Mirror"] > 0) multiOtherRareItems += 10; else multiOtherRareItems -= 10;
            if (hasOBjectOrParam["Ice Mirror"] > 0) multiOtherRareItems += 4;
            if (hasOBjectOrParam["Ice Mirror"] == 0 && hasOBjectOrParam["Magic Mirror"] == 0) multiOtherRareItems -= 50; //should not happen
            if (hasOBjectOrParam["Water Bolt"] > 0) multiOtherRareItems += 10;
            if (hasOBjectOrParam["Ice Blade"] > 0) multiOtherRareItems += 5;
            if (hasOBjectOrParam["Ice Boomerang"] > 0) multiOtherRareItems += 5;
            if (hasOBjectOrParam["Flurry Boots"] > 0) multiOtherRareItems += 4;
            if (hasOBjectOrParam["Hermes Boots"] > 0) multiOtherRareItems += 10; else multiOtherRareItems -= 10;
            if (hasOBjectOrParam["Valor"] > 0) multiOtherRareItems += 5;
            allScoreText += System.Environment.NewLine + "Other unmakeable chest items (" + multiOtherRareItems + "/100)"; //>91


            float multiNiceToHave = 0;
            if (hasOBjectOrParam["Staff of Regrowth"] > 0) multiNiceToHave += 41;
            if (hasOBjectOrParam["Aglet"] > 0) multiNiceToHave += 24;
            if (hasOBjectOrParam["Anklet of the Wind"] > 0) multiNiceToHave += 15;
            if (hasOBjectOrParam["Feral Claws"] > 0) multiNiceToHave += 6;
            if (hasOBjectOrParam["Muramasa"] > 0) multiNiceToHave += 4;
            if (hasOBjectOrParam["Cobalt Shield"] > 0) multiNiceToHave += 4;
            if (hasOBjectOrParam["Cloud Starfury"] > 0 && hasOBjectOrParam["Cloud Ballon"] > 0 && hasOBjectOrParam["Cloud Horseshoe"] > 0) multiNiceToHave += 6; // >80
            allScoreText += System.Environment.NewLine + "Makeable nice to have chest items (" + multiNiceToHave + "/100)";

            float multiWorldPos = 0;     //positive                                           
            if (hasOBjectOrParam["Evil only one side"] == 0) multiWorldPos++;
            if (hasOBjectOrParam["Water Bolt before Skeletron"] == 0) multiWorldPos++;
            if (hasOBjectOrParam["Pyramid Carpet"] > 0 && hasOBjectOrParam["Pyramid Bottle"] > 0 && hasOBjectOrParam["Pyramid Mask"] > 0 && hasOBjectOrParam["Tree Chest Loom"] > 0) multiWorldPos++;
            if (hasOBjectOrParam["Enchanted Sword"] > 0) multiWorldPos += 1;
            allScoreText += System.Environment.NewLine + "Positive World properties (" + (int)(multiWorldPos / 4.0 * 100) + "/100)";


            float multiWorldNeg = 0;     //negative                   
            if (hasOBjectOrParam["Beach penalty max"] > 4200) multiWorldNeg++;
            if (hasOBjectOrParam["Cloud Chest"] < 3) multiWorldNeg++;
            if (hasOBjectOrParam["Enchanted Sword Shrine"] == 0 && hasOBjectOrParam["Enchanted Sword"] < 2) multiWorldNeg++;//merged too one to reduce vanilla bug effect                  
            if (hasOBjectOrParam["Evil Tiles for Jungle Grass"] > 0) multiWorldNeg += 3;
            if (hasOBjectOrParam["Has evil Ocean"] > 0) multiWorldNeg += 2;
            if (hasOBjectOrParam["Ice surface more than half evil"] == 1) multiWorldNeg += 3;
            if (hasOBjectOrParam["Alchemy Table"] < 2) multiWorldNeg++;
            if (hasOBjectOrParam["Alchemy Table"] < 1) multiWorldNeg++;
            if (hasOBjectOrParam["Sharpening Station"] < 2) multiWorldNeg++;
            if (hasOBjectOrParam["Sharpening Station"] < 1) multiWorldNeg++;
            if (hasOBjectOrParam["Different functional noncraf. Statues"] < 26 - (Main.expertMode ? 0 : 2) ) multiWorldNeg++;
            if (hasOBjectOrParam["Jungle cavern not blocked by structure"] == 0) multiWorldNeg++;
            if (hasOBjectOrParam["Pathlength to 40% cavern entrance"] > 350 || hasOBjectOrParam["Tiles to mine for 40% cavern"] > 20) multiWorldNeg++;
            multiWorldNeg = (float)Math.Ceiling(0.5 * multiWorldNeg);
            allScoreText += System.Environment.NewLine + "Negative World properties (" + (int)(multiWorldNeg / 9.0 * 100) + "/100)";


            allScoreText += System.Environment.NewLine + "Bonus Score: ";
            //double boni = (1.0-(multiWorld / 12.0)) * (1.0+3.0*Math.Pow((multiNiceToHave / 7.0), 2.0)) * (1.0 + Math.Pow((multiOtherRareItems / 11.0), 2.0)) * (1.0 + 7.0*Math.Pow((multiUsefullRare / 6.0), 2.0)) * (1.0 + 2*Math.Pow((multiAppearance / 10.0), 2.0)) /64.0/3.0 *1337.0;

            allScoreText += System.Environment.NewLine + "Boni appearance chest: (" + (multiAppearance > 54 ? 1 : 0) + "/1)";
            double bonimult = (multiAppearance > 54 ? 1 : 0);

            allScoreText += System.Environment.NewLine + "Boni rare unmakeable: (" + ((int)multiUsefullRare) + "/5)";
            bonimult += multiUsefullRare;

            allScoreText += System.Environment.NewLine + "Boni other unmakeable: (" + (multiOtherRareItems > 91 ? 1 : 0) + "/1)";
            bonimult += (multiOtherRareItems > 91 ? 1 : 0);

            allScoreText += System.Environment.NewLine + "Boni makeable nice to have: (" + (multiNiceToHave > 80 ? 1 : 0) + "/1)";
            bonimult += (multiNiceToHave > 80 ? 1 : 0);

            allScoreText += System.Environment.NewLine + "Boni nice world: (" + (int)multiWorldPos + "/4)";
            bonimult += multiWorldPos;

            allScoreText += System.Environment.NewLine + "-Boni world not nice: (" + (int)multiWorldNeg + "/9)";
            bonimult -= multiWorldNeg;
            allScoreText += System.Environment.NewLine + "Boni total: (" + bonimult + "/12)";

            //double reduceMiss = (scoreW.missingCount * 10);
            double bonScor = sumUp(bonimult, 13.37, 1.337);
            allScoreText += System.Environment.NewLine + "Bonus score: " + (int)bonScor;
            score += bonScor;


            string quickBulbs = "";
            if (scoreW.itemLocation.ContainsKey(ItemID.PlanteraTrophy) || scoreW.itemLocation.ContainsKey(ItemID.PlanteraMask))
                quickBulbs = System.Environment.NewLine +System.Environment.NewLine + System.Environment.NewLine + "Quick Plantera Bulb:";
            if (scoreW.itemLocation.ContainsKey(ItemID.PlanteraTrophy))                          
                foreach (var bulb in scoreW.itemLocation[ItemID.PlanteraTrophy])
                {
                    int time = (bulb.Item2 / 10000) * 10000 - bulb.Item2;
                    int tims = (time % 100);
                    int timm = (time / 100);
                    quickBulbs += System.Environment.NewLine + "Plantera bulb might spawn at ( " + bulb.Item1 + " , " + -bulb.Item2 / 10000 + " ) after about " + (timm > 0 ? (timm + "min ") : "") + ( (tims + "sec") );
                }
            
            if (scoreW.itemLocation.ContainsKey(ItemID.PlanteraMask))
                foreach (var bulb in scoreW.itemLocation[ItemID.PlanteraMask])
                {
                    int time = (bulb.Item2 / 10000)*10000- bulb.Item2;
                    int tims = (time % 10)*10;
                    int timm = (time / 10);
                    quickBulbs += System.Environment.NewLine + "Plantera bulb might spawn at ( " + bulb.Item1 + " , " + -bulb.Item2/10000 + " ) after about " + (timm>0?(timm + "min "):"") + (timm<10 && tims>0 ? (tims + "sec"):"") ;
                    
                }
            if (scoreW.itemLocation.ContainsKey(ItemID.PlanteraTrophy) || scoreW.itemLocation.ContainsKey(ItemID.PlanteraMask))
                quickBulbs += System.Environment.NewLine + "(small values can be a little longer in game (+/- 5s), high values are shorter (90%+/-1min))" + System.Environment.NewLine;


            string missingItems = AcceptConditons.GetMissingUnmakeAbleItems(scoreW);
            allScoreText += System.Environment.NewLine + "Missing unmakeable items types: (" + scoreW.missingCount + "/" + (scoreW.missingCount + scoreW.missingCountNot) + ")";



            allScoreText += System.Environment.NewLine + "Seed " + seed + " total Score (beta): " + (int)score;




            //totod 4Pyramid min 1000 points ...............................
            string writeScoreStr = "";


            //writeScore(allScoreText);
            writeScoreStr = allScoreText;

            //writeScore(System.Environment.NewLine + "====", true);
            writeScoreStr += System.Environment.NewLine + "====";
            scoreW.hasOBjectOrParam["Score"] = (int)Math.Round(score);
            string itemlist = "";
            
            foreach (var item in hasOBjectOrParam)
            {
                int key;
                bool isDig = int.TryParse(item.Key, out key);

                if (!isDig)
                    itemlist += System.Environment.NewLine + item.Key + ": " + item.Value;
            }
            //all stuff
            //writeScore(itemlist, true);
            writeScoreStr += itemlist;

            allScoreText = itemlist + quickBulbs + Environment.NewLine + Environment.NewLine + missingItems + Environment.NewLine + Environment.NewLine + "Score(beta) for seed: " + allScoreText;

            //writeScore(System.Environment.NewLine + "==== all stuff ", true);
            writeScoreStr += System.Environment.NewLine + "==== all stuff ";
            itemlist = "";
            foreach (var item in hasOBjectOrParam)
            {
                int key;
                bool isDig = int.TryParse(item.Key, out key);

                if (isDig)
                {
                    //exclude (some?) moded content
                    if (ItemID.Search.ContainsId(key))
                    {
                        string itemName = ItemID.Search.GetName(key);
                        itemlist += System.Environment.NewLine + itemName + ": " + item.Value;
                    }
                }

            }
            //writeScore(itemlist, true);
            writeScoreStr += itemlist;
            writeScore(writeScoreStr);



            scoreW.scoreAsText = allScoreText;

            scoreW.score = (int)score;

            return (int)score;
        }

        private bool CheckOresMoon(generatorInfo genInfo)
        {
            bool allTrue = true;

            //writeDebugFile(genInfo.moonType + " " + genInfo.copperOrTin + " " + genInfo.ironOrLead + " " + genInfo.silverOrTung + " " + genInfo.goldOrPlat + " ");
            //writeDebugFile("lookfor: " + looking4moonType + " " + looking4copperTin + " " + looking4ironLead + " " + looking4silverTung + " " + looking4goldPlation + " ");

            allTrue = (looking4moonType.Equals("Random") || genInfo.moonType.Equals(looking4moonType)) &&
                      (looking4copperTin.Equals("Random") || genInfo.copperOrTin.Equals(looking4copperTin)) &&
                      (looking4ironLead.Equals("Random") || genInfo.ironOrLead.Equals(looking4ironLead)) &&
                      (looking4silverTung.Equals("Random") || genInfo.silverOrTung.Equals(looking4silverTung)) &&
                      (looking4goldPlation.Equals("Random") || genInfo.goldOrPlat.Equals(looking4goldPlation));

            //writeDebugFile("return " + allTrue);

            return allTrue;
        }

        public class WorldNameInfo
        {
            public string worldNameByUser;
            public string cseed;
            public string sizeEvilDiff;
            public string content;
            public string sscore;
            public string strares;

            public WorldNameInfo(string worldNameByUser, string cseed, string sizeEvilDiff, string content, string sscore, string strares)
                
            {
                this.worldNameByUser = worldNameByUser;
                this.cseed = cseed;
                this.sizeEvilDiff = sizeEvilDiff;
                this.content = content;
                this.sscore = sscore;
                this.strares = strares;
            }
        }

        private static WorldNameInfo createMapName(ScoreWorld score, bool valid, Configuration config, string worldNameByUser)
        {
            Dictionary<string, int> hasOBjectOrParam = score.hasOBjectOrParam; //Todo only ref

            string cseed = Main.ActiveWorldFileData.SeedText.PadLeft(10, '0');

            string sizeEvilDiff = Main.maxTilesX > 8000 ? "L" : Main.maxTilesX > 6000 ? "M" : "S";
            sizeEvilDiff += WorldGen.WorldGenParam_Evil == 0 ? "B" : WorldGen.WorldGenParam_Evil == 1 ? "R" : WorldGen.crimson ? "r" : "b";
            sizeEvilDiff += Main.expertMode ? "x" : "n";


            //name map like seed + number of pyramids in
            string content = score.pyramids.ToString();

            //DIGIT 2
            //check if it has Sandstorm Bottle (B), a Carpet (C), both (h) or more from each other, if not at least a blizzard (z)
            string nextDigit = "0";
            if (hasOBjectOrParam["Pyramid Mask"] > 0 && hasOBjectOrParam["Pyramid Bottle"] > 0 && hasOBjectOrParam["Pyramid Carpet"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Pyramid Carpet"] + hasOBjectOrParam["Pyramid Bottle"] > 2) ? "O" : "o";
            }
            else if (hasOBjectOrParam["Pyramid Carpet"] > 0 && hasOBjectOrParam["Pyramid Bottle"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Pyramid Carpet"] + hasOBjectOrParam["Pyramid Bottle"] > 2) ? "H" : "h";
            }
            else
            if (hasOBjectOrParam["Pyramid Bottle"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Pyramid Bottle"] > 1) ? "B" : "b";
            }
            else
            if (hasOBjectOrParam["Pyramid Carpet"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Pyramid Carpet"] > 1) ? "C" : "c";
            }
            else
            if (has(ref hasOBjectOrParam, ItemID.BlizzardinaBottle) > 0)
            {
                nextDigit = (has(ref hasOBjectOrParam, ItemID.BlizzardinaBottle) > 1) ? "Z" : "z";
            }
            content += nextDigit;


            //DIGIT 3
            //check if the map has lving trees (num), or even better with chests and Living Loom (l), only chest (t)
            nextDigit = hasOBjectOrParam["Tree"].ToString();
            if (hasOBjectOrParam["Tree Chest Loom"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Tree Chest Loom"] > 1) ? "L" : "l";
            }
            else
            if (hasOBjectOrParam["Tree Chest"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Tree Chest"] > 1) ? "T" : "t";
            }
            content += nextDigit;


            //DIGIT 4
            //check if map has flower boots (F) and staff of regrowth (R), or both for growing (G)
            nextDigit = "0";
            if (hasOBjectOrParam["Flower Boots"] > 0 && hasOBjectOrParam["Staff of Regrowth"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Flower Boots"] + hasOBjectOrParam["Staff of Regrowth"] > 2) ? "G" : "g";
            }
            else
            if (hasOBjectOrParam["Flower Boots"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Flower Boots"] > 1) ? "F" : "f";
            }
            else
            if (hasOBjectOrParam["Staff of Regrowth"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Staff of Regrowth"] > 1) ? "R" : "r";
            }
            content += nextDigit;

            //DIGIT 5
            //check how many clouds at the map, or even better wit Sky Mill (M with 3 clouds, m without, number of clouds else
            nextDigit = hasOBjectOrParam["Cloud Chest"].ToString();
            if (hasOBjectOrParam["Cloud Sky Mill"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Cloud Chest"] > 2) ? "M" : "m";
            }
            content += nextDigit;


            //DIGIT 6
            //check for Enchented Sword Shirnes (s) or without (E), big letter if more than one swords total, TODO ggf also nears ESS
            nextDigit = "0";
            if (hasOBjectOrParam["Enchanted Sword Shrine"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Enchanted Sword Shrine"] + hasOBjectOrParam["Enchanted Sword"] > 1) ? "S" : "s";
            }
            else
            if (hasOBjectOrParam["Enchanted Sword"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Enchanted Sword"] > 1) ? "E" : "e";
            }
            content += nextDigit;


            //DIGIT 7
            //check if lava charm (U) and water walking boots (K) exit, or both (V)
            nextDigit = "0";
            if (hasOBjectOrParam["Water Walking Boots"] > 0 && hasOBjectOrParam["Lava Charm"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Lava Charm"] + hasOBjectOrParam["Water Walking Boots"] > 2) ? "V" : "v";
            }
            else
            if (hasOBjectOrParam["Water Walking Boots"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Water Walking Boots"] > 1) ? "K" : "k";
            }
            else
            if (hasOBjectOrParam["Lava Charm"] > 0)
            {
                nextDigit = (hasOBjectOrParam["Lava Charm"] > 1) ? "U" : "u";
            }
            content += nextDigit;

            //DIGIT 8
            //check if any rare stuff happend, big letter if more then one
            //
            //W...Waterbolt before HM
            //I..Near Tree
            //Q..Near Cloud            
            //J..Fiberglas Fishing Pole
            //A..Near Altar
            //num..near chests

            nextDigit = "0";
            int total = hasOBjectOrParam["Water Bolt before Skeletron"] + hasOBjectOrParam["Near Tree"] + hasOBjectOrParam["Near Cloud"] + has(ref hasOBjectOrParam, ItemID.FiberglassFishingPole) + hasOBjectOrParam["Near Altar"] + (hasOBjectOrParam["Near Chest"] > 4 ? 1 : 0);
            if (hasOBjectOrParam["Water Bolt before Skeletron"] > 0)
            {
                nextDigit = (total > 1) ? "W" : "w";
            }
            else
            if (hasOBjectOrParam["Near Tree"] > 0)
            {
                nextDigit = (total > 1) ? "I" : "i";
            }
            else
            if (hasOBjectOrParam["Near Cloud"] > 0)
            {
                nextDigit = (total > 1) ? "Q" : "q";
            }
            else
            if (has(ref hasOBjectOrParam, ItemID.FiberglassFishingPole) > 0)
            {
                nextDigit = (total > 1) ? "J" : "j";
            }
            else
            if (hasOBjectOrParam["Near Altar"] > 0)
            {
                nextDigit = (total > 1) ? "A" : "a";
            }
            else
                nextDigit = hasOBjectOrParam["Near Chest"].ToString();
            content += nextDigit;


            //DIGIT 9
            //in general it shows the total map score divided by 333, x if over 9.50, X if over 10.500, - if negative score
            //but this score gets replaced if somee very rare stuff happen. E.g. ESS near spawn (N), spawn in sky (Y), dub. glitch chest (D), 4 or more Pyramids (P)(but less then 9k points)
            int allscore = ((int)(((float)(score.score)) / 333.0 + 0.5));
            nextDigit = allscore < 10 ? allscore.ToString() : (allscore < 11 ? "x" : "X");
            nextDigit = allscore < 0 ? "-" : nextDigit;
            if (hasOBjectOrParam["Chest duplication Glitch"] > 0 || hasOBjectOrParam["Pot duplication Glitch"] > 0 || hasOBjectOrParam["Life Crystal duplication Glitch"] > 0 || hasOBjectOrParam["Enchanted Sword duplication Glitch"] > 0) nextDigit = "D";
            else if (hasOBjectOrParam["Very Near Enchanted Sword"] > 0) nextDigit = "#";
            else if (hasOBjectOrParam["Spawn in Sky"] > 0) nextDigit = "Y";
            else if (hasOBjectOrParam[OptionsDict.Phase3.allChestItemsNoCraftFish] > 0) nextDigit = "@";
            else if (allscore < 10 && hasOBjectOrParam["Near Enchanted Sword"] > 0) nextDigit = "N";
            else if (allscore < 10 && hasOBjectOrParam["Enchanted Sword near Tree"] > 0 || hasOBjectOrParam["Enchanted Sword near Pyramid"] > 0) nextDigit = "+";
            else if (allscore < 10 && hasOBjectOrParam["Many Pyramids"] > 0) nextDigit = "P";
            content += nextDigit;

            string sscore = score.score.ToString().PadLeft(4, '0');
            //Main.worldName = Main.worldName + nextDigit + "_" + (sscore.PadLeft(4, '0')) + "_" + hasOBjectOrParam["No Ocean"] + "_" + hasOBjectOrParam["Beach penalty left"] + "_" + hasOBjectOrParam["Beach penalty right"];

            string strares = "";
            //debug
            if (hasOBjectOrParam["No Ocean"] > 0) strares += "_" + "NoOcean";
            if (hasOBjectOrParam["Spawn in Sky"] > 0) strares += "_" + "SpawnSky";
            if (hasOBjectOrParam["Spawn in Jungle biome"] > 0) strares += "_" + "SpawnJungle";
            if (hasOBjectOrParam["Spawn in Snow biome"] > 0) strares += "_" + "SpawnSnow";
            if (hasOBjectOrParam["Spawn in Marble or Granite biome"] > 0) strares += "_" + "SpawnMarbleGranite";

            if (hasOBjectOrParam[OptionsDict.Phase3.openTemple] > 0) strares += "_" + "OpenTemple";

            if (hasOBjectOrParam["Biome Item in normal Chest"] > 0) strares += "_" + "BiomeChestNormal";
            if (hasOBjectOrParam[OptionsDict.Phase2.dungeonStrangePos] > 0) strares += "_" + "DungeonStrange";
            if (hasOBjectOrParam["Floating island cabin in Dungeon"] > 0) strares += "_" + "FloatingCabinDungeon";
            if (hasOBjectOrParam["Pre Skeletron Dungeon Chest Risky"] > 0) strares += "_" + "DungeonPreSkelChestRisky";
            if (hasOBjectOrParam["Pre Skeletron Dungeon Chest Grab"] > 0) strares += "_" + "DungeonPreSkelChestGrab";

            if (hasOBjectOrParam["Chest duplication Glitch"] > 0) strares += "_" + "ChestDuplGlitch";
            if (hasOBjectOrParam["Pot duplication Glitch Single"] > 0 && hasOBjectOrParam["Pot duplication Glitch Single Cavern"]==0) strares += "_" + "PotDuplGlitchSi";            
            if (hasOBjectOrParam["Pot duplication Glitch Single Cavern"] > 0) strares += "_" + "PotDuplGlitchSiCav";            
            if (hasOBjectOrParam["Life Crystal duplication Glitch Single"] > 0) strares += "_" + "LifeCryDuplGlitchSi";
            if (hasOBjectOrParam["Life Crystal duplication Glitch"] > 0 && hasOBjectOrParam["Life Crystal duplication Glitch Single"]==0) strares += "_" + "LifeCryDuplGlitch";
            if (hasOBjectOrParam["Enchanted Sword duplication Glitch"] > 0) strares += "_" + "ESDuplGlitch";
            if (hasOBjectOrParam["Game breaker"] > 0) strares += "_" + "GameBreakerGlitch";
            if (hasOBjectOrParam["Near Enchanted Sword near Tree"] > 0) strares += "_" + "NearESnearTree";
            if (hasOBjectOrParam["Near Enchanted Sword near Pyramid"] > 0) strares += "_" + "NearESnearPyramid";
            if (hasOBjectOrParam["Near Enchanted Sword"] - hasOBjectOrParam["Very Near Enchanted Sword"] > 0 
                && hasOBjectOrParam["Near Enchanted Sword near Tree"] + hasOBjectOrParam["Near Enchanted Sword near Pyramid"] == 0) strares += "_" + "NearEnchantedSword";
            if (hasOBjectOrParam["Enchanted Sword near Tree"] - hasOBjectOrParam["Near Enchanted Sword near Tree"] > 0) strares += "_" + "ESnearTree";
            if (hasOBjectOrParam["Enchanted Sword near Pyramid"] - hasOBjectOrParam["Near Enchanted Sword near Pyramid"] > 0) strares += "_" + "ESnearPyramid";
            if (hasOBjectOrParam["Very Near Enchanted Sword"] > 0) strares += "_" + "VeryNearES";
            if (hasOBjectOrParam["Floating Island without chest"] > 0) strares += "_" + "CloudWithoutHouse";
            if (hasOBjectOrParam["Detonator at surface"] > 0) strares += "_" + "DetonatorSurface";
            if (hasOBjectOrParam[OptionsDict.Phase3.greenPyramid] > 0) strares += "_" + "GreenPyramid";
            if (hasOBjectOrParam[OptionsDict.Phase3.frozenTemple] > 0) strares += "_" + "FrozenTemple";
            if (hasOBjectOrParam[OptionsDict.Phase3.lonelyJungleTree] > 0) strares += "_" + "LonelyJungleTree";
            if (hasOBjectOrParam["Minecart Track close to spawn"] > 0) strares += "_" + "MinecartTrackSpawn";
            if (hasOBjectOrParam["ExplosiveDetonator close to spawn"] > 0) strares += "_" + "ExplosiveDetonatorSpawn";
            
            if (hasOBjectOrParam["Mushroom Biome above surface"] > 0) strares += "_" + "MushroomSurface";
            if (hasOBjectOrParam["Shadow Chest item in normal chest"] > 0) strares += "_" + "ShadowChestItemNormal";
            if (hasOBjectOrParam["All Paintings"] > 0) strares += "_" + "AllPaintings";
            if (hasOBjectOrParam[OptionsDict.Phase3.allChestItemsNoCraftFish] > 0) strares += "_" + "AllChestItems";
            if (hasOBjectOrParam["Number of Pyramids"] > Main.maxTilesY / 600 + 1) strares += "_" + hasOBjectOrParam["Number of Pyramids"] + "Pyramids";


            // strares sscore content cseed Main.worldName
            string worldName = "";

            foreach (var ci in config.configList)
            {

                //if (ci.phase > 0)
                //    break;
                //writeDebugFile(ci.name);

                if (ci.phase == 0 && config.opdict[OptionsDict.GeneralOptions.naming].Contains(ci.name))
                {
                    if (ci.name.Equals("World name"))
                        worldName += "_" + worldNameByUser;
                    else if (ci.name.Equals("Seed"))
                        worldName += "_" + cseed;
                    else if (ci.name.Equals("Size, evil type, difficulty"))
                        worldName += "_" + sizeEvilDiff;
                    else if (ci.name.Equals("Content in short"))
                        worldName += "_" + content;
                    else if (ci.name.Equals("Fantasy score"))
                        worldName += "_" + sscore;
                    else if (ci.name.Equals("Rares"))
                        worldName += "_" + strares;
                }
            }
            if (worldName.Length == 0)
                worldName = cseed + "_" + sizeEvilDiff + "_" + content + "_" + sscore + "_" + strares;
            else
                worldName = worldName.Substring(1, worldName.Length - 1);

            Main.worldName = worldName + (valid ? "" : "_unsure");

            Main.ActiveWorldFileData = WorldFile.CreateMetadata(Main.worldName, false, Main.expertMode);
            Main.ActiveWorldFileData.SetSeed(cseed);

            return new WorldNameInfo(worldNameByUser, cseed, sizeEvilDiff, content, sscore, strares);
        }


        private class AcceptConditons
        {
            public int chanceMaxPyramids;
            public int takeOnlyPyramids;

            // public bool takeAll;
            public bool omitRareAll;
            public List<string> omitRare;

            public int rares;
            public string rareText = "";
            public int points;


            public AcceptConditons()
            {
                chanceMaxPyramids = 2;
                takeOnlyPyramids = 0;
                //takeAll = false;
                omitRareAll = false;
                omitRare = new List<string>();
            }


            public int checkRares(ScoreWorld score, int step, List<string> omitRare)
            {
                rares = 0;
                Dictionary<string, int> hasOBjectOrParam = score.hasOBjectOrParam;
                rareText = "";
                Func<string, int> checkAdd = (string name) =>
                {
                    if (hasOBjectOrParam[name] > 0)
                        rareText += name + Environment.NewLine;

                    if (omitRare.Contains("Omit " + name))
                        return 0;
                    else
                        return hasOBjectOrParam[name];

                };

                if (step >= 2)
                {
                    if (!omitRare.Contains(OptionsDict.GeneralOptions.omitBadRare) && !omitRare.Contains(OptionsDict.GeneralOptions.omitBaCRare))
                    {
                        rares += checkAdd("No Ocean");
                    }
                    if (!omitRare.Contains(OptionsDict.GeneralOptions.omitCommonRare) && !omitRare.Contains(OptionsDict.GeneralOptions.omitBaCRare))
                    {
                        rares += checkAdd("Chest duplication Glitch");  //<--that might not find all int that step                    
                        
                        rares += checkAdd("Pre Skeletron Dungeon Chest Grab");
                        rares += checkAdd("Pre Skeletron Dungeon Chest Risky");                        

                    }
                    else
                    {
                        if (hasOBjectOrParam["Pre Skeletron Muramasa good positon"] > 0)
                        {
                            rares += checkAdd("Pre Skeletron Dungeon Chest Risky");
                            rares += checkAdd("Pre Skeletron Dungeon Chest Grab");
                        }
                    }
                    
                    
                    rares += checkAdd("Biome Item in normal Chest"); //<--that might not find all int that step
                    

                    if (!omitRare.Contains("Omit " + OptionsDict.Phase2.dungeonStrangePos))
                    {
                        if (!omitRare.Contains(OptionsDict.GeneralOptions.omitCommonRare) && !omitRare.Contains(OptionsDict.GeneralOptions.omitBaCRare))
                            rares += checkAdd("Dungeon in Snow Biome");
                        if (!omitRare.Contains(OptionsDict.GeneralOptions.omitBadRare) && !omitRare.Contains(OptionsDict.GeneralOptions.omitBaCRare))
                            rares += checkAdd("Dungeon far above surface");
                        if ( (!omitRare.Contains(OptionsDict.GeneralOptions.omitBadRare) && !omitRare.Contains(OptionsDict.GeneralOptions.omitBaCRare)) || hasOBjectOrParam["Dungeon below ground tree"]>0 ) 
                            rares += checkAdd("Dungeon below ground");
                    }

                    int pyramids = has(ref hasOBjectOrParam, ItemID.SandstorminaBottle) + has(ref hasOBjectOrParam, ItemID.PharaohsMask) + has(ref hasOBjectOrParam, ItemID.FlyingCarpet);
                    rares += checkAdd("Many Pyramids");

                    score.pyramids = pyramids; //overriten set geninfo ############## todo check for pyramid without chest, does exists?
                }

                if (step >= 3)
                {
                    //positive, check if can do in basic, do all in one?

                    if (!omitRare.Contains(OptionsDict.GeneralOptions.omitCommonRare) && !omitRare.Contains(OptionsDict.GeneralOptions.omitBaCRare))
                    {
                        rares += checkAdd("Near Enchanted Sword");
                        rares += checkAdd("Enchanted Sword near Tree");
                        rares += checkAdd("Enchanted Sword near Pyramid");
                        rares += checkAdd("Spawn in Snow biome");
                        rares += checkAdd(OptionsDict.Phase3.lonelyJungleTree);
                        rares += checkAdd(OptionsDict.Phase3.openTemple);
                        rares += checkAdd("Shadow Chest item in normal chest");                        
                        rares += checkAdd("Life Crystal duplication Glitch");
                        rares += checkAdd("Pot duplication Glitch Single");
                        

                    }

                    if (!omitRare.Contains(OptionsDict.GeneralOptions.omitBadRare) && !omitRare.Contains(OptionsDict.GeneralOptions.omitBaCRare))
                        rares += checkAdd("Floating Island without chest"); //bugged if you do large map first and small after they are forced to create
                    

                    rares += checkAdd("Spawn in Sky");
                    rares += checkAdd("Near Enchanted Sword near Tree");
                    rares += checkAdd("Near Enchanted Sword near Pyramid");
                    rares += checkAdd("Very Near Enchanted Sword");
                    rares += checkAdd("Spawn in Jungle biome");
                    rares += checkAdd(OptionsDict.Phase3.allChestItemsNoCraftFish);
                    rares += checkAdd("Floating island cabin in Dungeon");
                    rares += checkAdd("Detonator at surface");
                    rares += checkAdd(OptionsDict.Phase3.greenPyramid);                    
                    rares += checkAdd(OptionsDict.Phase3.frozenTemple);                    
                    rares += checkAdd("Minecart Track close to spawn");                    
                    rares += checkAdd("ExplosiveDetonator close to spawn");                    
                    rares += checkAdd("Enchanted Sword duplication Glitch");
                    rares += checkAdd("Life Crystal duplication Glitch Single");
                    rares += checkAdd("Pot duplication Glitch Single Cavern");
                    rares += checkAdd("Game breaker");


                    rares += checkAdd("Spawn in Marble or Granite biome");
                    
                    rares += checkAdd("Mushroom Biome above surface");
                    rares += checkAdd("All Paintings");

                }


                return rares;
            }


            public static string GetMissingUnmakeAbleItems(ScoreWorld score)
            {
                int missingCount = 0;
                int missingCountNot = 0;
                Dictionary<string, int> hasOBjectOrParam = score.hasOBjectOrParam;
                string itemlist = "Missing unmakeable items:" + Environment.NewLine;
                Action<string, string> CheckAddItem = (string name, string realName) =>
                {
                    if (hasOBjectOrParam[name] == 0)
                    {
                        itemlist += realName + Environment.NewLine;
                        missingCount++;
                    }
                    else
                        missingCountNot++;
                };
                CheckAddItem("Pyramid Bottle", "Sandstorm in a Bottle");
                CheckAddItem("Pyramid Carpet", "Flying Carpet");
                CheckAddItem("Pyramid Mask", "Pharaoh's Mask");
                CheckAddItem("Tree Chest", "Leaf Wand");
                CheckAddItem("Tree Chest", "Living Wood Wand");
                CheckAddItem("Tree Chest Loom", "Living Loom");
                CheckAddItem("Cloud Sky Mill", "Sky Mill");
                CheckAddItem("Ice Skates", "Ice Skates");
                CheckAddItem("Flurry Boots", "Flurry Boots");
                CheckAddItem("Blizzard in a Bottle", "Blizzard in a Bottle");
                CheckAddItem("Ice Machine", "Ice Machine");
                CheckAddItem("Snowball Cannon", "Snowball Cannon");
                CheckAddItem("Ice Boomerang", "Ice Boomerang");
                CheckAddItem("Ice Blade", "Ice Blade");
                CheckAddItem("Fish Pet", "Fish Pet");
                CheckAddItem("Ice Mirror", "Ice Mirror");
                CheckAddItem("Magic Mirror", "Magic Mirror");
                CheckAddItem("Hermes Boots", "Hermes Boots");
                CheckAddItem("Band of Regeneration", "Band of Regeneration");
                CheckAddItem("Lava Charm", "Lava Charm");
                CheckAddItem("Water Walking Boots", "Water Walking Boots");
                CheckAddItem("Shoe Spikes", "Shoe Spikes");
                CheckAddItem("Honey Dispenser", "Honey Dispenser");
                CheckAddItem("Living Mahogany Wand", "Living Mahogany Wand");
                CheckAddItem("Living Mahogany Wand", "Rich Mahogany Leaf Wand");
                CheckAddItem("Seaweed Pet", "Seaweed Pet");
                CheckAddItem("Flower Boots", "Flower Boots");
                CheckAddItem("Sharpening Station", "Sharpening Station");
                CheckAddItem("Alchemy Table", "Alchemy Table");
                CheckAddItem("Bone Welder", "Bone Welder");
                CheckAddItem("Valor", "Valor");
                CheckAddItem("Water Bolt", "Water Bolt");
                CheckAddItem("Dart Trap", "Dart Trap");
                CheckAddItem("Super Dart Trap", "Super Dart Trap");
                CheckAddItem("Flame Trap", "Flame Trap");
                CheckAddItem("Spiky Ball Trap", "Spiky Ball Trap");
                CheckAddItem("Spear Trap", "Spear Trap");
                CheckAddItem("Geyser", "Geyser");
                CheckAddItem("Detonator", "Detonator");
                

                CheckAddItem("Enchanted Sword", "Enchanted Sword");

                CheckAddItem("All Dungeon Walls", "Not all Dungeon walls present");


                if (hasOBjectOrParam["Different functional noncraf. Statues"] < 26 - (Main.expertMode ? 0 : 2) )
                {
                    itemlist += (26 - (Main.expertMode ? 0 : 2) - hasOBjectOrParam["Different functional noncraf. Statues"]).ToString() + " functional statue" + Environment.NewLine;
                    missingCount++;
                }
                else missingCountNot++;

                int normalStatues = hasOBjectOrParam["Different noncraf. Statues"] - hasOBjectOrParam["Different functional noncraf. Statues"];
                if (normalStatues < 30)
                {
                    itemlist += (30 - normalStatues).ToString() + " not functional statue" + Environment.NewLine;
                    missingCount++;
                }
                else missingCountNot++;

                if (hasOBjectOrParam["Number different Paintings"] < 51)
                {
                    itemlist += (51 - hasOBjectOrParam["Number different Paintings"]).ToString() + " Painting" + Environment.NewLine;
                    missingCount++;
                }
                else missingCountNot++;

                score.missingCount = missingCount;
                score.missingCountNot = missingCountNot;

                return itemlist;
            }



            public void resetPoints(ScoreWorld score)
            {
                score.rare = 0;
                score.points = 0;
                rares = 0;
                points = 0;
                rareText = "";
                score.missingCount = 0;
                score.missingCountNot = 0;
            }



            public string conditionCheck = "";
            public int checkConditions(ScoreWorld score, Configuration config, int step)
            {
                //step == stage ==~phase

                resetPoints(score);
                conditionCheck = "";//debug
                Dictionary<string, int> hasOBjectOrParam = score.hasOBjectOrParam;

                if (config == null)
                {
                    throw new Exception("TerrariaSeadSearch checkConditions had invalid config file (null)\n");
                }


                int points = 0;

                omitRareAll = false;
                omitRare.Clear();

                bool isInPositive = false;
                bool isInNegative = false;
                bool isInOmitRare = false;
                int cstep = -1; //phase -1


                int phase3Count = 0;
                int phase2Count = 0;
                int phaseStartingpoints = 0;
                bool cPquerry = false;

                bool pointsReseted = false;
                int pointsB4Reset = 0;

                int dummyTool = 0; //adds /sub more than one point
                bool dummySet = false;

                int negativeListPointSize = 1;

                const int CCcount = 21;
                const int CCcountNeg = 5; // max number for negative count
                int[] conditionConnectorIsTrue = new int[CCcount];
                int[] conditionConnectorIsFalse = new int[CCcount];
                bool[] conditionConnectorIsActive = new bool[CCcount];

                bool isPoint = false;

                Configuration.ConfigItemType lastType = Configuration.ConfigItemType.Other;
                //cast to list to omit overwriting error
                foreach (Configuration.ConfigurationItem cci in config.configList.ToList())
                {
                    if (cci.type == Configuration.ConfigItemType.SelectableText && cci.phase == 2)
                        phase2Count += 1;
                    if (cci.type == Configuration.ConfigItemType.SelectableText && cci.phase == 3 && !cci.name.Equals(OptionsDict.Phase3.continueEvaluation))
                        phase3Count += 1;

                    if (cci.phase <= step && cci.phase > 0)
                    {
                        isInOmitRare = false;
                        switch (cci.type)
                        {

                            case Configuration.ConfigItemType.SelectableText:
                                if (cci.phase == 1 && !cci.name.Equals(OptionsDict.Phase1.pyramidsPossible)) break;

                                int value = 0;
                                int condCon = -1;
                                if (!Int32.TryParse(cci.value, out value) && !cci.name.Equals(OptionsDict.Phase3.continueEvaluation))
                                {
                                    writeDebugFile("could not parse value " + cci.value + " with lenght " + cci.value.Length + "for key " + cci.name + " in stage " + cstep + " cciphzzzase ");
                                }
                                if (cci.name.Length > ((OptionsDict.Tools.conditionConnector).Length+1) && cci.name.Contains(OptionsDict.Tools.conditionConnector))
                                {
                                    if(!Int32.TryParse(cci.name.Substring((OptionsDict.Tools.conditionConnector).Length+1,1), out condCon) || condCon > CCcountNeg || condCon < 1)
                                    {
                                        writeDebugFile("could not parse value Condition Conector" + cci.name + " with substring :"+ cci.name.Substring((OptionsDict.Tools.conditionConnector).Length, 1) + ": in stage " + cstep + " cciphzzzase ");
                                    }
                                }

                                if (!hasOBjectOrParam.ContainsKey(cci.name) && !cci.name.Equals(OptionsDict.Phase3.continueEvaluation) 
                                    && !cci.name.Equals(OptionsDict.Tools.dummyPlus) && !cci.name.Equals(OptionsDict.Tools.dummyNeg) 
                                    && condCon == -1 && !cci.name.Equals(OptionsDict.Tools.conditionConnector)
                                    && !cci.name.Equals(OptionsDict.Tools.dummyNegEnhancer))
                                {
                                    writeDebugFile("could not find key " + cci.name + " in stage " + cstep + " cciphase " + cci.phase);
                                }

                                if (isInPositive)
                                {
                                    if (cci.name.Equals(OptionsDict.Tools.dummyPlus))
                                    {
                                        dummyTool += value;
                                        dummySet = true;
                                        conditionCheck += "in positive cPquerry " + cPquerry + "  tested dummy " + cci.name + ": " + value + "   , points:" + points + " stage " + cstep + Environment.NewLine;
                                    }
                                    else if (cci.name.Equals(OptionsDict.Tools.conditionConnector))
                                    {
                                        if (cci.phase == 2)
                                            conditionConnectorIsActive[value] = true;
                                        else if (cci.phase == 3)
                                            cPquerry = cPquerry && conditionConnectorIsTrue[value] > 0; //gets true if any of those in phase 2 where true too
                                       conditionCheck += "in positive cPquerry " + cPquerry + "  tested condCon " + cci.name + ": " + value + "   , points:" + points + " stage " + cstep + Environment.NewLine;
                                    }
                                    else
                                    {
                                        cPquerry = cPquerry && hasOBjectOrParam[cci.name] >= value;
                                        conditionCheck += "in positive cPquerry " + cPquerry + "  tested " + cci.name + ": " + hasOBjectOrParam[cci.name] + " >= " + value + "   , points:" + points + " stage " + cstep + Environment.NewLine;
                                    }
                                }
                                else if (isInNegative)
                                {
                                    if (cci.name.Equals(OptionsDict.Tools.dummyNeg))
                                    {
                                        points -= value;
                                        conditionCheck += "in negative cPquerry " + cPquerry + "  tested dummy " + cci.name + ": " + value + "   , points:" + points + " stage " + cstep + Environment.NewLine;
                                    }else if(condCon != -1 && cci.phase == 2) {
                                        points -= conditionConnectorIsFalse[condCon] > value ? negativeListPointSize : 0;
                                        conditionCheck += "in negative cPquerry " + cPquerry + "  tested condCon " + cci.name + ": id:"+condCon+" , falseVal:"+ conditionConnectorIsFalse[condCon] +" >" + value + "   , points:" + points + " stage " + cstep + Environment.NewLine;
                                    }else if (cci.name.Equals(OptionsDict.Tools.dummyNegEnhancer))
                                    {
                                        negativeListPointSize = value;
                                        conditionCheck += "in negative cPquerry " + cPquerry + "  tested " + cci.name + ": negate below by > " + value + "   , points:" + points + " stage " + cstep + Environment.NewLine;
                                    }
                                    else
                                    {
                                        points -= hasOBjectOrParam[cci.name] > value ? negativeListPointSize : 0;
                                        conditionCheck += "in negative cPquerry " + cPquerry + "  tested " + cci.name + ": " + hasOBjectOrParam[cci.name] + " > " + value + "   , points:" + points + " stage " + cstep + Environment.NewLine;
                                    }
                                }
                                else
                                {
                                    if (cci.name.Equals(OptionsDict.Phase3.continueEvaluation))
                                    {
                                        if (cci.value.Equals(OptionsDict.Phase3.continueEvaluationResetTag))
                                        {
                                            points = Math.Min(phaseStartingpoints, step - 1);
                                            pointsReseted = true;
                                            pointsB4Reset = points;
                                        }
                                        conditionCheck += "in selectable continue Eval " + cPquerry + "  tested " + cci.name + ": points:" + points + " stage " + cstep + Environment.NewLine;
                                    }
                                    else
                                    {
                                        points += hasOBjectOrParam[cci.name] >= value ? 1 : 0;
                                        conditionCheck += "in selectable cPquerry " + cPquerry + "  tested " + cci.name + ": " + hasOBjectOrParam[cci.name] + " >= " + value + "   , points:" + points + " stage " + cstep + Environment.NewLine;
                                    }
                                }

                                break;
                            case Configuration.ConfigItemType.SelectableListPositive:                                
                                isPoint = cPquerry && lastType != Configuration.ConfigItemType.SelectableListPositive;
                                points += isPoint ? (dummySet ? dummyTool : 1) : 0;
                                for(int condconi=1; condconi < CCcount; condconi++)
                                {
                                    if (!conditionConnectorIsActive[condconi]) continue;
                                    conditionConnectorIsActive[condconi] = false;
                                    if (isPoint) conditionConnectorIsTrue[condconi]++;
                                    else conditionConnectorIsFalse[condconi]++;

                                }
                                cPquerry = true;                                
                                dummyTool = 0; dummySet = false;
                                isInPositive = true; isInNegative = false;
                                negativeListPointSize = 1;
                                conditionCheck += "positive now cPquerry " + cPquerry + "  tested   , points:" + points + " stage " + cstep + Environment.NewLine;
                                break;
                            case Configuration.ConfigItemType.SelectableListNegative:
                                isPoint = cPquerry && lastType != Configuration.ConfigItemType.SelectableListPositive;
                                points += isPoint ? (dummySet ? dummyTool : 1) : 0;
                                for (int condconi = 1; condconi < CCcount; condconi++)
                                {
                                    if (!conditionConnectorIsActive[condconi]) continue;
                                    conditionConnectorIsActive[condconi] = false; // not needed here
                                    if (isPoint) conditionConnectorIsTrue[condconi]++;
                                    else conditionConnectorIsFalse[condconi]++;

                                }

                                cPquerry = false;                                
                                dummyTool = 0; dummySet = false;
                                isInNegative = true; isInPositive = false;
                                negativeListPointSize = 1;
                                conditionCheck += "negative now cPquerry " + cPquerry + "  tested    , points:" + points + " stage " + cstep + Environment.NewLine;
                                break;
                            case Configuration.ConfigItemType.Header:
                                points += cPquerry && lastType != Configuration.ConfigItemType.SelectableListPositive ? (dummySet ? dummyTool : 1) : 0;

                                phaseStartingpoints = points;
                                cPquerry = false;
                                isInNegative = false; isInPositive = false;
                                dummyTool = 0; dummySet = false;
                                negativeListPointSize = 1;
                                cstep++;
                                conditionCheck += "header now cPquerry " + cPquerry + "  tested    , points:" + points + " stage " + cstep + " zz " + Environment.NewLine;
                                break;

                        }


                    }
                    else if (cci.phase <= 0)
                    {
                        if (cci.type == Configuration.ConfigItemType.SelectableListOmitRare)
                        {                            //not really needed
                            isInOmitRare = true; isInNegative = false; isInPositive = false;
                        }
                        else if (cci.type != Configuration.ConfigItemType.SelectableText)
                        {
                            isInOmitRare = false;
                        }
                        else if (cci.type == Configuration.ConfigItemType.SelectableText && isInOmitRare)
                        {
                            if (cci.name.Equals(OptionsDict.GeneralOptions.omitRareAll)) omitRareAll = true;
                            else if (!omitRare.Contains(cci.name)) omitRare.Add(cci.name);
                        } else if (cci.type == Configuration.ConfigItemType.Header || cci.type == Configuration.ConfigItemType.Title)
                        {
                            cstep++;
                        }
                    }


                    lastType = cci.type;


                }
                conditionCheck += "all done now cPquerry " + cPquerry + "  , points:" + points + " stage " + cstep + Environment.NewLine;
                points += cPquerry && lastType != Configuration.ConfigItemType.SelectableListPositive ? 1 : 0;

                int rare = checkRares(score, step, omitRare);
                if (omitRareAll) rare = 0;

                conditionCheck += "all done after now cPquerry " + cPquerry + "  ,total  points:" + points + "and rares " + rare + " stage " + cstep + Environment.NewLine;


                if (pointsReseted)
                    points = Math.Max(pointsB4Reset, points);

                if (phase2Count == 0 && points > 0)
                    points++;
                if (phase3Count == 0 && (step != 1 || phase2Count == 0) && points > 1)
                    points++;

                conditionCheck += "total points " + points + Environment.NewLine;

                //writeDebugFile(conditionCheck);
                //writeDebugFile(" step " + step + " " + phase2Count + " 3:" + phase3Count + " p" + points);


                rares = rare;
                this.points = points;
                score.rare = rare;
                score.points = points;

                score.phase3Empty = phase3Count == 0 ? true : false;

                return points;
            }


        }


        private static Tuple<int,int,int> CheckPyramidHeight(int px, int py)
        {
            int yi = py;
            for (; yi<Main.maxTilesY; yi++)
            {
                
                int numCheck = (yi - py) / 2;
                int tilesNTrue = 0;
                //in case tunnel is below tip
                for(int add=0; add < numCheck; add++)
                {
                    Tile tile = Main.tile[px-add, yi];
                    if ((tile.active() && tile.type != TileID.SandstoneBrick) || (!tile.active() && tile.wall != WallID.SandstoneBrick))
                    {
                        tilesNTrue++;
                    }
                    tile = Main.tile[px + add, yi];
                    if ((tile.active() && tile.type != TileID.SandstoneBrick) || (!tile.active() && tile.wall != WallID.SandstoneBrick))
                    {
                        tilesNTrue++;
                    }
                }
                if(tilesNTrue > numCheck)
                {
                    break;
                }
                
            }
            int pheight = yi - py - 1; //Todo check if path down can be under tip.

            //find path
            int yEnd = yi;
            int xAdd = 1;
            int pxp = 0;
            for (; xAdd < pheight; xAdd++)
            {
                pxp = px + xAdd;
                Tile tile = Main.tile[pxp, yEnd];
                if ((tile.active() && tile.type == TileID.SandstoneBrick) || (!tile.active() && tile.wall == WallID.SandstoneBrick))
                {
                    break;
                }
                pxp = px - xAdd;
                tile = Main.tile[pxp, yEnd];
                if ((tile.active() && tile.type == TileID.SandstoneBrick) || (!tile.active() && tile.wall == WallID.SandstoneBrick))
                {
                    break;
                }
            }
            //path starth pxp, yEnd
            px = pxp;
            int pyp = yEnd;
            const int pathSearchMax = 5;
            bool pnEnd = true;
            for (; pyp < Main.maxTilesY && pnEnd; pyp++)
            {
                pnEnd = false;
                for (xAdd = 0; xAdd < pathSearchMax; xAdd++)
                {                    
                    pxp = Math.Min(px + xAdd, Main.maxTilesX-1);
                    Tile tile = Main.tile[pxp, pyp];
                    if ((tile.active() && tile.type == TileID.SandstoneBrick) || (!tile.active() && tile.wall == WallID.SandstoneBrick))
                    {
                        px = pxp;
                        pnEnd = true;
                        break;
                    }
                    pxp = Math.Max(px - xAdd,0);
                    tile = Main.tile[pxp, pyp];
                    if ((tile.active() && tile.type == TileID.SandstoneBrick) || (!tile.active() && tile.wall == WallID.SandstoneBrick))
                    {
                        px = pxp;
                        pnEnd = true;
                        break;
                    }
                }
            }
            int pExitCavernEntDist = pyp - (int)Main.rockLayer;
            int pPathSize = pyp - yEnd - 1;
            //int pTotalSize = pPathSize + pheight;

            return new Tuple<int, int, int>(pheight, pPathSize, pExitCavernEntDist);
        }

        private static int CheckPyramidOpenAirSurface(int px, int py)
        {
            //px py top of pyramid

            int os = 0; //open surface
            int nos = 0;
            int sid = -1;

            int pxs = px;
            int pys = py;

            const int maxNf = 20;
            while (sid < 2)
            {
                py = pys; px = pxs; nos = 0;
                while (nos < maxNf)
                {
                    Tile tile = Main.tile[px, py];
                    Tile tileOs = Main.tile[px + sid, py - 1];
                    Tile tileW = Main.tile[px - sid, py];

                    if (!tileOs.active() && tileOs.wall == WallID.None && (tile.active() && tile.type == TileID.SandstoneBrick) || (!tile.active() && !tileW.active() && tileW.wall == WallID.SandstoneBrick))
                    {
                        os++;
                        if (nos > 0) nos--;
                    }
                    else
                    {
                        if ((tile.active() && tile.type == TileID.LivingWood) || (!tile.active() && tile.wall == WallID.LivingWood))
                        {

                        }
                        else
                        {
                            nos++;
                        }
                    }
                    py++;
                    px += sid;

                }
                sid += 2;
            }

            return os;
        }


        private static void CheckAndSetTreeSizes(ref Dictionary<string,int> hasOBjectOrParam, HashSet<int> treesPos, int stage)
        {
            
                int maxTreeSize = 0;
                int minTreeSize = 9000;
                int maxTreeRootSize = 0;
                int maxTreeTotalSize = 0;
                int maxDepth = -100000;
                int deepCavernTreeNearMid = 0;
                int deepCavernTree = 0;
                int deepClostToCavernTreeNearMid = 0;
                int deepClostToCavernTree = 0;

            Func<int, int, bool> checkIfTree = (xt, yt) =>
                {
                    bool isTree = true;

                    for (int xti = xt; xti > xt - 10; xti--)
                    {
                        if (xti < 0 || (Main.tile[xti, yt].active() && Main.tile[xti, yt].type != TileID.LivingWood && Main.tile[xti, yt].type != TileID.LeafBlock
                        && Main.tile[xti, yt].type != TileID.Vines && Main.tile[xti, yt].wall != WallID.LivingWood && Main.tile[xti, yt].type != TileID.Trees
                        && Main.tile[xti, yt].type != TileID.PalmTree && Main.tile[xti, yt].type != TileID.Sunflower && Main.tile[xti, yt].type != TileID.Pots
                        && Main.tile[xti, yt].type != TileID.Plants && Main.tile[xti, yt].type != TileID.JunglePlants && Main.tile[xti, yt].type != TileID.FleshWeeds
                        && Main.tile[xti, yt].type != TileID.CorruptPlants && Main.tile[xti, yt].type != TileID.SmallPiles && Main.tile[xti, yt].type != TileID.LargePiles
                        && Main.tile[xti, yt].type != TileID.LargePiles2))
                        {
                            isTree = false;
                            break;
                        }
                        if (xti > 0 && !Main.tile[xti, yt].active() && Main.tile[xti, yt].wall != WallID.LivingWood)
                        {
                            break;
                        }
                    }
                    if (!isTree) return false;
                    for (int xti = xt; xti < xt + 10; xti++)
                    {
                        if (xti >= Main.maxTilesX || ( Main.tile[xti, yt].active() && Main.tile[xti, yt].type != TileID.LivingWood && Main.tile[xti, yt].type != TileID.LeafBlock
                        && Main.tile[xti, yt].type != TileID.Vines && Main.tile[xti, yt].wall != WallID.LivingWood && Main.tile[xti, yt].type != TileID.Trees
                        && Main.tile[xti, yt].type != TileID.PalmTree && Main.tile[xti, yt].type != TileID.Sunflower && Main.tile[xti, yt].type != TileID.Pots
                        && Main.tile[xti, yt].type != TileID.Plants && Main.tile[xti, yt].type != TileID.JunglePlants && Main.tile[xti, yt].type != TileID.FleshWeeds
                        && Main.tile[xti, yt].type != TileID.CorruptPlants && Main.tile[xti, yt].type != TileID.SmallPiles && Main.tile[xti, yt].type != TileID.LargePiles
                        && Main.tile[xti, yt].type != TileID.LargePiles2))
                        {

                            isTree = false;
                            break;
                        }
                        if (xti < Main.maxTilesX && !Main.tile[xti, yt].active() && Main.tile[xti, yt].wall != WallID.LivingWood)
                        {
                            break;
                        }
                    }

                    return isTree;
                };

                Func<int, int, int> findTreeTile = (xt, yt) =>
                {
                    int xl = xt;
                    int xr = xt;
                    const int maxDiff = 5;
                    bool found = Main.tile[xt, yt].wall == WallID.LivingWood || (Main.tile[xl, yt].active() && (Main.tile[xl, yt].type == TileID.LivingWood || Main.tile[xt, yt].type == TileID.LeafBlock));
                    while (xl> 0 && (!Main.tile[xl, yt].active() || (Main.tile[xl, yt].active() && (Main.tile[xl, yt].wall != WallID.LivingWood && Main.tile[xl, yt].type != TileID.LivingWood && Main.tile[xl, yt].type != TileID.LeafBlock))) && xt - xl <= maxDiff) { xl--; if (Main.tile[xl, yt].active() || Main.tile[xl, yt].wall == WallID.LivingWood) found = true; }
                    while (xr< Main.maxTilesX && (!Main.tile[xr, yt].active() || (Main.tile[xr, yt].active() && (Main.tile[xr, yt].wall != WallID.LivingWood && Main.tile[xr, yt].type != TileID.LivingWood && Main.tile[xr, yt].type != TileID.LeafBlock))) && xr - xt <= maxDiff) { xr++; if (Main.tile[xr, yt].active() || Main.tile[xr, yt].wall == WallID.LivingWood) found = true; }

                    int diffL = xt - xl;
                    int diffR = xr - xt;



                    if (diffL == diffR)
                    {
                        //if (diffL == maxDiff || !found || (diffL == 0 && diffR == 0 && (!Main.tile[xt, yt].active() && Main.tile[xt, yt].wall != WallID.LivingWood)) )
                        if (diffL == maxDiff || !found)
                            return -1;
                        else
                            return (xr + xl) / 2;
                    }
                    if (diffL < diffR)
                        return xl;
                    else
                        return xr;

                };

                Func<int, int, int> findTreeRootTile = (xt, yt) =>
                {
                    int xl = xt;
                    int xr = xt;
                    const int maxDiff = 5;
                    bool found = Main.tile[xt, yt].wall == WallID.LivingWood || (Main.tile[xl, yt].active() && (Main.tile[xl, yt].type == TileID.LivingWood || (Main.tile[xl, yt].type == TileID.Platforms && Main.tile[xl, yt].frameY == 414)));
                    while (xl> 0 && (!Main.tile[xl, yt].active() || (Main.tile[xl, yt].active() && (Main.tile[xl, yt].wall != WallID.LivingWood && Main.tile[xl, yt].type != TileID.LivingWood && !(Main.tile[xl, yt].type == TileID.Platforms && Main.tile[xl, yt].frameY == 414)))) && xt - xl <= maxDiff) { xl--; if ((Main.tile[xl, yt].active() && (Main.tile[xl, yt].type == TileID.LivingWood || (Main.tile[xl, yt].type == TileID.Platforms && Main.tile[xl, yt].frameY == 414))) || Main.tile[xl, yt].wall == WallID.LivingWood) found = true; }
                    while (xr < Main.maxTilesX && (!Main.tile[xr, yt].active() || (Main.tile[xr, yt].active() && (Main.tile[xr, yt].wall != WallID.LivingWood && Main.tile[xr, yt].type != TileID.LivingWood && !(Main.tile[xr, yt].type == TileID.Platforms && Main.tile[xr, yt].frameY == 414)))) && xr - xt <= maxDiff) { xr++; if ((Main.tile[xr, yt].active() && (Main.tile[xr, yt].type == TileID.LivingWood || (Main.tile[xr, yt].type == TileID.Platforms && Main.tile[xr, yt].frameY == 414))) || Main.tile[xr, yt].wall == WallID.LivingWood) found = true; }

                    int diffL = xt - xl;
                    int diffR = xr - xt;



                    if (diffL == diffR)
                    {
                        //if (diffL == maxDiff || !found || (diffL == 0 && diffR == 0 && (!Main.tile[xt, yt].active() && Main.tile[xt, yt].wall != WallID.LivingWood)) )
                        if (diffL == maxDiff || !found)
                            return -1;
                        else
                            return (xr + xl) / 2;
                    }
                    if (diffL < diffR)
                        return xl;
                    else
                        return xr;

                };



                foreach (var treeX in treesPos)
                {
                    int y = 1;
                    for (; y < Main.maxTilesY; y++)
                    {
                        if (Main.tile[treeX, y].type == TileID.LivingWood || Main.tile[treeX + 1, y].type == TileID.LivingWood || Main.tile[treeX - 1, y].type == TileID.LivingWood)
                            break;
                    }

                    int ymerk = y;


                    for (; y < Main.maxTilesY; y++)
                    {
                        if (!checkIfTree(treeX, y))
                            break;
                    }
                    //Main.tile[treeX, ymerk].active(true);
                    //Main.tile[treeX, ymerk].type = TileID.SandStoneSlab;
                    //Main.tile[treeX+1, ymerk].active(true);
                    //Main.tile[treeX+1, ymerk].type = TileID.SandStoneSlab;
                    //Main.tile[treeX-1, ymerk].active(true);
                    //Main.tile[treeX-1, ymerk].type = TileID.SandStoneSlab;


                    //Main.tile[treeX, y].active(true);
                    //Main.tile[treeX, y].type = TileID.Crimstone;
                    //Main.tile[treeX + 1, y].active(true);
                    //Main.tile[treeX + 1, y].type = TileID.Crimstone;
                    //Main.tile[treeX - 1, y].active(true);
                    //Main.tile[treeX - 1, y].type = TileID.Crimstone;

                    //root
                    y--;
                    int yStart = y;
                    int xt = treeX;
                    for (; y > 1; y--)
                    {
                        xt = findTreeTile(xt, y - 1);

                        if (xt == -1)
                            break;
                        else
                        {
                            if (stage > 2)
                            {
                                //Main.tile[xt, y].active(true);
                                //Main.tile[xt, y].type = TileID.CrimsonSandstone;
                            }
                        }
                    }
                    int yEnd = y;
                    int treeSize = yStart - yEnd;
                    maxTreeSize = Math.Max(maxTreeSize, treeSize);
                    minTreeSize = Math.Min(minTreeSize, treeSize);

                    //find deep roots

                    y = yStart + 1;
                    xt = treeX;
                    for (; y < Main.maxTilesY; y++)
                    {
                        xt = findTreeRootTile(xt, y + 1);

                        if (xt == -1)
                            break;
                        else
                        {
                            if (stage > 2)
                            {
                                //Main.tile[xt, y].active(true);
                                //Main.tile[xt, y].type = TileID.CrimsonSandstone;
                            }
                        }
                    }
                    yEnd = y;
                    int treeRootSize = yEnd - yStart;
                    maxTreeRootSize = Math.Max(maxTreeRootSize, treeRootSize);
                    maxTreeTotalSize = Math.Max(maxTreeTotalSize, treeRootSize + treeSize);
                    maxDepth = Math.Max(maxDepth, yEnd - (int)Main.rockLayer);

                    if (yEnd + 43 > Main.rockLayer)
                    {
                        deepClostToCavernTree++;
                        if (Math.Abs(Main.maxTilesX / 2 - treeX) < 300)
                            deepClostToCavernTreeNearMid++;

                        if (yEnd + 3 > Main.rockLayer)
                        {
                            deepCavernTree++;
                            if (Math.Abs(Main.maxTilesX / 2 - treeX) < 300)
                                deepCavernTreeNearMid++;
                        }
                    }
                }





                //writeDebugFile("max tree root siez " + maxTreeRootSize + " total " + maxTreeTotalSize + " " + seed);

                //it can happen that a tree is one tile smaller in phase 3 than 2

                hasOBjectOrParam["Max Living Tree Size"] = maxTreeSize;
                hasOBjectOrParam["Min Living Tree Size"] = minTreeSize;
                hasOBjectOrParam["Max Living Tree root Size"] = maxTreeRootSize;
                hasOBjectOrParam["Max Living Tree total Size"] = maxTreeTotalSize;
                hasOBjectOrParam[OptionsDict.Phase2.maxTreeExitCavDist] = maxDepth;

                hasOBjectOrParam["Tree to cavern layer"] = deepCavernTree;
                hasOBjectOrParam["Tree to cavern layer near mid"] = deepCavernTreeNearMid;
                hasOBjectOrParam["Tree close to cavern layer"] = deepClostToCavernTree;
                hasOBjectOrParam["Tree close to cavern layer near mid"] = deepClostToCavernTreeNearMid;



        }

        private static string getPathToSeedFile(AcceptConditons acond)
        {
            string smill = (Main.ActiveWorldFileData.Seed.ToString()).PadLeft(10, '0').Substring(0, 4);
            string evil = WorldGen.WorldGenParam_Evil == 1 ? "Crim" : WorldGen.WorldGenParam_Evil == 0 ? "Corr" : "Rand";


            string pathname = @".\currentSeeds\currentSeed" + smill + "_" + (Main.maxTilesY / 600 - 1).ToString() + (Main.expertMode ? "exp" : "nor") + evil + "_" + acond.chanceMaxPyramids + acond.takeOnlyPyramids+ ".txt";

            return pathname;
        }


        public void StoreMapAsPNG(bool includeInfo)
        {

            int dimX = Main.maxTilesX;
            int dimY = Main.maxTilesY;
            int scale = 1;
            /*while (dimX > 6200)
            {
                dimX /= 2;
                dimY /= 2;
                scale *= 2;
            }*/
            
            int bytes = dimX*dimY * 4;

            byte[] rgbValues = new byte[bytes];


            //todo in main loop ? (not needed there)
            List<Tuple<int,int>> hearts = new List<Tuple<int, int>>();
            List<Tuple<int, int>> demonAltars = new List<Tuple<int, int>>();
            List<Tuple<int, int>> rubies = new List<Tuple<int, int>>();
            List<Tuple<int, int>> diamonds = new List<Tuple<int, int>>();
            List<Tuple<int, int>> explosive = new List<Tuple<int, int>>();


            int indx = 0;
            for (int y = 0; y < Main.maxTilesY; y += scale)
                for (int x = 0; x < Main.maxTilesX; x += scale)
                {              
                    MapTile cur = MapHelper.CreateMapTile(x, y, 255);
                    Color cc = MapHelper.GetMapTileXnaColor(ref cur);

                    if (x > Main.offLimitBorderTiles && x < Main.maxTilesX - Main.offLimitBorderTiles && y > Main.offLimitBorderTiles && y < Main.maxTilesY - Main.offLimitBorderTiles)
                    {
                        if (Main.tile[x, y].type == TileID.Heart && Main.tile[x, y].frameX == 0 && Main.tile[x, y].frameY == 0)
                            hearts.Add(new Tuple<int, int>(x, y));
                        else if (Main.tile[x, y].type == TileID.DemonAltar && Main.tile[x, y].frameY == 0 && (
                            (Main.tile[x, y].frameX == 18 && Main.tile[x, y].wall != WallID.EbonstoneUnsafe && Main.tile[x, y].wall != WallID.CorruptGrassUnsafe && Main.tile[x, y + 2].type != TileID.Ebonstone && Main.tile[x - 1, y + 2].type != TileID.Ebonstone && Main.tile[x + 1, y + 2].type != TileID.Ebonstone) ||
                            (Main.tile[x, y].frameX == 72 && Main.tile[x, y].wall != WallID.CrimstoneUnsafe && Main.tile[x, y].wall != WallID.CrimsonGrassUnsafe && Main.tile[x, y + 2].type != TileID.Crimstone && Main.tile[x - 1, y + 2].type != TileID.Crimstone && Main.tile[x + 1, y + 2].type != TileID.Crimstone)
                            ))
                            demonAltars.Add(new Tuple<int, int>(x, y));
                        else if (Main.tile[x, y].active() && Main.tile[x, y].type == TileID.Ruby || (Main.tile[x, y].type == TileID.ExposedGems && Main.tile[x, y].frameX == 72) || (Main.tile[x, y].type == TileID.SmallPiles && Main.tile[x, y].frameX == 828 && Main.tile[x, y].frameY == 18) )
                            rubies.Add(new Tuple<int, int>(x, y));
                        else if (Main.tile[x, y].active() && Main.tile[x, y].type == TileID.Diamond || (Main.tile[x, y].type == TileID.ExposedGems && Main.tile[x, y].frameX == 90) || (Main.tile[x, y].type == TileID.SmallPiles && Main.tile[x, y].frameX == 864 && Main.tile[x, y].frameY == 18))
                            diamonds.Add(new Tuple<int, int>(x, y));
                        else if (Main.tile[x, y].active() && Main.tile[x, y].type == TileID.Explosives)
                            explosive.Add(new Tuple<int, int>(x, y));
                    }

                    rgbValues[indx++] = cc.B;
                    rgbValues[indx++] = cc.G; 
                    rgbValues[indx++] = cc.R; 
                    rgbValues[indx++] = cc.A;
                }

            
            //draw Spawm
            int aw = 0;

            if (stage>2 || includeInfo)
            for (int y = Main.spawnTileY-1; y > Main.spawnTileY-36; y--)
            {
                int x = Main.spawnTileX;
                int off = y * 4 * Main.maxTilesX + x * 4;

                rgbValues[off + 0] = 0;
                rgbValues[off + 1] = 80;
                rgbValues[off + 2] = 50;
                rgbValues[off + 3] = 255;

                for (int awi = 0; awi < (aw<18?aw:4); awi++) {
                    rgbValues[off + 0 + 4 * (awi / 3)] = 0;
                    rgbValues[off + 1 + 4 * (awi / 3)] = 80;
                    rgbValues[off + 2 + 4 * (awi / 3)] = 50;
                    rgbValues[off + 3 + 4 * (awi / 3)] = 255;

                    rgbValues[off + 0 - 4 * (awi / 3)] = 0;
                    rgbValues[off + 1 - 4 * (awi / 3)] = 80;
                    rgbValues[off + 2 - 4 * (awi / 3)] = 50;
                    rgbValues[off + 3 - 4 * (awi / 3)] = 255;                    
                }
                aw++;
            }


            if (includeInfo)
            {
                if(itemIDdoNotWant != null)
                foreach(var dnid in itemIDdoNotWant)
                {
                    if (score.itemLocation.ContainsKey(dnid))
                    {
                        score.itemLocation.Remove(dnid);

                    }
                }


                if (score.itemLocation.ContainsKey(ItemID.StoneBlock))
                {   //dummy for cavern path
                    foreach (var point in score.itemLocation[ItemID.StoneBlock])
                    {
                        int off = point.Item2 * 4 * Main.maxTilesX + point.Item1 * 4+2;                        
                        rgbValues[off] = (byte)(((0.25*(int)rgbValues[off]) + 192));

                    }
                }
                if (score.itemLocation.ContainsKey(ItemID.JungleShirt))
                    foreach (var point in score.itemLocation[ItemID.JungleShirt])
                    {
                        //dummy for jungle cave path
                        int off = point.Item2 * 4 * Main.maxTilesX + point.Item1 * 4 + 2;
                        rgbValues[off] = (byte)(((0.25 * (int)rgbValues[off]) + 192));

                    }

                if (score.itemLocation.ContainsKey(ItemID.JunglePants))
                    foreach (var point in score.itemLocation[ItemID.JunglePants])
                    {
                        //dummy for jungle cavern path
                        int off = point.Item2 * 4 * Main.maxTilesX + point.Item1 * 4 + 0;
                        rgbValues[off] = (byte)(((0.25 * (int)rgbValues[off]) + 192));

                    }

                if (score.itemLocation.ContainsKey(ItemID.ChaosFish))
                    foreach (var point in score.itemLocation[ItemID.ChaosFish])
                    {
                        //2 tp potions in chest
                        DrawItemImage(ref rgbValues, ItemID.TeleportationPotion, point, scale);

                    }
                if (score.itemLocation.ContainsKey(ItemID.CobaltBar))
                    foreach (var point in score.itemLocation[ItemID.CobaltBar])
                    {
                        //10 iron chest
                        DrawItemImage(ref rgbValues, ItemID.IronBar, point, scale);

                    }
                if (score.itemLocation.ContainsKey(ItemID.MythrilBar))
                    foreach (var point in score.itemLocation[ItemID.MythrilBar])
                    {
                        //12 iron chest
                        DrawItemImage(ref rgbValues, ItemID.IronBar, point, scale);

                    }

                if (score.itemLocation.ContainsKey(ItemID.PlanteraMask))
                    foreach (var bulb in score.itemLocation[ItemID.PlanteraMask])
                    {
                        
                        DrawItemImage(ref rgbValues, ItemID.PlanteraMask, new Tuple<int, int>(bulb.Item1, -bulb.Item2 / 10000), scale);

                    }
                if (score.itemLocation.ContainsKey(ItemID.PlanteraTrophy))
                    foreach (var bulb in score.itemLocation[ItemID.PlanteraTrophy])
                    {

                        DrawItemImage(ref rgbValues, ItemID.PlanteraTrophy, new Tuple<int, int>(bulb.Item1, -bulb.Item2 / 10000), scale);

                    }

                foreach (var itemloclist in score.itemLocation)
                {
                    if (itemloclist.Key != ItemID.StoneBlock && itemloclist.Key != ItemID.JungleShirt && itemloclist.Key != ItemID.JunglePants 
                        && itemloclist.Key != ItemID.PaladinsHammer && itemloclist.Key != ItemID.Ectoplasm && itemloclist.Key != ItemID.ChaosFish
                        && itemloclist.Key != ItemID.CobaltBar && itemloclist.Key != ItemID.MythrilBar && itemloclist.Key != ItemID.PotionStatue
                        && itemloclist.Key != ItemID.PlanteraMask && itemloclist.Key != ItemID.PlanteraTrophy )
                        foreach (var itemloc in itemloclist.Value)
                        {                          
                            DrawItemImage(ref rgbValues, itemloclist.Key, itemloc, scale);                        
                        }                    
                }

                if (score.itemLocation.ContainsKey(ItemID.PaladinsHammer)) 
                    foreach (var spot in score.itemLocation[ItemID.PaladinsHammer])
                    {
                        DrawCircle(ref rgbValues, spot, scale, new Color(0, 255, 255));
                    }
                if (score.itemLocation.ContainsKey(ItemID.Ectoplasm))
                    foreach (var spot in score.itemLocation[ItemID.Ectoplasm])
                    {
                        DrawCircle(ref rgbValues, spot, scale, new Color(0, 170, 170));
                    }

                foreach (var altars in demonAltars)
                {
                    DrawCircle(ref rgbValues, altars, scale, new Color(50, 150, 255));
                }
                foreach (var heart in hearts)
                {
                    DrawCircle(ref rgbValues, heart, scale, new Color(255,0,255));
                }                
                for (int ci = 0; ci < Main.maxChests; ci++)
                {
                    if(Main.chest[ci] != null && Main.chest[ci].x > Main.offLimitBorderTiles && Main.chest[ci].y > Main.offLimitBorderTiles && Main.chest[ci].x < Main.maxTilesX-Main.offLimitBorderTiles)
                        DrawCircle(ref rgbValues, new Tuple<int,int>(Main.chest[ci].x, Main.chest[ci].y), scale, new Color(245, 228, 24));
                }
                foreach (var diamond in diamonds)
                {
                    DrawCircle(ref rgbValues, diamond, scale, new Color(200, 200, 200), 5);
                }
                foreach (var diamond in diamonds)
                {
                    DrawCircle(ref rgbValues, diamond, scale, new Color(230, 230, 230), 3);
                }
                foreach (var ruby in rubies)
                {
                    DrawCircle(ref rgbValues, ruby, scale, new Color(255, 0, 180), 5);                    
                }
                foreach (var ruby in rubies)
                {                    
                    DrawCircle(ref rgbValues, ruby, scale, new Color(255, 0, 0), 3);
                }
                if (score.itemLocation.ContainsKey(ItemID.PotionStatue))
                    foreach (var spot in score.itemLocation[ItemID.PotionStatue])
                    {
                        DrawCircle(ref rgbValues, spot, scale, new Color(86, 195, 220), 17);
                        DrawCircle(ref rgbValues, spot, scale, new Color(100, 60, 40), 13);                        
                        DrawCircle(ref rgbValues, spot, scale, new Color(240, 200, 180), 11);
                    }

                foreach (var expl in explosive)
                {
                    DrawCircle(ref rgbValues, expl, scale, new Color(255, 240, 0), 3);
                    DrawCircle(ref rgbValues, expl, scale, new Color(255, 255, 160), 5);
                }

                if (score.itemLocation.ContainsKey(ItemID.PlanteraTrophy))
                    foreach (var bulb in score.itemLocation[ItemID.PlanteraTrophy])
                    {

                        DrawCircle(ref rgbValues, new Tuple<int, int>(bulb.Item1, -(bulb.Item2 / 10000) - 1), scale, new Color(255, 147, 182), 7);
                        DrawCircle(ref rgbValues, new Tuple<int, int>(bulb.Item1, -(bulb.Item2 / 10000) - 1), scale, new Color(255, 147, 182), 9);

                    }
                if (score.itemLocation.ContainsKey(ItemID.PlanteraMask))
                    foreach (var bulb in score.itemLocation[ItemID.PlanteraMask])
                    {

                        DrawCircle(ref rgbValues, new Tuple<int, int>(bulb.Item1, -(bulb.Item2/10000)-1), scale, new Color(255, 147, 182), 7);
                        DrawCircle(ref rgbValues, new Tuple<int, int>(bulb.Item1, -(bulb.Item2/10000)-1), scale, new Color(255, 147, 182), 9);

                    }

            }


            string filename = Main.worldPathName.Substring(0, Main.worldPathName.Length - 4) + ".png";

            if (ModLoader.windows) {
                //https://msdn.microsoft.com/en-us/library/system.drawing.imaging.bitmapdata(v=vs.110).aspx

                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(dimX, dimY, PixelFormat.Format32bppArgb);

                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);

                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                // Get the address of the first line.
                IntPtr ptr = bmpData.Scan0;


                // Copy the RGB values back to the bitmap
                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

                // Unlock the bits.
                bmp.UnlockBits(bmpData);

                bmp.Save(filename);
            }
            else {
                
                for (int x = 0; x < rgbValues.Length; x += 4)
                {
                    //change red and blue
                    byte b0 = rgbValues[x];
                    rgbValues[x] = rgbValues[x + 2];
                    rgbValues[x + 2] = b0;
                }

                using (FileStream fileStream = File.Create(filename))
                {                                       
                    typeof(PlatformUtilities).Assembly.GetType("Terraria.Utilities.PlatformUtilities").InvokeMember("SavePng", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new Object[] { fileStream, dimX, dimY, dimX, dimY, rgbValues });
                    

                }

                
            }

            rgbValues = null;
            hearts.Clear();
            hearts = null;
            demonAltars.Clear();
            demonAltars = null;
        }
              


        public void DrawItemImage(ref byte[] rgbValues, int itemID, Tuple<int, int> where, int scale)
        {


            Microsoft.Xna.Framework.Graphics.Texture2D cit = ModContent.GetTexture("Terraria/Item_" + itemID);
            
            Color[] tc = new Color[cit.Width * cit.Height];
            cit.GetData<Color>(tc);

            int x = where.Item1/scale;
            int y = where.Item2/scale+4;
            

            const int iconbgs = 48;

            int maxoff = rgbValues.Length-1;


            while (y < Main.maxTilesY/scale - 4*iconbgs)
            {
                int offt = (y+4) * Main.maxTilesX * 4 / scale + x * 4 ;
                //if (rgbValues[offt + 0] != (byte)255 || rgbValues[offt + 1] != (byte)255 || rgbValues[offt + 2] != (byte)255)

                if (offt + 3 > maxoff)
                    return;// changed from continue ??? bug maybe here possible

                if (rgbValues[offt + 3] != (byte)254)
                    break;
                else
                    y += 1;//iconbgs-1;
                
            }

            if(y >= Main.maxTilesY / scale - 4 * iconbgs)
            {
                while (x < Main.maxTilesX / scale - 4 * iconbgs)
                {
                    int offt = (y + 4) * Main.maxTilesX * 4 / scale + x * 4;
                    int offt2 = (y + 4 + iconbgs/2) * Main.maxTilesX * 4 / scale + x * 4;
                    int offt3 = (y + 4 + iconbgs/2) * Main.maxTilesX * 4 / scale + (x-iconbgs/2+4) * 4;
                    //if (rgbValues[offt + 0] != (byte)255 || rgbValues[offt + 1] != (byte)255 || rgbValues[offt + 2] != (byte)255)

                    if (offt + 3 > maxoff || offt2 + 3> maxoff || offt3 + 3 > maxoff)
                        return;// changed from continue ??? bug maybe here possible

                    if (rgbValues[offt + 3] != (byte)254 && rgbValues[offt2 + 3] != (byte)254 && rgbValues[offt3 + 3] != (byte)254)
                        break;
                    else
                        x += 1;//iconbgs - 1;

                }
            }



            for (int h = 0; h < iconbgs; h++)
                for (int w = 0; w < iconbgs; w++)
                {
                    int offt = (y + h ) * Main.maxTilesX * 4 / scale + x * 4 + w * 4 - iconbgs / 2 * 4;
                    //int imgof = h * iconbgs + w;

                    bool isWhite = ( (h- iconbgs / 2)* (h - iconbgs / 2) + (w- iconbgs / 2)* (w - iconbgs / 2) ) > (iconbgs * iconbgs / 4 );

                    if (offt + 3 > maxoff)
                        continue;

                    rgbValues[offt + 0] = isWhite ? rgbValues[offt + 0] : (byte)((rgbValues[offt + 0]>>2) + 192);
                    rgbValues[offt + 1] = isWhite ? rgbValues[offt + 1] : (byte)((rgbValues[offt + 1]>>2) + 192);
                    rgbValues[offt + 2] = isWhite ? rgbValues[offt + 2] : (byte)((rgbValues[offt + 2]>>2) + 192);
                    rgbValues[offt + 3] = isWhite ? rgbValues[offt + 3] : (byte)(254);
                }



            int yoff = iconbgs/2 - cit.Height / 2;
            for (int h = 0; h < cit.Height; h++)
                for (int w = 0; w < cit.Width; w++)
                {
                    int offt = (y + yoff+ h ) * Main.maxTilesX * 4 / scale + x * 4 + w * 4 - cit.Width / 2 * 4;
                    int imgof = h * cit.Width + w;

                    bool isWhite = (tc[imgof].A == 0); //;&& ( (h- cit.Height/2)* (h - cit.Height / 2) + (w-cit.Width/2)* (w - cit.Width / 2) ) > (cit.Width* cit.Height / 4 );

                    if (offt + 3 > maxoff)
                        continue;

                    rgbValues[offt + 0] = isWhite ? rgbValues[offt + 0] : (byte)((rgbValues[offt + 0] >> 2) + ((tc[imgof].B >> 1)+ (tc[imgof].B >> 2)));
                    rgbValues[offt + 1] = isWhite ? rgbValues[offt + 1] : (byte)((rgbValues[offt + 1] >> 2) + ((tc[imgof].G >> 1)+ (tc[imgof].G >> 2))); 
                    rgbValues[offt + 2] = isWhite ? rgbValues[offt + 2] : (byte)((rgbValues[offt + 2] >> 2) + ((tc[imgof].R >> 1)+ (tc[imgof].R >> 2))); 
                    rgbValues[offt + 3] = isWhite ? rgbValues[offt + 3] : (byte)254;// tc[imgof].A;
                }
            
            tc = null;
        }

        public void DrawCircle(ref byte[] rgbValues, Tuple<int, int> where, int scale, Color cc, int iconbgs = 17)
        {
            //const int iconbgs = 17;


            int x = where.Item1 / scale ;
            int y = where.Item2 / scale - iconbgs/2;//+1 removed


            int maxoff = rgbValues.Length - 1;


            for (int h = 0; h < iconbgs; h++)
                for (int w = 0; w < iconbgs; w++)
                {
                    int offt = (y + h) * Main.maxTilesX * 4 / scale + x * 4 + w * 4 - iconbgs / 2 * 4;
                    //int imgof = h * iconbgs + w;
                    float val = ((h - iconbgs / 2) * (h - iconbgs / 2) + (w - iconbgs / 2) * (w - iconbgs / 2));
                    bool isWhite = (val <= (iconbgs * iconbgs / 3) && val >= (iconbgs * iconbgs / 6));

                    //rgbValues[offt + 0] = isWhite ? (byte)(((cc.B >> 2) + (cc.B >> 1)) + (rgbValues[offt + 0] >> 2)) : rgbValues[offt + 0];
                    //rgbValues[offt + 1] = isWhite ? (byte)(((cc.G >> 2) + (cc.G >> 1)) + (rgbValues[offt + 0] >> 2)) : rgbValues[offt + 1];
                    //rgbValues[offt + 2] = isWhite ? (byte)(((cc.R >> 2) + (cc.R >> 1)) + (rgbValues[offt + 0] >> 2)) : rgbValues[offt + 2];
                    //rgbValues[offt + 3] = isWhite ? (byte)(((cc.A >> 2) + (cc.A >> 1)) + (rgbValues[offt + 0] >> 2)) : rgbValues[offt + 3];

                    if (offt + 3 > maxoff)
                        continue;

                    rgbValues[offt + 0] = isWhite ? (byte)cc.B : rgbValues[offt + 0];
                    rgbValues[offt + 1] = isWhite ? (byte)cc.G : rgbValues[offt + 1];
                    rgbValues[offt + 2] = isWhite ? (byte)cc.R : rgbValues[offt + 2];
                    rgbValues[offt + 3] = isWhite ? (byte)cc.A : rgbValues[offt + 3];
                }
                        
        }








        //##############################
        // legacy, need to update
        public struct generatorInfo
        {
            public int numPyrChance;
            public int numIsland;
            public int numTree;
            public int minTreeToMapMidDist;
            public int minPyramidToMapMidDist;
            public int minCloudToMapMidDist;
            public int numPyramids;

            public string copperOrTin;
            public string ironOrLead;
            public string silverOrTung;
            public string goldOrPlat;
            public string moonType;

            public List<Tuple<int, int, int>> pyramidPos;

            public generatorInfo(int value = -1)
            {
                numPyrChance = value;
                numIsland = value;
                numTree = value;
                minTreeToMapMidDist = value;
                minPyramidToMapMidDist = value;
                minCloudToMapMidDist = value;
                numPyramids = value;

                copperOrTin = "";
                ironOrLead = "";
                silverOrTung = "";
                goldOrPlat = "";
                moonType = "";
                pyramidPos = new List<Tuple<int, int, int>>();

            }

        }

        public static generatorInfo reviewWhatWasDone()
        {
            //was done in worldGen overwriting generation passes. For tmod do it afterwards to increase compatibility to other mods 
            //check how many pyramids, living trees and clouds exist

            generatorInfo genInfo = new generatorInfo();

            //tile saerch
            var treesPos = new HashSet<int>();
            var cloudPos = new HashSet<int>(); //change to list
            var pyramidPos = new List<Tuple<int,int,int>>();//x,y,xlast


            //todo: no border
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.rockLayer; y++) // changed to rocklayer
                {
                    var tile = Main.tile[x, y];

                    if (tile.active())
                    {

                        //Trees
                        if (tile.type == TileID.LivingWood)
                        {
                            //unique
                            int count = 0;
                            bool isTree = false;
                            for (int i = -2; i < 3; i++)
                                count += (Main.tile[x + i, y].type == TileID.LivingWood) ? 1 : 0;
                            isTree = count > 4;
                            count = 0;
                            for (int i = -1; i < 10; i++)
                                count += (Main.tile[x, y + i].type == TileID.LivingWood) ? 1 : 0;
                            isTree = isTree && (count > 10);

                            //already exists?
                            if (isTree == true)
                            {
                                int newPos = 0;
                                //TODO check map border
                                for (int i = -25; i < 26; i++)
                                {
                                    if (treesPos.Contains(x + i) && i != 0)
                                    {
                                        newPos = (x + i) - (int)Math.Ceiling(((float)i) / 4.0);
                                        treesPos.Remove(x + i);
                                        treesPos.Add(newPos);
                                    }
                                }
                                if (newPos == 0)
                                {
                                    treesPos.Add(x);
                                }
                            }

                            


                        }


                        //Clouds
                        else if (tile.type == TileID.Cloud)
                        {

                            int area = 120;
                            bool found = false;
                            for (int i = -area; i < area; i++)
                            {
                                if (cloudPos.Contains(x + i))
                                {
                                    found = true;
                                    if (Math.Abs(i) > 50)
                                    {
                                        cloudPos.Remove(x + i);
                                        int newPos = x + i - Math.Sign(i);
                                        cloudPos.Add(newPos);

                                        //Console.Write(newPos.ToString() + " ");


                                    }
                                    break;
                                }
                            }

                            if (!found) cloudPos.Add(x);

                        }

                        //Pyramids
                        else if (tile.type == TileID.SandstoneBrick || tile.wall == WallID.SandstoneBrick)
                        {
                            bool found = false;
                                                        
                            for (int i = 0; i < pyramidPos.Count; i++)
                            {
                                
                                int xdist = x-pyramidPos[i].Item1;
                                int xLastDist = x-pyramidPos[i].Item3;
                                if ( xdist < 15)
                                {
                                    found = true;
                                    //peak 
                                    if (pyramidPos[i].Item2 > y)
                                    {
                                        pyramidPos[i] = new Tuple<int,int,int>(x,y,x);
                                        break;
                                    }
                                    
                                }
                                if (xLastDist < 15 )
                                {
                                    found = true;
                                    if (pyramidPos[i].Item3 == x) break;
                                    pyramidPos[i] = new Tuple<int, int, int>(pyramidPos[i].Item1, pyramidPos[i].Item2, x);
                                    break;
                                }
                                
                            }

                            
                            if (!found) pyramidPos.Add(new Tuple<int, int, int>(x, y, x));

                            

                        }

                    }


                }
            }


            genInfo.numIsland = cloudPos.Count;
            genInfo.numTree = treesPos.Count;
            genInfo.numPyramids = pyramidPos.Count;
            genInfo.pyramidPos = pyramidPos;

            genInfo.minCloudToMapMidDist = 1337000;
            foreach (var cloudX in cloudPos)
            {
                int midDist = Math.Abs(Main.maxTilesX / 2 - cloudX);
                genInfo.minCloudToMapMidDist = Math.Min(genInfo.minCloudToMapMidDist, midDist);
            }

            genInfo.minTreeToMapMidDist = 1337000;
            foreach (var treeX in treesPos)
            {
                int midDist = Math.Abs(Main.maxTilesX / 2 - treeX);
                genInfo.minTreeToMapMidDist = Math.Min(genInfo.minTreeToMapMidDist, midDist);
            }
            
            genInfo.minPyramidToMapMidDist = 1337000;
            foreach (var pyramidXY in pyramidPos)
            {
                int midDist = Math.Abs(Main.maxTilesX / 2 - pyramidXY.Item1);
                genInfo.minPyramidToMapMidDist = Math.Min(genInfo.minPyramidToMapMidDist, midDist);
                
            }
            
            return genInfo;
        }

         

    }
}
