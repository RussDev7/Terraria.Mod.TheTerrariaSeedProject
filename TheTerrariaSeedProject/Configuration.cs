using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TheTerrariaSeedProject.UI;
using Terraria.Utilities;

using Terraria.ID;
using Terraria;
using System.IO;

namespace TheTerrariaSeedProject
{
    class Configuration
    {
        class ConfigurationTaks : Dictionary<string, Action<Configuration, string>>
        {

            public ConfigurationTaks()
            {
                Add("", AddNewline);
                Add(OptionsDict.title, AddPhaseStart);
                Add(OptionsDict.Phase1.title, AddPhaseStart);
                Add(OptionsDict.Phase2.title, AddPhaseStart);
                Add(OptionsDict.Phase3.title, AddPhaseStart);

                Add(OptionsDict.GeneralOptions.title, AddPhaseStart);

                Add(OptionsDict.SelectableList.positive, AddSelectableList);
                Add(OptionsDict.SelectableList.negative, AddSelectableList);
                Add(OptionsDict.SelectableList.name, AddSelectableList);
                Add(OptionsDict.SelectableList.omitRare, AddSelectableList);

                Add(OptionsDict.Configuration.configName, AddInputField);
                Add(OptionsDict.WorldInformation.worldName, AddInputField);
                Add(OptionsDict.Configuration.startingSeed, AddInputField);


            }

            

        }


        public List<ConfigurationItem> configList;


        public readonly OptionsDict opdict = new OptionsDict();
        readonly static ConfigurationTaks configTask = new ConfigurationTaks();

        int phase;
        public string configName = "";
        public int numSeedSearch = 1;
        public string startSeed = "0";


        protected Configuration()
        {
            Init();
        }
        protected void Init()
        {
            phase = -1;
            configList = new List<ConfigurationItem>();
        }

        public static Configuration GenerateConfiguration(List<Selectable> selectables)
        {           
             
            Configuration config = new Configuration();
            selectables.Sort();

            foreach(var sel in selectables)
            {
                string head = sel.GetHeader();
                string value = sel.GetValue();
                config.PorcessHeadAndValue(head, value);

            }
                      
            
            return config;
        }

        public static Configuration LoadConfiguration(string pathToConfigName)
        {

            Configuration config = new Configuration();

            if (!File.Exists(pathToConfigName))
            {
                Main.PlaySound(SoundID.Frog);
                return null;
            }

            string[] lines = System.IO.File.ReadAllLines(pathToConfigName);

            foreach (var line in lines)
            {
                Tuple<string, string> headAndValue = ParseLine(line);

                string head = headAndValue.Item1;
                string value = headAndValue.Item2;

                //legacy compabil
                if (head.Equals(OptionsDict.Phase1.pyramidsPossibleLegacy))
                    head = OptionsDict.Phase1.pyramidsPossible;

                config.PorcessHeadAndValue(head, value);
            }
            
            return config;
        }


        private void PorcessHeadAndValue(string head, string value)
        {
            if (head.Length == 0)
            {
                this.InsertLine("");
                return;
            }

            if (configTask.ContainsKey(head + value))
                this.InsertLine(head + value);
            else if (configTask.ContainsKey(head) && value.Length != 0) this.InsertLine(head + ": " + value);
            else this.InsertLine(head + " : " + value); ;
        }


        public bool SaveConfigFile(string pathToFolder)
        {
            if (configList == null) return false;

            Tuple<string,string> confiNameContent = GetConfigAsString();
            string configName = confiNameContent.Item1;
            string confiContent = confiNameContent.Item2;
            

            if (!System.IO.Directory.Exists(pathToFolder))
                System.IO.Directory.CreateDirectory(pathToFolder);//somehow i got erros without if check            
            System.IO.File.WriteAllText(pathToFolder + "config" + configName + ".txt", confiContent);

            return true;
        }

        //return configName and configContent
        public Tuple<string,string> GetConfigAsString()
        {
            string configName = "";
            string confiContent = "";
                       

            string lastLine = "";
            foreach (var line in configList)
            {
                string thisLine = line.name + (line.value.Length != 0 ? ": " : "") + line.value + System.Environment.NewLine;

                if (lastLine.Equals(thisLine) && configTask.ContainsKey(line.name)) continue;//e.g 2 postive in a row without 1st having content

                lastLine = thisLine;

                confiContent += thisLine;
                if (line.name.Equals(OptionsDict.Configuration.configName)) configName = line.value.Equals("config") ? "" : line.value;

            }
            if (configName.Length > 6 && configName.Substring(0, 6).Equals("config"))
                configName = configName.Substring(6, configName.Length - ("config").Length);

            return new Tuple<string,string>(configName , confiContent) ;
        }



        private void InsertLine(string line)
        {
            int size = line.Length;
            bool isavilLine = configTask.ContainsKey(line);
            Tuple<string, string> lineCont = ParseLine(line);
            bool isavilHead = configTask.ContainsKey(lineCont.Item1);

            if (isavilLine) configTask[line](this, line);
            else if(isavilHead) configTask[lineCont.Item1](this, line);
            else if (size>0 && line[0].Equals("#")) AddText(this, line);
            else AddSelectableText(this, line);            
        }

