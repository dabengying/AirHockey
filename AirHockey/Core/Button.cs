using System;
using AirHockey;
using AirHockey.Graphics;
using IrrKlang;
using OpenTK.Input;

namespace AirHockey
{

    class Button
    {
        public Button(Effect effect, OpenTK.Box2 b)
        {

            this.graphicsObject = new Visual(GeometryFactory.CreateBox(b), effect);
            box = b;
            pressed = false;
        }

        public void Update(Input input, ISoundEngine soundEngine, float deltaTime)
        {
            float x = input.ViewPosition.X;
            float y = input.ViewPosition.Y;
            over = (x > box.Left && x < box.Right && y < box.Top && y > box.Bottom);
            if (!over)
                pressed = false;

            if (input.IsPressed && over)
                    pressed = true;

            if (!input.IsPressed && pressed && over)
            {
                soundEngine.Play2D(ResourceManager.MediaPath + "buttonPress.wav");
                Clicked(this, null);
                pressed = false;
            }
        }

        public Visual GetGraphicsObject()
        {
            return graphicsObject;
            //  if(over && pressed)
            //renderer.DrawBox(box, new Color4(0, 0, 0, 0), new Color4(1, 0, 0, 1));
            // else if (over)
            // renderer.DrawBox(box, new Color4(0, 0, 0, 0), new Color4(0, 0, 1, 1));
        }

        public EventHandler Clicked;


        Visual graphicsObject;
        OpenTK.Box2 box;
        bool over;
        bool pressed;


    }

}
