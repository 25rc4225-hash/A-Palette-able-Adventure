using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace APalette_ableAdventure.GameObjects;

public class Block : Mover
{
    private int gravity = 0;

    // String for storing current movement direction
    private Vector2 _direction = Vector2.UnitY;

    private int fallCount = 0;

    public Block(Sprite sprite, float X, float Y)
    {
        _sprite = sprite;
        base.startSprite = _sprite;
        Position = new Vector2(X, Y);
        startPosition = Position;
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
    public override void Update(GameTime gameTime)
    {
        Position += _direction * fallCount;
        fallCount++;
    }
    public override void Land()
    {
        fallCount = 0;
    }
    public override string getType()
    {
        return "B";
    }
}
