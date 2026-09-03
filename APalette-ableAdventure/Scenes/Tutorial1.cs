using APalette_ableAdventure.GameObjects;
using APalette_ableAdventure.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using System;
using System.Collections.Generic;

namespace APalette_ableAdventure.Scenes;

public class Tutorial1 : Scene
{
    private enum GameState
    {
        Playing,
        Paused,
        GameOver
    }

    // Reference to palette
    private Palette _palette;

    private static int[,] map;
    private static List<string[]> entities;
    private static List<Mover> Movers;
    private static List<Block> Blocks;
    private static List<Button> Buttons;
    private static List<AnimatedSprite> Effectors;

    // Defines the tilemap to draw
    private static Tilemap _tilemap;

    // Defines the bounds of the room
    private Rectangle _roomBounds;

    // Sound effect for palette landing
    private SoundEffect _landingSoundEffect;

    private GameSceneUI _ui;

    private GameState _state;

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // During the game scene, we want to disable exit on escape. Instead,
        // the escape key will be used to return back to the title screen.
        Core.ExitOnEscape = false;

        // Create the room bounds by getting the bounds of the screen then
        // using the Inflate method to "Deflate" the bounds by the width and
        // height of a tile so that the bounds only covers the inside room of
        // the dungeon tilemap.
        _roomBounds = Core.GraphicsDevice.PresentationParameters.Bounds;
        _roomBounds.Inflate(-_tilemap.TileWidth, -_tilemap.TileHeight);//aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa Currently unneeded, revisit for border

        // Create any UI elements from the root element created in previous
        // scenes.
        GumService.Default.Root.Children.Clear();

        // Initialize the user interface for the game scene.
        InitializeUI();

