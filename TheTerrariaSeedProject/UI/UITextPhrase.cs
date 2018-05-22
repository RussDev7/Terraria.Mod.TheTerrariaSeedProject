using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria.UI;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace TheTerrariaSeedProject.UI
{
    class UITextPhrase : UIElement
    {
        public UIText uitext;
        public string text;
        public string value;
        public float ind;
        

        public UITextPhrase(float ind, string text, Texture2D iconTex = null )
        {
            Width = StyleDimension.Fill;
            Height.Pixels = 15;
            
            this.ind = ind;
            this.text = text;
            value = "";
            uitext = new UIText(text);
            

            uitext.Left.Set(3, 0);
            //uitext.Width.Pixels = 50;

            if (iconTex != null)
            {
                UIImage icon = new UIImage(iconTex);

                this.Append(icon);
                this.MarginBottom = icon.Height.Pixels/2;
                uitext.MarginTop = icon.Height.Pixels/4;
                uitext.MarginLeft += icon.Width.Pixels;
            }
            Append(uitext);


        }
        
        public void SetTo(UITextPhrase src)
        {
            uitext = src.uitext;
            text = src.text;
            value = src.value;
            ind = src.ind;
        }

        public void SetTo(string text)
        {
            this.text = text;
            uitext.SetText(text);            
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
    }
}
