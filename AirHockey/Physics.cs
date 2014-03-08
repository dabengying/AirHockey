using System;
using System.Collections.Generic;
using AirHockey;

namespace AirHockey
{

    enum PhysicsResult
    {
        PuckPaddleCollision,
        PuckTableCollision,
        NoCollision,
        BottomPlayerScores,
        TopPlayerScores
    }

    class Physics
    {
        bool SolveQuadratic(double a, double b, double c, out float t)
        {
            double discriminant = b * b - 4.0f * a * c;
            if (discriminant < 0.0f)
            {
                t = 0;
                return false;
            }
            // always the smaller anwser, the greater would be when the circle passes through
            t = (float) ((-b - Math.Sqrt(discriminant)) / (2.0 * a));
            
            return true;
        }

        bool SweepCircles(Vector2 pA, float rA, Vector2 vA, Vector2 pB, float rB, Vector2 vB, float deltaTime, out float contactTime, out Vector2 normal)
        {
            //solve for when relative_position + relative_velocity * t == sumRadii
            // square both sides and use quadratic formula. 

            Vector2 p = pB - pA;
            Vector2 v = vB - vA;
            float r = rA + rB;

            float t = 0.0f;

            if(SolveQuadratic((double)Vector2.Dot(v,v), 2.0 * (double)Vector2.Dot(v, p), (double)Vector2.Dot(p, p) - (double) r*r, out t))
            {
                if (t > 0.0f && t <= deltaTime)
                {

                    contactTime = t;
                    normal =  (pA + vA * contactTime) - (pB + vB * contactTime);
                    normal.Normalize();
                    return true;
                }
            }

            normal = new Vector2(0, 0);
            contactTime = 0.0f;
            return false;
        }
        
        bool SweepCircleEdge(float p, float r, float v, float edge, float deltaTime, out float contactTime)
        {
            //solve for t for p + v*t +- r = edge

            if(edge > 0) // right or top side 
            {
                contactTime = (edge - p - r) / v;
            }
            else // left or bottom side
            {
                contactTime = (edge - p + r) / v;
            }

            if (contactTime > 0 && contactTime < deltaTime)
                return true;

            return false;
        }

        public struct Contact
        {
            public float time;
            public Vector2 normal;
            public float restitution;
            public Vector2 relativeVelocity;
            public PhysicsResult collisionType;
        }

        bool FindFirstContact(Paddle[] paddles, Puck puck, float deltaTime, out Contact contact, out int paddle)
        {
            contact = new Contact();
            contact.time = float.PositiveInfinity;
            contact.collisionType = PhysicsResult.NoCollision;

            float contactTime;
            Vector2 normal;
            paddle = -1;

            if(SweepCircles(puck.position, Constants.puckRadius, puck.velocity, paddles[0].position, Constants.paddleRadius, paddles[0].velocity, deltaTime, out contactTime, out normal))
            {
                contact.normal = normal;
                contact.time = contactTime;
                contact.restitution = Constants.paddleRestitution;
                contact.relativeVelocity = puck.velocity - paddles[0].velocity;
                contact.collisionType = PhysicsResult.PuckPaddleCollision;
                paddle = 0;
            }
            if (SweepCircles(puck.position, Constants.puckRadius, puck.velocity, paddles[1].position, Constants.paddleRadius, paddles[1].velocity, deltaTime, out contactTime, out normal))
            {
                if (contactTime < contact.time)
                {
                    contact.normal = normal;
                    contact.time = contactTime;
                    contact.restitution = Constants.paddleRestitution;
                    contact.relativeVelocity = puck.velocity - paddles[1].velocity;
                    contact.collisionType = PhysicsResult.PuckPaddleCollision;
                    paddle = 1;
                }
            }

            if(SweepCircleEdge(puck.position.X, Constants.puckRadius, puck.velocity.X, -Constants.tableWidth * 0.5f, deltaTime, out contactTime))
            {
                if(contactTime < contact.time)
                {
                    contact.normal = new Vector2(1, 0);
                    contact.time = contactTime;
                    contact.restitution = Constants.tableRestitution;
                    contact.relativeVelocity = puck.velocity;
                    contact.collisionType = PhysicsResult.PuckTableCollision;
                }
            }

            if (SweepCircleEdge(puck.position.X, Constants.puckRadius, puck.velocity.X, Constants.tableWidth * 0.5f, deltaTime, out contactTime))
            {
                if (contactTime < contact.time)
                {
                    contact.normal = new Vector2(-1, 0);
                    contact.time = contactTime;
                    contact.restitution = Constants.tableRestitution;
                    contact.relativeVelocity = puck.velocity;
                    contact.collisionType = PhysicsResult.PuckTableCollision;
                }
            }

            
            // see if the the puck is going to hit a goal, if so don't sweep
          
            float t = (-Constants.tableHeight * 0.5f + Constants.puckRadius - puck.position.Y) / puck.velocity.Y;
            float x = puck.position.X + puck.velocity.X * t;
            if (x > Constants.goalWidth * 0.5f - Constants.puckRadius || x < -Constants.goalWidth * 0.5f + Constants.puckRadius)
            {
                if (SweepCircleEdge(puck.position.Y, Constants.puckRadius, puck.velocity.Y, -Constants.tableHeight * 0.5f, deltaTime, out contactTime))
                {
                    if (contactTime < contact.time)
                    {
                        contact.normal = new Vector2(0, 1);
                        contact.time = contactTime;
                        contact.restitution = Constants.tableRestitution;
                        contact.relativeVelocity = puck.velocity;
                        contact.collisionType = PhysicsResult.PuckTableCollision;
                    }
                }
            }

            t = (Constants.tableHeight * 0.5f - Constants.puckRadius - puck.position.Y) / puck.velocity.Y;
            x = puck.position.X + puck.velocity.X * t;

            if (x > Constants.goalWidth * 0.5f - Constants.puckRadius || x < -Constants.goalWidth * 0.5f + Constants.puckRadius)
            {
                if (SweepCircleEdge(puck.position.Y, Constants.puckRadius, puck.velocity.Y, Constants.tableHeight * 0.5f, deltaTime, out contactTime))
                {
                    if (contactTime < contact.time)
                    {
                        contact.normal = new Vector2(0, -1);
                        contact.time = contactTime;
                        contact.restitution = Constants.tableRestitution;
                        contact.relativeVelocity = puck.velocity;
                        contact.collisionType = PhysicsResult.PuckTableCollision;
                    }
                }
            }

            if (contact.time <= deltaTime)
                return true;


            return false;
        }

