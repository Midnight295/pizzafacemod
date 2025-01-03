using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace pizzafacemod.Particles
{
    public class Heatpuff : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.hostile = false;
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.scale = Main.rand.NextFloat(0.7f, 1.3f);
            Projectile.tileCollide = false;
            //Projectile.
            base.SetDefaults();
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > Main.projFrames[Projectile.type])
                    Projectile.Kill();
            }
            //Projectile.velocity *= 0;
            Projectile.velocity += new Microsoft.Xna.Framework.Vector2(0, -0.25f);
            //base.AI();
        }
    }
}
