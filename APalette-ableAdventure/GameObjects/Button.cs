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
    private Sprite _sprite;
    private Sprite _Off;
    private Sprite _On;

    private Microsoft.Xna.Framework.Vector2 Position { get; set; }

    private bool Pushed = false;
    
    public Button(Sprite Off, Sprite On, int X, int Y)
    {
        Position = new Microsoft.Xna.Framework.Vector2(X, Y);
        _Off = Off;
        _On = On;
        _sprite = Off;
    }

    public void Initialize()
    {
        Pushed = false;
        _sprite = _Off;
    }

    public void Push(GameTime gametime)
    {
        Pushed = true;
        _sprite = _On;
        Position += System.Numerics.Vector2.UnitY * 14;
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
    public Rectangle GetBounds()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, (int)_sprite.Width, (int)_sprite.Height);
    }
}
