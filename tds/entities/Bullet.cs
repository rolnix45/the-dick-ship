using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ahn.entities;

public abstract class Bullet
{
    public abstract Texture2D texture { get; protected init; }
    public abstract Vector2 position { get; set; }
    public int scale { get; protected init; }
    public bool is_alive { get; set; }
    public float delta_x { get; protected set; }
    public float delta_y { get; protected set; }

    protected float CalculateAngle(float x1, float y1, float x2, float y2)
    {
        var y = y1 - y2;
        var x = x1 - x2;
        return MathF.Atan2(y, x);
    }

    public void Cleanup()
    {
        texture.Dispose();
    }
}