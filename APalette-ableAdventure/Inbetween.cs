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

    public Inbetween()
    {

    }
    public void Initialize()
    {
        Scene = 1;
        Core.ChangeScene(new TitleScene());
    }
    public void Change(bool Title)
    {
        if (!Title)
        {
            Scene++;
        }
        switch (Scene)
        {
            case 0: Core.ChangeScene(new TitleScene()); break;
            case 1: Core.ChangeScene(new Tutorials("Tutorial0")); break;
            case 2: Core.ChangeScene(new Tutorials("Tutorial1")); break;
            case 3: Core.ChangeScene(new Tutorials("Tutorial2")); break;

            case 10: Core.ChangeScene(new TitleScene()); break;
        }
    }
}
