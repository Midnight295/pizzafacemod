using pizzafacemod.NPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace pizzafacemod.Items
{
    public class Pizza : ModItem
    {
        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.scale = 1f;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Pizzaface>());
        }

        public override bool? UseItem(Player player)
        {
            NPC.NewNPC(player.GetSource_ItemUse(Item), (int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<Pizzaface>());
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Pizza, 1)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
