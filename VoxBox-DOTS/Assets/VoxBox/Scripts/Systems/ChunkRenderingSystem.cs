using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using VoxBox.Scripts.Components.Buffers;
using VoxBox.Scripts.Components.Tags;

namespace VoxBox.Scripts.Systems {
    internal static partial class CustomForEach {
        // Declare the delegate that takes 12 parameters. T0 is used for the Entity argument
        [Unity.Entities.CodeGeneratedJobForEach.EntitiesForEachCompatible]
        public delegate void CustomForEachDelegate2<T0, T1, T2, T3, T4, T5, T6, T7, T8>(
            T0     t0,
            T1     t1,
            ref T2 t2,
            in  T3 t3,
            in  T4 t4,
            in  T5 t5,
            in  T6 t6,
            in  T7 t7,
            in  T8 t8
        );

        // Declare the function overload
        public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7, T8>(
            this TDescription                                          description,
            CustomForEachDelegate2<T0, T1, T2, T3, T4, T5, T6, T7, T8> codeToRun
        )
            where TDescription : struct, Unity.Entities.CodeGeneratedJobForEach.ISupportForEachWithUniversalDelegate =>
            LambdaForEachDescriptionConstructionMethods.ThrowCodeGenException<TDescription>();
    }

    public class ChunkRenderingSystem : SystemBase {
        private        World                                  _defaultWorld;
        private        EntityManager                          _entityManager;
        private static EndSimulationEntityCommandBufferSystem _commandsBuffer;
        private static Dictionary<int3, GameObject>           chunks = new Dictionary<int3, GameObject>();

        protected override void OnCreate() {
            //Debug.Log("Created System: ChunkRendering");
            _defaultWorld   = World.DefaultGameObjectInjectionWorld;
            _entityManager  = _defaultWorld.EntityManager;
            _commandsBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            ApplyMeshData();
            //CleanGameObjects();
        }

        private void ApplyMeshData() {
            var ecb = _commandsBuffer.CreateCommandBuffer();

            Entities //.WithAll<DisableSystemTag>()
               .WithAll<ChunkTag, UpdateChunkTag, RenderChunkTag>()
               .WithoutBurst()
               .ForEach(
                    (
                        Entity e,
                        int    entityInQueryIndex,
                        //ref DynamicBuffer<EntityBufferElement>  entityBuffer,
                        in DynamicBuffer<VertexBufferElement> vertexBuffer,
                        //in DynamicBuffer<NormalBufferElement>   normalBuffer,
                        in DynamicBuffer<UVBufferElement>       uvBuffer,
                        in DynamicBuffer<TriangleBufferElement> triangleBuffer,
                        in Translation                          translation,
                        in Rotation                             rotation
                    ) => {
                        //Debug.Log("Drawing mesh");
                        // create chunk mesh
                        //var chunkMeshEntity = entityBuffer[0].value;
                        if (vertexBuffer.Length <= 0) return;
                        
                        var mesh = new Mesh();

                        SetMesh(
                            ref mesh,
                            vertexBuffer.Reinterpret<float3>(),
                            uvBuffer.Reinterpret<float2>(),
                            //normalBuffer.Reinterpret<float3>(),
                            triangleBuffer.Reinterpret<int>()
                        );

                        // TODO: Create pool of gameobjects to recycle for new chunks to use to avoid instantiation of new go's
                        //Create a gameobject for the rendering
                        var go       = new GameObject($"Chunk ({e.Index}): {(int3)translation.Value}");
                        var filter   = go.AddComponent<MeshFilter>();
                        var renderer = go.AddComponent<MeshRenderer>();
                        var collider = go.AddComponent<MeshCollider>();
                        filter.sharedMesh       = mesh;
                        renderer.sharedMaterial = TextureAtlas.voxelMaterial;
                        collider.sharedMesh     = mesh;
                        go.transform.position   = translation.Value;
                        go.isStatic             = true;

                        if (!chunks.TryGetValue(
                                new int3(
                                    (int)translation.Value.x,
                                    (int)translation.Value.y,
                                    (int)translation.Value.z
                                ),
                                out var _
                            )) {
                            chunks.Add(
                                new int3(
                                    (int)translation.Value.x,
                                    (int)translation.Value.y,
                                    (int)translation.Value.z
                                ),
                                go
                            );
                        }

                        // Set done with meshing and updating
                        ecb.RemoveComponent<UpdateChunkTag>(e);
                        ecb.RemoveComponent<RenderChunkTag>(e);
                    }
                )
               .Run();
               //.Schedule();

            _commandsBuffer.AddJobHandleForProducer(Dependency);
        }

        // Destroy gameobjects attached to chunks that no longer exist
        private void CleanGameObjects() {
            var ecb = _commandsBuffer.CreateCommandBuffer();
            Entities
               .WithAll<ChunkTag, DestroyChunkTag>()
               .WithoutBurst()
               .ForEach(
                    (
                        Entity         e,
                        in Translation translation
                    ) => {
                        if (chunks.TryGetValue(
                            new int3(
                                (int)translation.Value.x,
                                (int)translation.Value.y,
                                (int)translation.Value.z
                            ),
                            out var _
                        )) {
                            GameObject.Destroy(chunks[(int3)translation.Value]);
                            chunks.Remove((int3)translation.Value);

                            ecb.RemoveComponent<DestroyChunkTag>(e);
                        }
                    }
                )
               .Run();

            _commandsBuffer.AddJobHandleForProducer(Dependency);
        }

        private void SetMesh(
            ref Mesh              mesh,
            DynamicBuffer<float3> vertices,
            DynamicBuffer<float2> uvs,
            //DynamicBuffer<float3> normals,
            DynamicBuffer<int> triangles
        ) {
            mesh.Clear();

            if (vertices.Length == 0) {
                return;
            }

            mesh.SetVertices(vertices.AsNativeArray());
            //mesh.SetNormals(normals.AsNativeArray());
            mesh.SetUVs(0, uvs.AsNativeArray());
            mesh.SetIndices(triangles.AsNativeArray(), MeshTopology.Triangles, 0);
            //mesh.RecalculateBounds();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            // this.verticesList.Clear();
            // this.normalsList.Clear();
            // this.uvsList.Clear();
            // this.trianglesList.Clear();
        }
    }
}