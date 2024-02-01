using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using ahn.io;
using ahn.scenes;
using log4net.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ahn;

public class TDS : Game
{
    private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(typeof(TDS));
    private readonly string[] args;
    private static readonly Dictionary<int, Scene> _scenes = new();
    private int active_scene;
    
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    public const int _winWidth = 1280;
    public const int _winHeight = 960;
    
    private float d_mult = 1f;
    public static float frame_delta { get; private set; }
    public static GameTime g_time { get; private set; }
    
    private static bool close;
    public static bool clip_cursor { get; set; }
    public static bool show_cursor { get; set; }

    private Rectangle client_bounds;
    private Rectangle no_cursor_bounds;

    [DllImport("user32.dll")]
    static extern void ClipCursor(ref Rectangle r);
    [DllImport("user32.dll")]
    static extern void GetClipCursor(ref Rectangle r);

    public TDS(string[] args)
    {
        XmlConfigurator.Configure(File.OpenRead("log4net.config"));
        _log.Info("starting...");
        this.args = args;
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        clip_cursor = true;
        show_cursor = false;
        IsMouseVisible = show_cursor;
        var ts = new TitleScene();
        _scenes.Add(ts.scene_id, ts);
        var gs = new GameScene();
        _scenes.Add(gs.scene_id, gs);
        _log.Debug("DEBUG");
        _log.Info("INFO");
        _log.Warn("WARNING");
        _log.Error("ERROR");
        _log.Fatal("FATAL");
    }

    private void HandleArgs()
    {
        _log.Debug("arg count " + args.Length);
        foreach (var a in args)
        {
            _log.Debug(a);
            switch (a)
            {
                case "--fastmode":
                    _log.Info("fastmode on");
                    d_mult = 1.33f;
                    break;
                case "--slowmode":
                    _log.Info("slowmode on");
                    d_mult = 0.667f;
                    break;
            }
        }
    }

    public static void Close() =>
        close = true;

    private void ChangeScene(int scene_id)
    {
        _scenes[active_scene].UnloadContent();
        _scenes[scene_id].Init();
        _scenes[scene_id].LoadContent(Content);
        active_scene = scene_id;
        _log.Info($"scene changed to {scene_id.ToString()}");
    }
    
    protected override void Initialize()
    {
        HandleArgs();
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = _winWidth;
        _graphics.PreferredBackBufferHeight = _winHeight;
        GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        _graphics.ApplyChanges();
        _scenes[active_scene].Init();
        client_bounds = Window.ClientBounds;
        client_bounds.Width += client_bounds.X;
        client_bounds.Height += client_bounds.Y;
        GetClipCursor(ref no_cursor_bounds);
        base.Initialize();
        _log.Info("initialized!");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("Debug Controlls: \n" +
                      "F1 Kill Player\n" +
                      "F2 Debug Info\n" +
                      "F3 Show Hitboxes\n" +
                      "F4 Add Life\n" + 
                      "F5 Change Stage To Dick Space\n" +
                      "F6 Change Stage To Kwatera Jaspera\n");
        Console.ForegroundColor = ConsoleColor.White;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _scenes[active_scene].LoadContent(Content);
    }

    protected override void UnloadContent()
    {
        _scenes[active_scene].UnloadContent();
        _spriteBatch.Dispose();
    }

    protected override void Update(GameTime gameTime)
    {
        g_time = gameTime;
        if (clip_cursor && IsActive)
        {
            ClipCursor(ref client_bounds);
        }
        else
        {
            ClipCursor(ref no_cursor_bounds);
        }
        IsMouseVisible = show_cursor;
        Input.Update();
        if (active_scene == 0 && Keyboard.GetState().IsKeyDown(Keys.Enter)) ChangeScene(1); 
        _scenes[active_scene].Update();
        base.Update(gameTime);
        frame_delta = gameTime.ElapsedGameTime.Milliseconds / 1000f * d_mult;
        if (!close) return;
        _scenes[active_scene].UnloadContent();
        Exit();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        _scenes[active_scene].Draw(_spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}