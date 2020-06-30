﻿using Unity.Entities;
using UnityEngine;

namespace VoxBox.Scripts.Components.Buffers {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class NeighborBufferElementAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            var neighborBuffer = dstManager.AddBuffer<NeighborBufferElement>(entity);
            // TODO: Add neighbors to the array
        }
    }
}
