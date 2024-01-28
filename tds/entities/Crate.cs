using System;
using ahn.entities.enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ahn.entities;

internal enum Type
{
    life,
    firerate,
    damage
}

internal sealed class Crate : Enemy
{
    private Type _type { get; init; }
    public override Texture2D texture { get; protected set; }
    public override Vector2 position { get; protected set; }
    public override int scale { get; }
    private const int speed = 500;
    protected override SoundEffect hit_sound { get; set; }
    protected override SoundEffect defl_hit_sound { get; set; }

    public Crate()
    {
        scale = 64;
        health = 3;
    }
    
    public void LoadContent(ContentManager c)
    {
        texture = c.Load<Texture2D>("crate");
        hit_sound = c.Load<SoundEffect>("enemyHit");
    }

    private readonly Random rnd = new();
    private Type RollType()
    {
        while (true)
        {
            Type type;
            var typer = rnd.Next(1, 4);
            if (typer % 3 == 0)
            {
                type = Type.life;
            }
            else if (typer % 2 == 0)
            {
                type = Type.firerate;
                if (Player.firerate_max) continue;
            }
            else
            {
                type = Type.damage;
                if (Player.damage_max) continue;
            }
            return type;
        }
    }

    public void Spawn(Texture2D t, SoundEffect hs)
    {
        var c = new Crate
        {
            position = new Vector2(TDS._winWidth + 64, rnd.Next(0, TDS._winHeight - scale)),
            _type = RollType(),
            texture = t,
            hit_sound = hs
        };
        EnemyHandler.Enemies.Add(c);
    }

    private void LifeUpgrade()
    {
        Player.lifes++;
    }

    private void FirerateUpgrade()
    {
        if (Player.fire_rate <= 100) Player.firerate_max = true;
        Player.fire_rate -= 65;
    }

    private void DamageUpgrade()
    {
        if (Player.damage >= 10) Player.damage_max = true;
        Player.damage++;
    }

    private void Use()
    {
        switch (_type)
        {
            case Type.life:
                LifeUpgrade();
                break;
            case Type.firerate:
                FirerateUpgrade();
                break;
            case Type.damage:
                DamageUpgrade();
                break;
        }
    }

    public SoundEffect geths()
    {
        return hit_sound;
    }
    
    public override void Update(Player player, Enemy enemy)
    {
        base.Update(player, enemy);
        if (health <= 0)
        {
            Use();
            EnemyHandler.Enemies.Remove(this);
        }
        position = new Vector2(position.X - speed * TDS.frame_delta, position.Y);
    }
}