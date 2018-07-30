using System;
using System.Collections.Generic;
using AirHockey;

namespace AirHockey
{

    enum Collider
    {
        Paddle,
        Table,
    }


    class Shape
    {
    }

    class Circle : Shape
    {
        Vector2 pos;
        float r;
    }

    class AAHalfSpace : Shape
    {
        enum DirOpen { PosX, PosY, NegX, NegY };
        float coord;
    }

    class Segment : Shape
    {
        Vector2 A, B;
    }



    class Collision
    {
        bool Test(Shape a, Shape b)
        {
            if ( a is Circle  && b is Circle )
            {

            }


            return false;
        }

        bool Collide(Shape a, Shape b)//, ContactInfo info)
        {
            if ( a is Circle || b is Circle )
            {
                return true;
            }
            return false;
        }
    }




    class Physics
    {
        public struct Contact
        {
            public Vector2 normal;
            public float restitution;
            public Vector2 relativeVelocity;
            public Collider collider;
        }

        public event EventHandler<Contact> Collision;


        public int CheckForScore(GameFrame gameFrame)
        {
            Body puck = gameFrame.Puck;

            if (puck.position.Y - Constants.puckRadius > Constants.tableHeight * 0.5f) // let the puck get inside the goal before showing the score.
            {
                return 0;
            }
            else if (puck.position.Y + Constants.puckRadius < -Constants.tableHeight * 0.5f) // let the puck get inside the goal before showing the score.
            {
                return 1;
            }


            return -1;
        }

        public void Step(GameFrame gameFrame, float deltaTime)
        {

            Contact contact;
            float time;
            int numIterations = 0; // to prevent infinite loops when squeezed
            int maxIterations = 10;

            ApplyTurbulence(gameFrame, deltaTime);
            ClampVelocity(gameFrame);

            while (FindFirstContact(gameFrame.Paddles, gameFrame.Puck, deltaTime, out contact, out time) && 
                numIterations < maxIterations)
            {
                deltaTime -= time * 0.999f;
                Integrate(gameFrame, time * 0.999f); //TODO: should ensure GapEpsilon seperation, this is arbitrary
                ResolveContact(gameFrame.Puck, contact);

                numIterations++;
            }

            // integrate the rest of the frame
            Integrate(gameFrame, deltaTime);

            ClampPaddlePosition(gameFrame.Paddles, 0);
            ClampPaddlePosition(gameFrame.Paddles, 1);

            PositionCorrection(gameFrame.Puck, gameFrame.Paddles);

            // apply drag
            gameFrame.Puck.velocity *= Constants.puckDrag;
        }


        const float GapElsilon = 0.0001f;

        void ApplyTurbulence(GameFrame gameFrame, float deltaTime)
        {   //TODO: temp, make good
            Random rand = new Random();
            int i = rand.Next(0, 100);
            float a = 3.14f * 2 * (float)i / 100.0f;
            Vector2 turbulence = new Vector2(MathFunctions.Cos(a), MathFunctions.Sin(a)) * deltaTime * 2;

            gameFrame.Puck.velocity += turbulence;
        }


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

        bool SweepCircles(Vector2 pA, float rA, Vector2 vA, Vector2 pB, float rB, Vector2 vB, float deltaTime,
            out float contactTime, out Vector2 normal)
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
            //solve for t in p + v*t +- r = edge

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



        bool FindFirstContact(Body[] paddles, Body puck, float deltaTime, out Contact contact, out float firstContactTime)
        {
            contact = new Contact();
            firstContactTime = float.PositiveInfinity;

            float contactTime;
            Vector2 normal;

            for (int i = 0; i < 2; i++)
            {
                if (SweepCircles(
                    puck.position, Constants.puckRadius, puck.velocity, 
                    paddles[i].position, Constants.paddleRadius, paddles[i].velocity, 
                    deltaTime, out contactTime, out normal))
                {
                    if (contactTime < firstContactTime)
                    {
                        firstContactTime = contactTime;
                        contact.normal = normal;
                        contact.restitution = Constants.paddleRestitution;
                        contact.relativeVelocity = puck.velocity - paddles[1].velocity;
                        contact.collider = Collider.Paddle;
                    }
                }
            }

            
            if(SweepCircleEdge(puck.position.X, Constants.puckRadius, puck.velocity.X, -Constants.tableWidth * 0.5f, 
                deltaTime, out contactTime))
            {
                if(contactTime < firstContactTime)
                {
                    contact.normal = new Vector2(1, 0);
                    firstContactTime = contactTime;
                    contact.collider = Collider.Table;
                }
            }

            if (SweepCircleEdge(puck.position.X, Constants.puckRadius, puck.velocity.X, Constants.tableWidth * 0.5f, 
                deltaTime, out contactTime))
            {
                if (contactTime < firstContactTime)
                {
                    contact.normal = new Vector2(-1, 0);
                    firstContactTime = contactTime;
                    contact.collider = Collider.Table;
                }
            }

            
            // see if the the puck is going to hit a goal, if so don't sweep
          
            float t = (-Constants.tableHeight * 0.5f + Constants.puckRadius - puck.position.Y) / puck.velocity.Y;
            float x = puck.position.X + puck.velocity.X * t;
            if (x > Constants.goalWidth * 0.5f - Constants.puckRadius || x < -Constants.goalWidth * 0.5f + Constants.puckRadius)
            {
                if (SweepCircleEdge(puck.position.Y, Constants.puckRadius, puck.velocity.Y,
                    -Constants.tableHeight * 0.5f, deltaTime, out contactTime))
                {
                    if (contactTime < firstContactTime)
                    {
                        contact.normal = new Vector2(0, 1);
                        firstContactTime = contactTime;
                        contact.collider = Collider.Table;
                    }
                }
            }

            t = (Constants.tableHeight * 0.5f - Constants.puckRadius - puck.position.Y) / puck.velocity.Y;
            x = puck.position.X + puck.velocity.X * t;

            if (x > Constants.goalWidth * 0.5f - Constants.puckRadius || x < -Constants.goalWidth * 0.5f + Constants.puckRadius)
            {
                if (SweepCircleEdge(puck.position.Y, Constants.puckRadius, puck.velocity.Y, 
                    Constants.tableHeight * 0.5f, deltaTime, out contactTime))
                {
                    if (contactTime < firstContactTime)
                    {
                        contact.normal = new Vector2(0, -1);
                        firstContactTime = contactTime;
                        contact.collider = Collider.Table;
                    }
                }
            }

            if (firstContactTime <= deltaTime)   // found a contact
            {
                if (contact.collider == Collider.Table)
                {
                    contact.restitution = Constants.tableRestitution;
                    contact.relativeVelocity = puck.velocity;
                }

                Collision?.Invoke(this, contact);
                return true;
            }



            return false;
        }

