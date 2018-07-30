using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace AirHockey.Graphics
{
    public class TextureEffect : Effect
    {
        public TextureEffect(int texture)
            : base(@"..\..\shaders\texture.vsh", @"..\..\shaders\texture.psh") {
            TextureId = texture;
            textureLocation = GL.GetUniformLocation(ProgramId, "tex");
            WorldViewMatrix = Matrix3.Identity;
            worldViewMatrixLocation = GL.GetUniformLocation(ProgramId, "worldViewMatrix");
        }

        public TextureEffect(string textureFile) 
            : base(@"..\..\shaders\texture.vsh", @"..\..\shaders\texture.psh")
        {
            TextureId = ResourceManager.GetTexture(textureFile);
            textureLocation = GL.GetUniformLocation(ProgramId, "tex");
            WorldViewMatrix = Matrix3.Identity;
            worldViewMatrixLocation = GL.GetUniformLocation(ProgramId, "worldViewMatrix");
           
        }

        public override void Begin()
        {
            base.Begin();
            GL.Uniform1(textureLocation, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.UniformMatrix3(worldViewMatrixLocation, true, ref WorldViewMatrix);
        }

        public override void End()
        {
            base.End();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public Matrix3 WorldViewMatrix;
        public int TextureId;

        int textureLocation;
        int worldViewMatrixLocation;
    }
}
