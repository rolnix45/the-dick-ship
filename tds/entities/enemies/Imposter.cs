using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ahn.entities.enemies;

internal sealed class Imposter : Enemy
{
    private const int speed = 300;
    public override Texture2D texture { get; protected set; }
    private readonly Texture2D bullet_texture;
    public override Vector2 position { get; protected set; }
    public override int scale { get; }
    protected override SoundEffect hit_sound { get; set; }
    protected override SoundEffect defl_hit_sound { get; set; }

    private Imposter(Texture2D t, Texture2D bt, SoundEffect hs, SoundEffect dhs)
    {
        texture = t;
        bullet_texture = bt;
        scale = 48;
        position = new Vector2();
        health = 1;
        hit_sound = hs;
        defl_hit_sound = dhs;
    }

    public static void Spawn(Texture2D t, Texture2D bt, SoundEffect hs, SoundEffect dhs)
    {
        var rnd = new Random();
        var i = new Imposter(t, bt, hs, dhs);
        i.position = new Vector2(TDS._winWidth, rnd.Next(i.scale, TDS._winHeight - i.scale));
        EnemyHandler.Enemies.Add(i);
    }

    private const int firerate = 1250;
    private float next_time_to_fire;
    private void Shoot()
    {
        if (!(TDS.g_time.ElapsedGameTime.Milliseconds >= next_time_to_fire)) return;
        next_time_to_fire = TDS.g_time.ElapsedGameTime.Milliseconds + firerate;
        var enemy_bullet = new EnemyBullet(bullet_texture, "imposter");
        enemy_bullet.Create(new Vector2(position.X - 10 + scale / 2f, position.Y + scale / 2f), -450);
        EnemyHandler.EnemyBullets.Add(enemy_bullet);
    }

    public override void Update(Player p, Enemy e)
    {
        base.Update(p, e);
        if (position.X < 0 || health <= 0)
        {
            EnemyHandler.Enemies.Remove(this);
            if (health > 0) return;
            Player.score++;
            return;
        }
        Shoot();
        position = new Vector2(position.X - speed * TDS.frame_delta, position.Y);
    }

}