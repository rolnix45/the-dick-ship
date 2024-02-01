using ahn.entities;
using ahn.entities.enemies;
using ahn.io;
using ahn.scenes;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ahn.stages;

public class KwateraJaspera : Stage
{
    public Stages stage_id { get; private set; }
    public Texture2D background { get; private set; }
    private TwujStaryNajebany tsn { get; set; }

    public void Init(Player p)
    {
        stage_id = Stages.kwatera_jaspera;
        tsn = new TwujStaryNajebany(p);
    }

    public void LoadContent(ContentManager c)
    {
        background = c.Load<Texture2D>("kwjsback");
        tsn.LoadContent(c);
    }

    public void UnloadContent()
    {
        background.Dispose();
        tsn.Cleanup();
    }

    private float time_to_spawn_tsn;
    private const int spawn_rate = 500;
    public void Spawn()
    {
        // TWUJ STARY NAJEBANY
        if (TDS.g_time.TotalGameTime.TotalMilliseconds >= time_to_spawn_tsn)
        {
            time_to_spawn_tsn = (float)TDS.g_time.TotalGameTime.TotalMilliseconds + spawn_rate;
            tsn.Spawn();
        }
    }

    public void Restart()
    { }

    public void Update()
    {
        Spawn();
        if (Input.KeyPressed(Keys.F5))
        {
            GameScene.next_stage = Stages.dick_space;
        }

        if (Input.KeyPressed(Keys.F7))
        {
            GameScene.next_stage = Stages.posnania;
        }
    }

    public void Draw(SpriteBatch sb)
    { }
}