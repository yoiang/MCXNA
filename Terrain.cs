using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAUtility;

namespace MCXNA
{
    class Corner
    {   
        public Vector3 mLocation;
        public Vector3 mNormal;
        public Corner() : this( Vector3.Zero, Vector3.Zero)
        {
        }

        public Corner(Vector3 setLocation)
            : this(setLocation, Vector3.Zero)
        {
        }

        public Corner(Vector3 setLocation, Vector3 setNormal) 
        {
            mLocation = setLocation;
            mNormal = setNormal;
        }
    }
    class Square
    {
        public Corner[,] mCorners;
        public Square[,] mSubdivisions;
        public int mDepth;
        public float mWidth;
        public float[] mAverageHeight;

        public BoundingBox mBoundingBox;

        public Square( int setDepth, float setWidth, Corner[,] setCorners)
        {
            mDepth = setDepth;
            mWidth = setWidth;
            mCorners = new Corner[2, 2];
            mCorners[0, 0] = setCorners[0, 0];
            mCorners[0, 1] = setCorners[0, 1];
            mCorners[1, 0] = setCorners[1, 0];
            mCorners[1, 1] = setCorners[1, 1];
            mAverageHeight = new float[2];

            calcAverageHeight();
            calcNormals();

            mBoundingBox = new BoundingBox();
            calcBoundingBox();
        }

        public void calcAverageHeight()
        {
            mAverageHeight[0] = (mCorners[0, 0].mLocation.Y + mCorners[1, 0].mLocation.Y + mCorners[1, 0].mLocation.Y) / 3.0f;
            mAverageHeight[1] = (mCorners[0, 1].mLocation.Y + mCorners[1, 1].mLocation.Y + mCorners[1, 0].mLocation.Y) / 3.0f;
        }

        public void calcNormals()
        {
            Vector3 Normal1 = Vector3.Cross(mCorners[0, 1].mLocation - mCorners[0, 0].mLocation, mCorners[1, 0].mLocation - mCorners[0,0].mLocation);
            Vector3 Normal2 = Vector3.Cross(mCorners[1,1].mLocation - mCorners[0,1].mLocation, mCorners[1,0].mLocation - mCorners[0,1].mLocation);
            Normal1.Normalize();
            Normal2.Normalize();
            mCorners[0, 0].mNormal = Normal1;
            mCorners[0, 1].mNormal = (Normal1 + Normal2) / 2.0f; 
            mCorners[1, 0].mNormal = (Normal1 + Normal2) / 2.0f;
            mCorners[1, 1].mNormal = Normal2;
            //            Normal[0] = (2 -1) X ( 3 - 1)
        }

        public void calcBoundingBox()
        {
            mBoundingBox.Min = mCorners[ 0, 0 ].mLocation;
            mBoundingBox.Max = mBoundingBox.Min;
            foreach( Corner Corner in mCorners )
            {
                mBoundingBox.Min = Vector3.Min( Corner.mLocation, mBoundingBox.Min );
                mBoundingBox.Max = Vector3.Max( Corner.mLocation, mBoundingBox.Max );
            }
        }

        public bool InsideViewFrustum()
        {
            return ( ( MCGameArgs )GameArgs.Current ).Player.ViewFrustum.Contains( mBoundingBox ) != ContainmentType.Disjoint;
        }

