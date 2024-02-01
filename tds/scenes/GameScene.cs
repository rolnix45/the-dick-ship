using System.Collections.Generic;
using System.Linq;
using ahn.entities;
using ahn.entities.enemies;
using ahn.io;
using ahn.particles;
using ahn.stages;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Content;

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
    
    private static readonly Dictionary<Stages, Stage> _stages = new();
    private static Stages current_stage { get; set; }
    public static Stages next_stage { get; set; }
    private readonly DickSpace dick_space = new();
    private readonly KwateraJaspera kwatera_jaspera = new();
    private readonly Posnania posnania = new();
    private readonly BetweenAnimation between_animation = new();
    
    private Player player { get; set; }
    private EntityHandler entityHandler;
    private ContentManager _contentManager;
    
    private SpriteFont font;
    private SpriteFont bfont;
    private Texture2D rect;
    
    private bool death_sound_played;
    private bool show_debug_info = true;
    public static bool damaged { get; set; }
    public static bool draw_hitboxes { get; private set; }
    public static bool anim_end { get; set; }

    public GameScene() =>
        scene_id = 1;

    public void Init()
    {
        _stages.Add(Stages.between, between_animation);
        _stages.Add(Stages.dick_space, dick_space);
        _stages.Add(Stages.kwatera_jaspera, kwatera_jaspera);
        _stages.Add(Stages.posnania, posnania);
        _state = State.Running;
        player = new Player();
        entityHandler = new EntityHandler();
        player.Init();
        current_stage = Stages.dick_space;
        next_stage = current_stage;
        _stages[current_stage].Init(player);
        _stages[Stages.between].Init(player);
        anim_end = false;
    }
    
    private void ChangeStage(bool to_betw, Stages new_stage)
    {
        if (current_stage != Stages.between && to_betw)
        {
            entityHandler.KillAllEnemiesAndBullets();
            _stages[Stages.between].Init(player);
            _stages[Stages.between].LoadContent(_contentManager);
            _stages[new_stage].Init(player);
            _stages[new_stage].LoadContent(_contentManager);
            between_animation.SetupNext(_stages[new_stage].background, _stages[current_stage].background, new_stage);
            _log.Info("stage changed to " + Stages.between);
        }
        else
        {
            next_stage = new_stage;
            current_stage = new_stage;
            _log.Info("stage changed to " + new_stage);
        }
    }
    
    public void LoadContent(ContentManager c)
    {
        _contentManager = c;
        font = c.Load<SpriteFont>("font");
        bfont = c.Load<SpriteFont>("bfont");
        rect = new Texture2D(c.GetGraphicsDevice(), 1, 1);
        rect.SetData(new []{Color.White});
        player.LoadContent(c);
        _stages[current_stage].LoadContent(c);
    }

    public void UnloadContent()
    {
        rect.Dispose();
        player.Cleanup();
        entityHandler.Cleanup();
        _stages[current_stage].UnloadContent();
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
        TDS.clip_cursor = true;
        TDS.show_cursor = false;
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

        if (Input.KeyPressed(Keys.F4))
        {
            Player.lifes++;
        }
        
        if (anim_end)
        {
            anim_end = false;
            current_stage = next_stage;
            ChangeStage(false, next_stage);
        }

        if (next_stage != current_stage)
        {
            ChangeStage(true, next_stage);
            next_stage = Stages.between;
            current_stage = Stages.between;
        }
    }

    private void UpdateEntities()
    {
        foreach (var e in EntityHandler.Entities.ToList())
        {
            e.Update(player, e);
        }
        damaged = false;

        foreach (var b in EntityHandler.EnemyBullets.ToList().Where(b => b.tag is "boss1" or "imposter" or "twujstarynajebany"))
        {
            if (b.position.X > 0 && b.is_alive)
            {
                var newpos = new Vector2(b.position.X + b.delta_x * TDS.frame_delta,
                    b.position.Y + b.delta_y * TDS.frame_delta);
                b.position = newpos;
            }
            else
            {
                EntityHandler.EnemyBullets.Remove(b);
            }
        }
    }

    private void Restart()
    {
        entityHandler.KillAllEnemiesAndBullets();
        death_sound_played = false;
        UpdateEntities();
        player.Init();
        _stages[current_stage].Restart();
        _state = State.Running;
        _log.Info("restarted");
    }
    
    private void PausedEvents()
    {
        TDS.clip_cursor = false;
        TDS.show_cursor = true;
        if (_state == State.Dead && !death_sound_played)
        {
            death_sound_played = true;
            player.death_sound.Play();
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
    
    public void Update()
    {
        switch (_state)
        {
            case State.Running:
                RunningEvents();
                UpdateEntities();
                player.Update();
                _stages[current_stage].Update();
                ParticleSystem.UpdateParticles();
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
        var info =
            "DLT: " + TDS.frame_delta.ToString("0.000") +
            "\nTIM: " + TDS.g_time.TotalGameTime.TotalMilliseconds.ToString("0") +
            "\nPBC: " + Player._bullets.Count +
            "\nENM: " + EntityHandler.Entities.Count +
            "\nEBC: " + EntityHandler.EnemyBullets.Count +
            "\nPOS: " + player.position.X.ToString("0") + " " + player.position.Y.ToString("0") +
            "\nDMG: " + Player.damage +
            "\nCUR: " + Player.cumrate +
            "\nPAR: " + ParticleSystem.ParticlesList.Count +
            "\nSTG: " + current_stage;
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
        if (_stages[current_stage] is BetweenAnimation) return;
        sb.Draw(
            _stages[current_stage].background,
            Vector2.Zero,
            _state != State.Dead ? Color.White : Color.Red
        );
        ParticleSystem.DrawParticles(sb);
        entityHandler.Draw(sb);
        player.Draw(sb);
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
        sb.Draw(
            rect,
            new Rectangle(TDS._winWidth / 2 - 175, TDS._winHeight / 2 - 87, 350, 175),
            new Rectangle(0, 0, 1, 1),
            Color.Black,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            0.9f
        );
        sb.DrawString(
            bfont,
            "DEAD",
            new Vector2(TDS._winWidth / 2f, TDS._winHeight / 2f),
            Color.Crimson,
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
        sb.Draw(
            rect,
            new Rectangle(TDS._winWidth / 2 - 175, TDS._winHeight / 2 - 87, 350, 175),
            new Rectangle(0, 0, 1, 1),
            Color.Navy,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            0.9f
        );
        sb.DrawString(
            bfont,
            "PAUSED",
            new Vector2(TDS._winWidth / 2f, TDS._winHeight / 2f),
            Color.White,
            0f,
            bfont.MeasureString("PAUSED") / 2f,
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
                _stages[current_stage].Draw(sb);
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
}