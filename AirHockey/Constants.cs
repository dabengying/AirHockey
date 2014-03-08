using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey
{
    class Constants
    {
        //units are based on feet...

        public const float tableWidth = 4.0f; // official 8'x4'
        public const float tableHeight = 8.0f;
        public const float tableRestitution = 0.9f;

        public const float paddleRadius = 0.3f; // official diam 4 1/16 inches
        public const float puckRadius = 0.175f; // official diam 3.25 inches

        public const float paddleRestitution = 0.9f;
        public const float maxPuckSpeed = 16.0f;
        public const float puckDrag = 0.98f; 

        public const float centerLineOverlap = 0.8f; // as a fraction of the paddle radius, technically 1 for official

        public const float goalWidth = tableHeight / 3.0f; // as a fraction of tableWidth
    }
}
