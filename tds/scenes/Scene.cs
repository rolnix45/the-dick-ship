using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ahn.scenes;

public interface Scene
{
    public int scene_id { get; }
    public void Init();
    public void LoadContent(ContentManager c);
    public void UnloadContent();
    public void Update();
    public void Draw(SpriteBatch sb);
}