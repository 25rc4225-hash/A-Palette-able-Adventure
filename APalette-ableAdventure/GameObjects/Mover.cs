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
    protected Sprite _sprite;
    protected Sprite startSprite;

    // String for storing current movement direction
    private Vector2 _direction;

    private string _type;
    private string startType;

    /// Gets or Sets position of mover
    public Vector2 Position { get; set; }
    public Vector2 startPosition { get; set; }
    private bool hidden = false;
    private Sprite rSprite;
    private Sprite lSprite;
    private Sprite uSprite;
    private Sprite dSprite;
    private bool isAffecting = false;
    private List<string> cantMove = new List<string>();

    /// Creates Mover
    public Mover(string type, List<Sprite> Sprites, float X, float Y)
    {
        _type = type;
        startType = _type;
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
        startSprite = _sprite;
        Position = new Vector2(X, Y);
        startPosition = Position;
    }
    public Mover() { }

    /// Initializes the Mover, can be used to reset it back to an initial state.
    public void Initialize()
    {
        Position = startPosition;
        _type = startType;
        _sprite = startSprite;
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

    public virtual string getType()
    {
        return _type;
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
        return (int)Position.Y;
    }

    /// Updates Mover
    public virtual void Update(GameTime gameTime)
    {
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

    public bool Movable(string direction)
    {
        return !cantMove.Contains(direction);
    }
    public void noMoving(string direction)
    {
        if (!cantMove.Contains(direction))
        {
            cantMove.Add(direction);
        }
    }
    public void resetMove(string direction)
    {
        cantMove.RemoveAll(x => x == direction);
    }
    public void totalMoveReset()
    {
        cantMove = new List<string>();
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


    public virtual void Land() { }
}