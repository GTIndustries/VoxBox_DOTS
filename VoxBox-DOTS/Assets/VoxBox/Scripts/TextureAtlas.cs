using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.U2D;

namespace VoxBox.Scripts {
    public enum VoxelID {
        NULL      = -666,
        LOGO      = -11,
        AIR       = -1,
        BEDROCK   =  0,
        GRASS     =  1,
        COBBLE    =  2,
        LIMESTONE =  3,
        DIRT      =  4,
        LOG       =  5
    }

    public enum TextureID {
        NULL,
        LOGO,
        AIR,
        BEDROCK,
        GRASS,
        GRASS_SIDE,
        COBBLE,
        LIMESTONE,
        DIRT,
        LOG_TOP,
        LOG_SIDE,
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

    public class TextureAtlas : MonoBehaviour/*, IDisposable*/ {
        [SerializeField] private SpriteAtlas _voxelAtlas = null;
        [SerializeField] private Material _voxelMaterial = null;
        public static SpriteAtlas voxelAtlas;
        public static Material voxelMaterial;
        public static Dictionary<int, UV> textureUVs = new Dictionary<int, UV>();
        
        public static readonly Dictionary<TextureID, string> TextureNames = new Dictionary<TextureID, string> {
            {TextureID.NULL,       "debug"},
            {TextureID.LOGO,       "logo"},
            {TextureID.AIR,        "air"}, 
            {TextureID.GRASS,      "grass"}, 
            {TextureID.GRASS_SIDE, "grass_side"}, 
            {TextureID.COBBLE,     "cobble"}, 
            {TextureID.LIMESTONE,  "stone"}, 
            {TextureID.BEDROCK,    "bedrock"}, 
            {TextureID.DIRT,       "dirt"}, 
            {TextureID.LOG_TOP,    "wood_log_top"}, 
            {TextureID.LOG_SIDE,   "wood_log_side"}
        };
        
        public static readonly Dictionary<VoxelID, string> VoxelNames = new Dictionary<VoxelID, string> {
            {VoxelID.NULL,      "Null"},
            {VoxelID.LOGO,      "GTIndustries Logo"},
            {VoxelID.AIR,       "Air"}, 
            {VoxelID.GRASS,     "Grass"}, 
            {VoxelID.COBBLE,    "Cobblestone"}, 
            {VoxelID.LIMESTONE, "Limestone"}, 
            {VoxelID.BEDROCK,   "Bedrock"}, 
            {VoxelID.DIRT,      "Dirt"}, 
            {VoxelID.LOG,       "Oak Log"}
        };
        
        //private static readonly int BaseMap    = Shader.PropertyToID("_MainTex"); // SRP
        //private static readonly int BaseMap    = Shader.PropertyToID("_BaseColorMap"); // HDRP
        private static readonly int BaseMap    = Shader.PropertyToID("_BaseMap"); // URP
        // private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");
        // private static readonly int Metallic   = Shader.PropertyToID("_Metallic");

        private void Awake() {
            MaterialSetup();
        }

        private void Start() {
            PopulateTextureDict();
        }

        private static void PopulateTextureDict() {
            var sprites = ((TextureID[])Enum.GetValues(typeof(TextureID))).ToDictionary(
                textureID => textureID, 
                textureID => voxelAtlas.GetSprite(GetTextureName(textureID))
            );

            foreach (var textureID in (int[])Enum.GetValues(typeof(TextureID))) {
                // Debug.Log($"{textureID} | Added to UV dictionary");
                textureUVs.Add(
                    textureID, 
                    new UV(sprites[(TextureID)textureID].uv[0],
                           sprites[(TextureID)textureID].uv[1],
                           sprites[(TextureID)textureID].uv[2],
                           sprites[(TextureID)textureID].uv[3]));
            }
        }

        private void MaterialSetup() {
            voxelAtlas = _voxelAtlas;
            voxelMaterial = new Material(_voxelMaterial);
            //voxelMaterial.CopyPropertiesFromMaterial(_voxelMaterial);
            var voxelTexture = voxelAtlas.GetSprite("debug").texture;
            voxelMaterial.SetTexture(BaseMap, voxelTexture);
            // var voxelShader  = voxelMaterial.shader;
            // var voxelTexture = voxelAtlas.GetSprite("debug").texture;
            // voxelTexture.anisoLevel = 0;
            // voxelMaterial           = new Material(voxelShader);
            // voxelMaterial.SetTexture(BaseMap, voxelTexture);
            // voxelMaterial.SetFloat(Smoothness, 0f);
            // voxelMaterial.SetFloat(Metallic, 0.5f);
            //voxelMaterial.mainTexture = voxelAtlas.GetSprite("debug").texture;
        }
    
        public static string GetTextureName(TextureID textureID) {
            string textureName;

            if (!TextureNames.TryGetValue(textureID, out textureName)) {
                // if couldn't be found, use air
                Debug.Log("TextureID's texture name not found in dictionary");
                TextureNames.TryGetValue(TextureID.NULL, out textureName);
            }
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
                VoxelID.LIMESTONE   => TextureID.LIMESTONE,
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

        // public static UV GetTextureUVs(TextureID textureID, NativeArray<UV> textureArray) {
        //     return textureID switch {
        //         TextureID.NULL       => textureArray[0],
        //         TextureID.LOGO       => textureArray[1],
        //         TextureID.AIR        => textureArray[2],
        //         TextureID.BEDROCK    => textureArray[3],
        //         TextureID.GRASS      => textureArray[4],
        //         TextureID.GRASS_SIDE => textureArray[5],
        //         TextureID.COBBLE     => textureArray[6],
        //         TextureID.LIMESTONE  => textureArray[7],
        //         TextureID.DIRT       => textureArray[8],
        //         TextureID.LOG_TOP    => textureArray[9],
        //         TextureID.LOG_SIDE   => textureArray[10],
        //         _                    => textureArray[0]
        //     };
        // }

        // public void Dispose() {
        //     textureUVs.Dispose();
        // }
        //
        // ~TextureAtlas() {
        //     Dispose();
        // }
    }
}