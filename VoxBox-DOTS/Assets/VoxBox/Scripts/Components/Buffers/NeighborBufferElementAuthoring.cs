using Unity.Entities;
using UnityEngine;

namespace VoxBox.Scripts.Components.Buffers {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class NeighborBufferElementAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
        private static int NeighborArrayLength { get; } = 6;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            var neighborBuffer = dstManager.AddBuffer<NeighborBufferElement>(entity);

            // TODO: Add neighbors to the array
            for (var i = 0; i < NeighborArrayLength; ++i) {
                neighborBuffer.Add(null);
            }
        }
    }
}