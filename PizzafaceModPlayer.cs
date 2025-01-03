using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace pizzafacemod
{
    public partial class PizzafaceModPlayer : ModPlayer
    {
        public bool bossbeenkilled = false;
       
        public bool lap2 = false;
        public bool lap3 = false;
        public bool lap2hasbeenentered = false;
        public bool lap3hasbeenentered = false;
        public bool hideplayer = false;
        public bool lap2spawnedboss = false;
        public bool lap3spawnedboss = false;
        public int lastbosstype;

        public bool showLapCount;



        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {   
            if (hideplayer == true)
            {
                drawInfo.hideEntirePlayer = true;
            }
            base.HideDrawLayers(drawInfo);
        }
        public override void UpdateDead()
        {
            hideplayer = false;
            lap2 = false;
            lap2hasbeenentered = false;
            lap3hasbeenentered = false;
            lap3 = false;
            lap2spawnedboss = false;
            lap3spawnedboss = false;
            lastbosstype = 0;
            base.UpdateDead();
        }

        public override void ResetInfoAccessories()
        {
            showLapCount = false;
        }
    }    
}
