using Unity.Entities;
using UnityEngine;

namespace VoxBox.Scripts.Components.Buffers {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class TriangleBufferElementAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
        private static int TriangleArrayLength { get; } = 0;//GameWorld.ChunkSize * GameWorld.ChunkSize * GameWorld.ChunkSize * 6 * 6;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            var voxelBuffer = dstManager.AddBuffer<TriangleBufferElement>(entity);
            
            for (var i = 0; i < TriangleArrayLength; ++i) {
                voxelBuffer.Add(0);
            }
        }
    }
}
