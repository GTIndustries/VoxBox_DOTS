using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.U2D;
using VoxBox.Scripts.Systems;

namespace VoxBox.Scripts {
    public enum Direction {
        NORTH = 0, 
        EAST  = 1, 
        SOUTH = 2, 
        WEST  = 3, 
        UP    = 4, 
        DOWN  = 5
    }

    public enum VoxelID {
        NULL,
        LOGO,
        AIR,
        BEDROCK,
        GRASS,
        COBBLE,
        LIMESTONE,
        DIRT,
        LOG
    }

    // public enum TextureID {
    //     NULL,
    //     LOGO,
    //     AIR,
    //     BEDROCK,
    //     GRASS,
    //     GRASS_SIDE,
    //     COBBLE,
    //     STONE,
    //     DIRT,
    //     LOG_TOP,
    //     LOG_SIDE,
    // }

    public class TextureAtlas : MonoBehaviour/*, IDisposable*/ {
        // [SerializeField] private SpriteAtlas _voxelAtlas = null;
        [SerializeField] private Material _voxelMaterial = null;
        // public static SpriteAtlas voxelAtlas;
        public static Material voxelMaterial;
        
        //private static readonly int BaseMap    = Shader.PropertyToID("_MainTex"); // SRP
        //private static readonly int BaseMap    = Shader.PropertyToID("_BaseColorMap"); // HDRP
        private static readonly int BaseMap    = Shader.PropertyToID("_BaseMap"); // URP
        // private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");
        // private static readonly int Metallic   = Shader.PropertyToID("_Metallic");

        private void Awake() {
            Debug.Log("TextureAtlas::Start: Materials loading");
            MaterialSetup();
            Debug.Log("TextureAtlas::Start: Materials loaded!");
        }

        private void MaterialSetup() {
            ref var voxelAtlas = ref World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<VoxelRegistrationSystem>().voxelAtlas;
            // voxelAtlas = _voxelAtlas;
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
        
        // public static TextureID GetFaceTexture(VoxelID voxelID, Direction face) {
        //     return voxelID switch {
        //         VoxelID.NULL    => TextureID.NULL,
        //         VoxelID.LOGO    => TextureID.LOGO,
        //         VoxelID.AIR     => TextureID.AIR,
        //         VoxelID.BEDROCK => TextureID.BEDROCK,
        //         VoxelID.GRASS   => face switch {
        //             Direction.NORTH => TextureID.GRASS_SIDE,
        //             Direction.EAST  => TextureID.GRASS_SIDE,
        //             Direction.SOUTH => TextureID.GRASS_SIDE,
        //             Direction.WEST  => TextureID.GRASS_SIDE,
        //             Direction.UP    => TextureID.GRASS,
        //             Direction.DOWN  => TextureID.DIRT,
        //             _               => TextureID.NULL
        //         },
        //         VoxelID.COBBLE  => TextureID.COBBLE,
        //         VoxelID.LIMESTONE   => TextureID.LIMESTONE,
        //         VoxelID.DIRT    => TextureID.DIRT,
        //         VoxelID.LOG     => face switch {
        //             Direction.NORTH => TextureID.LOG_SIDE,
        //             Direction.EAST  => TextureID.LOG_SIDE,
        //             Direction.SOUTH => TextureID.LOG_SIDE,
        //             Direction.WEST  => TextureID.LOG_SIDE,
        //             Direction.UP    => TextureID.LOG_TOP,
        //             Direction.DOWN  => TextureID.LOG_TOP,
        //             _               => TextureID.NULL
        //         },
        //         _               => TextureID.NULL
        //     };
        // }
    }
}