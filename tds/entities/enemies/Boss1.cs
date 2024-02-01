using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#pragma warning disable CS4014

namespace ahn.entities.enemies;

internal sealed class Boss1 : Entity
{
    public override Texture2D texture { get; protected set; }
    public override Vector2 position { get; protected set; }
    private SoundEffect hit_sound { get; set; }
    private SoundEffect defl_hit_sound { get; set; }
    private SoundEffect death_sound;
    private SoundEffect shoot_sound;
    private const int speed = 350;
    private const int firerate = 1500;
    private float next_time_to_fire;
    private readonly Player _player;
    private Texture2D bullet_texture;
    private static bool is_protected;

    public Boss1(Player p)
    {
        _player = p;
        scale = 256;
    }

    public void LoadContent(ContentManager c)
    {
        hit_sound = c.Load<SoundEffect>("enemyHit");
        defl_hit_sound = c.Load<SoundEffect>("enemyHitProt");
        death_sound = c.Load<SoundEffect>("bossDeath");
        shoot_sound = c.Load<SoundEffect>("shoot");
        texture = c.Load<Texture2D>("boss1");
        bullet_texture = c.Load<Texture2D>("boss1Bullet");
    }

    public void Spawn()
    {
        health = 16;
        position = new Vector2(TDS._winWidth + 25, TDS._winHeight / 2 - scale / 2);
        EntityHandler.Entities.Add(this);
    }

    protected override void Damage()
    {
        if (is_protected)
        {
            defl_hit_sound.Play();
            return;
        }
        hit_sound.Play();
        health--;
    }

    protected override void Kill()
    {
        death_sound.Play();
        Player.score += 100;
        EntityHandler.Entities.Remove(this);
    }

    private void Attack()
    {
        if (TDS.g_time.TotalGameTime.TotalMilliseconds < next_time_to_fire) return;
        next_time_to_fire = (float)TDS.g_time.TotalGameTime.TotalMilliseconds + firerate;
        var rnd = new Random();
        var attack_type = rnd.Next(0, 3);
        EnemyBullet bullet;
        switch (attack_type)
        {
            case 0: // SIX SHOT
                for (var i = -20; i < 40; i += 10)
                {
                    shoot_sound.Play();
                    bullet = new EnemyBullet(bullet_texture, "boss1");
                    bullet.Create(new Vector2(position.X, position.Y + scale / 2f), _player, i);
                    EntityHandler.EnemyBullets.Add(bullet);
                }
                break;
            case 1: // LOTS OF SHITS
                shoot_sound.Play();
                async Task Shoot()
                {
                    for (var i = 0; i < 16; i++)
                    {
                        shoot_sound.Play();
                        bullet = new EnemyBullet(bullet_texture, "boss1");
                        bullet.Create(new Vector2(position.X, position.Y + scale / 2f), _player);
                        EntityHandler.EnemyBullets.Add(bullet);
                        await Task.Delay(50);
                    }
                }
                Shoot();
                break;
            case 2: // a circle
                for (var i = 0; i < 360; i += 20)
                {
                    if (i % 8 == 0) shoot_sound.Play();
                    bullet = new EnemyBullet(bullet_texture, "boss1");
                    bullet.Create(new Vector2(position.X, position.Y + scale / 2f), _player, i);
                    EntityHandler.EnemyBullets.Add(bullet);
                }
                break;
        }
    }

    protected override void AliveUpdate(Player p, Entity e)
    {
        base.AliveUpdate(p, e);
        if (position.X > TDS._winWidth / 1.5f)
        {
            is_protected = true;
            position = new Vector2(position.X - speed * TDS.frame_delta, position.Y);
            return;
        }
        is_protected = false;
        Attack();
    }

    public void Cleanup()
    {
        death_sound.Dispose();
        shoot_sound.Dispose();
        bullet_texture.Dispose();
    }
}