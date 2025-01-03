using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace pizzafacemod.Items
{
    public class LapInfoDisplay : InfoDisplay
    {
        public static Color Lap1Color => new(255, 255, 255, Main.mouseTextColor);
        public static Color Lap2Color => new(152, 80, 248, Main.mouseTextColor);
        public static Color Lap3Color => new(234, 139, 11, Main.mouseTextColor);


        public override bool Active()
        {
            return Main.LocalPlayer.GetModPlayer<PizzafaceModPlayer>().showLapCount;
        }

        public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
        {
            PizzafaceModPlayer modplayer = Main.LocalPlayer.GetModPlayer<PizzafaceModPlayer>();
            int lapcount = 0;

            if (!modplayer.lap2hasbeenentered && !modplayer.lap3hasbeenentered)
            {
                displayColor = Lap1Color;
                lapcount = 1;
            }

            if (modplayer.lap2hasbeenentered)
            {
                displayColor = Lap2Color;
                lapcount = 2;
            }

            if (modplayer.lap3hasbeenentered)
            {
                displayColor = Lap3Color;
                lapcount = 3;
            }


            return Language.GetTextValue($"Mods.pizzafacemod.InfoDisplay.Lapcounter", lapcount);
        }
    }
}
