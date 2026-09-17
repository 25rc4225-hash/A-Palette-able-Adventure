using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace APalette_ableAdventure.GameObjects;
public class Water
{
    private AnimatedSprite _sprite;

    private string _type;

    /// Gets or Sets position of block
    public Vector2 Position { get; set; }

    public Water(string Type, List<AnimatedSprite> Animations, int X, int Y)
    {
        _type = Type;
        switch (_type)
        {
            case "W1": _sprite = Animations[0]; break;
            case "W2": _sprite = Animations[1]; break;
            case "M": _sprite = Animations[2]; break;
            case "F": _sprite = Animations[3]; break;
        }
        Position = new Vector2(X, Y);
    }

    public bool IsSurface()
    {
        if (_type == "W1" || _type == "W2")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// Updates water
    public void Update(GameTime gameTime)
    {
        _sprite.Update(gameTime);
    }

    /// Draws water
    public void Draw()
    {
        _sprite.Draw(Core.SpriteBatch, Position);
    }

    /// Returns rectangle bounds for water
    public Rectangle GetBounds()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, (int)_sprite.Width, (int)_sprite.Height);
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
}
