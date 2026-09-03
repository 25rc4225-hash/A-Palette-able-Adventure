using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class TextureAtlas
{
    // Stores animations added to this atlas.
    private Dictionary<string, Animation> _animations;
    private Dictionary<string, TextureRegion> _regions;

    /// Gets or Sets the source texture represented by this texture atlas.
    public Texture2D Texture { get; set; }

    /// Creates a new texture atlas.
    public TextureAtlas()
    {
        _animations = new Dictionary<string, Animation>();
        _regions = new Dictionary<string, TextureRegion>();
    }

    /// Creates a new texture atlas instance using the given texture.
    public TextureAtlas(Texture2D texture)
    {
        Texture = texture;
        _animations = new Dictionary<string, Animation>();
        _regions = new Dictionary<string, TextureRegion>();
    }

    /// Creates a new region and adds it to this texture atlas.
    public void AddRegion(string name, int x, int y, int width, int height)
    {
        TextureRegion region = new TextureRegion(Texture, x, y, width, height);
        _regions.Add(name, region);
    }

    /// Gets the region from this texture atlas with the specified name.
    public TextureRegion GetRegion(string name)
    {
        return _regions[name];
    }

    /// Removes the region from this texture atlas with the specified name.
    public bool RemoveRegion(string name)
    {
        return _regions.Remove(name);
    }

    /// Removes all regions from this texture atlas.
    public void Clear()
    {
        _regions.Clear();
    }

    /// Creates a new texture atlas based on a texture atlas xml configuration file.
    public static TextureAtlas FromFile(ContentManager content, string fileName)
    {
        TextureAtlas atlas = new TextureAtlas();

        string filePath = Path.Combine(content.RootDirectory, fileName);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                // The <Texture> element contains the content path for the Texture2D to load.
                // So we will retrieve that value then use the content manager to load the texture.
                string texturePath = root.Element("Texture").Value;
                atlas.Texture = content.Load<Texture2D>(texturePath);

                // The <Regions> element contains individual <Region> elements, each one describing
                // a different texture region within the atlas.
                // So we retrieve all of the <Region> elements then loop through each one
                // and generate a new TextureRegion instance from it and add it to this atlas.
                var regions = root.Element("Regions")?.Elements("Region");

                if (regions != null)
                {
                    foreach (var region in regions)
                    {
                        string name = region.Attribute("name")?.Value;
                        int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                        int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                        int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                        int height = int.Parse(region.Attribute("height")?.Value ?? "0");

                        if (!string.IsNullOrEmpty(name))
                        {
                            atlas.AddRegion(name, x, y, width, height);
                        }
                    }
                }

                // The <Animations> element contains individual <Animation> elements, each one describing
                // a different animation within the atlas.
                // So we retrieve all of the <Animation> elements then loop through each one
                // and generate a new Animation instance from it and add it to this atlas.
                var animationElements = root.Element("Animations").Elements("Animation");

                if (animationElements != null)
                {
                    foreach (var animationElement in animationElements)
                    {
                        string name = animationElement.Attribute("name")?.Value;
                        float delayInMilliseconds = float.Parse(animationElement.Attribute("delay")?.Value ?? "0");
                        TimeSpan delay = TimeSpan.FromMilliseconds(delayInMilliseconds);

                        List<TextureRegion> frames = new List<TextureRegion>();

                        var frameElements = animationElement.Elements("Frame");

                        if (frameElements != null)
                        {
                            foreach (var frameElement in frameElements)
                            {
                                string regionName = frameElement.Attribute("region").Value;
                                TextureRegion region = atlas.GetRegion(regionName);
                                frames.Add(region);
                            }
                        }

                        Animation animation = new Animation(frames, delay);
                        atlas.AddAnimation(name, animation);
                    }
                }

                return atlas;
            }
        }
    }
    
    /// Creates a new sprite using the region from this texture atlas with the specified name.
    public Sprite CreateSprite(string regionName)
    {
        TextureRegion region = GetRegion(regionName);
        return new Sprite(region);
    }

    /// Adds the given animation to this texture atlas with the specified name.
    public void AddAnimation(string animationName, Animation animation)
    {
        _animations.Add(animationName, animation);
    }

    /// Gets the animation from this texture atlas with the specified name.
    public Animation GetAnimation(string animationName)
    {
        return _animations[animationName];
    }

    /// Removes the animation with the specified name from this texture atlas.
    public bool RemoveAnimation(string animationName)
    {
        return _animations.Remove(animationName);
    }

    /// Creates a new animated sprite using the animation from this texture atlas with the specified name.
    public AnimatedSprite CreateAnimatedSprite(string animationName)
    {
        Animation animation = GetAnimation(animationName);
        return new AnimatedSprite(animation);
    }
}