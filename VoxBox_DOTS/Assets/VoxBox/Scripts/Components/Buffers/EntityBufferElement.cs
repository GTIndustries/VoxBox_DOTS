using System;
using Unity.Collections;
using Unity.Entities;

namespace VoxBox.Scripts.Components.Buffers {
    [InternalBufferCapacity(1)]
    [Serializable]
    public struct EntityBufferElement : IBufferElementData {
        public static implicit operator Entity(EntityBufferElement e) { return e.value; }
        public static implicit operator EntityBufferElement(Entity e) { return new EntityBufferElement { value = e }; }
        
        // Actual value each buffer element will store.
        [NativeDisableParallelForRestriction]
        public Entity value;
    }
}
