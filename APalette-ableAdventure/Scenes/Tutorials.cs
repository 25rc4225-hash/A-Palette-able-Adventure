using APalette_ableAdventure.GameObjects;
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

public class Tutorials : Scene
{
    private string mapName = "";
    public Tutorials(string Map)
    {
        mapName = Map;
    }
    private enum GameState
    {
        Playing,
        Paused,
        GameOver
    }

    // Reference to palette
    private Palette _palette;

    private static int[,] map;
    private static int[,] entMap;
    private static List<string[]> entities;
    private List<AnimatedSprite> pAnimations = new List<AnimatedSprite>();
    private static List<Mover> Movings = new List<Mover>();
    private static List<Mover> newMovers = new List<Mover>();
    private List<Sprite> mSprites = new List<Sprite>();
    private Sprite gravBlock = new Sprite();
    private static List<GameObjects.Button> Buttons = new List<GameObjects.Button>();
    private static List<Effector> Effectors = new List<Effector>();
    private static Portal Portal;
    private static List<Water> Waters = new List<Water>();

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
        entities = new List<string[]>();
        Movings = new List<Mover>();
        Buttons = new List<GameObjects.Button>();
        Effectors = new List<Effector>();
        Waters = new List<Water>();
    }

    private void InitializeNewGame()
    {
        // Initialize the palette
        _palette.Initialize();

        foreach (var mover in Movings)
        {
            mover.unHide();
            mover.Initialize();
            mover.totalMoveReset();
        }
        foreach (var mover in newMovers)
        {
            Movings.Remove(mover);
        }
        foreach (GameObjects.Button button in Buttons)
        {
            button.Initialize();
        }
        foreach (Water water in Waters)
        {
            water.Initialize();
        }
        foreach (Effector effector in Effectors)
        {
            effector.unHide();
        }

        // Set the game state to playing.
        _state = GameState.Playing;
    }

    public override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file.
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");

        Layout Map = new Layout(Content, $"images/{mapName}_Map.xml");
        map = Map.GetMap();
        entMap = Map.GetEntMap();

        // Create the tilemap from the XML configuration file.
        _tilemap = Map.CreateTileMap();
        _tilemap.Scale = Scale;

        // Load the bounce sound effect for the bat.
        SoundEffect jumpSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // ---------------------------------------------------------------------------------
        //// Loading a SoundEffect using the content pipeline
        //SoundEffect voiceLine = Content.Load<SoundEffect>($"{mapName}VoiceLine");

        //// Play the sound effect with default settings
        //voiceLine.Play();
        // ---------------------------------------------------------------------------------

        // Create the animated sprite for the slime from the atlas.
        AnimatedSprite paletteRNull = atlas.CreateAnimatedSprite("rNpalette-animation");
        paletteRNull.Scale = Scale;
        pAnimations.Add(paletteRNull);
        AnimatedSprite paletteLNull = atlas.CreateAnimatedSprite("lNpalette-animation");
        paletteLNull.Scale = Scale;
        pAnimations.Add(paletteLNull);
        AnimatedSprite paletteRight = atlas.CreateAnimatedSprite("Rpalette-animation");
        paletteRight.Scale = Scale;
        pAnimations.Add(paletteRight);
        AnimatedSprite paletteLeft = atlas.CreateAnimatedSprite("Lpalette-animation");
        paletteLeft.Scale = Scale;
        pAnimations.Add(paletteLeft);

        MonoGameLibrary.Graphics.Sprite moverR = atlas.CreateSprite("MoverR");
        moverR.Scale = Scale;
        mSprites.Add(moverR);
        MonoGameLibrary.Graphics.Sprite moverL = atlas.CreateSprite("MoverL");
        moverL.Scale = Scale;
        mSprites.Add(moverL);
        MonoGameLibrary.Graphics.Sprite moverU = atlas.CreateSprite("MoverU");
        moverU.Scale = Scale;
        mSprites.Add(moverU);
        MonoGameLibrary.Graphics.Sprite moverD = atlas.CreateSprite("MoverD");
        moverD.Scale = Scale;
        mSprites.Add(moverD);

        gravBlock = atlas.CreateSprite("gravBlock");
        gravBlock.Scale = Scale;

        Sprite onButton = atlas.CreateSprite("onButton");
        onButton.Scale = Scale;
        Sprite offButton = atlas.CreateSprite("offButton");
        offButton.Scale = Scale;

        List<AnimatedSprite> eAnimations = new List<AnimatedSprite>();
        AnimatedSprite Ecks = atlas.CreateAnimatedSprite("Ecks");
        Ecks.Scale = Scale;
        eAnimations.Add(Ecks);
        AnimatedSprite Clock = atlas.CreateAnimatedSprite("Clock");
        Clock.Scale = Scale;
        eAnimations.Add(Clock);
        AnimatedSprite antiClock = atlas.CreateAnimatedSprite("antiClock");
        antiClock.Scale = Scale;
        eAnimations.Add(antiClock);
        AnimatedSprite VertEqual = atlas.CreateAnimatedSprite("VertEqual");
        VertEqual.Scale = Scale;
        eAnimations.Add(VertEqual);
        AnimatedSprite HorizEqual = atlas.CreateAnimatedSprite("HorizEqual");
        HorizEqual.Scale = Scale;
        eAnimations.Add(HorizEqual);

        AnimatedSprite EndPortal = atlas.CreateAnimatedSprite("EndPortal");
        EndPortal.Scale = new Vector2(5.25f, 5.25f);

        List<AnimatedSprite> wAnimations = new List<AnimatedSprite>();
        AnimatedSprite Water1 = atlas.CreateAnimatedSprite("Water1");
        Water1.Scale = Scale;
        wAnimations.Add(Water1);

        AnimatedSprite Water2 = atlas.CreateAnimatedSprite("Water2");
        Water2.Scale = Scale;
        wAnimations.Add(Water2);

        AnimatedSprite Water3 = atlas.CreateAnimatedSprite("Water3");
        Water3.Scale = Scale;
        wAnimations.Add(Water3);

        AnimatedSprite Water4 = atlas.CreateAnimatedSprite("Water4");
        Water4.Scale = Scale;
        wAnimations.Add(Water4);

        AnimatedSprite MidWater = atlas.CreateAnimatedSprite("MidWater");
        MidWater.Scale = Scale;
        wAnimations.Add(MidWater);

        AnimatedSprite BottomW = atlas.CreateAnimatedSprite("BottomW");
        BottomW.Scale = Scale;
        wAnimations.Add(BottomW);

        entities = Map.GetEntites();

        int waterNum = 0;

        foreach (string[] entity in entities)
        {
            if (entity[0] == "P")
            {
                // Create the palette
                _palette = new Palette(pAnimations, float.Parse(entity[1]), float.Parse(entity[2]));
            }
            else if ("LRUD".Contains(entity[0]))
            {
                Movings.Add(new Mover(entity[0], mSprites, float.Parse(entity[1]), float.Parse(entity[2])));
            }
            else if (entity[0] == "G")
            {
                Movings.Add(new Block(gravBlock, float.Parse(entity[1]), float.Parse(entity[2])));
            }
            else if (entity[0] == "B")
            {
                Buttons.Add(new GameObjects.Button(offButton, onButton, float.Parse(entity[1]) - 11, float.Parse(entity[2]) + 13));
            }
            else if ("XCAVH".Contains(entity[0]))
            {
                Effectors.Add(new Effector(entity[0], eAnimations, float.Parse(entity[1]), float.Parse(entity[2])));
            }
            else if ("SMF".Contains(entity[0]))
            {
                if (entity[0] == "S")
                {
                    if (waterNum == 0)
                    {
                        entity[0] = "W1";
                    }
                    else if (waterNum == 1)
                    {
                        entity[0] = "W2";
                    }
                    else if (waterNum == 2)
                    {
                        entity[0] = "W3";
                    }
                    else if (waterNum == 3)
                    {
                        entity[0] = "W4";
                    }
                    waterNum++;
                    waterNum %= 4;
                }
                Waters.Add(new Water(entity[0], wAnimations, float.Parse(entity[1]), float.Parse(entity[2])));
            }
            else if (entity[0] == "E")
            {
                Portal = new Portal(EndPortal, float.Parse(entity[1]), float.Parse(entity[2]));
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
            Reset();
            Core.ChangeScene(new TitleScene());
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

        foreach (var mover in Movings)
        {
            mover.Update(gameTime);
        }
        foreach (Water water in Waters)
        {
            water.Update(gameTime);
        }
        Portal.Update(gameTime);

        bool allPressed = true;
        bool isButton = false;
        foreach (GameObjects.Button button in Buttons)
        {
            isButton = true;
            if (!button.isPushed())
            {
                allPressed = false;
            }
        }

        bool toHide = false;

        if (allPressed && isButton)
        {
            foreach (Effector effector in Effectors)
            {
                if (effector.whichType() == "X")
                {
                    effector.Hide();
                }
            }
            toHide = true;
        }

        // Perform collision checks.
        CollisionChecks(gameTime, toHide);

        if (GameController.Reset())
        {
            InitializeNewGame();
        }
    }

    private void CollisionChecks(GameTime gameTime, bool hidden)
    {
        // Capture the current bounds of the palette
        Rectangle paletteBounds = _palette.GetBounds();
        Rectangle portalBounds = Portal.GetBounds();

        if (paletteBounds.Intersects(portalBounds))
        {
            Game1.NextLevel(false);
        }

        List<Rectangle> mapBlocks = MapCollision(map);
        List<Rectangle> entMapBlocks = MapCollision(entMap);

        bool pIsMoved = false;

        foreach (Water water in Waters)
        {
            Rectangle waterBounds = water.GetBounds();
            if (paletteBounds.Intersects(waterBounds))
            {
                _state = GameState.GameOver;
            }
        }

        foreach (GameObjects.Button button in Buttons)
        {
            Rectangle buttonBounds = button.GetBounds();
            if (paletteBounds.Intersects(buttonBounds))
            {
                // Find the distance from the edge of the button to mover
                float distanceLeft = Math.Abs(buttonBounds.Left - paletteBounds.Right);
                float distanceRight = Math.Abs(buttonBounds.Right - paletteBounds.Left);
                float distanceTop = Math.Abs(buttonBounds.Top - paletteBounds.Bottom);
                float distanceBottom = Math.Abs(buttonBounds.Bottom - paletteBounds.Top);

                // Determine which mover edge is the closest.
                float minDistance = Math.Min(Math.Min(distanceLeft, distanceRight), Math.Min(distanceTop, distanceBottom));

                if (minDistance == distanceLeft)
                {
                    _palette.Shift("left", (int)minDistance);
                }
                else if (minDistance == distanceRight)
                {
                    _palette.Shift("right", (int)minDistance);
                }
                else if (minDistance == distanceTop)
                {
                    _palette.Shift("up", (int)minDistance);
                    if (!button.isPushed())
                    {
                        button.Push(gameTime);
                    }
                }
                else if (minDistance == distanceBottom)
                {
                    _palette.Shift("down", (int)minDistance);
                }
            }
        }

        foreach (Rectangle entBound in entMapBlocks)
        {
            if (!hidden)
            {
                mapBlocks.Add(entBound);
            }
        }

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
                    pIsMoved = true;
                }
                if (minDistance == distanceRight)
                {
                    _palette.Shift("right", (int)minDistance);
                    pIsMoved = true;
                }
                if (minDistance == distanceTop)
                {
                    _palette.Shift("up", (int)minDistance);
                    pIsMoved = true;
                    // Therefore Palette is on terrain
                    _palette.Land();
                }
                if (minDistance == distanceBottom)
                {
                    _palette.Shift("down", (int)minDistance);
                    pIsMoved = true;
                }
            }
        }

        for (int i = 0; i < Movings.Count; i++)
        {
            var mover = Movings[i];
            Rectangle moverBounds = mover.GetBounds();
            foreach (Rectangle rect in mapBlocks)
            {
                if (moverBounds.Intersects(rect))
                {
                    // Find the distance from the edge of the rect to mover
                    float distanceLeft = Math.Abs(rect.Left - moverBounds.Right);
                    float distanceRight = Math.Abs(rect.Right - moverBounds.Left);
                    float distanceTop = Math.Abs(rect.Top - moverBounds.Bottom);
                    float distanceBottom = Math.Abs(rect.Bottom - moverBounds.Top);

                    // Determine which mover edge is the closest.
                    float minDistance = Math.Min(Math.Min(distanceLeft, distanceRight), Math.Min(distanceTop, distanceBottom));

                    if (minDistance == distanceLeft)
                    {
                        mover.noMoving("R");
                        mover.Shift("left", (int)minDistance);
                    }
                    else if (minDistance == distanceRight)
                    {
                        mover.noMoving("L");
                        mover.Shift("right", (int)minDistance);
                    }
                    else if (minDistance == distanceTop)
                    {
                        mover.Shift("up", (int)minDistance);
                        mover.Land();
                    }
                    else if (minDistance == distanceBottom)
                    {
                        mover.noMoving("U");
                        mover.Shift("down", (int)minDistance);
                    }
                }
            }
            if (moverBounds.Intersects(paletteBounds))
            {
                // Find the distance from the edge of the palette to mover
                float distanceLeft = Math.Abs(moverBounds.Left - paletteBounds.Right);
                float distanceRight = Math.Abs(moverBounds.Right - paletteBounds.Left);
                float distanceTop = Math.Abs(moverBounds.Top - paletteBounds.Bottom);
                float distanceBottom = Math.Abs(moverBounds.Bottom - paletteBounds.Top);

                // Determine which mover edge is the closest.
                float minDistance = Math.Min(Math.Min(distanceLeft, distanceRight), Math.Min(distanceTop, distanceBottom));

                if (mover.getType() == "B")
                {
                    if (minDistance == distanceLeft)
                    {
                        if (mover.Movable("R"))
                        {
                            mover.Shift("right", (int)minDistance);
                        }
                        _palette.Shift("left", (int)minDistance);
                    }
                    else if (minDistance == distanceRight)
                    {
                        if (mover.Movable("L"))
                        {
                            mover.Shift("left", (int)minDistance);
                        }
                        _palette.Shift("right", (int)minDistance);
                    }
                    else if (minDistance == distanceTop)
                    {
                        _palette.Shift("up", (int)minDistance);
                        // If palette is moved up, palette is standing on mover
                        _palette.Land();
                    }
                    else if (minDistance == distanceBottom)
                    {
                        if (mover.Movable("U"))
                        {
                            mover.Shift("up", (int)minDistance);
                        }
                        _palette.Shift("down", (int)minDistance);
                    }
                }
                else
                {
                    if (minDistance == distanceLeft)
                    {
                        _palette.Shift("left", (int)minDistance);
                    }
                    else if (minDistance == distanceRight)
                    {
                        _palette.Shift("right", (int)minDistance);
                    }
                    else if (minDistance == distanceTop)
                    {
                        _palette.Shift("up", (int)minDistance);
                        // If palette is moved up, palette is standing on mover
                        _palette.Land();
                    }
                    else if (minDistance == distanceBottom)
                    {
                        _palette.Shift("down", (int)minDistance);
                    }
                }
            }
            for (int j = 0; j < Movings.Count; j++)
            {
                if (i != j)
                {
                    Mover mover2 = Movings[j];
                    Rectangle M2Bounds = mover2.GetBounds();
                    if (moverBounds.Intersects(M2Bounds))
                    {
                        // Find the distance from the edge of the rect to mover
                        float distanceLeft = Math.Abs(M2Bounds.Right - moverBounds.Left);
                        float distanceRight = Math.Abs(M2Bounds.Left - moverBounds.Right);
                        float distanceTop = Math.Abs(M2Bounds.Bottom - moverBounds.Top);
                        float distanceBottom = Math.Abs(M2Bounds.Top - moverBounds.Bottom);

                        // Determine which mover edge is the closest.
                        float minDistance = Math.Min(Math.Min(distanceLeft, distanceRight), Math.Min(distanceTop, distanceBottom));

                        if (mover.getType() == "U" && mover2.getType() == "D")
                        {
                            mover.Shift("down", (int)minDistance / 2);
                            mover2.Shift("up", (int)minDistance / 2);
                        }
                        else if (mover.getType() == "D" && mover2.getType() == "U")
                        {
                            mover.Shift("up", (int)minDistance / 2);
                            mover2.Shift("down", (int)minDistance / 2);
                        }
                        else if (mover.getType() == "R" && mover2.getType() == "L")
                        {
                            mover.Shift("left", (int)minDistance / 2);
                            mover2.Shift("right", (int)minDistance / 2);
                        }
                        else if (mover.getType() == "L" && mover2.getType() == "R")
                        {
                            mover.Shift("right", (int)minDistance / 2);
                            mover2.Shift("left", (int)minDistance / 2);
                        }
                        else if (mover.getType() == "B" && mover2.getType() == "B")
                        {
                            mover.Shift("right", (int)minDistance / 2);
                            mover2.Shift("left", (int)minDistance / 2);
                        }
                        else
                        {
                            if (minDistance == distanceLeft)
                            {
                                if (mover.getType() == "L")
                                {
                                    if (mover2.Movable("L"))
                                    {
                                        mover2.Shift("left", (int)minDistance);
                                    }
                                    else
                                    {
                                        mover.Shift("right", (int)minDistance);
                                        mover.noMoving("L");
                                    }
                                }
                                else if (mover2.getType() == "R")
                                {
                                    if (mover.Movable("R"))
                                    {
                                        mover.Shift("right", (int)minDistance);
                                    }
                                    else
                                    {
                                        mover2.Shift("left", (int)minDistance);
                                        mover2.noMoving("R");
                                    }
                                }
                                else
                                {
                                    if (mover.Movable("R") && mover2.Movable("L"))
                                    {
                                        mover.Shift("right", (int)minDistance / 2);
                                        mover2.Shift("left", (int)minDistance / 2);
                                    }
                                    else if (mover.Movable("R"))
                                    {
                                        mover.Shift("right", (int)minDistance);
                                        mover.noMoving("L");
                                    }
                                    else if (mover2.Movable("L"))
                                    {
                                        mover2.Shift("left", (int)minDistance);
                                        mover2.noMoving("R");
                                    }
                                }
                            }
                            else if (minDistance == distanceRight)
                            {
                                if (mover.getType() == "R")
                                {
                                    if (mover2.Movable("R"))
                                    {
                                        mover2.Shift("right", (int)minDistance);
                                    }
                                    else
                                    {
                                        mover.Shift("left", (int)minDistance);
                                        mover.noMoving("R");
                                    }
                                }
                                else if (mover2.getType() == "L")
                                {
                                    if (mover.Movable("L"))
                                    {
                                        mover.Shift("left", (int)minDistance);
                                    }
                                    else
                                    {
                                        mover2.Shift("right", (int)minDistance);
                                        mover2.noMoving("L");
                                    }
                                }
                                else
                                {
                                    if (mover.Movable("R") && mover2.Movable("L"))
                                    {
                                        mover.Shift("right", (int)minDistance / 2);
                                        mover2.Shift("left", (int)minDistance / 2);
                                    }
                                    else if (mover.Movable("R"))
                                    {
                                        mover.Shift("right", (int)minDistance);
                                        mover.noMoving("L");
                                    }
                                    else if (mover2.Movable("L"))
                                    {
                                        mover2.Shift("left", (int)minDistance);
                                        mover2.noMoving("R");
                                    }
                                }
                            }
                            else if (minDistance == distanceTop)
                            {
                                if (mover.getType() == "U")
                                {
                                    if (mover2.Movable("U"))
                                    {
                                        mover2.Shift("up", (int)minDistance);
                                    }
                                    else
                                    {
                                        mover.Shift("down", (int)minDistance);
                                        mover.noMoving("U");
                                    }
                                }
                                else if (mover2.getType() == "D")
                                {
                                    if (mover.Movable("D"))
                                    {
                                        mover.Shift("down", (int)minDistance);
                                    }
                                    else
                                    {
                                        mover2.Shift("up", (int)minDistance);
                                        mover2.noMoving("D");
                                    }
                                }
                                else
                                {
                                    if (mover.Movable("D") && mover2.Movable("U"))
                                    {
                                        mover.Shift("down", (int)minDistance / 2);
                                        mover2.Shift("up", (int)minDistance / 2);
                                    }
                                    else if (mover.Movable("D"))
                                    {
                                        mover.Shift("down", (int)minDistance);
                                        mover.noMoving("U");
                                    }
                                    else if (mover2.Movable("U"))
                                    {
                                        mover2.Shift("up", (int)minDistance);
                                        mover2.noMoving("D");
                                    }
                                }
                            }
                            else if (minDistance == distanceBottom)
                            {
                                if (mover.getType() == "D")
                                {
                                    if (mover2.Movable("D"))
                                    {
                                        mover2.Shift("down", (int)minDistance);
                                    }
                                    else
                                    {
                                        mover.Shift("up", (int)minDistance);
                                        mover.noMoving("D");
                                    }
                                }
                                else if (mover2.getType() == "U")
                                {
                                    if (mover.Movable("U"))
                                    {
                                        mover.Shift("up", (int)minDistance);
                                    }
                                    else
                                    {
                                        mover2.Shift("down", (int)minDistance);
                                        mover2.noMoving("U");
                                    }
                                }
                                else
                                {
                                    if (mover.Movable("U") && mover2.Movable("D"))
                                    {
                                        mover.Shift("up", (int)minDistance / 2);
                                        mover2.Shift("down", (int)minDistance / 2);
                                    }
                                    else if (mover.Movable("U"))
                                    {
                                        mover.Shift("up", (int)minDistance);
                                        mover.noMoving("D");
                                    }
                                    else if (mover2.Movable("D"))
                                    {
                                        mover2.Shift("down", (int)minDistance);
                                        mover2.noMoving("U");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (GameObjects.Button button in Buttons)
            {
                Rectangle buttonBounds = button.GetBounds();
                if (moverBounds.Intersects(buttonBounds))
                {
                    // Find the distance from the edge of the button to mover
                    float distanceLeft = Math.Abs(buttonBounds.Left - moverBounds.Right);
                    float distanceRight = Math.Abs(buttonBounds.Right - moverBounds.Left);
                    float distanceTop = Math.Abs(buttonBounds.Top - moverBounds.Bottom);
                    float distanceBottom = Math.Abs(buttonBounds.Bottom - moverBounds.Top);

                    // Determine which mover edge is the closest.
                    float minDistance = Math.Min(Math.Min(distanceLeft, distanceRight), Math.Min(distanceTop, distanceBottom));

                    if (minDistance == distanceLeft)
                    {
                        mover.Shift("left", (int)minDistance);
                    }
                    else if (minDistance == distanceRight)
                    {
                        mover.Shift("right", (int)minDistance);
                    }
                    else if (minDistance == distanceTop)
                    {
                        mover.Shift("up", (int)minDistance);
                        if (!button.isPushed())
                        {
                            button.Push(gameTime);
                        }
                        mover.Hide();
                    }
                    else if (minDistance == distanceBottom)
                    {
                        mover.Shift("down", (int)minDistance);
                    }
                }
            }
            foreach (Effector effector in Effectors)
            {
                string whichFace = "";
                bool touching = false;
                Rectangle effBounds = effector.GetBounds();
                if (moverBounds.Intersects(effBounds))
                {
                    // Find the distance from the edge of the effector to mover
                    float distanceLeft = Math.Abs(effBounds.Left - moverBounds.Right);
                    float distanceRight = Math.Abs(effBounds.Right - moverBounds.Left);
                    float distanceTop = Math.Abs(effBounds.Top - moverBounds.Bottom);
                    float distanceBottom = Math.Abs(effBounds.Bottom - moverBounds.Top);

                    // Determine which mover edge is the closest.
                    float minDistance = Math.Min(Math.Min(distanceLeft, distanceRight), Math.Min(distanceTop, distanceBottom));

                    if (minDistance == distanceLeft)
                    {
                        mover.Shift("left", (int)minDistance);
                        whichFace = "left";
                        touching = true;
                    }
                    else if (minDistance == distanceRight)
                    {
                        mover.Shift("right", (int)minDistance);
                        whichFace = "right";
                        touching = true;
                    }
                    else if (minDistance == distanceTop)
                    {
                        mover.Shift("up", (int)minDistance);
                        whichFace = "top";
                        touching = true;
                    }
                    else if (minDistance == distanceBottom)
                    {
                        mover.Shift("down", (int)minDistance);
                        whichFace = "bottom";
                        touching = true;
                    }
                }
                else if (moverBounds.Top == effBounds.Bottom && moverBounds.Left < effBounds.Right && moverBounds.Right > effBounds.Left)
                {
                    whichFace = "bottom";
                    touching = true;
                }
                else if (moverBounds.Bottom == effBounds.Top && moverBounds.Left < effBounds.Right && moverBounds.Right > effBounds.Left)
                {
                    whichFace = "top";
                    touching = true;
                }
                else if (moverBounds.Right == effBounds.Left && moverBounds.Top < effBounds.Bottom && moverBounds.Bottom > effBounds.Top)
                {
                    whichFace = "left";
                    touching = true;
                }
                else if (moverBounds.Left == effBounds.Right && moverBounds.Top < effBounds.Bottom && moverBounds.Bottom > effBounds.Top)
                {
                    whichFace = "right";
                    touching = true;
                }
                if (touching && !effector.hasAffected() && !mover.hasAffected())
                {
                    effector.Affect();
                    mover.Affect();
                    bool equals = false;
                    Mover newMover = new Mover();
                    switch (effector.whichType())
                    {
                        case "C":
                            switch (mover.getType())
                            {
                                case "R": mover.changeType("U"); break;
                                case "L": mover.changeType("D"); break;
                                case "U": mover.changeType("L"); break;
                                case "D": mover.changeType("R"); break;
                            }
                            break;
                        case "A":
                            switch (mover.getType())
                            {
                                case "R": mover.changeType("D"); break;
                                case "L": mover.changeType("U"); break;
                                case "U": mover.changeType("R"); break;
                                case "D": mover.changeType("L"); break;
                            }
                            break;
                        case "V":
                            if (whichFace == "top")
                            {
                                equals = true;
                                if (mover.getType() == "B")
                                {
                                    newMover = new Block(gravBlock, mover.getX(), effector.getY() + mover.GetHeight());
                                }
                                else
                                {
                                    newMover = new Mover(mover.getType(), mSprites, mover.getX(), effector.getY() + mover.GetHeight());
                                }
                            }
                            else if (whichFace == "bottom")
                            {
                                equals = true;
                                if (mover.getType() == "B")
                                {
                                    newMover = new Block(gravBlock, mover.getX(), effector.getY() - mover.GetHeight());
                                }
                                else
                                {
                                    newMover = new Mover(mover.getType(), mSprites, mover.getX(), effector.getY() - mover.GetHeight());
                                }
                            }
                            else
                            {
                                break;
                            }
                            break;
                        case "H":
                            if (whichFace == "right")
                            {
                                equals = true;
                                if (mover.getType() == "B")
                                {
                                    newMover = new Block(gravBlock, effector.getX() - mover.GetWidth(), mover.getY());
                                }
                                else
                                {
                                    newMover = new Mover(mover.getType(), mSprites, effector.getX() - mover.GetWidth(), mover.getY());
                                }
                            }
                            else if (whichFace == "left")
                            {
                                equals = true;
                                if (mover.getType() == "B")
                                {
                                    newMover = new Block(gravBlock, effector.getX() + mover.GetWidth(), mover.getY());
                                }
                                else
                                {
                                    newMover = new Mover(mover.getType(), mSprites, effector.getX() + mover.GetWidth(), mover.getY());
                                }
                            }
                            else
                            {
                                break;
                            }
                            break;
                    }
                    if (equals)
                    {
                        Rectangle bounds = newMover.GetBounds();
                        bool clear = true;
                        if (bounds.Intersects(_palette.GetBounds()))
                        {
                            clear = false;
                        }
                        foreach (var Mover in Movings)
                        {
                            Rectangle mBounds = Mover.GetBounds();
                            if (bounds.Intersects(mBounds))
                            {
                                clear = false;
                            }
                        }
                        foreach (GameObjects.Button button in Buttons)
                        {
                            Rectangle bBounds = button.GetBounds();
                            if (bounds.Intersects(bBounds))
                            {
                                clear = false;
                            }
                        }
                        foreach (Effector eff in Effectors)
                        {
                            Rectangle fBounds = eff.GetBounds();
                            if (bounds.Intersects(fBounds))
                            {
                                clear = false;
                            }
                        }
                        if (clear)
                        {
                            newMovers.Add(newMover);
                            Movings.Add(newMover);
                            i++;
                        }
                        else
                        {
                            effector.unAffect();
                            mover.unAffect();
                        }
                    }
                }
                if (!touching && effector.hasAffected() && mover.hasAffected())
                {
                    effector.unAffect();
                    mover.unAffect();
                }
            }
        }
    }

    static List<Rectangle> MapCollision(int[,] Map)
    {
        int groups = 0;

        /// Each pass through the system
        int[,] flatpass = new int[Map.GetLength(0), Map.GetLength(1)];
        int[,] sizepass = new int[Map.GetLength(0), Map.GetLength(1)];
        int[,] groupingpass = new int[Map.GetLength(0), Map.GetLength(1)];

        /// Iterates through the system horizontally, grouping the hits in each row
        /// and assigns each a size and index value while labelling the groups
        for (int y = 0; y < Map.GetLength(0); y++)
        {
            int ones = 0;
            for (int x = 0; x < Map.GetLength(1); x++)
            {
                if (Map[y, x] == 1)
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
                    flatpass[y, Map.GetLength(1) - 1 - z] = ones * 100 + z + 1;
                    groupingpass[y, Map.GetLength(1) - 1 - z] = groups;
                }
            }
        }

        /// Iterates vertically through the system to combine groups
        for (int x = 0; x < Map.GetLength(1); x++)
        {
            int same = 1;
            /// Pre sets the check value as the top input in the column
            int value = flatpass[0, x];
            for (int y = 0; y < Map.GetLength(0) - 1; y++)
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
                sizepass[Map.GetLength(0) - z - 1, x] = same * (value / 100);
                groupingpass[Map.GetLength(0) - z - 1, x] = groupingpass[Map.GetLength(0) - same, x];
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

        foreach (var mover in Movings)
        {
            mover.Draw();
        }
        foreach (GameObjects.Button button in Buttons)
        {
            button.Draw();
        }
        foreach (Effector effector in Effectors)
        {
            effector.Draw();
        }
        foreach (Water water in Waters)
        {
            water.Draw();
        }
        Portal.Draw();

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();

        // Draw the UI.
        _ui.Draw();
    }
}