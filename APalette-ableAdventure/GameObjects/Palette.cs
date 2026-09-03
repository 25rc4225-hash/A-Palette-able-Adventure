using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace APalette_ableAdventure.GameObjects;

public class Palette
{
    // Palette has constant speed of 5
    private const float MOVEMENT_SPEED = 4.0f;

    // The _sprite that is being modified
    private AnimatedSprite _sprite;

    // String for storing current movement direction
    private string _movement = "N";

    // Sound effect for palette jumping
    private SoundEffect _jumpingSoundEffect;

    // Buffer to queue inputs input by player during input polling.
    private Queue<Vector2> _inputBuffer;

    // The maximum size of the buffer queue.
    private const int MAX_BUFFER_SIZE = 2;

    // Next move in the buffer
    private Vector2 _nextMove;

    // Previous move in the buffer
    private Vector2 _preMove = Vector2.Zero;

    // Whether jumping or not
    private bool _jumping;

    // Progression through jumping
    private int _jumpPoint = 0;

    // Whether in the air or not
    private bool _inAir = true;

    // Progression through jumping
    private int _fallPoint = 0;

    // Dictionary to store different palette movement animations
    private Dictionary<string, AnimatedSprite> _animations = new Dictionary<string, AnimatedSprite>();

    /// Gets or Sets position of palette
    public Vector2 Position { get; set; }

    /// Creates palette
    public Palette(AnimatedSprite none, AnimatedSprite right, AnimatedSprite left,/* AnimatedSprite air,*/ SoundEffect jumpingSoundEffect, int x, int y)
    {
        _animations.Add("N", none);
        _animations.Add("R", right);
        _animations.Add("L", left);
        //_animations["U"] = air;
        _jumpingSoundEffect = jumpingSoundEffect;
        Position = new Vector2(x, y);
    }

    /// Initializes the palette, can be used to reset it back to an initial state.
    public void Initialize()
    {
        // initialize the input buffer.
        _inputBuffer = new Queue<Vector2>(MAX_BUFFER_SIZE);
    }

    /// Adds movements to the input buffer
    private void HandleInput()
    {
        Vector2 potentialNextDirection = Vector2.Zero;

        if (GameController.MoveUp())
        {
            potentialNextDirection = -Vector2.UnitY;
        }
        else if (GameController.MoveLeft())
        {
            potentialNextDirection = -Vector2.UnitX;
        }
        else if (GameController.MoveRight())
        {
            potentialNextDirection = Vector2.UnitX;
        }

        // If a new direction was input, consider adding it to the buffer
        if (_inputBuffer.Count < MAX_BUFFER_SIZE)
        {
            _inputBuffer.Enqueue(potentialNextDirection);
        }
    }

    /// Moves palette
    private void Move()
    {
        // Get the next direction from the input buffer if one is available
        if (_inputBuffer.Count > 0)
        {
            _nextMove = _inputBuffer.Dequeue();
        }

        // Checks if you can jump then does if possible
        if (_nextMove == -Vector2.UnitY && _inAir == false)
        {
            _jumping = true;
            _inAir = true;
            _jumpPoint = 20;
            // Play the jump sound effect.
            Core.Audio.PlaySoundEffect(_jumpingSoundEffect);
        }

        // If in the air then repeat previous move
        if (_inAir)
        {
            _nextMove = _preMove;
        }

        switch (_nextMove)
        {
            case (-1,0): _movement = "L"; break;
            case (1, 0): _movement = "R"; break;
            case (0, 0): _movement = "N"; break;
        }

        // Assigns current move for the next repetition as _preMove
        _preMove = _nextMove;

        // Horizontal velocity is increased for more natural movement
        _nextMove *= 1.25f;

        // Applies any effect due to jump cycle
        _nextMove -= Vector2.UnitY * (_jumpPoint / 5);

        // If not jumping but still in the air, fall by 1 (subject to change with adition of gravity)--------------------------------------------------------------------------------
        if (!_jumping)
        {
            _nextMove += Vector2.UnitY * (_fallPoint / 5);
            _fallPoint++;
        }

        // Above and below have been swapped to ensure that
        // palette has one cycle where no vertical movement is applied after jumping

        // Decrements _jumpPoint if possible
        if (_jumpPoint > 0)
        {
            _jumpPoint--;
        }
        // If finished jumping, update var
        else
        {
            _jumping = false;
        }

        // Update the position of the bat based on the velocity.
        Position += _nextMove * MOVEMENT_SPEED;
    }

    /// Updates palette
    public void Update(GameTime gameTime)
    {
        // Changes the animation of the sprite based on which direction its going
        _sprite = _animations[_movement];

        // Update the animated sprite
        _sprite.Update(gameTime);

        // Handle any player input
        HandleInput();

        // Checks input buffer for movement
        Move();
    }

    /// Draws palette
    public void Draw()
    {
        _sprite.Draw(Core.SpriteBatch, Position);
    }

    /// Returns rectangle bounds for palette
    public Rectangle GetBounds()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, (int)_sprite.Width, (int)_sprite.Height);
    }

    /// Shifts the palette by 1 so it stays in the room bounds
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

    /// Tells palette its touching the floor
    public void Land()
    {
        _inAir = false;
        _fallPoint = 0;
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