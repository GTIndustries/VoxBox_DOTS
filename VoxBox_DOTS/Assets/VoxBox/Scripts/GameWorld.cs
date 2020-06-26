using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VoxBox.Scripts.Components.Buffers;
using VoxBox.Scripts.Components.Tags;

namespace VoxBox.Scripts {
    public class GameWorld : MonoBehaviour {
                         public        int           viewDistanceHorizontal    = 5;
                         public        int           viewDistanceVertical      = 5;
                         public  const int           ChunkSize                 = 25;
        
        [SerializeField] private       GameObject    chunkGameObjectPrefab     = null; 
        [SerializeField] private       GameObject    chunkMeshGameObjectPrefab = null;
                         private       Entity        _chunkEntityPrefab;
                         private       Entity        _chunkMeshEntityPrefab;
                         private       World         _defaultWorld;
                         private       EntityManager _entityManager;

        private void Start() {
            Debug.Log("GameWorld::Start: Beginning Setup...");
            _defaultWorld  = World.DefaultGameObjectInjectionWorld;
            _entityManager = _defaultWorld.EntityManager;

            using (var blobAssetStore = new BlobAssetStore()) {
                var conversionSettings = GameObjectConversionSettings.FromWorld(_defaultWorld, blobAssetStore);
                _chunkEntityPrefab =
                    GameObjectConversionUtility.ConvertGameObjectHierarchy(chunkGameObjectPrefab, conversionSettings);
                _chunkMeshEntityPrefab =
                    GameObjectConversionUtility.ConvertGameObjectHierarchy(chunkMeshGameObjectPrefab, conversionSettings);
            }
            Debug.Log("GameWorld::Start: Setup complete!");
            
            Debug.Log("GameWorld::Start: Creating world...");
            CreateWorld();
            Debug.Log("GameWorld::Start: World done!");
        }

        private void CreateWorld() {
            for (var y = viewDistanceVertical; y > -viewDistanceVertical; --y) {
                for (var x = -viewDistanceHorizontal; x < viewDistanceHorizontal; ++x) {
                    for (var z = -viewDistanceHorizontal; z < viewDistanceHorizontal; ++z) {
                        //Debug.Log($"Working... {x}, {y}, {z}");
                        // CreateChunk(new int3(x, y, z));
                        
                        // only generate chunk if it is within the sphere of vision
                        if (Vector3.Distance(
                                new Vector3(0, 0, 0),
                                new Vector3(x, y, z)
                            )
                          < ChunkSize * 0.5) {
                            CreateChunk(new int3(x * ChunkSize, 
                                                 y * ChunkSize, 
                                                 z * ChunkSize));
                        }
                    }
                }
            }
        }
        
        private void CreateChunk(int3 worldPosition) {
            // Instantiate Chunk data entity
            var newChunk = _entityManager.Instantiate(_chunkEntityPrefab);
#if DEBUG
            _entityManager.SetName(newChunk, $"Chunk_{worldPosition.x}_{worldPosition.y}_{worldPosition.z}");
#endif
            _entityManager.SetComponentData(newChunk, new Translation {
                Value = worldPosition
            });
            // TODO: Create system to find serialized chunk data and only regen chunk if not saved
            _entityManager.AddComponentData(newChunk, new GenerateTerrainTag());
            
               // Instantiate Chunk mesh entity
//             var newChunkMesh = _entityManager.Instantiate(_chunkMeshEntityPrefab);
// #if DEBUG
//             _entityManager.SetName(newChunkMesh, $"ChunkMesh_{worldPosition.x}_{worldPosition.y}_{worldPosition.z}");
// #endif
//             //_entityManager.SetSharedComponentData(newChunkMesh, new RenderMesh());
//             _entityManager.SetComponentData(newChunkMesh, new Translation {
//                 Value = worldPosition
//             });
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
