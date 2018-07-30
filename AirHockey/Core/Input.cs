using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirHockey.Graphics;
using OpenTK.Input;

namespace AirHockey
{
    class Input
    {
        public Input(AppWindow app, Renderer renderer)
        {
            this.app = app;
            this.renderer = renderer;
        }
        public Vector2 ClientPosition
        {
            get
            {
                return new Vector2(app.MouseX, app.MouseY);
            }
        }
        public Vector2 ViewPosition
        {
            get
            {
                return new Vector2(ClientToView(app.MouseX, app.MouseY));
            }
        }

       public bool IsPressed
        {
            get
            {
                return OpenTK.Input.Mouse.GetState()[MouseButton.Left];
            }
        }

        public Vector2 ClientToView(int x, int y)
        {
            return new Vector2(
                renderer.ViewWidth * (float)x / (float)renderer.ClientWidth - renderer.ViewWidth * 0.5f,
                renderer.ViewHeight * (renderer.ClientHeight - y) / (float)renderer.ClientHeight - renderer.ViewHeight * 0.5f);
        }

        AppWindow app;
        Renderer renderer;

    }
}
