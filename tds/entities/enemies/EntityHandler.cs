using System;
using System.Collections.Generic;
using System.Linq;
using ahn.particles;
using ahn.scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable UseStringInterpolation

namespace ahn.entities.enemies;

internal sealed class EntityHandler
{
    private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(typeof(EntityHandler));

    public static readonly List<Entity> Entities = new();
    public static readonly List<EnemyBullet> EnemyBullets = new();

    public void KillAllEnemiesAndBullets()
    {
        _log.Debug(string.Format("clr {0}e {1}pb {2}eb {3}", Entities.Count, Player._bullets.Count, EnemyBullets.Count, ParticleSystem.ParticlesList.Count));
        Entities.Clear();
        Player._bullets.Clear();
        EnemyBullets.Clear();
        ParticleSystem.ParticlesList.Clear();
    }

    private void DrawBullets(ref SpriteBatch sb, EnemyBullet b)
    {
        sb.Draw(
            b.texture,
            new Rectangle((int)b.position.X, (int)b.position.Y, b.scale, b.scale),
            new Rectangle(0, 0, b.texture.Width, b.texture.Height),
            Color.White,
            b.angle,
            Vector2.Zero,
            SpriteEffects.None,
            0.5f
        );
        if (!GameScene.draw_hitboxes) return;
        Util.DrawRectangle(
            sb,
            new Rectangle((int)b.position.X, (int)b.position.Y, b.scale, b.scale),
            Color.Crimson
        );
    }

    private void DrawEntities(ref SpriteBatch sb, Entity e)
    {
        if (e.scale != 0)
        {
            sb.Draw(
                e.texture,
                new Rectangle((int)e.position.X, (int)e.position.Y, e.scale, e.scale),
                new Rectangle(0, 0, e.texture.Width, e.texture.Height),
                Color.White,
                0f,
                Vector2.Zero, 
                SpriteEffects.None,
                0.5f
            );
            if (!GameScene.draw_hitboxes) return;
            Util.DrawRectangle(
                sb,
                new Rectangle((int)e.position.X, (int)e.position.Y, e.scale, e.scale),
                Color.Red
            );
        }
        else
        {
            if (e is Shield && !Shield.shield_on) return;
            sb.Draw(
                e.texture,
                new Rectangle((int)e.position.X, (int)e.position.Y, e.texture.Width, e.texture.Height),
                new Rectangle(0, 0, e.texture.Width, e.texture.Height),
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.5f
            );
            if (!GameScene.draw_hitboxes) return;
            Util.DrawRectangle(
                sb,
                new Rectangle((int)e.position.X, (int)e.position.Y, e.texture.Width, e.texture.Height),
                Color.Red
            );
        }
    }
    
    public void Draw(SpriteBatch sb)
    {
        foreach (var b in EnemyBullets)
        {
            DrawBullets(ref sb, b);
        }
        foreach (var e in Entities.Where(e => !e.dead))
        {
            DrawEntities(ref sb, e);
        }
    }

    public void Cleanup()
    {
        //foreach (var e in Entities)
        //{
        //    e.Cleanup();
        //}
        Entities.Clear();
        //foreach (var b in EnemyBullets)
        //{
        //   b.Cleanup();
        //}
        EnemyBullets.Clear();
    }
}