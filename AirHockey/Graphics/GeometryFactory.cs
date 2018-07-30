using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Graphics
{
    class GeometryFactory
    {

        public static Geometry CreateBox(OpenTK.Box2 box)
        {
            Vector2[] vertices = new Vector2[4] {
                new Vector2(box.Left, box.Bottom),
                new Vector2(box.Left,  box.Top),
                new Vector2(box.Right, box.Top),
                new Vector2(box.Right, box.Bottom)
            };

            Color4[] colors = new Color4[4]
            {
                new Color4(1.0f, 1.0f, 1.0f, 1.0f),
                new Color4(1.0f, 1.0f, 1.0f, 1.0f),
                new Color4(1.0f, 1.0f, 1.0f, 1.0f),
                new Color4(1.0f, 1.0f, 1.0f, 1.0f)
            };

            Vector2[] texCoords = new Vector2[4]
            {
                 new Vector2(0, 1),
                 new Vector2(0, 0),
                 new Vector2(1, 0),
                 new Vector2(1, 1)
            };

            int[] indices = new int[] { 0, 1, 2, 0, 2, 3 };
            Geometry geom = new Geometry(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, vertices, colors, texCoords, indices);
            return geom;

        }

        public static Geometry CreateCircle(float radius, Color4 color)
        {

            const int numCircleVerts = 64;

            Vector2[] Vertices = new Vector2[numCircleVerts + 1];
            Color4[] Colors = new Color4[numCircleVerts + 1];
            Vector2[] TexCoords = new Vector2[numCircleVerts + 1];



            for (int i = 0; i < numCircleVerts; i++)
            {
                float angle = 2.0f * (float)MathFunctions.PI * (float)i / (float)numCircleVerts;
                Vertices[i] = new Vector2(0, 0);
                Vertices[i].X = (float)Math.Cos(angle) * radius;
                Vertices[i].Y = (float)Math.Sin(angle) * radius;
            }
            Vertices[numCircleVerts].X = 0;
            Vertices[numCircleVerts].Y = 0;

            for (int i = 0; i < numCircleVerts; i++)
            {
                Colors[i] = color;
                TexCoords[i].X = 0;
                TexCoords[i].Y = 0;
            }


            int[] indices = new int[numCircleVerts * 3];
            for (int i = 0; i < numCircleVerts; i++)
            {
                indices[i * 3 + 0] = numCircleVerts - 1;
                indices[i * 3 + 1] = i;
                indices[i * 3 + 2] = (i + 1) % numCircleVerts;

            }

            Geometry geom = new Geometry(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, Vertices, Colors, null, indices);
            return geom;

        }
    }
}
