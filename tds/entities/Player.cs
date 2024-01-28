using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ahn.io;
using ahn.scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ahn.entities;

internal sealed class Player
{
    private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(typeof(Player));
    public static readonly List<Bullet> _bullets = new();
    public static int score;
    private Texture2D main_texture;
    private Texture2D empty_texture;
    private Texture2D texture;
    private Texture2D bullet_texture;
    public SoundEffect death_sound;
    private SoundEffect hit_sound;
    public Vector2 position;
    public int scale { get; private set; }
    private float next_time_to_fire;
    public static int fire_rate = 450;
    private int speed_mul;
    public static bool invincible;
    public static int lifes;
    public static int damage;

    public static bool firerate_max;
    public static bool damage_max;

    public void Init()
    {
        scale = 64;
        position = new Vector2(100, TDS._winHeight / 2 - scale / 2);
        speed_mul = 600;
        score = 0;
        lifes = 1;
        old_lifes = lifes;
        damage = 1;
        firerate_max = false;
        damage_max = false;
        Blink(8);
        _log.Debug("player spawned");
    }

    public void LoadContent(ContentManager c)
    {
        texture = c.Load<Texture2D>("kutas");
        empty_texture = c.Load<Texture2D>("e");
        main_texture = texture;
        bullet_texture = c.Load<Texture2D>("bullet");
        death_sound = c.Load<SoundEffect>("playerDeath");
        hit_sound = c.Load<SoundEffect>("playerHit");
    }

    private void HandleBullets()
    {
        var plr_bullet = new PlayerBullet(bullet_texture);
        if (Input.LMBHeld() && TDS.g_time.TotalGameTime.TotalMilliseconds >= next_time_to_fire)
        {
            next_time_to_fire = (float)TDS.g_time.TotalGameTime.TotalMilliseconds + fire_rate;
            plr_bullet.Create(new Vector2(position.X + scale / 2f, position.Y + scale / 4f));
            _bullets.Add(plr_bullet);
        }

        foreach (var b in _bullets.ToList())
        {
            if (b.position.X < TDS._winWidth && b.is_alive)
            {
                b.position = new Vector2(b.position.X + b.delta_x * TDS.frame_delta, b.position.Y + b.delta_y * TDS.frame_delta);
            }
            else
            {
                _bullets.Remove(b);
            }
        }
    }

    private void MoveMouse()
    {
        var center_plr_pos = new Vector2(
            position.X + scale / 2f,
            position.Y + scale / 2f
        );
        var m_pos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        Vector2.Lerp(ref m_pos, ref center_plr_pos, 0.90f, out center_plr_pos);
        center_plr_pos.X = center_plr_pos.X switch
        {
            < 0 => 0,
            > TDS._winWidth => TDS._winWidth,
            _ => center_plr_pos.X
        };
        center_plr_pos.Y = center_plr_pos.Y switch
        {
            < 0 => 0,
            > TDS._winHeight => TDS._winHeight,
            _ => center_plr_pos.Y
        };
        position = new Vector2(center_plr_pos.X - scale / 2f, center_plr_pos.Y - scale / 2f);
    }

    private int old_lifes;
    public void Update() 
    {
        MoveMouse();
        HandleBullets();
        if (lifes >= old_lifes)
        {
            old_lifes = lifes;
            return;
        }
        old_lifes = lifes;
        hit_sound.Play();
        Blink(2);
    }

    private void Blink(int t)
    {
        async Task Blinking()
        {
            await Task.Run(delegate
            {
                invincible = true;
                for (var i = 0; i <= t; i++)
                {
                    texture = empty_texture;
                    Task.Delay(75).Wait();
                    texture = main_texture;
                    Task.Delay(75).Wait();
                }
                invincible = false;
            });
        }
        if (!Blinking().IsCompleted) return;
        texture = main_texture;
        invincible = false;
        Blinking().Dispose();
    }

    public void Draw(SpriteBatch sb)
    {
        foreach (var b in _bullets)
        {
            sb.Draw(
                b.texture,
                b.position,
                Color.White
            );
            if (!GameScene.draw_hitboxes) continue;
            Util.DrawRectangle(sb, new Rectangle((int)b.position.X, (int)b.position.Y, b.scale, b.scale), Color.Purple);
        }
        if (GameScene._state == State.Dead) return;
        sb.Draw(
            texture,
            new Rectangle((int)position.X, (int)position.Y, scale, scale),
            new Rectangle(0, 0, texture.Width, texture.Height),
            Color.White
        );
        if (!GameScene.draw_hitboxes) return;
        Util.DrawRectangle(sb, new Rectangle((int)position.X, (int)position.Y, scale, scale), Color.Blue);
    }

    public void Cleanup()
    {
        foreach (var b in _bullets)
        {
            b.Cleanup();
        }
        texture.Dispose();
        main_texture.Dispose();
        empty_texture.Dispose();
        death_sound.Dispose();
    }
}