using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNAUtility;

namespace MCXNA
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MCGame : Microsoft.Xna.Framework.Game
    {
        MCGameArgs mGameArgs;

        Terrain mTerrain;

        public MCGame()
        {
            mGameArgs = new MCGameArgs( this );

            Content.RootDirectory = "Content";

            mGameArgs.Player = new Player( Vector3.Zero, new Vector3( 0.0f, 0.0f, 0.0f ) );
            mGameArgs.addWorldObject( mGameArgs.Player );

            mGameArgs.Sun = new Sun();
            mGameArgs.Sun.Location = new Vector3( 0.0f, 100.0f, 0.0f );
            mGameArgs.Sun.Rotation = new Vector3( 0.0f, 0.0f, 0.0f );
            mGameArgs.Sun.setTime( 4 );
            mGameArgs.addWorldObject( mGameArgs.Sun );

            mTerrain = new Terrain( "Terrain", 1000 );
            mGameArgs.addWorldObject( mTerrain );
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Initialize()
        {
            /*            int centerX = Window.ClientBounds.Width / 2;
                        int centerY = Window.ClientBounds.Height / 2;
                        //
                        Mouse.SetPosition(centerX, centerY);
             * 
             *             _spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);

                        _spriteBatch.DrawString(_spriteFont, "Patch count: " + _terrain.PatchCount, new Vector2(10, 10), Color.Yellow);
                        _spriteBatch.DrawString(_spriteFont, "Drawn patch count: " + _terrain.DrawnPatchCount, new Vector2(10, 30), Color.Yellow);

                        _spriteBatch.End();
                        */
            mGameArgs.ProjectionMatrix = mGameArgs.Player.getProjectionMatrix();

            mGameArgs.GraphicsDevice = mGameArgs.GraphicsDeviceManager.GraphicsDevice;

            mGameArgs.BasicEffect = new BasicEffect( mGameArgs, null );
            mGameArgs.BasicEffect.VertexColorEnabled = true;

            base.Initialize();
        }

        protected void handleInput(GameTime GameTime)
        {
            KeyboardState KeyboardState = Keyboard.GetState();
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            } else if ( KeyboardState.IsKeyDown(Keys.OemPlus) )
            {
                mTerrain.increaseSubdivisions();
            }
            else if (KeyboardState.IsKeyDown(Keys.OemMinus))
            {
                mTerrain.decreaseSubdivisions();
            } else if ( KeyboardState.IsKeyDown(Keys.Tab))
            {
                mGameArgs.Debug = !mGameArgs.Debug;
            }
            else
            {
                mGameArgs.Player.handleInput(KeyboardState, GameTime);
            }
            // Allows the game to exit
            //            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //              this.Exit();
        }

        protected override void Update(GameTime gameTime)
        {
            handleInput(gameTime);

            //mPlayer.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            mGameArgs.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
 
            mGameArgs.ViewMatrix = mGameArgs.Player.getViewMatrix();
            mGameArgs.WorldMatrix = Matrix.Identity;

            if( mGameArgs.Debug )
            {
                XNAUtility.Debug.drawAxis( 10.0f );
            }
            
            base.Draw(gameTime);
        }

    }

    public class MCGameArgs : XNAUtility.GameArgs
    {
        public MCGameArgs( Game setGame )
            : base( setGame )
        {
        }

        public Player Player;
        public Sun Sun;
    }
}
