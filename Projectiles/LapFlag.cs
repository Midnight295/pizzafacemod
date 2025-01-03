using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace pizzafacemod.Projectiles
{
    public class LapFlag : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 400;
            Projectile.height = 150;
            Projectile.timeLeft = 99999999;
            Projectile.scale = 2f;
            Projectile.tileCollide = false;
            base.SetDefaults();
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("pizzafacemod/Projectiles/LapFlagSfx"), Projectile.Center);
            base.OnSpawn(source);
        }
        public override void AI()
        {
            Projectile.velocity += Main.LocalPlayer.velocity;
            DrawOriginOffsetY = 100;
            DrawOriginOffsetX = -275;
            //Projectile.Kill();
            Projectile.Center += Main.rand.NextVector2Circular(1, 1);
            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(Main.LocalPlayer.Center + new Microsoft.Xna.Framework.Vector2(0, -300)) * 4;
                Projectile.velocity += Main.LocalPlayer.velocity;
                if (Projectile.Distance(Main.LocalPlayer.Center + new Microsoft.Xna.Framework.Vector2(0, -300)) <= 50)
                {
                    Projectile.ai[0] = 1;
                }
            }
            if (Projectile.ai[0] == 1)
            {
                Projectile.velocity *= 0;
                Projectile.velocity += Main.LocalPlayer.velocity;
                if (++Projectile.ai[1] >= 60)
                {
                    Projectile.ai[0] = 2;
                }
            }

            if (Projectile.ai[0] == 2)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(Main.LocalPlayer.Center + new Microsoft.Xna.Framework.Vector2(0, -800)) * 4;
                Projectile.velocity += Main.LocalPlayer.velocity;
                if (Projectile.Distance(Main.LocalPlayer.Center + new Microsoft.Xna.Framework.Vector2(0, -800)) <= 50)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Main.LocalPlayer.GetModPlayer<PizzafaceModPlayer>().lap3 || ClientConfig.Instance.Lap2Skip == true ? ModContent.Request<Texture2D>("pizzafacemod/Projectiles/Lap3Flag").Value : ModContent.Request<Texture2D>("pizzafacemod/Projectiles/LapFlag").Value;
            int num156 = texture2D13.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Microsoft.Xna.Framework.Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Microsoft.Xna.Framework.Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}
