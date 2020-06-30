using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace VoxBox.Scripts.Components.Buffers {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class NormalBufferElementAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
        private static int NormalArrayLength { get; } = 1;//GameWorld.ChunkSize * GameWorld.ChunkSize * GameWorld.ChunkSize * 6;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            var voxelBuffer = dstManager.AddBuffer<NormalBufferElement>(entity);
            
            for (var i = 0; i < NormalArrayLength; ++i) {
                voxelBuffer.Add(float3.zero);
            }
        }
    }
}
