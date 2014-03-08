using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Diagnostics;
    using System.IO;

    using System.Drawing;
    using System.Drawing.Imaging;

    using OpenTK;
    using OpenTK.Graphics;



    public class Program 
    {
        private class ProgramGameWindow : GameWindow
        {
            Program program;

            public double Time { get; set; }


            public ProgramGameWindow(Program p, int width, int height)
                : base(width, height)
            {
                program = p;
            }

            protected override void OnLoad(EventArgs e)
            {

                program.OnLoad(e);
            }


            protected override void OnUnload(EventArgs e)
            {
                program.OnUnload(e);
            }

            protected override void OnResize(EventArgs e)
            {
                program.OnResize(e);
            }

            protected override void OnUpdateFrame(FrameEventArgs eventArgs)
            {
                Time += eventArgs.Time;
                program.OnUpdateFrame((float)eventArgs.Time);
            }


            protected override void OnRenderFrame(FrameEventArgs eventArgs)
            {
                program.OnRenderFrame((float)eventArgs.Time);
            }

            new protected void SwapBuffers()
            {
                base.SwapBuffers();
            }
        }


        public int Width
        {
            get
            {
                return gameWindow.Width;
            }

            set
            {
                gameWindow.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return gameWindow.Height;
            }
            set
            {
                gameWindow.Height = value;
            }
        }


       public double Time 
       {
           get { return gameWindow.Time; }
       }

        public Program(int width, int height)
        {
            gameWindow = new ProgramGameWindow(this, width, height);

            
        }

        protected virtual void OnLoad(EventArgs e)
        {

        }

      

        protected virtual void OnUnload(EventArgs e)
        {

        }

        protected virtual void OnResize(EventArgs e)
        {
        }


        protected virtual void OnUpdateFrame(float deltaTime)
        {
            

        }

        protected virtual void OnRenderFrame(float deltaTime)
        {
        }

        protected void SwapBuffers()
        {
            gameWindow.SwapBuffers();
        }

        public void Run()
        {
            gameWindow.Run();
        }

        public int MouseX { get { return gameWindow.Mouse.X;  } }
        public int MouseY { get { return gameWindow.Mouse.Y;  } }

        ProgramGameWindow gameWindow;

    }
    
}
