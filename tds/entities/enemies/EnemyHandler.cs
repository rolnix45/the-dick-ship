using System.Collections.Generic;
using ahn.scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
// ReSharper disable UseStringInterpolation

namespace ahn.entities.enemies;

sealed class EnemyHandler
{
    private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(typeof(EnemyHandler));

    public static readonly List<Enemy> Enemies = new();
    public static readonly List<EnemyBullet> EnemyBullets = new();

    public void KillAllEnemiesAndBullets()
    {
        _log.Debug(string.Format("clr {0}e {1}pb {2}eb", Enemies.Count, Player._bullets.Count, EnemyBullets.Count));
        Enemies.Clear();
        Player._bullets.Clear();
        EnemyBullets.Clear();
    }

    public void Draw(SpriteBatch sb)
    {
        foreach (var b in EnemyBullets)
        {
            sb.Draw(
                b.texture,
                b.position,
                Color.White
            );
            if (!GameScene.draw_hitboxes) continue;
            Util.DrawRectangle(sb, new Rectangle((int)b.position.X, (int)b.position.Y, b.scale, b.scale), Color.Crimson);
        }
        foreach (var e in Enemies)
        {
            sb.Draw(
                e.texture,
                new Rectangle((int)e.position.X, (int)e.position.Y, e.scale, e.scale),
                new Rectangle(0, 0, e.texture.Width, e.texture.Height),
                Color.White
            );
            if (!GameScene.draw_hitboxes) continue;
            Util.DrawRectangle(sb, new Rectangle((int)e.position.X, (int)e.position.Y, e.scale, e.scale), Color.Red);
        }
    }

    public void Cleanup()
    {
        foreach (var e in Enemies)
        {
            e.Cleanup();
        }

        foreach (var b in EnemyBullets)
        {
            b.Cleanup();
        }
    }
}