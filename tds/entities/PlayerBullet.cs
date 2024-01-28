using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ahn.entities;

internal sealed class PlayerBullet : Bullet
{
    public override Texture2D texture { get; protected init; }
    public override Vector2 position { get; set; }

    public PlayerBullet(Texture2D t)
    {
        texture = t;
        position = new Vector2();
        scale = 32;
        is_alive = true;
    }

    public void Create(Vector2 pos)
    {
        position = pos;
        delta_x = 1750;
        delta_y = 0;
    }
}