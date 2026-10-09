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
    public Vector2 startPosition { get; set; }
    private bool hidden = false;
    private Sprite rSprite;
    private Sprite lSprite;
    private Sprite uSprite;
    private Sprite dSprite;
    private bool affected = false;

    /// Creates Mover
    public Mover(string type, List<Sprite> Sprites, int X, int Y)
    {
        _type = type;
        rSprite = Sprites[0];
        lSprite = Sprites[1];
        uSprite = Sprites[2];
        dSprite = Sprites[3];
        switch (_type)
        {
            case "R": _sprite = rSprite; break;
            case "L": _sprite = lSprite; break;
            case "U": _sprite = uSprite; break;
            case "D": _sprite = dSprite; break;
        }
        Position = new Vector2(X, Y);
        startPosition = Position;
    }

    /// Initializes the Mover, can be used to reset it back to an initial state.
    public void Initialize()
    {
        Position = startPosition;
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

    public string getType()
    {
        return _type;
    }
    public void affect()
    {
        affected = true;
    }
    public void unaffect()
    {
        affected = false;
    }
    public bool isAffected() 
    { 
        return affected; 
    }
    public void changeType(string newType)
    {
        _type = newType;
        switch (_type)
        {
            case "R": _sprite = rSprite; break;
            case "L": _sprite = lSprite; break;
            case "U": _sprite = uSprite; break;
            case "D": _sprite = dSprite; break;
        }
    }
    public int getX()
    {
        return (int)Position.X;
    }
    public int getY()
    {
        return (int)Position.X;
    }

    /// Updates Mover
    public void Update(GameTime gameTime)
    {
        // Checks input buffer for movement
        Move();
    }

    public void Hide()
    {
        hidden = true;
    }
    public void unHide()
    {
        hidden = false;
    }

    /// Draws mover
    public void Draw()
    {
        if (!hidden)
        {
            _sprite.Draw(Core.SpriteBatch, Position);
        }
    }

    /// Returns rectangle bounds for Mover
    public Rectangle GetBounds()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, (int)_sprite.Width, (int)_sprite.Height);
    }

    /// Shifts the mover by 1 so it stays in the room bounds
    public void Shift(string direction, int amount)
    {
        switch (direction)
        {
            case "up": Position -= Vector2.UnitY * amount; break;
            case "down": Position += Vector2.UnitY * amount; break;
            case "left": Position -= Vector2.UnitX * amount; break;
            case "right": Position += Vector2.UnitX * amount; break;
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