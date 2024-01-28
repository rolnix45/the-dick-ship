using System.Linq;
using ahn.entities;
using ahn.entities.enemies;
using ahn.io;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ahn.scenes;

public enum State
{
    Running,
    Dead,
    Paused
}

public class GameScene : Scene
{
    private static readonly ILog _log =
        LogManager.GetLogger(typeof(GameScene));
    
    public static State _state { get; private set; }
    public int scene_id { get; }
    private Player _player { get; set; }
    private Boss1 _boss1 { get; set; }
    private Crate _crate { get; set; }
    private EnemyHandler _enemy_handler;
    private Texture2D background;
    private SpriteFont font;
    private SpriteFont bfont;
    private bool show_debug_info = true;
    private Texture2D imp_texture;
    private Texture2D imp_bull_texture;
    private SoundEffect e_hit_sound;
    private SoundEffect e_def_hit_sound;
    private bool death_sound_played;
    private bool boss1_spawned;
    public static bool damaged { get; set; }
    public static bool draw_hitboxes { get; private set; }

    public GameScene() =>
        scene_id = 1;

    public void Init(GraphicsDeviceManager gdm)
    {
        _state = State.Running;
        _player = new Player();
        _enemy_handler = new EnemyHandler();
        _player.Init();
        _boss1 = new Boss1(_player);
        _crate = new Crate();
    }
    
    public void LoadContent(ContentManager c)
    {
        background = c.Load<Texture2D>("background");
        font = c.Load<SpriteFont>("font");
        bfont = c.Load<SpriteFont>("bfont");
        imp_texture = c.Load<Texture2D>("enemy1");
        imp_bull_texture = c.Load<Texture2D>("enemy1Bullet");
        e_hit_sound = c.Load<SoundEffect>("enemyHit");
        e_def_hit_sound = c.Load<SoundEffect>("enemyHitProt");
        _player.LoadContent(c);
        _boss1.LoadContent(c);
        _crate.LoadContent(c);
    }

    public static void DamagePlayer()
    {
        if (Player.invincible) return;
        Player.lifes--;
        if (Player.lifes > 0) return;
        _state = State.Dead;
        _log.Debug("player dead");
    }

    private void RunningEvents()
    {
        if (Input.KeyPressed(Keys.F1))
        {
            DamagePlayer();
        }

        if (Input.KeyPressed(Keys.F2))
        {
            show_debug_info = !show_debug_info;
        }

        if (Input.KeyPressed(Keys.F3))
        {
            draw_hitboxes = !draw_hitboxes;
        }
        
        if (Input.KeyPressed(Keys.Escape))
        {
            _state = State.Paused;
        }
    }

    private void UpdateEnemies()
    {
        foreach (var e in EnemyHandler.Enemies.ToList())
        {
            e.Update(_player, e);
        }
        damaged = false;

        foreach (var b in EnemyHandler.EnemyBullets.ToList())
        {
            switch (b.tag)
            {
                case "boss1" or "imposter":
                    if (b.position.X > 0 && b.is_alive)
                    {
                        var newpos = new Vector2(b.position.X + b.delta_x * TDS.frame_delta, b.position.Y + b.delta_y * TDS.frame_delta);
                        b.position = newpos;
                    }
                    else
                    {
                        EnemyHandler.EnemyBullets.Remove(b);
                    }
                    break;
            }
        }
    }

    private void Restart()
    {
        _enemy_handler.KillAllEnemiesAndBullets();
        death_sound_played = false;
        boss1_spawned = false;
        UpdateEnemies();
        _player.Init();
        _state = State.Running;
        _log.Info("restarted");
    }
    
    private void PausedEvents()
    {
        if (_state == State.Dead && !death_sound_played)
        {
            death_sound_played = true;
            _player.death_sound.Play();
        }
        
        if (Input.KeyPressed(Keys.Back))
        {
            Restart();
        }

        if (Input.KeyPressed(Keys.Escape) && _state == State.Paused)
        {
            _state = State.Running;
        }

        if (Input.KeyHeld(Keys.Escape) && Input.KeyHeld(Keys.LeftShift))
        {
            TDS.Close();
        }
    }

