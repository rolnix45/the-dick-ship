using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ahn.entities.enemies;

internal sealed class EnemyBullet : Bullet
{
    public override Texture2D texture { get; protected init; }
    public override Vector2 position { get; set; }
    public readonly string tag;
    private const float bullet_velocity = 800;
    public float angle { get; private set; }

    public EnemyBullet(Texture2D t, string tag)
    {
        texture = t;
        position = new Vector2();
        scale = 16;
        is_alive = true;
        this.tag = tag;
    }

    public void Create(Vector2 pos, int speed)
    {
        position = pos;
        delta_x = speed;
        delta_y = 0;
    }

    public void Create(Vector2 pos, Player p)
    {
        var p_pos = p.position;
        position = new Vector2(pos.X, pos.Y);
        angle = CalculateAngle(
            p_pos.X + p.scale / 2f,
            p_pos.Y + p.scale / 2f,
            pos.X,
            pos.Y
        );
        delta_x = 180f / MathF.PI * MathF.Cos(angle) * bullet_velocity * TDS.frame_delta;
        delta_y = 180f / MathF.PI * MathF.Sin(angle) * bullet_velocity * TDS.frame_delta;
        angle *= 180f / MathF.PI;
    }

    public void Create(Vector2 pos, Player p, float ang)
    {
        var player_pos = p.position;
        position = new Vector2(pos.X, pos.Y);
        angle = CalculateAngle(
            player_pos.X + p.scale / 2f,
            player_pos.Y + p.scale / 2f,
            pos.X,
            pos.Y
        );
        var rad_ang = ang * MathF.PI / 180f;
        delta_x = 180f / MathF.PI * MathF.Cos(angle + rad_ang) * bullet_velocity * TDS.frame_delta;
        delta_y = 180f / MathF.PI * MathF.Sin(angle + rad_ang) * bullet_velocity * TDS.frame_delta;
        angle = rad_ang;
    }
}