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

    private readonly AnimatedSprite _sprite;

    /// Gets or Sets position of effector
    public Vector2 Position { get; set; }
    public Effector(string Type, List<AnimatedSprite> Animations, int X, int Y)
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
    }
    /// Draws effector
    public void Draw()
    {
        _sprite.Draw(Core.SpriteBatch, Position);
    }
    /// Returns rectangle bounds for effector
    public Rectangle GetBounds()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, (int)_sprite.Width, (int)_sprite.Height);
    }
    /// Updates effector
    public void Update(GameTime gameTime)
    {
        _sprite.Update(gameTime);
    }
}
