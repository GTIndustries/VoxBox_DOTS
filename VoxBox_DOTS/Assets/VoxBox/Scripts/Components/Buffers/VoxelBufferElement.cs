using System;
using Unity.Entities;

namespace VoxBox.Scripts.Components.Buffers {
    [InternalBufferCapacity(0)]
    [Serializable]
    public struct VoxelBufferElement : IBufferElementData {
        public static implicit operator VoxelID(VoxelBufferElement e) { return e.value; }
        public static implicit operator VoxelBufferElement(VoxelID e) { return new VoxelBufferElement { value = e }; }
        
        public VoxelID value;
    }
}
