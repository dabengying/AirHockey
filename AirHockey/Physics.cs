using System;
using System.Collections.Generic;
using GameLib;

namespace AirHockey
{
    class Physics
    {
        public void DoPaddleCollision(int player, Puck puck, float puckRadius, Paddle[] paddles, float paddleRadius, float paddleElasticity, float maxPuckSpeed, AI ai)
        {
            Vector2 diff = puck.position - paddles[player].position;
            float sumRadii = paddleRadius + puckRadius;
            if (diff.GetLengthSquared() < sumRadii * sumRadii)
            {
                // since the paddle is treated as almost infinite mass, the impules equations just becomes the reflection equation with some restitution.

                float length = diff.GetLength();
                Vector2 normal = diff / length;
                Vector2 relativeVelocity = puck.velocity - paddles[player].velocity;

                float j = -(1.0f + paddleElasticity) * Vector2.DotProduct(relativeVelocity, normal);

                puck.position += normal * (sumRadii - length); // position correction, move out of overlap, TODO: add an epsilon?

                if (j > 0) // never move towards the collison object 
                {
                    puck.velocity += normal * j;
                }
                if (puck.velocity.GetLength() > maxPuckSpeed)
                {
                    puck.velocity = maxPuckSpeed * puck.velocity / puck.velocity.GetLength();
                }

                if (player == 1)
                    ai.PuckCollision();
            }
        }

        public int DoTableCollision(Puck puck, float puckRadius, Table table, float goalWidth)
        {
            // returns the scoring player, or -1 if no score.

            // if past and edge, correct position and reflect velocity off of normal

            bool collision = false;

            if (puck.position.X + puckRadius > table.width * 0.5f)
            {
                puck.position.X = table.width * 0.5f - puckRadius;
                puck.velocity.X = -puck.velocity.X * table.elasticity;
                collision = true;
            }
            if (puck.position.X - puckRadius < -table.width * 0.5f)
            {
                puck.position.X = -table.width * 0.5f + puckRadius;
                puck.velocity.X = -puck.velocity.X * table.elasticity;
                collision = true;
            }


            if (puck.position.Y + puckRadius > table.height * 0.5f)
            {

                if (puck.position.X > goalWidth * 0.5f || puck.position.X < -goalWidth * 0.5f)
                {
                    puck.position.Y = table.height * 0.5f - puckRadius;
                    puck.velocity.Y = -puck.velocity.Y * table.elasticity;
                    collision = true;
                }

                if (puck.position.Y - puckRadius > table.height * 0.5f) // let the puck get inside the goal before showing the score.
                    return 0;

            }
            if (puck.position.Y - puckRadius < -table.height * 0.5f)
            {

                if (puck.position.X > goalWidth * 0.5f || puck.position.X < -goalWidth * 0.5f)
                {
                    puck.position.Y = -table.height * 0.5f + puckRadius;
                    puck.velocity.Y = -puck.velocity.Y * table.elasticity;
                    collision = true;
                }

                if (puck.position.Y + puckRadius < -table.height * 0.5f) // let the puck get inside the goal before showing the score.
                    return 1;

            }

            return -1;
        }
    }
}