        public void collectVertices( VertexQueue<VertexPositionNormalTexture> VertexQueue, int TestDrawDepth )
        {
           /* if( TestDrawDepth > 0 )
            {
                if( !InsideViewFrustum() )
                {
                    return;
                }
            }*/

            if( mSubdivisions == null )
            {
                VertexQueue.add( new VertexPositionNormalTexture( mCorners[ 0, 0 ].mLocation, mCorners[ 0, 0 ].mNormal, new Vector2( 0, 0 ) ) );
                VertexQueue.add( new VertexPositionNormalTexture( mCorners[ 1, 0 ].mLocation, mCorners[ 1, 0 ].mNormal, new Vector2( 1, 0 ) ) );
                VertexQueue.add( new VertexPositionNormalTexture( mCorners[ 0, 1 ].mLocation, mCorners[ 0, 1 ].mNormal, new Vector2( 0, 1 ) ) );

                VertexQueue.add( new VertexPositionNormalTexture( mCorners[ 1, 0 ].mLocation, mCorners[ 1, 0 ].mNormal, new Vector2( 1, 0 ) ) );
                VertexQueue.add( new VertexPositionNormalTexture( mCorners[ 1, 1 ].mLocation, mCorners[ 1, 1 ].mNormal, new Vector2( 1, 1 ) ) );
                VertexQueue.add( new VertexPositionNormalTexture( mCorners[ 0, 1 ].mLocation, mCorners[ 0, 1 ].mNormal, new Vector2( 0, 1 ) ) );
            } else
            {
                mSubdivisions[ 0, 0 ].collectVertices( VertexQueue, TestDrawDepth - 1 );
                mSubdivisions[ 0, 1 ].collectVertices( VertexQueue, TestDrawDepth - 1 );
                mSubdivisions[ 1, 0 ].collectVertices( VertexQueue, TestDrawDepth - 1 );
                mSubdivisions[ 1, 1 ].collectVertices( VertexQueue, TestDrawDepth - 1 );
            }
        }

        public void Draw(int TestDrawDepth )
        {
            if( TestDrawDepth > 0 )
            {
                if( !InsideViewFrustum() )
                {
                    return;
                }
            }

            if ( mSubdivisions == null )
            {
                VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[ 6 ];

                vertices[0].Position = mCorners[0, 0].mLocation;
                vertices[0].TextureCoordinate = new Vector2(0, 0);
                vertices[ 0 ].Normal = mCorners[ 0, 0 ].mNormal;
                //= TerrainTextures[travTextures];

                vertices[1].Position = mCorners[1, 0].mLocation;
                vertices[ 1 ].TextureCoordinate = new Vector2( 1, 0 );
                vertices[ 1 ].Normal = mCorners[ 1, 0 ].mNormal;
                
                vertices[2].Position = mCorners[0, 1].mLocation;
                vertices[ 2 ].TextureCoordinate = new Vector2( 0, 1 );
                vertices[ 2 ].Normal = mCorners[ 0, 1 ].mNormal;

                vertices[ 3 ].Position = mCorners[ 1, 0 ].mLocation;
                vertices[ 3 ].TextureCoordinate = new Vector2( 1, 0 );
                vertices[ 3 ].Normal = mCorners[ 1, 0 ].mNormal;

                vertices[4].Position = mCorners[1, 1].mLocation;
                vertices[ 4 ].TextureCoordinate = new Vector2( 1, 1 );
                vertices[ 4 ].Normal = mCorners[ 1, 1 ].mNormal;

                vertices[ 5 ].Position = mCorners[ 0, 1 ].mLocation;
                vertices[ 5 ].TextureCoordinate = new Vector2( 0, 1 );
                vertices[ 5 ].Normal = mCorners[ 0, 1 ].mNormal;

                GameArgs.Current.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);
            } else
            {
                mSubdivisions[ 0, 0 ].Draw( TestDrawDepth - 1 );
                mSubdivisions[ 0, 1 ].Draw( TestDrawDepth - 1 );
                mSubdivisions[ 1, 0 ].Draw( TestDrawDepth - 1 );
                mSubdivisions[ 1, 1 ].Draw( TestDrawDepth - 1 );
                
                //TODO: nonrecursive opt
            }

        }

