using System;
using ahn.entities.enemies;
using ahn.io;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ahn.entities;

internal enum Type
{
    life,
    firerate,
    damage,
    double_shot,
    shield
}

internal sealed class Crate : Entity
{
    private Type _type { get; init; }
    public override Texture2D texture { get; protected set; }
    public override Vector2 position { get; protected set; }
    private const int speed = 500;
    public SoundEffect hit_sound { get; private set; }

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
            var r = rnd.NextDouble();
            switch (r)
            {
                case <= 0.32:
                    type = Type.life;
                    break;
                
                case > 0.32 and <= 0.64 when Player.cumrate_max: continue;
                case > 0.32 and <= 0.64:
                    type = Type.firerate;
                    break;
                
                case > 0.64 and <= 0.86 when Player.damage_max: continue;
                case > 0.64 and <= 0.86:
                    type = Type.damage;
                    break;
                
                case > 0.86 and <= 0.90 when Player.double_shot: continue;
                case > 0.86 and <= 0.90:
                    type = Type.double_shot;
                    break;
                
                case > 0.90 when Shield.shield_on: continue;
                case > 0.90:
                    type = Type.shield;
                    break;
                
                default:
                    continue;
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
        EntityHandler.Entities.Add(c);
    }

    private void LifeUpgrade()
    {
        Player.lifes++;
    }

    private void FirerateUpgrade()
    {
        if (Player.cumrate <= 100) Player.cumrate_max = true;
        Player.cumrate -= 65;
    }

    private void DamageUpgrade()
    {
        if (Player.damage >= 10) Player.damage_max = true;
        Player.damage++;
    }

    private void DoubleShotUpgrade()
    {
        Player.double_shot = true;
    }

    private void ShieldUpgrade()
    {
        Player.summon_shield = true;
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
            case Type.double_shot:
                DoubleShotUpgrade();
                break;
            case Type.shield:
                ShieldUpgrade();
                break;
        }
    }

    protected override void Damage()
    {
        hit_sound.Play();
        health--;
    }

    protected override void Kill()
    {
        Use();
        EntityHandler.Entities.Remove(this);
    }

    protected override void AliveUpdate(Player player, Entity entity)
    {
        base.AliveUpdate(player, entity);
        position = new Vector2(position.X - speed * TDS.frame_delta, position.Y);
    }

    public void Cleanup()
    {
        texture.Dispose();
        hit_sound.Dispose();
    }
}