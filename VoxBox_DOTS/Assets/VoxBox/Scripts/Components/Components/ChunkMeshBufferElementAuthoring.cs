using Unity.Entities;
using UnityEngine;
using VoxBox.Scripts.Components.Buffers;

namespace VoxBox.Scripts.Components.Components {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class ChunkMeshBufferElementAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
        private static int VoxelArrayLength { get; } = GameWorld.ChunkSize * GameWorld.ChunkSize * GameWorld.ChunkSize;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            // var voxelBuffer = dstManager.AddBuffer<ChunkMeshBufferElement>(entity);
            //
            // for (var i = 0; i < VoxelArrayLength; ++i) {
            //     voxelBuffer.Add(new ChunkMeshBufferElement());
            // }
        }
    }
}
