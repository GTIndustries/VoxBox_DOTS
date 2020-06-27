using Unity.Entities;
using Unity.Mathematics;

namespace VoxBox.Scripts.Components.Buffers {
    /// <summary>
    ///     The buffer of mesh vertices.
    /// </summary>
    [InternalBufferCapacity(0)]
    public struct VertexBufferElement : IBufferElementData {
        public static implicit operator float3(VertexBufferElement e) { return e.value; }
        public static implicit operator VertexBufferElement(float3 e) { return new VertexBufferElement { value = e }; }
        /// <summary>
        ///     The Vertex.
        /// </summary>
        public float3 value;
    }

    /// <summary>
    ///     The buffer of mesh uvs.
    /// </summary>
    [InternalBufferCapacity(0)]
    public struct UVBufferElement : IBufferElementData {
        public static implicit operator float2(UVBufferElement e) { return e.value; }
        public static implicit operator UVBufferElement(float2 e) { return new UVBufferElement { value = e }; }
        /// <summary>
        ///     The uv.
        /// </summary>
        public float2 value;
    }

    /// <summary>
    ///     The buffer of mesh normals.
    /// </summary>
    [InternalBufferCapacity(0)]
    public struct NormalBufferElement : IBufferElementData {
        public static implicit operator float3(NormalBufferElement e) { return e.value; }
        public static implicit operator NormalBufferElement(float3 e) { return new NormalBufferElement { value = e }; }
        /// <summary>
        ///     The normal.
        /// </summary>
        public float3 value;
    }

    /// <summary>
    ///     The buffer of mesh triangles.
    /// </summary>
    [InternalBufferCapacity(0)]
    public struct TriangleBufferElement : IBufferElementData {
        public static implicit operator int(TriangleBufferElement e) { return e.value; }
        public static implicit operator TriangleBufferElement(int e) { return new TriangleBufferElement { value = e }; }
        /// <summary>
        ///     The triangle.
        /// </summary>
        public int value;
    }
}