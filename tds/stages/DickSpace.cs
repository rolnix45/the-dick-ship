using ahn.entities;
using ahn.entities.enemies;
using ahn.io;
using ahn.scenes;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ahn.stages;

public class DickSpace : Stage
{
    public Texture2D background { get; private set; }
    private Boss1 boss1 { get; set; }
    private Crate crate { get; set; }
    private TwujStaryNajebany tsn { get; set; }
    private Imposter imp { get; set; }
    private bool boss1_spawned;

    public Stages stage_id { get; private set; }

    public void Init(Player p)
    {
        stage_id = Stages.dick_space;
        crate = new Crate();
        boss1 = new Boss1(p);
        tsn = new TwujStaryNajebany(p);
        imp = new Imposter();
    }

    public void LoadContent(ContentManager c)
    {
        background = c.Load<Texture2D>("background");
        crate.LoadContent(c);
        boss1.LoadContent(c);
        tsn.LoadContent(c);
        imp.LoadContent(c);
    }

    public void UnloadContent()
    {
        background.Dispose();
        crate.Cleanup();
        boss1.Cleanup();
        tsn.Cleanup();
        imp.Cleanup();
    }

    private float time_to_spawn_imp;
    private float time_to_spawn_crate;
    private float time_to_spawn_tsn;
    private const int spawn_rate = 500;
    public void Spawn()
    {
        // IMPOSTER
        if (TDS.g_time.TotalGameTime.TotalMilliseconds >= time_to_spawn_imp)
        {
            time_to_spawn_imp = (float)TDS.g_time.TotalGameTime.TotalMilliseconds + spawn_rate;
            imp.Spawn();
        }
        
        // TWUJ STARY NAJEBANY
        if (Player.score >= 10 && TDS.g_time.TotalGameTime.TotalMilliseconds >= time_to_spawn_tsn)
        {
            time_to_spawn_tsn = (float)TDS.g_time.TotalGameTime.TotalMilliseconds + spawn_rate * 7.5f;
            tsn.Spawn();
        }

        // BOSS1
        if (Player.score >= 20 && !boss1_spawned)
        {
            boss1_spawned = true;
            boss1.Spawn();
        }
        
        // CRATE
        if (Player.score >= 20 && TDS.g_time.TotalGameTime.TotalMilliseconds >= time_to_spawn_crate && !EntityHandler.Entities.Contains(boss1))
        {
            time_to_spawn_crate = (float)TDS.g_time.TotalGameTime.TotalMilliseconds + spawn_rate * 23;
            crate.Spawn(crate.texture, crate.hit_sound);
        } 
    }

    public void Restart()
    {
        boss1_spawned = false;
    }

    public void Update()
    {
        if (Player.score >= 150 || Input.KeyPressed(Keys.F6))
        {
            GameScene.next_stage = Stages.kwatera_jaspera;
        }

        if (Input.KeyPressed(Keys.F7))
        {
            GameScene.next_stage = Stages.posnania;
        }
        Spawn();
    }

    public void Draw(SpriteBatch sb)
    { }
}