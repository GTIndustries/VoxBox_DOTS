using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
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
        WATER,
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

    public struct WorldData {
        public static readonly int3[] Directions = {
            new int3( 0,  0, -1), // North
            new int3( 1,  0,  0), // East
            new int3( 0,  0,  1), // South
            new int3(-1,  0,  0), // West
            new int3( 0,  1,  0), // Up
            new int3( 0, -1,  0), // Down
        };
    }

    public class TextureAtlas : MonoBehaviour/*, IDisposable*/ {
        [FormerlySerializedAs("_voxelMaterial")] [SerializeField] private Material voxelMat = null;
        public static Material voxelMaterial;
        
        //private static readonly int BaseMap    = Shader.PropertyToID("_MainTex"); // SRP
        //private static readonly int BaseMap    = Shader.PropertyToID("_BaseColorMap"); // HDRP
        private static readonly int BaseMap    = Shader.PropertyToID("_BaseMap"); // URP

        private void Awake() {
            //Debug.Log("TextureAtlas::Start: Materials loading");
            MaterialSetup();
            //Debug.Log("TextureAtlas::Start: Materials loaded!");
        }

        private void MaterialSetup() {
            ref var voxelAtlas = ref World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<VoxelRegistrationSystem>().voxelAtlas;
            voxelMaterial = new Material(voxelMat);
            var voxelTexture = voxelAtlas.GetSprite("debug").texture;
            voxelTexture.anisoLevel = 8;
            voxelMaterial.SetTexture(BaseMap, voxelTexture);
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