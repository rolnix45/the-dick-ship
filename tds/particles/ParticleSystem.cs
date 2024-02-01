using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace ahn.particles;

internal static class ParticleSystem
{
    public static readonly List<Particles> ParticlesList = new();
    
    public static void UpdateParticles()
    {
        foreach (var p in ParticlesList.ToList())
        {
            p.Update();
        }
    }
    
    public static void DrawParticles(SpriteBatch sb)
    {
        foreach (var p in ParticlesList.ToList())
        {
            p.Draw(sb);
        }
    }
    
}