using System.Linq;
using ahn.particles;
using ahn.scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ahn.entities.enemies;

internal abstract class Entity
{
    protected static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(typeof(Entity));

    public abstract Texture2D texture { get; protected set; }
    public abstract Vector2 position { get; protected set; }
    public DeathParticles _dp { get; protected set; }
    public int scale { get; protected init; }
    public bool dead { get; private set; }
    private int time_to_remove;
    protected int health;
    
    private bool CheckCollision(Player p, Bullet b) =>
        p.position.X < b.position.X + b.scale && p.position.X + p.scale > b.position.X &&
        p.position.Y < b.position.Y + b.scale && p.position.Y + p.scale > b.position.Y;

    private bool CheckCollision(Player p, Entity e) =>
        p.position.X < e.position.X + e.scale && p.position.X + p.scale > e.position.X &&
        p.position.Y < e.position.Y + e.scale && p.position.Y + p.scale > e.position.Y;

    private bool CheckCollision(Entity e, Bullet b) =>
        scale != 0
            ? b.position.X < e.position.X + e.scale && b.position.X + b.scale > e.position.X &&
              b.position.Y < e.position.Y + e.scale && b.position.Y + b.scale > e.position.Y
            : b.position.X < e.position.X + e.texture.Width && b.position.X + b.scale > e.position.X &&
              b.position.Y < e.position.Y + e.texture.Height && b.position.Y + b.scale > e.position.Y;

    protected abstract void Damage();

    protected virtual void Kill()
    {
        dead = true;
        time_to_remove = (int)TDS.g_time.TotalGameTime.TotalSeconds + 1;
    }

    protected virtual void AliveUpdate(Player p, Entity e)
    {
        foreach (var b in EntityHandler.EnemyBullets.ToList()
            .Where(b => CheckCollision(e, b) && e is Shield && Shield.shield_on))
        {
            e.Damage();
            if (e.health <= 0)
            {
                e.Kill();
            }

            b.is_alive = false;
        }

        if (e is Shield) return;
        foreach (var b in Player._bullets.Where(b => CheckCollision(e, b)))
        {
            b.is_alive = false;
            e.Damage();
            if (e.health <= 0)
            {
                e.Kill();
            }
        }

        if (GameScene._state == State.Dead) return;
        if (CheckCollision(p, e))
        {
            if (e is Crate)
            {
                e.Damage();
                return;
            }

            GameScene.DamagePlayer();
            e.health--;
        }

        foreach (var b in EntityHandler.EnemyBullets.ToList()
            .Where(b => CheckCollision(p, b) && !GameScene.damaged))
        {
            GameScene.damaged = true;
            GameScene.DamagePlayer();
            b.is_alive = false;
        }
    }

    private void DeadUpdate()
    {
        if (!(TDS.g_time.TotalGameTime.TotalSeconds >= time_to_remove)) return;
        EntityHandler.Entities.Remove(this);
        ParticleSystem.ParticlesList.Remove(_dp);
    }
    
    public void Update(Player p, Entity e)
    {
        if (dead)
        {
            DeadUpdate();
        }
        else
        {
            AliveUpdate(p, e);
        }
    }
}