        void ClampPaddlePosition(Paddle[] paddles, int player) //player 0 is on bottom, player 1 on top 
        {
            float radius = Constants.paddleRadius;
            float epsilon = 0.0001f;

            if (player == 0)
            {
                if (paddles[player].position.Y - radius * Constants.centerLineOverlap > 0)
                {
                    paddles[player].position.Y = radius * Constants.centerLineOverlap + epsilon;
                    if (paddles[player].velocity.Y > 0)
                        paddles[player].velocity.Y = 0;

                }
            }
            if (player == 1)
            {
                if (paddles[player].position.Y + radius * Constants.centerLineOverlap < 0)
                {
                    paddles[player].position.Y = -radius * Constants.centerLineOverlap - epsilon;
                    if (paddles[player].velocity.Y < 0)
                        paddles[player].velocity.Y = 0;
                }
            }
            if (paddles[player].position.X + radius > Constants.tableWidth * 0.5f)
            {
                paddles[player].position.X = Constants.tableWidth * 0.5f - radius + epsilon;
                if (paddles[player].velocity.X > 0)
                    paddles[player].velocity.X = 0;
            }
            if (paddles[player].position.X - radius < -Constants.tableWidth * 0.5f)
            {
                paddles[player].position.X = -Constants.tableWidth * 0.5f + radius - epsilon;
                if (paddles[player].velocity.X < 0)
                    paddles[player].velocity.X = 0;
            }
            if (paddles[player].position.Y + radius > Constants.tableHeight * 0.5f)
            {
                paddles[player].position.Y = Constants.tableHeight * 0.5f - radius + epsilon;
                if (paddles[player].velocity.Y > 0)
                    paddles[player].velocity.Y = 0;
            }
            if (paddles[player].position.Y - radius < -Constants.tableHeight * 0.5f)
            {
                paddles[player].position.Y = -Constants.tableHeight * 0.5f + radius - epsilon;
                if (paddles[player].velocity.Y < 0)
                    paddles[player].velocity.Y = 0;
            }
        }

        public PhysicsResult Update(Puck puck, Paddle[] paddles, float deltaTime)
        {
            PhysicsResult result = PhysicsResult.NoCollision;

            ClampPaddlePosition(paddles, 0);
            ClampPaddlePosition(paddles, 1);

            const float epsilon = 0.9990f; // don't actually touch
            Contact contact;
            int numIterations = 0; // to prevent infinite loops when squeezed
            int paddle;

            if (puck.velocity.GetLength() > Constants.maxPuckSpeed)
            {
                puck.velocity = Constants.maxPuckSpeed * puck.velocity / puck.velocity.GetLength();
            }

            while(FindFirstContact(paddles, puck, deltaTime, out contact, out paddle) && numIterations < 10)
            {
                // since the paddle is treated as almost infinite mass, the impules equations just becomes the reflection equation with some restitution.

                result = contact.collisionType;

                deltaTime -= contact.time;
                puck.position += puck.velocity * (contact.time * epsilon);

                
                paddles[0].position += paddles[0].velocity * (contact.time * epsilon);
                paddles[1].position += paddles[1].velocity * (contact.time * epsilon);

                if(paddle != -1) // TODO: keep this? prevents all overlap except is cases where the puck is squeezed between 2 objects.
                    paddles[paddle].velocity = new Vector2(0, 0);

                float j = -(1.0f + contact.restitution) * Vector2.Dot(contact.relativeVelocity, contact.normal);

                if (j > 0) // never move towards the collison object, should never happen but just in case.
                {
                    puck.velocity += contact.normal * j;
                }

                if (puck.velocity.GetLength() > Constants.maxPuckSpeed)
                {
                    puck.velocity = Constants.maxPuckSpeed * puck.velocity / puck.velocity.GetLength();
                }

                numIterations++;
            }
            
            puck.position += puck.velocity * deltaTime;

            int p = PositionCorrection(puck, paddles);
            if (p == 0)
                return PhysicsResult.BottomPlayerScores;
            if (p == 1)
                return PhysicsResult.TopPlayerScores;
            
            paddles[0].position += paddles[0].velocity * deltaTime;
            paddles[1].position += paddles[1].velocity * deltaTime;

            return result;
        }


