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
    public addBlocks(string Map, List<string[]> blocksToAdd)
    {
        mapNum = Map;
        addedBlocks = blocksToAdd;
    }
    private enum GameState
    {
        Playing,
        Paused,
        Placing,
        GameOver
    }


    private static int[,] map;
    private static string[,] entMap;
    private static string[,] fullMap;
    private static List<string[]> addedBlocks;
    private static string denyList;


    Sprite Palette = new Sprite();
    List<Sprite> mSprites = new List<Sprite>();
    Sprite gravBlock = new Sprite();
    Sprite Button = new Sprite();
    List<Sprite> eSprites = new List<Sprite>();
    Sprite EndPortal = new Sprite();
    Sprite Water = new Sprite();
    Sprite Terrain = new Sprite();
    Sprite Background = new Sprite();
    Sprite BuildArea = new Sprite();

    static string currentBlock = "";
    static int blockX = 1, blockY = 1;
    static int blockSelector =-1;
    static int blockNum;


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
        entMap = Map.GetEntities();
        fullMap = new string[map.GetLength(0), map.GetLength(1)];
        denyList = Map.getDenyList();
        blockNum = Map.getblockCount();
        CycleThruBlocks();
        foreach (string[] block in addedBlocks)
        {
            fullMap[(int)(float.Parse(block[2]) / _tilemap.TileHeight), (int)(float.Parse(block[1]) / _tilemap.TileWidth)] = block[0];
        }

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                fullMap[y, x] = entMap[y, x];
                if (fullMap[y, x] == "0")
                {
                    fullMap[y, x] = map[y, x].ToString();
                }
            }
        }

        // Create the tilemap from the XML configuration file.
        _tilemap = Map.CreateTileMap();
        _tilemap.Scale = Scale;

        // Load the error sound effect for the placing.
        errorSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Create the sprite for the slime from the atlas.
        Palette = atlas.CreateSprite("defaultPalette");
        Palette.Scale = Scale;

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
        Sprite Clock = atlas.CreateSprite("defaultClock");
        Clock.Scale = Scale;
        eSprites.Add(Clock);
        Sprite antiClock = atlas.CreateSprite("defaultAntiC");
        antiClock.Scale = Scale;
        eSprites.Add(antiClock);
        Sprite VertEqual = atlas.CreateSprite("defaultVert");
        VertEqual.Scale = Scale;
        eSprites.Add(VertEqual);
        Sprite HorizEqual = atlas.CreateSprite("defaultHoriz");
        HorizEqual.Scale = Scale;
        eSprites.Add(HorizEqual);

        EndPortal = atlas.CreateSprite("defaultPortal");
        EndPortal.Scale = Scale;

        Water = atlas.CreateSprite("defaultWater");
        Water.Scale = Scale;

        Terrain = atlas.CreateSprite("defaultGround");
        Terrain.Scale = Scale;

        Background = atlas.CreateSprite("BackGround");
        Background.Scale = Scale;

        BuildArea = atlas.CreateSprite("BuildArea");
        BuildArea.Scale = Scale;
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
        else if (GameController.ShiftLeft())
        {
            potentialNextDirection = "L";
        }
        else if (GameController.ShiftRight())
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
        else if (GameController.LeftClick() || GameController.Space())
        {
            potentialNextDirection = "click";
        }
        else if (GameController.wasMoved())
        {
            potentialNextDirection = "moved";
        }
        else if (GameController.Reset())
        {
            potentialNextDirection = "undo";
        }

        // If a new direction was input, consider adding it to the buffer
        if (_inputBuffer.Count < MAX_BUFFER_SIZE)
        {
            _inputBuffer.Enqueue(potentialNextDirection);
        }
    }

    public override void Update(GameTime gameTime)
    {
        // Ensure the UI is always updated.
        _ui.Update(gameTime);

        if (GameController.Skip())
        {
            Game1.NextLevel(false);
        }
        if (GameController.goBack())
        {
            Game1.PreviousLevel();
        }

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


        HandleInput();

        // Get the next direction from the input buffer if one is available
        if (_inputBuffer.Count > 0)
        {
            _nextMove = _inputBuffer.Dequeue();
        }
        bool place = false;
        bool done = false;
        bool moved = false;
        bool mouseDelta = false;
        bool undo = false;
        switch (_nextMove)
        {
            case "": break;
            case "U": blockY--; moved = true; break;
            case "D": blockY++; moved = true; break;
            case "L": blockX--; moved = true; break;
            case "R": blockX++; moved = true; break;
            case "E": RotateBlock("E"); break;
            case "Q": RotateBlock("Q"); break;
            case "F": CycleThruBlocks(); break;
            case "_": done = true; break;
            case "click": place = true; break;
            case "moved": mouseDelta = true; break;
            case "undo": undo = true; break;
        }
        if (done)
        {
            Core.ChangeScene(new Levels(mapNum, addedBlocks));
        }
        if (mouseDelta)
        {
            blockX = (int)Math.Floor(GameController.getPos().X / _tilemap.TileWidth);
            blockY = (int)Math.Floor(GameController.getPos().Y / _tilemap.TileHeight);
        }
        if (blockY < 0)
        {
            blockY = 0;
        }
        else if (blockY > fullMap.GetLength(0) - 1)
        {
            blockY = fullMap.GetLength(0) - 1;
        }
        if (blockX < 0)
        {
            blockX = 0;
        }
        if (blockX > fullMap.GetLength(1) - 1)
        {
            blockX = fullMap.GetLength(1) - 1;
        }
        float screenX = blockX * _tilemap.TileWidth + _tilemap.TileWidth / 2;
        float screenY = blockY * _tilemap.TileHeight + _tilemap.TileHeight / 2;
        if (moved)
        {
            GameController.setPos(screenX, screenY);
        }

        if (place)
        {
            if (fullMap[blockY, blockX] == "2" && addedBlocks.Count < blockNum)
            {
                addedBlocks.Add(new string[] { currentBlock, (blockX * _tilemap.TileWidth).ToString(), (blockY * _tilemap.TileHeight).ToString() });
                fullMap[blockY, blockX] = currentBlock;
            }
            else
            {
                // Play the sound effect with default settings
                errorSoundEffect.Play();
            }
        }
        if (undo)
        {
            if (addedBlocks.Count != 0)
            {
                string[] removeBlock = addedBlocks[addedBlocks.Count - 1];
                addedBlocks.Remove(removeBlock);
                fullMap[(int)(float.Parse(removeBlock[2]) / _tilemap.TileHeight), (int)(float.Parse(removeBlock[1]) / _tilemap.TileWidth)] = "2";
            }
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
            case 0: 
                if (denyList.Contains("G"))
                {
                    CycleThruBlocks();
                }
                else
                {
                    currentBlock = "G";
                }
                break;
            case 1:
                if (denyList.Contains("M"))
                {
                    CycleThruBlocks();
                }
                else
                {
                    currentBlock = "R";
                }
                break;
            case 2:
                if (denyList.Contains("X"))
                {
                    CycleThruBlocks();
                }
                else
                {
                    currentBlock = "X";
                }
                break;
            case 3:
                if (denyList.Contains("="))
                {
                    CycleThruBlocks();
                }
                else
                {
                    currentBlock = "H";
                }
                break;
            case 4:
                if (denyList.Contains("R"))
                {
                    CycleThruBlocks();
                }
                else
                {
                    currentBlock = "C";
                }
                break;
            case 5:
                if (denyList.Contains("B"))
                {
                    CycleThruBlocks();
                }
                else
                {
                    currentBlock = "B";
                }
                break;
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

    public override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);


        for (int y = 0; y < fullMap.GetLength(0); y++)
        {
            for (int x = 0; x < fullMap.GetLength(1); x++)
            {
                float X = x * _tilemap.TileWidth;
                float Y = y * _tilemap.TileHeight;
                Vector2 Pos = new Vector2(X, Y);
                switch (fullMap[y, x])
                {
                    case "0": Background.Draw(Core.SpriteBatch, Pos); break;
                    case "1": Terrain.Draw(Core.SpriteBatch, Pos); break;
                    case "2": BuildArea.Draw(Core.SpriteBatch, Pos); break;
                    case "P": Palette.Draw(Core.SpriteBatch, Pos); break;
                    case "R": mSprites[0].Draw(Core.SpriteBatch, Pos); break;
                    case "L": mSprites[1].Draw(Core.SpriteBatch, Pos); break;
                    case "U": mSprites[2].Draw(Core.SpriteBatch, Pos); break;
                    case "D": mSprites[3].Draw(Core.SpriteBatch, Pos); break;
                    case "G": gravBlock.Draw(Core.SpriteBatch, Pos); break;
                    case "B": Button.Draw(Core.SpriteBatch, Pos); break;
                    case "X": eSprites[0].Draw(Core.SpriteBatch, Pos); break;
                    case "C": eSprites[1].Draw(Core.SpriteBatch, Pos); break;
                    case "A": eSprites[2].Draw(Core.SpriteBatch, Pos); break;
                    case "V": eSprites[3].Draw(Core.SpriteBatch, Pos); break;
                    case "H": eSprites[4].Draw(Core.SpriteBatch, Pos); break;
                    case "E": EndPortal.Draw(Core.SpriteBatch, Pos); break;
                    case "W": Water.Draw(Core.SpriteBatch, Pos); break;
                }
            }
        }
        float screenX = blockX * _tilemap.TileWidth;
        float screenY = blockY * _tilemap.TileHeight;
        switch (currentBlock)
        {
            case "R": mSprites[0].Draw(Core.SpriteBatch, new Vector2(screenX, screenY)); break;
            case "L": mSprites[1].Draw(Core.SpriteBatch, new Vector2(screenX, screenY)); break;
            case "U": mSprites[2].Draw(Core.SpriteBatch, new Vector2(screenX, screenY)); break;
            case "D": mSprites[3].Draw(Core.SpriteBatch, new Vector2(screenX, screenY)); break;
            case "G": gravBlock.Draw(Core.SpriteBatch, new Vector2(screenX, screenY)); break;
            case "B": Button.Draw(Core.SpriteBatch, new Vector2(screenX, screenY)); break;
            case "X": eSprites[0].Draw(Core.SpriteBatch, new Vector2(screenX, screenY)); break;
            case "C": eSprites[1].Draw(Core.SpriteBatch, new Vector2(screenX, screenY)); break;
            case "A": eSprites[2].Draw(Core.SpriteBatch, new Vector2(screenX, screenY)); break;
            case "V": eSprites[3].Draw(Core.SpriteBatch, new Vector2(screenX, screenY)); break;
            case "H": eSprites[4].Draw(Core.SpriteBatch, new Vector2(screenX, screenY)); break;
        }
        foreach (string[] block in addedBlocks)
        {
            switch (block[0])
            {
                case "R": mSprites[0].Draw(Core.SpriteBatch, new Vector2(float.Parse(block[1]), float.Parse(block[2]))); break;
                case "L": mSprites[1].Draw(Core.SpriteBatch, new Vector2(float.Parse(block[1]), float.Parse(block[2]))); break;
                case "U": mSprites[2].Draw(Core.SpriteBatch, new Vector2(float.Parse(block[1]), float.Parse(block[2]))); break;
                case "D": mSprites[3].Draw(Core.SpriteBatch, new Vector2(float.Parse(block[1]), float.Parse(block[2]))); break;
                case "G": gravBlock.Draw(Core.SpriteBatch, new Vector2(float.Parse(block[1]), float.Parse(block[2]))); break;
                case "B": Button.Draw(Core.SpriteBatch, new Vector2(float.Parse(block[1]), float.Parse(block[2]))); break;
                case "X": eSprites[0].Draw(Core.SpriteBatch, new Vector2(float.Parse(block[1]), float.Parse(block[2]))); break;
                case "C": eSprites[1].Draw(Core.SpriteBatch, new Vector2(float.Parse(block[1]), float.Parse(block[2]))); break;
                case "A": eSprites[2].Draw(Core.SpriteBatch, new Vector2(float.Parse(block[1]), float.Parse(block[2]))); break;
                case "V": eSprites[3].Draw(Core.SpriteBatch, new Vector2(float.Parse(block[1]), float.Parse(block[2]))); break;
                case "H": eSprites[4].Draw(Core.SpriteBatch, new Vector2(float.Parse(block[1]), float.Parse(block[2]))); break;
            }
        }


        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();

        // Draw the UI.
        _ui.Draw();
    }
}