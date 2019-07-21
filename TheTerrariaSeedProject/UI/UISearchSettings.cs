using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Gamepad;
using Terraria.World.Generation;

using Terraria;
using Terraria.ModLoader;
using Terraria.Map;
using System.IO;
using System.Reflection;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheTerrariaSeedProject;
using Terraria.ModLoader.UI;
using System.Threading;



namespace TheTerrariaSeedProject.UI
{
    internal class UISearchSettings : UIState
    {
        private UIGenProgressBar progressBar = new UIGenProgressBar();
        private UIHeader progressMessage = new UIHeader();
        public GenerationProgress progress;

        public UIPanel buttonPanel;

        public UIPanel optionPanel;
        public UIList optionList;

        public UIPanel detailsPanel;
        public UIListDescription detailsList;

        public UIPanel helpPanel;

        public UIImageButton searchButton;
        public UIImageButton optionsButton;
        public UIImageButton stopButton;
        public UIImageButton positiveButton;
        public UIImageButton configLoadButton;
        public UIImageButton configSaveButton;
        public UIImageButton clearButton;
        public UIImageButton helpButton;

        public UIScrollbar optionListScrollbar;
        public UIScrollbar detailsListScrollbar;
        public float currentSize = 0;

        public WorldGenSeedSearch wgss; //made to public
        public OptionsDict opdict;
        public InfoPanel infopanel;


        

        const float panelWidth = 230;

        public Configuration currentConfig = null;

        public static class IconNames
        {
            public const string search = "@iconSearch";
            public const string options = "@iconOption";
            public const string configLoad = "@iconConfigLoad";
            public const string configSave = "@iconConfigSave";
            public const string positive = "@iconPositive";
            public const string reset = "@iconReset";
            public const string stop = "@iconStop";
            public const string help = "@iconHelp";
        }


        public bool writeText = false;
        public bool writeTextUpdating = false;
        public string writtenText = "";
            /*
            "..# 00.01.02.03.04.05.06.07.08.09.10.11.12 |++ \n" +
            "-----------------------------\n" +
            ".0# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".1# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".2# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".3# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".4# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".5# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".6# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".7# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            ".8# 00.00.00.00.00.00.00.00.00.00.00.00.00 #00 \n" +
            ".9# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            "10# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            "11# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            "12# 14.23.39.48.56.65.73.80.91.12.11.12.13 #88 \n" +
            "-----------------------------\n" +
            "++: 14.23.39.48.56.65.73.80.91.12.11.12.13 :88 \n";*/

        public string countText = "";


        public bool writeStats = false;
        public string writtenStats = "";

        public Dictionary<string, Texture2D> iconDict;