        public bool PaddlePositionCorrection(int player, Puck puck, Paddle[] paddles)
        {
            Vector2 diff = puck.position - paddles[player].position;
            float sumRadii = Constants.paddleRadius + Constants.puckRadius;
            if (diff.GetLengthSquared() < sumRadii * sumRadii)
            {
                // since the paddle is treated as almost infinite mass, the impules equations just becomes the reflection equation with some restitution.

                Vector2 normal;

                float length = diff.GetLength();
                if (length > 0.001f)
                {
                    normal = diff / length;
                }
                else
                {
                    normal = new Vector2(0, 0);
                }
                Vector2 relativeVelocity = puck.velocity - paddles[player].velocity;

                float j = -(1.0f + Constants.paddleRestitution) * Vector2.Dot(relativeVelocity, normal);

                puck.position += normal * (sumRadii - length); // position correction, move out of overlap, TODO: add an epsilon?

                if (j > 0) // never move towards the collison object 
                {
                    puck.velocity += normal * j;
                }
                if (puck.velocity.GetLength() > Constants.maxPuckSpeed)
                {
                    puck.velocity = Constants.maxPuckSpeed * puck.velocity / puck.velocity.GetLength();
                }

                return true;
            }

            return false;
        }

        public int PositionCorrection(Puck puck, Paddle[] paddles)
        {
            float epsilon = 0.0001f;
            bool collision;
            // if there is a collision from both the paddle and the table, the puck is being squeezed between the two.
            // the contact get set to the table collision, it's better to have some puck/paddle overlap than to have the puck go off the screen and break the game. 

            //TODO: should I collision correct the puck and prevent all overlap??


            if(PaddlePositionCorrection(0, puck, paddles))
            {
                collision = true;
            }
            if (PaddlePositionCorrection(1, puck, paddles))
            {
                collision = true;
            }

            if (puck.position.X + Constants.puckRadius > Constants.tableWidth * 0.5f)
            {
                puck.position.X = Constants.tableWidth * 0.5f - Constants.puckRadius + epsilon;
                puck.velocity.X = -puck.velocity.X * Constants.tableRestitution;
                collision = true;
            }
            if (puck.position.X - Constants.puckRadius < -Constants.tableWidth * 0.5f)
            {
                puck.position.X = -Constants.tableWidth * 0.5f + Constants.puckRadius - epsilon;
                puck.velocity.X = -puck.velocity.X * Constants.tableRestitution;
                collision = true;
            }


            if (puck.position.Y + Constants.puckRadius > Constants.tableHeight * 0.5f)
            {

                if (puck.position.X > Constants.goalWidth * 0.5f - Constants.puckRadius|| puck.position.X < -Constants.goalWidth * 0.5f + Constants.puckRadius)
                {
                    puck.position.Y = Constants.tableHeight * 0.5f - Constants.puckRadius + epsilon;
                    puck.velocity.Y = -puck.velocity.Y * Constants.tableRestitution;
                    collision = true;
                }

                else if (puck.position.Y - Constants.puckRadius > Constants.tableHeight * 0.5f) // let the puck get inside the goal before showing the score.
                {
                    return 0;
                }
            }
            if (puck.position.Y - Constants.puckRadius < -Constants.tableHeight * 0.5f)
            {

                if (puck.position.X > Constants.goalWidth * 0.5f - Constants.puckRadius || puck.position.X < -Constants.goalWidth * 0.5f + Constants.puckRadius)
                {
                    puck.position.Y = -Constants.tableHeight * 0.5f + Constants.puckRadius - epsilon;
                    puck.velocity.Y = -puck.velocity.Y * Constants.tableRestitution;
                    collision = true;
                }

                else if (puck.position.Y + Constants.puckRadius < -Constants.tableHeight * 0.5f) // let the puck get inside the goal before showing the score.
                {
                    return 1;
                }
            }

            return -1;
        }
    }
}
