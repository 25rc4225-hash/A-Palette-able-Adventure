using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoGameLibrary.Scenes;

public abstract class Scene : IDisposable
{
    /// Gets the ContentManager used for loading scene-specific assets.
    protected ContentManager Content { get; }

    /// Gets a value that indicates if the scene has been disposed of.
    public bool IsDisposed { get; private set; }

    /// Creates a new scene instance.
    public Scene()
    {
        // Create a content manager for the scene
        Content = new ContentManager(Core.Content.ServiceProvider);

        // Set the root directory for content to the same as the root directory
        // for the game's content.
        Content.RootDirectory = Core.Content.RootDirectory;
    }

    // Finalizer, called when object is cleaned up by garbage collector.
    ~Scene() => Dispose(false);

    /// Initializes the scene.
    public virtual void Initialize()
    {
        LoadContent();
    }

    /// Override to provide logic to load content for the scene.
    public virtual void LoadContent() { }

    /// Unloads scene-specific content.
    public virtual void UnloadContent()
    {
        Content.Unload();
    }

    /// Updates this scene.
    public virtual void Update(GameTime gameTime) { }

    /// Draws this scene.
    public virtual void Draw(GameTime gameTime) { }

    /// Disposes of this scene.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// Disposes of this scene.
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            UnloadContent();
            Content.Dispose();
        }
        IsDisposed = true;
    }
}