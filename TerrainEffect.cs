using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MCXNA
{
    class TerrainEffect
    {
        Effect mEffect;

        public Matrix WorldViewProjection 
        { 
            get { return mEffect.Parameters[ "WorldViewProjection" ].GetValueMatrix(); } 
            set { mEffect.Parameters[ "WorldViewProjection" ].SetValue( value ); } 
        }

        public Vector3 SunDirection 
        { 
            get { return mEffect.Parameters[ "g_vecLightDirection"].GetValueVector3(); } 
            set { mEffect.Parameters[ "g_vecLightDirection" ].SetValue( value); } 
        }

        public Vector3 SunAmbientIntensity 
        { 
            get { return mEffect.Parameters[ "SunAmbientIntensity" ].GetValueVector3(); } 
            set { mEffect.Parameters[ "SunAmbientIntensity" ].SetValue( value ); } 
        } 

        // A vector containing the different heights used to divide the terrain in ground, mud, rock and snow areas.
        // Values are stored from the lowest to the highest heights.
        public Vector3 TextureHeights 
        { 
            get { return mEffect.Parameters[ "g_vecHeights" ].GetValueVector3(); } 
            set { mEffect.Parameters[ "g_vecHeights" ].SetValue( value ); } 
        } 

        // Textures used for the terrain.
        public Texture2D GroundTexture 
        {
            get { return mEffect.Parameters[ "g_texGround" ].GetValueTexture2D(); } 
            set { mEffect.Parameters[ "g_texGround" ].SetValue( value ); } 
        }
        public Texture2D MudTexture
        {
            get { return mEffect.Parameters[ "g_texMud" ].GetValueTexture2D(); } 
            set { mEffect.Parameters[ "g_texMud" ].SetValue( value ); } 
        }
        public Texture2D RockTexture
        {
            get { return mEffect.Parameters[ "g_texRock" ].GetValueTexture2D(); } 
            set { mEffect.Parameters[ "g_texRock" ].SetValue( value ); } 
        }
        public Texture2D SnowTexture
        {
            get { return mEffect.Parameters[ "g_texSnow" ].GetValueTexture2D(); } 
            set { mEffect.Parameters[ "g_texSnow" ].SetValue( value ); } 
        }

        public TerrainEffect( GraphicsDevice GraphicsDevice, CompilerOptions Options, EffectPool Pool ) 
        {
            mEffect = XNAUtility.GameArgs.Current.ContentManager.Load<Effect>( "Terrain" );
            mEffect.CurrentTechnique = mEffect.Techniques[ "DefaultTechnique" ];
        }

        public void Begin()
        {
            mEffect.Begin();
            mEffect.CurrentTechnique.Passes[ 0 ].Begin();
        }

        public void End()
        {
            mEffect.CurrentTechnique.Passes[ 0 ].End();
            mEffect.End();
        }
    }
}
