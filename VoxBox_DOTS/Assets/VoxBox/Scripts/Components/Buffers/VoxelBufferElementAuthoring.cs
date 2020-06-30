using Unity.Entities;
using UnityEngine;

namespace VoxBox.Scripts.Components.Buffers {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class VoxelBufferElementAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
        private static int VoxelArrayLength { get; } = GameWorld.ChunkSize * GameWorld.ChunkSize * GameWorld.ChunkSize;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            var voxelBuffer = dstManager.AddBuffer<VoxelBufferElement>(entity);
            
            for (var i = 0; i < VoxelArrayLength; ++i) {
                voxelBuffer.Add(VoxelID.NULL);
            }
        }
    }
}
