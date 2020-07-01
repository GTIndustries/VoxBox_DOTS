using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VoxBox.Scripts.Components.Buffers;
using VoxBox.Scripts.Components.Tags;

namespace VoxBox.Scripts.Systems {
    internal static class CustomForEach {
        // Declare the delegate that takes 12 parameters. T0 is used for the Entity argument
        [Unity.Entities.CodeGeneratedJobForEach.EntitiesForEachCompatible]
        public delegate void CustomForEachDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            T0     t0,
            T1     t1,
            ref T2 t2,
            ref T3 t3,
            ref T4 t4,
            ref T5 t5,
            ref T6 t6,
            ref T7 t7,
            in  T8 t8,
            in  T9 t9
        );

        // Declare the function overload
        public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            this TDescription                                             description,
            CustomForEachDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> codeToRun
        )
            where TDescription : struct, Unity.Entities.CodeGeneratedJobForEach.ISupportForEachWithUniversalDelegate =>
            LambdaForEachDescriptionConstructionMethods.ThrowCodeGenException<TDescription>();
    }

    public class ChunkMeshingSystem : SystemBase, IDisposable {
        private        World                                  _defaultWorld;
        private        EntityManager                          _entityManager;
        private static EndSimulationEntityCommandBufferSystem _commandsBuffer;           

        private const int ChunkSize = GameWorld.ChunkSize;
        // private readonly List<Vector3> verticesList  = new List<Vector3>();
        // private readonly List<Vector3> normalsList   = new List<Vector3>();
        // private readonly List<Vector2> uvsList       = new List<Vector2>();
        // private readonly List<int>     trianglesList = new List<int>();

        protected override void OnCreate() {
            _commandsBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            _defaultWorld  = World.DefaultGameObjectInjectionWorld;
            _entityManager = _defaultWorld.EntityManager;

            //_textureUVs = new NativeHashMap<int, UV>(1, Allocator.Persistent);
        }

        protected override void OnUpdate() {
            var group = GetEntityQuery(
                ComponentType.ReadOnly<ChunkTag>(),
                ComponentType.ReadOnly<UpdateChunkTag>(),
                ComponentType.ReadOnly<CreateMeshChunkTag>()
            );
            var ecb = _commandsBuffer.CreateCommandBuffer().ToConcurrent();

            if (group.CalculateEntityCount() != 0) {
                var textureUVs = new NativeArray<UV>(TextureAtlas.textureUVs.Count, Allocator.TempJob);
                textureUVs.CopyFrom(TextureAtlas.textureUVs.Values.ToArray());
                
                // var temp1 = TextureAtlas.textureUVs.Keys.ToArray();
                // var temp2 = TextureAtlas.textureUVs.Values.ToArray();
                
                // for (var i = 0; i < TextureAtlas.textureUVs.Count(); ++i) {
                //     if (!textureUVs.TryAdd(temp1[i], temp2[i])) Debug.Log("Texture failed to acquire");
                // }

                Entities.WithAll<ChunkTag, UpdateChunkTag, CreateMeshChunkTag>()
                        .WithReadOnly(textureUVs)
                        .ForEach(
                             (
                                 Entity                                       e,
                                 int                                          entityInQueryIndex,
                                 ref DynamicBuffer<VoxelBufferElement>        voxelBuffer,
                                 ref DynamicBuffer<VertexBufferElement>       vertexBuffer,
                                 ref DynamicBuffer<NormalBufferElement>       normalBuffer,
                                 ref DynamicBuffer<UVBufferElement>           uvBuffer,
                                 ref DynamicBuffer<TriangleBufferElement>     triangleBuffer,
                                 ref DynamicBuffer<VisibleFacesBufferElement> visibleFacesBuffer
                             ) => {
                                 vertexBuffer.Clear();
                                 normalBuffer.Clear();
                                 uvBuffer.Clear();
                                 triangleBuffer.Clear();

                                 // calculate chunk mesh
                                 for (var y = 0; y < ChunkSize; ++y) {
                                     for (var x = 0; x < ChunkSize; ++x) {
                                         for (var z = 0; z < ChunkSize; ++z) {
                                             var index = GetIndex(x, y, z);
                                             var voxel = voxelBuffer[index];
                                             //Debug.Log($"Voxel: {voxel.value}");
                                             if (voxel == VoxelID.AIR) continue;

                                             // TODO: Calculate mesh
                                             var faceBuffer = visibleFacesBuffer[index];
                                             // for (var i = 0; i < 6; ++i)
                                             //    Debug.Log($"{(Direction)i}: {faceBuffer[i]}");

                                             if (faceBuffer[0] != TextureID.AIR) {
                                                 FaceDataNorth(
                                                     x,
                                                     y,
                                                     z,
                                                     voxel,
                                                     ref vertexBuffer,
                                                     ref triangleBuffer,
                                                     ref uvBuffer,
                                                     textureUVs
                                                 );
                                             }

                                             if (faceBuffer[1] != TextureID.AIR) {
                                                 FaceDataEast(
                                                     x,
                                                     y,
                                                     z,
                                                     voxel,
                                                     ref vertexBuffer,
                                                     ref triangleBuffer,
                                                     ref uvBuffer,
                                                     textureUVs
                                                 );
                                             }

                                             if (faceBuffer[2] != TextureID.AIR) {
                                                 FaceDataSouth(
                                                     x,
                                                     y,
                                                     z,
                                                     voxel,
                                                     ref vertexBuffer,
                                                     ref triangleBuffer,
                                                     ref uvBuffer,
                                                     textureUVs
                                                 );
                                             }

                                             if (faceBuffer[3] != TextureID.AIR) {
                                                 FaceDataWest(
                                                     x,
                                                     y,
                                                     z,
                                                     voxel,
                                                     ref vertexBuffer,
                                                     ref triangleBuffer,
                                                     ref uvBuffer,
                                                     textureUVs
                                                 );
                                             }

                                             if (faceBuffer[4] != TextureID.AIR) {
                                                 FaceDataUp(
                                                     x,
                                                     y,
                                                     z,
                                                     voxel,
                                                     ref vertexBuffer,
                                                     ref triangleBuffer,
                                                     ref uvBuffer,
                                                     textureUVs
                                                 );
                                             }

                                             if (faceBuffer[5] != TextureID.AIR) {
                                                 FaceDataDown(
                                                     x,
                                                     y,
                                                     z,
                                                     voxel,
                                                     ref vertexBuffer,
                                                     ref triangleBuffer,
                                                     ref uvBuffer,
                                                     textureUVs
                                                 );
                                             }
                                         }
                                     }
                                 }

                                 // Set done with meshing and updating
                                 //ecb.RemoveComponent<UpdateChunkTag>(entityInQueryIndex, e);
                                 ecb.RemoveComponent<CreateMeshChunkTag>(
                                     entityInQueryIndex,
                                     e
                                 );
                                 ecb.AddComponent<RenderChunkTag>(entityInQueryIndex, e);
                             }
                         )
                        .WithDeallocateOnJobCompletion(textureUVs)
                        .ScheduleParallel();

                // temp1.Dispose();
                // temp2.Dispose();
            }

            //textureUVs.Dispose();
            _commandsBuffer.AddJobHandleForProducer(Dependency);
        }

        private static int GetIndex(int x, int y, int z) {
            return x + (y * ChunkSize * ChunkSize) + (z * ChunkSize);
        }

        private static void FaceDataUp(
            int                                      x,
            int                                      y,
            int                                      z,
            VoxelID                                  voxelID,
            ref DynamicBuffer<VertexBufferElement>   vertexBuffer,
            ref DynamicBuffer<TriangleBufferElement> triangleBuffer,
            ref DynamicBuffer<UVBufferElement>       uvBuffer,
            in  NativeArray<UV>               textureUVs
        ) {
            vertexBuffer.Add(new float3(x - 0.5f, y + 0.5f, z - 0.5f));
            vertexBuffer.Add(new float3(x - 0.5f, y + 0.5f, z + 0.5f));
            vertexBuffer.Add(new float3(x + 0.5f, y + 0.5f, z + 0.5f));
            vertexBuffer.Add(new float3(x + 0.5f, y + 0.5f, z - 0.5f));

            triangleBuffer.Add(vertexBuffer.Length - 4);
            triangleBuffer.Add(vertexBuffer.Length - 3);
            triangleBuffer.Add(vertexBuffer.Length - 2);

            triangleBuffer.Add(vertexBuffer.Length - 4);
            triangleBuffer.Add(vertexBuffer.Length - 2);
            triangleBuffer.Add(vertexBuffer.Length - 1);

            var uvs = new NativeArray<float2>(4, Allocator.Temp);
            FaceUVs(x, y, z, voxelID, Direction.UP, ref uvs, textureUVs);
            uvBuffer.Add(uvs[0]);
            uvBuffer.Add(uvs[1]);
            uvBuffer.Add(uvs[2]);
            uvBuffer.Add(uvs[3]);
            uvs.Dispose();
        }

        private static void FaceDataDown(
            int                                      x,
            int                                      y,
            int                                      z,
            VoxelID                                  voxelID,
            ref DynamicBuffer<VertexBufferElement>   vertexBuffer,
            ref DynamicBuffer<TriangleBufferElement> triangleBuffer,
            ref DynamicBuffer<UVBufferElement>       uvBuffer,
            in  NativeArray<UV>               textureUVs
        ) {
            vertexBuffer.Add(new float3(x - 0.5f, y - 0.5f, z + 0.5f));
            vertexBuffer.Add(new float3(x - 0.5f, y - 0.5f, z - 0.5f));
            vertexBuffer.Add(new float3(x + 0.5f, y - 0.5f, z - 0.5f));
            vertexBuffer.Add(new float3(x + 0.5f, y - 0.5f, z + 0.5f));

            triangleBuffer.Add(vertexBuffer.Length - 4);
            triangleBuffer.Add(vertexBuffer.Length - 3);
            triangleBuffer.Add(vertexBuffer.Length - 2);

            triangleBuffer.Add(vertexBuffer.Length - 4);
            triangleBuffer.Add(vertexBuffer.Length - 2);
            triangleBuffer.Add(vertexBuffer.Length - 1);

            var uvs = new NativeArray<float2>(4, Allocator.Temp);
            FaceUVs(x, y, z, voxelID, Direction.DOWN, ref uvs, textureUVs);
            uvBuffer.Add(uvs[0]);
            uvBuffer.Add(uvs[1]);
            uvBuffer.Add(uvs[2]);
            uvBuffer.Add(uvs[3]);
            uvs.Dispose();
        }

        private static void FaceDataNorth(
            int                                      x,
            int                                      y,
            int                                      z,
            VoxelID                                  voxelID,
            ref DynamicBuffer<VertexBufferElement>   vertexBuffer,
            ref DynamicBuffer<TriangleBufferElement> triangleBuffer,
            ref DynamicBuffer<UVBufferElement>       uvBuffer,
            in  NativeArray<UV>               textureUVs
        ) {
            vertexBuffer.Add(new float3(x - 0.5f, y - 0.5f, z - 0.5f));
            vertexBuffer.Add(new float3(x - 0.5f, y + 0.5f, z - 0.5f));
            vertexBuffer.Add(new float3(x + 0.5f, y + 0.5f, z - 0.5f));
            vertexBuffer.Add(new float3(x + 0.5f, y - 0.5f, z - 0.5f));

            triangleBuffer.Add(vertexBuffer.Length - 4);
            triangleBuffer.Add(vertexBuffer.Length - 3);
            triangleBuffer.Add(vertexBuffer.Length - 2);

            triangleBuffer.Add(vertexBuffer.Length - 4);
            triangleBuffer.Add(vertexBuffer.Length - 2);
            triangleBuffer.Add(vertexBuffer.Length - 1);

            var uvs = new NativeArray<float2>(4, Allocator.Temp);
            FaceUVs(x, y, z, voxelID, Direction.NORTH, ref uvs, textureUVs);
            uvBuffer.Add(uvs[0]);
            uvBuffer.Add(uvs[1]);
            uvBuffer.Add(uvs[2]);
            uvBuffer.Add(uvs[3]);
            uvs.Dispose();
        }

        private static void FaceDataEast(
            int                                      x,
            int                                      y,
            int                                      z,
            VoxelID                                  voxelID,
            ref DynamicBuffer<VertexBufferElement>   vertexBuffer,
            ref DynamicBuffer<TriangleBufferElement> triangleBuffer,
            ref DynamicBuffer<UVBufferElement>       uvBuffer,
            in  NativeArray<UV>               textureUVs
        ) {
            vertexBuffer.Add(new float3(x + 0.5f, y - 0.5f, z - 0.5f));
            vertexBuffer.Add(new float3(x + 0.5f, y + 0.5f, z - 0.5f));
            vertexBuffer.Add(new float3(x + 0.5f, y + 0.5f, z + 0.5f));
            vertexBuffer.Add(new float3(x + 0.5f, y - 0.5f, z + 0.5f));

            triangleBuffer.Add(vertexBuffer.Length - 4);
            triangleBuffer.Add(vertexBuffer.Length - 3);
            triangleBuffer.Add(vertexBuffer.Length - 2);

            triangleBuffer.Add(vertexBuffer.Length - 4);
            triangleBuffer.Add(vertexBuffer.Length - 2);
            triangleBuffer.Add(vertexBuffer.Length - 1);

            var uvs = new NativeArray<float2>(4, Allocator.Temp);
            FaceUVs(x, y, z, voxelID, Direction.EAST, ref uvs, textureUVs);
            uvBuffer.Add(uvs[0]);
            uvBuffer.Add(uvs[1]);
            uvBuffer.Add(uvs[2]);
            uvBuffer.Add(uvs[3]);
            uvs.Dispose();
        }

        private static void FaceDataSouth(
            int                                      x,
            int                                      y,
            int                                      z,
            VoxelID                                  voxelID,
            ref DynamicBuffer<VertexBufferElement>   vertexBuffer,
            ref DynamicBuffer<TriangleBufferElement> triangleBuffer,
            ref DynamicBuffer<UVBufferElement>       uvBuffer,
            in  NativeArray<UV>               textureUVs
        ) {
            vertexBuffer.Add(new float3(x + 0.5f, y - 0.5f, z + 0.5f));
            vertexBuffer.Add(new float3(x + 0.5f, y + 0.5f, z + 0.5f));
            vertexBuffer.Add(new float3(x - 0.5f, y + 0.5f, z + 0.5f));
            vertexBuffer.Add(new float3(x - 0.5f, y - 0.5f, z + 0.5f));

            triangleBuffer.Add(vertexBuffer.Length - 4);
            triangleBuffer.Add(vertexBuffer.Length - 3);
            triangleBuffer.Add(vertexBuffer.Length - 2);

            triangleBuffer.Add(vertexBuffer.Length - 4);
            triangleBuffer.Add(vertexBuffer.Length - 2);
            triangleBuffer.Add(vertexBuffer.Length - 1);

            var uvs = new NativeArray<float2>(4, Allocator.Temp);
            FaceUVs(x, y, z, voxelID, Direction.SOUTH, ref uvs, textureUVs);
            uvBuffer.Add(uvs[0]);
            uvBuffer.Add(uvs[1]);
            uvBuffer.Add(uvs[2]);
            uvBuffer.Add(uvs[3]);
            uvs.Dispose();
        }

        private static void FaceDataWest(
            int                                      x,
            int                                      y,
            int                                      z,
            VoxelID                                  voxelID,
            ref DynamicBuffer<VertexBufferElement>   vertexBuffer,
            ref DynamicBuffer<TriangleBufferElement> triangleBuffer,
            ref DynamicBuffer<UVBufferElement>       uvBuffer,
            in NativeArray<UV> textureUVs
        ) {
            vertexBuffer.Add(new float3(x - 0.5f, y - 0.5f, z + 0.5f));
            vertexBuffer.Add(new float3(x - 0.5f, y + 0.5f, z + 0.5f));
            vertexBuffer.Add(new float3(x - 0.5f, y + 0.5f, z - 0.5f));
            vertexBuffer.Add(new float3(x - 0.5f, y - 0.5f, z - 0.5f));

            triangleBuffer.Add(vertexBuffer.Length - 4);
            triangleBuffer.Add(vertexBuffer.Length - 3);
            triangleBuffer.Add(vertexBuffer.Length - 2);

            triangleBuffer.Add(vertexBuffer.Length - 4);
            triangleBuffer.Add(vertexBuffer.Length - 2);
            triangleBuffer.Add(vertexBuffer.Length - 1);

            var uvs = new NativeArray<float2>(4, Allocator.Temp);
            FaceUVs(x, y, z, voxelID, Direction.WEST, ref uvs, textureUVs);
            uvBuffer.Add(uvs[0]);
            uvBuffer.Add(uvs[1]);
            uvBuffer.Add(uvs[2]);
            uvBuffer.Add(uvs[3]);
            uvs.Dispose();
        }

        public static void FaceUVs(
            int                        x,
            int                        y,
            int                        z,
            VoxelID                    voxelID,
            Direction                  direction,
            ref NativeArray<float2>    uvs,
            in NativeArray<UV> textureUVs
        ) {
            var uv = textureUVs[(int)TextureAtlas.GetFaceTexture(voxelID, direction)];
            //var fastNoise = new FastNoise();

            var rotation = 0;
            // if (ShouldRotate(direction)) {
            //     rotation = Mathf.RoundToInt(
            //         (Terrain.fastNoise.GetSimplex(
            //              (x / (float)Chunk.chunkSize), 
            //              (y / (float)Chunk.chunkSize), 
            //              (z / (float)Chunk.chunkSize)) * 16f) % 4);
            // }

            switch (rotation) {
                case 0:
                    uvs[1] = new float2(uv.uv0.x, uv.uv0.y);
                    uvs[2] = new float2(uv.uv1.x, uv.uv1.y);
                    uvs[0] = new float2(uv.uv2.x, uv.uv2.y);
                    uvs[3] = new float2(uv.uv3.x, uv.uv3.y);
                    // Debug.Log(uv.uv0);
                    // Debug.Log(uv.uv1);
                    // Debug.Log(uv.uv2);
                    // Debug.Log(uv.uv3);
                    break;
                case 1:
                    uvs[2] = new float2(uv.uv0.x, uv.uv0.y);
                    uvs[3] = new float2(uv.uv1.x, uv.uv1.y);
                    uvs[1] = new float2(uv.uv2.x, uv.uv2.y);
                    uvs[0] = new float2(uv.uv3.x, uv.uv3.y);
                    break;
                case 2:
                    uvs[3] = new float2(uv.uv0.x, uv.uv0.y);
                    uvs[0] = new float2(uv.uv1.x, uv.uv1.y);
                    uvs[2] = new float2(uv.uv2.x, uv.uv2.y);
                    uvs[1] = new float2(uv.uv3.x, uv.uv3.y);
                    break;
                case 3:
                    uvs[0] = new float2(uv.uv0.x, uv.uv0.y);
                    uvs[1] = new float2(uv.uv1.x, uv.uv1.y);
                    uvs[3] = new float2(uv.uv2.x, uv.uv2.y);
                    uvs[2] = new float2(uv.uv3.x, uv.uv3.y);
                    break;
                default:
                    uvs[2] = new float2(uv.uv0.x, uv.uv0.y);
                    uvs[1] = new float2(uv.uv1.x, uv.uv1.y);
                    uvs[3] = new float2(uv.uv2.x, uv.uv2.y);
                    uvs[0] = new float2(uv.uv3.x, uv.uv3.y);
                    break;
            }
        }

        public static UV GetTextureUVs(TextureID textureID, NativeArray<UV> uvs) {
            return textureID switch {
                TextureID.NULL       => uvs[0],
                TextureID.LOGO       => uvs[1],
                TextureID.AIR        => uvs[2],
                TextureID.BEDROCK    => uvs[3],
                TextureID.GRASS      => uvs[4],
                TextureID.GRASS_SIDE => uvs[5],
                TextureID.COBBLE     => uvs[6],
                TextureID.LIMESTONE  => uvs[7],
                TextureID.DIRT       => uvs[8],
                TextureID.LOG_TOP    => uvs[9],
                TextureID.LOG_SIDE   => uvs[10],
                _                    => uvs[0]
            };
        }

        public void Dispose() { }
    }
}