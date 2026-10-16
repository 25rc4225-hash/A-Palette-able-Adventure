using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace APalette_ableAdventure.GameObjects;

public class Portal
{
    private AnimatedSprite _sprite;

    public Vector2 Position { get; set; }

    public Portal(AnimatedSprite sprite, int X, int Y)
    {
        _sprite = sprite;
        Position = new Vector2(X, Y);
    }
    /// Updates portal
    public void Update(GameTime gameTime)
    {
        _sprite.Update(gameTime);
    }

    /// Draws portal
    public void Draw()
    {
        _sprite.Draw(Core.SpriteBatch, Position);
    }

    /// Returns rectangle bounds for portal
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
