using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;

namespace pizzafacemod.Items
{
    public class Lapometer : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Radar);
        }

        public override void UpdateInfoAccessory(Player player)
        {
            player.GetModPlayer<PizzafaceModPlayer>().showLapCount = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
