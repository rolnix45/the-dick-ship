using System;
using ahn.io;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ahn.scenes;

public class TitleScene : Scene
{
    public int scene_id { get; }
    private const string text = "Press ENTER to START";
    private Texture2D background;
    private SpriteFont font;
    private Vector2 text_pos;

    public TitleScene() =>
        scene_id = 0;

    public void Init(GraphicsDeviceManager gdm) =>
        text_pos = new Vector2(TDS._winWidth / 2.667f, TDS._winHeight / 1.2f);

    public void LoadContent(ContentManager c)
    {
        background = c.Load<Texture2D>("menuBackground");
        font = c.Load<SpriteFont>("bfont");
    }
    
    public void Update()
    {
        if (Input.KeyPressed(Keys.Escape)) TDS.Close();
        text_pos.X += MathF.PI / 180f * MathF.Sin((float)TDS.g_time.TotalGameTime.TotalMilliseconds / 200f) * 800f;
    }

    public void Draw(SpriteBatch sb)
    {
        sb.Draw(background, Vector2.Zero, Color.White);
        sb.DrawString(
            font,
            text,
            text_pos, 
            Color.Cyan,
            0,
            font.MeasureString(text) / 2,
            1.0f, 
            SpriteEffects.None,
            0.5f
        );
    }

    public void CleanUp() =>
        background.Dispose();
}