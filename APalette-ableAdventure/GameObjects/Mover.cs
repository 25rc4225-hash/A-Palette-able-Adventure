using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace APalette_ableAdventure.GameObjects;

public class Mover
{
    private const float MOVEMENT_SPEED = 2.0f;

    // The _sprite that is being modified
    private Sprite _sprite;

    // String for storing current movement direction
    private Vector2 _direction;

    private string _type;

    /// Gets or Sets position of mover
    public Vector2 Position { get; set; }

    /// Creates Mover
    public Mover(string type, Sprite R, Sprite L, Sprite U, Sprite D, int X, int Y)
    {
        _type = type;
        switch (_type)
        {
            case "R": _sprite = R; break;
            case "L": _sprite = L; break;
            case "U": _sprite = U; break;
            case "D": _sprite = D; break;
        }
    }

    /// Initializes the Mover, can be used to reset it back to an initial state.
    public void Initialize(Vector2 startingPosition)
    {
        Position = startingPosition;
    }

    /// Moves mover
    private void Move()
    {
        switch (_type)
        {
            case "R": _direction = Vector2.UnitX; break;
            case "L": _direction = -Vector2.UnitX; break;
            case "U": _direction = -Vector2.UnitY; break;
            case "D": _direction = Vector2.UnitY; break;
        }
        // Update the position of the Mover based on the velocity.
        Position += _direction * MOVEMENT_SPEED;
    }

    /// Updates Mover
    public void Update(GameTime gameTime)
    {
        // Checks input buffer for movement
        Move();
    }

    /// Draws mover
    public void Draw()
    {
        _sprite.Draw(Core.SpriteBatch, Position);
    }

    /// Returns rectangle bounds for Mover
    public Rectangle GetBounds()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, (int)_sprite.Width, (int)_sprite.Height);
    }

    /// Shifts the mover by 1 so it stays in the room bounds
    public void Shift(string direction)
    {
        switch (direction)
        {
            case "up": Position -= Vector2.UnitY; break;
            case "down": Position += Vector2.UnitY; break;
            case "left": Position -= Vector2.UnitX; break;
            case "right": Position += Vector2.UnitX; break;
        }
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