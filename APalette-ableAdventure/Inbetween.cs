using APalette_ableAdventure.GameObjects;
using APalette_ableAdventure.UI;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using APalette_ableAdventure.Scenes;
using Gum.Forms;
using Microsoft.Xna.Framework.Media;

namespace APalette_ableAdventure;

public class Inbetween
{
    private int Scene = 0;
    private const int startScene = 1;
    private const int lastScene = 8;

    public Inbetween()
    {

    }
    public void Initialize()
    {
        Scene = startScene;
        Core.ChangeScene(new TitleScene());
    }
    public void Change(bool Title)
    {
        if (!Title)
        {
            Scene++;
        }
        if(Scene == 0)
        {
            Core.ChangeScene(new TitleScene());
        }
        else
        {
            if (Scene <= lastScene)
            {
                Core.ChangeScene(new Levels(Scene.ToString()));
            }
            else
            {
                Scene = 1;
                Core.ChangeScene(new TitleScene());
            }
        }
    }
}
