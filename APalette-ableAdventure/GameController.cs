using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Input;
using System.Collections.Generic;

namespace APalette_ableAdventure
{
    /// Provides a game-specific input abstraction that maps physical inputs
    /// to game actions, bridging our input system with game-specific functionality.
    public static class GameController
    {
        private static KeyboardInfo s_keyboard => Core.Input.Keyboard;
        private static MouseInfo s_mouse => Core.Input.Mouse;

        /// Returns true if the player has triggered the "move up" action.
        public static bool MoveUp()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Up) ||
                   s_keyboard.WasKeyJustPressed(Keys.W);
        }
        /// Returns true if the player has triggered the "space" action.
        public static bool Space()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Space);
        }
        /// Returns true if the player has triggered the "move down" action.
        public static bool MoveDown()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Down) ||
                   s_keyboard.WasKeyJustPressed(Keys.S);
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

        /// Returns true if the player has triggered the "shift left" action.
        public static bool ShiftLeft()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Left) ||
                   s_keyboard.WasKeyJustPressed(Keys.A);
        }

        /// Returns true if the player has triggered the "move right" action.
        public static bool ShiftRight()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Right) ||
                   s_keyboard.WasKeyJustPressed(Keys.D);
        }


        /// Returns true if the player has triggered the "rotate clockwise" action.
        public static bool SpinClock()
        {
            return s_keyboard.WasKeyJustPressed(Keys.E);
        }

        /// Returns true if the player has triggered the "rotate anticlockwise" action.
        public static bool SpinAntiClock()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Q);
        }

        /// Returns true if the player has triggered the "CycleBlocks" action.
        public static bool CycleBlocks()
        {
            return s_keyboard.WasKeyJustPressed(Keys.F);
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

        public static bool Reset()
        {
            return s_keyboard.WasKeyJustPressed(Keys.R);
        }

        public static bool LeftClick()
        {
            return s_mouse.WasButtonJustPressed(MouseButton.Left);
        }

        public static bool Skip()
        {
            return s_mouse.WasButtonJustPressed(MouseButton.XButton2);
        }
        public static bool goBack()
        {
            return s_mouse.WasButtonJustPressed(MouseButton.XButton1);
        }
        public static bool Build()
        {
            return s_keyboard.WasKeyJustPressed(Keys.B);
        }


        public static void setPos(float X, float Y)
        {
            s_mouse.SetPosition(X, Y);
        }
        public static bool wasMoved()
        {
            return s_mouse.WasMoved;
        }
        public static Vector2 getPos()
        {
            return s_mouse.GetPosition();
        }
    }
}
