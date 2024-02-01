using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ahn.entities.enemies;

internal sealed class TwujStaryNajebany : Entity
{
    public override Texture2D texture { get; protected set; }
    private Texture2D bullet_texture;
    public override Vector2 position { get; protected set; }
    private SoundEffect hit_sound;
    private const int speed = 450;
    private Player _player;
    
    private TwujStaryNajebany()
    {}
    
    public TwujStaryNajebany(Player p)
    {
        _player = p;
    }
    
    public void LoadContent(ContentManager c)
    {
        texture = c.Load<Texture2D>("tsn");
        hit_sound = c.Load<SoundEffect>("enemyHit");
        bullet_texture = c.Load<Texture2D>("enemy1Bullet");
    }

    public void Spawn()
    {
        var rnd = new Random();
        var tsn = new TwujStaryNajebany
        {
            texture = texture,
            bullet_texture = bullet_texture,
            hit_sound = hit_sound,
            _player = _player,
            health = 2
        };
        tsn.position = new Vector2(TDS._winWidth, rnd.Next(tsn.scale, TDS._winHeight - tsn.scale / 2));
        EntityHandler.Entities.Add(tsn);
    }

    private const int firerate = 1750;
    private float next_time_to_fire;
    private void Attack()
    {
        if (!(TDS.g_time.TotalGameTime.TotalMilliseconds >= next_time_to_fire)) return;
        next_time_to_fire = (float)TDS.g_time.TotalGameTime.TotalMilliseconds + firerate;
        var enemy_bullet = new EnemyBullet(bullet_texture, "twujstarynajebany");
        enemy_bullet.Create(new Vector2(position.X + texture.Width / 2f, position.Y + texture.Height / 2f), _player);
        EntityHandler.EnemyBullets.Add(enemy_bullet);
    }
    
    protected override void Damage()
    {
        health--;
        hit_sound.Play();
    }

    protected override void Kill()
    {
        Player.score++;
        EntityHandler.Entities.Remove(this);
    }

    protected override void AliveUpdate(Player p, Entity e)
    {
        base.AliveUpdate(p, e);
        if (position.X < 0)
        {
            EntityHandler.Entities.Remove(this);
            return;
        }
        Attack();
        position = new Vector2(position.X - speed * TDS.frame_delta, position.Y);
    }

    public void Cleanup()
    {
        texture.Dispose();
        bullet_texture.Dispose();
        hit_sound.Dispose();
    }
}