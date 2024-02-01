using ahn.entities;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ahn.stages;

public enum Stages
{
    between,
    dick_space,
    kwatera_jaspera,
    posnania
}

public interface Stage
{
    public Stages stage_id { get; }
    Texture2D background { get; }
    public void Init(Player p);
    public void LoadContent(ContentManager c);
    public void UnloadContent();
    public void Spawn();
    public void Restart();
    public void Update();
    public void Draw(SpriteBatch sb);
}