        public void subdivide(float Width)
        {
            if (mSubdivisions == null)
            {
                mSubdivisions = new Square[2, 2];

                Corner[,] Corners = new Corner[2, 2];
                Corners[0, 0] = mCorners[0, 0];
                Corners[0, 1] = new Corner((mCorners[0, 0].mLocation + mCorners[0, 1].mLocation) / 2.0f);
                Corners[1, 0] = new Corner((mCorners[0, 0].mLocation + mCorners[1, 0].mLocation) / 2.0f);
                Corners[1, 1] = new Corner((mCorners[0, 0].mLocation + mCorners[1, 1].mLocation) / 2.0f);
                mSubdivisions[0, 0] = new Square(mDepth + 1, Width, Corners);

                Corners[0, 0] = mSubdivisions[0, 0].mCorners[0, 1];
                Corners[0, 1] = mCorners[0, 1];
                Corners[1, 0] = mSubdivisions[0, 0].mCorners[1, 1];
                Corners[1, 1] = new Corner((mCorners[0, 1].mLocation + mCorners[1, 1].mLocation) / 2.0f);
                mSubdivisions[0, 1] = new Square(mDepth + 1, Width, Corners);

                Corners[0, 0] = mSubdivisions[0, 0].mCorners[1, 0];
                Corners[0, 1] = mSubdivisions[0, 0].mCorners[1, 1];
                Corners[1, 0] = mCorners[1, 0];
                Corners[1, 1] = new Corner((mCorners[1, 0].mLocation + mCorners[1, 1].mLocation) / 2.0f);
                mSubdivisions[1, 0] = new Square(mDepth + 1, Width, Corners);

                Corners[0, 0] = mSubdivisions[0, 0].mCorners[1, 1];
                Corners[0, 1] = mSubdivisions[0, 1].mCorners[1, 1];
                Corners[1, 0] = mSubdivisions[1, 0].mCorners[1, 1];
                Corners[1, 1] = mCorners[1,1];
                mSubdivisions[1, 1] = new Square(mDepth + 1, Width, Corners);
            }
            else
            {
                throw (new Exception("Square already subdivided"));
            }
        }

        public void increaseSubdivisions(float HeightRange, float HeightRangeDecreaseFactor, Random Random)
        {
            if (mSubdivisions == null)
            {
                // diamond step
                subdivide(mWidth / 2.0f);
                float AverageHeight = (mCorners[0, 0].mLocation.Y + mCorners[0, 1].mLocation.Y + mCorners[1, 0].mLocation.Y + mCorners[1, 1].mLocation.Y) / 4.0f;
                mSubdivisions[0, 0].mCorners[1, 1].mLocation.Y = AverageHeight + (float)Random.Next((int)HeightRange) - HeightRange / 2.0f;
            }
            else
            {
                // TODO: nonrecursive opt
                mSubdivisions[0, 0].increaseSubdivisions(HeightRange * HeightRangeDecreaseFactor, HeightRangeDecreaseFactor, Random);
                mSubdivisions[0, 1].increaseSubdivisions(HeightRange * HeightRangeDecreaseFactor, HeightRangeDecreaseFactor, Random);
                mSubdivisions[1, 0].increaseSubdivisions(HeightRange * HeightRangeDecreaseFactor, HeightRangeDecreaseFactor, Random);
                mSubdivisions[1, 1].increaseSubdivisions(HeightRange * HeightRangeDecreaseFactor, HeightRangeDecreaseFactor, Random);
            }
        }

