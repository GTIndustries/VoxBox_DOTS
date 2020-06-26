using Unity.Entities;
using UnityEngine;

namespace VoxBox.Scripts.Components.Buffers {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class VisibleFacesBufferElementAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
        private static int VoxelArrayLength { get; } = GameWorld.ChunkSize * GameWorld.ChunkSize * GameWorld.ChunkSize;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            var voxelBuffer = dstManager.AddBuffer<VisibleFacesBufferElement>(entity);
            
            for (var i = 0; i < VoxelArrayLength; ++i) {
                voxelBuffer.Add(VisibleFacesBufferElement.None);
            }
        }
    }
}
