using System;
using System.Collections.Generic;
using ahn.entities.enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Content;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;

namespace ahn.particles;

internal sealed class DeathParticles : Particles
{
    private Texture2D part_texture;

    private void LoadContent(ContentManager c)
    {
        part_texture = new Texture2D(c.GetGraphicsDevice(), 1, 1);
        var data = new Color[1];
        for (var i = 0; i < data.Length; i++)
        {
            data[i] = Color.White;
        }
        part_texture.SetData(data);
        var tex_region = new TextureRegion2D(part_texture, 0, 0, 10, 10);
        particle = new ParticleEffect(autoTrigger: false)
        {
            Emitters = new List<ParticleEmitter>
            {
                new(tex_region, 30, TimeSpan.FromSeconds(0.350), Profile.Circle(10f, Profile.CircleRadiation.Out))
                {
                    Parameters = new ParticleReleaseParameters
                    {
                        Speed = 350f,
                        Quantity = 10,
                        Rotation = new Range<float>(-1f, 1f),
                        Scale = new Range<float>(.2f, .5f),
                        Color = new HslColor(0.06f, 1.0f, 0.35f)
                    }
                }
            }
        };
    }

    public DeathParticles Create(float ls = 600f)
    {
       var p = new DeathParticles();
       p.LoadContent(_contentManager);
       ParticleSystem.ParticlesList.Add(p);
       lifespan = ls;
       return p;
    }
    
    public void Trigger(Entity e)
    {
        TriggerOnce();
        particle.Position = e.position;
    }

    protected override void Cleanup()
    {
        particle.Dispose();
        part_texture.Dispose();
    }

    private DeathParticles() 
    { }
    
    public DeathParticles(ContentManager c) : base(c)
    { }
}