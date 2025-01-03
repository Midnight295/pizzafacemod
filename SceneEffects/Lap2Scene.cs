using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace pizzafacemod.SceneEffects
{
    public class Lap2Scene : ModSceneEffect
    {
        public string LapThemeStyle;

        public void ConfigNames()
        {
            if (ClientConfig.Instance.LapThemes == CharacterNames.Peppino)
            {
                LapThemeStyle = "Peppino";
            }

            if (ClientConfig.Instance.LapThemes == CharacterNames.Noise)
            {
                LapThemeStyle = "Noise";
            }

            if (ClientConfig.Instance.LapThemes == CharacterNames.Pizzelle)
            {
                LapThemeStyle = "Pizzelle";
            }
        }
        public override int Music => MusicLoader.GetMusicSlot("pizzafacemod/Music/Lap2" + LapThemeStyle);
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override bool IsSceneEffectActive(Player player)
        {
            PizzafaceModPlayer modPlayer = Main.LocalPlayer.GetModPlayer<PizzafaceModPlayer>();
            ConfigNames();
            return modPlayer.lap2hasbeenentered && ClientConfig.Instance.LapThemes != CharacterNames.Off && modPlayer.lap3hasbeenentered == false;
        }

        
    }
}
