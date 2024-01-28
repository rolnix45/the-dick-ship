using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ahn;

internal sealed class Util
{
    private static Texture2D point_texture;

    public static void DrawRectangle(SpriteBatch sb, Rectangle r, Color c, int lw = 1)
    {
        if (point_texture == null)
        {
            point_texture = new Texture2D(sb.GraphicsDevice, 1, 1);
            point_texture.SetData(new []{Color.White});
        }
        sb.Draw(point_texture, new Rectangle(r.X, r.Y, lw, r.Height + lw), c);
        sb.Draw(point_texture, new Rectangle(r.X, r.Y, r.Width + lw, lw), c);
        sb.Draw(point_texture, new Rectangle(r.X + r.Width, r.Y, lw, r.Height + lw), c);
        sb.Draw(point_texture, new Rectangle(r.X, r.Y + r.Height, r.Width + lw, lw), c);
    }
}