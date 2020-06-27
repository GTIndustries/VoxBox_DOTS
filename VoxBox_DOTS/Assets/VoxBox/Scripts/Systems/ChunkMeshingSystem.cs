using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VoxBox.Scripts.Components.Buffers;
using VoxBox.Scripts.Components.Tags;

namespace VoxBox.Scripts.Systems {
    static class CustomForEach {
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
            var ecb = _commandsBuffer.CreateCommandBuffer().ToConcurrent();
            var ecb2 = _commandsBuffer.CreateCommandBuffer();

            Entities.WithAll<ChunkTag, UpdateChunkTag, RenderChunkTag>()
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
                             // calculate chunk mesh
                             for (var y = 0; y < ChunkSize; ++y) {
                                 for (var x = 0; x < ChunkSize; ++x) {
                                     for (var z = 0; z < ChunkSize; ++z) {
                                         // TODO: Calculate mesh
                                         for (var d = 0; d < 6; ++d) {
                                             switch ((Direction)d) {
                                                 case Direction.NORTH:
                                                     if (visibleFacesBuffer[GetIndex(x, y, z)][d]
                                                      != TextureID.AIR) {
                                                         FaceDataNorth(x, y, z, ref vertexBuffer, ref triangleBuffer);
                                                     }

                                                     break;
                                                 case Direction.EAST:
                                                     if (visibleFacesBuffer[GetIndex(x, y, z)][d]
                                                      != TextureID.AIR) {
                                                         FaceDataEast(x, y, z, ref vertexBuffer, ref triangleBuffer);
                                                     }

                                                     break;
                                                 case Direction.SOUTH:
                                                     if (visibleFacesBuffer[GetIndex(x, y, z)][d]
                                                      != TextureID.AIR) {
                                                         FaceDataSouth(x, y, z, ref vertexBuffer, ref triangleBuffer);
                                                     }

                                                     break;
                                                 case Direction.WEST:
                                                     if (visibleFacesBuffer[GetIndex(x, y, z)][d]
                                                      != TextureID.AIR) {
                                                         FaceDataWest(x, y, z, ref vertexBuffer, ref triangleBuffer);
                                                     }

                                                     break;
                                                 case Direction.UP:
                                                     if (visibleFacesBuffer[GetIndex(x, y, z)][d]
                                                      != TextureID.AIR) {
                                                         FaceDataUp(x, y, z, ref vertexBuffer, ref triangleBuffer);
                                                     }

                                                     break;
                                                 case Direction.DOWN:
                                                     if (visibleFacesBuffer[GetIndex(x, y, z)][d]
                                                      != TextureID.AIR) {
                                                         FaceDataDown(x, y, z, ref vertexBuffer, ref triangleBuffer);
                                                     }

                                                     break;
                                                 default:
                                                     throw new ArgumentOutOfRangeException();
                                             }
                                         }
                                     }
                                 }
                             } 
                             
                             // Set done with meshing and updating
                             ecb.RemoveComponent<UpdateChunkTag>(entityInQueryIndex, e);
                             ecb.RemoveComponent<RenderChunkTag>(entityInQueryIndex, e);
                             ecb.AddComponent<MeshedChunkTag>(entityInQueryIndex, e);
                         }
                     )
                    .ScheduleParallel();

            // Entities.WithAll<ChunkTag, UpdateChunkTag, RenderChunkTag>()
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
            //                  ecb2.AddComponent<MeshedChunkTag>(e);
            //              }
            //          )
            //         .Run();
            
            _commandsBuffer.AddJobHandleForProducer(Dependency);
        }

        private static int GetIndex(int x, int y, int z) {
            return x + (y * ChunkSize * ChunkSize) + (z * ChunkSize);
        }

        private static void SetMesh(
            Mesh                  mesh,
            DynamicBuffer<float3> vertices,
            DynamicBuffer<float2> uvs,
            DynamicBuffer<float3> normals,
            DynamicBuffer<int>    triangles
        ) {
            mesh.Clear();

            if (vertices.Length == 0) {
                return;
            }

            mesh.SetVertices(vertices.AsNativeArray() /*, 0, vertices.AsNativeArray().Length*/);
            //mesh.SetNormals(normals.AsNativeArray(), 0, vertices.AsNativeArray().Length);
            mesh.SetUVs(0, uvs.AsNativeArray() /*, 0, vertices.AsNativeArray().Length*/);
            mesh.SetIndices(
                triangles.AsNativeArray() /*, 0, triangles.AsNativeArray().Length*/,
                MeshTopology.Triangles,
                0
            );
            mesh.RecalculateNormals();
            // this.verticesList.Clear();
            // this.normalsList.Clear();
            // this.uvsList.Clear();
            // this.trianglesList.Clear();
        }

        private static void FaceDataUp(
            int                                      x,
            int                                      y,
            int                                      z,
            ref DynamicBuffer<VertexBufferElement>   vertexBuffer,
            ref DynamicBuffer<TriangleBufferElement> triangleBuffer
        ) {
            vertexBuffer.Add(new float3(x - 0.5f, y + 0.5f, z + 0.5f));
            vertexBuffer.Add(new float3(x + 0.5f, y + 0.5f, z + 0.5f));
            vertexBuffer.Add(new float3(x + 0.5f, y + 0.5f, z - 0.5f));
            vertexBuffer.Add(new float3(x - 0.5f, y + 0.5f, z - 0.5f));

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
            vertexBuffer.Add(new float3(x - 0.5f, y - 0.5f, z - 0.5f));
            vertexBuffer.Add(new float3(x + 0.5f, y - 0.5f, z - 0.5f));
            vertexBuffer.Add(new float3(x + 0.5f, y - 0.5f, z + 0.5f));
            vertexBuffer.Add(new float3(x - 0.5f, y - 0.5f, z + 0.5f));

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