        public UISearchSettings(GenerationProgress progress, Mod mod, WorldGenSeedSearch wgss)
        {
            this.wgss = wgss;

            iconDict = new Dictionary<string, Texture2D>();
            iconDict.Add(IconNames.search, mod.GetTexture("search"));
            iconDict.Add(IconNames.options, mod.GetTexture("options"));
            iconDict.Add(IconNames.configLoad, mod.GetTexture("configLoad"));
            iconDict.Add(IconNames.configSave, mod.GetTexture("configSave"));
            iconDict.Add(IconNames.positive, mod.GetTexture("positive"));
            iconDict.Add(IconNames.reset, Main.trashTexture);
            iconDict.Add(IconNames.stop, mod.GetTexture("stop"));
            iconDict.Add(IconNames.help, mod.GetTexture("help"));//

            searchButton = new UIImageButton(mod.GetTexture("search"));
            optionsButton = new UIImageButton(mod.GetTexture("options"));
          

            configLoadButton = new UIImageButton(mod.GetTexture("configLoad"));
            configSaveButton = new UIImageButton(mod.GetTexture("configSave"));
            //clearButton = new UIImageButton(mod.GetTexture("clear"));

            clearButton = new UIImageButton(Main.trashTexture);
            positiveButton = new UIImageButton(mod.GetTexture("positive"));

            stopButton = new UIImageButton(mod.GetTexture("stop"));

            helpButton = new UIImageButton(mod.GetTexture("help"));

            float spacing = 3f;


      
            

            detailsPanel = new UIPanel();
            detailsPanel.SetPadding(5);
            detailsPanel.HAlign = 1f;
            detailsPanel.VAlign = 0.5f;
            detailsPanel.MarginRight = 19;
            detailsPanel.Width.Set(0, 0.45f);
            detailsPanel.Height.Set(0f, 0.5f);
            detailsPanel.BackgroundColor = new Color(73, 94, 171);
            Append(detailsPanel);



            detailsListScrollbar = new UIScrollbar();
            detailsListScrollbar.SetView(100f, 1000f);
            detailsListScrollbar.Height.Set(-12f, 1f);
            detailsListScrollbar.HAlign = 1f;
            detailsListScrollbar.Top.Pixels = 6;
            detailsListScrollbar.MarginRight = 2;

            detailsList = new UIListDescription(this);
            detailsList.Width.Set(0, 1f);
            detailsList.Height.Set(0f, 1f);
            detailsList.ListPadding = 12f;
            detailsList.MarginRight = 10;
            detailsPanel.Append(detailsList);

         

            detailsPanel.Append(detailsListScrollbar);
            detailsList.SetScrollbar(detailsListScrollbar);

            detailsList.Width.Set(-detailsListScrollbar.MarginRight - detailsListScrollbar.Width.Pixels, 1f);
            detailsList.SetAlignWidth(getDescListWith());

            optionPanel = new UIPanel();
            optionPanel.SetPadding(5);
            optionPanel.HAlign = 0f;
            optionPanel.VAlign = 0.5f;
            optionPanel.Left.Pixels = 20;
            optionPanel.Width.Set(0, 0.45f);
            optionPanel.Height.Set(0f, 0.5f);
            optionPanel.BackgroundColor = new Color(73, 94, 171);


            
            optionListScrollbar = new UIScrollbar();
            optionListScrollbar.SetView(100f, 1000f);
            optionListScrollbar.Height.Set(-12f, 1f);
            optionListScrollbar.HAlign = 1.0f;
            optionListScrollbar.Top.Pixels = 6;
            optionListScrollbar.MarginRight = 2;

            optionList = new UIList();
            optionList.Width.Set(-optionListScrollbar.MarginRight - optionListScrollbar.Width.Pixels, 1f);
            optionList.Height.Set(0f, 1f);
            optionList.ListPadding = 12f;
            optionPanel.Append(optionList);

            optionPanel.Append(optionListScrollbar);
            optionList.SetScrollbar(optionListScrollbar);

            opdict = new OptionsDict();
            infopanel = new InfoPanel(optionList, detailsList, opdict);

            Append(optionPanel);

                             
            
            helpPanel = new UIPanel();
            helpPanel.SetPadding(5);
            helpPanel.HAlign = 0.0f;
            helpPanel.VAlign = 0.25f;
            helpPanel.Top.Pixels = -2 * spacing - helpButton.Height.Pixels * 2;
            helpPanel.Left.Pixels = 20;
            helpPanel.BackgroundColor = new Color(73, 94, 171);
            helpPanel.Width.Set(helpButton.Width.Pixels + 2 * spacing + helpPanel.PaddingLeft + helpPanel.PaddingRight, 0f);
            helpPanel.Height.Set(helpButton.Height.Pixels + 2 * spacing + helpPanel.PaddingTop + helpPanel.PaddingBottom, 0f);

            helpButton.Left.Pixels = spacing;

            helpButton.OnMouseOver += helpHoverIn;
            helpButton.OnMouseOut += helpHoverOut;

            SelectableText tt = new SelectableText(new UITextPhrase(0, ""), detailsList, opdict, "", opdict.HelpDict[OptionsDict.Help.helpBut]);
            helpButton.OnClick += tt.select; //direct select without tt dummy don't work if you do it right after a seed was done -> collection changing size TODO
            helpPanel.Append(tt.self);

            helpPanel.Append(helpButton);
            Append(helpPanel);


            

            
            int spacingVFac = 4;
            buttonPanel = new UIPanel();
            buttonPanel.SetPadding(0);
            buttonPanel.HAlign = 0.5f;
            buttonPanel.VAlign = 0.5f;
            //buttonPanel.Top.Set(360f, 0f);			
            buttonPanel.Width.Set(searchButton.Width.Pixels + 2 * spacing, 0f);
            buttonPanel.BackgroundColor = new Color(73, 94, 171);

            float totalSpace = spacing * spacingVFac;


            buttonPanel.Append(searchButton);
            searchButton.OnClick += searchClick;
            searchButton.Left.Pixels = spacing;
            searchButton.Top.Pixels = totalSpace;            
            totalSpace += spacing * spacingVFac + 32;

            buttonPanel.Append(optionsButton);
            optionsButton.OnClick += optionsClick;
            optionsButton.Left.Pixels = spacing;
            optionsButton.Top.Pixels = totalSpace;
            totalSpace += spacing * spacingVFac + 32;

            totalSpace += spacing * spacingVFac * 1.5f;

            buttonPanel.Append(configLoadButton);
            configLoadButton.OnClick += configLoadClick;
            configLoadButton.Left.Pixels = spacing;
            configLoadButton.Top.Pixels = totalSpace;
            totalSpace += spacing * spacingVFac + 32;

            buttonPanel.Append(configSaveButton);
            configSaveButton.OnClick += configSaveClick;
            configSaveButton.Left.Pixels = spacing;
            configSaveButton.Top.Pixels = totalSpace;
            totalSpace += spacing * spacingVFac + 32;

            totalSpace += spacing * spacingVFac * 1.5f;

            buttonPanel.Append(positiveButton);
            positiveButton.OnClick += positiveClick;
            positiveButton.Left.Pixels = spacing;
            positiveButton.Top.Pixels = totalSpace;
            totalSpace += spacing * spacingVFac + 32;


            buttonPanel.Append(clearButton);
            clearButton.OnClick += clearClick;
            clearButton.Left.Pixels = spacing;
            clearButton.Top.Pixels = totalSpace;
            totalSpace += spacing * spacingVFac + 32;



            totalSpace += spacing * spacingVFac * 2;

            buttonPanel.Append(stopButton);
            stopButton.OnClick += stopClick;
            stopButton.Left.Pixels = spacing;
            stopButton.Top.Pixels = totalSpace;
            totalSpace += spacing * spacingVFac + 32;

            buttonPanel.Height.Pixels = totalSpace;
            Append(buttonPanel);

            this.progressBar.MarginBottom = 70;
            this.progressBar.HAlign = 0.5f;
            this.progressBar.VAlign = 1f;
            this.progressBar.Recalculate();
            this.progressMessage.CopyStyle(this.progressBar);
            UIHeader expr_78_cp_0 = this.progressMessage;
            expr_78_cp_0.MarginBottom = 120f;
            this.progressMessage.Recalculate();
            this.progress = progress;
            base.Append(this.progressBar);
            base.Append(this.progressMessage);

            hoverUI = new HoverItem(this);

            Init();
            currentConfig = Configuration.GenerateConfiguration(infopanel.selectables);
            currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, OptionsDict.GeneralOptions.omitRareAll, "");
            SetToConfiguration(currentConfig);

            
        }

        

