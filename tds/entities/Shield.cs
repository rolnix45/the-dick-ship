using ahn.entities.enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ahn.entities;

internal sealed class Shield : Entity
{
    public override Texture2D texture { get; protected set; }
    public override Vector2 position { get; protected set; }
    private SoundEffect defl_hit_sound { get; set; }
    public static bool shield_on;

    public void LoadContent(ContentManager c)
    {
        texture = c.Load<Texture2D>("shield");
        defl_hit_sound = c.Load<SoundEffect>("enemyHitProt");
    }

    public void Summon()
    {
        var s = new Shield
        {
            defl_hit_sound = defl_hit_sound,
            texture = texture,
            position = position,
            health = 10
        };
        EntityHandler.Entities.Add(s);
    }

    protected override void Damage()
    {
        health--;
        defl_hit_sound.Play();
    }

    protected override void Kill()
    {
        shield_on = false;
        EntityHandler.Entities.Remove(this);
    }

    protected override void AliveUpdate(Player p, Entity e)
    {
        base.AliveUpdate(p, e);
        position = new Vector2(p.position.X + 55, p.position.Y);
    }

    public void Cleanup()
    {
        texture.Dispose();
        defl_hit_sound.Dispose();
    }
}