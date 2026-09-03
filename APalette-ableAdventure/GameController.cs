using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Input;

namespace APalette_ableAdventure
{
    /// Provides a game-specific input abstraction that maps physical inputs
    /// to game actions, bridging our input system with game-specific functionality.
    public static class GameController
    {
        private static KeyboardInfo s_keyboard => Core.Input.Keyboard;

        /// Returns true if the player has triggered the "move up" action.
        public static bool MoveUp()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Up) ||
                   s_keyboard.WasKeyJustPressed(Keys.W) ||
                   s_keyboard.WasKeyJustPressed(Keys.Space);
        }

        /// Returns true if the player has triggered the "move left" action.
        public static bool MoveLeft()
        {
            return s_keyboard.IsKeyDown(Keys.Left) ||
                   s_keyboard.IsKeyDown(Keys.A);
        }

        /// Returns true if the player has triggered the "move right" action.
        public static bool MoveRight()
        {
            return s_keyboard.IsKeyDown(Keys.Right) ||
                   s_keyboard.IsKeyDown(Keys.D);
        }

        /// Returns true if the player has triggered the "pause" action.
        public static bool Pause()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Escape);
        }

        /// Returns true if the player has triggered the "action" button,
        /// typically used for menu confirmation.
        public static bool Action()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Enter);
        }
    }
}
