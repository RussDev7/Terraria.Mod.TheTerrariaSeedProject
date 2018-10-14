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

        public int stage = 0;
        public bool ended = false;
        public bool searchForSeed = false;
        public bool inSearch = false;
        public bool isInCreation = false;
        public bool gotoCreation = false;
        //int numPyr;

        //public int dungeonSide;
        ushort jungleHut;
        int numPyrChance;

        //int seed = 200369066;
        public int seed;
        public int lastSeed = 0;
        public int lastStage = 0;

        generatorInfo genInfo;
        ScoreWorld score;
        AcceptConditons acond;

                
        string startDate;
        int numSearched;
        int numPyramidChanceTrue, numPyramidChanceTrueComputed;
        int[] passedStage;
        int[] couldNotGenerateStage;
        int rareGenOnly;
        int rareGen;

        int paterCondsTrue;
        int paterScore;

        //string seedPath;

        int condsTrue;
        int rareMax;

        const int chanceSize = 20;
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
        string worldName = "";

        //todo somethime better
        string looking4copperTin = "";
        string looking4ironLead = "";
        string looking4silverTung = "";
        string looking4goldPlation = "";
        string looking4moonType = "";
        string looking4hallowSide = "";


        string mapSize = "0";

        bool search4MoonOres = false;
        int localnumPyr = 0;
        int localDungeonSide = 0;
        int storeMMImg = -1;
        int storeStats = -1;
        int stepsize = 1;

        public override void Initialize()
        {
            //TODO all in one, else endless stacks, edit: ????????


            if (Main.menuMode != 10) { stage = 0; return; }


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


                if (!callingFun.Equals("do_worldGenCallBack")) return;

                if (!System.IO.Directory.Exists(Main.SavePath + OptionsDict.Paths.configPath))
                    System.IO.Directory.CreateDirectory(Main.SavePath + OptionsDict.Paths.configPath);


                //stuff only need to done once
                acond = new AcceptConditons();

                worldName = Main.worldName;
                string seedText = Main.ActiveWorldFileData.SeedText;

                //do seed search?
                if (!((worldName.Length > 0 && worldName[0] == '?') || (seedText.Length > 0 && seedText[0] == '?')))
                    return;
                if (worldName.Length > 0 && worldName[0] == '?')
                    worldName = worldName.Substring(1, worldName.Length - 1);
                Main.worldName = worldName.Length == 0 ? "SeedSearch" : worldName;

                gotToMain = false;

                nameToID = new Dictionary<string, int>();

                seed = ParseAndSetSeed(Main.ActiveWorldFileData.SeedText);


                InitSearch();



                score = new ScoreWorld(); score.init();
                genProg = new GenerationProgress();
                uiss = new UISearchSettings(genProg, mod, this);


                stage++;


                bool continueEval = false;
                statsUpdated = false;
                int numSeedSearch = 1000 * 1000;
                int numStopSearch = numSeedSearch;
                stepsize = 1;

                while (stage > 0 && !ended && !gotToMain)
                {
                    {   // <-- without this linux\mono need more memory

                        checkButtons();
                        if (gotToMain) { Exit(); break; };

                        if (numSeedSearch-- <= 0) {  goToOptions(); }
                        else if (numStopSearch <= 0) {  goToOptions(); }
                    
                        if (gotoCreation) stage = 1;
                        if (gotoCreation) StoreLastStats();

                        if (stage == 1 && !ended && searchForSeed == false)
                        {


                            //setup world
                            Main.worldName = uiss.infopanel.Search4ElementWithHeaderName(OptionsDict.WorldInformation.worldName).GetValue();

                            Selectable seedIbox = uiss.infopanel.Search4ElementWithHeaderName(OptionsDict.Configuration.startingSeed);
                            seedIbox.SetValue(seed.ToString());

                            //avoid cureent vanilla bug
                            BugAvoidCloudChest(true);
                            if (!TryToGenerate()) continue;

                            //TODO set up config values, still needed?

                            seed = ParseAndSetSeed(currentConfiguration.FindConfigItemValue(OptionsDict.Configuration.startingSeed, 0));


                            worldName = currentConfiguration.FindConfigItemValue(OptionsDict.WorldInformation.worldName, 0);
                            Main.worldName = worldName;
                            mapSize = currentConfiguration.FindConfigItemValue(OptionsDict.WorldInformation.worldSize, 0); //todo custom sizes

                            if (mapSize.Contains("x"))
                            {
                                int xsi, ysi;
                                Int32.TryParse(mapSize.Substring(0, mapSize.IndexOf('x')), out xsi);
                                Int32.TryParse(mapSize.Substring(mapSize.IndexOf('x')+1, mapSize.Length- mapSize.IndexOf('x')-1), out ysi);

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



                            search4MoonOres = true;
                            if (looking4copperTin.Equals("Random") && looking4ironLead.Equals("Random") && looking4silverTung.Equals("Random") && looking4goldPlation.Equals("Random") && looking4moonType.Equals("Random") && looking4hallowSide.Equals("Random"))
                                search4MoonOres = false;

                            //avoid cureent vanilla bug
                            BugAvoidCloudChest();

                            WorldGen.setWorldSize();
                            string evilType = currentConfiguration.FindConfigItemValue(OptionsDict.WorldInformation.evilType, 0);
                            WorldGen.WorldGenParam_Evil = evilType.Equals("Corruption") ? 0 : evilType.Equals("Crimson") ? 1 : -1;
                            string diffi = currentConfiguration.FindConfigItemValue(OptionsDict.WorldInformation.difficulty, 0);
                            Main.expertMode = diffi.Equals("Normal") ? false : true;
                            continueEval = !currentConfiguration.FindConfigItemValue(OptionsDict.Phase3.continueEvaluation, 3).Equals(OptionsDict.Phase3.continueEvaluationResetTag);

                            numSeedSearch = currentConfiguration.numSeedSearch;
                            numStopSearch = Int32.Parse(currentConfiguration.FindConfigItemValue(OptionsDict.Configuration.stopSearchNum, 0));
                            stepsize = Int32.Parse(currentConfiguration.FindConfigItemValue(OptionsDict.Configuration.stepSize, 0));

                            InitSearch();
                            Main.ActiveWorldFileData.SetSeed(seed.ToString());

                            clearWorld(stage);
                            continue;

                        }



                        if (stage == 1 && !ended)
                        {
                            //write some stats
                            paterCondsTrue = 0;
                            paterScore = -10000000;
                            statsUpdated = false;
                            score.clear(); score.init();

                            double ratio = numPyramidChanceTrue == 0 ? 0 : ((float)numSearched) / ((float)numPyramidChanceTrue);

                            
                            //check if there is the chance the maps has n pyramids

                            Main.ActiveWorldFileData.SetSeed(seed.ToString());


                            bool oreMoon = true;
                            if (search4MoonOres)
                            {
                                stage = 23;
                                TryToGenerate(); //--> genInfo ores
                                stage = stage == 23 ? 1 : stage;

                                oreMoon = CheckOresMoon(genInfo);

                                if (oreMoon)
                                {
                                    //check hardmode evil biome side
                                    //that might not work in later terraria versions!!!!
                                    UnifiedRandom dummy = new UnifiedRandom(seed);
                                    int a = dummy.Next(200, 300);
                                    a += dummy.Next(300, 400);
                                    bool evilIsLeft = dummy.Next(2) == 0 && a > 0;

                                    if ((localDungeonSide < 0 && evilIsLeft) || (localDungeonSide > 0 && !evilIsLeft))
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
                                }

                            }

                            if (oreMoon && stage == 1)
                            {
                                numPyramidChanceTrueComputed++;
                                genInfo = new generatorInfo();

                                if (!TryToGenerate()) continue; //--> genInfo                                              

                                //genInfo.numPyrChance = numPyr;
                                //numPyrChance = numPyr;

                                genInfo.numPyrChance = localnumPyr;
                                numPyrChance = localnumPyr;



                                //chanceCount[numPyr] += 1;
                                chanceCount[localnumPyr] += 1;
                                uiss.ChangeCountText(hasCount, chanceCount, numPyrChance);

                                score.insertGenInfo(genInfo);
                                condsTrue = acond.checkConditions(score, currentConfiguration, stage);

                                //oreMoon = CheckOresMoon(genInfo);
                            }
                            else
                                condsTrue = 0;

                            rareMax = 0;
                            //writeDebugFile(" geninfo " + genInfo.numPyrChance + " " + genInfo.numPyramids + " for seed " + Main.ActiveWorldFileData.Seed + " cond tru " + condsTrue );


                            //clearWorld(stage);


                            lastSeed = seed; lastStage = 1;

                            if (condsTrue > 0 && oreMoon)
                            {
                                passedStage[stage]++;
                                stage = (condsTrue == 3 ? 42 : condsTrue + 1); // goto 2 if no conditions set in phase 1, or got 3 if also no set in 2
                                if (stage > 2) passedStage[2]++;
                                if (stage > 3) passedStage[3]++;

                                if (stage > 0 && !ended && !gotoCreation)
                                {
                                    int newStage = stage;
                                    //for some reason i need to do this with not stage 1 to reset, else i get other world for e.g. 678683, small expert, vanilla has NearES, this has not, if I generate it with stage 1 or don't generated at all
                                    //happen if I go from 1 to 42, if i build stage 2 it works too // ESS and jungel tres still rotate
                                    stage = 23;
                                    TryToGenerate();
                                    stage = stage == 23 ? newStage : stage;
                                }

                            }
                            else
                                startNextSeed();

                            //writeToDescList(seed +" left stage 1 next stage " + stage + " construe " + condsTrue);// that take too much time, 20% faster without
                        }

                        if (stage == 2 && !ended && !gotoCreation)
                        {

                            writeToDescList(GenerateSeedStateText(), 1);


                            //check how many Pyramids world actually has and how many living trees, and which items they contain and how far away from mid map

                            if (!TryToGenerate()) continue; //--> genInfo

                            //writeDebugFile(" seed " + seed + " chance " + numPyrChance + "(" + genInfo.numPyrChance + ") has pyramid " + genInfo.numPyramids);

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
                                if (condsTrue > 2 || score.rare > 0)
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


                            //writeToDescList(seed+" left stage 2 next stage " + stage + " construe " + condsTrue);
                            //writeStatsOld();
                        }

                        bool tryAgain = false;
                        bool skipStage3 = true;
                        if (stage == 3 && !ended && !gotoCreation)
                        {
                            skipStage3 = false;
                            writeToDescList(GenerateSeedStateText(), 1);

                            //check again to be sure
                            //clearWorld(stage);
                            if (!TryToGenerate()) continue; //--> genInfo
                            analyzeWorld(score, genInfo);
                            paterScore = computeScore(score);
                            paterCondsTrue = acond.checkConditions(score, currentConfiguration, stage);

                            rareMax = Math.Max(score.rare, rareMax);

                                                        

                            //if world gen messed around again
                            tryAgain = false;
                            int trials = 2;
                            while ((paterCondsTrue < condsTrue || score.rare < rareMax) && trials-- > 0)
                            {
                                tryAgain = true;
                                //clearWorld(stage);
                                if (!TryToGenerate()) continue; //--> genInfo
                                analyzeWorld(score, genInfo);

                                paterCondsTrue = Math.Max(acond.checkConditions(score, currentConfiguration, stage), paterCondsTrue);
                                rareMax = Math.Max(score.rare, rareMax);
                                paterCondsTrue = Math.Max(acond.checkConditions(score, currentConfiguration, stage), paterCondsTrue); // to check if score > value
                                paterScore = Math.Max(computeScore(score), paterScore);

                                
                                //writeDebugFile("3run " +  seed + " " + trials + " cond " + paterCondsTrue + "(" + condsTrue + ")" + " rare " + score.rare + "(" + rareMax + ")");
                            }


                            condsTrue = paterCondsTrue;

                            lastSeed = seed; lastStage = 3;

                            if (!statsUpdated)
                            {
                                numPyramidChanceTrue++;
                                hasCount[numPyrChance, score.pyramids] += 1;
                                chanceCount[numPyrChance] -= 1;
                                uiss.ChangeCountText(hasCount, chanceCount, numPyrChance, score.pyramids);
                                statsUpdated = true;
                            }

                            writeToDescList(GenerateStatsText(), -2);

                            //if (condsTrue > 1) { Main.PlaySound(8, -1, -1, 1, 1f, 0f); }
                            if (condsTrue > 2 || score.rare > 0) { passedStage[stage]++; stage = 42; }
                            else
                            {
                                if (storeMMImg == 0 || storeMMImg == 1)
                                {
                                    createMapName(score, !tryAgain && trials < 2, currentConfiguration, worldName);
                                    StoreMapAsPNG(storeMMImg % 2 > 0);
                                }
                                if (storeStats == 0)
                                    StoreLastStats(true);

                                startNextSeed();
                            }


                            //writeToDescList(seed+" left stage 3 next stage " + stage +" construe " + condsTrue);

                        }


                        if (stage == 42 && !ended && !gotoCreation)
                        {
                            int repeat = 1;
                            bool repeating = false;
                            while (repeat-- > 0 )
                            {

                                //generate and write map file
                                writeToDescList(GenerateSeedStateText(), 1);

                                //due to vanilla world generation is buged and sometimes generates various maps with same seed, generate more than one until good

                                paterCondsTrue = condsTrue;
                                //###check if good map, else redo generation again   
                                int scoreVal;
                                //create again if score less     
                                int MAX_Runs = 2;
                                int runs = 0;
                                bool somethingwentwrong = false;



                                if (tryAgain || skipStage3 || repeating)
                                    do
                                    {
                                        if (runs > 10 * MAX_Runs) { break; }
                                        if (((runs) / MAX_Runs) % 4 > 1 && runs % 4 == 1)
                                        {
                                            //try other start conditions                                        
                                            writeDebugFile("mix up WG: " + seed + " trials " + runs + " score " + "(" + paterScore + ")" + " cond " + "(" + paterCondsTrue + ")" + " rare " + "(" + rareMax + ")");
                                            stage = 2;
                                            Main.ActiveWorldFileData.SetSeed((seed - 1).ToString());
                                            if (!TryToGenerate()) { somethingwentwrong = true; break; ; } //--> genInfo
                                            Main.ActiveWorldFileData.SetSeed((seed).ToString());
                                            stage = stage == 2 ? 42 : stage;
                                        }


                                        if (!TryToGenerate()) { somethingwentwrong = true; break; ; } //--> genInfo

                                        analyzeWorld(score, genInfo);
                                        scoreVal = computeScore(score);
                                        condsTrue = acond.checkConditions(score, currentConfiguration, 3);
                                        //writeDebugFile(" run " + runs + " constru " + condsTrue + " pater true" + paterCondsTrue);

                                        if (!repeating)
                                        {
                                            if (runs >= MAX_Runs && condsTrue >= paterCondsTrue && score.rare >= rareMax) paterScore = (int)((paterScore > 0 ? 0.97f : 1.031f) * ((float)paterScore) - 50.0f);
                                            if (runs >= 4 * MAX_Runs)
                                            {
                                                if (!(score.rare < rareMax))
                                                    paterCondsTrue--;
                                                else if (!(condsTrue < paterCondsTrue))
                                                    rareMax--;
                                            }
                                        }

                                        //rare condition if phase 2 did pass and had rare, nothing in phase 3. And here for some reason does not get condition true again.

                                        //writeDebugFile("B:run " + seed + " " + runs + " score " + scoreVal + "(" + paterScore + ")" + " cond " + condsTrue + "(" + paterCondsTrue + ")" + " rare " + score.rare + "(" + rareMax + ")");

                                        runs++;
                                    } while ((((scoreVal < paterScore) || (condsTrue < paterCondsTrue)) || (score.rare < rareMax)) && !ended && !repeating); //add break for rare bool "wasrare?"
                                                                                                                                               //&& !acond.takeAll

                                if (!somethingwentwrong && !statsUpdated && !repeating )
                                {
                                    numPyramidChanceTrue++;
                                    hasCount[numPyrChance, score.pyramids] += 1;
                                    chanceCount[numPyrChance] -= 1;
                                    uiss.ChangeCountText(hasCount, chanceCount, numPyrChance, score.pyramids);
                                    statsUpdated = true;
                                }
                                lastSeed = seed; lastStage = 42;
                                if (!somethingwentwrong && (score.rare > 0 || condsTrue > 2 || repeating))
                                {
                                    createMapName(score, !tryAgain && runs < 2, currentConfiguration, worldName);

                                    WorldFile.saveWorld(false, true);//Main.ActiveWorldFileData.IsCloudSave = false

                                    bool generated = true;

                                    if (score.hasOBjectOrParam["Chest Doub Glitch"] > 0)
                                    {
                                        //sometimes a chest does not get saved

                                        WorldFile.loadWorld(false);
                                        PostWorldGen();


                                        analyzeWorld(score, genInfo);
                                        computeScore(score);
                                        condsTrue = acond.checkConditions(score, currentConfiguration, 3);



                                        if ((score.rare > 0 || (condsTrue > 2 || repeating)))
                                        {
                                            //delete old write new,
                                            FileUtilities.Delete(Main.worldPathName, false);
                                            string name = Main.worldPathName.Substring(0, Main.worldPathName.Length - 3) + "twld";
                                            FileUtilities.Delete(name, false);
                                            createMapName(score, !tryAgain && runs < 2, currentConfiguration, worldName);
                                            WorldFile.saveWorld(false, true);
                                        }
                                        //else
                                        //delete wrong map sed?
                                    }

                                    if (generated)
                                    {
                                        if (!repeating)
                                        {
                                            Main.PlaySound(41, -1, -1, 1, 0.7f, 0f);

                                            passedStage[4]++;

                                            if (condsTrue > 2)
                                                numStopSearch--;

                                            if (score.rare > 0) rareGen++;
                                            if (score.rare > 0 && condsTrue < 3) rareGenOnly++;
                                        }


                                        //foreach (var elem in score.hasOBjectOrParam)
                                        //    wrtei += elem.Key + ": " + elem.Value + Environment.NewLine;
                                        if (storeMMImg >= 0) StoreMapAsPNG(storeMMImg % 2 > 0);

                                        if (storeStats >= 0) StoreLastStats(true);

                                        writeToDescList(GenerateStatsText(), -2);

                                        repeating = true;

                                    }
                                    


                                }


                                //start again with next seed
                                clearWorld(stage);
                                

                            }//end while repeat
                            startNextSeed();
                        }
                        


                    }//without this linux needs more memory

                }
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


        private void BugAvoidCloudChest(bool before=false)
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
            if (uiss.writeText && stats1text2bothNeg!=1) return;
            //uiss.writeTextUpdating = true;

            if (stats1text2bothNeg == 1 || stats1text2bothNeg == -1)            
                uiss.writtenStats = wrtei;
            if(stats1text2bothNeg == 2 || stats1text2bothNeg == -2)            
                uiss.writtenText = wrtei;

            if (stats1text2bothNeg == 1 || stats1text2bothNeg < 0)            
                uiss.writeStats = true;
            if (stats1text2bothNeg == 2 || stats1text2bothNeg < 0)
                uiss.writeText = true;
                   

        }

        private int ParseAndSetSeed(string seedText)
        {

            
            //is million given?
            //string st = Main.ActiveWorldFileData.SeedText;

            if (seedText.Length > 0 && seedText[0] == '?' )
                seedText = seedText.Substring(1, seedText.Length - 1);
            Main.ActiveWorldFileData.SetSeed(seedText);
            int seed = Main.ActiveWorldFileData.Seed;

            int tseed = 0;
            if (seedText.Length < 6 && (seedText.EndsWith("m") || seedText.EndsWith("M") || seedText.EndsWith(",") || seedText.EndsWith(".")))
            {
                if (Int32.TryParse(seedText.Substring(0, seedText.Length - 1), out tseed))
                {
                    if(seedText.EndsWith(",") || seedText.EndsWith("."))
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
        public void goToOptions(bool wait=false)
        {
            if (!ended && stage>=0 && !gotoCreation)
            {
                gotoptSeed = seed;
                gotoptStage = stage;                                
                gotoCreation = true;
                searchForSeed = false;
                uiss.writeText = true;
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
            
            /*
            //trial to avoid freezing worlds, do not work
            //stopWatchWorldGen = new Stopwatch();
            //stopWatchWorldGen.Start();
            clearWorld(stage);

            while (uiss.writeTextUpdating == true || uiss.rephrasing || uiss.detailsList.isUpdating || uiss.writeStats || uiss.writeText) { Thread.Sleep(10); };
            inSearch = true;

            Thread newThread = null;
            try
            {

                newThread = new Thread(new ThreadStart(StartWGT));
                newThread.Start();
                int wait = 500;
                while ((newThread.IsAlive && wait-->0 )|| isInCreation || gotoCreation)
                {
                    Thread.Sleep(100);
                }
                if (newThread.IsAlive)
                {
                    writeDebugFile(" freezing seed detected " + seed + " isInCreation " + isInCreation + " gotoptSeed " + gotoptSeed + " gotoptStage " + gotoptStage + " gotoCreation " + gotoCreation + " searchForSeed " + searchForSeed + " inSearch " + inSearch);
                    clearWorld(stage);
                    
                    newThread.Abort("Information from Main.");
                    //writeDebugFile("abort send");
                    throw new Exception();
                    
                }

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
                    writeDebugFile(" could not build seed " + seed + " with size " + Main.maxTilesX + " and evil type " + WorldGen.WorldGenParam_Evil + " expert mode " + Main.expertMode + " in stage " + stage + " talive " + (newThread!=null?newThread.IsAlive:false)) ;
                    writeDebugFile(ex.Message);
                    writeDebugFile(ex.ToString());
                    writeDebugFile("stage " + stage);

                    //todod remove from stats
                    couldNotGenerateStage[stage == 42 ? 4 : stage == 23 ? 1 : stage]++;

                    if((newThread != null ? newThread.IsAlive : false))
                    {
                        //after freeze next worlf might be differnt, do dummy world
                        
                        int cstage = stage;
                        int cseed = seed;
                        stage = 23;
                        Main.ActiveWorldFileData.SetSeed("0");
                        //clearWorld(stage);
                        //StartWGT();
                        Main.ActiveWorldFileData.SetSeed(cseed.ToString());
                        stage = stage == 23 ? cstage : stage;

                    }

                    if (statsUpdated)
                    {
                        hasCount[numPyrChance, score.pyramids] -= 1;

                    }
                    else
                    {
                        chanceCount[numPyrChance] -= 1;
                    }

                    uiss.SetCountText(hasCount, chanceCount);

                    startNextSeed();
                }
                inSearch = false;
                return false;
            }

            inSearch = false;
            return true;
            */

            
            //stopWatchWorldGen = new Stopwatch();
            //stopWatchWorldGen.Start();
            clearWorld(stage);
                       
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
                    writeDebugFile(" could not build seed " + seed + " with size " + Main.maxTilesX + " and evil type " + WorldGen.WorldGenParam_Evil + " expert mode " + Main.expertMode + " in stage " + stage) ;
                    writeDebugFile(ex.Message );                    
                    writeDebugFile(ex.ToString());
                    writeDebugFile("stage " + stage);

                    //todod remove from stats
                    couldNotGenerateStage[stage==42?4:stage]++;

                    if (statsUpdated)
                    {
                        hasCount[numPyrChance, score.pyramids] -= 1;
                        
                    }
                    else
                    {
                        chanceCount[numPyrChance] -= 1;                        
                    }
                    
                    uiss.SetCountText(hasCount, chanceCount);

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

        }

        //int seedCount = 0;
        private void startNextSeed()
        {
                        
            stage = 1;
            seed+=stepsize;            
            numSearched++;            
            if (seed == Int32.MinValue) seed = 0; //else vanilla crashes here            
            if (seed < 0) seed = Int32.MaxValue+seed+1;            

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
            stats += System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + "out of " + (numSearched-ng).ToString().PadLeft(6, '_') + " total";

            //System.IO.File.WriteAllText(OptionsDict.Paths.debugPath + "_stats.txt", stats);//////////////////// TODO
            return stats;
        }

        private string GenerateSeedStateText()
        {
            TimeSpan ts = runTime.Elapsed;
            string times = "";
            if (ts.TotalDays > 1 || ts.TotalHours > 1)
                times = Math.Round(ts.TotalHours) + "h";
            else if(ts.TotalMinutes > 1)
                times = Math.Round(ts.TotalMinutes) + "min";
            else if (ts.TotalSeconds > 1)
                times = Math.Round(ts.TotalSeconds) + "s";
            else
                times = Math.Round(ts.TotalMilliseconds) + "ms";

            string sss = "seed " + seed + (stage==42? " builing phase": (" phase " + stage)) + " time: " + times;
            return sss;
        }

        string lastScoreText = "";
        private string GenerateStatsText( bool doFull = false, bool doLog = false)
        {
          int seed_ = seed;
          int stage_ = stage;

            if (gotoCreation || numSearched == 0)
            {                
                seed_ = lastSeed;//todo test if also work for gotCreation
                stage_ = lastStage;
            }



            string stats = "";
            int didNotfinished = doFull == true  ? 0 : 1;

            stats += "Last eval. seed: " + seed_ + (stage_ > 1 && acond.rares > 0? (" Rares: " + acond.rares) : "") + (stage_ > 2? (" Score: "+ score.score):"")+Environment.NewLine;
            TimeSpan rt = runTime.Elapsed;
            stats += "Runtime: " + ((int)rt.TotalHours).ToString().PadLeft(2,'0') + ":" + rt.Minutes.ToString().PadLeft(2, '0') + ":" + rt.Seconds.ToString().PadLeft(2, '0') + Environment.NewLine;
                        
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
            stats += "Failed to generate: " + numErr + " (" + couldNotGenerateStage[1] + ", " + couldNotGenerateStage[2] + ", " + couldNotGenerateStage[3] + ", " + couldNotGenerateStage[4] + ")"+ Environment.NewLine+ Environment.NewLine;

            stats += "Distribution: Pyramid chance with how many it has (out of "+ (numPyramidChanceTrueComputed - numErr) + "). " + Environment.NewLine; 
            stats += uiss.countText;
            stats += Environment.NewLine+"(in short form: e.g. '42' means at least 2 with 4 times zero, so over 20000 times.)" + Environment.NewLine;

            if (doFull)
            {
                stats += Environment.NewLine+ Environment.NewLine + "Extended Distribution, normal numbers:";
                stats += Environment.NewLine +getFullDistrString();
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
            if(lastScoreText.Length>0)
                stats += Environment.NewLine+ Environment.NewLine;
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
            wso += Environment.NewLine+"Last stats: " + Environment.NewLine;

            string fullStats = GenerateStatsText(true, storeLog);

            wso += Environment.NewLine + fullStats;
            if(!storeLog)
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
            throw new Exception("TerrariaSeadSearch stopped by user (don't care about this)\n");
        }

        int maxTilesYOld = 0;
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            if (ended || gotToMain)
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
            } else
             if (stage > 0 || gotoCreation || isInCreation)
            {
                int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Reset"));
                if (genIndex != -1)
                {
                    tasks.Insert(genIndex, new PassLegacy("Setup seed options", delegate (GenerationProgress progress)
                    {
                        if(!searchForSeed)
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

                    if (ended)
                    {
                        //other tasks not needed now
                        tasks.RemoveRange(genIndex + 1, tasks.Count - genIndex - 1);
                    }

                }
            }

                



            //lose 1 % speed if added before
            if (stage > 0)
            {
                for (int i = tasks.Count - 1; i > 0; i--)
                {
                    int j = i;
                    tasks.Insert(i, new PassLegacy("stop check", delegate (GenerationProgress progress)
                    {

                        //TimeSpan ts = stopWatchWorldGen.Elapsed;
                        //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        //(int)(ts.TotalHours), ts.Minutes, ts.Seconds,
                        //ts.Milliseconds / 10);
                        //writeDebugFile(" worldgentime after step "+ j  +"  "+ tasks[(int)Math.Min(2*j, tasks.Count-1)].Name + " " + elapsedTime);


                        if (ended)
                        {
                            Exit();
                        }

                    }));
                }
            }



            //should be between 0 and 1
            if (stage == 23)
            {
                /*
                int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Reset"));//clouds work
                if (genIndex != -1)
                {                    
                    tasks.Insert(genIndex + 1, new PassLegacy("Lookup ore and moon type", delegate (GenerationProgress progress)
                    {
                        //progress.Message = "Reset WorldGen to increase chance for equal map with vanilla";
                        //also used to dertermine ores
                        progress.Message = "Lookup ore and moon type for seed " + seed;

                    }));

                    //other tasks not needed now
                    tasks.RemoveRange(genIndex + 2, tasks.Count - genIndex - 2);
                }*/                

                int resetIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Reset"));
                if (resetIndex != -1)
                {
                    //credits to jopojelly added some modified version:
                    tasks.Insert(resetIndex + 1, new PassLegacy("Lookup ore and moon type and dungeon side ", (progress) => DetectDungeonSide(progress, tasks[resetIndex] as PassLegacy)));

                    //other tasks not needed now
                    tasks.RemoveRange(resetIndex + 2, tasks.Count - resetIndex - 2);
                }
            }


            if (!searchForSeed && (stage > 0 || ended))
            {
                int resetIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Reset"));
                if (resetIndex != -1)
                {
                    //other tasks not needed now
                    tasks.Insert(resetIndex + 1, new PassLegacy("Setup configuration", delegate (GenerationProgress progress)
                    {
                        progress.Message = "Setup configuration";
                    }));                    
                    tasks.RemoveRange(resetIndex + 2, tasks.Count - resetIndex - 2);
                }
            }
            else if (stage == 1)
            {
                
                int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sand"));
                if (genIndex != -1)
                {
                    tasks.Insert(genIndex - 1, new PassLegacy("Reduce maxTilesY to skip tile Runner in Sand pass ", delegate (GenerationProgress progress)
                    {
                        progress.Message = "Analyze World for Pyramids and Living Trees";
                        //maxTilesYOld = Main.maxTilesY;
                        Main.maxTilesY = -Math.Abs(Main.maxTilesY);

                    }));
                }
                int SandIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sand"));
                if (SandIndex != -1)
                {
                    //credits to jopojelly for replacing vanilla source code with this:
                    tasks.Insert(SandIndex + 1, new PassLegacy("Lookup chance of Pyramids for seed ", (progress) => DetectNumPyr(progress, tasks[SandIndex] as PassLegacy)));
                   
                    //other tasks not needed now
                    tasks.RemoveRange(SandIndex + 2, tasks.Count - SandIndex - 2);
                }
            }
            else
            if (stage == 2)
            {
                int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Wood Tree Walls"));
                if (genIndex != -1)
                {
                    tasks.Insert(genIndex + 1, new PassLegacy("Analyze World basic", delegate (GenerationProgress progress)
                    {
                        progress.Message = "Analyze World for Pyramids and Living Trees";


                    }));

                    //other tasks not needed now
                    tasks.RemoveRange(genIndex + 2, tasks.Count - genIndex - 2);
                }
            }
            
            else if (stage == 3)
            {               
                
                if (tasks.Count > 0)
                {

                    tasks.Add(new PassLegacy("Analyze World", delegate (GenerationProgress progress)
                    {
                        progress.Message = "Analyze World for Jungle chest items and rare stuff";


                    }));                    
                }
            }




        }

        
        //from jopojelly to replace vanilla code
        private void DetectNumPyr(GenerationProgress progress, PassLegacy SandPass)
        {
            Main.maxTilesY = Math.Abs(-Main.maxTilesY);
            progress.Message = "Looking chance of Pyramids for seed "+ seed;
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
            if (stage > 0)
            {
                genInfo = new generatorInfo();
                genInfo.copperOrTin = WorldGen.CopperTierOre == 7 ? "Copper" : "Tin";
                genInfo.ironOrLead = WorldGen.IronTierOre == 6 ? "Iron" : "Lead";
                genInfo.silverOrTung = WorldGen.SilverTierOre == 9 ? "Silver" : "Tungsten";
                genInfo.goldOrPlat = WorldGen.GoldTierOre == 8 ? "Gold" : "Platin";
                genInfo.moonType = Main.moonType == 0 ? "White" : Main.moonType == 1 ? "Orange" : Main.moonType == 2 ? "Green" : "Random";                
            }
            if (stage > 1 && stage != 23)
            {                
                genInfo = reviewWhatWasDone(); //checks pyramid, lving tree, clouds and their distance
                genInfo.numPyrChance = numPyrChance;
            }

        }


        private void ComputePathlength(ref int[,] pathLength)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime;

            int maxsize = (Main.maxTilesX * 6 + Main.maxTilesY * 4); // values can be creater!
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



        private void analyzeWorld(ScoreWorld score, generatorInfo genInfo, bool doFull = true)
        {


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            score.clear(); score.init();

            Dictionary<string, int> hasOBjectOrParam = score.hasOBjectOrParam; //Todo only ref
            hasOBjectOrParam.Clear();

            score.insertGenInfo(genInfo);



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

            hasOBjectOrParam.Add("Near Tree", 0);

            hasOBjectOrParam.Add("Water Bolt before Skeletron", 0);
            hasOBjectOrParam.Add("Water Bolt", 0);

            hasOBjectOrParam.Add("Temple Distance", 0);
            hasOBjectOrParam.Add("Temple horizontal distance", 0);
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
            hasOBjectOrParam.Add("Pathlength to Iron/Lead Bar", 1000000);
            hasOBjectOrParam.Add("Pathlength to Gold/Platinum Bar", 1000000);

            hasOBjectOrParam.Add("Pathlength to Bomb", 1000000);
            hasOBjectOrParam.Add("Pathlength to Jester's Arrow", 1000000);

            hasOBjectOrParam.Add("Pathlength to Boots", 1000000);
            hasOBjectOrParam.Add("Pathlength to Enchanted Sword", 1000000);
            hasOBjectOrParam.Add("Pathlength to Bee Hive", 1000000);
            hasOBjectOrParam.Add("Pathlength to Temple Door", 1000000);
            hasOBjectOrParam.Add("Pathlength to free ShadowOrb/Heart", 1000000);
            hasOBjectOrParam.Add("Pathlength to Boomstick", 1000000);
            hasOBjectOrParam.Add("Pathlength to Suspicious Looking Eye", 1000000);
            hasOBjectOrParam.Add("Pathlength to Snowball Cannon", 1000000);

            hasOBjectOrParam.Add("Pathlength to Slime Staute", 1000000);
            hasOBjectOrParam.Add("Pathlength to Shark Staute", 1000000);            
            hasOBjectOrParam.Add("Pathlength to Anvil", 1000000);

            hasOBjectOrParam.Add("Pathlength into 40% cavern layer", 1000000);
            hasOBjectOrParam.Add("Pathlength to 40% cavern entrance", 1000000);
            hasOBjectOrParam.Add("Tiles to mine for 40% cavern", 1000000);

            hasOBjectOrParam.Add("Pathlength to cavern entrance to mid of Jungle", 1000000);
            hasOBjectOrParam.Add("Tiles to mine for mid Jungle cavern", 1000000);
            hasOBjectOrParam.Add("Pathlength to cavern entrance to deep Jungle", 1000000);
            hasOBjectOrParam.Add("Tiles to mine for deep Jungle cavern", 1000000);




            
            hasOBjectOrParam.Add("neg. Pathlength to Teleport Potion", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Iron/Lead Bar", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Gold/Platinum Bar", -1000000);

            hasOBjectOrParam.Add("neg. Pathlength to Bomb", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Jester's Arrow", -1000000);

            hasOBjectOrParam.Add("neg. Pathlength to Boots", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Enchanted Sword", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Bee Hive", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Temple Door", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to free ShadowOrb/Heart", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Boomstick", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Suspicious Looking Eye", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Snowball Cannon", -1000000);

            hasOBjectOrParam.Add("neg. Pathlength to Slime Staute", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Shark Staute", -1000000);
            hasOBjectOrParam.Add("neg. Pathlength to Anvil", -1000000);

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


            hasOBjectOrParam.Add("High Hive", 0);


            hasOBjectOrParam.Add("Spawn in Sky", 0);

            hasOBjectOrParam.Add("Open Temple", 0);


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



            hasOBjectOrParam.Add("Chest Doub Glitch", 0);



            hasOBjectOrParam.Add("Dungeon tiles above surface", 0);
            hasOBjectOrParam.Add("Dungeon far above surface", 0);
            hasOBjectOrParam.Add("Dungeon below ground", 0);
            hasOBjectOrParam.Add("Dungeon has good Pos", 0);
            hasOBjectOrParam.Add("Dungeon in Snow Biome", 0);
            hasOBjectOrParam.Add("Dungeon has strange Pos", 0);
            hasOBjectOrParam.Add("Floating island cabin in Dungeon", 0);
            hasOBjectOrParam.Add("Pre Skeletron Dungeon Chest Risky", 0);
            hasOBjectOrParam.Add("Pre Skeletron Dungeon Chest Grab", 0);
            hasOBjectOrParam.Add("Pre Skeletron Golden Key Risky", 0);
            hasOBjectOrParam.Add("Pre Skeletron Golden Key Grab", 0);

            hasOBjectOrParam.Add("Pre Skeletron Muramasa Chest reachable", 0);
            hasOBjectOrParam.Add("Pre Skeletron Cobalt Shield Chest reachable", 0);
            hasOBjectOrParam.Add("Pre Skeletron Shadow Key Chest reachable", 0);


            hasOBjectOrParam.Add("Alchemy Table", 0);
            hasOBjectOrParam.Add("Sharpening Station", 0);
            hasOBjectOrParam.Add("Dungeon farm spot", 0);
            hasOBjectOrParam.Add("Dungeon farm spot 3Wall inters.", 0);
            hasOBjectOrParam.Add("Dungeon farm spot 3Wall in line", 0);


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
            {
                int cury = blsY;
                int size = hasOBjectOrParam["Beach right"] - hasOBjectOrParam["Beach left"];
                int[] diff = new int[size];
                bool[] trueval = new bool[size];
                int startX = hasOBjectOrParam["Beach left"];
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
                    if (CheckIfSolidAndNoTree(curx, cury) || Main.tile[curx, cury].type == TileID.LivingWood || Main.tile[curx, cury].wall == WallID.LivingWood || Main.tile[curx, cury].type == TileID.LeafBlock || isInDungeon(curx, cury))
                        trueval[countx] = true;
                    else
                        trueval[countx] = false;

                    diff[countx] = (int)Main.worldSurface - cury;

                    //debug
                    //Main.tile[curx, cury].active(true);
                    //Main.tile[curx, cury].type = (ushort)(196 + (trueval[countx]==true? 1: 0));
                }
                //TODO merge to one function
                startX = hasOBjectOrParam["Beach right"];
                int[] diff2 = new int[size];
                bool[] trueval2 = new bool[size];



                for (int countx = 0; countx < size; countx++)
                {
                    int curx = startX - countx - 1;

                    ushort ttype = Main.tile[curx, cury].type;
                    if (ttype != TileID.LivingWood && Main.tile[curx, cury].wall != WallID.LivingWood && ttype != TileID.LeafBlock && !isInDungeon(curx, cury) && ttype != TileID.PinkDungeonBrick && ttype != TileID.GreenDungeonBrick && ttype != TileID.BlueDungeonBrick)
                    {                        
                        int i;
                        for(i = -maxv; i < maxv; i++) 
                            if (CheckIfSolidAndNoTree(curx, cury + i) || Main.tile[curx, cury + i].wall != WallID.None) { cury += i; break; }
                        if (i == maxv) cury += maxv;

                    }
                    if (CheckIfSolidAndNoTree(curx, cury) || Main.tile[curx, cury].type == TileID.LivingWood || Main.tile[curx, cury].wall == WallID.LivingWood || Main.tile[curx, cury].type == TileID.LeafBlock || isInDungeon(curx, cury))
                        trueval2[size - countx - 1] = true;
                    else
                        trueval2[size - countx - 1] = false;

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
                    //Main.tile[hasOBjectOrParam["Beach left"] + i, (int)Main.worldSurface - diff[i]].active(true);
                    //Main.tile[hasOBjectOrParam["Beach left"] + i, (int)Main.worldSurface - diff[i]].type = (ushort)(196 + (trueval[i] == true ? 1 : 0)+(trueval2[ i] == true ? 1 : 0));
                }

                diff = null;
                diff2 = null;
                trueval = null;
                trueval2 = null;

                mean /= (double)size;
                meanSq /= (double)size;

                hasOBjectOrParam["Surface average height (aprox.)"] = (int)Math.Round(mean);
                hasOBjectOrParam["Surface height (sqrt) variance"] = (int)Math.Round(Math.Sqrt(meanSq - mean * mean));
                hasOBjectOrParam["Surface max-min height"] = (max - min);


            }


            //pathfinding
            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            //writeDebugFile(" analyze time before pathfinding " + elapsedTime);

            
            int[,] pathLength = null;
            

            if (doFull)
            {
                ComputePathlength(ref pathLength);                

            }


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
                        (Main.tile[cx, cy - 1].active() && Main.tile[cx, cy - 1].type == TileID.ClosedDoor) ||
                        (Main.tile[cx + 1, cy - 1].active() && Main.tile[cx + 1, cy - 1].type == TileID.ClosedDoor) ||
                        !Main.tile[cx, cy + 2].active() || !Main.tile[cx + 1, cy + 2].active()
                        )
                    {

                        //hasOBjectOrParam["Chest Doub Glitch"] += 1; writeDebugFile("maybe found doubglitch (buggy) at " + cx + " " + cy + " in seed " + Main.ActiveWorldFileData.Seed + " can get overridden: " + (!doFull));
                        if (score.itemLocation.ContainsKey(ItemID.DynastyChest))
                            score.itemLocation[ItemID.DynastyChest].Add(new Tuple<int, int>(cx, cy));
                        else
                            score.itemLocation.Add(ItemID.DynastyChest, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });

                    }


                    //TODO Grab distance trough walls not included yet,, edit: still not?
                    ushort chestWall = Main.tile[cx, cy].wall;
                    if ( (Main.tile[cx, cy].frameX == 72 || (chest.item[0].type == ItemID.GoldenKey && Main.tile[cx, cy].frameX == 0)) && Main.tile[cx, cy].frameY == 0 && Main.tile[cx, cy].type == 21)
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
                            }
                            else if (Main.tile[cx, cy].frameX == 0 && chest.item[0].type == ItemID.GoldenKey && chest.y > Main.worldSurface + 2)
                            {
                                //excluding golden keys above surface, they are counted at other place
                                hasOBjectOrParam["Pre Skeletron Golden Key Grab"] += 1;
                                //writeDebugFile("########### can grab golden key at " + cx + " " + cy + " " + seed);
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
                            if(chest.item[0].type == ItemID.Muramasa)
                                hasOBjectOrParam["Pre Skeletron Muramasa Chest reachable"] = 1;
                            else if (chest.item[0].type == ItemID.CobaltShield)
                                hasOBjectOrParam["Pre Skeletron Cobalt Shield Chest reachable"] = 1;
                            else if (chest.item[0].type == ItemID.ShadowKey)
                                hasOBjectOrParam["Pre Skeletron Shadow Key Chest reachable"] = 1;
                        }


                    }

                    int pathl = doFull ? FindShortestPathInRange(ref pathLength, cx, cy) : 1000000;


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
                        else if (doFull && item.type == ItemID.TeleportationPotion && (!isInDungeon(cx, cy) || cy < Main.worldSurface + 2))
                        {
                            if (pathl < hasOBjectOrParam["Pathlength to Teleport Potion"])
                            {

                                hasOBjectOrParam["Pathlength to Teleport Potion"] = pathl;
                                hasOBjectOrParam["Nearest Teleportation Potion count"] = item.stack;

                                if (score.itemLocation.ContainsKey(ItemID.TeleportationPotion))
                                    score.itemLocation[ItemID.TeleportationPotion] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                else
                                    score.itemLocation.Add(ItemID.TeleportationPotion, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                            }

                        }
                        else if (doFull && (item.type == ItemID.IronBar || item.type == ItemID.LeadBar))
                        {
                            if (pathl < hasOBjectOrParam["Pathlength to Iron/Lead Bar"])
                            {

                                hasOBjectOrParam["Pathlength to Iron/Lead Bar"] = pathl;
                                if (score.itemLocation.ContainsKey(item.type))
                                    score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                else
                                    score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                            }

                        }
                        else if (doFull && (item.type == ItemID.GoldBar || item.type == ItemID.PlatinumBar))
                        {
                            if (pathl < hasOBjectOrParam["Pathlength to Gold/Platinum Bar"])
                            {
                                hasOBjectOrParam["Pathlength to Gold/Platinum Bar"] = pathl;
                                if (score.itemLocation.ContainsKey(item.type))
                                    score.itemLocation[item.type] = new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) };
                                else
                                    score.itemLocation.Add(item.type, new List<Tuple<int, int>> { new Tuple<int, int>(cx, cy) });
                            }

                        }
                        else if (doFull && (item.type == ItemID.SuspiciousLookingEye))
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
                        else if (doFull && (item.type == ItemID.SnowballCannon))
                        {
                            if (pathl < hasOBjectOrParam["Pathlength to Snowball Cannon"])
                            {
                                hasOBjectOrParam["Pathlength to Snowball Cannon"] = pathl;
                            }

                        }
                        else if (cy <= Main.worldSurface + 2 && item.type == ItemID.GoldenKey)
                        {
                            hasOBjectOrParam["Pre Skeletron Golden Key Grab"] += 1;
                        }
                        else if (doFull && item.type == ItemID.Boomstick)
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
                        else if (doFull && item.type == ItemID.Bomb)
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
                        else if (doFull && item.type == ItemID.JestersArrow)
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


                    }
                }
            }


            int goldenKeys = hasOBjectOrParam["Pre Skeletron Golden Key Grab"] + hasOBjectOrParam["Pre Skeletron Golden Key Risky"];

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

            int[] wallCounts = new int[256];

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

            List<Tuple<int, int, int, int>> dungeonFarmSpotCandidates = new List<Tuple<int, int, int, int>>(); // xmin xmax of inner wall, ymin ymax of detection




            //todo: no border
            for (int x = 10; x < Main.maxTilesX - 10; x++)
            {
                for (int y = 10; y < Main.maxTilesY - 10; y++)
                {
                    var tile = Main.tile[x, y];

                    if (!tile.active())
                        wallCounts[tile.wall] += 1;
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

                                        Console.Write(newPos.ToString() + " ");


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

                        //high hives
                        else if (tile.type == TileID.Larva && tile.frameX == 18 && tile.frameY == 18)
                        {


                            int pathl = FindShortestPathInRange(ref pathLength, x, y, 2, 2, 2, 2);
                            if (pathl < hasOBjectOrParam["Pathlength to Bee Hive"])
                                hasOBjectOrParam["Pathlength to Bee Hive"] = pathl;


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
                                    if (isInDungeon(xi, yi) && Main.tile[xi, yi].wall != tw1 && Main.tile[xi, yi].wall != tw2)
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
                                //farm spot line from above
                                const int rangeMin = 16; //16
                                const int rangeMax = 56;//56 ,42

                                bool doSearch = true;
                                const int ignoreX = rangeMin;
                                const int ignoreY = 1;//4  TODO: recode, atm only works for 1, if creater regions cant connect anymore with 100%
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
                                        if (Main.tile[x - i, y].wall != twl) { isGood = false; break; }
                                    if (isGood)
                                        for (int i = 2; i < rangeMin; i++)
                                            if (Main.tile[x + i, y].wall != twr) { isGood = false; break; }
                                    if (isGood)
                                    {
                                        //search for 3rd wall type in line
                                        int start3 = -1;
                                        int ndw = 0;
                                        const int maxNotDung = 5;
                                        for (int i = rangeMin + 1; i < rangeMax; i++)
                                        {
                                            bool isGood3 = false;
                                            if (isInDungeon(x + i, +y))
                                            {
                                                if (Main.tile[x + i, y].wall != twl && Main.tile[x + i, y].wall != twr)
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
                                            if (((isInDungeon(x - rangeMin, y - npcSpawnRange) && isInDungeon(x - rangeMin, y - npcSpawnRange - 1) && isInDungeon(x - rangeMin, y - npcSpawnRange - 2))
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
                            if ((tile.type == (ushort)187) && (tile.frameX == (short)918) && (tile.frameY == (short)0))
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

                                if (!inSand && checkIfNearSpawn(x, y, 350, 200) && pathl < 700)
                                {
                                    hasOBjectOrParam["Near Enchanted Sword"] += 1;
                                }
                                if (checkIfNearTree(x, y, 80, 50) && tile.wall != 68)
                                {
                                    hasOBjectOrParam["Enchanted Sword near Tree"] += 1;

                                    if (!inSand && checkIfNearSpawn(x, y, 350, 2000) && pathl < 1050)
                                    {
                                        hasOBjectOrParam["Near Enchanted Sword near Tree"] += 1;
                                    }
                                }
                                if (checkIfNearPyramid(x, y, 100, 50) && tile.wall != 68)
                                {
                                    hasOBjectOrParam["Enchanted Sword near Pyramid"] += 1;
                                    if (!inSand && checkIfNearSpawn(x, y, 400, 2000) && pathl < 1200)
                                    {
                                        hasOBjectOrParam["Near Enchanted Sword near Pyramid"] += 1;
                                    }

                                }



                                if (pathl < hasOBjectOrParam["Pathlength to Enchanted Sword"])
                                {
                                    hasOBjectOrParam["Pathlength to Enchanted Sword"] = pathl;
                                }




                            }
                            //todo sort by frequenzy

                            //Temple Door
                            if (tile.type == TileID.ClosedDoor && tile.frameY == 612)
                            {
                                hasOBjectOrParam["Temple Distance"] = getDistanceToSpawn(x, y);
                                hasOBjectOrParam["Temple horizontal distance"] = Math.Abs(x - Main.spawnTileX);

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
                                    hasOBjectOrParam["Open Temple"] = 1;
                                }

                            }

                            //Altar
                            else if (tile.type == TileID.DemonAltar && (tile.frameY == (short)0) && (tile.frameX == (short)18 || tile.frameX == (short)72))
                            {
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
                                    if (score.itemLocation.ContainsKey(ItemID.SlimeCrown))
                                        score.itemLocation[ItemID.SlimeCrown] = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) };
                                    else
                                        score.itemLocation.Add(ItemID.SlimeCrown, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });
                                }
                                else if (fx == 1800 && fy == 0 && pathLength[sx, sy] < hasOBjectOrParam["Pathlength to Shark Staute"])
                                {
                                    hasOBjectOrParam["Pathlength to Shark Staute"] = pathLength[sx, sy];
                                    if (score.itemLocation.ContainsKey(ItemID.Minishark))
                                        score.itemLocation[ItemID.Minishark] = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) };
                                    else
                                        score.itemLocation.Add(ItemID.Minishark, new List<Tuple<int, int>> { new Tuple<int, int>(x, y) });
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
                            else if (tile.type == TileID.Detonator && tile.frameX == 0 && tile.frameX == 0)
                            {
                                hasOBjectOrParam["Detonator"]++;
                            }
                            //Mushroom biome
                            else if (tile.type == TileID.MushroomGrass || tile.type == TileID.MushroomPlants || tile.type == TileID.MushroomTrees)
                            {
                                if (checkIfNearSpawn(x, y, 150, 400))
                                {
                                    hasOBjectOrParam["Near Mushroom Biome count"] += 1;
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

                            } else if(tile.type == TileID.Anvils && tile.frameY == 0 && (tile.frameX == 0 || tile.frameX == 36))
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

            //pictures
            hasOBjectOrParam["Number different Paintings"] = paintingsCount.Count;
            int totalP = 0;
            foreach (var pic in paintingsCount) totalP += pic.Value / 72;
            hasOBjectOrParam["Number Paintings"] = totalP;

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


            //half double count
            hasOBjectOrParam["Near Chest"] = hasOBjectOrParam["Near Chest"] / 2; // todo refin

            //count total clouds
            hasOBjectOrParam["Cloud Chest"] = has(ref hasOBjectOrParam, ItemID.ShinyRedBalloon) + has(ref hasOBjectOrParam, ItemID.Starfury) + has(ref hasOBjectOrParam, ItemID.LuckyHorseshoe);

            //good trees
            hasOBjectOrParam["Tree Chest"] = has(ref hasOBjectOrParam, ItemID.LivingWoodWand); //same 

            hasOBjectOrParam["Tree"] = treesPos.Count;



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
            for (int x = Main.dungeonX - 50; x < Main.dungeonX; x++)
                for (int y = Main.dungeonY - 50; y < Main.dungeonY - 40; y++)
                    blockCountL += (Main.tile[x, y].active() && Main.tile[x, y].type != 189) ? 1 : 0; // dont count clouds
            for (int x = Main.dungeonX; x < Main.dungeonX + 50; x++)
                for (int y = Main.dungeonY - 50; y < Main.dungeonY - 40; y++)
                    blockCountR += (Main.tile[x, y].active() && Main.tile[x, y].type != 189) ? 1 : 0; // dont count clouds


            hasOBjectOrParam["Dungeon below ground"] = (blockCountL > 100 && blockCountR > 100) || (blockCountR + blockCountL > 500) ? 1 : 0;

            int xOSD = Main.dungeonX < Main.maxTilesX / 2 ? xOSL : xOSR;
            if (Math.Abs(Main.maxTilesX / 2 - xOSD) - Math.Abs(Main.maxTilesX / 2 - Main.dungeonX) > 200 && hasOBjectOrParam["Dungeon below ground"] == 0 && hasOBjectOrParam["Dungeon tiles above surface"] == 0)
            {
                hasOBjectOrParam["Dungeon has good Pos"] = 1;
            }

            hasOBjectOrParam["Dungeon far above surface"] = hasOBjectOrParam["Dungeon tiles above surface"] > 95 ? 1 : 0;

            if (hasOBjectOrParam["Dungeon below ground"] > 0 || hasOBjectOrParam["Dungeon far above surface"] > 0 || hasOBjectOrParam["Dungeon in Snow Biome"] > 0)
                hasOBjectOrParam["Dungeon has strange Pos"] = 1;



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

            hasOBjectOrParam["Jungle biome distance to mid"] = localDungeonSide < 0 ? (leftmostSurfaceJungleTilex - Main.maxTilesX / 2) : (Main.maxTilesX / 2 - rightmostSurfaceJungleTilex);
            hasOBjectOrParam["Snow biome distance to mid"] = localDungeonSide > 0 ? (leftmostSurfaceSnowTilex - Main.maxTilesX / 2) : (Main.maxTilesX / 2 - rightmostSurfaceSnowTilex);




            if (doFull)
            {


                if (hasOBjectOrParam["Cloud Chest"] + 1 + ((Main.maxTilesX > 6000) ? 1 : 0) + ((Main.maxTilesX > 8000) ? 1 : 0) < hasOBjectOrParam["Number of Clouds"])
                    hasOBjectOrParam["Floating Island without chest"] = 1;

                //find path
                Stopwatch stopWatchway = new Stopwatch();
                stopWatchway.Start();

                int startx = 100;
                int endx = Main.maxTilesX - 100;
                int cy = (int)(Main.rockLayer + 0.4 * ((Main.maxTilesY - Main.rockLayer) - 200));
                int cmp = Int32.MaxValue;
                int cmxi = 0;

                for (int cxi = startx; cxi <= endx; cxi++)
                {
                    if (pathLength[cxi, cy] < cmp && !isInDungeon(cxi, cy))
                    {
                        cmp = pathLength[cxi, cy];
                        cmxi = cxi;
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
                    Tuple<List<Tuple<int, int>>, int, int> entrpathJ = FindCaveEntrance(ref pathLength, jmxi, jy, true, leftmostCavernJungleTilex + jungleWide / 10, rightmostCavernJungleTilex + jungleWide / 10);
                    score.itemLocation.Add(ItemID.JungleShirt, entrpathJ.Item1);

                    Tuple<int, int> entrJ = entrpathJ.Item1.Last();
                    jmex = entrJ.Item1;
                    jmey = entrJ.Item2;

                    hasOBjectOrParam["Pathlength to cavern entrance to mid of Jungle"] = pathLength[entrJ.Item1, entrJ.Item2] / pathNormFac;
                    hasOBjectOrParam["Tiles to mine for mid Jungle cavern"] = entrpathJ.Item2;

                    if (jmex > leftmostSurfaceJungleTilex + jungleWideSurf / 10 && jmex < rightmostSurfaceJungleTilex + jungleWideSurf / 10 && entrpathJ.Item2 < Main.maxTilesY / 120 && entrpathJ.Item3 == 0)
                        hasOBjectOrParam["Free cavern to mid Jungle"] = 1;
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
                    Tuple<List<Tuple<int, int>>, int, int> entrpathJB = FindCaveEntrance(ref pathLength, jmxi, jmyi, true, leftmostCavernJungleTilex + jungleWide / 10, rightmostCavernJungleTilex + jungleWide / 10);
                    score.itemLocation.Add(ItemID.JunglePants, entrpathJB.Item1);

                    Tuple<int, int> entrJB = entrpathJB.Item1.Last();
                    jmexd = entrJB.Item1;
                    jmeyd = entrJB.Item2;
                    hasOBjectOrParam["Pathlength to cavern entrance to deep Jungle"] = pathLength[jmexd, jmeyd] / pathNormFac;
                    hasOBjectOrParam["Tiles to mine for deep Jungle cavern"] = entrpathJB.Item2;

                    if (jmexd > leftmostSurfaceJungleTilex + jungleWideSurf / 10 && jmexd < rightmostSurfaceJungleTilex + jungleWideSurf / 10 && entrpathJB.Item2 < Main.maxTilesY / 60 && entrpathJB.Item3 == 0)
                        hasOBjectOrParam["Free cavern to deep Jungle"] = 1;
                }
                if (Math.Abs(jmexd - jmex) < 5 && Math.Abs(jmeyd - jmey) < 5 && hasOBjectOrParam["Free cavern to deep Jungle"] == 1 && hasOBjectOrParam["Free cavern to mid Jungle"] == 1)
                    hasOBjectOrParam["Jungle cavern not blocked by structure"] = 1;


                ts = stopWatchway.Elapsed;
                elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);


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
                for (int xi = Main.spawnTileX - 6; xi < Main.spawnTileX + 7; xi++)
                    for (int yi = Main.spawnTileY - 6; yi < Main.spawnTileY + 7; yi++)
                    {
                        if (Main.tile[xi, yi].active())
                        {

                            if (Main.tile[xi, yi].type == TileID.SnowBlock || Main.tile[xi, yi].type == TileID.IceBlock || Main.tile[xi, yi].type == TileID.FleshIce || Main.tile[xi, yi].type == TileID.CorruptIce)
                                numS++;
                            if (Main.tile[xi, yi].type == TileID.JungleGrass || Main.tile[xi, yi].type == TileID.JunglePlants || Main.tile[xi, yi].type == TileID.JunglePlants2 || Main.tile[xi, yi].type == TileID.JungleThorns || Main.tile[xi, yi].type == TileID.JungleVines)
                                numJ++;
                        }
                    }
                hasOBjectOrParam.Add("Spawn in Snow biome", numS > 10 ? 1 : 0);
                hasOBjectOrParam.Add("Spawn in Jungle biome", numJ > 10 ? 1 : 0);


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
                hasOBjectOrParam.Add("Ground Distance", (int)Math.Abs(Main.worldSurface - Main.spawnTileY));
                hasOBjectOrParam.Add("Rock Distance", (int)Math.Abs(Main.rockLayer - Main.spawnTileY));

                hasOBjectOrParam.Add("Seaweed Pet", has(ref hasOBjectOrParam, ItemID.Seaweed));
                hasOBjectOrParam.Add("Fish Pet", has(ref hasOBjectOrParam, ItemID.Fish));



                hasOBjectOrParam.Add("Enchanted Sword near Pyramid or Tree", hasOBjectOrParam["Enchanted Sword near Pyramid"] + hasOBjectOrParam["Enchanted Sword near Tree"]);

                hasOBjectOrParam["Very Near Enchanted Sword"] += hasOBjectOrParam["Near Enchanted Sword near Pyramid"] + hasOBjectOrParam["Near Enchanted Sword near Tree"];





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




                hasOBjectOrParam.Add("All chest items you can't craft or fish", hasOBjectOrParam["All Pyramid Items"] > 0 &&
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
                                                                                hasOBjectOrParam["Angel Statue placed"] + hasOBjectOrParam["Angel Statue chest"] > 0
                                                                                ? 1 : 0);
                //hasOBjectOrParam.Add("Has Alchemy, Sharpening, Enchanted",  );



                //negative value of pathlength for postive list
                hasOBjectOrParam["neg. Pathlength to Temple Door"] = -hasOBjectOrParam["Pathlength to Temple Door"];
                hasOBjectOrParam["neg. Pathlength to Boots"] = -hasOBjectOrParam["Pathlength to Boots"];
                hasOBjectOrParam["neg. Pathlength to Iron/Lead Bar"] = -hasOBjectOrParam["Pathlength to Iron/Lead Bar"];
                hasOBjectOrParam["neg. Pathlength to Gold/Platinum Bar"] = -hasOBjectOrParam["Pathlength to Gold/Platinum Bar"];
                hasOBjectOrParam["neg. Pathlength to Bomb"] = -hasOBjectOrParam["Pathlength to Bomb"];
                hasOBjectOrParam["neg. Pathlength to Jester's Arrow"] = -hasOBjectOrParam["Pathlength to Jester's Arrow"];
                hasOBjectOrParam["neg. Pathlength to Suspicious Looking Eye"] = -hasOBjectOrParam["Pathlength to Suspicious Looking Eye"];
                hasOBjectOrParam["neg. Pathlength to Snowball Cannon"] = -hasOBjectOrParam["Pathlength to Snowball Cannon"];
                hasOBjectOrParam["neg. Pathlength to Teleport Potion"] = -hasOBjectOrParam["Pathlength to Teleport Potion"];
                hasOBjectOrParam["neg. Pathlength to Enchanted Sword"] = -hasOBjectOrParam["Pathlength to Enchanted Sword"];
                hasOBjectOrParam["neg. Pathlength to Bee Hive"] = -hasOBjectOrParam["Pathlength to Bee Hive"];
                hasOBjectOrParam["neg. Pathlength to Boomstick"] = -hasOBjectOrParam["Pathlength to Boomstick"];

                hasOBjectOrParam["neg. Pathlength to Slime Staute"] = -hasOBjectOrParam["Pathlength to Slime Staute"];
                hasOBjectOrParam["neg. Pathlength to Shark Staute"] = -hasOBjectOrParam["Pathlength to Shark Staute"];                
                hasOBjectOrParam["neg. Pathlength to Anvil"] = -hasOBjectOrParam["Pathlength to Anvil"];

                hasOBjectOrParam["neg. Pathlength to free ShadowOrb/Heart"] = -hasOBjectOrParam["Pathlength to free ShadowOrb/Heart"];
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


        Tuple<List<Tuple<int, int>>,int, int> FindCaveEntrance(ref int[,] pathlength, int x, int y, bool excludeLastMiningTiles=true, int borderLeftX = -1, int borderRightX = 100000)
        {
            List < Tuple<int, int> > path= new List<Tuple<int, int>>();
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

                if (minv ==      pathlength[x + 1, y - 1]) { x++; y--; }
                else if (minv == pathlength[x    , y - 1]) { y--; }
                else if (minv == pathlength[x - 1, y - 1]) { x--; y--; }
                else if (minv == pathlength[x - 1, y    ]) { x--; }
                else if (minv == pathlength[x - 1, y + 1]) { x--; y++; }
                else if (minv == pathlength[x    , y + 1]) { y++; }
                else if (minv == pathlength[x + 1, y + 1]) { x++; y++; }
                else if (minv == pathlength[x + 1, y    ]) { x++; y++; }

                if (minv < ominv - 60 && !(Main.tile[x,y].liquid>0) )
                    ctilesToMine++;
                else
                {
                    //exclude last tiles 
                    tilesToMine += ctilesToMine;
                    ctilesToMine = 0;
                }

                if (ox == x && oy == y )
                {
                    if(x != Main.spawnTileX && y != Main.spawnTileY)
                        writeDebugFile("entrance finding failed for seed " + seed);
                    //should not happen  , only if start is in spawn like in seed 170170245
                    break;
                }
                path.Add(new Tuple<int, int>(x, y));
            }           

            if(!excludeLastMiningTiles) tilesToMine += ctilesToMine;
            return new Tuple<List<Tuple<int, int>>, int, int>(path, tilesToMine, tilesOutside);
        }


        const int pathNormFac = 5;
        private void AddWayPoints(ref int[,] pathLength, ref List<Tuple<int, int>>[] waypoints, ref short[,] travelCost, int x, int y, int l)
        {
            if (pathLength[x, y] < l || x< 42 || y<10 || x>Main.maxTilesX-43 || y>Main.maxTilesY-11) return;
            

            const int costLR = pathNormFac;
            const int costU = 5;
            const int costD = 3;
            const int costDLR = 6;
            const int costULR = 7;
                     
            //careful: tracelCost in short with value max 10,000, 3 times close to max short value
            int tcl = travelCost[x - 1, y] + travelCost[x - 1, y + 1] + travelCost[x - 1, y - 1];
            int tcr = travelCost[x + 1, y] + travelCost[x + 1, y + 1] + travelCost[x + 1, y - 1];
            int tcu = travelCost[x , y - 1] + travelCost[x + 1, y - 1];
            int tcd = travelCost[x, y + 1] + travelCost[x + 1, y + 1];

            int mpl = l + MIN_PATH_LENGTH;
            if (pathLength[x - 1,y]  > mpl )
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x-1, y, l, costLR , tcl);
            if (pathLength[x + 1, y] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x + 1, y, l, costLR , tcr);
            if (pathLength[x, y + 1] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x , y+1 , l, costD , tcd);
            if (pathLength[x, y -1] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x, y - 1, l, costU , tcu, true);
            if (pathLength[x - 1, y - 1] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x - 1, y - 1, l, costULR , tcu + tcl, true);
            if (pathLength[x + 1, y - 1] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x +1 , y - 1, l, costULR , tcu + tcr, true);
            if (pathLength[x - 1, y + 1] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x - 1, y + 1, l, costDLR , tcd + tcl);
            if (pathLength[x + 1, y + 1] > mpl)
                AddPoint(ref pathLength, ref waypoints, ref travelCost, x + 1, y + 1, l, costDLR , tcd + tcr);

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

            const short costWebThorn = 25;
            const short costMineCart = -17;
            const short costDirt = 60;
            const short costStone = 120;


            int bon = 0;
            if (y > Main.maxTilesY - 190)
                bon += costUnderWBoni;
            else if(y > (int)(Main.rockLayer + 0.55 * ((Main.maxTilesY - Main.rockLayer) - 200)) )
                bon += costCavernLavaBoni;
            else if(y > Main.rockLayer)
                bon += costCavernBoni;
            else if (y > Main.worldSurface)
                bon += costUnderGBoni;

            ushort type = Main.tile[x, y].type;

            if (Main.tile[x, y].liquid > 0)
            {
                if (x==0 || x==Main.maxTilesX-1 || y==0 || y==Main.maxTilesY-1)
                    bon += costFullLiqBoni;
                else if (Main.tile[x + 1, y].liquid > 0 && Main.tile[x - 1, y].liquid > 0 && Main.tile[x + 1, y - 1].liquid > 0 && Main.tile[x - 1, y - 1].liquid > 0 && Main.tile[x, y - 1].liquid > 0 && Main.tile[x + 1, y + 1].liquid > 0 && Main.tile[x - 1, y + 1].liquid > 0 && Main.tile[x, y + 1].liquid > 0)
                    bon += costFullLiqBoni;

                return costWater + bon + (Main.tile[x, y].honey() ? costHoneyBon : 0) + (Main.tile[x, y].lava() ? costLavaBon : 0);
            }



            bool val=  ( (!Main.tile[x, y].active()                               
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

            if (type == TileID.Cobweb || type == TileID.CorruptThorns || type == TileID.CrimtaneThorns || type == TileID.JungleThorns )
                return costWebThorn + bon;

            if (type == TileID.Platforms)
                return costPlatform + bon;

            if (type == TileID.MinecartTrack)
                return costMineCart + bon;

            if (type == TileID.Dirt || type == TileID.Grass || type == TileID.FleshGrass || type == TileID.CorruptGrass || type == TileID.JungleGrass || type == TileID.MushroomGrass || type == TileID.Sand || type == TileID.ClayBlock || type == TileID.Mud || type == TileID.Silt || type == TileID.Ash || type == TileID.SnowBlock || type == TileID.Slush || type == TileID.HardenedSand || type == TileID.CrimsonHardenedSand || type == TileID.CorruptHardenedSand || type == TileID.Hive)
                return costDirt + bon;

            if (type == TileID.BlueDungeonBrick || type == TileID.GreenDungeonBrick || type == TileID.PinkDungeonBrick || type == TileID.Obsidian || type == TileID.Ebonstone || type == TileID.Crimstone || type == TileID.Hellstone || type == TileID.LihzahrdBrick || type == TileID.Demonite || type == TileID.Crimtane || (type == TileID.ClosedDoor && (Main.tile[x, y].frameY == 594 || Main.tile[x, y].frameY == 612 || Main.tile[x, y].frameY == 620)) )
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

            if(pathLength[x,y] > npl)
            {
                pathLength[x, y] = npl;
                if (npl < waypoints.Length)
                {
                    if (waypoints[npl] == null)
                        waypoints[npl] = new List<Tuple<int, int>>();                    
                    waypoints[npl].Add(new Tuple<int, int>(x, y)); //can be in more than on list, remove from old?                    
                }
            }

        }

        //e.g. you can also access stuff through walls
        private int FindShortestPathInRange(ref int[,] pathLength, int x, int y, int mx=2, int px=3, int my=2, int py=3)
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
            public Dictionary<int, List<Tuple<int,int>>> itemLocation;
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
                itemLocation = new Dictionary<int, List<Tuple<int, int> >>();
                missingCount = 0;
                missingCountNot = 0;
                phase3Empty = false;

        }

            public void clear()
            {
                hasOBjectOrParam.Clear();
                hasOBjectOrParam = null;
            }

            public void insertGenInfo(generatorInfo genInfo)
            {

                maxPyramidCountChance = genInfo.numPyrChance;
                pyramids = genInfo.numPyramids;
                floatingIslands = genInfo.numIsland; //todo do funtion for score which sets geninfo
                closestTreeToMid = genInfo.minTreeToMapMidDist;
                closestCloudToMid = genInfo.minCloudToMapMidDist;
                closestPyramidToMid = genInfo.minPyramidToMapMidDist;
                trees = genInfo.numTree;
                
                hasOBjectOrParam.Add("Pyramids possible", maxPyramidCountChance);
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
                        notInSand =  0;
                    }                    
                    else if ((Main.tile[tx, y].wall != WallID.HardenedSand && Main.tile[tx, y].wall != WallID.Sandstone) )                    
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
                for (int tx = x; tx > x-dim; tx--)
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
                for (int ty = y; ty < y+dim; ty++)
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

        private static bool checkIfNearTree(int curX, int curY, int xlookLR, int yaboveLook)
        {
            bool near = false;
            xlookLR = Math.Abs(xlookLR);
            int y = curY - yaboveLook;


            for (int x = curX - xlookLR; x < curX + xlookLR; x++)
            {
                if (x >= 0 && x < Main.maxTilesX && Main.tile[x, y].active() && ((Main.tile[x, y].type == TileID.LivingWood) || (Main.tile[x, y].wall == WallID.LivingWood)))
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


        private static bool checkIfNearSpawn(int curX, int curY, int xdiff, int ydiff)
        {
            return Math.Abs(curX - Main.spawnTileX) <= xdiff && Math.Abs(curY - Main.spawnTileY) <= ydiff;
        }

        private static int getDistanceToSpawn(int curX, int curY)
        {
            return (int)Math.Sqrt((Main.spawnTileX - curX) * (Main.spawnTileX - curX) + (Main.spawnTileY - curY) * (Main.spawnTileY - curY));
        }

        public static void writeDebugFile(string content, bool newFile=false)
        {
            using (System.IO.StreamWriter file =
             new System.IO.StreamWriter(Main.SavePath + OptionsDict.Paths.debugPath+@".\debug.txt", !newFile))
            {
                file.WriteLine(content);
            }
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
                if (dx > Main.maxTilesX-2 || dx < 1) break;
                while (!Main.tile[dx, dy].active() || (Main.tile[dx, dy].active() && Main.tile[dx, dy].active() && (Main.tile[dx, dy].type != 41 && Main.tile[dx, dy].type != 43 && Main.tile[dx, dy].type != 44)) && (Main.tile[dx - dir, dy].active() &&
                    (Main.tile[dx - dir, dy].type == 41 || Main.tile[dx - dir, dy].type == 43 || Main.tile[dx - dir, dy].type == 44)))
                {
                    //Main.tile[dx, dy].active(true);
                    // Main.tile[dx, dy].type = 45;
                    dy += 1;
                    wentDown++;
                    if (dy == Main.maxTilesY) break;
                }
                if (dx == Main.maxTilesX-1) break; if (dx == 0) break; if (dy == Main.maxTilesY) break;

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
            //fffffffffff   in Terrarai schienen die nicht solid zu sein, in Tedit jedoch wohl


            //hier vines sind nicht solid aber schlimm wenn man nach oben geht?
            //if (tile.IsActive && World.TileProperties[Main.tile[posX, posY - dist].Type].IsSolid &&
            //    Main.tile[posX, posY - dist].Type != 51 && Main.tile[posX, posY - dist].Type != 52 &&
            //        Main.tile[posX, posY - dist].Type != 62)

            //liquid &&  Main.tile[posX, posY - dist].liquid != 255

            if (Main.tile[posX, posY - dist].type == TileID.LivingWood || Main.tile[posX, posY - dist].wall == WallID.LivingWood || Main.tile[posX, posY - dist].type == TileID.LeafBlock || Main.tile[posX, posY - dist].liquid == 255)
                return 0;

            bool up = false;



            while (CheckIfSolidAndNoTree(posX, posY - dist) ) {

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
                if (Main.tile[posX, tempY].liquid != 255 )
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
            const int CHECK_ABOVE_RANGE = 52; //hatte 40, aber dungeon erkkents da nicht min 50, zu hoch? //TODO cloud tiles nicht beachten

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
            if (fac == 1) return start * howMany ;
            
            return start * (Math.Pow(fac, howMany ) - 1) / (fac - 1);
            
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

            allScoreText += System.Environment.NewLine + "Score Water Bolt " + (int)score;

            score -= hasOBjectOrParam["Meteorite Bar unlocked"] == 0 ? 20 : -30;
            score += sumUp(hasOBjectOrParam["Meteorite Bar unlocked"], 20, 0.9);

            allScoreText += System.Environment.NewLine + "Score Meteroite " + (int)score;


            score -= hasOBjectOrParam["Lava Charm"] == 0 ? 50 : 0;
            score += sumUp(hasOBjectOrParam["Lava Charm"], 30, 0.3);

            score -= hasOBjectOrParam["Water Walking Boots"] == 0 ? 50 : 0;
            score += sumUp(hasOBjectOrParam["Water Walking Boots"], 30, 0.3);

            allScoreText += System.Environment.NewLine + "Score Lava Waders " + (int)score;




            score += hasOBjectOrParam["Aglet"] > 0 ? sumUp(hasOBjectOrParam["Aglet"], 25, 0.4) : -50;
            allScoreText += System.Environment.NewLine + "Score Aglet " + (int)score;

            score += hasOBjectOrParam["Magic Mirror"] > 0 ? sumUp(hasOBjectOrParam["Magic Mirror"], 30, 0.4) : (hasOBjectOrParam["Ice Mirror"]==0?-250:-100) ;//together with ice -500 if both 0
            score += hasOBjectOrParam["Band of Regeneration"] > 0 ? sumUp(hasOBjectOrParam["Band of Regeneration"], 30, 0.4)-20 : -10 ;
            score += hasOBjectOrParam["Band of Regeneration"] < 4 ? -40 : 0;
            score += hasOBjectOrParam["Shoe Spikes"] > 0 ? sumUp(hasOBjectOrParam["Shoe Spikes"], 30, 0.4)-20 : -50;
            score += hasOBjectOrParam["Hermes Boots"] > 0 ? sumUp(hasOBjectOrParam["Hermes Boots"], 20, 0.4)-20 : (hasOBjectOrParam["Flurry Boots"] == 0 ? -50 : -5);
            score -= 140; //penalty because they are not rare
            allScoreText += System.Environment.NewLine + "Score Forest Chest items " + (int)score;








            score += hasOBjectOrParam["Ice Skates"] > 0 ? 50+sumUp(hasOBjectOrParam["Ice Skates"], 35, 0.4) : -60;
            allScoreText += System.Environment.NewLine + "Score Ice Skates " + (int)score;

            score += hasOBjectOrParam["Blizzard in a Bottle"] > 0 ? sumUp(hasOBjectOrParam["Blizzard in a Bottle"], 30, 0.4) : (hasOBjectOrParam["Pyramid Bottle"]>0?-5:-30) ;
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

            allScoreText += System.Environment.NewLine + "Score High Hives " + (int)score;

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
            score += hasOBjectOrParam["Very Near Enchanted Sword"] > 0 ? sumUp(hasOBjectOrParam["Very Near Enchanted Sword"], 500, 1.0) : 0;
            score += hasOBjectOrParam["Enchanted Sword near Pyramid"] > 0 ? sumUp(hasOBjectOrParam["Enchanted Sword near Pyramid"] - hasOBjectOrParam["Near Enchanted Sword near Pyramid"], 50, 1.0) : 0;
            score += hasOBjectOrParam["Enchanted Sword near Tree"] > 0 ? sumUp(hasOBjectOrParam["Enchanted Sword near Tree"] - hasOBjectOrParam["Near Enchanted Sword near Tree"], 50, 1.0) : 0;

            allScoreText += System.Environment.NewLine + "Score Near ES " + (int)score;

            score += hasOBjectOrParam["Near Tree"] > 0 ? sumUp(hasOBjectOrParam["Near Tree"], 100, 1.0) : 0;

            allScoreText += System.Environment.NewLine + "Score Near Tree " + (int)score;

            score += hasOBjectOrParam["Near Altar"] > 0 ? sumUp(hasOBjectOrParam["Near Altar"], 50, 0.2) : 0;

            allScoreText += System.Environment.NewLine + "Score Near Altar " + (int)score;

            double webCount = sumUp(hasOBjectOrParam["Near Spider Web count"], 0.09, 0.9999);
            score += hasOBjectOrParam["Near Spider Web count"] > 0 ? (webCount > 50 ? 50 : webCount) : -50;

            allScoreText += System.Environment.NewLine + "Score Near Spider Web " + (int)score;

            double musCount = sumUp(hasOBjectOrParam["Near Mushroom Biome count"], 0.09, 0.9999);
            score += hasOBjectOrParam["Near Mushroom Biome count"] > 0 ? (musCount > 50 ? 50 : musCount) : -25;

            allScoreText += System.Environment.NewLine + "Score Near Mushroom Biome count " + (int)score;

            score += hasOBjectOrParam["Near Chest"] > 0 ? sumUp(hasOBjectOrParam["Near Chest"], 5, 1.05) : -300;

            allScoreText += System.Environment.NewLine + "Score Near Chest " + (int)score;

            score += hasOBjectOrParam["Near Cloud"] > 0 ? 75 * hasOBjectOrParam["Near Cloud"] : 0;

            allScoreText += System.Environment.NewLine + "Score Near Cloud " + (int)score;

                       

            score += hasOBjectOrParam["Dungeon Distance"] < 1200 ? 100 : 0;
            score += hasOBjectOrParam["Dungeon Distance"] > 1700 ? -50 : 0;

            allScoreText += System.Environment.NewLine + "Score DungDist " + (int)score;

            score += hasOBjectOrParam["Temple Distance"] < 600 ? 0.5 * (600 - hasOBjectOrParam["Temple Distance"]) : 0;//todo dist to open entrance not door
            score += hasOBjectOrParam["Temple Distance"] < 750 ? 100 : 0;
            score += hasOBjectOrParam["Temple Distance"] < 850 ? 40 : 0;
            score += hasOBjectOrParam["Temple Distance"] < 900 ? 10 : 0;
            score += hasOBjectOrParam["Temple Distance"] > 1350 ? -80 : 0;

            int dpf = (hasOBjectOrParam["Temple Distance"] * 2 *100) / hasOBjectOrParam["Pathlength to Temple Door"];
            score += (dpf - 100);

            allScoreText += System.Environment.NewLine + "Score TempleDistPath " + (int)score;
            

            score += hasOBjectOrParam["Ground Distance"] < 50 ? 40 : 0;
            score += hasOBjectOrParam["Ground Distance"] > 120 ? -60 : 0;

            allScoreText += System.Environment.NewLine + "Score GroundDist " + (int)score;

            score += hasOBjectOrParam["Rock Distance"] < 150 ? 40 : 0;
            score += hasOBjectOrParam["Rock Distance"] > 250 ? -60 : 0;

            allScoreText += System.Environment.NewLine + "Score RockDist " + (int)score;

            score += hasOBjectOrParam["Hermes Flurry Boots Distance"] < 150 && hasOBjectOrParam["Pathlength to Boots"]  < 450 ? 60 : 0; 
            score -= 0.05 * hasOBjectOrParam["Hermes Flurry Boots Distance"];
            score -= 0.015 * hasOBjectOrParam["Pathlength to Boots"];
            
            allScoreText += System.Environment.NewLine + "Score BootsDistPath " + (int)score;

            
            //near objects
            double tval = 0.05 * (1000 - hasOBjectOrParam["Pathlength to Teleport Potion"]);
            score += tval > -5 ? tval : -5;
            allScoreText += System.Environment.NewLine + "Score Path Teleport Potion " + (int)score;

            tval = 0.1 * (500 - hasOBjectOrParam["Pathlength to Iron/Lead Bar"]);
            score += tval > -10 ? tval : -10;
            allScoreText += System.Environment.NewLine + "Score Path Iron/Lead Bar " + (int)score;

            tval = 0.1 * (800 - hasOBjectOrParam["Pathlength to Gold/Platinum Bar"]);
            score += tval > -7 ? tval : -7;
            allScoreText += System.Environment.NewLine + "Score Path Gold/Platinum Bar " + (int)score;

            tval = 0.1 * (500 - hasOBjectOrParam["Pathlength to Bomb"]);
            score += tval > -3 ? tval : -3;
            allScoreText += System.Environment.NewLine + "Score Path Bomb " + (int)score;

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
            score += hasOBjectOrParam["Pathlength to free ShadowOrb/Heart"] < 2500 && hasOBjectOrParam["Free ShadowOrb/Heart"] > 0 ? 40 + sumUp(hasOBjectOrParam["Free ShadowOrb/Heart"], 30, 0.725) : 0;
            allScoreText += System.Environment.NewLine + "Score (Path) Exposed Orbs/Heart " + (int)score;
            




            //beach
            score -= hasOBjectOrParam["Beach penalty left"] < 0 ? 0 : hasOBjectOrParam["Beach penalty left"] > 5000 ? 100 : 0.02 * hasOBjectOrParam["Beach penalty left"];
            score -= hasOBjectOrParam["Beach penalty right"] < 0 ? 0 : hasOBjectOrParam["Beach penalty right"] > 5000 ? 100 : 0.02 * hasOBjectOrParam["Beach penalty right"];

            score += hasOBjectOrParam["Beach penalty left"] < 0 ? 0 : hasOBjectOrParam["Beach penalty left"] > 1000 ? 0 : 0.04 * (1000 - hasOBjectOrParam["Beach penalty left"]);
            score += hasOBjectOrParam["Beach penalty right"] < 0 ? 0 : hasOBjectOrParam["Beach penalty right"] > 1000 ? 0 : 0.04 * (1000 - hasOBjectOrParam["Beach penalty right"]);

            allScoreText += System.Environment.NewLine + "Score Beach " + (int)score;

            

            //dungeon pos            
            score += hasOBjectOrParam["Dungeon has good Pos"] > 0 ? 15 : -55;
            allScoreText += System.Environment.NewLine + "Score Dungeon Pos " + (int)score;

            float wallNum = hasOBjectOrParam["Green Dungeon Walls"] + hasOBjectOrParam["Blue Dungeon Walls"] + hasOBjectOrParam["Pink Dungeon Walls"];
            score += hasOBjectOrParam["All Dungeon Walls"] > 1337 ? 100.0 * hasOBjectOrParam["All Dungeon Walls"]/wallNum : -135;
            allScoreText += System.Environment.NewLine + "Score All Dungeon Walls " + (int)score;

            score += hasOBjectOrParam["All Dungeon Items"] > 0 ? 40 : -55;
            allScoreText += System.Environment.NewLine + "Score All Dungeon Items " + (int)score;
            
            

            score += hasOBjectOrParam["Flame Trap"] > 0 ? 15 + hasOBjectOrParam["Flame Trap"] * 5 : -42;
            allScoreText += System.Environment.NewLine + "Score Flame Traps " + (int)score;
            
            score += sumUp(hasOBjectOrParam["Number different Paintings"], 5, 1.1) * 0.05 - 15;
            score += hasOBjectOrParam["Number Paintings"] * (0.5f / mapScale) - 2;
            allScoreText += System.Environment.NewLine + "Score Paintings " + (int)score;


            score += hasOBjectOrParam["Different functional noncraf. Statues"] == 26 ? hasOBjectOrParam["Number functional noncraf. Statues"] * (0.25f / (mapScale - 1)) : -100;
            score += hasOBjectOrParam["Different noncraf. Statues"] == 56 ? hasOBjectOrParam["Number noncraf. Statues"] * (0.05f / (mapScale - 1)) : -50;
            allScoreText += System.Environment.NewLine + "Score Statues " + (int)score;

            score += hasOBjectOrParam["Alchemy Table"] > 0 ? sumUp(hasOBjectOrParam["Alchemy Table"], 30, 0.72)-30 : -150;
            allScoreText += System.Environment.NewLine + "Score Alchemy Table " + (int)score;

            score += hasOBjectOrParam["Sharpening Station"] > 0 ? sumUp(hasOBjectOrParam["Sharpening Station"], 23, 0.65)-23 : -150;
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

            if (hasOBjectOrParam["Open Temple"] > 0)
            {
                score += 1337;
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
            }else
            if (hasOBjectOrParam["Spawn in Snow biome"] > 0)
            {
                score += 420;
                allScoreText += System.Environment.NewLine + "Score Spawn in Snow biome " + (int)score;
            }

  

            score += hasOBjectOrParam["Biome Item in normal Chest"] > 0 ? 1337 * hasOBjectOrParam["Biome Item in normal Chest"] : 0; //should be patched
            if (hasOBjectOrParam["Biome Item in normal Chest"] > 0) allScoreText += System.Environment.NewLine + "Score BiomeNormalChest " + (int)score;

            score += hasOBjectOrParam["Dungeon in Snow Biome"] > 0 ? 420 : 0;            
            score += hasOBjectOrParam["Dungeon far above surface"] > 0 ? 42 : 0;            
            score += hasOBjectOrParam["Dungeon below ground"] > 0 ? 420 : 0;            
            if(hasOBjectOrParam["Dungeon has strange Pos"] > 0)allScoreText += System.Environment.NewLine + "Score Dungeon has strange Pos " + (int)score;

            score += hasOBjectOrParam["Floating island cabin in Dungeon"] > 0 ? 420 : 0;
            if (hasOBjectOrParam["Floating island cabin in Dungeon"] > 0) allScoreText += System.Environment.NewLine + "Score Floating cabin in Dungeon " + (int)score;


            score += hasOBjectOrParam["Pre Skeletron Dungeon Chest Risky"] > 0 ? 100 * hasOBjectOrParam["Pre Skeletron Dungeon Chest Risky"] : 0;
            score += hasOBjectOrParam["Pre Skeletron Dungeon Chest Grab"] > 0 ? 200 * hasOBjectOrParam["Pre Skeletron Dungeon Chest Grab"] : 0;
            if (hasOBjectOrParam["Pre Skeletron Dungeon Chest Grab"] > 0 || hasOBjectOrParam["Pre Skeletron Dungeon Chest Risky"] > 0)
                allScoreText += System.Environment.NewLine + "Score Pre Skeletron Dungeon Chest " + (int)score;

            if (hasOBjectOrParam["Chest Doub Glitch"] > 0)
            {
                score += hasOBjectOrParam["Chest Doub Glitch"] > 0 ? 420 * hasOBjectOrParam["Chest Doub Glitch"] : 0;
                allScoreText += System.Environment.NewLine + "Score Chest Doub Glitch " + (int)score;
            }

            if(hasOBjectOrParam["All chest items you can't craft or fish"] > 0)
            {
                score += 1337;
                allScoreText += System.Environment.NewLine + "Score  all chest items you can't craft or fish! " + (int)score;
            }



            allScoreText += System.Environment.NewLine + "Bonus Score:";
            float multiAppearance = 0;
            if(hasOBjectOrParam["Pyramid Mask"] > 0) multiAppearance+=3;
            if(hasOBjectOrParam["Tree Chest"] > 0) multiAppearance+=11;
            if(hasOBjectOrParam["Tree Chest Loom"] > 0) multiAppearance+=22;            
            if(hasOBjectOrParam["Living Mahogany Wand"] > 0) multiAppearance+=4;
            if(hasOBjectOrParam["Honey Dispenser"] > 0) multiAppearance+=2;
            if(hasOBjectOrParam["Ice Machine"] > 0) multiAppearance+=7;
            if(hasOBjectOrParam["Bone Welder"] > 0) multiAppearance+=5;
            if(hasOBjectOrParam["Cloud Sky Mill"] > 0) multiAppearance+=10;
            if(hasOBjectOrParam["Fish Pet"] > 0) multiAppearance+=18;
            if(hasOBjectOrParam["Seaweed Pet"] > 0) multiAppearance+=18;
            allScoreText += System.Environment.NewLine + "Appearance chest items ("+ multiAppearance+"/100)";//extra point if > 54

            float multiUsefullRare = 0;
            if(hasOBjectOrParam["Pyramid Carpet"] > 0) multiUsefullRare += 1;
            if(hasOBjectOrParam["Pyramid Bottle"] > 0)   multiUsefullRare+=1;
            if(hasOBjectOrParam["Flower Boots"] > 0)   multiUsefullRare+=1;            
            if(hasOBjectOrParam["Ice Skates"] > 0 )   multiUsefullRare+=1;
            if(hasOBjectOrParam["Lava Charm"] > 0 && hasOBjectOrParam["Water Walking Boots"] > 0) multiUsefullRare+=1;                    
            allScoreText += System.Environment.NewLine + "Rare unmakeable chest items (" + (int)(multiUsefullRare/5.0*100) + "/100)";

            float multiOtherRareItems = 0;
            if (hasOBjectOrParam["Blizzard in a Bottle"] > 0) multiOtherRareItems+=17;
            if (hasOBjectOrParam["Snowball Cannon"] > 0) multiOtherRareItems+=10;
            if (hasOBjectOrParam["Shoe Spikes"] > 0) multiOtherRareItems+=10;
            if (hasOBjectOrParam["Band of Regeneration"] > 0) multiOtherRareItems+=10; 
            if (hasOBjectOrParam["Magic Mirror"] > 0) multiOtherRareItems+=10; else multiOtherRareItems -= 10;
            if (hasOBjectOrParam["Ice Mirror"] > 0) multiOtherRareItems+= 4;
            if (hasOBjectOrParam["Ice Mirror"] == 0 && hasOBjectOrParam["Magic Mirror"] == 0) multiOtherRareItems -= 50; //should not happen
            if (hasOBjectOrParam["Water Bolt"] > 0) multiOtherRareItems+=10;
            if (hasOBjectOrParam["Ice Blade"] > 0) multiOtherRareItems+=5;
            if (hasOBjectOrParam["Ice Boomerang"] > 0) multiOtherRareItems+= 5;
            if (hasOBjectOrParam["Flurry Boots"] > 0) multiOtherRareItems += 4;
            if (hasOBjectOrParam["Hermes Boots"] > 0) multiOtherRareItems += 10; else multiOtherRareItems -= 10;
            if (hasOBjectOrParam["Valor"] > 0) multiOtherRareItems+=5;
            allScoreText += System.Environment.NewLine + "Other unmakeable chest items (" + multiOtherRareItems + "/100)"; //>91

           
            float multiNiceToHave = 0;
            if (hasOBjectOrParam["Staff of Regrowth"] > 0) multiNiceToHave+=41;
            if (hasOBjectOrParam["Aglet"] > 0) multiNiceToHave+=24;
            if (hasOBjectOrParam["Anklet of the Wind"] > 0) multiNiceToHave+=15;
            if (hasOBjectOrParam["Feral Claws"] > 0) multiNiceToHave+=6;
            if (hasOBjectOrParam["Muramasa"] > 0) multiNiceToHave+=4;
            if (hasOBjectOrParam["Cobalt Shield"] > 0) multiNiceToHave+=4;
            if (hasOBjectOrParam["Cloud Starfury"] > 0 && hasOBjectOrParam["Cloud Ballon"] > 0 && hasOBjectOrParam["Cloud Horseshoe"] > 0) multiNiceToHave+=6; // >80
            allScoreText += System.Environment.NewLine + "Makeable nice to have chest items (" + multiNiceToHave + "/100)";

            float multiWorldPos = 0;     //positive                                           
            if (hasOBjectOrParam["Evil only one side"] == 0) multiWorldPos++;
            if (hasOBjectOrParam["Water Bolt before Skeletron"] == 0) multiWorldPos++;
            if (hasOBjectOrParam["Pyramid Carpet"] > 0 && hasOBjectOrParam["Pyramid Bottle"] > 0 && hasOBjectOrParam["Pyramid Mask"] > 0 && hasOBjectOrParam["Tree Chest Loom"] > 0) multiWorldPos++;
            if (hasOBjectOrParam["Enchanted Sword"] > 0) multiWorldPos += 1;
            allScoreText += System.Environment.NewLine + "Positive World properties (" + (int)(multiWorldPos / 4.0 * 100) + "/100)";


            float multiWorldNeg = 0;     //negative                   
            if(hasOBjectOrParam["Beach penalty max"] > 4200) multiWorldNeg++;
            if(hasOBjectOrParam["Cloud Chest"] < 3 ) multiWorldNeg++;            
            if(hasOBjectOrParam["Enchanted Sword Shrine"] == 0 && hasOBjectOrParam["Enchanted Sword"] < 2) multiWorldNeg++;//merged too one to reduce vanilla bug effect                  
            if(hasOBjectOrParam["Evil Tiles for Jungle Grass"] > 0) multiWorldNeg+=3;
            if(hasOBjectOrParam["Has evil Ocean"] > 0) multiWorldNeg+=2;            
            if(hasOBjectOrParam["Ice surface more than half evil"] == 1) multiWorldNeg+=3;
            if(hasOBjectOrParam["Alchemy Table"] < 2) multiWorldNeg++;
            if(hasOBjectOrParam["Alchemy Table"] < 1) multiWorldNeg++;
            if(hasOBjectOrParam["Sharpening Station"] < 2) multiWorldNeg++;
            if(hasOBjectOrParam["Sharpening Station"] < 1) multiWorldNeg++;
            if(hasOBjectOrParam["Different functional noncraf. Statues"] < 26) multiWorldNeg++;
            if(hasOBjectOrParam["Jungle cavern not blocked by structure"] == 0) multiWorldNeg++;
            if(hasOBjectOrParam["Pathlength to 40% cavern entrance"] > 350 || hasOBjectOrParam["Tiles to mine for 40% cavern"]>20 ) multiWorldNeg++;
            multiWorldNeg = (float)Math.Ceiling(0.5*multiWorldNeg);
            allScoreText += System.Environment.NewLine + "Negative World properties (" + (int)(multiWorldNeg/9.0*100) + "/100)";


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
            allScoreText += System.Environment.NewLine + "Boni total: (" + bonimult +"/12)";
                        
            //double reduceMiss = (scoreW.missingCount * 10);
            double bonScor = sumUp(bonimult, 13.37, 1.337);
            allScoreText += System.Environment.NewLine + "Bonus score: " + (int)bonScor;
            score += bonScor;
            string missingItems = AcceptConditons.GetMissingUnmakeAbleItems(scoreW);
            allScoreText += System.Environment.NewLine + "Missing unmakeable items types: (" + scoreW.missingCount + "/" + (scoreW.missingCount + scoreW.missingCountNot) + ")";

                    

            allScoreText += System.Environment.NewLine + "Seed "+ seed +  " total Score (beta): " + (int)score;



            //totod 4Pyramid min 1000 points ...............................

            writeScore(allScoreText);

            writeScore(System.Environment.NewLine + "====", true);
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
            writeScore(itemlist, true);
            
            allScoreText = itemlist + Environment.NewLine + Environment.NewLine + missingItems + Environment.NewLine + Environment.NewLine+ "Score(beta) for seed: " + allScoreText;

            writeScore(System.Environment.NewLine + "==== all stuff ", true);
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
            writeScore(itemlist, true);


            
            
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

        private static void createMapName(ScoreWorld score, bool valid, Configuration config, string worldNameByUser)
        {
            Dictionary<string, int> hasOBjectOrParam = score.hasOBjectOrParam; //Todo only ref

            string cseed = Main.ActiveWorldFileData.SeedText.PadLeft(10, '0');

            string sizeEvilDiff = Main.maxTilesX > 8000? "L" : Main.maxTilesX > 6000 ? "M" : "S";            
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
            if (hasOBjectOrParam["Chest Doub Glitch"] > 0) nextDigit = "D";
            else if (hasOBjectOrParam["Very Near Enchanted Sword"] > 0) nextDigit = "#";
            else if (hasOBjectOrParam["Spawn in Sky"] > 0) nextDigit = "Y";
            else if (hasOBjectOrParam["All chest items you can't craft or fish"] > 0) nextDigit = "@";
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

            if (hasOBjectOrParam["Open Temple"] > 0) strares += "_" + "OpenTemple";

            if (hasOBjectOrParam["Biome Item in normal Chest"] > 0) strares += "_" + "BiomeChestNormal";
            if (hasOBjectOrParam["Dungeon has strange Pos"] > 0) strares += "_" + "DungeonStrange";
            if (hasOBjectOrParam["Floating island cabin in Dungeon"] > 0) strares += "_" + "FloatingCabinDungeon";
            if (hasOBjectOrParam["Pre Skeletron Dungeon Chest Risky"] > 0) strares += "_" + "DungeonPreSkelChestRisky";
            if (hasOBjectOrParam["Pre Skeletron Dungeon Chest Grab"] > 0) strares += "_" + "DungeonPreSkelChestGrab";

            if (hasOBjectOrParam["Chest Doub Glitch"] > 0) strares += "_" + "ChestDoubGlitch";
            if (hasOBjectOrParam["Near Enchanted Sword near Tree"] > 0) strares += "_" + "NearESnearTree";
            if (hasOBjectOrParam["Near Enchanted Sword near Pyramid"] > 0) strares += "_" + "NearESnearPyramid";
            if (hasOBjectOrParam["Near Enchanted Sword"]- hasOBjectOrParam["Very Near Enchanted Sword"] > 0) strares += "_" + "NearEnchantedSword";
            if (hasOBjectOrParam["Enchanted Sword near Tree"]- hasOBjectOrParam["Near Enchanted Sword near Tree"] > 0) strares += "_" + "ESnearTree";
            if (hasOBjectOrParam["Enchanted Sword near Pyramid"]- hasOBjectOrParam["Near Enchanted Sword near Pyramid"] > 0) strares += "_" + "ESnearPyramid";
            if (hasOBjectOrParam["Very Near Enchanted Sword"]- hasOBjectOrParam["Near Enchanted Sword near Tree"]- hasOBjectOrParam["Near Enchanted Sword near Pyramid"] > 0) strares += "_" + "VeryNearES";
            if (hasOBjectOrParam["Floating Island without chest"] >0) strares += "_" + "CloudWithoutHouse";
            if (hasOBjectOrParam["All chest items you can't craft or fish"] > 0) strares += "_" + "AllChestItems";            
            if (hasOBjectOrParam["Number of Pyramids"] > Main.maxTilesY/600+1) strares += "_" + hasOBjectOrParam["Number of Pyramids"]+"Pyramids";


            // strares sscore content cseed Main.worldName
            string worldName = "";
           
            foreach(var ci in config.configList)
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
                worldName = cseed + "_" +sizeEvilDiff + "_" + content + "_" + sscore + "_" + strares;
            else
                worldName = worldName.Substring(1, worldName.Length - 1);

            Main.worldName = worldName + (valid ? "":"_unsure"); 

            Main.ActiveWorldFileData = WorldFile.CreateMetadata(Main.worldName, Main.ActiveWorldFileData.IsCloudSave, Main.expertMode);
            Main.ActiveWorldFileData.SetSeed(cseed);
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

            
            public int checkRares(ScoreWorld score,  int step, List<string> omitRare)
            {
                rares = 0;
                Dictionary<string, int> hasOBjectOrParam = score.hasOBjectOrParam;
                rareText = "";
                Func<string, int> checkAdd = (string name) =>
                {
                    if(hasOBjectOrParam[name]>0)
                        rareText += name + Environment.NewLine;

                    if (omitRare.Contains("Omit " + name))
                        return 0;
                    else                                            
                        return hasOBjectOrParam[name];                   

                };
                
                if (step >= 2)
                {


                    rares += checkAdd("Chest Doub Glitch");  //<--that might not find all int that step                    
                    rares += checkAdd("Pre Skeletron Dungeon Chest Risky");
                    rares += checkAdd("Pre Skeletron Dungeon Chest Grab");
                    rares += checkAdd("Biome Item in normal Chest"); //<--that might not find all int that step
                    rares += checkAdd("No Ocean");

                    if (!omitRare.Contains("Omit "+"Dungeon has strange Pos"))
                    {
                        rares += checkAdd("Dungeon in Snow Biome");
                        rares += checkAdd("Dungeon far above surface");
                        rares += checkAdd("Dungeon below ground");
                    }

                    int pyramids = has(ref hasOBjectOrParam, ItemID.SandstorminaBottle) + has(ref hasOBjectOrParam, ItemID.PharaohsMask) + has(ref hasOBjectOrParam, ItemID.FlyingCarpet);
                    rares += checkAdd("Many Pyramids");        

                    score.pyramids = pyramids; //overriten set geninfo ############## todo check for pyramid without chest, does exists?
                }

                if (step >= 3)
                {
                    //positive, check if can do in basic, do all in one?
                    rares += checkAdd("Near Enchanted Sword");
                    rares += checkAdd("Spawn in Sky");
                    rares += checkAdd("Open Temple");

                    rares += checkAdd("Enchanted Sword near Tree");
                    rares += checkAdd("Near Enchanted Sword near Tree");

                    rares += checkAdd("Enchanted Sword near Pyramid");
                    rares += checkAdd("Near Enchanted Sword near Pyramid");

                    rares += checkAdd("Very Near Enchanted Sword");

                    rares += checkAdd("Spawn in Jungle biome");
                    rares += checkAdd("Spawn in Snow biome");

                    rares += checkAdd("All chest items you can't craft or fish");

                    rares += checkAdd("Floating Island without chest"); //bugged if you do large map first and small after they are forced to create
                    rares += checkAdd("Floating island cabin in Dungeon");
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


                if (hasOBjectOrParam["Different functional noncraf. Statues"] < 26)
                {
                    itemlist += (26 - hasOBjectOrParam["Different functional noncraf. Statues"]).ToString() + " functional statue" + Environment.NewLine;
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
                resetPoints(score);
                conditionCheck="";//debug
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
                int cstep = -1;
                

                int phase3Count = 0;
                int phase2Count = 0;
                int phaseStartingpoints = 0;
                bool cPquerry = false;

                bool pointsReseted = false;
                int pointsB4Reset = 0;

                int dummyTool = 0; //adds /sub more than one point
                bool dummySet = false;


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
                                if (!Int32.TryParse(cci.value, out value) && !cci.name.Equals(OptionsDict.Phase3.continueEvaluation))
                                {
                                    writeDebugFile("could not parse value " + cci.value + " with lenght " + cci.value.Length + "for key " + cci.name + " in stage " + cstep + " cciphzzzase ");
                                }
                                if (!hasOBjectOrParam.ContainsKey(cci.name) && !cci.name.Equals(OptionsDict.Phase3.continueEvaluation) && !cci.name.Equals(OptionsDict.Tools.dummyPlus) && !cci.name.Equals(OptionsDict.Tools.dummyNeg))
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
                                        conditionCheck += "in negative cPquerry " + cPquerry + "  tested dummy " + cci.name + ": " +  value + "   , points:" + points + " stage " + cstep + Environment.NewLine;
                                    }
                                    else
                                    {
                                        points -= hasOBjectOrParam[cci.name] > value ? 1 : 0;
                                        conditionCheck += "in negative cPquerry " + cPquerry + "  tested " + cci.name + ": " + hasOBjectOrParam[cci.name] + " > " + value + "   , points:" + points + " stage " + cstep + Environment.NewLine;
                                    }
                                }
                                else
                                {
                                    if (cci.name.Equals(OptionsDict.Phase3.continueEvaluation) )
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
                                isInPositive = true; isInNegative = false;
                                points += cPquerry && lastType != Configuration.ConfigItemType.SelectableListPositive ? (dummySet ? dummyTool : 1) : 0;
                                cPquerry = true;
                                dummyTool = 0; dummySet = false;
                                conditionCheck += "positive now cPquerry " + cPquerry + "  tested   , points:" + points + " stage " + cstep + Environment.NewLine;
                                break;
                            case Configuration.ConfigItemType.SelectableListNegative:
                                points += cPquerry && lastType != Configuration.ConfigItemType.SelectableListPositive ? (dummySet ? dummyTool : 1) : 0;
                                cPquerry = false;
                                isInNegative = true; isInPositive = false;
                                dummyTool = 0; dummySet = false;
                                conditionCheck += "negative now cPquerry " + cPquerry + "  tested    , points:" + points + " stage " + cstep + Environment.NewLine;
                                break;
                            case Configuration.ConfigItemType.Header:
                                points += cPquerry && lastType != Configuration.ConfigItemType.SelectableListPositive ? (dummySet?dummyTool:1) : 0;
                                phaseStartingpoints = points;
                                cPquerry = false;
                                isInNegative = false; isInPositive = false;
                                dummyTool = 0; dummySet = false;
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
                        else if(cci.type != Configuration.ConfigItemType.SelectableText )
                        {
                            isInOmitRare = false;
                        }
                        else if(cci.type == Configuration.ConfigItemType.SelectableText && isInOmitRare)
                        {
                            if (cci.name.Equals(OptionsDict.GeneralOptions.omitRareAll)) omitRareAll = true;
                            else if(!omitRare.Contains(cci.name)) omitRare.Add(cci.name);                            
                        }else if (cci.type == Configuration.ConfigItemType.Header || cci.type == Configuration.ConfigItemType.Title)
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

                conditionCheck += "all done after now cPquerry " + cPquerry + "  ,total  points:" + points +"and rares " + rare+ " stage " + cstep + Environment.NewLine;


                if (pointsReseted)
                    points = Math.Max(pointsB4Reset, points);

                if (phase2Count == 0 && points > 0)
                    points++;
                if (phase3Count == 0 && (step!=1 || phase2Count==0) && points > 1)
                    points++;
                

                //writeDebugFile(" step " + step + " " + phase2Count + " 3:" + phase3Count + " p" + points);


                rares = rare;
                this.points = points;
                score.rare = rare;
                score.points = points;

                score.phase3Empty = phase3Count == 0 ? true : false;

                return points;
            }


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
                        else if (Main.tile[x, y].active() && Main.tile[x, y].type == TileID.Ruby || (Main.tile[x, y].type == TileID.ExposedGems && Main.tile[x, y].frameX == 72))
                            rubies.Add(new Tuple<int, int>(x, y));
                    }

                    rgbValues[indx++] = cc.B;
                    rgbValues[indx++] = cc.G; 
                    rgbValues[indx++] = cc.R; 
                    rgbValues[indx++] = cc.A;
                }

            
            //draw Spawm
            int aw = 0;

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
                                

                foreach (var itemloclist in score.itemLocation)
                {
                    if (itemloclist.Key != ItemID.StoneBlock && itemloclist.Key != ItemID.JungleShirt && itemloclist.Key != ItemID.JunglePants && itemloclist.Key != ItemID.PaladinsHammer && itemloclist.Key != ItemID.Ectoplasm)
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
                        DrawCircle(ref rgbValues, spot, scale, new Color(0, 200, 200));
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
                foreach (var ruby in rubies)
                {
                    DrawCircle(ref rgbValues, ruby, scale, new Color(255, 0, 180), 5);                    
                }
                foreach (var ruby in rubies)
                {                    
                    DrawCircle(ref rgbValues, ruby, scale, new Color(255, 0, 0), 3);
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


            Microsoft.Xna.Framework.Graphics.Texture2D cit = ModLoader.GetTexture("Terraria/Item_" + itemID);
            
            Color[] tc = new Color[cit.Width * cit.Height];
            cit.GetData<Color>(tc);

            int x = where.Item1/scale;
            int y = where.Item2/scale+4;
                        

            const int iconbgs = 48;

            
            while(y < Main.maxTilesY/scale - 4*iconbgs)
            {
                int offt = (y+2) * Main.maxTilesX * 4 / scale + x * 4 ;
                if (rgbValues[offt + 0] != (byte)255 || rgbValues[offt + 1] != (byte)255 || rgbValues[offt + 2] != (byte)255)
                    break;
                else
                    y += iconbgs-1;
                
            }

            for (int h = 0; h < iconbgs; h++)
                for (int w = 0; w < iconbgs; w++)
                {
                    int offt = (y + h ) * Main.maxTilesX * 4 / scale + x * 4 + w * 4 - iconbgs / 2 * 4;
                    //int imgof = h * iconbgs + w;

                    bool isWhite = ( (h- iconbgs / 2)* (h - iconbgs / 2) + (w- iconbgs / 2)* (w - iconbgs / 2) ) > (iconbgs * iconbgs / 4 );

                    rgbValues[offt + 0] = isWhite ? rgbValues[offt + 0] : (byte)255;
                    rgbValues[offt + 1] = isWhite ? rgbValues[offt + 1] : (byte)255;
                    rgbValues[offt + 2] = isWhite ? rgbValues[offt + 2] : (byte)255;
                    rgbValues[offt + 3] = isWhite ? rgbValues[offt + 3] : (byte)255;
                }



            int yoff = iconbgs/2 - cit.Height / 2;
            for (int h = 0; h < cit.Height; h++)
                for (int w = 0; w < cit.Width; w++)
                {
                    int offt = (y + yoff+ h ) * Main.maxTilesX * 4 / scale + x * 4 + w * 4 - cit.Width / 2 * 4;
                    int imgof = h * cit.Width + w;

                    bool isWhite = (tc[imgof].A == 0); //;&& ( (h- cit.Height/2)* (h - cit.Height / 2) + (w-cit.Width/2)* (w - cit.Width / 2) ) > (cit.Width* cit.Height / 4 );

                    rgbValues[offt + 0] = isWhite ? rgbValues[offt + 0] : tc[imgof].B;
                    rgbValues[offt + 1] = isWhite ? rgbValues[offt + 1] : tc[imgof].G;
                    rgbValues[offt + 2] = isWhite ? rgbValues[offt + 2] : tc[imgof].R;
                    rgbValues[offt + 3] = isWhite ? rgbValues[offt + 3] : tc[imgof].A;
                }
            
            tc = null;
        }

        public void DrawCircle(ref byte[] rgbValues, Tuple<int, int> where, int scale, Color cc, int iconbgs = 17)
        {
            //const int iconbgs = 17;


            int x = where.Item1 / scale ;
            int y = where.Item2 / scale - iconbgs/2+1;


            


            for (int h = 0; h < iconbgs; h++)
                for (int w = 0; w < iconbgs; w++)
                {
                    int offt = (y + h) * Main.maxTilesX * 4 / scale + x * 4 + w * 4 - iconbgs / 2 * 4;
                    //int imgof = h * iconbgs + w;
                    float val = ((h - iconbgs / 2) * (h - iconbgs / 2) + (w - iconbgs / 2) * (w - iconbgs / 2));
                    bool isWhite = (val <= (iconbgs * iconbgs / 3) && val >= (iconbgs * iconbgs / 6));

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
                for (int y = 0; y < Main.maxTilesY; y++)
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

                                        Console.Write(newPos.ToString() + " ");


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
