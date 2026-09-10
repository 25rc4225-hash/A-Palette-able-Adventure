using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace APalette_ableAdventure.GameObjects;

public class Button
{
    // The _sprite that is being modified
    private AnimatedSprite _sprite;

    private Microsoft.Xna.Framework.Vector2 Position { get; set; }

    private bool Pushed = false;
    
    public Button(AnimatedSprite sprite, int X, int Y)
    {
        Position = new Microsoft.Xna.Framework.Vector2(X, Y);
        _sprite = sprite;
    }

    public void Push(GameTime gametime)
    {
        Pushed = true;
        _sprite.Update(gametime);
    }

    public bool isPushed()
    {
        return Pushed;
    }

    /// Draws button
    public void Draw()
    {
        _sprite.Draw(Core.SpriteBatch, Position);
    }
}
