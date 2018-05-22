using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;

namespace TheTerrariaSeedProject.UI
{
    class SelectableList
    {

        
       
        public string which;
        public int elemNum;
        public float loc;

        public SelectableList( string which)
        {
            elemNum = 0;
            loc = 0;
            this.which = which;
            

        }

        

    }
}
