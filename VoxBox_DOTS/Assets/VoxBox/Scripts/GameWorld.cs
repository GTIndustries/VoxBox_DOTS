using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using VoxBox.Scripts.Components.Buffers;
using VoxBox.Scripts.Components.Tags;

namespace VoxBox.Scripts {
    public class GameWorld : MonoBehaviour {
        public       int viewDistanceHorizontal = 5;
        public       int viewDistanceVertical   = 5;
        public const int ChunkSize              = 25;

        [SerializeField] private GameObject                   chunkGameObjectPrefab     = null;
        [SerializeField] private GameObject                   chunkMeshGameObjectPrefab = null;
        [SerializeField] private Mesh                         defaultCubeMesh           = null;
        private                  Entity                       _chunkEntityPrefab;
        private                  Entity                       _chunkMeshEntityPrefab;
        private                  World                        _defaultWorld;
        private                  EntityManager                _entityManager;
        public static            Dictionary<int3, GameObject> chunks = new Dictionary<int3, GameObject>();

        private IEnumerator Start() {
            Debug.Log("GameWorld::Start: Beginning Setup...");
            //yield return null;

            _defaultWorld  = World.DefaultGameObjectInjectionWorld;
            _entityManager = _defaultWorld.EntityManager;

            using (var blobAssetStore = new BlobAssetStore()) {
                var conversionSettings = GameObjectConversionSettings.FromWorld(_defaultWorld, blobAssetStore);
                _chunkEntityPrefab =
                    GameObjectConversionUtility.ConvertGameObjectHierarchy(chunkGameObjectPrefab, conversionSettings);
                _chunkMeshEntityPrefab =
                    GameObjectConversionUtility.ConvertGameObjectHierarchy(
                        chunkMeshGameObjectPrefab,
                        conversionSettings
                    );
            }

            Debug.Log("GameWorld::Start: Setup complete!");
            //yield return null;

            Debug.Log("GameWorld::Start: Creating world...");
            //yield return null;

            CreateWorld();

            Debug.Log("GameWorld::Start: World done!");
            yield return null;
        }

        private void CreateWorld() {
            var chunkCount = 0;

            for (var y = viewDistanceVertical; y > -viewDistanceVertical; --y) {
                for (var x = -viewDistanceHorizontal; x < viewDistanceHorizontal; ++x) {
                    for (var z = -viewDistanceHorizontal; z < viewDistanceHorizontal; ++z) {
                        //Debug.Log($"Working... {x}, {y}, {z}");
                        // CreateChunk(new int3(x, y, z));

                        // only generate chunk if it is within the sphere of vision
                        var centerPosition = new Vector3(0, 0, 0);

                        if (Vector3.Distance(centerPosition, new Vector3(x, y, z)) < viewDistanceHorizontal
                         && Vector3.Distance(centerPosition, new Vector3(x, y, z)) < viewDistanceVertical) {
                            CreateChunk(
                                new int3(
                                    x * ChunkSize,
                                    y * ChunkSize,
                                    z * ChunkSize
                                )
                            );
                            ++chunkCount;
                        }
                    }
                }
            }

            Debug.Log($"Number of chunks: {chunkCount}");
        }

        private void CreateChunk(int3 worldPosition) {
            // Instantiate Chunk data entity
            var newChunk = _entityManager.Instantiate(_chunkEntityPrefab);
#if DEBUG
            _entityManager.SetName(newChunk, $"Chunk_{worldPosition.x}_{worldPosition.y}_{worldPosition.z}");
#endif
            _entityManager.SetComponentData(
                newChunk,
                new Translation {
                    Value = worldPosition
                }
            );
            // TODO: Create system to find serialized chunk data and only regen chunk if not saved
            _entityManager.AddComponentData(newChunk, new GenerateTerrainTag());

            // _entityManager.AddSharedComponentData(newChunk, new RenderMesh {
            //     mesh     = new Mesh(),
            //     material = TextureAtlas.voxelMaterial
            // });
            // _entityManager.AddComponentData(newChunk, new RenderBounds());
            // _entityManager.AddComponentData(newChunk, new LocalToWorld());
            // _entityManager.SetComponentData(newChunk, new Translation {
            //     Value = worldPosition
            // });
            // // TODO: Create system to find serialized chunk data and only regen chunk if not saved
            // _entityManager.AddComponent<GenerateTerrainTag>(newChunk);

            // Instantiate Chunk mesh entity
//             var newChunkMesh = _entityManager.Instantiate(_chunkMeshEntityPrefab);
// #if DEBUG
//             _entityManager.SetName(newChunkMesh, $"ChunkMesh_{worldPosition.x}_{worldPosition.y}_{worldPosition.z}");
// #endif
//             // _entityManager.AddSharedComponentData(
//             //     newChunkMesh,
//             //     new RenderMesh {
//             //         mesh     = new Mesh(),
//             //         material = TextureAtlas.voxelMaterial
//             //     }
//             // );
//
//             // _entityManager.AddComponentData(newChunkMesh, new RenderBounds());
//             // _entityManager.AddComponentData(newChunkMesh, new LocalToWorld());
//
//             //_entityManager.SetSharedComponentData(newChunkMesh, new RenderMesh());
//             _entityManager.SetComponentData(
//                 newChunkMesh,
//                 new Translation {
//                     Value = worldPosition
//                 }
//             );
//
//             // Give both entities a reference to the other
//             var entityBuffer = _entityManager.GetBuffer<EntityBufferElement>(newChunk);
//             entityBuffer.Add(newChunkMesh);
//
//             entityBuffer = _entityManager.GetBuffer<EntityBufferElement>(newChunkMesh);
//             entityBuffer.Add(newChunk);
        }
    }
}