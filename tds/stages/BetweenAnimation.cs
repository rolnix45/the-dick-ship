using System;
using ahn.entities;
using ahn.scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
// ReSharper disable MemberCanBePrivate.Global

namespace ahn.stages;

public class BetweenAnimation : Stage
{
    private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(typeof(BetweenAnimation));
    public Stages stage_id { get; private set; }
    public Texture2D background { get; private set; }
    private Texture2D new_background { get; set;  }
    private SpriteFont font;
    private readonly Tweener tweener = new();
    private Player player;
    private bool player_to_center;
    private bool player_out_off_screen;
    private bool player_back_to_center;
    private Vector2 center;
    private Vector2 off_screen;
    public Vector2 stage_text_pos = new(TDS._winWidth * 2, TDS._winHeight / 4f);
    public float background_color { get; set; }
    private Stages next;

    public void Init(Player p)
    {
        stage_id = Stages.between;
        player = p;
        center = new Vector2(125, TDS._winHeight / 2f - p.scale / 2f);
        off_screen = new Vector2(TDS._winWidth + p.scale, center.Y);
        player_to_center = true;
        player_out_off_screen = false;
        player_back_to_center = false;
        background_color = 1f;
    }

    public void SetupNext(Texture2D new_bg, Texture2D old_bg, Stages _next)
    {
        player_to_center = true;
        player_out_off_screen = false;
        player_back_to_center = false;
        player.changing_stage = true;
        background_color = 1f;
        background = old_bg;
        new_background = new_bg;
        next = _next;
    }

    public void LoadContent(ContentManager c)
    {
        font = c.Load<SpriteFont>("bfont");
    }
    
    public void UnloadContent()
    { }
    public void Spawn()
    { }

    public void Restart()
    {
        _log.Fatal("wrong stage?");
        _log.Fatal("trying to fix...");
        throw new Exception("restart between stages is bad");
    }
    
    private void Tween()
    {
        if (player_to_center)
        {
            player_to_center = false;
            tweener.TweenTo(player, p => player.position, center, 2, 1)
                .Easing(EasingFunctions.QuadraticInOut)
                .OnEnd(_ =>
                {
                    player_out_off_screen = true;
                    if (tweener.FindTween(this, "background_alpha") == null)
                        tweener.TweenTo(this, b => background_color, 0f, 1, 0.5f)
                            .Easing(EasingFunctions.Linear)
                            .OnEnd(_ => background = new_background);
                });
        }
        else if (player_out_off_screen)
        {
            player_out_off_screen = false;
            tweener.TweenTo(player, p => player.position, off_screen, 2, 1)
                .Easing(EasingFunctions.ExponentialIn)
                .OnBegin(_ =>
                {
                    tweener.TweenTo(this, stp => stage_text_pos, 
                        new Vector2(TDS._winWidth / 2f, TDS._winHeight / 4f), 2f, 0.1f)
                        .Easing(EasingFunctions.QuarticOut)
                    .OnEnd(_ =>
                        {
                            tweener.TweenTo(this, stp => stage_text_pos,
                                    new Vector2(TDS._winWidth * -2f, TDS._winHeight / 4f), 2f)
                                .Easing(EasingFunctions.QuarticIn)
                                .OnEnd(_ =>
                                    stage_text_pos = new Vector2(TDS._winWidth * 2f, TDS._winHeight / 4f)
                                );
                        });
                })
                .OnEnd(_ =>
                {
                    player_back_to_center = true;
                });
        }
        else if (player_back_to_center)
        {
            player_back_to_center = false;
            tweener.TweenTo(player, p => player.position, center, 2, 1)
                .OnBegin(_ =>
                {
                    tweener.TweenTo(this, b => background_color, 1f, 1)
                        .Easing(EasingFunctions.Linear);
                })
                .Easing(EasingFunctions.CircleOut)
                .OnEnd(_ =>
                {
                    GameScene.next_stage = next;
                    GameScene.anim_end = true;
                    player.changing_stage = false;
                });
        }
    }
    
    public void Update()
    {
        Tween();
        tweener.Update(TDS.g_time.GetElapsedSeconds());
    }

    public void Draw(SpriteBatch sb)
    {
        string t;
        switch (next)
        {
            case Stages.dick_space:
                t = "THE DICK SPACE";
                break;
            case Stages.kwatera_jaspera:
                t = "KWATERA JASPERA";
                break;
            case Stages.posnania:
                t = "POSNANIA";
                break;
            default:
                t = null;
                break;
        }
        sb.DrawString(
            font, 
            t,
            stage_text_pos,
            Color.Beige,
            0f,
            font.MeasureString(t) / 2f,
            1f,
            SpriteEffects.None,
            1f
        );
        sb.Draw(
            background,
            Vector2.Zero,
            new Color(new Vector3(background_color))
        );
        player.Draw(sb);
    }
}