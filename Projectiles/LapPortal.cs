using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pizzafacemod.NPCs;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;

using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Main;

namespace pizzafacemod.Projectiles
{
    public class LapPortal : ModProjectile
    {
        int camlerptimer;
        int fadeintimer = 0;
        int fadeouttimer = 0;
        bool dontspawnifcoffin = false;
        public override void SetStaticDefaults()
        {
            projFrames[Projectile.type] = 3;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 50;
            Projectile.height = 150;
            Projectile.timeLeft = 99999999;
            base.SetDefaults();
        }

        public override void AI()
        {
            PizzafaceModPlayer modPlayer = LocalPlayer.GetModPlayer<PizzafaceModPlayer>();
            Player player = LocalPlayer;
            Projectile.velocity *= 0;
            //DrawOriginOffsetY = 150;

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 2)
                    Projectile.frame = 0;
            }

            Projectile.timeLeft = 300;

            if (Projectile.ai[0] == 0)
            {
                if (Projectile.Distance(player.Center) <= 25)
                {
                    Projectile.ai[0] = 1;
                    SoundEngine.PlaySound(new SoundStyle("pizzafacemod/Projectiles/LapEnter"), Projectile.Center);

                }
            }

            if (Projectile.ai[0] == 1)
            {
                ++Projectile.ai[1];

                player.velocity = new Vector2(0, -0.4f);
                CameraPanSystem.PanTowards(Projectile.Center, 1 / 30f);
                CameraPanSystem.Zoom = Utilities.InverseLerp(0f, 1f, ++camlerptimer * 0.1f);
                hideUI = true;
                blockInput = true;
                player.immune = true;



                if (++Projectile.ai[1] >= 200)
                {
                    player.Spawn(PlayerSpawnContext.RecallFromItem);
                    player.velocity *= 0;
                    Projectile.ai[0] = 2;
                }
            }

            if (Projectile.ai[0] == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("pizzafacemod/Projectiles/LapExit"), LocalPlayer.Center);
                Projectile.Center = Main.LocalPlayer.Center;
                Projectile.ai[0] = 3;
                Projectile.ai[1] = 0;
            }

            /*if (modPlayer.lap2 == false)
            {
                Main.NewText("lap 2 is false");
            }*/

            if (Projectile.ai[0] == 3)
            {
                Projectile.ai[1] = 0;
                if (Projectile.ai[2] >= 100)
                {
                    hideUI = false;
                    blockInput = false;
                    player.immune = false;
                    
                }

                if (++Projectile.ai[2] >= 150)
                {
                    Projectile.Kill();
                    modPlayer.lap2hasbeenentered = true;
                    if (modPlayer.lap2hasbeenentered == true && modPlayer.lap2spawnedboss == false && ClientConfig.Instance.Lap2Skip == false)
                    {
                        SpawnBosses();
                        modPlayer.lap2spawnedboss = true;
                    }
                    Projectile.NewProjectile(Terraria.Entity.GetSource_None(), player.Center + new Vector2(0, -800), Vector2.Zero, ModContent.ProjectileType<LapFlag>(), 0, 0, -1, 0, 0, 0);

                    if (modPlayer.lap3 == true || ClientConfig.Instance.Lap2Skip == true)
                    {
                        modPlayer.lap3hasbeenentered = true;
                        if (modPlayer.lap3hasbeenentered == true && modPlayer.lap3spawnedboss == false)
                        {
                            SpawnBosses();
                            modPlayer.lap3spawnedboss = true;
                        }
                        NPC.NewNPCDirect(Terraria.Entity.GetSource_None(), (int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<Pizzaface>(), 0, -1, 0, 0, 0);
                    }
                }
            }
        }

        public void SpawnBosses()
        {
            PizzafaceModPlayer modPlayer = LocalPlayer.GetModPlayer<PizzafaceModPlayer>();
            Player player = LocalPlayer;
            if (modPlayer.lastbosstype == NPCID.WallofFleshEye || modPlayer.lastbosstype == NPCID.WallofFlesh)
            {
                NPC.SpawnWOF(Main.LocalPlayer.position);
            }

            if (ModLoader.TryGetMod("FargowiltasSouls", out Mod FargowiltasSouls))
            {
                if (FargowiltasSouls.TryFind("CursedCoffin", out ModNPC CursedCoffin))
                {
                    if (modPlayer.lastbosstype == CursedCoffin.Type)
                    {
                        dontspawnifcoffin = true;
                        NPC.NewNPC(player.GetSource_FromThis(), (int)player.Center.X, (int)player.Center.Y - 80, modPlayer.lastbosstype);


                    }
                }
            }

            if (modPlayer.lastbosstype != NPCID.WallofFlesh && modPlayer.lastbosstype != NPCID.WallofFleshEye && dontspawnifcoffin == false)
            {
                NPC.SpawnOnPlayer(player.whoAmI, modPlayer.lastbosstype);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            PizzafaceModPlayer modPlayer = LocalPlayer.GetModPlayer<PizzafaceModPlayer>();
            Texture2D Warning = ModContent.Request<Texture2D>(modPlayer.lap3 || ClientConfig.Instance.Lap2Skip == true ? "pizzafacemod/Projectiles/Lap3Warning" : "pizzafacemod/Projectiles/Lap2Warning", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle rectangle = new(0, 0, Warning.Width, Warning.Height);
            Vector2 origin2 = rectangle.Size() / 2f;
            Vector2 bluh = new Vector2(0, (float)Math.Sin(GameUpdateCount / 30f * MathHelper.TwoPi) * 5);
            if (Projectile.ai[0] != 1 && Projectile.ai[0] != 2 && Projectile.ai[0] != 3)
                EntitySpriteDraw(Warning, Projectile.Center + bluh - screenPosition + new Vector2(5, -8), new Rectangle?(rectangle), lightColor, 0, origin2, 1, SpriteEffects.None, 0);

            float opacity = 0;
            Texture2D Pixel = Luminance.Assets.MiscTexturesRegistry.Pixel.Value;
            Vector2 screenArea = ScreenSize.ToVector2();
            Vector2 pos = screenArea * 0.5f;
            Rectangle faderectangle = new(0, 0, Pixel.Width, Pixel.Height);
            if (Projectile.ai[1] >= 150)
            {
                opacity = Utilities.InverseLerp(0, 1, ++fadeintimer * 0.07f);
                //EntitySpriteDraw(Luminance.Assets.MiscTexturesRegistry.Pixel.Value, screenArea * 0.5f, null, Color.Black * opacity, 0f, Luminance.Assets.MiscTexturesRegistry.Pixel.Value.Size() * 0.5f, screenArea * 2f, 0, 0f);
            }

            if (Projectile.ai[2] > 0)
            {
                opacity = Utilities.InverseLerp(1, 0, fadeouttimer * 0.07f);
                if (Projectile.ai[2] >= 100)
                {
                    ++fadeouttimer;
                }
                Projectile.Opacity = 0;
                //EntitySpriteDraw(Luminance.Assets.MiscTexturesRegistry.Pixel.Value, screenArea * 0.5f, null, Color.Black * opacity, 0f, Luminance.Assets.MiscTexturesRegistry.Pixel.Value.Size() * 0.5f, screenArea * 2f, 0, 0f);
            }
            EntitySpriteDraw(Pixel, pos, new Rectangle?(faderectangle), Color.Black * opacity, 0f, faderectangle.Size() / 2f, screenArea * 2f, 0, 0f);
        }


    }
}
