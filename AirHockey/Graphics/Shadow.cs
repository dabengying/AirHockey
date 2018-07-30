using System;
using OpenTK;

namespace AirHockey.Graphics
{
    public class Shadow
    {
        const float infty = 100;
        float numbraFactor = 0.6f;
        const float shadowAlpha = 0.20f;
        const float shadowEdgeAlpha = shadowAlpha / 2.0f;

        public Shadow(Vector2 lightPosition, Vector2 circlePosition, float circleRadius)
        {


            Lines = TangentLineToCirleThroughPoint(lightPosition, circlePosition, circleRadius);
            if (Lines.Length >= 2)
            {
                Vertices = new Vector2[18];
                Colors = new Color4[18];

                Lines[0].Direction.Normalize();
                Lines[1].Direction.Normalize();
                Vector2 D = Lines[1].Point - Lines[0].Point;

                Line[] modifiedLines = new Line[2];
                modifiedLines[0].Point = Lines[0].Point + (1.0f - numbraFactor) * D;
                modifiedLines[0].Direction = Lines[0].Direction;
                modifiedLines[1].Point = Lines[1].Point - (1.0f - numbraFactor) * D;
                modifiedLines[1].Direction = Lines[1].Direction;

                CreateUmbraTrapeziod(modifiedLines, infty, 0);

                float angle = (float)Math.Acos(Vector2.Dot(Lines[0].Direction, Lines[1].Direction)) * 1.5f;
                if (circlePosition.X > lightPosition.X)
                    angle = -angle;
               

                Line outsideLine = new Line();
                outsideLine.Point = Lines[0].Point;
                outsideLine.Direction = new Rotation2(angle) * Lines[0].Direction;
                Line insideLine = new Line();
                insideLine.Point = modifiedLines[0].Point;
                insideLine.Direction = Lines[0].Direction;

                CreatePnumbraTrapeziod(insideLine, outsideLine, infty, 6);

                outsideLine.Point = Lines[1].Point;
                outsideLine.Direction = new Rotation2(-angle) * Lines[1].Direction;
                insideLine.Point = modifiedLines[1].Point;
                insideLine.Direction = modifiedLines[1].Direction;

                CreatePnumbraTrapeziod(insideLine, outsideLine, infty, 12);3
            }

        }

        void CreateUmbraTrapeziod(Line[] sides, float height, int start)
        {
            Vertices[start + 0] = sides[0].Point;
            Vertices[start + 1] = sides[1].Point;
            Vertices[start + 2] = sides[1].Point - sides[1].Direction * height;
            Vertices[start + 3] = sides[0].Point;
            Vertices[start + 4] = sides[1].Point - sides[1].Direction * height;
            Vertices[start + 5] = sides[0].Point - sides[0].Direction * height;

            Colors[start + 0] = new Color4(0, 0, 0, shadowAlpha);
            Colors[start + 1] = new Color4(0, 0, 0, shadowAlpha);
            Colors[start + 2] = new Color4(0, 0, 0, shadowEdgeAlpha);
            Colors[start + 3] = new Color4(0, 0, 0, shadowAlpha);
            Colors[start + 4] = new Color4(0, 0, 0, shadowEdgeAlpha);
            Colors[start + 5] = new Color4(0, 0, 0, shadowEdgeAlpha);
        }

        void CreatePnumbraTrapeziod(Line insideLine, Line outsideLine, float height, int start)
        {
            Vertices[start + 0] = insideLine.Point;
            Vertices[start + 1] = insideLine.Point - insideLine.Direction * height;
            Vertices[start + 2] = outsideLine.Point;
            Vertices[start + 3] = outsideLine.Point;
            Vertices[start + 4] = outsideLine.Point - outsideLine.Direction * height;
            Vertices[start + 5] = insideLine.Point - insideLine.Direction * height;

            Colors[start + 0] = new Color4(0, 0, 0, shadowAlpha);
            Colors[start + 1] = new Color4(0, 0, 0, shadowEdgeAlpha);
            Colors[start + 2] = new Color4(0, 0, 0, 0.0f);
            Colors[start + 3] = new Color4(0, 0, 0, 0.0f);
            Colors[start + 4] = new Color4(0, 0, 0, 0.0f);
            Colors[start + 5] = new Color4(0, 0, 0, shadowEdgeAlpha);
        }

        public Line[] Lines;

        public Vector2[] Vertices;
        public Color4[] Colors;

        Line[] TangentLineToCirleThroughPoint(Vector2 p, Vector2 c, float r)
        {
            float dCP2 = (c - p).GetLengthSquared();
            float r2 = r * r;
            if (dCP2 < r2)
                return new Line[0];
            else if (dCP2 == r2)
            {
                Vector2 u = p - c;
                Line L = new Line();
                L.Point = p;
                L.Direction = u.PerpCCW();
                return new Line[1] { L };
            }
            else
            {
                Vector2 u = p - c;
                float ux2 = u.X * u.X;
                float ux4 = ux2 * ux2;
                float uy2 = u.Y * u.Y;
                float r4 = r2 * r2;
                float numerator = r2 * uy2;
                float denominator = ux2 + uy2;
                float rad = MathFunctions.Sqrt(-(r4 * ux2) + r2 * ux4 + r2 * ux2 * uy2);
                Vector2 v0 = new Vector2(), v1 = new Vector2();
                v0.X = (r2 - (numerator + u.Y * rad) / denominator) / u.X;
                v0.Y = ((r2 * u.Y) + rad) / denominator;
                v1.X = (r2 - (numerator - u.Y * rad) / denominator) / u.X;
                v1.Y = ((r2 * u.Y) - rad) / denominator;
                Line L0 = new Line(), L1 = new Line();
                L0.Point = c + v0;
                L0.Direction = new Vector2(-v0.Y, v0.X);
                L1.Point = c + v1;
                L1.Direction = new Vector2(v1.Y, -v1.X);
                if (Vector2.Dot((c - p), L0.Direction) > 0)
                    L0.Direction = -L0.Direction;
                if (Vector2.Dot((c - p), L1.Direction) > 0)
                    L1.Direction = -L1.Direction;
                return new Line[2] { L0, L1 };
            }
        }
    }

    

}
