using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

namespace VoxBox.Scripts {
    public enum VoxelID {
        NULL    = -666,
        LOGO    = -11,
        AIR     = -1,
        BEDROCK =  0,
        GRASS   =  1,
        COBBLE  =  2,
        STONE   =  3,
        DIRT    =  4,
        LOG     =  5
    }

    public enum TextureID {
        NULL       = -666,
        LOGO       = -11,
        AIR        = -1,
        BEDROCK    =  0,
        GRASS      =  1,
        GRASS_SIDE =  2,
        COBBLE     =  3,
        STONE      =  4,
        DIRT       =  5,
        LOG_TOP    =  6,
        LOG_SIDE   =  7,
    }
    
    public enum Direction {
        NORTH = 0, 
        EAST  = 1, 
        SOUTH = 2, 
        WEST  = 3, 
        UP    = 4, 
        DOWN  = 5
    }

    public struct UV {
        public Vector2 uv0;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector2 uv3;

        public UV(Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3) {
            this.uv0 = uv0;
            this.uv1 = uv1;
            this.uv2 = uv2;
            this.uv3 = uv3;
        }
    }

    public class TextureAtlas : MonoBehaviour {
        [SerializeField] private SpriteAtlas voxelSpriteAtlas;
        
        public static            SpriteAtlas voxelAtlas;
        //[SerializeField] private Material _voxelMaterial;
        public static Material voxelMaterial;
        public static Dictionary<TextureID, UV> textureUvs = new Dictionary<TextureID, UV>();
        public static Dictionary<TextureID, string> textureNames = new Dictionary<TextureID, string> {
            {TextureID.NULL,       "debug"},
            {TextureID.LOGO,       "logo"},
            {TextureID.AIR,        "air"}, 
            {TextureID.GRASS,      "grass"}, 
            {TextureID.GRASS_SIDE, "grass_side"}, 
            {TextureID.COBBLE,     "cobble"}, 
            {TextureID.STONE,      "stone"}, 
            {TextureID.BEDROCK,    "bedrock"}, 
            {TextureID.DIRT,       "dirt"}, 
            {TextureID.LOG_TOP,    "wood_log_top"}, 
            {TextureID.LOG_SIDE,   "wood_log_side"}
        };
        public static Dictionary<VoxelID, string> voxelNames = new Dictionary<VoxelID, string> {
            {VoxelID.NULL,    "Null"},
            {VoxelID.LOGO,    "GTIndustries Logo"},
            {VoxelID.AIR,     "Air"}, 
            {VoxelID.GRASS,   "Grass"}, 
            {VoxelID.COBBLE,  "Cobblestone"}, 
            {VoxelID.STONE,   "Limestone"}, 
            {VoxelID.BEDROCK, "Bedrock"}, 
            {VoxelID.DIRT,    "Dirt"}, 
            {VoxelID.LOG,     "Oak Log"}
        };
        private static readonly int BaseMap                = Shader.PropertyToID("_BaseMap");
        private static readonly int Smoothness             = Shader.PropertyToID("_Smoothness");
        private static readonly int ReceiveShadows         = Shader.PropertyToID("_ReceiveShadows");
        private static readonly int EnvironmentReflections = Shader.PropertyToID("_EnvironmentReflections");

        private void Awake() {
            voxelAtlas = voxelSpriteAtlas;
            //voxelMaterial = _voxelMaterial;
            //voxelMaterial.SetTexture("_BaseMap", voxelAtlas.GetSprite("debug").texture);
            MaterialSetup();
            PopulateTextureDict();
        }

        private static void PopulateTextureDict() {
            var sprites = ((TextureID[])Enum.GetValues(typeof(TextureID))).ToDictionary(
                textureID => textureID, 
                textureID => voxelAtlas.GetSprite(GetTextureName(textureID))
            );

            foreach (var textureID in (TextureID[])Enum.GetValues(typeof(TextureID))) {
                // Debug.Log($"{textureID} | Added to UV dictionary");
                textureUvs.Add(
                    textureID, 
                    new UV(sprites[textureID].uv[0],
                           sprites[textureID].uv[1],
                           sprites[textureID].uv[2],
                           sprites[textureID].uv[3]));
            }
        }

        private static void MaterialSetup() {
            var voxelShader  = Shader.Find("Universal Render Pipeline/Lit");
            var voxelTexture = voxelAtlas.GetSprite("debug").texture;
            voxelTexture.anisoLevel = 0;
            voxelMaterial           = new Material(voxelShader);
            voxelMaterial.SetTexture(BaseMap, voxelTexture);
            voxelMaterial.SetFloat(Smoothness, 0f);
            voxelMaterial.SetFloat(ReceiveShadows, 1f);
            voxelMaterial.SetFloat(EnvironmentReflections, 1f);
            voxelMaterial.enableInstancing        = true;
            voxelMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            //voxelMaterial.mainTexture = voxelAtlas.GetSprite("debug").texture;
        }
    
        public static string GetTextureName(TextureID textureID) {
            string textureName;
            if (!textureNames.TryGetValue(textureID, out textureName)) // if couldn't be found, use air
                textureNames.TryGetValue(TextureID.AIR, out textureName);
            return textureName;
        }
        
        public static TextureID GetFaceTexture(VoxelID voxelID, Direction face) {
            return voxelID switch {
                VoxelID.NULL    => TextureID.NULL,
                VoxelID.LOGO    => TextureID.LOGO,
                VoxelID.AIR     => TextureID.AIR,
                VoxelID.BEDROCK => TextureID.BEDROCK,
                VoxelID.GRASS   => face switch {
                    Direction.NORTH => TextureID.GRASS_SIDE,
                    Direction.EAST  => TextureID.GRASS_SIDE,
                    Direction.SOUTH => TextureID.GRASS_SIDE,
                    Direction.WEST  => TextureID.GRASS_SIDE,
                    Direction.UP    => TextureID.GRASS,
                    Direction.DOWN  => TextureID.DIRT,
                    _               => TextureID.NULL
                },
                VoxelID.COBBLE  => TextureID.COBBLE,
                VoxelID.STONE   => TextureID.STONE,
                VoxelID.DIRT    => TextureID.DIRT,
                VoxelID.LOG     => face switch {
                    Direction.NORTH => TextureID.LOG_SIDE,
                    Direction.EAST  => TextureID.LOG_SIDE,
                    Direction.SOUTH => TextureID.LOG_SIDE,
                    Direction.WEST  => TextureID.LOG_SIDE,
                    Direction.UP    => TextureID.LOG_TOP,
                    Direction.DOWN  => TextureID.LOG_TOP,
                    _               => TextureID.NULL
                },
                _               => TextureID.NULL
            };
        }
    }
}