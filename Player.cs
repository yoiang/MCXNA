using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XNAUtility;

namespace MCXNA
{
    public class Player : Camera
    {
        const float CONVERT_TO_RADIANS = 3.1415f / 180.0f;
        public Vector3 mVelocity;
        public Vector3 mRotationVelocity;

        public Vector3 mAcceleration;
        public Vector3 mRotationAcceleration;

        public float mMaxSpeed;
        public float mMaxRotationSpeed;

        public Player( Vector3 setLocation, Vector3 setDirection )
            : base( "Player", 0.1f, 1000.0f, ( float )GameArgs.Current.Game.Window.ClientBounds.Width / ( float )GameArgs.Current.Game.Window.ClientBounds.Height, setLocation, setDirection )
        {
            mVelocity = Vector3.Zero;
            mRotationVelocity = Vector3.Zero;

            mAcceleration = Vector3.Zero;
            mRotationAcceleration = Vector3.Zero;

            mMaxSpeed = 10;
            mMaxRotationSpeed = 5;
        }

        public bool handleInput(KeyboardState KeyboardState, GameTime GameTime)
        {
            float elapsedTime = ( float )GameTime.ElapsedGameTime.TotalSeconds;

            if( KeyboardState.IsKeyDown( Keys.S ) )
            {
                mAcceleration.Z -= elapsedTime;
            }
            if( KeyboardState.IsKeyDown( Keys.W ) )
            {
                mAcceleration.Z += elapsedTime;
            }

            if( KeyboardState.IsKeyDown( Keys.A ) )
            {
                mVelocity.X += elapsedTime;
            }

            if( KeyboardState.IsKeyDown( Keys.D ) )
            {
                mVelocity.X -= elapsedTime;
            }

            if( KeyboardState.IsKeyDown( Keys.Left ) )
            {
                mRotationVelocity.Y += MathHelper.ToRadians( -elapsedTime );
            }

            if( KeyboardState.IsKeyDown( Keys.Right ) )
            {
                mRotationVelocity.Y += MathHelper.ToRadians( elapsedTime );
            }

            if( KeyboardState.IsKeyDown( Keys.Down ) )
            {
                mRotationVelocity.X += MathHelper.ToRadians( elapsedTime );
            }

            if( KeyboardState.IsKeyDown( Keys.Up ) )
            {
                mRotationVelocity.X += MathHelper.ToRadians( -elapsedTime );
            }

            if( KeyboardState.IsKeyDown( Keys.Space ) )
            {
                mAcceleration = Vector3.Zero;
                mRotationAcceleration = Vector3.Zero;
                mVelocity = Vector3.Zero;
                mRotationVelocity = Vector3.Zero;
            } 
            
            return false;
        }

        public override void Update(GameTime GameTime)
        {
            base.Update(GameTime);
            
            mVelocity += mAcceleration;
            float Speed = ( float )Math.Sqrt( Math.Pow( mVelocity.X, 2 ) + Math.Pow( mVelocity.Y, 2 ) + Math.Pow( mVelocity.Z, 2 ) );
            if( Speed > mMaxSpeed )
            {
                float scaleBy = mMaxSpeed / Speed;
                mVelocity *= scaleBy;
            }

            mRotationVelocity += mRotationAcceleration;
            Speed = ( float )Math.Sqrt( Math.Pow( mRotationVelocity.X, 2 ) + Math.Pow( mRotationVelocity.Y, 2 ) + Math.Pow( mRotationVelocity.Z, 2 ) );
            if( Speed > mMaxRotationSpeed )
            {
                float scaleBy = mMaxRotationSpeed / Speed;
                mRotationVelocity *= scaleBy;
            }

            Rotation += mRotationVelocity;

            Vector3 Forward = Vector3.Normalize( new Vector3( ( float )Math.Sin( -Rotation.Y ), ( float )Math.Sin( Rotation.X ), ( float )Math.Cos( -Rotation.Y ) ) );
            Vector3 Left = Vector3.Normalize( new Vector3( ( float )Math.Cos( Rotation.Y ), 0.0f, ( float )Math.Sin( Rotation.Y ) ) );

            Location += Forward * mVelocity.Z;
            Location += Left * mVelocity.X;
       }

/*        public bool handleKeyUp(KeyboardEventArgs Input, float DeltaTime)
        {
            switch (Input.Key)
            {
                case Keys.S:
                    mAcceleration.Z += 0.1f;
                    //                    Location[2] += 10;
                    break;
                case Keys.W:
                    mAcceleration.Z -= 0.1f;
                    //                    Location[2] -= 10;
                    break;
                case Keys.A:
                    //                    Location[0] -= 10;
                    break;
                case Keys.D:
                    //                    Location[0] += 10;
                    break;
                default:
                    return false;
            }
            return true;
        }*/

        /*
        public bool handleMouseMotion(MouseMotionEventArgs Input, float DeltaTime)
        {
            if (Direction.X - Input.RelativeY > -80.0 && Direction.X - Input.RelativeY < 80.0)
            {
                Direction.X -= Input.RelativeY;
            }
            Direction.Y -= Input.RelativeX;
            return true;
        }*/
    }
}
