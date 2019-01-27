using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

using Terraria.GameInput;
using Microsoft.Xna.Framework.Graphics;

namespace TheTerrariaSeedProject.UI
{
    class InfoPanel
    {
        public UIList uielem;
        public UIListDescription uidesc;
        public OptionsDict opdict;

        public List<Selectable> selectables;
        
        public InfoPanel(UIList uielem , UIListDescription uidesc, OptionsDict opdict)
        {
            this.uielem = uielem;
            this.uidesc = uidesc;
            this.opdict = opdict;
            selectables = new List<Selectable>();
            
        }

        public SelectableText AddSelectable(string header, string properties = "")
        {
            UITextPhrase textHead = new UITextPhrase(uielem.Count+1, header);
            SelectableText newSel = new SelectableText(textHead, uidesc, header, properties);
            selectables.Add(new Selectable(newSel));
            uielem.Add(textHead);
            return newSel;
        }
        public SelectableText AddSelectable(string header, string[] properties)
        {
            UITextPhrase textHead = new UITextPhrase(uielem.Count + 1, header);
            SelectableText newSel = new SelectableText(textHead, uidesc, header, properties);
            selectables.Add(new Selectable(newSel));
            uielem.Add(textHead);
            return newSel;
        }
        public SelectableText AddSelectable(string header, List<string> properties)
        {
            
            UITextPhrase textHead = new UITextPhrase(uielem.Count + 1, header);
            SelectableText newSel = new SelectableText(textHead, uidesc, header, properties);
            selectables.Add(new Selectable(newSel));
            uielem.Add(textHead);
            return newSel;
        }

        public SelectableText AddSelectable(string header, int from, int steps, int stepSize = 1, int factor1 = 1, int factor2 = 0)
        {
            UITextPhrase textHead = new UITextPhrase(uielem.Count + 1, header);
            List<string> values = new List<string>();
            int n = from;
            int cfactor = 0;
            int factor = 1;
            for (int i = 0; i < steps; i++)
            {
                cfactor = cfactor == factor1 && factor2 != 0 ? factor2 : factor1;
                factor *= cfactor;
                n = (from + i* stepSize) * factor;
                values.Add(n.ToString());
            }
            SelectableText newSel = new SelectableText(textHead, uidesc, header, values);
            selectables.Add(new Selectable(newSel));
            uielem.Add(textHead);            
            return newSel;
        }


        public const char listKindPositive = '+';
        public const char listKindNegative = '-';
        public const char listKindNaming = '*';
        public const char listKindOmitRare = '/';

        public SelectableText AddSelectableList(string selListFromDict, char kind)
        {
            SelectableList selList = new SelectableList(selListFromDict);
            return AddSelectableList(selList, kind);

        }
        
        public SelectableText AddSelectableList(SelectableList selList, char kind)
        {

            SelectableText selt;
            switch (kind)
            {
                case listKindPositive:
                    (selt = this.AddSelectable(OptionsDict.SelectableList.positive, OptionsDict.SelectableList.positiveDescription)).setCustomColor(Color.LightGreen); break;
                case listKindNegative:
                    (selt = this.AddSelectable(OptionsDict.SelectableList.negative, OptionsDict.SelectableList.negativeDescription)).setCustomColor(Color.PaleVioletRed); break;
                case listKindNaming:
                    (selt = this.AddSelectable(OptionsDict.SelectableList.name, OptionsDict.SelectableList.nameDescription)).setCustomColor(Color.LightGoldenrodYellow); break;
                case listKindOmitRare:
                    (selt = this.AddSelectable(OptionsDict.SelectableList.omitRare, OptionsDict.SelectableList.omitRareDescription)).setCustomColor(Color.LightGoldenrodYellow); break;


                default:
                    selt = this.AddSelectable("?VVVVVVV select VVVVVVV?", "here you can add what?"); break;
            }


            UITextPhrase textHead = new UITextPhrase(uielem.Count + 1, OptionsDict.SelectableList.addingTag);
            selList.elemNum = 0;
            selList.loc = uielem.Count +1;
            //SelectableText newSel = new SelectableText(textHead, uidesc, uielem, selList, opdict, "Add", opdict[selList.which]);sdfd
            SelectableText newSel = new SelectableText(textHead, this, selList, OptionsDict.SelectableList.addingTag, opdict[selList.which]);
            newSel.SetValue(selList.which);

            uielem.Add(textHead);
            newSel.setCustomColor(Color.SlateGray);
            //SelectableText bo = this.AddSelectable("'''''''''''''''''''''''''''''''''''''''''''''''''''''", new List<string>{});
            //if (isPositive) bo.setCustomColor(Color.LightGreen); else bo.setCustomColor(Color.PaleVioletRed); 
            return newSel;
        }


