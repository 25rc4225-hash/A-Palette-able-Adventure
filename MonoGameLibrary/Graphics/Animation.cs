using System;
using System.Collections.Generic;

namespace MonoGameLibrary.Graphics;

public class Animation
{
    /// The texture regions that make up the frames of this animation.
    /// The order of the regions within the collection
    /// are the order that the frames should be displayed in.
    public List<TextureRegion> Frames { get; set; }

    /// The amount of time to delay between each frame before
    /// moving to the next frame for this animation.
    public TimeSpan Delay { get; set; }

    /// Creates a new animation.
    public Animation()
    {
        Frames = new List<TextureRegion>();
        Delay = TimeSpan.FromMilliseconds(100);
    }

    /// Creates a new animation with the specified frames and delay.
    public Animation(List<TextureRegion> frames, TimeSpan delay)
    {
        Frames = frames;
        Delay = delay;
    }
}