        public void decreaseSubdivisions()
        {
            if (mSubdivisions != null)
            {
                if (mSubdivisions[0, 0].mSubdivisions == null)
                {
                    mSubdivisions = null;
                }
                else
                {
                    // TODO: nonrecursive opt
                    mSubdivisions[0, 0].decreaseSubdivisions();
                    mSubdivisions[0, 1].decreaseSubdivisions();
                    mSubdivisions[1, 0].decreaseSubdivisions();
                    mSubdivisions[1, 1].decreaseSubdivisions();
                }
            }
        }
    }

    class Terrain : Placeable
    {
        public Square mRoot;
        public float mWidth;
        public int mDepth;

        TerrainEffect mEffect;
        TerrainTexture[] mTerrainTextures;
        VertexDeclaration mVertexDeclaration;
        
        public bool mDrawWater;
        public float mWaterHeight;
        Texture[] mWaterTexture;

        VertexQueue<VertexPositionNormalTexture> mBufferedVertices;
        bool mForceClearBuffer;

        public Terrain( string setName, int mSetWidth ) : base( setName )
        {
            mWidth = mSetWidth;
            Corner[,] Corners = new Corner[2,2];
            Corners[0, 0] = new Corner(new Vector3(0, 0, 0)); 
            Corners[0, 1] = new Corner(new Vector3(0, 0, mWidth)); 
            Corners[1, 0] = new Corner(new Vector3(mWidth, 0, 0));
            Corners[1, 1] = new Corner(new Vector3(mWidth, 0, mWidth));
            mRoot = new Square(1, mWidth, Corners);
            mDepth = 1;

            /*
           // mDrawWater = true;
            mWaterHeight = mTerrainTextures[0].mHeightCutoff;
            mWaterTexture = new Texture[1];
            mWaterTexture[0] = new Texture(256, 256);
            mWaterTexture[0].generateSolidTexture(Color.Blue);
            mWaterTextureGroup = new TextureGroup(mWaterTexture);
*/        }

        ~Terrain()
        {
        }

        protected override void LoadContent()
        {
            mTerrainTextures = new TerrainTexture[ 4 ];
            mTerrainTextures[ 0 ] = new TerrainTexture( 256, 256, Color.Aquamarine, 0.0f );
            mTerrainTextures[ 1 ] = new TerrainTexture( 256, 256, Color.BlanchedAlmond, 10.0f );
            mTerrainTextures[ 2 ] = new TerrainTexture( 256, 256, Color.Green, 30.0f );
            mTerrainTextures[ 3 ] = new TerrainTexture( 256, 256, Color.Gray, 50.0f );

            mEffect = new TerrainEffect( GameArgs.Current.GraphicsDevice, CompilerOptions.None, null );
            //mEffect = GameArgs.Current.ContentManager.Load<TerrainEffect>( "Terrain" );

            mVertexDeclaration = new VertexDeclaration( GameArgs.Current.GraphicsDevice, VertexPositionNormalTexture.VertexElements );

            QueueVertices();

            base.LoadContent();
        }


        public override void setDebugData( DebugForm DebugForm )
        {
            if( DebugForm != null )
            {
                DebugForm.setDebugValue( Name + "|Terrain|Subdivisions", ( mDepth - 1 ).ToString() );
                DebugForm.setDebugValue( Name + "|Terrain|Triangles", ( ( int )Math.Pow( 4, mDepth - 1 ) * 6 ).ToString() );
                base.setDebugData( DebugForm );
            }
        }

        public void increaseSubdivisions()
        {
            Random Random = new Random((int)System.DateTime.Now.Ticks);
            mRoot.increaseSubdivisions(mWidth / 2.0f, (float)Math.Pow(2.0f, -1.0f), Random);
            mDepth++;

            setDebugData();

            mForceClearBuffer = true;
        }

        public void decreaseSubdivisions()
        {
            if( mRoot.mSubdivisions != null )
            {
                mRoot.decreaseSubdivisions();
                mDepth--;

                setDebugData();

                mForceClearBuffer = true;
            }
        }

        public void QueueVertices()
        {
            double TriangleCount = Math.Pow(4, mDepth - 1) * 6;
            if (TriangleCount > short.MaxValue)
            {
                throw (new Exception("TriangleCount exceeds short.MaxValue"));
            }
            mBufferedVertices = new VertexQueue<VertexPositionNormalTexture>((short)TriangleCount);
            mRoot.collectVertices(mBufferedVertices, 2);
        }

        public override void Draw( GameTime GameTime )
        {
            Matrix WorldViewProjection = GameArgs.Current.WorldMatrix * GameArgs.Current.ViewMatrix * GameArgs.Current.ProjectionMatrix;

            GameArgs.Current.GraphicsDevice.VertexDeclaration = mVertexDeclaration;

            mEffect.WorldViewProjection = WorldViewProjection;

            mEffect.SunDirection = ( ( MCGameArgs )GameArgs.Current ).Sun.Rotation;
            mEffect.SunAmbientIntensity = (( MCGameArgs )GameArgs.Current ).Sun.AmbientIntensity;

            mEffect.TextureHeights = new Vector3( mTerrainTextures[ 0 ].mHeightCutoff, mTerrainTextures[ 1 ].mHeightCutoff, mTerrainTextures[ 2 ].mHeightCutoff );

            mEffect.GroundTexture = mTerrainTextures[ 0 ];
            mEffect.MudTexture = mTerrainTextures[ 1 ] ;
            mEffect.RockTexture = mTerrainTextures[ 2 ] ;
            mEffect.SnowTexture = mTerrainTextures[ 3 ] ;

            GameArgs.Current.GraphicsDevice.VertexDeclaration = new VertexDeclaration( GameArgs.Current, VertexPositionNormalTexture.VertexElements );

            mEffect.Begin();

//            mRoot.Draw( 2 );

            if (mBufferedVertices == null || mForceClearBuffer)
            {
                QueueVertices();
            }
            mBufferedVertices.draw(GameArgs.Current.GraphicsDevice);

            mEffect.End();

            if( Debug )
            {
                GameArgs.Current.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
                GameArgs.Current.BasicEffect.Begin();
                GameArgs.Current.BasicEffect.CurrentTechnique.Passes[ 0 ].Begin();

                mBufferedVertices.draw(GameArgs.Current.GraphicsDevice);

                GameArgs.Current.BasicEffect.CurrentTechnique.Passes[ 0 ].End();
                GameArgs.Current.BasicEffect.End();
                GameArgs.Current.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
            }
        }

        public void drawWater()
        {/*
            float[] AmbientAndDiffuse = { 1.0f, 1.0f, 1.0f, 1.0f };
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE, AmbientAndDiffuse);
            float[] Specular = { 1.0f, 1.0f, 1.0f, 1.0f };
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, Specular);
            */
            /*
            Gl.glColor4f(1.0f, 1.0f, 1.0f, 0.9f);
            Gl.glEnable(Gl.GL_BLEND);

            mWaterTextureGroup.bindTexture(0);
            Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3f(0.0f, mWaterHeight, 0.0f);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3f(0.0f, mWaterHeight, mWidth);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3f(mWidth, mWaterHeight, 0.0f);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3f(mWidth, mWaterHeight, mWidth);
            Gl.glEnd();
            Gl.glDisable(Gl.GL_BLEND);

            Gl.glColor4f(1.0f, 1.0f, 1.0f, 1.0f);*/
        }

/*        public void drawNonRecursive(float Width)
        {
            System.Collections.Generic.List<Square> travSquares = new System.Collections.Generic.List<Square>();
            travSquares.Add(mRoot);
            while (travSquares.Count > 0)
            {
                if (travSquares[0].mSubdivisions != null)
                {
                    foreach( Square Square in travSquares[0].mSubdivisions )
                    {
                        travSquares.Add(Square);
                    }
                    /*
                     *
                mSubdivisions[0, 0].draw(Width / 2, MaxColorHeight, TextureGroup, TextureHeights, bDebug);
                Gl.glPushMatrix();
                Gl.glTranslatef(0, 0, Width / 2);
                mSubdivisions[0, 1].draw(Width / 2, MaxColorHeight, TextureGroup, TextureHeights, bDebug);
                Gl.glPopMatrix();
                Gl.glPushMatrix();
                Gl.glTranslatef(Width / 2, 0, 0);
                mSubdivisions[1, 0].draw(Width / 2, MaxColorHeight, TextureGroup, TextureHeights, bDebug);
                Gl.glTranslatef(0, 0, Width / 2);
                mSubdivisions[1, 1].draw(Width / 2, MaxColorHeight, TextureGroup, TextureHeights, bDebug);
                Gl.glPopMatrix();
                     */
        /*
                }
                else
                {
                    float SquareWidth = Width / (travSquares[0].mDepth * 2);
                    float AverageHeight = (travSquares[0].mCorners[0, 0].mHeight + travSquares[0].mCorners[0, 1].mHeight + travSquares[0].mCorners[1, 0].mHeight + travSquares[0].mCorners[1, 1].mHeight) / 4.0f;
      /*              if (AverageHeight < mTerrainTextureHeights[0])
                    {
                        mTerrainTextureGroup.bindTexture(0);
                    }
                    else if (AverageHeight < mTerrainTextureHeights[1])
                    {
                        mTerrainTextureGroup.bindTexture(1);
                    }
                    else
                    {
                        mTerrainTextureGroup.bindTexture(2);
                    }
                    */
        /*
                    Gl.glBegin(Gl.GL_TRIANGLE_STRIP);

                    Gl.glTexCoord2f(0, 0);
                    Gl.glVertex3f(0, travSquares[0].mCorners[0, 0].mHeight, 0);

                    Gl.glTexCoord2f(0, 1);
                    Gl.glVertex3f(0, travSquares[0].mCorners[0, 1].mHeight, SquareWidth);

                    Gl.glTexCoord2f(1, 0);
                    Gl.glVertex3f(SquareWidth, travSquares[0].mCorners[1, 0].mHeight, 0);

                    Gl.glTexCoord2f(1, 1);
                    Gl.glVertex3f(SquareWidth, travSquares[0].mCorners[1, 1].mHeight, SquareWidth);

                    Gl.glEnd();
                    if (mDebug)
                    {
                        Gl.glColor3f(0, 0, 0);
                        Gl.glBegin(Gl.GL_LINE_STRIP);
                        Gl.glVertex3f(0, travSquares[0].mCorners[0, 0].mHeight, 0);
                        Gl.glVertex3f(0, travSquares[0].mCorners[0, 1].mHeight, SquareWidth);
                        Gl.glVertex3f(SquareWidth, travSquares[0].mCorners[1, 0].mHeight, 0);
                        Gl.glVertex3f(SquareWidth, travSquares[0].mCorners[1, 1].mHeight, SquareWidth);
                        Gl.glEnd();
                        Gl.glColor3f(1, 1, 1);
                    }
                }
                travSquares.RemoveAt(0);
            }
        }

        /* cool error
         *         public void drawNonRecursive()
        {
            System.Collections.Generic.List<Square> travSquares = new System.Collections.Generic.List<Square>();
            travSquares.Add(mRoot);
            while (travSquares.Count > 0)
            {
                if (travSquares[0].mSubdivisions != null)
                {
                    foreach( Square Square in travSquares[0].mSubdivisions )
                    {
                        travSquares.Add(Square);
                    }
                }
                else
                {
                    float AverageHeight = (travSquares[0].mCorners[0, 0].mHeight + travSquares[0].mCorners[0, 1].mHeight + travSquares[0].mCorners[1, 0].mHeight + travSquares[0].mCorners[1, 1].mHeight) / 4.0f;
                    if (AverageHeight < mTerrainTextureHeights[0])
                    {
                        mTerrainTextureGroup.bindTexture(0);
                    }
                    else if (AverageHeight < mTerrainTextureHeights[1])
                    {
                        mTerrainTextureGroup.bindTexture(1);
                    }
                    else
                    {
                        mTerrainTextureGroup.bindTexture(2);
                    }

                    Gl.glBegin(Gl.GL_TRIANGLE_STRIP);

                    Gl.glTexCoord2f(0, 0);
                    Gl.glVertex3f(0, travSquares[0].mCorners[0, 0].mHeight, 0);

                    Gl.glTexCoord2f(0, 1);
                    Gl.glVertex3f(0, travSquares[0].mCorners[0, 1].mHeight, mWidth);

                    Gl.glTexCoord2f(1, 0);
                    Gl.glVertex3f(mWidth, travSquares[0].mCorners[1, 0].mHeight, 0);

                    Gl.glTexCoord2f(1, 1);
                    Gl.glVertex3f(mWidth, travSquares[0].mCorners[1, 1].mHeight, mWidth);

                    Gl.glEnd();
                    if (mDebug)
                    {
                        Gl.glColor3f(0, 0, 0);
                        Gl.glBegin(Gl.GL_LINE_STRIP);
                        Gl.glVertex3f(0, travSquares[0].mCorners[0, 0].mHeight, 0);
                        Gl.glVertex3f(0, travSquares[0].mCorners[0, 1].mHeight, mWidth);
                        Gl.glVertex3f(mWidth, travSquares[0].mCorners[1, 0].mHeight, 0);
                        Gl.glVertex3f(mWidth, travSquares[0].mCorners[1, 1].mHeight, mWidth);
                        Gl.glEnd();
                        Gl.glColor3f(1, 1, 1);
                    }
                }
                travSquares.RemoveAt(0);
            }
        }
*/
        
        
        /*	int bTexture, bEdging ;
	GLuint iLowTexName, iMidTexName, iHighTexName ;
	double dLowHeight, dMidHeight, dHighHeight ;
*/
/*
	void CreateMap(long lHori, long lVert) ;
	void Clear() ;

	void GenerateFlatMap(double dScale, double dHeight, long lHori, long lVert) ;
	int ReadRaw(char *szFilename, long lHori, long lVert, double dScale, double dScaleHeight) ;
	int ReadMap(char *szFilename) ;

	int SetLowHeight(double dHeight, char *szTexFilename) ;
	int SetMidHeight(double dHeight, char *szTexFilename) ;
	int SetHighHeight(double dHeight, char *szTexFilename) ;

	double CheckCollision(Point ptCameraPosition) ;

	double Height(long lHori, long lVert) ;
	double Height(long lHori, long lVert, double dHeight) ;
	void DisplaZ(Point ptCameraPosition, Point ptCameraOrientation, double dScale) ;*/
    }
}
