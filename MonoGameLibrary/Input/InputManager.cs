using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

public class InputManager
{
    /// Gets the state information of keyboard input.
    public KeyboardInfo Keyboard { get; private set; }

    /// Gets the state information of mouse input.
    public MouseInfo Mouse { get; private set; }

    /// Creates a new InputManager.
    public InputManager()
    {
        Keyboard = new KeyboardInfo();
        Mouse = new MouseInfo();
    }

    /// Updates the state information for the keyboard, mouse, and gamepad inputs.
    public void Update(GameTime gameTime)
    {
        Keyboard.Update();
        Mouse.Update();
    }
}