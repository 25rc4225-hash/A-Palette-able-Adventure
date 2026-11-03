using APalette_ableAdventure.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using MonoGameTestProject;
using System;
using System.Collections.Generic;

namespace APalette_ableAdventure.Scenes;

public class addBlocks : Scene
{
    private string mapNum = "";
    public addBlocks(string Map)
    {
        mapNum = Map;
    }
    private enum GameState
    {
        Playing,
        Paused,
        Placing,
        GameOver
    }


    private static int[,] map;
    private static int[,] entMap;
    private static string[,] fullMap;
    private static string[,] addingMap;

    Sprite palette = new Sprite();
    List<Sprite> mSprites = new List<Sprite>();
    Sprite gravBlock = new Sprite();
    Sprite Button = new Sprite();
    List<Sprite> eSprites = new List<Sprite>();
    Sprite EndPortal = new Sprite();
    Sprite Water = new Sprite();

    static bool placing = true;
    static string currentBlock = "G";
    static int blockX = 0, blockY = 0;
    static int blockSelector = 0;


    // Buffer to queue inputs input by player during input polling.
    private Queue<string> _inputBuffer;

    // The maximum size of the buffer queue.
    private const int MAX_BUFFER_SIZE = 2;
    string _nextMove;


    SoundEffect errorSoundEffect;


    private static Vector2 Scale = new Vector2(3.334f, 3.334f);

    // Defines the tilemap to draw
    private static Tilemap _tilemap;

    // Defines the bounds of the room
    private Rectangle _roomBounds;

    private GameSceneUI _ui;

    private GameState _state;

    public override void Initialize()
    {
        Reset();
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
        Game1.Title();
    }

    private void Reset()
    {

    }

    private void InitializeNewGame()
    {
        // initialize the input buffer.
        _inputBuffer = new Queue<string>(MAX_BUFFER_SIZE);

        // Set the game state to playing.
        _state = GameState.Playing;
    }

    public override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file.
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");