        // Initialize a new game to be played.
        InitializeNewGame();
    }

    private void InitializeUI()
    {
        // Clear out any previous UI element incase we came here
        // from a different scene.
        GumService.Default.Root.Children.Clear();

        // Create the game scene ui instance.
        _ui = new GameSceneUI();

        // Subscribe to the events from the game scene ui.
        _ui.ResumeButtonClick += OnResumeButtonClicked;
        _ui.RetryButtonClick += OnRetryButtonClicked;
        _ui.QuitButtonClick += OnQuitButtonClicked;
    }

    private void OnResumeButtonClicked(object sender, EventArgs args)
    {
        // Change the game state back to playing.
        _state = GameState.Playing;
    }

    private void OnRetryButtonClicked(object sender, EventArgs args)
    {
        // Player has chosen to retry, so initialize a new game.
        InitializeNewGame();
    }

    private void OnQuitButtonClicked(object sender, EventArgs args)
    {
        // Player has chosen to quit, so return back to the title scene.
        Core.ChangeScene(new TitleScene());
    }

    private void InitializeNewGame()
    {
        // Initialize the palette
        _palette.Initialize();

        // Initialize the mover
        _mover.Initialize(Vector2.UnitY * (Core.GraphicsDevice.PresentationParameters.BackBufferHeight / 2));  //(subject to change with map creation)--------------------------------------------------------------------------------

        // Set the game state to playing.
        _state = GameState.Playing;
    }

    public override void LoadContent()  //(subject to change with map creation)--------------------------------------------------------------------------------
    {
        // Create the texture atlas from the XML configuration file.
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");

        Layout Map = new Layout(Content, "images/Tutorial1_Map.xml");
        map = Map.GetMap();

        // Create the tilemap from the XML configuration file.
        _tilemap = Map.CreateTileMap();
        _tilemap.Scale = new Vector2(3.334f, 3.334f);

        // Load the bounce sound effect for the bat.
        SoundEffect jumpSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Create the animated sprite for the slime from the atlas.
        AnimatedSprite paletteRight = atlas.CreateAnimatedSprite("palette-animation");
        paletteRight.Scale = new Vector2(3.5f, 3.5f);
        AnimatedSprite paletteLeft = atlas.CreateAnimatedSprite("palette-animation");
        paletteLeft.Scale = new Vector2(3.5f, 3.5f);
        AnimatedSprite paletteNull = atlas.CreateAnimatedSprite("palette-animation");
        paletteNull.Scale = new Vector2(3.5f, 3.5f);

        MonoGameLibrary.Graphics.Sprite moverR = atlas.CreateSprite("MoverR");
        moverR.Scale = new Vector2(3.5f, 3.5f);
        MonoGameLibrary.Graphics.Sprite moverL = atlas.CreateSprite("MoverL");
        moverL.Scale = new Vector2(3.5f, 3.5f);
        MonoGameLibrary.Graphics.Sprite moverU = atlas.CreateSprite("MoverU");
        moverU.Scale = new Vector2(3.5f, 3.5f);
        MonoGameLibrary.Graphics.Sprite moverD = atlas.CreateSprite("MoverD");
        moverD.Scale = new Vector2(3.5f, 3.5f);

        MonoGameLibrary.Graphics.Sprite gravBlock = atlas.CreateSprite("gravBlock");
        gravBlock.Scale = new Vector2(3.5f, 3.5f);

        AnimatedSprite Button = atlas.CreateAnimatedSprite("Button");
        Button.Scale = new Vector2(3.5f, 3.5f);

        entities = Map.GetEntites();

        foreach (string[] entity in entities)
        {
            if (entity[0] == "P")
            {
                // Create the palette
                _palette = new Palette(paletteNull, paletteRight, paletteLeft, jumpSoundEffect, int.Parse(entity[1]), int.Parse(entity[2]));
            }
            else if ("LRUD".Contains(entity[0]))
            {
                Movers.Add(new Mover(entity[0], moverR, moverL, moverU, moverD, int.Parse(entity[1]), int.Parse(entity[2])));
            }
            else if (entity[0] == "G")
            {
                Blocks.Add(new Block(gravBlock, int.Parse(entity[1]), int.Parse(entity[2])));
            }
            else if (entity[0] == "B")
            {
                Buttons.Add(new Button(Button, int.Parse(entity[1]), int.Parse(entity[2])));
            }
            else if ("XR=".Contains(entity[0]))
            {
                Effectors.Add(new Effector(entity[0],)
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        // Ensure the UI is always updated.
        _ui.Update(gameTime);

        // If the game is in a game over state, immediately return back
        // here.
        if (_state == GameState.GameOver)
        {
            return;
        }

        // If the pause button is pressed, toggle the pause state.
        if (GameController.Pause())
        {
            TogglePause();
        }

        // At this point, if the game is paused, just return back early.
        if (_state == GameState.Paused)
        {
            return;
        }

        // Update the palette
        _palette.Update(gameTime);

        _mover.Update(gameTime);

        // Perform collision checks.
        CollisionChecks();
    }

    private void CollisionChecks()
    {
        // Capture the current bounds of the palette
        Rectangle paletteBounds = _palette.GetBounds();

        // Capture the current bounds of the mover.
        Rectangle moverBounds = _mover.GetBounds();

        List<Rectangle> mapBlocks = MapCollision(); // ----------------------------------------------------------------------------------------------------------

        foreach (Rectangle rect in mapBlocks)
        {
            if (paletteBounds.Intersects(rect))
            {
                // Find the distance from the edge of the palette to rect
                float distanceLeft = Math.Abs(rect.Left - paletteBounds.Right);
                float distanceRight = Math.Abs(rect.Right - paletteBounds.Left);
                float distanceTop = Math.Abs(rect.Top - paletteBounds.Bottom);
                float distanceBottom = Math.Abs(rect.Bottom - paletteBounds.Top);

                // Determine which rect edge is the closest.
                float minDistance = Math.Min(Math.Min(distanceLeft, distanceRight), Math.Min(distanceTop, distanceBottom));

                if (minDistance == distanceLeft)
                {
                    _palette.Shift("left", (int)minDistance);
                }
                if (minDistance == distanceRight)
                {
                    _palette.Shift("right", (int)minDistance);
                }
                if (minDistance == distanceTop)
                {
                    _palette.Shift("up", (int)minDistance);
                    // Therefore Palette is on terrain
                    _palette.Land();
                }
                if (minDistance == distanceBottom)
                {
                    _palette.Shift("down", (int)minDistance);
                }
                // Checks if palette is standing on terrain
                //if (paletteBounds.Bottom == rect.Top && paletteBounds.Right > rect.Left && paletteBounds.Left < rect.Right)
                //{
                //    _palette.Land();
                //}
            }
            if (moverBounds.Intersects(rect))
            {
                // Find the distance from the edge of the paleete to mover
                float distanceLeft = Math.Abs(rect.Left - moverBounds.Right);
                float distanceRight = Math.Abs(rect.Right - moverBounds.Left);
                float distanceTop = Math.Abs(rect.Top - moverBounds.Bottom);
                float distanceBottom = Math.Abs(rect.Bottom - moverBounds.Top);

                // Determine which mover edge is the closest.
                float minDistance = Math.Min(Math.Min(distanceLeft, distanceRight), Math.Min(distanceTop, distanceBottom));

                if (minDistance == distanceLeft)
                {
                    for (int i = 0; i < minDistance; i++)
                    {
                        _mover.Shift("left");
                    }
                }
                else if (minDistance == distanceRight)
                {
                    for (int i = 0; i < minDistance; i++)
                    {
                        _mover.Shift("right");
                    }
                }
                else if (minDistance == distanceTop)
                {
                    for (int i = 0; i < minDistance; i++)
                    {
                        _mover.Shift("up");
                    }
                }
                else if (minDistance == distanceBottom)
                {
                    for (int i = 0; i < minDistance; i++)
                    {
                        _mover.Shift("down");
                    }
                }
            }
        }

        if (true)
        {
            // Checks if palette is above map
            if (paletteBounds.Top < _roomBounds.Top)
            {
                // Moves palette back into game box
                _palette.Shift("down", (_roomBounds.Top - paletteBounds.Top));
            }
            // Checks if palette is below map
            if (paletteBounds.Bottom > _roomBounds.Bottom)
            {
                // Moves palette back into game box
                _palette.Shift("up", paletteBounds.Bottom - _roomBounds.Bottom);
            }
            // Checks if palette is left of map
            if (paletteBounds.Left < _roomBounds.Left)
            {
                // Moves palette back into game box
                _palette.Shift("right", _roomBounds.Left - paletteBounds.Left);
            }
            // Checks if palette is right of map
            if (paletteBounds.Right > _roomBounds.Right)
            {
                // Moves palette back into game box
                _palette.Shift("left", paletteBounds.Right - _roomBounds.Right);
            }

            // Checks if palette is touching bottom of map
            if (paletteBounds.Bottom == _roomBounds.Bottom)
            {
                _palette.Land();
            }


            // Checks if mover is above map
            if (moverBounds.Top < _roomBounds.Top)
            {
                for (int i = 0; i < _roomBounds.Top - moverBounds.Top; i++)
                {
                    // Moves mover back into game box
                    _mover.Shift("down");
                }
            }
            // Checks if mover is below map
            if (moverBounds.Bottom > _roomBounds.Bottom)
            {
                for (int i = 0; i < moverBounds.Bottom - _roomBounds.Bottom; i++)
                {
                    // Moves mover back into game box
                    _mover.Shift("up");
                }
            }
            // Checks if mover is left of map
            if (moverBounds.Left < _roomBounds.Left)
            {
                for (int i = 0; i < _roomBounds.Left - moverBounds.Left; i++)
                {
                    // Moves mover back into game box
                    _mover.Shift("right");
                }
            }
            // Checks if mover is right of map
            if (moverBounds.Right > _roomBounds.Right)
            {
                for (int i = 0; i < moverBounds.Right - _roomBounds.Right; i++)
                {
                    // Moves mover back into game box
                    _mover.Shift("left");
                }
            }
        }

        if (paletteBounds.Intersects(moverBounds))
        {
            // Find the distance from the edge of the paleete to mover
            float distanceLeft = Math.Abs(moverBounds.Left - paletteBounds.Right);
            float distanceRight = Math.Abs(moverBounds.Right - paletteBounds.Left);
            float distanceTop = Math.Abs(moverBounds.Top - paletteBounds.Bottom);
            float distanceBottom = Math.Abs(moverBounds.Bottom - paletteBounds.Top);

            // Determine which mover edge is the closest.
            float minDistance = Math.Min(Math.Min(distanceLeft, distanceRight), Math.Min(distanceTop, distanceBottom));

            if (minDistance == distanceLeft)
            {
                _palette.Shift("left", (int)minDistance);
            }
            if (minDistance == distanceRight)
            {
                _palette.Shift("right", (int)minDistance);
            }
            if (minDistance == distanceTop)
            {
                _palette.Shift("up", (int)minDistance);
            }
            if (minDistance == distanceBottom)
            {
                _palette.Shift("down", (int)minDistance);
            }
        }
        // Checks if palette is standing on mover
        if (paletteBounds.Bottom == moverBounds.Top && paletteBounds.Right > moverBounds.Left && paletteBounds.Left < moverBounds.Right)
        {
            _palette.Land();
        }

        return;
    }

    static List<Rectangle> MapCollision()
    {
        int groups = 0;

        /// Each pass through the system
        int[,] flatpass = new int[map.GetLength(0), map.GetLength(1)];
        int[,] sizepass = new int[map.GetLength(0), map.GetLength(1)];
        int[,] groupingpass = new int[map.GetLength(0), map.GetLength(1)];

        /// Iterates through the system horizontally, grouping the hits in each row
        /// and assigns each a size and index value while labelling the groups
        for (int y = 0; y < map.GetLength(0); y++)
        {
            int ones = 0;
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1)
                {
                    ones++;
                }
                else
                {
                    groups++;
                    for (int z = 0; z < ones; z++)
                    {
                        flatpass[y, x - 1 - z] = /*size*/ones * 100 + /*index*/z + 1; ///Example:  31    3 (size of the group its in), 1 (decreasing index, so last hit in the group)
                        groupingpass[y, x - 1 - z] = groups; //Labelling
                    }
                    ones = 0;
                    /// Assigns each non-hit as 00 to follow the hit pattern
                    flatpass[y, x] = 0000;
                }
            }
            /// If the count reaches the end of the row without a non-hit,
            /// it automatically finishes the group
            if (ones != 0)
            {
                groups++;
                for (int z = 0; z < ones; z++)
                {
                    flatpass[y, map.GetLength(1) - 1 - z] = ones * 100 + z + 1;
                    groupingpass[y, map.GetLength(1) - 1 - z] = groups;
                }
            }
        }

        /// Iterates vertically through the system to combine groups
        for (int x = 0; x < map.GetLength(1); x++)
        {
            int same = 1;
            /// Pre sets the check value as the top input in the column
            int value = flatpass[0, x];
            for (int y = 0; y < map.GetLength(0) - 1; y++)
            {
                /// If the one below has the same size group and is at the same index, they are grouped
                /// -------------
                if (value == flatpass[y + 1, x])
                {
                    value = flatpass[y + 1, x];
                    same++;
                }
                else
                {
                    for (int z = 0; z < same; z++)
                    {
                        /// Counts number of same rows and multiplies it by the group size to find the total in the combined group
                        sizepass[y - z, x] = same * (value / 100);
                        /// Assigns the group number to the upper row in the new group           Example:   2 2 2 2             2 2 2 2
                        ///                                                                                 3 3 3 3     -->     2 2 2 2
                        ///                                                                                 5 5 5 5             2 2 2 2
                        groupingpass[y - z, x] = groupingpass[y - same + 1, x];
                    }
                    same = 1;
                    value = flatpass[y + 1, x];
                }
                /// -------------
            }
            /// If the count reaches the end of the column without a non-grouping,
            /// it automatically finishes the group
            for (int z = 0; z < same; z++)
            {
                sizepass[map.GetLength(0) - z - 1, x] = same * (value / 100);
                groupingpass[map.GetLength(0) - z - 1, x] = groupingpass[map.GetLength(0) - same, x];
            }
        }

        List<int> doneGroups = new List<int>();
        List<Rectangle> groupRects = new List<Rectangle>();

        for (int y = 0; y < groupingpass.GetLength(0); y++)
        {
            for (int x = 0; x < groupingpass.GetLength(1); x++)
            {
                if (groupingpass[y, x] != 0)
                {
                    if (!doneGroups.Contains(groupingpass[y, x]))
                    {
                        // Prevents rectangles being made for the same group multiple times
                        doneGroups.Add(groupingpass[y, x]);
                        // Creates a new collision rectangle with
                        //                          (top left pos) x and y,                                      distance from top left to top right, and distance from top left to bottom left
                        groupRects.Add(new Rectangle(x * (int)_tilemap.TileWidth, y * (int)_tilemap.TileHeight, flatpass[y, x] / 100 * (int)_tilemap.TileHeight, (sizepass[y, x] / (flatpass[y, x] / 100)) * (int)_tilemap.TileHeight));
                    }
                }
            }
        }

        return groupRects;
    }

    private void CreateEntities()
    {

    }

    private void TogglePause()
    {
        if (_state == GameState.Paused)
        {
            // We're now unpausing the game, so hide the pause panel.
            _ui.HidePausePanel();

            // And set the state back to playing.
            _state = GameState.Playing;
        }
        else
        {
            // We're now pausing the game, so show the pause panel.
            _ui.ShowPausePanel();

            // And set the state to paused.
            _state = GameState.Paused;
        }
    }

    private void GameOver()
    {
        // Show the game over panel.
        _ui.ShowGameOverPanel();

        // Set the game state to game over.
        _state = GameState.GameOver;
    }

    public override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the tilemap
        _tilemap.Draw(Core.SpriteBatch);

        // Draw the palette
        _palette.Draw();

        // Draw the mover
        _mover.Draw();

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();

        // Draw the UI.
        _ui.Draw();
    }
}