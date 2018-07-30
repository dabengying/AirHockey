using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace AirHockey.Graphics {

    public class Visual
    {
        public Visual(Geometry geometry, Effect effect)
        {
            Geometry = geometry;
            Effect = effect;
            WorldMatrix = Matrix3.Identity;
        }

        public Geometry Geometry;
        public Effect Effect;
        public Matrix3 WorldMatrix;
    }
}
