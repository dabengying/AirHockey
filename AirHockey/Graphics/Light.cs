using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Graphics
{

    public class Light
    {
        public Vector2 Position;
        public Color4 Color;

        // The intensity is 1/(k*dist^P + 1)
        public float K;
        public float P;
    }
    

    
     public struct Line
     {
         public Vector2 Point;
         public Vector2 Direction;
     }

    

}
