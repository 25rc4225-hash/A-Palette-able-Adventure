using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace APalette_ableAdventure.GameObjects;

public class Block
{
    // The _sprite that is being modified
    private Sprite _sprite;

    private int gravity = 0;

    // String for storing current movement direction
    private Vector2 _direction = Vector2.UnitY;

    /// Gets or Sets position of block
    public Vector2 Position { get; set; }

    private int fallCount = 0;

    public Block(Sprite sprite, int X, int Y)
    {
        _sprite = sprite;
        Position = new Vector2(X, Y);
    }

    public void changeGrav(string way)
    {
        if (way == "clock")
        {
            gravity++;
        }
        else
        {
            gravity--;
        }
        gravity = (gravity + 4) % 4;
        switch (gravity)
        {
            case 0: _direction = Vector2.UnitY; break;
            case 1: _direction = -Vector2.UnitX; break;
            case 2: _direction = -Vector2.UnitY; break;
            case 3: _direction = Vector2.UnitX; break;
        }
        fallCount = 0;
    }
    public void Update()
    {
        Position += -_direction * fallCount;
        fallCount++;
    }
    public void Land()
    {
        fallCount = 0;
    }
}
