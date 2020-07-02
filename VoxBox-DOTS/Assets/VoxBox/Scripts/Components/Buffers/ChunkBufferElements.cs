using System;
using Unity.Collections;
using Unity.Entities;

namespace VoxBox.Scripts.Components.Buffers {
    [InternalBufferCapacity(0)]
    [Serializable]
    public struct VoxelBufferElement : IBufferElementData {
        public static implicit operator VoxelID(VoxelBufferElement e) {
            return e.value;
        }

        public static implicit operator VoxelBufferElement(VoxelID e) {
            return new VoxelBufferElement {
                value = e
            };
        }

        public VoxelID value;
    }

    [Serializable]
    public struct VisibleFacesBufferElement : IBufferElementData {
        /// <summary>
        ///     VisibleFacesBufferElement with none visible.
        /// </summary>
        public static readonly VisibleFacesBufferElement None =
            new VisibleFacesBufferElement {
                north = false,
                east  = false,
                south = false,
                west  = false,
                up    = false,
                down  = false,
            };

        /// <summary>
        /// The front face, -1 is invisible otherwise texture id.
        /// </summary>
        public bool north;

        /// <summary>
        /// The right face, -1 is invisible otherwise texture id.
        /// </summary>
        public bool east;

        /// <summary>
        /// The back face, -1 is invisible otherwise texture id.
        /// </summary>
        public bool south;

        /// <summary>
        /// The left face, -1 is invisible otherwise texture id.
        /// </summary>
        public bool west;

        /// <summary>
        /// The top face, -1 is invisible otherwise texture id.
        /// </summary>
        public bool up;

        /// <summary>
        /// The bottom face, -1 is invisible otherwise texture id.
        /// </summary>
        public bool down;

        /// <summary>
        ///     The this.
        /// </summary>
        /// <param name="face">
        ///     Face index. See <see cref="Faces"/> for mappings.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if unknown cube face.
        /// </exception>
        /// <returns>
        ///     The texture id of the face requested. -1 if invisible.
        /// </returns>
        public bool this[int face] =>
            face switch {
                0 => north,
                1 => east,
                2 => south,
                3 => west,
                4 => up,
                5 => down,
                _ => throw new ArgumentOutOfRangeException("Out of index:" + face)
            };
    }

    [InternalBufferCapacity(6)]
    [Serializable]
    public struct NeighborBufferElement : IBufferElementData {
        public static implicit operator Entity(NeighborBufferElement e) {
            return e.value;
        }

        public static implicit operator NeighborBufferElement(Entity e) {
            return new NeighborBufferElement {
                value = e
            };
        }

        // Actual value each buffer element will store.
        public Entity value;
    }

    [InternalBufferCapacity(1)]
    [Serializable]
    public struct EntityBufferElement : IBufferElementData {
        public static implicit operator Entity(EntityBufferElement e) {
            return e.value;
        }

        public static implicit operator EntityBufferElement(Entity e) {
            return new EntityBufferElement {
                value = e
            };
        }

        // Actual value each buffer element will store.
        [NativeDisableParallelForRestriction] public Entity value;
    }
}