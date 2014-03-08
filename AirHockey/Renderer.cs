using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System.Drawing;
using System.Drawing.Imaging;

using System;

namespace AirHockey
{
    public class Renderer
    {
        public Renderer()
        {
            loadedTextures = new Dictionary<string, int>();
            ClearColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
        }

        //TODO: make this draw text
        /*
        int CreateNumberTexture(int number)
        {
            int texture;
            Bitmap bmp;

            // Create Bitmap and OpenGL texture
            bmp = new Bitmap(Width, Height); // match window size

            texture = GL.GenTexture();
            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, texture);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, OpenTK.Graphics.OpenGL.TextureParameterName.TextureMagFilter, (int)OpenTK.Graphics.OpenGL.All.Linear);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, OpenTK.Graphics.OpenGL.TextureParameterName.TextureMinFilter, (int)OpenTK.Graphics.OpenGL.All.Linear);
            GL.TexImage2D(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, IntPtr.Zero); // just allocate memory, so we can update efficiently using TexSubImage2D

            // Render text using System.Drawing.
            // Do this only when text changes.
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.Clear(Color.Transparent);

                Font font = new Font("Arial", 22);
                Brush brush = new SolidBrush(Color.Black);
                PointF point = new PointF(0, 0);

                char ch = (char)((int)'0' + number);
                String s = new String(ch, 1);
                gfx.DrawString(s, font, brush, point);
            }

            // Upload the Bitmap to OpenGL.
            // Do this only when text changes.
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            OpenTK.Graphics.OpenGL.GL.TexImage2D(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba, Width, Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            return texture;
        }

        void DrawTextTexture(int textureID, Vector2 position)
        {
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            //  GL.MatrixMode(MatrixMode.Projection);
            //  GL.LoadIdentity();
            //  GL.Ortho(0, Width, Height, 0, -1, 1);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0f, 1f); GL.Vertex2(-viewWidth * 0.5f + position.X, -viewHeight * 0.5f + position.Y);
            GL.TexCoord2(1f, 1f); GL.Vertex2(viewWidth * 0.5f + position.X, -viewHeight * 0.5f + position.Y);
            GL.TexCoord2(1f, 0f); GL.Vertex2(viewWidth * 0.5f + position.X, viewHeight * 0.5f + position.Y);
            GL.TexCoord2(0f, 0f); GL.Vertex2(-viewWidth * 0.5f + position.X, viewHeight * 0.5f + position.Y);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, 0);

        }
        */

        public String MediaPath;

        public int CreateTextureFromFile(String filename)
        {
            int texture = 0;

            if (!loadedTextures.TryGetValue(filename, out texture))
            {

                Bitmap bitmap = new Bitmap(MediaPath + filename);

                GL.GenTextures(1, out texture);
                GL.BindTexture(TextureTarget.Texture2D, texture);

                BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                bitmap.UnlockBits(data);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                loadedTextures.Add(filename, texture);
            }

            return texture;
        }

        protected void SetColor(Color4 color)
        {
            GL.Color4(color.R, color.G, color.B, color.A);
        }


        public void Viewport(int width, int height)
        {
            OpenTK.Graphics.OpenGL.GL.Viewport(new Size(width, height));
        }

        public void OrthoCentered(float width, float height)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            OpenTK.Graphics.OpenGL.GL.Ortho(-width * 0.5f, width * 0.5f, -height * 0.5f, height * 0.5f, -1, 1);
        }

        public void DrawCylinder(Vector2 A, Vector2 B, float radius, Color4 fillColor, Color4 outlineColor)
        {


            Vector2 AB = B - A;
            float ang0 = (float)System.Math.Atan2(AB.Y, AB.X) + (float)MathFunctions.PI / 2.0f;

            const float N = 8;

            if (fillColor.A > 0.0f)
            {
                SetColor(fillColor);
                GL.Begin(PrimitiveType.TriangleFan);
                for (int n = 0; n <= N; n++)
                {
                    float f = ang0 + ((float)n / (float)N) * (float)MathFunctions.PI;
                    Vector2 v = A + radius * new Vector2((float)MathFunctions.Cos(f), (float)MathFunctions.Sin(f));
                    GL.Vertex2(v.X, v.Y);
                }
                for (int n = 0; n <= N; n++)
                {
                    float f = ang0 + (float)MathFunctions.PI + ((float)n / (float)N) * (float)MathFunctions.PI;
                    Vector2 v = B + radius * new Vector2((float)MathFunctions.Cos(f), (float)MathFunctions.Sin(f));
                    GL.Vertex2(v.X, v.Y);
                }
                GL.End();


            }

            if (outlineColor.A > 0.0f)
            {
                SetColor(outlineColor);
                GL.Begin(PrimitiveType.LineLoop);
                for (int n = 0; n <= N; n++)
                {
                    float f = ang0 + ((float)n / (float)N) * (float)MathFunctions.PI;
                    Vector2 v = A + radius * new Vector2((float)MathFunctions.Cos(f), (float)MathFunctions.Sin(f));
                    GL.Vertex2(v.X, v.Y);
                }
                for (int n = 0; n <= N; n++)
                {
                    float f = ang0 + (float)MathFunctions.PI + ((float)n / (float)N) * (float)MathFunctions.PI;
                    Vector2 v = B + radius * new Vector2((float)MathFunctions.Cos(f), (float)MathFunctions.Sin(f));
                    GL.Vertex2(v.X, v.Y);
                }
                GL.End();
            }



        }

