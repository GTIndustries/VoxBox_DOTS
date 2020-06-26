using System;
using Unity.Entities;

namespace VoxBox.Scripts.Components.Buffers {
    [InternalBufferCapacity(6)]
    [Serializable]
    public struct NeighborBufferElement : IBufferElementData {
        public static implicit operator Entity(NeighborBufferElement e) { return e.value; }
        public static implicit operator NeighborBufferElement(Entity e) { return new NeighborBufferElement { value = e }; }

        // Actual value each buffer element will store.
        public Entity value;
    }
}
