using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace VoxBox.Scripts.Components.Buffers {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class UVBufferElementAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
        private static int UVArrayLength { get; } = 1;//GameWorld.ChunkSize * GameWorld.ChunkSize * GameWorld.ChunkSize * 4 * 6;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            var voxelBuffer = dstManager.AddBuffer<UVBufferElement>(entity);
            
            for (var i = 0; i < UVArrayLength; ++i) {
                voxelBuffer.Add(float2.zero);
            }
        }
    }
}
