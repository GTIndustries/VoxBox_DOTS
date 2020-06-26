using System;
using Unity.Mathematics;
using UnityEngine;

namespace VoxBox.Scripts.VoxelTypes {
    [Serializable]
    public class Voxel {
        private VoxelID _voxelID;
        public VoxelID VoxelID {
            get {
                return _voxelID;
            }
            private protected set {
                _voxelID = value;
            }
        }

        public bool changed = true;

        public Voxel() {
            VoxelID = VoxelID.NULL;
        }

        // public virtual MeshData VoxelData(Chunk chunk, int x, int y, int z, MeshData meshData) {
        //     // IF THIS FUNCTION IS OVERRIDEN, REMEMBER TO INCLUDE THIS LINE
        //     // OR ELSE YOU WILL USE THE SETTING OF THE LAST BLOCK
        //     meshData.useRenderDataForCollider = true;
        //
        //     if (!chunk.GetVoxel(x, y + 1, z).IsSolid(Direction.DOWN) || chunk.GetVoxel(x, y + 1, z).IsTransparent(Direction.DOWN)) {
        //         meshData = FaceDataUp(chunk, x, y, z, meshData);
        //     }
        //     if (!chunk.GetVoxel(x, y - 1, z).IsSolid(Direction.UP) || chunk.GetVoxel(x, y - 1, z).IsTransparent(Direction.UP)) {
        //         meshData = FaceDataDown(chunk, x, y, z, meshData);
        //     }
        //     if (!chunk.GetVoxel(x, y, z + 1).IsSolid(Direction.SOUTH) || chunk.GetVoxel(x, y, z + 1).IsTransparent(Direction.SOUTH)) {
        //         meshData = FaceDataNorth(chunk, x, y, z, meshData);
        //     }
        //     if (!chunk.GetVoxel(x, y, z - 1).IsSolid(Direction.NORTH) || chunk.GetVoxel(x, y, z - 1).IsTransparent(Direction.NORTH)) {
        //         meshData = FaceDataSouth(chunk, x, y, z, meshData);
        //     }
        //     if (!chunk.GetVoxel(x + 1, y, z).IsSolid(Direction.WEST) || chunk.GetVoxel(x + 1, y, z).IsTransparent(Direction.WEST)) {
        //         meshData = FaceDataEast(chunk, x, y, z, meshData);
        //     }
        //     if (!chunk.GetVoxel(x - 1, y, z).IsSolid(Direction.EAST) || chunk.GetVoxel(x - 1, y, z).IsTransparent(Direction.EAST)) {
        //         meshData = FaceDataWest(chunk, x, y, z, meshData);
        //     }
        //
        //     return meshData;
        // }
        //
        // protected virtual MeshData FaceDataUp(Chunk chunk, int x, int y, int z, MeshData meshData) {
        //     meshData.AddVertex(new float3(x - 0.5f, y + 0.5f, z - 0.5f));
        //     meshData.AddVertex(new float3(x - 0.5f, y + 0.5f, z + 0.5f));
        //     meshData.AddVertex(new float3(x + 0.5f, y + 0.5f, z + 0.5f));
        //     meshData.AddVertex(new float3(x + 0.5f, y + 0.5f, z - 0.5f));
        //
        //     meshData.AddQuadTriangles();
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.UP)[0]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.UP)[1]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.UP)[2]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.UP)[3]);
        //
        //     return meshData;
        // }
        //
        // protected virtual MeshData FaceDataDown(Chunk chunk, int x, int y, int z, MeshData meshData) {
        //     meshData.AddVertex(new float3(x - 0.5f, y - 0.5f, z + 0.5f));
        //     meshData.AddVertex(new float3(x - 0.5f, y - 0.5f, z - 0.5f));
        //     meshData.AddVertex(new float3(x + 0.5f, y - 0.5f, z - 0.5f));
        //     meshData.AddVertex(new float3(x + 0.5f, y - 0.5f, z + 0.5f));
        //
        //     meshData.AddQuadTriangles();
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.DOWN)[0]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.DOWN)[1]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.DOWN)[2]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.DOWN)[3]);
        //  
        //     return meshData;
        // }
        //
        // protected virtual MeshData FaceDataNorth(Chunk chunk, int x, int y, int z, MeshData meshData) {
        //     meshData.AddVertex(new float3(x + 0.5f, y - 0.5f, z + 0.5f));
        //     meshData.AddVertex(new float3(x + 0.5f, y + 0.5f, z + 0.5f));
        //     meshData.AddVertex(new float3(x - 0.5f, y + 0.5f, z + 0.5f));
        //     meshData.AddVertex(new float3(x - 0.5f, y - 0.5f, z + 0.5f));
        //
        //     meshData.AddQuadTriangles();
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.NORTH)[0]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.NORTH)[1]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.NORTH)[2]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.NORTH)[3]);
        //  
        //     return meshData;
        // }
        //
        // protected virtual MeshData FaceDataEast(Chunk chunk, int x, int y, int z, MeshData meshData) {
        //     meshData.AddVertex(new float3(x + 0.5f, y - 0.5f, z - 0.5f));
        //     meshData.AddVertex(new float3(x + 0.5f, y + 0.5f, z - 0.5f));
        //     meshData.AddVertex(new float3(x + 0.5f, y + 0.5f, z + 0.5f));
        //     meshData.AddVertex(new float3(x + 0.5f, y - 0.5f, z + 0.5f));
        //
        //     meshData.AddQuadTriangles();
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.EAST)[0]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.EAST)[1]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.EAST)[2]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.EAST)[3]);
        //  
        //     return meshData;
        // }
        //
        // protected virtual MeshData FaceDataSouth(Chunk chunk, int x, int y, int z, MeshData meshData) {
        //     meshData.AddVertex(new float3(x - 0.5f, y - 0.5f, z - 0.5f));
        //     meshData.AddVertex(new float3(x - 0.5f, y + 0.5f, z - 0.5f));
        //     meshData.AddVertex(new float3(x + 0.5f, y + 0.5f, z - 0.5f));
        //     meshData.AddVertex(new float3(x + 0.5f, y - 0.5f, z - 0.5f));
        //
        //     meshData.AddQuadTriangles();
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.SOUTH)[0]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.SOUTH)[1]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.SOUTH)[2]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.SOUTH)[3]);
        //  
        //     return meshData;
        // }
        //
        // protected virtual MeshData FaceDataWest(Chunk chunk, int x, int y, int z, MeshData meshData) {
        //     meshData.AddVertex(new float3(x - 0.5f, y - 0.5f, z + 0.5f));
        //     meshData.AddVertex(new float3(x - 0.5f, y + 0.5f, z + 0.5f));
        //     meshData.AddVertex(new float3(x - 0.5f, y + 0.5f, z - 0.5f));
        //     meshData.AddVertex(new float3(x - 0.5f, y - 0.5f, z - 0.5f));
        //
        //     meshData.AddQuadTriangles();
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.WEST)[0]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.WEST)[1]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.WEST)[2]);
        //     meshData.uv.Add(FaceUVs(x, y, z, Direction.WEST)[3]);
        //  
        //     return meshData;
        // }

        public virtual bool IsSolid(Direction direction) {
            return direction switch {
                Direction.NORTH => true,
                Direction.EAST  => true,
                Direction.SOUTH => true,
                Direction.WEST  => true,
                Direction.UP    => true,
                Direction.DOWN  => true,
                _               => false
            };
        }
    
        public virtual bool IsOpaque(Direction direction) {
            return direction switch {
                Direction.NORTH => true,
                Direction.EAST  => true,
                Direction.SOUTH => true,
                Direction.WEST  => true,
                Direction.UP    => true,
                Direction.DOWN  => true,
                _               => false
            };
        }
    
        public virtual bool ShouldRotate(Direction direction) {
            return direction switch {
                Direction.NORTH => false,
                Direction.EAST  => false,
                Direction.SOUTH => false,
                Direction.WEST  => false,
                Direction.UP    => false,
                Direction.DOWN  => false,
                _               => false
            };
        }
    
        public virtual TextureID GetFaceTexture(Direction face) {
            return face switch {
                Direction.NORTH => TextureID.NULL,
                Direction.EAST  => TextureID.NULL,
                Direction.SOUTH => TextureID.NULL,
                Direction.WEST  => TextureID.NULL,
                Direction.UP    => TextureID.NULL,
                Direction.DOWN  => TextureID.NULL,
                _               => TextureID.NULL
            };
        }
    
        // public virtual float2[] FaceUVs(int x, int y, int z, Direction direction) {
        //     var UVs = new float2[4];
        //     var uv  = TextureAtlas.textureUvs[GetFaceTexture(direction)];
        //     //var fastNoise = new FastNoise();
        //
        //     var rotation = 0;
        //     if (ShouldRotate(direction)) {
        //         rotation = Mathf.RoundToInt(
        //             (Terrain.fastNoise.GetSimplex(
        //                  (x / (float)Chunk.chunkSize), 
        //                  (y / (float)Chunk.chunkSize), 
        //                  (z / (float)Chunk.chunkSize)) * 16f) % 4);
        //     }
        //
        //     switch (rotation) {
        //         case 0:
        //             UVs[1] = new float2(uv.uv0.x, uv.uv0.y);
        //             UVs[2] = new float2(uv.uv1.x, uv.uv1.y);
        //             UVs[0] = new float2(uv.uv2.x, uv.uv2.y);
        //             UVs[3] = new float2(uv.uv3.x, uv.uv3.y);
        //             // Debug.Log(uv.uv0);
        //             // Debug.Log(uv.uv1);
        //             // Debug.Log(uv.uv2);
        //             // Debug.Log(uv.uv3);
        //             break;
        //         case 1:
        //             UVs[2] = new float2(uv.uv0.x, uv.uv0.y);
        //             UVs[3] = new float2(uv.uv1.x, uv.uv1.y);
        //             UVs[1] = new float2(uv.uv2.x, uv.uv2.y);
        //             UVs[0] = new float2(uv.uv3.x, uv.uv3.y);
        //             break;
        //         case 2:
        //             UVs[3] = new float2(uv.uv0.x, uv.uv0.y);
        //             UVs[0] = new float2(uv.uv1.x, uv.uv1.y);
        //             UVs[2] = new float2(uv.uv2.x, uv.uv2.y);
        //             UVs[1] = new float2(uv.uv3.x, uv.uv3.y);
        //             break;
        //         case 3:
        //             UVs[0] = new float2(uv.uv0.x, uv.uv0.y);
        //             UVs[1] = new float2(uv.uv1.x, uv.uv1.y);
        //             UVs[3] = new float2(uv.uv2.x, uv.uv2.y);
        //             UVs[2] = new float2(uv.uv3.x, uv.uv3.y);
        //             break;
        //         default:
        //             UVs[2] = new float2(uv.uv0.x, uv.uv0.y);
        //             UVs[1] = new float2(uv.uv1.x, uv.uv1.y);
        //             UVs[3] = new float2(uv.uv2.x, uv.uv2.y);
        //             UVs[0] = new float2(uv.uv3.x, uv.uv3.y);
        //             break;
        //     }
        //
        //     return UVs;
        // }
    }
}