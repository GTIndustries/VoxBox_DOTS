using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace VoxBox.Scripts.Components.Buffers {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class VertexBufferElementAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
        private static int VertexArrayLength { get; } = 0;//GameWorld.ChunkSize * GameWorld.ChunkSize * GameWorld.ChunkSize * 4 * 6;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            var voxelBuffer = dstManager.AddBuffer<VertexBufferElement>(entity);
            
            for (var i = 0; i < VertexArrayLength; ++i) {
                voxelBuffer.Add(float3.zero);
            }
        }
    }
}
