using Luminance.Core.Sounds;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pizzafacemod.Particles;
using pizzafacemod.Projectiles;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace pizzafacemod.NPCs
{
    [AutoloadBossHead]
    public class Pizzaface : ModNPC
    {
        int phasedelay;
        int idledelay;
        int movementlerpslow;
        int movementlerpnormal;
        int explosiontimer;
        int deathtimer;
        int smoketimer;
        int cloudtimer;
        int afterimagetimer;
        public LoopedSoundInstance Loop;
        public LoopedSoundInstance Loop2;
        bool haslaughsoundbeenplayed;
        bool wasjustidle = false;
        bool wasjustslow = false;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.TrailingMode[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            Main.npcFrameCount[NPC.type] = 70;
            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = Vector2.UnitY * 40,
                PortraitScale = 0.9f,
                //PortraitPositionXOverride = 0,
                //PortraitPositionYOverride = 15
            });

        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.scale = 1.3f;
            NPC.height = 50;
            NPC.knockBackResist = 0f;
            NPC.Opacity = 1;
            NPC.lifeMax = 100000;
            NPC.aiStyle = -1;        
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            //SceneEffectPriority = SceneEffectPriority.;

            if (NPC.IsABestiaryIconDummy)
            {
                NPC.lifeMax = 999999;
                NPC.damage = 999999;
                NPC.defense = 99999;
            }
            
            base.SetDefaults();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            ContentSamples.NpcBestiaryRarityStars[ModContent.NPCType<Pizzaface>()] = 5;
            BestiaryUICollectionInfo info;
            if (NPC.active)
            {   
                info.UnlockState = BestiaryEntryUnlockState.CanShowDropsWithDropRates_4;
            }        
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement($"Mods.{Mod.Name}.Bestiary.{Name}")
            });
        }

        public override void OnSpawn(IEntitySource source)
        {
            Main.BestiaryTracker.Kills.RegisterKill(NPC);
            NPC.ai[0] = -1;
            base.OnSpawn(source);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.KillMe(PlayerDeathReason.ByCustomReason(Language.GetTextValue($"Mods.pizzafacemod.DeathReasons." + Main.rand.Next(1, 10), target.name)), 1000, 0, false);         
            base.OnHitPlayer(target, hurtInfo);
        }

        public override void AI()
        {
            PizzafaceModPlayer modPlayer = Main.LocalPlayer.GetModPlayer<PizzafaceModPlayer>();
            if (NPC.ai[0] >= -1)
            {
                NPC.DiscourageDespawn(9999);
            }        
            DrawOffsetY = 50;

         

            //spawn animation
            if (NPC.ai[0] == -1)
            {
                NPC.velocity *= Vector2.Zero;
                NPC.damage = 0;
            }

            //death animation
            if (NPC.ai[0] == -2)
            {
                NPC.velocity *= Vector2.Zero;
                NPC.Center += Main.rand.NextVector2Circular(2, 2);

                if (++explosiontimer >= 25)
                {
                    SoundEngine.PlaySound(new SoundStyle("pizzafacemod/NPCs/DeathExplosion"), NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_None(), NPC.Center + Main.rand.NextVector2Circular(50, 50), Vector2.Zero, ModContent.ProjectileType<Explosion>(), 0, 0, -1, 0, 0, 0);
                    explosiontimer = 0;
                }

                if (++deathtimer >= 180)
                {
                    NPC.ai[0] = -3;
                    deathtimer = 0;
                }
            }

            if (NPC.ai[0] == -3)
            {
                SoundEngine.PlaySound(new SoundStyle("pizzafacemod/NPCs/KillEnemy"), NPC.Center);

                int flingdir = 0;

                if (NPC.Center.X <= Main.LocalPlayer.Center.X)
                {
                    flingdir = 10;
                }

                if (NPC.Center.X >= Main.LocalPlayer.Center.X)
                {
                    flingdir = -10;
                }

                NPC.velocity = new Vector2(flingdir * NPC.spriteDirection, -13);
                NPC.noGravity = false;
                NPC.ai[0] = -4;
            }

            if (NPC.ai[0] == -4)
            {
                modPlayer.lap2 = false;
                modPlayer.lap2hasbeenentered = false;
                modPlayer.lap2spawnedboss = false;
                modPlayer.lap3 = false;
                modPlayer.lap3hasbeenentered = false;
                modPlayer.lap3spawnedboss = false;
                NPC.timeLeft = 30;
                if (++cloudtimer >= 25)
                {
                    Projectile.NewProjectile(NPC.GetSource_None(), NPC.Center + Main.rand.NextVector2Circular(50, 50), Vector2.Zero, ModContent.ProjectileType<CloudPuff>(), 0, 0, -1, 0, 0, 0);
                    cloudtimer = 0;
                }
            }


            //normal state, constant movement towards player
            if (NPC.ai[0] == 0)
            {
                movementlerpslow = 0;
                NPC.damage = 1;
                float lerp = MathHelper.Lerp(ClientConfig.Instance.PizzaSpeed / 2, ClientConfig.Instance.PizzaSpeed, ++movementlerpnormal * 0.03f);
                if (lerp >= ClientConfig.Instance.PizzaSpeed)
                {
                    lerp = ClientConfig.Instance.PizzaSpeed;
                }
                NPC.velocity = NPC.DirectionTo(Main.LocalPlayer.Center) * lerp;


                Loop ??= LoopedSoundManager.CreateNew(new SoundStyle("pizzafacemod/NPCs/PizzafaceMoving"), () =>
                {
                    return !NPC.active || NPC.ai[0] != 0 || Main.LocalPlayer.dead || NPC.ai[0] == 2;
                });

                Loop?.Update(NPC.Center, sound =>
                {
                    sound.Volume = 1f;
                });

                if (Loop2?.HasLoopSoundBeenStarted == true)
                {
                    Loop2?.Stop();
                }

                if (Loop?.HasBeenStopped == true && Loop?.HasLoopSoundBeenStarted == true)
                {
                    Loop?.Restart();
                    Loop2?.Stop();
                }
            }

            //broken state, much slower
            if (NPC.ai[0] == 1)
            {
                movementlerpnormal = 0;
                NPC.damage = 1;
                float lerp = MathHelper.Lerp(ClientConfig.Instance.PizzaSpeed, ClientConfig.Instance.PizzaSpeed / 2, ++movementlerpslow * 0.07f);
                if (lerp <= ClientConfig.Instance.PizzaSpeed / 2)
                {
                    lerp = ClientConfig.Instance.PizzaSpeed / 2;
                }
                NPC.velocity = NPC.DirectionTo(Main.LocalPlayer.Center) * lerp;


                Loop2 ??= LoopedSoundManager.CreateNew(new SoundStyle("pizzafacemod/NPCs/PizzafaceHaywire"), () =>
                {
                    return !NPC.active || NPC.ai[0] != 1 || Main.LocalPlayer.dead || NPC.ai[0] == 2;
                });

                Loop2?.Update(NPC.Center, sound =>
                {
                    sound.Volume = 0.2f;
                });
                

                if (Loop?.HasLoopSoundBeenStarted == true)
                {
                    Loop?.Stop();
                }

                if (Loop2?.HasBeenStopped == true && Loop2?.HasLoopSoundBeenStarted == true)
                {
                    Loop2?.Restart();
                    Loop?.Stop();
                }

                if (++smoketimer >= 9)
                {
                    Projectile.NewProjectile(NPC.GetSource_None(), NPC.Center + Main.rand.NextVector2Circular(80, 80), new Vector2(0, NPC.velocity.Y), ModContent.ProjectileType<Heatpuff>(), 0, 0, -1, 0, 0, 0);
                    smoketimer = 0;
                }

                //SoundEngine.PlaySound(new SoundStyle("pizzafacemod/NPCs/PizzafaceHaywire") with { Volume = 0.3f, IsLooped = true, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew}, NPC.Center);
            }

            //idle state, do nothing.
            if (NPC.ai[0] == 2)
            {
                movementlerpnormal = 0;
                movementlerpslow = 0;
                NPC.damage = 0;
                NPC.velocity *= 0;
                Loop?.Stop();
                Loop2?.Stop();
            }      

            //fucking die lmao
            if (NPC.ai[0] == 3)
            {
                NPC.ai[0] = -2;
                Loop?.Stop();
                Loop2?.Stop();
            }

            if (NPC.ai[0] == 4)
            {
                NPC.velocity *= 0;
                NPC.alpha += 2;

                Loop?.Stop();
                Loop2?.Stop();

                if (NPC.alpha > 255)
                {
                    NPC.active = false;
                }
            }

            if (modPlayer.bossbeenkilled)
            {
                NPC.ai[0] = 3;
                modPlayer.bossbeenkilled = false;
            }

            if (Main.LocalPlayer.dead)
            {
                NPC.ai[0] = 4;
                Loop?.Stop();
            }

            //this handles ALL of pizzaface's form changes
            SmartAI();
        }

        public override void FindFrame(int frameHeight)
        {   
            //intro sequence
            if (NPC.ai[0] == -1)
            {
                if (NPC.frame.Y > 29 * frameHeight)
                {
                    NPC.ai[0] = 0;
                }

                if (NPC.frame.Y == 8 * frameHeight && haslaughsoundbeenplayed == false)
                {
                    SoundEngine.PlaySound(new SoundStyle("pizzafacemod/NPCs/PizzafaceLaugh") with { Volume = 0.7f }, NPC.Center);
                    haslaughsoundbeenplayed = true;
                }

                if (++NPC.frameCounter >= 4)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= 29 * frameHeight)
                        NPC.ai[0] = 0;
                }
            }

            //normal state
            if (NPC.ai[0] == 0 || NPC.ai[0] == 4 || NPC.IsABestiaryIconDummy)
            {
                if (NPC.frame.Y > 44 * frameHeight)
                {
                    NPC.frame.Y = 29 * frameHeight;
                }

                if (++NPC.frameCounter >= 3)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= 44 * frameHeight)
                        NPC.frame.Y = 29 * frameHeight;
                }
            }

            //broken state
            if (NPC.ai[0] == 1)
            {   
                if (NPC.frame.Y < 45 * frameHeight)
                {
                    NPC.frame.Y = 45 * frameHeight;
                }

                //NPC.frame.Y = 16;
                if (++NPC.frameCounter >= 4)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= 52 * frameHeight)
                        NPC.frame.Y = 45 * frameHeight;
                }
            }

            //idle state
            if (NPC.ai[0] == 2)
            {
                if (NPC.frame.Y < 53 * frameHeight)
                {
                    NPC.frame.Y = 53 * frameHeight;
                }

                if (++NPC.frameCounter >= 3)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= 61 * frameHeight)
                        NPC.frame.Y = 53 * frameHeight;
                }
            }

            //death animation
            if (NPC.ai[0] <= -2)
            {
                if (NPC.frame.Y < 62 * frameHeight)
                {
                    NPC.frame.Y = 62 * frameHeight;
                }

                if (++NPC.frameCounter >= 3)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                        NPC.frame.Y = 62 * frameHeight;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Rectangle rectangle = NPC.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            float opacity = 0.5f;//MathHelper.Lerp(1, 0, ++afterimagetimer * 0.01f);
            float numberthatilerplmao = MathHelper.Lerp(1, 12, afterimagetimer * 0.01f);
            
            if (opacity <= 0)
            {
                afterimagetimer = 0;
                opacity = 1;
            }

            Color color26 = Color.DarkRed;

            if (NPC.ai[0] >= 0)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {   
                    if (NPC.ai[0] != 4)
                    {
                        Color color27 = color26;
                        color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                        Vector2 value4 = NPC.oldPos[i];
                        float num165 = NPC.oldRot[i];
                        Main.EntitySpriteDraw(texture2D13, value4 + NPC.Size / 2 - Main.screenPosition + new Vector2(0, -15), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27 * opacity, num165, origin2, NPC.scale, SpriteEffects.None, 0);
                    }
                    
                }
            }
                
                                   
            //Main.EntitySpriteDraw(texture2D13, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(Rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0);
            return true;
        }

        public void SmartAI()
        {   
            if (ModLoader.TryGetMod("FargowiltasSouls", out Mod FargowiltasSouls))
            {
                if (FargowiltasSouls.TryFind("MutantBoss", out ModNPC MutantBoss))
                {
                    int mutantIndex = NPC.FindFirstNPC(MutantBoss.Type); 
                    if (mutantIndex != -1)
                    {   
                        NPC mutant = Main.npc[mutantIndex];
                        if (mutant.active)
                        {



                            //delays for slow attack                                   

                            if (mutant.ai[0] == 27) // skele prime attack
                                phasedelay = 45; // 0.75 second

                            if (mutant.ai[0] == 30) //fishrons
                                phasedelay = 45; // 0.75 second

                            if (mutant.ai[0] == 37) // shadow hands
                                phasedelay = 90; //1.5 seconds

                            if (mutant.ai[0] == 36)
                                phasedelay = 60;

                            if (mutant.ai[0] == 40) //okuu spheres p2
                            {
                                phasedelay = 195; // 3.25 seconds
                                NPC.velocity *= 0.75f;
                            }

                            if (mutant.ai[0] == 43) //twinrangs
                                phasedelay = 180; // 3 seconds

                            if (mutant.ai[0] == 44) //empress
                                phasedelay = 120; // 2 seconds

                            if (mutant.ai[0] == -3) //okuu p3
                                phasedelay = 120; // 2 seconds

                            //delays for idle attac
                            if (mutant.ai[0] == 8) // void rays p2
                                idledelay = 60; // 1 second
                            if (mutant.ai[0] == 12) // void rays p2
                                idledelay = 120; // 2 seconds

                            if (mutant.ai[0] == 17) // bullet hell p2
                            {
                                idledelay = 180; // 3 seconds
                            }  

                            if (mutant.ai[0] == 20) // eoc
                                idledelay = 120; // 2 seconds

                            if (mutant.ai[0] == 34) // nuke
                                idledelay = 60; // 1 second

                            if (mutant.ai[0] == 43) //twinrangs
                                idledelay = 180; // 3 seconds

                            if (mutant.ai[0] == -2) // void rays p3
                                idledelay = 180; // 2 seconds

                            if (mutant.ai[0] == -4) // bullet hell p3
                                idledelay = 180; // 2 seconds


                            //slow state
                            if (mutant.ai[0] == 9 || mutant.ai[0] == 27 || mutant.ai[0] == 37 || mutant.ai[0] == 30 || mutant.ai[0] == 36 || mutant.ai[0] == 40 || mutant.ai[0] == 44 || mutant.ai[0] == -3)
                            {
                                NPC.ai[0] = 1;
                                wasjustslow = true;
                            }
                            else
                            {
                                if (wasjustslow == true)
                                {
                                    if (--phasedelay <= 0)
                                    {
                                        NPC.ai[0] = 0;
                                        wasjustslow = false;
                                    } 
                                }
                                                              
                            }

                            //idle state
                            if (mutant.ai[0] == -2 || mutant.ai[0] == 8|| mutant.ai[0] == 12 || mutant.ai[0] == 17 || mutant.ai[0] == 20 || mutant.ai[0] == 34 || mutant.ai[0] == 43 || mutant.ai[0] == -4)
                            {
                                NPC.ai[0] = 2;
                                wasjustidle = true;
                            }
                            else
                            {   
                                if (wasjustidle == true)
                                {
                                    if (--idledelay <= 0)
                                    {
                                        NPC.ai[0] = 0;
                                        wasjustidle = false;
                                    }
                                }
                                
                                
                                
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                if (FargowiltasSouls.TryFind("CursedCoffin", out ModNPC CursedCoffin))
                {
                    int CoffinIndex = NPC.FindFirstNPC(CursedCoffin.Type);
                    if (CoffinIndex != -1)
                    {
                        NPC Coffin = Main.npc[CoffinIndex];
                        if (Coffin.active && NPC.ai[0] != -1)
                        {
                            NPC.ai[0] = 1;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
    }

    public class PizzafaceModGlobalNPC : GlobalNPC
    {   
        public override void OnKill(NPC npc)
        {

            PizzafaceModPlayer modPlayer = Main.LocalPlayer.GetModPlayer<PizzafaceModPlayer>();
            if (npc.boss)
            {
                int pizzafaceindex = NPC.FindFirstNPC(ModContent.NPCType<Pizzaface>());
                if (pizzafaceindex != -1)
                {
                    NPC pizzaface = Main.npc[pizzafaceindex];
                    if (pizzaface.active)
                    {
                        modPlayer.bossbeenkilled = true;
                    }
                        
                }

                modPlayer.lastbosstype = npc.type;
                

                if (modPlayer.lap2 == false && ClientConfig.Instance.Lap2Skip == false)
                {
                    Projectile.NewProjectile(NPC.GetSource_None(), npc.Center, Vector2.Zero, ModContent.ProjectileType<LapPortal>(), 0, 0, -1, 0, 0, 0);
                    modPlayer.lap2 = true;
                }
                if ((modPlayer.lap3 == false && modPlayer.lap2hasbeenentered == true) || (ClientConfig.Instance.Lap2Skip == true && modPlayer.lap3 == false))
                {
                    Projectile.NewProjectile(NPC.GetSource_None(), npc.Center, Vector2.Zero, ModContent.ProjectileType<LapPortal>(), 0, 0, -1, 0, 0, 0);
                    modPlayer.lap3 = true;
                }


            }
            
          
            base.OnKill(npc);
        }
    }
}

