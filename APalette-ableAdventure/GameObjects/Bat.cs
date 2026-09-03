using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace APalette_ableAdventure.GameObjects
{
    public class Bat
    {
        private const float MOVEMENT_SPEED = 5.0f;

        // The velocity of the bat that defines the direction and how much in that
        // direction to update the bats position each update cycle.
        private Vector2 _velocity;

        // The AnimatedSprite used when drawing the bat.
        private AnimatedSprite _sprite;

        // The sound effect to play when the bat bounces off the edge of the room.
        private SoundEffect _bounceSoundEffect;

        /// Gets or Sets the position of the bat.
        public Vector2 Position { get; set; }

        /// Creates a new Bat using the specified animated sprite and sound effect.
        public Bat(AnimatedSprite sprite, SoundEffect bounceSoundEffect)
        {
            _sprite = sprite;
            _bounceSoundEffect = bounceSoundEffect;
        }

        /// Randomizes the velocity of the bat.
        public void RandomizeVelocity()
        {
            // Generate a random angle
            float angle = (float)(Random.Shared.NextDouble() * MathHelper.TwoPi);

            // Convert the angle to a direction vector
            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);
            Vector2 direction = new Vector2(x, y);

            // Multiply the direction vector by the movement speed to get the
            // final velocity
            _velocity = direction * MOVEMENT_SPEED;
        }

        /// Handles a bounce event when the bat collides with a wall or boundary.
        public void Bounce(Vector2 normal)
        {
            Vector2 newPosition = Position;

            // Adjust the position based on the normal to prevent sticking to walls.
            if (normal.X != 0)
            {
                // We are bouncing off a vertical wall (left/right).
                // Move slightly away from the wall in the direction of the normal.
                newPosition.X += normal.X * (_sprite.Width * 0.1f);
            }

            if (normal.Y != 0)
            {
                // We are bouncing off a horizontal wall (top/bottom).
                // Move slightly way from the wall in the direction of the normal.
                newPosition.Y += normal.Y * (_sprite.Height * 0.1f);
            }

            // Apply the new position
            Position = newPosition;

            // Normalize before reflecting
            normal.Normalize();

            // Apply reflection based on the normal.
            _velocity = Vector2.Reflect(_velocity, normal);

            // Play the bounce sound effect.
            Core.Audio.PlaySoundEffect(_bounceSoundEffect);
        }

        /// Returns a Circle value that represents collision bounds of the bat.
        public Circle GetBounds()
        {
            int x = (int)(Position.X + _sprite.Width * 0.5f);
            int y = (int)(Position.Y + _sprite.Height * 0.5f);
            int radius = (int)(_sprite.Width * 0.25f);

            return new Circle(x, y, radius);
        }

        /// Updates the bat.
        public void Update(GameTime gameTime)
        {
            // Update the animated sprite
            _sprite.Update(gameTime);

            // Update the position of the bat based on the velocity.
            Position += _velocity;
        }

        /// Draws the bat.
        public void Draw()
        {
            _sprite.Draw(Core.SpriteBatch, Position);
        }
    }
}
