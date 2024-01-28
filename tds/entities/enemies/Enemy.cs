using System.Linq;
using ahn.scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ahn.entities.enemies;

internal abstract class Enemy
{
    protected static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(typeof(Enemy));

    public abstract Texture2D texture { get; protected set; }
    public abstract Vector2 position { get; protected set; }
    public abstract int scale { get; }
    protected abstract SoundEffect hit_sound { get; set; }
    protected abstract SoundEffect defl_hit_sound { get; set; }
    public int health;
    protected bool is_protected;

    private bool CheckCollision(Player p, Bullet b) =>
        p.position.X < b.position.X + b.scale && p.position.X + p.scale > b.position.X &&
        p.position.Y < b.position.Y + b.scale && p.position.Y + p.scale > b.position.Y;

    private bool CheckCollision(Player p, Enemy e) =>
        p.position.X < e.position.X + e.scale && p.position.X + p.scale > e.position.X &&
        p.position.Y < e.position.Y + e.scale && p.position.Y + p.scale > e.position.Y;

    private void CheckCollision(Enemy e, Bullet b)
    {
        if (!(b.position.X < e.position.X + e.scale) || !(b.position.X + b.scale > e.position.X) ||
            !(b.position.Y < e.position.Y + e.scale) || !(b.position.Y + b.scale > e.position.Y)) return;
        b.is_alive = false;
        e.health -= Player.damage;
        if (is_protected) defl_hit_sound.Play();
        else hit_sound.Play();
    }

    public virtual void Update(Player p, Enemy e)
    {
        foreach (var b in Player._bullets)
        {
            CheckCollision(e, b);
        }
        if (GameScene._state == State.Dead) return;
        if (CheckCollision(p, e))
        {
            if (e is Crate)
            {
                e.health--;
                return;
            }
            GameScene.DamagePlayer();
            e.health--;
        }
        foreach (var b in EnemyHandler.EnemyBullets.ToList().Where(b => CheckCollision(p, b) && !GameScene.damaged))
        {
            GameScene.damaged = true;
            GameScene.DamagePlayer();
            b.is_alive = false;
        }
    }

    public void Cleanup()
    {
        texture.Dispose();
        hit_sound?.Dispose();
        defl_hit_sound?.Dispose();
    }
}