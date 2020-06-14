using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace VoxBoxAssets.Scripts {
    public class GameManager : MonoBehaviour {
                         public        int viewDistanceHorizontal = 5;
                         public        int viewDistanceVertical   = 5;
                         public  const int ChunkSize              = 10;
        
        [SerializeField] private       GameObject    voxelGameObjectPrefab = null; 
                         private       World         _defaultWorld;
                         private       EntityManager _entityManager;
                         private       Entity        _voxelEntityPrefab;
        
        private void Start() {
            Debug.Log("GameWorld::Start: Beginning Setup...");
            _defaultWorld  = World.DefaultGameObjectInjectionWorld;
            _entityManager = _defaultWorld.EntityManager;

            using (var blobAssetStore = new BlobAssetStore()) {
                var conversionSettings = GameObjectConversionSettings.FromWorld(_defaultWorld, blobAssetStore);
                _voxelEntityPrefab =
                    GameObjectConversionUtility.ConvertGameObjectHierarchy(voxelGameObjectPrefab, conversionSettings);
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
                        CreateChunk(new int3(x, y, z));
                    }
                }
            }
        }
        
        private void CreateChunk(int3 worldPosition) {
            // Instantiate Chunk data entity
            var newChunk = _entityManager.Instantiate(_voxelEntityPrefab);
#if DEBUG
            _entityManager.SetName(newChunk, $"Voxel_{worldPosition.x}_{worldPosition.y}_{worldPosition.z}");
#endif
            _entityManager.SetComponentData(newChunk, new Translation {
                Value = worldPosition
            });

            if (worldPosition.y < 0 || worldPosition.y > 5) {
                _entityManager.AddSharedComponentData(newChunk, new RenderMesh());
            }
            // _entityManager.SetComponentData(newChunk, new Update {
            //     update = false
            // });
            // _entityManager.SetComponentData(newChunk, new GenerateTerrain {
            //     generateTerrain = true
            // });
        }
    }
}
