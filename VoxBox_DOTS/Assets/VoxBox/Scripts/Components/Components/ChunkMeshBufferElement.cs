using System;
using Unity.Entities;
using VoxBox.Scripts.Components.Buffers;
using VoxBox.Scripts.Systems;

namespace VoxBox.Scripts.Components.Components {
    [InternalBufferCapacity(0)]
    public struct ChunkMeshBufferElement : IBufferElementData {
        // public static implicit operator CubeModel(ChunkMeshBufferElement e) { return e.value; }
        // public static implicit operator ChunkMeshBufferElement(CubeModel e) { return new ChunkMeshBufferElement { value = e }; }
        // /// <summary>
        // ///     The uv.
        // /// </summary>
        // public CubeModel value;
    }
}