        Layout Map = new Layout(Content, $"images/Level{mapNum}_Map.xml");
        map = Map.GetMap();
        entMap = Map.GetEntMap();
        addingMap = new string[map.GetLength(0), map.GetLength(1)];

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] != 0)
                {
                    fullMap[y, x] = map[y, x].ToString();
                }
                else
                {
                    fullMap[y, x] = entMap[y, x].ToString();
                }
            }
        }

        // Create the tilemap from the XML configuration file.
        _tilemap = Map.CreateTileMap();
        _tilemap.Scale = Scale;

        // Load the error sound effect for the placing.
        errorSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Create the sprite for the slime from the atlas.
        palette = atlas.CreateSprite("defaultPalette");
        palette.Scale = Scale;

        MonoGameLibrary.Graphics.Sprite moverR = atlas.CreateSprite("defaultRight");
        moverR.Scale = Scale;
        mSprites.Add(moverR);
        MonoGameLibrary.Graphics.Sprite moverL = atlas.CreateSprite("defaultLeft");
        moverL.Scale = Scale;
        mSprites.Add(moverL);
        MonoGameLibrary.Graphics.Sprite moverU = atlas.CreateSprite("defaultUp");
        moverU.Scale = Scale;
        mSprites.Add(moverU);
        MonoGameLibrary.Graphics.Sprite moverD = atlas.CreateSprite("defaultDown");
        moverD.Scale = Scale;
        mSprites.Add(moverD);

        gravBlock = atlas.CreateSprite("defaultBlock");
        gravBlock.Scale = Scale;

        Button = atlas.CreateSprite("defaultButton");
        Button.Scale = Scale;

        Sprite Ecks = atlas.CreateSprite("defaultEcks");
        Ecks.Scale = Scale;
        eSprites.Add(Ecks);
        AnimatedSprite Clock = atlas.CreateAnimatedSprite("defualtClock");
        Clock.Scale = Scale;
        eSprites.Add(Clock);
        AnimatedSprite antiClock = atlas.CreateAnimatedSprite("defaultAntiC");
        antiClock.Scale = Scale;
        eSprites.Add(antiClock);
        AnimatedSprite VertEqual = atlas.CreateAnimatedSprite("defaultVert");
        VertEqual.Scale = Scale;
        eSprites.Add(VertEqual);
        AnimatedSprite HorizEqual = atlas.CreateAnimatedSprite("defaultHoriz");
        HorizEqual.Scale = Scale;
        eSprites.Add(HorizEqual);

        EndPortal = atlas.CreateSprite("defaultPortal");
        EndPortal.Scale = new Vector2(5.25f, 5.25f);

        Sprite Water = atlas.CreateSprite("defaultWater");
        Water.Scale = Scale;
    }

    /// Adds movements to the input buffer
    private void HandleInput()
    {
        string potentialNextDirection = "";

        if (GameController.MoveUp())
        {
            potentialNextDirection = "U";
        }
        else if (GameController.MoveDown())
        {
            potentialNextDirection = "D";
        }
        else if (GameController.MoveLeft())
        {
            potentialNextDirection = "L";
        }
        else if (GameController.MoveRight())
        {
            potentialNextDirection = "R";
        }
        else if (GameController.SpinClock())
        {
            potentialNextDirection = "E";
        }
        else if (GameController.SpinAntiClock())
        {
            potentialNextDirection = "Q";
        }
        else if (GameController.CycleBlocks())
        {
            potentialNextDirection = "F";
        }
        else if (GameController.Action())
        {
            potentialNextDirection = "_";
        }

        // If a new direction was input, consider adding it to the buffer
        if (_inputBuffer.Count < MAX_BUFFER_SIZE)
        {
            _inputBuffer.Enqueue(potentialNextDirection);
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (true)
        {
            // Ensure the UI is always updated.
            _ui.Update(gameTime);

            // If the game is in a game over state, immediately return back
            // here.
            if (_state == GameState.GameOver)
            {
                InitializeNewGame();
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
        }


        HandleInput();
        // Get the next direction from the input buffer if one is available
        if (_inputBuffer.Count > 0)
        {
            _nextMove = _inputBuffer.Dequeue();
        }
        bool place = false;
        switch (_nextMove)
        {
            case "": break;
            case "U": blockY--; break;
            case "D": blockY++; break;
            case "L": blockX--; break;
            case "R": blockX++; break;
            case "E": RotateBlock("E"); break;
            case "Q": RotateBlock("Q"); break;
            case "F": CycleThruBlocks(); break;
            case "_": place = true; break;
        }
        if (place)
        {
            if (fullMap[blockY, blockX] == "0")
            {
                fullMap[blockY, blockX] = currentBlock;
                addingMap[blockY, blockX] = currentBlock;
            }
            else
            {
                // Play the sound effect with default settings
                errorSoundEffect.Play();
            }
        }

        if (GameController.Reset())
        {
            InitializeNewGame();
        }
    }

    static void RotateBlock(string direction)
    {
        switch (direction)
        {
            case "E":
                switch (currentBlock)
                {
                    case "R": currentBlock = "D"; break;
                    case "L": currentBlock = "U"; break;
                    case "U": currentBlock = "R"; break;
                    case "D": currentBlock = "L"; break;
                    case "H": currentBlock = "V"; break;
                    case "V": currentBlock = "H"; break;
                    case "C": currentBlock = "A"; break;
                    case "A": currentBlock = "C"; break;
                }
                break;
            case "Q":
                switch (currentBlock)
                {
                    case "R": currentBlock = "U"; break;
                    case "L": currentBlock = "D"; break;
                    case "U": currentBlock = "L"; break;
                    case "D": currentBlock = "R"; break;
                    case "H": currentBlock = "V"; break;
                    case "V": currentBlock = "H"; break;
                    case "C": currentBlock = "A"; break;
                    case "A": currentBlock = "C"; break;
                }
                break;
        }
    }

    static void CycleThruBlocks()
    {
        blockSelector++;
        blockSelector %= 6;
        switch (blockSelector)
        {
            case 0: currentBlock = "G"; break;
            case 1: currentBlock = "R"; break;
            case 2: currentBlock = "X"; break;
            case 3: currentBlock = "H"; break;
            case 4: currentBlock = "C"; break;
            case 5: currentBlock = "B"; break;
        }
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

    private void ProcessMap()
    {

    }

    public override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the tilemap
        _tilemap.Draw(Core.SpriteBatch);


        for (int y = 0; y < fullMap.GetLength(0); y++)
        {
            for (int x = 0; x < fullMap.GetLength(1); x++)
            {
                switch (fullMap[y, x])
                {

                }
            }
        }



        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();

        // Draw the UI.
        _ui.Draw();
    }
}