        public void DrawBox(Box2 box, Color4 fillColor, Color4 outlineColor)
        {
            if (fillColor.A > 0.0f)
            {
                SetColor(fillColor);
                GL.Begin(PrimitiveType.Quads);

                GL.Vertex2(box.Left, box.Bottom);
                GL.Vertex2(box.Right, box.Bottom);
                GL.Vertex2(box.Right, box.Top);
                GL.Vertex2(box.Left, box.Top);

                GL.End();
            }

            if (outlineColor.A > 0.0f)
            {
                SetColor(outlineColor);
                GL.Begin(PrimitiveType.LineLoop);

                GL.Vertex2(box.Left, box.Bottom);
                GL.Vertex2(box.Right, box.Bottom);
                GL.Vertex2(box.Right, box.Top);
                GL.Vertex2(box.Left, box.Top);

                GL.End();
            }
        }

        public void DrawCircle(Vector2 center, float radius, Color4 fillColor, Color4 outlineColor)
        {
            const int numCircleVerts = 16;

            if (fillColor.A > 0.0f)
            {
                SetColor(fillColor);
                GL.Begin(PrimitiveType.TriangleFan);
                for (int i = 0; i < numCircleVerts; i++)
                {
                    Vector2 v = center + radius * new Vector2((float)MathFunctions.Cos(2.0f * MathFunctions.PI * (float)i / (float)numCircleVerts), (float)MathFunctions.Sin(2.0f * MathFunctions.PI * (float)i / (float)numCircleVerts));
                    GL.Vertex2(v.X, v.Y);
                }
                GL.End();
            }

            if (outlineColor.A > 0.0f)
            {
                SetColor(outlineColor);
                GL.Begin(PrimitiveType.LineLoop);
                for (int i = 0; i < numCircleVerts; i++)
                {
                    Vector2 v = center + radius * new Vector2((float)MathFunctions.Cos(2.0f * MathFunctions.PI * (float)i / (float)numCircleVerts), (float)MathFunctions.Sin(2.0f * MathFunctions.PI * (float)i / (float)numCircleVerts));
                    GL.Vertex2(v.X, v.Y);
                }
                GL.End();
            }
        }

        public void DrawLine(Vector2 A, Vector2 B, Color4 color)
        {
            SetColor(color);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(A.X, A.Y);
            GL.Vertex2(B.X, B.Y);
            GL.End();
        }

        public void DrawVector2(Vector2 tail, Vector2 head, Color4 color) // TODO: scale this correctly
        {
            Vector2 v = head - tail;
            v *= 2.0f;
            head = v + tail;

            Rotation2 rotation1 = new Rotation2(0.4f);
            Rotation2 rotation2 = new Rotation2(-0.4f);
            Vector2 dir1, dir2;
            dir1 = rotation1 * (tail - head) * 0.1f;
            dir2 = rotation2 * (tail - head) * 0.1f;

            SetColor(color);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(tail.X, tail.Y);
            GL.Vertex2(head.X, head.Y);

            GL.Vertex2(head.X, head.Y);
            GL.Vertex2(head.X + dir1.X, head.Y + dir1.Y);

            GL.Vertex2(head.X, head.Y);
            GL.Vertex2(head.X + dir2.X, head.Y + dir2.Y);

            GL.End();

        }

        

        public void DrawTexturedQuad(int TextureID, Box2 Box)
        {
            GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.BindTexture(TextureTarget.Texture2D, TextureID);

            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(Box.Left, Box.Bottom);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(Box.Right, Box.Bottom);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(Box.Right, Box.Top);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(Box.Left, Box.Top);

            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void DrawTexturedQuad(int TextureID, Box2 Box, Color4 color, bool flipX)
        {
            SetColor(color);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.BindTexture(TextureTarget.Texture2D, TextureID);

            GL.Begin(PrimitiveType.Quads);
            if (!flipX)
            {
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(Box.Left, Box.Bottom);
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(Box.Right, Box.Bottom);
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(Box.Right, Box.Top);
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(Box.Left, Box.Top);
            }
            else
            {
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(Box.Left, Box.Bottom);
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(Box.Right, Box.Bottom);
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(Box.Right, Box.Top);
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(Box.Left, Box.Top);
            }
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void DrawTexturedQuad(int TextureID, Box2 Box, Box2 subImageBox)
        {
            GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.BindTexture(TextureTarget.Texture2D, TextureID);

            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(subImageBox.Left, subImageBox.Top); GL.Vertex2(Box.Left, Box.Bottom);
            GL.TexCoord2(subImageBox.Right, subImageBox.Top); GL.Vertex2(Box.Right, Box.Bottom);
            GL.TexCoord2(subImageBox.Right, subImageBox.Bottom); GL.Vertex2(Box.Right, Box.Top);
            GL.TexCoord2(subImageBox.Left, subImageBox.Bottom); GL.Vertex2(Box.Left, Box.Top);

            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Clear()
        {
            GL.ClearColor(ClearColor.R, ClearColor.G, ClearColor.B, ClearColor.A);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;

            GL.Viewport(0, 0, Width, Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
        }


        public Color4 ClearColor { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        Dictionary<String, int> loadedTextures;
    }
}
