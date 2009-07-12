using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAUtility;

namespace MCXNA
{
    public class Light : Placeable
    {
        public Vector3 DiffuseIntensity;

        public Light(string setName ) : this( setName, Vector3.Zero )
        {
        }
        public Light( string setName, Vector3 setLocation )
            : base( setName, setLocation )
        {
            DiffuseIntensity = Vector3.Zero;
        }

        public virtual void applyLight()
        {
/*            Gl.glEnable(mGLLightName);

            Gl.glLightfv(mGLLightName, Gl.GL_DIFFUSE, mDiffuseIntensity.Value);
            Gl.glLightfv(mGLLightName, Gl.GL_POSITION, mLocation.Value);
*/        }

        public override void Draw(GameTime GameTime)
        {
            base.Draw( GameTime );
            if( Debug )
            {
                GameArgs.Current.GraphicsDevice.VertexDeclaration = new VertexDeclaration( GameArgs.Current.GraphicsDevice, VertexPositionColor.VertexElements );
                
                BasicEffect BasicEffect = new BasicEffect( GameArgs.Current.GraphicsDevice, null );
                GameArgs.Current.setBasicEffectMatrices( BasicEffect );
                BasicEffect.World *= Matrix.CreateTranslation( -Location );
                BasicEffect.World *= Matrix.CreateRotationX( Rotation.X );
                BasicEffect.World *= Matrix.CreateRotationY( Rotation.Y );
                BasicEffect.World *= Matrix.CreateRotationZ( Rotation.Z );

                BasicEffect.Begin();
                BasicEffect.CurrentTechnique.Passes[ 0 ].Begin();

                VertexPositionColor[] vertices = new VertexPositionColor[ 10 ];

                vertices[ 0 ].Position = new Vector3( -5.0f, 5.0f, 5.0f );
                vertices[ 0 ].Color = Color.Red;
                vertices[ 1 ].Position = new Vector3( 5.0f, 5.0f, 5.0f );
                vertices[ 1 ].Color = Color.Red;

                vertices[ 2 ].Position = new Vector3( -5.0f, -5.0f, 5.0f );
                vertices[ 2 ].Color = Color.Red;
                vertices[ 3 ].Position = new Vector3( 5.0f, -5.0f, 5.0f );
                vertices[ 3 ].Color = Color.Red;

                vertices[ 4 ].Position = new Vector3( -5.0f, -5.0f, -5.0f );
                vertices[ 4 ].Color = Color.Red;
                vertices[ 5 ].Position = new Vector3( 5.0f, -5.0f, -5.0f );
                vertices[ 5 ].Color = Color.Red;

                vertices[ 6 ].Position = new Vector3( -5.0f, 5.0f, -5.0f );
                vertices[ 6 ].Color = Color.Red;
                vertices[ 7 ].Position = new Vector3( 5.0f, 5.0f, -5.0f );
                vertices[ 7 ].Color = Color.Red;

                vertices[ 8 ].Position = new Vector3( -5.0f, -5.0f, -5.0f );
                vertices[ 8 ].Color = Color.Red;
                vertices[ 9 ].Position = new Vector3( 5.0f, -5.0f, -5.0f );
                vertices[ 9 ].Color = Color.Red;



                GameArgs.Current.GraphicsDevice.DrawUserPrimitives( PrimitiveType.TriangleStrip, vertices, 0, 5 );

                BasicEffect.CurrentTechnique.Passes[ 0 ].End();
                BasicEffect.End();
            }
            /*
            if (mDebug)
            {
                Gl.glDisable(Gl.GL_LIGHTING);
                Gl.glColor3f(1.0f, 0.0f, 0.0f);
                Gl.glTranslatef(mLocation.X, mLocation.Y, mLocation.Z);
                Gl.glBegin(Gl.GL_QUAD_STRIP);

                Gl.glVertex3f(-5.0f, -5.0f, -5.0f);
                Gl.glVertex3f(5.0f, -5.0f, -5.0f);
                Gl.glVertex3f(-5.0f, 5.0f, -5.0f);
                Gl.glVertex3f(5.0f, 5.0f, -5.0f);

                Gl.glVertex3f(-5.0f, 5.0f, 5.0f);
                Gl.glVertex3f(5.0f, 5.0f, 5.0f);

                Gl.glVertex3f(-5.0f, -5.0f, 5.0f);
                Gl.glVertex3f(5.0f, -5.0f, 5.0f);

                Gl.glVertex3f(-5.0f, -5.0f, -5.0f);
                Gl.glVertex3f(5.0f, -5.0f, -5.0f);

                Gl.glEnd();
//                Gl.glBegin(Gl.GL_QUADS);


  //              Gl.glEnd();
                Gl.glTranslatef(-mLocation.X, -mLocation.Y, -mLocation.Z);
                Gl.glEnable(Gl.GL_LIGHTING);
            }*/
        }

        public override void setDebugData( DebugForm DebugForm )
        {
            if( DebugForm != null )
            {
                DebugForm.setDebugValue( Name + "|Light|Diffuse", DiffuseIntensity.ToString() );
                base.setDebugData( DebugForm );
            }
        }
    }

    public class Sun : Light
    {
        public Vector3 AmbientIntensity;

        public Sun() : this( Vector3.Zero )
        {
        }
        public Sun( Vector3 setLocation )
            : base( "Sun", setLocation )
        {
            AmbientIntensity = Vector3.Zero;
        }

        public void setTime( float Hour )
        {
            if( Hour > 19 || Hour < 5 )
            {
                AmbientIntensity = Vector3.Zero;
                DiffuseIntensity = Vector3.Zero;
            } else if( Hour >= 4 && Hour < 5 )
            {
                AmbientIntensity = Vector3.Zero;
                DiffuseIntensity = new Vector3( 0.9f, 0.9f, 0.9f );
                Rotation = new Vector3( 1.0f, -0.2f, 0.0f );
            } else if( Hour >= 5 && Hour < 11 )
            {
                AmbientIntensity = new Vector3( 0.3f, 0.3f, 0.3f );
                DiffuseIntensity = new Vector3( 0.7f, 0.7f, 0.7f );
                Rotation = new Vector3( 0.5f, -0.5f, 0.0f );
            } else if( Hour >= 11 && Hour < 13 )
            {
                AmbientIntensity = new Vector3( 0.8f, 0.8f, 0.8f );
                DiffuseIntensity = new Vector3( 0.2f, 0.2f, 0.2f );
                Rotation = new Vector3( 0.0f, -1.0f, 0.0f );
            } else if( Hour >= 13 && Hour < 19 )
            {
                AmbientIntensity = new Vector3( 0.3f, 0.3f, 0.3f );
                DiffuseIntensity = new Vector3( 0.7f, 0.7f, 0.7f );
                Rotation = new Vector3( -0.5f, -0.5f, 0.0f );
            } else if( Hour >= 19 && Hour < 20 )
            {
                AmbientIntensity = Vector3.Zero;
                DiffuseIntensity = new Vector3( 0.9f, 0.9f, 0.9f );
                Rotation = new Vector3( -1.0f, -0.2f, 0.0f );
            }
        }

        public override void setDebugData( DebugForm DebugForm )
        {
            if( DebugForm != null )
            {
                DebugForm.setDebugValue( Name + "|Sun|Ambient", AmbientIntensity.ToString() );
                base.setDebugData( DebugForm );
            }
        }

    }
}