        private void Init()
        {
            optionList.Clear();
            infopanel.selectables.Clear();


            int mapSizeMain = Main.maxTilesY / 600 - 1;
            string mapSize = mapSizeMain == 1 ? "Small" : mapSizeMain == 2 ? "Medium" : mapSizeMain == 3 ? "Large" : "Unknown";
            string diff = Main.expertMode ? "Expert" : "Normal";
            string evilType = WorldGen.WorldGenParam_Evil == 1 ? "Crimson" : WorldGen.WorldGenParam_Evil == 0 ? "Corruption" : "Random";

            addDictToInfo(OptionsDict.title).setCustomColor(Color.DarkOrange);


            addDictToInfo(OptionsDict.WorldInformation.worldSize).SetValue(mapSize);
            addDictToInfo(OptionsDict.WorldInformation.difficulty).SetValue(diff);
            addDictToInfo(OptionsDict.WorldInformation.evilType).SetValue(evilType);
            infopanel.AddTextInput(OptionsDict.Configuration.configName).SetValue("config");
            infopanel.AddTextInput(OptionsDict.WorldInformation.worldName).SetValue(Main.worldName);
            infopanel.AddTextInput(OptionsDict.Configuration.startingSeed).SetValue(Main.ActiveWorldFileData.Seed.ToString());
            addDictToInfo(OptionsDict.Configuration.searchSeedNum).SetValue("10000");
            addDictToInfo(OptionsDict.Configuration.stopSearchNum).SetValue("100");
            addDictToInfo(OptionsDict.Configuration.stepSize).SetValue("1");


            addDictToInfo(OptionsDict.Configuration.storeMMPic).SetValue("Off");
            addDictToInfo(OptionsDict.Configuration.storeStats).SetValue("Off");
            //addDictToInfo(OptionsDict.GeneralOptions.searchRare).SetValue(opdict[OptionsDict.GeneralOptions.searchRare][opdict[OptionsDict.GeneralOptions.searchRare].Count - 1]);
            addSelectListToInfo(OptionsDict.GeneralOptions.omitRare, InfoPanel.listKindOmitRare);
            addSelectListToInfo(OptionsDict.GeneralOptions.naming, InfoPanel.listKindNaming);


            addFreeLine();
            addDictToInfo(OptionsDict.Phase1.title).setCustomColor(Color.Orange);
            addDictToInfo(OptionsDict.Phase1.copperTin).SetValue("Random");
            addDictToInfo(OptionsDict.Phase1.ironLead).SetValue("Random");
            addDictToInfo(OptionsDict.Phase1.silverTungsten).SetValue("Random");
            addDictToInfo(OptionsDict.Phase1.goldPlatin).SetValue("Random");
            addDictToInfo(OptionsDict.Phase1.moonType).SetValue("Random");
            addDictToInfo(OptionsDict.Phase1.hallowSide).SetValue("Random");
            addDictToInfo(OptionsDict.Phase1.dungeonWallColor).SetValue("Random");
            addDictToInfo(OptionsDict.Phase1.dungeonSide).SetValue("Random");
            addDictToInfo(OptionsDict.Phase1.boost).SetValue("10");
            addDictToInfo(OptionsDict.Phase1.boostUGheightMin).SetValue("0");
            addDictToInfo(OptionsDict.Phase1.boostUGheightMax).SetValue("1000");
            addDictToInfo(OptionsDict.Phase1.boostHeightMin).SetValue("0");
            addDictToInfo(OptionsDict.Phase1.boostHeightMax).SetValue("300");
            addDictToInfo(OptionsDict.Phase1.boostCavernLayeroffsetMin).SetValue("0");
            addDictToInfo(OptionsDict.Phase1.boostCavernLayeroffsetMax).SetValue("300");
            addDictToInfo(OptionsDict.Phase1.boostMidPyramid).SetValue("0");
            if (!WorldGenSeedSearch.isPubRel)
            {
                //addDictToInfo(OptionsDict.Phase1.boostRockLayerOffset).SetValue("0");
                //addDictToInfo(OptionsDict.Phase1.boostSurfRockLayerOffset).SetValue("0");
                //addDictToInfo(OptionsDict.Phase1.boostSpawnRockSeedOffsetMin).SetValue("-1000");
                //addDictToInfo(OptionsDict.Phase1.boostSpawnRockSeedOffsetMax).SetValue("100");


                addDictToInfo(OptionsDict.Phase1.boostES).SetValue("0");
                addDictToInfo(OptionsDict.Phase1.boostESgran).SetValue("0");
                
                
                addDictToInfo(OptionsDict.Phase1.boostMidCloud).SetValue("-1");
                addDictToInfo(OptionsDict.Phase1.boostMidCloudInner2).SetValue("-1");
                addDictToInfo(OptionsDict.Phase1.boostMidCloudNum).SetValue("0");
                addDictToInfo(OptionsDict.Phase1.boostMidCloud1stLake).SetValue("0");
            }
            addDictToInfo(OptionsDict.Phase1.boostMidTree).SetValue("0");
            addDictToInfo(OptionsDict.Phase1.pyramidsPossible).SetValue("0");
            addDictToInfo(OptionsDict.Phase1.boostPyr).SetValue("0");

            addFreeLine();
            addDictToInfo(OptionsDict.Phase2.title).setCustomColor(Color.Orange);
            addSelectListToInfo(OptionsDict.Phase2.positive, InfoPanel.listKindPositive);
            addSelectListToInfo(OptionsDict.Phase2.negative, InfoPanel.listKindNegative);
                         


            addFreeLine();
            addDictToInfo(OptionsDict.Phase3.title).setCustomColor(Color.Orange);
            addDictToInfo(OptionsDict.Phase3.continueEvaluation).SetValue("Start new");
            addSelectListToInfo(OptionsDict.Phase3.positive, InfoPanel.listKindPositive);
            addSelectListToInfo(OptionsDict.Phase3.negative, InfoPanel.listKindNegative);


            InitCountText();
            writtenText = "";
            writtenStats = "";


        }

        const int statsDimPy = 8;
        const int statsDimCou = 15;
        private void InitCountText(){
            
            countText = "##. 00";

            for (int j = 1; j <= statsDimPy; j++)
            {
                countText += "."+ j.ToString().PadLeft(2, '0');
            }
            countText += " #.+" + Environment.NewLine; // + "--------------------------\n";

            for (int i = 0; i <= statsDimCou; i++)
            {
                countText += i.ToString().PadLeft(2, '.')+"# 00";
                for (int j = 1; j <= statsDimPy; j++)
                    countText += ".00";
                countText += " #00"+Environment.NewLine;
            }
            //countText += "--------------------------\n";
            countText += ".+# 00";
            for (int j = 1; j <= statsDimPy; j++)
            {
                countText += ".00";
            }
            countText += " ?00";
        }

        public void SetCountText(int[,] hasCount, int[] chanceCount)
        {
            //todo, very very slow
            for (int c = 0; c <= statsDimCou; c++) {
                int num = 0;
                for (int p = 0; p <= statsDimPy; p++)
                {
                    num += hasCount[c, p];
                    ChangeCountText(hasCount, chanceCount, c, p);

                }
                ChangeCountText(hasCount, chanceCount, c);
            }







        }



        private Action<char[], int, int, int> GetSetPowFV = (chars_, chance_, numPyr_, value_) =>
        {
            int temp = 1;
            int c = -1;

            while (temp <= value_)
            {
                c++;
                temp *= 10;
            }
            if (c == -1) c++;
            char pow10 = (char)(c + '0');
            char fd = (char)((10 * value_) / temp + '0');

            int lineLl = (statsDimPy + 3) * 3 + 1 + Environment.NewLine.Length;
            int xoff = (numPyr_> statsDimPy ? 5:4);

            int pos = lineLl * (chance_ + 1) + xoff + 3 * numPyr_;
            chars_[pos] = pow10;
            chars_[pos + 1] = fd;
        };

