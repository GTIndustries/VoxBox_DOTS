﻿using Unity.Entities;
using UnityEngine;

namespace VoxBox.Scripts.Components.Buffers {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class EntityBufferElementAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            var entityBuffer = dstManager.AddBuffer<EntityBufferElement>(entity);
        }
    }
}
