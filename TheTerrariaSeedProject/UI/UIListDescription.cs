using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria;
using Microsoft.Xna.Framework.Graphics;

namespace TheTerrariaSeedProject.UI
{
    class UIListDescription : UIList
    {
        public int lastInd;
        float alignWidth;
        public bool isUpdating =false;

        public string fulltext;
        public List<UITextPhrase> entryList; // todo needed?

        UISearchSettings uiss;


        public UIListDescription(UISearchSettings uiss, float alignWidth = 100)
        {
            lastInd = 0; 
            fulltext = "";
            entryList = new List<UITextPhrase>();
            this.SetPadding(4);
            this.alignWidth = alignWidth;
            this.uiss = uiss;           
                
        }


        public void SetAlignWidth(float alignWidth)
        {
            this.alignWidth = alignWidth;
        }

        public void AddText(string text, bool isReshape = false)
        {
            isUpdating = true;
        
            String[] lines = text.Split('\n');



            for (int lineInd = 0; lineInd < lines.Length; lineInd++)
            {
                String[] substrings = lines[lineInd].Split(' ');

                string line = "";
                string lastline = "";


                Texture2D icon = null;
                bool thisLineStartsWithIcon = false;
                int iconwith = 0;

      

                
                for (int stringInd = 0; stringInd < substrings.Length; stringInd++)
                {
                    

                    line += (line.Equals("") ? "" : " ") + substrings[stringInd];



                    if (line.Length > 0 && line[0] == '@' && uiss.iconDict.ContainsKey(line))
                    {
                        icon = uiss.iconDict[line];
                        line = "";
                        thisLineStartsWithIcon = true;
                        iconwith = icon.Width;
                    }
                    else if (!thisLineStartsWithIcon)
                    {
                        icon = null;
                        iconwith = 0;
                    }

                        UITextPhrase entry = new UITextPhrase(lastInd, line, icon);
                    entry.uitext.HAlign = 0;

                    entry.uitext.SetText(line);
                    float wi = (entry.uitext.GetDimensions()).Width + iconwith; 
                    if (wi > alignWidth)
                    {
                        if (lastline.Equals(""))
                        {
                            lastline = line; //todo
                        }

                        entry.uitext.SetText(lastline);
                        thisLineStartsWithIcon = false;
                        iconwith = 0;
                        entry.HAlign = 0.5f;
                        this.Add(entry);
                        entryList.Add(entry);
                        line = substrings[stringInd];

                        lastline = "";
                        lastInd++;
                    }
                    else
                        lastline = line;

                }
                if (!line.Equals("") || ((lines[lineInd].Equals("") || lines[lineInd].Equals(" ")) && lineInd!=0)) 
                {
                    UITextPhrase entry = new UITextPhrase(lastInd++, line, icon);
                    
                    this.Add(entry);
                    entryList.Add(entry);
                }

            }


            if (!isReshape)                            
                fulltext += "\n"+text;
            
            


            isUpdating = false; ;
        }

        public void Rephrase(float alignWidth)
        {
            //if (isUpdating ) return;
            //List<string> newFulltext = fulltext.ToList();

            lastInd = 0; 
            this.alignWidth = alignWidth;
            this.Clear();
            entryList.Clear();

            //foreach (string line in newFulltext)
           AddText(fulltext, true);
            //fulltext = newFulltext;

        }


        public void UpdateText(string newText)
        {
            isUpdating = true;
            ClearCurrent();
            AddText(newText);
            Rephrase(alignWidth);
            isUpdating = false;
            

        }

        public void UpdateText(List<string> newText)
        {
            isUpdating = true;
            ClearCurrent();

            foreach (string line in newText)
                fulltext += "\n" + line;
            Rephrase(alignWidth);
            isUpdating = false;

        }
        
        public void UpdateText(string[] newText)
        {
            isUpdating = true;
            ClearCurrent();
            foreach (string line in newText)
                fulltext += "\n" + line;
            Rephrase(alignWidth);
            isUpdating = false;
        }
        

        public void ClearCurrent()
        {
            isUpdating = true;
            lastInd = 0;
            this.Clear();
            entryList.Clear();
            //fulltext.Clear();
            fulltext = "";
            Rephrase(alignWidth);
            isUpdating = false;
        }


    }
}
