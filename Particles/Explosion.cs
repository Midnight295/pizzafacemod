using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace pizzafacemod.Particles
{
    public class Explosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 17;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.hostile = false;
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.scale = 0.75f;
            //Projectile.
            base.SetDefaults();
        }

        public override void AI()
        {
            DrawOriginOffsetX = -400;
            DrawOriginOffsetY = -125;
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > Main.projFrames[Projectile.type])
                    Projectile.Kill();
            }
            Projectile.velocity *= 0;
            //base.AI();
        }
    }
}
