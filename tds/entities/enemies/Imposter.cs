﻿using System;
using ahn.particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ahn.entities.enemies;

internal sealed class Imposter : Entity
{
    private const int speed = 300;
    public override Texture2D texture { get; protected set; }
    public override Vector2 position { get; protected set; }
    private Texture2D bullet_texture;
    private SoundEffect hit_sound { get; set; }

    public void LoadContent(ContentManager c)
    {
        _dp = new DeathParticles(c);
        texture = c.Load<Texture2D>("enemy1");
        bullet_texture = c.Load<Texture2D>("enemy1Bullet");
        hit_sound = c.Load<SoundEffect>("enemyHit");
    }

    public void Spawn()
    {
        var rnd = new Random();
        var i = new Imposter
        {
            texture = texture,
            bullet_texture = bullet_texture,
            hit_sound = hit_sound,
            health = 1,
            scale = 48,
            _dp = _dp.Create()
        };
        i.position = new Vector2(TDS._winWidth, rnd.Next(i.scale, TDS._winHeight - i.scale));
        EntityHandler.Entities.Add(i);
    }

    private const int firerate = 2750;
    private float next_time_to_fire;
    private void Shoot()
    {
        if (!(TDS.g_time.TotalGameTime.TotalMilliseconds >= next_time_to_fire)) return;
        next_time_to_fire = (float)TDS.g_time.TotalGameTime.TotalMilliseconds + firerate;
        var enemy_bullet = new EnemyBullet(bullet_texture, "imposter");
        enemy_bullet.Create(new Vector2(position.X - 10 + scale / 2f, position.Y + scale / 2f), -450);
        EntityHandler.EnemyBullets.Add(enemy_bullet);
    }

    protected override void Damage()
    {
        health--;
    }

    protected override void Kill()
    {
        base.Kill();
        _dp.Trigger(this);
        hit_sound.Play();
        Player.score++;
    }

    protected override void AliveUpdate(Player p, Entity e)
    {
        base.AliveUpdate(p, e);
        if (position.X < 0)
        {
            EntityHandler.Entities.Remove(this);
            ParticleSystem.ParticlesList.Remove(e._dp);
            return;
        }
        Shoot();
        position = new Vector2(position.X - speed * TDS.frame_delta, position.Y);
    }

    public void Cleanup()
    {
        texture.Dispose();
        hit_sound.Dispose();
        bullet_texture.Dispose();
    }
}