        public void ChangeCountText(int[,] hasCount, int[] chanceCount, int chance)
        {
            if (chance > statsDimCou) return;


            char[] chars = countText.ToCharArray();

            int value = chanceCount[chance];

            for(int i=0; i <= statsDimPy; i++)            
                value += hasCount[chance, i];            

            GetSetPowFV(chars, chance, statsDimPy+ 1 , value);

            value = 0;
            for (int i = 0; i <= statsDimCou; i++)
                value += chanceCount[i];

            GetSetPowFV(chars, statsDimCou +1, statsDimPy + 1, value);

            countText = new string(chars);

        }


        public void ChangeCountText(int[,] hasCount, int[] chanceCount, int chance, int numPyr)
        {
            if (chance > statsDimCou || numPyr > statsDimPy)
            {
                countText += "seed " + wgss.seed + " had a pyramid chance of " + chance + " and " + numPyr + " pyramids!"+ Environment.NewLine; 

                return;
            }


            char[] chars = countText.ToCharArray();
            
            int value = hasCount[chance, numPyr];
            GetSetPowFV(chars, chance, numPyr, value);

            value = 0;
            for (int i = 0; i <= statsDimCou; i++)
                value += hasCount[i, numPyr];

            GetSetPowFV(chars, statsDimCou + 1, numPyr, value);

            value = 0;
            for (int i = 0; i <= statsDimCou; i++)
                value += chanceCount[i];
            GetSetPowFV(chars, statsDimCou + 1, statsDimPy + 1, value);

            //WorldGenSeedSearch.writeDebugFile(" total chances " + value);
            countText = new string(chars); //tood faster                       

        }

        private void addFreeLine()
        {
            infopanel.AddSelectable("");
        }

        private SelectableText addDictToInfo(string what)
        {
            //bug avoid
            if (what.Equals(OptionsDict.WorldInformation.worldSize) && wgss.mediumWasDone && !wgss.largeWasDone)
                return infopanel.AddSelectable(what, opdict[OptionsDict.Bug.worldSizeBugMinMed]);
            else if (what.Equals(OptionsDict.WorldInformation.worldSize) && wgss.largeWasDone)
                return infopanel.AddSelectable(what, opdict[OptionsDict.Bug.worldSizeBugMinLarge]);

            return infopanel.AddSelectable(what, opdict[what]);
        }
        private SelectableText addSelectListToInfo(string what, char kind)
        {
            return infopanel.AddSelectableList(new SelectableList(what), kind);
        }



        float lastProgessMargin = 0;
        public void HideUnhideProgressBar()
        {
            lastProgessMargin = this.progressBar.MarginBottom;
            this.progressBar.MarginBottom = -this.progressBar.MarginBottom;
            //this.Recalculate();  can crash mod here -_> update
        }





        public bool rephrasing = false;
        int lastOptionSize = 0;
               
