using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader.Config;

namespace pizzafacemod
{
    class ClientConfig : ModConfig
    {
        public static ClientConfig Instance;
        public override void OnLoaded()
        {
            Instance = this;
        }
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public const string ModName = "pizzafacemod";

        [Header("PizzaPursuit")]
        [DefaultValue(false)]
        public bool Lap2Skip;

        [Range(1f, 10f)]
        [Increment(1f)]
        [DefaultValue(5)]
        [Slider]
        [DrawTicks]
        public float PizzaSpeed;

        [Header("Music")]
        [DefaultValue(0)]
        [DrawTicks]        
        public CharacterNames LapThemes;

        
    }
}
