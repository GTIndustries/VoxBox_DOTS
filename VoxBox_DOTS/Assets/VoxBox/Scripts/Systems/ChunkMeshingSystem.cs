using System;
using System.Collections.Generic;
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

    public struct CubeModel : IDisposable {
        public NativeArray<Vector3> Vertices;
        public NativeArray<Vector3> Normals;
        public NativeArray<Vector2> Uvs;
        public NativeArray<int>     Indices;

        public static CubeModel Create() {
            return new CubeModel {
                Vertices = new NativeArray<Vector3>(
                    new[] {
                        new Vector3(0f, 0f, 0f),
                        new Vector3(0f, 1f, 1f),
                        new Vector3(0f, 1f, 0f),
                        new Vector3(0f, 0f, 1f),
                        new Vector3(1f, 0f, 0f),
                        new Vector3(1f, 1f, 1f),
                        new Vector3(1f, 1f, 0f),
                        new Vector3(1f, 0f, 1f),
                        new Vector3(0f, 1f, 0f),
                        new Vector3(0f, 1f, 1f),
                        new Vector3(1f, 1f, 0f),
                        new Vector3(1f, 1f, 1f),
                        new Vector3(0f, 0f, 0f),
                        new Vector3(0f, 0f, 1f),
                        new Vector3(1f, 0f, 0f),
                        new Vector3(1f, 0f, 1f),
                        new Vector3(0f, 0f, 0f),
                        new Vector3(0f, 1f, 0f),
                        new Vector3(1f, 1f, 0f),
                        new Vector3(1f, 0f, 0f),
                        new Vector3(0f, 0f, 1f),
                        new Vector3(0f, 1f, 1f),
                        new Vector3(1f, 1f, 1f),
                        new Vector3(1f, 0f, 1f)
                    },
                    Allocator.Persistent
                ),
                Normals = new NativeArray<Vector3>(
                    new[] {
                        new Vector3(-1, 0, 0),
                        new Vector3(+1, 0, 0),
                        new Vector3(0, +1, 0),
                        new Vector3(0, -1, 0),
                        new Vector3(0, 0, -1),
                        new Vector3(0, 0, +1)
                    },
                    Allocator.Persistent
                ),
                Uvs = new NativeArray<Vector2>(
                    new[] {
                        new Vector2(1f, 0f),
                        new Vector2(0f, 1f),
                        new Vector2(1f, 1f),
                        new Vector2(0f, 0f),
                        new Vector2(0f, 0f),
                        new Vector2(1f, 1f),
                        new Vector2(0f, 1f),
                        new Vector2(1f, 0f),
                        new Vector2(0f, 0f),
                        new Vector2(0f, 1f),
                        new Vector2(1f, 0f),
                        new Vector2(1f, 1f),
                        new Vector2(0f, 0f),
                        new Vector2(0f, 1f),
                        new Vector2(1f, 0f),
                        new Vector2(1f, 1f),
                        new Vector2(0f, 0f),
                        new Vector2(0f, 1f),
                        new Vector2(1f, 1f),
                        new Vector2(1f, 0f),
                        new Vector2(1f, 0f),
                        new Vector2(1f, 1f),
                        new Vector2(0f, 1f),
                        new Vector2(0f, 0f)
                    },
                    Allocator.Persistent
                ),
                Indices = new NativeArray<int>(
                    new[] {
                        0,
                        1,
                        2,
                        0,
                        3,
                        1, // left -
                        0,
                        1,
                        3,
                        0,
                        2,
                        1, // right +
                        0,
                        3,
                        2,
                        0,
                        1,
                        3, // top +
                        0,
                        3,
                        1,
                        0,
                        2,
                        3, // bottom -
                        0,
                        2,
                        3,
                        0,
                        1,
                        2, // front +
                        0,
                        2,
                        1,
                        0,
                        3,
                        2 // back -
                    },
                    Allocator.Persistent
                )
            };
        }

        public void Dispose() {
            Vertices.Dispose();
            Normals.Dispose();
            Uvs.Dispose();
            Indices.Dispose();
        }
    }

    public class ChunkMeshingSystem : SystemBase {
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
        }

        protected override void OnUpdate() {
            var ecb  = _commandsBuffer.CreateCommandBuffer().ToConcurrent();
            var ecb2 = _commandsBuffer.CreateCommandBuffer();

            Entities.WithAll<ChunkTag, UpdateChunkTag, CreateMeshChunkTag>()
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
                                         if (voxelBuffer[index] == VoxelID.AIR) continue;

                                         // TODO: Calculate mesh
                                         var faceBuffer = visibleFacesBuffer[index];

                                         if (faceBuffer[0] != TextureID.AIR) {
                                             FaceDataNorth(
                                                 x,
                                                 y,
                                                 z,
                                                 ref vertexBuffer,
                                                 ref triangleBuffer
                                             );
                                         }

                                         if (faceBuffer[1] != TextureID.AIR) {
                                             FaceDataEast(
                                                 x,
                                                 y,
                                                 z,
                                                 ref vertexBuffer,
                                                 ref triangleBuffer
                                             );
                                         }

                                         if (faceBuffer[2] != TextureID.AIR) {
                                             FaceDataSouth(
                                                 x,
                                                 y,
                                                 z,
                                                 ref vertexBuffer,
                                                 ref triangleBuffer
                                             );
                                         }

                                         if (faceBuffer[3] != TextureID.AIR) {
                                             FaceDataWest(
                                                 x,
                                                 y,
                                                 z,
                                                 ref vertexBuffer,
                                                 ref triangleBuffer
                                             );
                                         }

                                         if (faceBuffer[4] != TextureID.AIR) {
                                             FaceDataUp(
                                                 x,
                                                 y,
                                                 z,
                                                 ref vertexBuffer,
                                                 ref triangleBuffer
                                             );
                                         }

                                         if (faceBuffer[5] != TextureID.AIR) {
                                             FaceDataDown(
                                                 x,
                                                 y,
                                                 z,
                                                 ref vertexBuffer,
                                                 ref triangleBuffer
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
                    .ScheduleParallel();

            // Entities.WithAll<DisableSystemTag>().WithAll<ChunkTag, UpdateChunkTag, RenderChunkTag>()
            //         .WithoutBurst()
            //         .ForEach(
            //              (
            //                  Entity                                   e,
            //                  int                                      entityInQueryIndex,
            //                  ref DynamicBuffer<VertexBufferElement>   vertexBuffer,
            //                  ref DynamicBuffer<NormalBufferElement>   normalBuffer,
            //                  ref DynamicBuffer<UVBufferElement>       uvBuffer,
            //                  ref DynamicBuffer<TriangleBufferElement> triangleBuffer,
            //                  in  Translation                          translation,
            //                  in  Rotation                             rotation
            //              ) => {
            //                  // create chunk mesh
            //                  // var mesh = new Mesh();
            //                  // SetMesh(
            //                  //     mesh,
            //                  //     vertexBuffer.Reinterpret<float3>(),
            //                  //     uvBuffer.Reinterpret<float2>(),
            //                  //     normalBuffer.Reinterpret<float3>(),
            //                  //     triangleBuffer.Reinterpret<int>()
            //                  // );
            //                  //
            //                  // Graphics.DrawMeshNow(mesh, translation.Value, rotation.Value);
            //
            //                  // Set done with meshing and updating
            //                  ecb2.RemoveComponent<UpdateChunkTag>(e);
            //                  ecb2.RemoveComponent<RenderChunkTag>(e);
            //              }
            //          )
            //         .Run();

            // Entities.WithAll<ChunkTag, UpdateChunkTag, CreateMeshChunkTag>()
            //         .ForEach(
            //              (
            //                  Entity                                       e,
            //                  int                                          entityInQueryIndex,
            //                  ref DynamicBuffer<VoxelBufferElement>        voxelBuffer,
            //                  ref DynamicBuffer<VertexBufferElement>       vertexBuffer,
            //                  ref DynamicBuffer<NormalBufferElement>       normalBuffer,
            //                  ref DynamicBuffer<UVBufferElement>           uvBuffer,
            //                  ref DynamicBuffer<TriangleBufferElement>     triangleBuffer,
            //                  ref DynamicBuffer<VisibleFacesBufferElement> visibleFacesBuffer
            //              ) => {
            //                  // for (var n = 0; n < 4; n++) {
            //                  //     // Our vertex model is specifically organized so that it'll work on any face
            //                  //     var vertexIndex = face * 4 + n;
            //                  //     var vertices    = ModelVertices[vertexIndex];
            //                  //     vertices.x              += start.x + (n == 2 || n == 3 ? dif.x : 0);
            //                  //     vertices.y              += start.y + (n == 1 || n == 2 ? dif.y : 0);
            //                  //     vertices.z              += start.z + (n == 1 || n == 3 ? dif.z : 0);
            //                  //     Vertices[vertCount + n] =  vertices;
            //                  //
            //                  //     Vector3 uv = ModelUVs[vertexIndex];
            //                  //     uv.x *= 1 + uvX;
            //                  //     uv.y *= 1 + uvY;
            //                  //     uv.z =  texture;
            //                  //
            //                  //     UVs[vertCount + n] = uv;
            //                  //
            //                  //     Normals[vertCount + n] = ModelNormals[face];
            //                  // }
            //                  //
            //                  // for (var n = 0; n < 6; n++)
            //                  //     Indices[trisCount + n] = vertCount + ModelIndices[face * 6 + n];
            //                  //
            //                  // // ...
            //              }
            //          )
            //         .ScheduleParallel();

            _commandsBuffer.AddJobHandleForProducer(Dependency);
        }

        private static int GetIndex(int x, int y, int z) {
            return x + (y * ChunkSize * ChunkSize) + (z * ChunkSize);
        }

        private static void FaceDataUp(
            int                                      x,
            int                                      y,
            int                                      z,
            ref DynamicBuffer<VertexBufferElement>   vertexBuffer,
            ref DynamicBuffer<TriangleBufferElement> triangleBuffer
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
        }

        private static void FaceDataDown(
            int                                      x,
            int                                      y,
            int                                      z,
            ref DynamicBuffer<VertexBufferElement>   vertexBuffer,
            ref DynamicBuffer<TriangleBufferElement> triangleBuffer
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
        }

        private static void FaceDataNorth(
            int                                      x,
            int                                      y,
            int                                      z,
            ref DynamicBuffer<VertexBufferElement>   vertexBuffer,
            ref DynamicBuffer<TriangleBufferElement> triangleBuffer
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
        }

        private static void FaceDataEast(
            int                                      x,
            int                                      y,
            int                                      z,
            ref DynamicBuffer<VertexBufferElement>   vertexBuffer,
            ref DynamicBuffer<TriangleBufferElement> triangleBuffer
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
        }

        private static void FaceDataSouth(
            int                                      x,
            int                                      y,
            int                                      z,
            ref DynamicBuffer<VertexBufferElement>   vertexBuffer,
            ref DynamicBuffer<TriangleBufferElement> triangleBuffer
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
        }

        private static void FaceDataWest(
            int                                      x,
            int                                      y,
            int                                      z,
            ref DynamicBuffer<VertexBufferElement>   vertexBuffer,
            ref DynamicBuffer<TriangleBufferElement> triangleBuffer
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
        }
    }
}