        void ClampPaddlePosition(Body[] paddles, int player) //player 0 is on bottom, player 1 on top 
        {
            float radius = Constants.paddleRadius;

            if (player == 0)
            {
                if (paddles[player].position.Y - radius * Constants.centerLineOverlap > 0)
                {
                    paddles[player].position.Y = radius * Constants.centerLineOverlap + GapElsilon;
                }
            }
            if (player == 1)
            {
                if (paddles[player].position.Y + radius * Constants.centerLineOverlap < 0)
                {
                    paddles[player].position.Y = -radius * Constants.centerLineOverlap - GapElsilon;
                }
            }

            if (paddles[player].position.X + radius > Constants.tableWidth * 0.5f)
            {
                paddles[player].position.X = Constants.tableWidth * 0.5f - radius - GapElsilon;
            }
            if (paddles[player].position.X - radius < -Constants.tableWidth * 0.5f)
            {
                paddles[player].position.X = -Constants.tableWidth * 0.5f + radius + GapElsilon;
            }
            if (paddles[player].position.Y + radius > Constants.tableHeight * 0.5f)
            {
                paddles[player].position.Y = Constants.tableHeight * 0.5f - radius - GapElsilon;
            }
            if (paddles[player].position.Y - radius < -Constants.tableHeight * 0.5f)
            {
                paddles[player].position.Y = -Constants.tableHeight * 0.5f + radius + GapElsilon;
            }
        }

        
        void ClampVelocity(GameFrame gameFrame)
        {
            if (gameFrame.Puck.velocity.GetLength() > Constants.maxPuckSpeed)
            {
                gameFrame.Puck.velocity = Constants.maxPuckSpeed * gameFrame.Puck.velocity / gameFrame.Puck.velocity.GetLength();
            }
        }

        void Integrate(GameFrame gameFrame, float deltaTime)
        {
            foreach(Body body in gameFrame.Bodies)
            {
                body.position += body.velocity * deltaTime;
            }
        }

        void ResolveContact(Body body, Contact contact)
        {
            // since the paddle is treated as almost infinite mass, 
            //the impules equations just becomes the reflection equation with some restitution.
            float j = -(1.0f + contact.restitution) * Vector2.Dot(contact.relativeVelocity, contact.normal);

            if (j > 0) // never move towards the collison object
            {
                body.velocity += contact.normal * j;
            }
        }


        void PositionCorrection(Body puck, Body[] paddles)
        {

            PaddlePositionCorrection(puck, paddles[0]);
            PaddlePositionCorrection(puck, paddles[1]);
            TablePositionCorrection(puck);
        }

        bool PaddlePositionCorrection(Body puck, Body paddle)
        {
            Vector2 diff = puck.position - paddle.position;
            float sumRadii = Constants.paddleRadius + Constants.puckRadius;
            if (diff.GetLengthSquared() < sumRadii * sumRadii)
            {
                float length = diff.GetLength();
                Contact contact = new Contact();
                contact.collider = Collider.Paddle;
                contact.relativeVelocity = puck.velocity - paddle.velocity;
                contact.restitution = Constants.paddleRestitution;
                contact.normal = diff / length; 

                // position correction, move out of overlap
                puck.position += contact.normal * (sumRadii - length + GapElsilon); 
                
                ResolveContact(puck, contact);
                Collision?.Invoke(this, contact);
                return true;
            }

            return false;
        }

        public void TablePositionCorrection(Body puck)
        {

            if (puck.position.X + Constants.puckRadius > Constants.tableWidth * 0.5f)
                puck.position.X = Constants.tableWidth * 0.5f - Constants.puckRadius - GapElsilon;

            if (puck.position.X - Constants.puckRadius < -Constants.tableWidth * 0.5f)
                puck.position.X = -Constants.tableWidth * 0.5f + Constants.puckRadius + GapElsilon;


            if (puck.position.Y + Constants.puckRadius > Constants.tableHeight * 0.5f)
                if (puck.position.X > Constants.goalWidth * 0.5f - Constants.puckRadius ||
                    puck.position.X < -Constants.goalWidth * 0.5f + Constants.puckRadius)
                    puck.position.Y = Constants.tableHeight * 0.5f - Constants.puckRadius - GapElsilon;


            if (puck.position.Y - Constants.puckRadius < -Constants.tableHeight * 0.5f)
                if (puck.position.X > Constants.goalWidth * 0.5f - Constants.puckRadius ||
                    puck.position.X < -Constants.goalWidth * 0.5f + Constants.puckRadius)
                    puck.position.Y = -Constants.tableHeight * 0.5f + Constants.puckRadius + GapElsilon;
        }
    }
}
