using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLib
{
    public struct Rotation2
    {

        public Rotation2(float angle)
        {
            c = MathFunctions.Cos(angle);
            s = MathFunctions.Sin(angle);
        }
        public Rotation2(float Cos, float Sin)
        {
            c = Cos;
            s = Sin;
        }

        public static Vector2 operator *(Rotation2 R, Vector2 v)
        {
            return new Vector2(v.X * R.c - v.Y * R.s, v.X * R.s + v.Y * R.c);
        }

        public static Rotation2 operator *(Rotation2 a, Rotation2 b)
        {
            return new Rotation2(a.c * b.c - a.s * b.s, a.s * b.c + a.c * b.s);
        }

        public static Rotation2 operator-(Rotation2 r)
        {
            return new Rotation2(-r.c, -r.s);
        }
        public float Angle()
        {
            return (float)System.Math.Atan2(s, c);
        }

        float c, s;
    }
}
