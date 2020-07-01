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
    public class ChunkRenderingSystem : SystemBase {
        private        World                                  _defaultWorld;
        private        EntityManager                          _entityManager;
        private static EndSimulationEntityCommandBufferSystem _commandsBuffer;

        protected override void OnCreate() {
            _defaultWorld   = World.DefaultGameObjectInjectionWorld;
            _entityManager  = _defaultWorld.EntityManager;
            _commandsBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            var ecb = _commandsBuffer.CreateCommandBuffer();


            Entities //.WithAll<DisableSystemTag>()
               .WithAll<ChunkTag, UpdateChunkTag, RenderChunkTag>()
               .WithoutBurst()
               .WithStructuralChanges()
               .ForEach(
                    (
                        Entity                                  e,
                        int                                     entityInQueryIndex,
                        in DynamicBuffer<VertexBufferElement>   vertexBuffer,
                        in DynamicBuffer<NormalBufferElement>   normalBuffer,
                        in DynamicBuffer<UVBufferElement>       uvBuffer,
                        in DynamicBuffer<TriangleBufferElement> triangleBuffer,
                        in Translation                          translation,
                        in Rotation                             rotation
                    ) => {
                        //Debug.Log("Drawing mesh");
                        // create chunk mesh
                        var mesh = new Mesh();

                        if (_entityManager.HasComponent<RenderMesh>(e)) {
                            var renderMesh =
                                _entityManager.GetSharedComponentData<RenderMesh>(e);
                            mesh = renderMesh.mesh;
                        }

                        SetMesh(
                            ref mesh,
                            vertexBuffer.Reinterpret<float3>(),
                            uvBuffer.Reinterpret<float2>(),
                            normalBuffer.Reinterpret<float3>(),
                            triangleBuffer.Reinterpret<int>()
                        );

                        //Graphics.DrawMeshNow(mesh, translation.Value, rotation.Value);
                        
                        // if (_entityManager.HasComponent<RenderMesh>(e)) {
                        //     var renderMesh =
                        //         _entityManager.GetSharedComponentData<RenderMesh>(e);
                        //     _entityManager.SetSharedComponentData(
                        //         e,
                        //         new RenderMesh() {
                        //             mesh = mesh, 
                        //             material = TextureAtlas.voxelMaterial
                        //         }
                        //     );
                        // }
                        if (_entityManager.HasComponent<RenderBounds>(e)) {
                            //Debug.Log("Set RenderBounds");
                            _entityManager.SetComponentData(
                                e,
                                new RenderBounds() {
                                    Value = mesh.bounds.ToAABB()
                                }
                            );
                        }

                        // Set done with meshing and updating
                        _entityManager.RemoveComponent<UpdateChunkTag>(e);
                        _entityManager.RemoveComponent<RenderChunkTag>(e);
                    }
                )
               .Run();

            _commandsBuffer.AddJobHandleForProducer(Dependency);
        }

        private static void SetMesh(
            ref Mesh                  mesh,
            DynamicBuffer<float3> vertices,
            DynamicBuffer<float2> uvs,
            DynamicBuffer<float3> normals,
            DynamicBuffer<int>    triangles
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
            mesh.RecalculateNormals();
            // this.verticesList.Clear();
            // this.normalsList.Clear();
            // this.uvsList.Clear();
            // this.trianglesList.Clear();
        }
    }
}