    private float time_to_spawn_imp;
    private float time_to_spawn_crate;
    private const int spawn_rate = 500;
    private void Spawn()
    {
        // IMPOSTER
        if (TDS.g_time.TotalGameTime.TotalMilliseconds >= time_to_spawn_imp)
        {
            time_to_spawn_imp = (float)TDS.g_time.TotalGameTime.TotalMilliseconds + spawn_rate;
            Imposter.Spawn(imp_texture, imp_bull_texture, e_hit_sound, e_def_hit_sound);
        }

        // BOSS1
        if (Player.score >= 20 && !boss1_spawned)
        {
            boss1_spawned = true;
            _boss1.Spawn();
        }
        
        // CRATE
        if (Player.score >= 20 && TDS.g_time.TotalGameTime.TotalMilliseconds >= time_to_spawn_crate && !EnemyHandler.Enemies.Contains(_boss1))
        {
            time_to_spawn_crate = (float)TDS.g_time.TotalGameTime.TotalMilliseconds + spawn_rate * 23;
            _crate.Spawn(_crate.texture, _crate.geths());
        } 
    }
    
    public void Update()
    {
        switch (_state)
        {
            case State.Running:
                RunningEvents();
                UpdateEnemies();
                Spawn();
                _player.Update();
                break;
            case State.Paused:
                PausedEvents();
                break;
            case State.Dead:
                PausedEvents();
                break;
        }
    }

    private void DebugInfo(SpriteBatch sb)
    {
        var info = string.Format(
            "DLT: {0:0.000}\nTIM: {1:0}\nPBC: {2}\nENM: {3}\nEBC: {4}\nPOS: {5:0} {6:0}\nDMG: {7}\nFIR: {8}", 
            TDS.frame_delta,
            TDS.g_time.TotalGameTime.TotalMilliseconds,
            Player._bullets.Count,
            EnemyHandler.Enemies.Count,
            EnemyHandler.EnemyBullets.Count,
            _player.position.X, _player.position.Y,
            Player.damage,
            Player.fire_rate
        );
        sb.DrawString(
            font,
            info,
            new Vector2(10, 10), 
            Color.White,
            0,
            Vector2.Zero,
            1.0f, 
            SpriteEffects.None,
            0.5f
        );
    }

    private void CommonDraw(SpriteBatch sb)
    {
        sb.Draw(
            background,
            Vector2.Zero,
            Color.White
        );
        _enemy_handler.Draw(sb);
        _player.Draw(sb);
        var info = $"LIFES: {Player.lifes}\n" +
                       $"SCORE: {Player.score}";
        sb.DrawString(
            font,
            info,
            new Vector2(10, TDS._winHeight - 55),
            Color.White,
            0f,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            1f
        );
        if (show_debug_info) DebugInfo(sb);
    }

    private void DeadDraw(SpriteBatch sb)
    {
        sb.DrawString(
            font,
            "HOLD LSHIFT AND ESC TO CLOSE\nRESTART WITH BACKSPACE",
            new Vector2(TDS._winWidth - 400, 5),
            Color.White
        );
        sb.DrawString(
            bfont,
            "DEAD",
            new Vector2(TDS._winWidth / 2f, TDS._winHeight / 2f),
            Color.White,
            0f,
            bfont.MeasureString("DEAD") / 2f,
            1f,
            SpriteEffects.None,
            1f
        );
    }

    private void PausedDraw(SpriteBatch sb)
    {
        sb.DrawString(
            font,
            "HOLD LSHIFT AND ESC TO CLOSE\nRESTART WITH BACKSPACE",
            new Vector2(TDS._winWidth - 400, 5),
            Color.White
        );
        const string text = "chuj";
        sb.DrawString(
            bfont,
            text,
            new Vector2(TDS._winWidth / 2f, TDS._winHeight / 2f),
            Color.White,
            0f,
            bfont.MeasureString(text) / 2f,
            1f,
            SpriteEffects.None,
            1f
        );
    }
    
    public void Draw(SpriteBatch sb)
    {
        switch (_state)
        {
            case State.Running:
                CommonDraw(sb);
                break;
            case State.Paused:
                CommonDraw(sb);
                PausedDraw(sb);
                break;
            case State.Dead:
                CommonDraw(sb);
                DeadDraw(sb);
                break;
        }
    }

    public void CleanUp()
    {
        _boss1.SCleanup();
        _player.Cleanup();
        _enemy_handler.Cleanup();
        e_hit_sound.Dispose();
        e_def_hit_sound.Dispose();
        background.Dispose();
    }
}