        public override void Update(GameTime gameTime)
        {
            if (detailsList != null && detailsPanel != null && detailsListScrollbar != null && !writeTextUpdating)
            {
                //changed to not write long list if select option
                //if (!writeTextUpdating && !detailsList.isUpdating && !rephrasing && (!wgss.inSearch || wgss.isInCreation) && (this.currentSize != (this.GetDimensions()).Width || writeText || ((detailsList.entryList.Count == 0) && writeStats) || (detailsList.entryList.Count == 0 && wgss.isInCreation && writtenText.Length > 0)))
                if (!writeTextUpdating && !detailsList.isUpdating && !rephrasing && (!wgss.inSearch || wgss.isInCreation) && ( (this.currentSize != (this.GetDimensions()).Width && detailsList.entryList.Count != 0) || writeText || ((detailsList.entryList.Count == 0) && writeStats) ))
                {                    
                    rephrasing = true;
                    writeText = true;

                    float currentSize = getDescListWith();
                    detailsList.fulltext = writtenStats + Environment.NewLine + writtenText; //if alawys true

                    detailsList.Rephrase(currentSize);
                    this.currentSize = (this.GetDimensions()).Width;
                    writeStats = false;
                    writeText = false;
                    rephrasing = false;
                }
                else
                if (!writeTextUpdating && !detailsList.isUpdating && !rephrasing && !wgss.inSearch && writeStats && !writeText )
                {
                    //if (!writeTextUpdating && !detailsList.isUpdating && !rephrasing && !wgss.inSearch && writeStats && !writeText && this.currentSize == (this.GetDimensions()).Width)

                    //also throws erro
                    if (this.currentSize == (this.GetDimensions()).Width)
                    {
                        rephrasing = true;
                        writeText = true;
                        if (detailsList.entryList.Count > 0)
                            detailsList.entryList[0].SetTo(writtenStats);
                        
                        writeStats = false;
                        writeText = false;
                        rephrasing = false;
                    }
                    else
                    {
                        this.currentSize = (this.GetDimensions()).Width;
                    }
                }
                
                if (infopanel != null && infopanel.uielem.Count != lastOptionSize && wgss.isInCreation)
                {

                    SetToConfiguration(Configuration.GenerateConfiguration(infopanel.selectables), true); // was false, test debug true, seems to work so far but should not be needed

                    lastOptionSize = infopanel.uielem.Count;
                    //WorldGenSeedSearch.writeDebugFile(" updated size to " + lastOptionSize);
                }

                if (infopanel != null && hoverUI != null)
                {
                    if (hoverUI.waitTime > 0 )
                    {
                        hoverUI.waitTime -= gameTime.ElapsedGameTime.TotalSeconds;
                        if ( (hoverUI.waitTime <= 0 && hoverUI.doUpdate) || hoverUI.hoverTitleNew.Length == 0 ) 
                        {
                            //hoverUI.doUpdate = true; //not needed, with those active it can happen ui does not go a away if you move close to border
                            hoverUI.Update(); 
                        }                       
                    }
                }
                

                if (this.progressBar!=null && lastProgessMargin != this.progressBar.MarginBottom)
                    this.Recalculate(); //something goes wrong herre with overhaul mod

                                


            }
        }



        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            this.progressBar.SetProgress(this.progress.TotalProgress, this.progress.Value);
            this.progressMessage.Text = this.progress.Message;
            this.UpdateGamepadSquiggle();                        
           
        }

        private float getDescListWith()
        {
            //chances are high something wrong here
            return (detailsPanel.GetDimensions()).Width - (detailsListScrollbar.GetDimensions()).Width - detailsListScrollbar.MarginRight -
                detailsListScrollbar.MarginLeft - 
                detailsList.MarginRight - detailsList.PaddingRight - detailsList.PaddingLeft -detailsPanel.PaddingRight;
        }


        public void UpdateProgress(GenerationProgress progress)
        {
            this.progress = progress;
            this.progressBar.SetProgress(this.progress.TotalProgress, this.progress.Value);
            this.progressMessage.Text = this.progress.Message;
        }

        private void UpdateGamepadSquiggle()
        {
            Vector2 value = new Vector2((float)Math.Cos((double)(Main.GlobalTime * 6.28318548f)), (float)Math.Sin((double)(Main.GlobalTime * 6.28318548f * 2f))) * new Vector2(30f, 15f) + Vector2.UnitY * 20f;
            UILinkPointNavigator.Points[3000].Unlink();
            UILinkPointNavigator.SetPosition(3000, new Vector2((float)Main.screenWidth, (float)Main.screenHeight) / 2f + value);
        }



        private void stopClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (wgss.inSearch)
            {
                wgss.ended = true;
                wgss.searchForSeed = false;
                wgss.stage = -1;
            }
            else if(wgss.isInCreation)
            {
                wgss.ended = true;
                wgss.searchForSeed = false;
                wgss.stage = -1;
            }

        }


        private void SetBackToDefault()
        {
            //if (wgss.isInCreation || !wgss.inSearch)
            if (wgss.isInCreation)
                if (!wgss.searchForSeed)
                {
                    Init();
                    infopanel.selectables.Sort();
                    currentConfig = Configuration.GenerateConfiguration(infopanel.selectables);
                    
                }
        }


        double lastClear = 0;
        private void clearClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (wgss.isInCreation || wgss.searchForSeed)            
            {
                if (Main.GlobalTime - lastClear > 3 || wgss.searchForSeed)
                {
                    //reverse changes
                    if (currentConfig != null)
                        SetToConfiguration(currentConfig);

                    if (wgss.isInCreation)
                    {
                        while (writeText || writeStats) Thread.Sleep(30);
                        writtenText = "";
                        while (writeText || writeStats) Thread.Sleep(30);
                        writtenStats = "";
                        writeText = true;
                    }
                }
                else
                    SetBackToDefault();

                lastClear = Main.GlobalTime;
            }
        }



        private void searchClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!wgss.searchForSeed && wgss.isInCreation)
            {
                currentConfig = Configuration.GenerateConfiguration(infopanel.selectables);
                
                //infopanel.selectables.Clear();
                infopanel.uidesc.ClearCurrent();


                InitCountText();                
                wgss.searchForSeed = true;
                wgss.stage = 0;
                

            }
        }

        private void optionsClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (wgss.inSearch && !wgss.isInCreation)
            {
                wgss.goToOptions(true);
            }
            else if (wgss.isInCreation)
            {
                if (detailsList != null && detailsList.entryList != null && detailsList.entryList.Count == 0 && !writeText && !writeStats)
                {
                    writeText = true;
                    writeStats = true;
                }
            }
        }


        private void configSaveClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!wgss.searchForSeed)
            {
                infopanel.selectables.Sort();

                Configuration config = Configuration.GenerateConfiguration(infopanel.selectables);

                config.SaveConfigFile(Main.SavePath + OptionsDict.Paths.configPath);
            }
        }

        private void configLoadClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!wgss.searchForSeed)
            {
                infopanel.selectables.Sort();
                Configuration config = Configuration.GenerateConfiguration(infopanel.selectables); //todo something faster to get name

                string configName = config.configName;

                if (configName.Length > 5 && configName.Substring(0, 6).Equals("config"))
                    configName = configName.Substring(6, configName.Length - ("config").Length);


                config = Configuration.LoadConfiguration(Main.SavePath + OptionsDict.Paths.configPath + "config" + configName + ".txt");

                if (config != null)
                    SetToConfiguration(config);
            }
        }

      

        int currentPositive = 0;
        private void positiveClick(UIMouseEvent evt, UIElement listeningElement)
        {
            //writeConfig();
            //WriteTextToDescList();

            if (!wgss.searchForSeed)
            {
                SetBackToDefault();
                string name = "config";
                if (currentPositive == 0)
                {
                    name = "Basics";
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, OptionsDict.GeneralOptions.omitRareAll, "");

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.SelectableText, OptionsDict.Configuration.configName, name);

                    currentConfig.ChangeValueOfSelectableText(1, Configuration.ConfigItemType.Header, OptionsDict.Phase1.pyramidsPossible, "3");


                    
                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.InputField, OptionsDict.Configuration.searchSeedNum, "1000");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Pyramid Bottle", "1");

                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Evil Tiles for Jungle Grass", "0");

                }
                else if (currentPositive == 1)
                {
                    name = "GoodSeed";
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Spawn in Snow biome", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Dungeon far above surface", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Pre Skeletron Dungeon Chest Risky", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, OptionsDict.GeneralOptions.omitRareAll, "");

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.SelectableText, OptionsDict.Configuration.configName, name);


                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.InputField, OptionsDict.Configuration.searchSeedNum, "10000");
                    currentConfig.ChangeValueOfSelectableText(1, Configuration.ConfigItemType.Header, OptionsDict.Phase1.pyramidsPossible, "4");

                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Tree Chest", "1");
                    currentConfig.InsertSelectableText(2,Configuration.ConfigItemType.SelectableListPositive, "Pyramid Carpet", "1");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Pyramid Bottle", "1");
                    

                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Evil Tiles for Jungle Grass", "0");

                }
                else if (currentPositive == 2)
                {
                    name = "ManyPyramid_JungleSnow";
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Spawn in Snow biome", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Dungeon far above surface", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Pre Skeletron Dungeon Chest Risky", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Enchanted Sword near Tree", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Enchanted Sword near Pyramid", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Near Enchanted Sword", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Floating Island without chest", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit No Ocean", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Chest duplication Glitch", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, OptionsDict.GeneralOptions.omitRareAll, "");

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.SelectableText, OptionsDict.Configuration.configName, name);

                    string size = currentConfig.FindConfigItemValue("World size", 0);
                    int numPyramid = size.Equals("Large") ? 6 : size.Equals("Medium") ? 5 : size.Equals("Small") ? 4 : 7;
                    int numPyramidChance = size.Equals("Large") ? 7 : size.Equals("Medium") ? 6 : size.Equals("Small") ? 5 : 10;
                    int numPyramidBoost = size.Equals("Large") ? 150 : size.Equals("Medium") ? 110 : size.Equals("Small") ? 70 : 150;



                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.InputField, OptionsDict.Configuration.searchSeedNum, "1000000");
                    currentConfig.ChangeValueOfSelectableText(1, Configuration.ConfigItemType.Header, OptionsDict.Phase1.pyramidsPossible, (numPyramidChance).ToString());
                    currentConfig.ChangeValueOfSelectableText(1, Configuration.ConfigItemType.Header, OptionsDict.Phase1.boost, (numPyramidBoost).ToString());

                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Snow biome surface overlap mid", "10");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Jungle biome surface overlap mid", "10");


                    currentConfig.InsertPositiveList(2);
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Number of Pyramids", numPyramid.ToString());
                    
                    

                }
                else if (currentPositive == 3)
                {
                    name = "VeryRare";
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Spawn in Snow biome", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Dungeon far above surface", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Pre Skeletron Dungeon Chest Risky", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Enchanted Sword near Tree", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Enchanted Sword near Pyramid", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Near Enchanted Sword", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Floating Island without chest", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit No Ocean", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Chest duplication Glitch", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, OptionsDict.GeneralOptions.omitRareAll, "");

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.SelectableText, OptionsDict.Configuration.configName, name);

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.InputField, OptionsDict.Configuration.searchSeedNum, "1000000");
                    currentConfig.ChangeValueOfSelectableText(1, Configuration.ConfigItemType.Header, OptionsDict.Phase1.pyramidsPossible, "4");
                    currentConfig.ChangeValueOfSelectableText(3, Configuration.ConfigItemType.Header, OptionsDict.Phase3.continueEvaluation, OptionsDict.Phase3.continueEvaluatioTakeOverTag);


                    currentConfig.InsertPositiveList(2);
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Snow biome surface overlap mid", "1000");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Jungle biome surface overlap mid", "500");

                    currentConfig.InsertPositiveList(2);
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Evil only one side", "1");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Tree Chest Loom", "1");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Pyramid Carpet", "1");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Pyramid Bottle", "1");
                                       

                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Has evil Ocean", "0");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Ice surface more than half evil", "0");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Distance Tree to mid", "700");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Distance Cloud to mid", "700");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Distance Pyramid to mid", "800");                    
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Evil Tiles for Jungle Grass", "0");



                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Staff of Regrowth", "1");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Flower Boots", "1");

                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListNegative, "Temple door distance", (Main.maxTilesX / 16 * 6).ToString() );                    
                }
                else if (currentPositive == 4)
                {
                    name = "ExtremelyRare";
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Spawn in Snow biome", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Dungeon far above surface", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Pre Skeletron Dungeon Chest Risky", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Enchanted Sword near Tree", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Enchanted Sword near Pyramid", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Near Enchanted Sword", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Floating Island without chest", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit No Ocean", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, "Omit Chest duplication Glitch", "");
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, OptionsDict.GeneralOptions.omitRareAll, "");

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.SelectableText, OptionsDict.Configuration.configName, name);

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.InputField, OptionsDict.Configuration.searchSeedNum, "10000000");
                    currentConfig.ChangeValueOfSelectableText(1, Configuration.ConfigItemType.Header, OptionsDict.Phase1.pyramidsPossible, "4");
                    
                    currentConfig.ChangeValueOfSelectableText(3, Configuration.ConfigItemType.Header, OptionsDict.Phase3.continueEvaluation, OptionsDict.Phase3.continueEvaluatioTakeOverTag);


                    currentConfig.InsertPositiveList(2);
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Snow biome surface overlap mid", "1000");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Jungle biome surface overlap mid", "500");

                    currentConfig.InsertPositiveList(2);
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Evil only one side", "1");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Tree Chest Loom", "1");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Pyramid Carpet", "1");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "Pyramid Bottle", "1");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, "All Dungeon Walls", "100");

                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Has evil Ocean", "0");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Ice surface more than half evil", "0");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Distance Tree to mid", "700");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Distance Cloud to mid", "700");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Distance Pyramid to mid", "800");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Distance Dungeon to mid", (Main.maxTilesX / 16 * 5).ToString());
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Jungle biome distance to mid", (Main.maxTilesX / 16 * 3).ToString());
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Snow biome distance to mid", (Main.maxTilesX / 16 * 3).ToString());

                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Evil Tiles for Jungle Grass", "0");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Evil Tiles for Sand", "20000");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Surface average height (aprox.)", "100");
                    


                    //currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Lava Charm", "1");
                    //currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Water Walking Boots", "1");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Staff of Regrowth", "1");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Flower Boots", "1");
                    //currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Aglet", "1");

                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListNegative, "Temple door horizontal distance", (Main.maxTilesX / 16 * 3).ToString());
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListNegative, "Temple at depth (%)", "30");
                    //currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListNegative, "Pathlength to Boots", "1300");
                    
                }
                else if (currentPositive == 5)
                {
                    name = "FlatWorld";
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, OptionsDict.GeneralOptions.omitRareAll, "");

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.SelectableText, OptionsDict.Configuration.configName, name);

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.InputField, OptionsDict.Configuration.searchSeedNum, "10000");
                                        
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListPositive, OptionsDict.Tools.dummyPlus, "1");
                    
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Surface average height (aprox.)", "100");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Surface height (sqrt) variance", "30");
                    currentConfig.InsertSelectableText(2, Configuration.ConfigItemType.SelectableListNegative, "Surface max-min height", "135");
                }
                else if (currentPositive == 6)
                {
                    name = "SpeedRun";
                    currentConfig.InsertSelectableText(0, Configuration.ConfigItemType.SelectableListOmitRare, OptionsDict.GeneralOptions.omitRareAll, "");

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.SelectableText, OptionsDict.Configuration.configName, name);

                    currentConfig.ChangeValueOfSelectableText(0, Configuration.ConfigItemType.InputField, OptionsDict.Configuration.searchSeedNum, "1000000");

                    currentConfig.InsertPositiveList(3);
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Pre Skeletron Muramasa Chest reachable", "1");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Pre Skeletron Golden Key Grab", "1");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Pre Skeletron Dungeon Chest Risky", "1");                    
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, OptionsDict.Tools.dummyPlus, "8");

                    currentConfig.InsertPositiveList(3);
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Pre Skeletron Muramasa Chest reachable", "1");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Pre Skeletron Golden Key Grab", "1");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Pre Skeletron Dungeon Chest Grab", "1");                    
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, OptionsDict.Tools.dummyPlus, "9");

                    currentConfig.InsertPositiveList(3);
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "neg. Pathlength to free ShadowOrb/Heart", "-1000");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Free ShadowOrb/Heart", "3");
                    //currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "neg. Pathlength to Boots", "-1500");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, OptionsDict.Tools.dummyPlus, "10");

                    currentConfig.InsertPositiveList(3);
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "neg. Pathlength to Bee Hive", "-1000");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "neg. Pathlength to Enchanted Sword", "-500");
                    //currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "neg. Pathlength to Boots", "-1500");
                    //currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Very Near Enchanted Sword", "1");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, OptionsDict.Tools.dummyPlus, "10");

                    //currentConfig.InsertPositiveList(3);
                    //currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Jungle cavern not blocked by structure", "1");
                    //currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Free cavern to mid Jungle", "1");
                    //currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, OptionsDict.Tools.dummyPlus, "1");
                                       
                    currentConfig.InsertPositiveList(3);
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Near Sunflower", "7");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, OptionsDict.Tools.dummyPlus, "1");

                    currentConfig.InsertPositiveList(3);
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Near Chest", "8");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, OptionsDict.Tools.dummyPlus, "1");
                                        
                    currentConfig.InsertPositiveList(3);
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Meteorite Bar unlocked", "1");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "neg. Pathlength to Teleport Potion", "-650");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, "Nearest Teleportation Potion count", "2");                    
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListPositive, OptionsDict.Tools.dummyPlus, "10");

                    
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListNegative, "Dungeon Distance", (Main.maxTilesX / 16 * 6).ToString());
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListNegative, "Temple door distance", (Main.maxTilesX / 16 * 6).ToString());
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListNegative, "Pathlength to Boots", "1000" );
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListNegative, "Pathlength to Iron/Lead Bar", "1000");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListNegative, "Pathlength to Gold/Platinum", "1000");

                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListNegative, "Pathlength to Slime Staute", "1500");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListNegative, "Pathlength to Shark Staute", "1500");
                    currentConfig.InsertSelectableText(3, Configuration.ConfigItemType.SelectableListNegative, "Pathlength to Anvil", "1500");
                    

                }



                SetToConfiguration(currentConfig);
                Selectable configNameInput = infopanel.Search4ElementWithHeaderName(OptionsDict.Configuration.configName);
                configNameInput.SetValue(name);

                currentPositive++;
                if (currentPositive > 6) currentPositive = 0;
                //writeText = true; 
            }            
        }

      
        public void WriteTextToDescList() { 
            if(writeTextUpdating) return;

            writeTextUpdating = true;
            detailsList.Clear();
            detailsList.UpdateText(writtenText);
            writeTextUpdating = false;
        }


        public void writeConfig()
        {
            infopanel.selectables.Sort();
            Configuration config = Configuration.GenerateConfiguration(infopanel.selectables); //todo something faster to get name

            string all = "";
            foreach (var cc in config.configList)
            {
                all += cc.name + "_ " + cc.value + "_ " + cc.phase + "_ " + cc.type + Environment.NewLine;
            }
            WorldGenSeedSearch.writeDebugFile(all);

        }




        public void SetToConfiguration(Configuration config, bool setToCurrent = true)
        {
            optionList.Clear();
                        

            infopanel.selectables.Clear();

            Configuration.ConfigItemType lastType = Configuration.ConfigItemType.Other;
            int stage = 0;
            bool inList = false;
            SelectableText lastAdd = null;
            foreach (Configuration.ConfigurationItem cci in config.configList)
            {
                if (cci.type == Configuration.ConfigItemType.SelectableListPositive && cci.type == lastType) continue;

                if ((lastType == Configuration.ConfigItemType.SelectableListPositive ||
                    lastType == Configuration.ConfigItemType.SelectableListNegative ||
                    lastType == Configuration.ConfigItemType.SelectableListName ||
                    lastType == Configuration.ConfigItemType.SelectableListOmitRare) &&
                    (cci.type == Configuration.ConfigItemType.SelectableText))
                        inList = true;
                if(cci.type != Configuration.ConfigItemType.SelectableText)
                        inList = false;

                if (cci.type == Configuration.ConfigItemType.SelectableListNegative && lastType != Configuration.ConfigItemType.SelectableListPositive)
                    addSelectListToInfo(stage == 2 ? OptionsDict.Phase2.positive : OptionsDict.Phase3.positive, InfoPanel.listKindPositive); //add another positve

                switch (cci.type)
                {
                    case Configuration.ConfigItemType.Title:
                        addDictToInfo(OptionsDict.title).setCustomColor(Color.DarkOrange);
                        break;
                    case Configuration.ConfigItemType.Header:
                        addDictToInfo(cci.name).setCustomColor(Color.Orange);
                        stage++;
                        break;
                    case Configuration.ConfigItemType.InputField:
                        infopanel.AddTextInput(cci.name).SetValue(cci.value);
                        break;
                    case Configuration.ConfigItemType.NewLine:
                        addFreeLine();
                        break;
                    case Configuration.ConfigItemType.SelectableText:
                        if (opdict.ContainsKey(cci.name))
                        {
                            //addDictToInfo(cci.name).SetValue(cci.value);
                            if (inList)
                                lastAdd.SelectOptionProp(cci.name).SetValue(cci.value);
                            else
                                addDictToInfo(cci.name).SetValue(cci.value);
                        }
                        //else
                            //infopanel.AddSelectable(cci.name, OptionsDict.vForUnknown); // <-- breaks formating
                        
                        break;
                        
                    case Configuration.ConfigItemType.SelectableListPositive:
                        lastAdd = addSelectListToInfo(stage== 2? OptionsDict.Phase2.positive : OptionsDict.Phase3.positive, InfoPanel.listKindPositive);
                        break;
                    case Configuration.ConfigItemType.SelectableListNegative:
                        lastAdd = addSelectListToInfo(stage == 2 ? OptionsDict.Phase2.negative : OptionsDict.Phase3.negative, InfoPanel.listKindNegative);                        
                        break;
                    case Configuration.ConfigItemType.SelectableListName:
                        lastAdd = addSelectListToInfo(OptionsDict.GeneralOptions.naming, InfoPanel.listKindNaming);
                        break;
                    case Configuration.ConfigItemType.SelectableListOmitRare:
                        lastAdd = addSelectListToInfo(OptionsDict.GeneralOptions.omitRare, InfoPanel.listKindOmitRare);
                        break;
                    case Configuration.ConfigItemType.Text:
                        infopanel.AddSelectable(cci.name);
                        break;
                }


                lastType = cci.type;
            }

            infopanel.selectables.Sort();
            if (setToCurrent)
            {
                currentConfig = Configuration.GenerateConfiguration(infopanel.selectables);
                
            }
            //infopanel.uielem.RecalculateChildren();
            //infopanel.uielem.Recalculate();



        }

        HoverItem hoverUI = null;
        class HoverItem
        {
            const int hoverInforBoxWidth = 500;
            public UIPanel hoverInfo =null;            
            UIListDescription elements = null;
            UISearchSettings uiss = null;

            const int mysteriousExtraSpace = 4;
            public HoverItem(UISearchSettings uiss)
            {
                this.uiss = uiss;
                hoverInfo = new UIPanel();
                hoverInfo.SetPadding(6);
                hoverInfo.PaddingBottom = 0;
                hoverInfo.MarginLeft = 13370;
                hoverInfo.MarginRight = 0;
                hoverInfo.HAlign = 0;
                hoverInfo.VAlign = 0;


                elements = new UIListDescription(null, hoverInforBoxWidth);
                elements.Width.Set(0, 1f);
                elements.Height.Set(0f, 1f);
                elements.ListPadding = 12f;
                elements.MarginLeft = 0;
                elements.MarginRight = 0;
                elements.PaddingLeft = 0;
                elements.PaddingRight = 0;


                hoverInfo.Append(elements);
                uiss.Append(hoverInfo);
            }

            public string hoverTitleCurrent = "abc";
            public string hoverTitleNew = "";
            public float locX = 0;
            public float locY = 0;
            public float offY = 0;

            public bool doUpdate = false;
            public double waitTime = 0;

            public void Update()
            {
                string newTitle = hoverTitleNew;
                string descText = newTitle;
                bool containsNeg = false;
                bool containsPathlength = false;
                bool containsDistance = false;
                waitTime = 0;
                


                if (descText.Contains("neg. ") && !uiss.opdict.HelpDict.ContainsKey(descText))
                {
                    containsNeg = true;
                    descText = descText.Substring(("neg. ").Length);
                }
                else if (descText.Contains("Omit ") && !uiss.opdict.HelpDict.ContainsKey(descText))
                {
                    descText = descText.Substring(("Omit ").Length);
                }
                if (descText.Contains(OptionsDict.Phase3.pathlength))
                {
                    containsPathlength = true;
                }
                else if (descText.Contains(OptionsDict.GeneralOptions.distance) || descText.Contains(OptionsDict.GeneralOptions.distanceS))
                {
                    containsDistance = true;
                }

                if (newTitle.Length == 0 || descText.Length == 0 || uiss.opdict == null || uiss.opdict.HelpDict == null || (!uiss.opdict.HelpDict.ContainsKey(descText) && !containsNeg && !containsPathlength && !containsDistance))
                {
                    if (hoverInfo != null)
                    {
                        hoverInfo.MarginLeft = (hoverInfo.MarginLeft % 100000) + 100000;
                        doUpdate = true;
                    }
                    hoverTitleCurrent = newTitle;
                    //WorldGenSeedSearch.writeDebugFile(" it is 0 ");

                    return;
                }


                if (uiss.opdict.HelpDict.ContainsKey(descText))
                    descText = uiss.opdict.HelpDict[descText];
                else
                    descText = newTitle;
                if (containsPathlength)
                    descText += (descText.Length != 0 ? " \n" : "") + uiss.opdict.HelpDict[OptionsDict.Phase3.pathlength];
                else if (containsDistance)
                    descText += (descText.Length != 0 ? " \n" : "") + uiss.opdict.HelpDict[OptionsDict.GeneralOptions.distance];
                if (containsNeg)
                    descText += (descText.Length != 0 ? " \n" : "") + uiss.opdict.HelpDict["neg. "];


                float totalH = uiss.GetOuterDimensions().Height;
                float totalW = uiss.GetOuterDimensions().Width;
                float mX = locX;
                float mY = locY;

                float panelWidth = hoverInforBoxWidth + hoverInfo.PaddingLeft + hoverInfo.PaddingRight + mysteriousExtraSpace;
                hoverInfo.Width.Set(panelWidth, 0);
                //hoverInfo.BackgroundColor = new Color(73, 94, 171);
                hoverInfo.BackgroundColor = new Color(88, 109, 186);
                hoverInfo.MarginLeft = mX - (mX > totalW - panelWidth ? panelWidth - (totalW - mX) : 0);
                
                elements.UpdateText(descText);
                elements.Recalculate();
                float boxH = elements.GetTotalHeight() + hoverInfo.PaddingTop + hoverInfo.PaddingBottom;
                hoverInfo.Height.Set(boxH, 0);
                hoverInfo.MarginTop = mY + ((mY > totalH - boxH) ? -offY - 0.5f * elements.ListPadding - boxH : offY + 0.5f * elements.ListPadding);

                if(elements.maxSize < 0.95f*panelWidth)
                    hoverInfo.Width.Set(elements.maxSize +hoverInfo.PaddingLeft + hoverInfo.PaddingRight + mysteriousExtraSpace, 0);


                hoverTitleCurrent = newTitle;
                
                if(hoverTitleNew.Equals(newTitle))
                    doUpdate = false;

            }

        }
                       
        
        public void changeHoverUI(string lookUp, float mx, float my, float offsetH, float waitTime = 1.0f)
        {
            //if (!targetUIdesc.uiss.wgss.isInCreation && targetUIdesc.uiss.wgss.stage <= 1)
            //    return;

            //WorldGenSeedSearch.writeDebugFile(" do " + lookUp + " :l="+(lookUp.Length) +" at " + mx +":" +my  + " cur:" + hoverUI.hoverTitleCurrent + " new:" + hoverUI.hoverTitleNew);

            if (lookUp.Equals(hoverUI.hoverTitleCurrent) && !hoverUI.doUpdate)
                return;
            //WorldGenSeedSearch.writeDebugFile(" passed ");

            if (!lookUp.Equals(hoverUI.hoverTitleCurrent))
                hoverUI.doUpdate = true;

            hoverUI.hoverTitleNew = lookUp;
            hoverUI.locX = mx;
            hoverUI.locY = my;
            hoverUI.offY = offsetH;           
            hoverUI.waitTime = waitTime;
        }

        public void helpHoverIn(UIMouseEvent evt, UIElement listeningElement)
        {
            changeHoverUI(OptionsDict.Help.helpButHover, evt.MousePosition.X, evt.MousePosition.Y, listeningElement.Height.Pixels * 0.5f, 0.1f);
        }
        public void helpHoverOut(UIMouseEvent evt, UIElement listeningElement)
        {
            changeHoverUI("", 0, 0, 0);
        }


    }

}