        static private void AddNewline(Configuration config, string line)
        {
            config.configList.Add(new ConfigurationItem("", "", config.phase, ConfigItemType.NewLine));
        }
                
        static private void AddPhaseStart(Configuration config, string line)
        {
            ConfigItemType cit = ConfigItemType.Header;
            if (line.Equals(OptionsDict.title)) cit = ConfigItemType.Title;

            config.phase++;
            config.configList.Add(new ConfigurationItem(line, "", config.phase, cit));
        }
         

        static private void AddSelectableList(Configuration config, string line)
        {
            ConfigItemType cit;

            if (line.Equals(OptionsDict.SelectableList.positive)) cit = ConfigItemType.SelectableListPositive;
            else if (line.Equals(OptionsDict.SelectableList.negative)) cit = ConfigItemType.SelectableListNegative;
            else if (line.Equals(OptionsDict.SelectableList.omitRare)) cit = ConfigItemType.SelectableListOmitRare;
            else cit = ConfigItemType.SelectableListName;
                        
            config.configList.Add(new ConfigurationItem(line, "", config.phase, cit));
        }

        static private void AddText(Configuration config, string line)
        {            
            config.configList.Add(new ConfigurationItem(line, "", config.phase, ConfigItemType.Text));
        }

        static private void AddSelectableText(Configuration config, string line)
        {
            Tuple<string, string> lineCont = ParseLine(line);
            config.configList.Add(new ConfigurationItem(lineCont.Item1, lineCont.Item2, config.phase, ConfigItemType.SelectableText));

            if (lineCont.Item1.Equals(OptionsDict.Configuration.searchSeedNum))
                config.numSeedSearch = Int32.Parse(lineCont.Item2);
        }

        static private void AddInputField(Configuration config, string line)
        {
            Tuple<string, string> lineCont = ParseLine(line);
            config.configList.Add(new ConfigurationItem(lineCont.Item1, lineCont.Item2, config.phase, ConfigItemType.InputField));

            if (lineCont.Item1.Equals(OptionsDict.Configuration.configName))
                config.configName = lineCont.Item2;
            else if (lineCont.Item1.Equals(OptionsDict.Configuration.startingSeed))
                config.startSeed = lineCont.Item2;

        }

        static private Tuple<string,string> ParseLine(string line)
        {
            string[] headAndValue = line.Split(':');

            if (headAndValue.Length != 2)
                if (headAndValue.Length == 1)
                    headAndValue = new string[] { headAndValue[0], "" };
                else
                    headAndValue = new string[] { "something went wrong", "here" };

            return new Tuple<string, string>(headAndValue[0].Trim(), headAndValue[1].Trim());
        }





        public string FindConfigItemValue(string keyName, int phase)
        {
            foreach( var ci in configList)
            {
                if(ci.name.Equals(keyName) && ci.phase == phase)
                {
                    return ci.value;
                }
            }
            return "";
        }

        public bool InsertSelectableText(int phase, ConfigItemType belowWhat, string header, string value)
        {
            ConfigurationItem newItem = new ConfigurationItem(header, value, phase, ConfigItemType.SelectableText);
            int loc = 0;
            bool found = false;
            foreach(var elm in configList)
            {
                loc++;
                if(elm.phase == phase && elm.type == belowWhat)
                {
                    found = true;
                    break;
                }
            }
            if (found)
                configList.Insert(loc, newItem);
            else
                return false;

            return true;
        }

        public bool InsertPositiveList(int phase)
        {
            ConfigurationItem newItem = new ConfigurationItem(OptionsDict.SelectableList.positive, "", phase, ConfigItemType.SelectableListPositive);
            int loc = 0;
            bool found = false;
            foreach (var elm in configList)
            {
                
                if (elm.phase == phase && elm.type == ConfigItemType.SelectableListPositive)
                {
                    found = true;
                    break;
                }
                loc++;
            }
            if (found)
                configList.Insert(loc, newItem);
            else
                return false;

            return true;
        }

        public bool ChangeValueOfSelectableText(int phase, ConfigItemType belowWhat, string headerName, string newValue)
        {            
            int loc = 0;
            bool searchNow = false;
            bool found = false;
            foreach (var elm in configList)
            {               
                if (elm.phase == phase && elm.type == belowWhat && !searchNow)
                {
                    searchNow = true;
                    
                }
                else if(searchNow && elm.name.Equals(headerName)){
                    found = true;
                    break;
                }
                loc++;
            }

            if (found)
                configList[loc] = new ConfigurationItem(headerName, newValue, phase, configList[loc].type);
            else
                return searchNow;

            return true;            
        }


        public enum ConfigItemType { Title, Header, InputField, SelectableText, SelectableListPositive, SelectableListNegative, SelectableListName, SelectableListOmitRare, Text, NewLine, Other };
        public class ConfigurationItem
        {
            

            readonly public string name;
            readonly public string value;
            readonly public int phase;
            readonly public ConfigItemType type;


            public ConfigurationItem(string name, string value, int phase, ConfigItemType type)
            {                
                this.name = name;
                this.value = value;
                this.phase = phase;
                this.type = type;
            }

        }

    }

}
