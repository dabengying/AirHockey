using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System.Drawing;
using System.Drawing.Imaging;

using System;

namespace AirHockey.Graphics
{
    public class Renderer
    {
        public Renderer()
        {
            Camera = new Camera();
            ClearColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
            ShadowEffect = new VertexColorEffect();
            
        }

        DeferredLighting lightBuffer;
        int clientWidth;
        int clientHeight;

        public Camera Camera;
        public Color4 ClearColor { get; set; }

        public int ClientWidth
        {
            get { return clientWidth; }
        }
        public int ClientHeight
        {
            get { return clientHeight; }
        }

        public void Viewport(int width, int height)
        {
            clientWidth = width;
            clientHeight = height;
            lightBuffer = new DeferredLighting(width, height);
            OpenTK.Graphics.OpenGL.GL.Viewport(new Size(width, height));
        }

        public float ViewWidth
        {
            get { return Camera.Width; }
        }
        public float ViewHeight
        {
            get { return Camera.Height; }
        }

        public void Clear()
        {
            GL.ClearColor(ClearColor.R, ClearColor.G, ClearColor.B, ClearColor.A);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void DrawScene(Scene scene)
        {
            foreach (Visual visual in scene.Visuals)
            {
                switch(visual.Effect)
                {
                    case VertexColorEffect cce:
                        Matrix3 worldViewMatrix1 = Camera.ProjectionMatrix * Camera.ViewMatrix * visual.WorldMatrix;
                        cce.WorldViewMatrix = worldViewMatrix1;
                        break;
                    case TextureEffect te:
                        Matrix3 worldViewMatrix2 = Camera.ProjectionMatrix * Camera.ViewMatrix * visual.WorldMatrix;
                        te.WorldViewMatrix = worldViewMatrix2;
                        break;


                }

                visual.Effect.Begin();
                visual.Geometry.Draw();

                visual.Effect.End();
            }

            DrawShadows(scene);
            lightBuffer.Camera = Camera;
            lightBuffer.Draw();

        }

        void DrawShadows(Scene scene)
        {
            foreach(Shadow shadow in scene.Shadows)
            {
                if(shadow.Vertices != null)
                {
                    float[] vertices = new float[2 * 4];
                    Color4[] colors = new Color4[4];
                    for(int i = 0; i < 4; i++)
                    {
                        vertices[i * 2 + 0] = shadow.Vertices[i].X;
                        vertices[i * 2 + 1] = shadow.Vertices[i].Y;
                        colors[i].R = 0.0f;
                        colors[i].G = 0.0f;
                        colors[i].B = 0.0f;
                        colors[i].A = 0.2f;
                    }

                    Geometry shadowGeometry = 
                        new Geometry(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, shadow.Vertices, shadow.Colors, null, null);

                    ShadowEffect.Begin();
                    ShadowEffect.WorldViewMatrix = Camera.ProjectionMatrix * Camera.ViewMatrix;

                    shadowGeometry.Draw();

                    ShadowEffect.End();
                    GL.BindVertexArray(0);

                }



            }
        }

        VertexColorEffect ShadowEffect;

        public void OrthoCentered(float width, float height)
        {
            Camera.Width = width;
            Camera.Height = height;
        }
    }
}