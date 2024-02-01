using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;

namespace ahn.particles;

internal abstract class Particles
{
    protected ParticleEffect particle;
    protected float lifespan;
    private float timer;
    private bool play;
    protected readonly ContentManager _contentManager;

    protected Particles()
    {}

    protected Particles(ContentManager c)
    {
        _contentManager = c;
    }

    protected void TriggerOnce()
    {
        timer = (float)TDS.g_time.TotalGameTime.TotalMilliseconds + 350;
        play = true;
        particle.Trigger();
    }

    public void Update()
    {
        if (!play) return;
        particle.Update((float)TDS.g_time.ElapsedGameTime.TotalSeconds);
        if (!(TDS.g_time.TotalGameTime.TotalMilliseconds > timer)) return;
        play = false;
        Cleanup();
        ParticleSystem.ParticlesList.Remove(this);
    }

    public void Draw(SpriteBatch sb)
    {
        if (!play) return;
        sb.Draw(particle);
    }

    protected abstract void Cleanup();
}