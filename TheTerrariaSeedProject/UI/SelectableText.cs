using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;

using System.Threading;
using System.Threading.Tasks;

namespace TheTerrariaSeedProject.UI
{
    class SelectableText
    {
        string target;
        public List<string> values;
        public UITextPhrase self;
        UIListDescription targetUIdesc;
        OptionsDict opdict;
        UIList uilist;
        public SelectableList selList;
        InfoPanel infoPanel;
        Selectable thisAsSel;
        

        public SelectableText(UITextPhrase self, UIListDescription targetUIdesc, OptionsDict opdict, string header, string properties = "")
        {
            values = new List<string>();
            if(!properties.Equals("")) values.Add(properties);
            Create(self, targetUIdesc, opdict, header, values);
        }

        public SelectableText(UITextPhrase self, UIListDescription targetUIdesc, OptionsDict opdict, string header, string[] properties)
        {
            values = new List<string>();
            for (int i = 0; i < properties.Length; i++)
                values.Add(properties[i]);
            Create(self, targetUIdesc, opdict, header, values);
        }
        public SelectableText(UITextPhrase self, UIListDescription targetUIdesc, OptionsDict opdict, string header, List<string> properties)
        {
            values = properties;
            Create(self, targetUIdesc, opdict, header, values);
        }

        private void Create(UITextPhrase self, UIListDescription targetUIdesc, OptionsDict opdict, string header, List<string> properties)
        {
            
            this.opdict = opdict;
            target = header;
            values = properties!=null? properties.ToList(): new List<string>();
            if (values.Count > 1) values.Insert(0, "# " + header);
            this.self = self;
            this.targetUIdesc = targetUIdesc;
            if (properties != null && properties.Count > 0)
            {

                self.OnClick += select;               
                self.OnMouseOver += ChangeToGrey;
                self.OnMouseOut += ChangeToWhite;
                
            }
        }

        public void setCustomColor(Color color)
        {
            self.uitext.TextColor = color;
        }


        public void select(UIMouseEvent evt, UIElement listeningElement)
        {
            
            targetUIdesc.UpdateText(values);
            if (values.Count > 1 || (values.Count==1 && values[0].Equals(OptionsDict.SelectableList.removeTag)) )
            {
                //it is selectable
                foreach (var elem in targetUIdesc.entryList)
                {
                    if (elem.uitext.Text.Length > 0 && elem.uitext.Text.Substring(0, 1).Equals("#"))
                    {
                        elem.uitext.TextColor = Color.LightGray;
                        continue;
                    }
                    if (elem.uitext.Text.Length > 2 && elem.uitext.Text.Substring(0, 1).Equals("X"))
                    {
                        elem.uitext.TextColor = Color.PaleVioletRed;

                        elem.OnClick += RemoveOption;
                    }
                    else
                        elem.OnClick += SelectProp;
                    elem.OnMouseOver += ChangeToGrey;
                    elem.OnMouseOut += ChangeToWhite;
                }

            }

        }



        public SelectableText(UITextPhrase self, InfoPanel infoPanel, SelectableList selList, string header, List<string> properties, bool isOption = true)
        {
            this.opdict = infoPanel.opdict;
            this.uilist = infoPanel.uielem;
            this.selList = selList;
            this.infoPanel = infoPanel;
            target = header;
            values = properties!=null?properties.ToList(): new List<string>();            
            if (values.Count > 1 || (values.Count == 1 && values[0].Equals(OptionsDict.SelectableList.removeTag)) ) values.Insert(0, "# " + header);
            this.self = self;
            this.targetUIdesc = infoPanel.uidesc;
            if (properties != null && properties.Count > 0)
            {
                if (isOption)
                    self.OnClick += SelectOption;
                else
                    self.OnClick += select;

                self.OnMouseOver += ChangeToGrey;
                self.OnMouseOut += ChangeToWhite;
            }

        }



