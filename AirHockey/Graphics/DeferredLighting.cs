using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace AirHockey.Graphics
{
    class DeferredLighting
    {
        public DeferredLighting(int width, int height)
        {
            CreateFrameBuffer(width, height);
            screenGeometry = GeometryFactory.CreateBox(new OpenTK.Box2(-1, 1, 1, -1));
            lightEffect = new LightEffect();
            blendEffect = new TextureEffect(textureId);

            
            lightGeometry = GeometryFactory.CreateCircle(1.5f, new Color4(1.0f, 0.0f, 1.0f, 1.0f));

            Light l = new Light();
            l.Position = new Vector2(0, 0);
            l.Radius = 1.5f;
           
            l.Color = new Color4(1.0f, 1, 1, 0.0f);
            lightEffect.Light = l;
            
        }

        void CreateFrameBuffer(int width, int height)
        {
            frameBufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);

            textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height,
                0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)0);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, textureId, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Draw()
        {
            Ambient = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);
            GL.ClearColor(Ambient.R, Ambient.G, Ambient.B, Ambient.A);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            lightEffect.Begin();
            lightEffect.ViewMatrix = Camera.ViewMatrix;
            lightEffect.ViewProjectionMatrix = Camera.ProjectionMatrix * Camera.ViewMatrix;
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            lightGeometry.Draw();

            lightEffect.End();


            Blend();
        }

        void Blend()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            blendEffect.Begin();
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.DstColor, BlendingFactorDest.Zero);
            screenGeometry.Draw();
            blendEffect.End();
        }

        public Camera Camera;

        Color4 Ambient;
        Geometry lightGeometry;
        Geometry screenGeometry;
        LightEffect lightEffect;
        TextureEffect blendEffect;
        int frameBufferId;
        int textureId;
    }
}
