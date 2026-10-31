using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace APalette_ableAdventure.GameObjects;

public class Effector
{
    private string _type;
    private bool hidden = false;
    private readonly AnimatedSprite _sprite;
    private bool isAffecting = false;

    /// Gets or Sets position of effector
    public Vector2 Position { get; set; }
    public Effector(string Type, List<AnimatedSprite> Animations, float X, float Y)
    {
        _type = Type;
        switch (_type)
        {
            case "X": _sprite = Animations[0]; break;
            case "C": _sprite = Animations[1]; break;
            case "A": _sprite = Animations[2]; break;
            case "V": _sprite = Animations[3]; break;
            case "H": _sprite = Animations[4]; break;
        }
        Position = new Vector2(X, Y);
        hidden = false;
    }
    public void Hide()
    {
        hidden = true;
    }
    public void unHide()
    {
        hidden = false;
    }

    /// Draws effector
    public void Draw()
    {
        if (!hidden)
        {
            _sprite.Draw(Core.SpriteBatch, Position);
        }
    }
    
    /// Returns rectangle bounds for effector
    public Rectangle GetBounds()
    {
        if (!hidden)
        {
            return new Rectangle((int)Position.X, (int)Position.Y, (int)_sprite.Width, (int)_sprite.Height);
        }
        else
        {
            return new Rectangle(0, 0, 0, 0);
        }
    }
    /// Updates effector
    public void Update(GameTime gameTime)
    {
        _sprite.Update(gameTime);
    }
    public string whichType()
    {
        return _type;
    }

    // Return sprite height
    public int GetHeight()
    {
        return (int)_sprite.Height;
    }

    // Returns sprite width
    public int GetWidth()
    {
        return (int)_sprite.Width;
    }
    public int getX()
    {
        return (int)Position.X;
    }
    public int getY()
    {
        return (int)Position.Y;
    }
    public void Affect()
    {
        isAffecting = true;
    }
    public bool hasAffected()
    {
        return isAffecting;
    }
    public void unAffect()
    {
        isAffecting = false;
    }
}