        public void SelectOption(UIMouseEvent evt, UIElement listeningElement)
        {
            targetUIdesc.UpdateText(values);
            if (values.Count > 1 )
            {
                //it is selectable
                foreach (var elem in targetUIdesc.entryList)
                {
                    if (elem.uitext.Text.Length > 0 && elem.uitext.Text.Substring(0, 1).Equals("#"))
                    {
                        elem.uitext.TextColor = Color.LightGray;
                        continue;
                    }
                    
                    elem.OnClick += SelectOptionProp;

                    elem.OnMouseOver += ChangeToGrey;
                    elem.OnMouseOut += ChangeToWhite;
                }

            }

        }


        private void ChangeToGrey(UIMouseEvent evt, UIElement listeningElement)
        {

            
            if(targetUIdesc!=null)
                targetUIdesc.uiss.changeHoverUI((listeningElement as UITextPhrase).text , evt.MousePosition.X, evt.MousePosition.Y, (listeningElement as UITextPhrase).Height.Pixels);
           


            Color cold = ((UITextPhrase)listeningElement).uitext.TextColor;
            ((UITextPhrase)listeningElement).uitext.TextColor = new Color(cold.R / 2, cold.G / 2, cold.B / 2, cold.A / 2);
        }
        private void ChangeToWhite(UIMouseEvent evt, UIElement listeningElement)
        {
            
            if (targetUIdesc != null)
                targetUIdesc.uiss.changeHoverUI("", 0, 0, 0);

            Color cold = ((UITextPhrase)listeningElement).uitext.TextColor;
            ((UITextPhrase)listeningElement).uitext.TextColor = new Color(cold.R * 2, cold.G * 2, cold.B * 2, cold.A * 2); ;
        }

        private void SelectProp(UIMouseEvent evt, UIElement listeningElement)
        {
            self.value = ((UITextPhrase)listeningElement).text;
            Update();
            targetUIdesc.ClearCurrent();
        }

        public void Update()
        {
            if(!self.text.Equals(OptionsDict.SelectableList.addingTag))
                if(self.value.Length == 0)
                    self.uitext.SetText(self.text);
                else
                    self.uitext.SetText(self.text + ": " + self.value);
        }



        
        private void SelectOptionProp(UIMouseEvent evt, UIElement listeningElement)
        {
            
            SelectOptionProp(((UITextPhrase)listeningElement).text);
        }


        public SelectableText SelectOptionProp(string selfValueHeaderDictText)
        {
            self.value = selfValueHeaderDictText;
            selList.elemNum += 1;
            
            float loc = selList.loc - 1f / (1f + selList.elemNum);
            UITextPhrase textHead = new UITextPhrase(loc, self.value);
            List<string> newEntries = opdict[self.value].ToList();            
            newEntries.Add(OptionsDict.SelectableList.removeTag);
            SelectableText selTex = new SelectableText(textHead, infoPanel, selList, self.value, newEntries, false);
            uilist.Add(textHead);
            thisAsSel = new Selectable(selTex);
            selTex.thisAsSel = thisAsSel;

            infoPanel.selectables.Add(thisAsSel);
            targetUIdesc.ClearCurrent();

            if (newEntries.Count > 1)
            {                
                if(selList.which.Equals(OptionsDict.Phase2.positive) || selList.which.Equals(OptionsDict.Phase3.positive))
                    thisAsSel.SetValue(newEntries[1]);
                else if (selList.which.Equals(OptionsDict.Phase2.negative) || selList.which.Equals(OptionsDict.Phase3.negative))
                    thisAsSel.SetValue(newEntries[Math.Max(0,(newEntries.Count-1)*3/4-1)]  );
                                
            }


            return selTex;
        }




        private void RemoveOption(UIMouseEvent evt, UIElement listeningElement)
        {
            
            targetUIdesc.ClearCurrent();
            values.Clear();            
            opdict = null;
            this.uilist.Remove(self);
            infoPanel.selectables.Remove(thisAsSel);            

        }



        public string GetValue()
        {
            return self.value;
        }
        public void SetValue(string val)
        {
            self.value = val;
            Update();
        }
        public string GetHeader()
        {
            return self.text;
        }
        public void SetHeader(string header)
        {
            self.text = header;
        }


    }
}