        public UISortInputText AddTextInput(string header)
        {

            UISortInputText textPanel = new UISortInputText(uielem.Count + 1, header);
            uielem.Add(textPanel);
            selectables.Add(new Selectable(textPanel));
            return textPanel;
        }

        

        public Selectable Search4ElementWithHeaderName(string headerName)
        {
            foreach(var sel in selectables)
            {
                if (sel.GetHeader().Equals(headerName))
                    return sel;
            }
            return null;


        }


    }

    class UISortInputText : UIPanel
    {
        public float ind;
        public NewUITextBox inputbox;
        UIText textHead;

        public UISortInputText(float ind, string header)
        {
            this.ind = ind;
            textHead = new UIText(header);
            // SelectableText newSel = new SelectableText(textHead, uidesc, header, properties);
            //selectables.Add(newSel);
            textHead.HAlign = 0f;
                        
            SetPadding(0);
            HAlign = 0f;
            VAlign = 0f;
            Width.Set(0, 1);
            Height.Set(15, 0);
            BackgroundColor = new Color(0, 0, 0, 1);
            BorderColor = new Color(0, 0, 0, 1);
            
            inputbox = new NewUITextBox("input here");
            

            Append(textHead);
            Append(inputbox);
        }
      

        public override int CompareTo(object obj)
        {
            UISortInputText otherIsP = obj as UISortInputText;
            UITextPhrase otherIsT = obj as UITextPhrase;

            if (otherIsP == null)
                return ind.CompareTo(otherIsT.ind);
            else
                return ind.CompareTo(otherIsP.ind);
        }

        public string GetValue()
        {
            return inputbox.currentString;
        }
        public void SetValue(string val)
        {
            inputbox.currentString = val;
        }
        public string GetHeader()
        {
            return textHead.Text;
        }
        public void SetHeader(string header)
        {
            textHead.SetText(header);
        }


    }

    class Selectable : IComparable
    {

        SelectableText selText;
        UISortInputText inputText;

        public Func<string> GetValue;
        public Action<string> SetValue;
        public Func<string> GetHeader;
        public float ind;

        public Selectable(SelectableText selText)
        {
            this.selText = selText;
            this.inputText = null;
            GetValue = GetValueSelText;
            SetValue = SetValueSelText;
            GetHeader = GetHeaderSelText;
            ind = selText.self.ind;
        }


        public  Selectable(UISortInputText inputText)
        {
            this.selText = null;
            this.inputText = inputText;
            GetValue = GetValueInputText;
            SetValue = SetValueInputText;
            GetHeader = GetHeaderInputText;
            ind = inputText.ind;
        }

        private string GetValueSelText()
        {
            return selText.GetValue();
        }
        public void SetValueSelText(string val)
        {
            selText.SetValue(val);
        }
        public void SetValuesSelText(List<string> values)
        {
            selText.values = values;
        }
        private string GetHeaderSelText()
        {
            return selText.GetHeader();
        }


        private string GetValueInputText()
        {
            return inputText.GetValue();
        }
        public void SetValueInputText(string val)
        {
            inputText.SetValue(val);
        }
        private string GetHeaderInputText()
        {
            return inputText.GetHeader();
        }


        public static int Compare(Selectable sel1, Selectable sel2)
        {
            return sel1.ind.CompareTo(sel2.ind);
        }

        public int CompareTo(object obj)
        {
            return ind.CompareTo((obj as Selectable).ind);
        }



    }




















}
