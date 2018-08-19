using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace AirHockey.Graphics
{
    class LightEffect : Effect
    {
        public LightEffect() 
            : base(@"..\..\shaders\light.vsh", @"..\..\shaders\light.psh")
        {
            viewMatrixLocation = GL.GetUniformLocation(ProgramId, "viewMatrix");
            viewProjectionMatrixLocation = GL.GetUniformLocation(ProgramId, "projectionViewMatrix");
            positionLocation = GL.GetUniformLocation(ProgramId, "position");
            radiusLocation = GL.GetUniformLocation(ProgramId, "radius");
            colorLocation = GL.GetUniformLocation(ProgramId, "color");
            ViewMatrix = Matrix3.Identity;
            ViewProjectionMatrix = Matrix3.Identity;
        }

        public override void Begin()
        {
            base.Begin();

            GL.UniformMatrix3(viewMatrixLocation, true, ref ViewMatrix);
            GL.UniformMatrix3(viewProjectionMatrixLocation, true, ref ViewProjectionMatrix);
            GL.Uniform2(positionLocation, Light.Position.X, Light.Position.Y);
            GL.Uniform4(colorLocation, Light.Color.R, Light.Color.G, Light.Color.B, Light.Color.A);
            GL.Uniform1(radiusLocation, Light.Radius);
        }

        public override void End()
        {
            base.End();
        }

        public Matrix3 ViewProjectionMatrix;
        public Matrix3 ViewMatrix;
        public Light Light;

        int viewProjectionMatrixLocation;
        int viewMatrixLocation;
        int positionLocation;
        int radiusLocation;
        int colorLocation;      
    }
}
