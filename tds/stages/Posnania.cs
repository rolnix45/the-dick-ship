using ahn.entities;
using ahn.io;
using ahn.scenes;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ahn.stages;

public class Posnania : Stage
{
    public Stages stage_id { get; private set; }
    public Texture2D background { get; private set; }
    public void Init(Player p)
    {
        stage_id = Stages.posnania;
    }

    public void LoadContent(ContentManager c)
    {
        background = c.Load<Texture2D>("posnaniaback");
    }

    public void UnloadContent()
    {
    }

    public void Spawn()
    {
    }

    public void Restart()
    {
    }

    public void Update()
    {
        if (Input.KeyPressed(Keys.F5))
        {
            GameScene.next_stage = Stages.dick_space;
        }
        if (Input.KeyPressed(Keys.F6))
        {
            GameScene.next_stage = Stages.kwatera_jaspera;
        }
    }

    public void Draw(SpriteBatch sb)
    {
    }
}