using System;
using Unity.Entities;

namespace VoxBox.Scripts.Components.Buffers {
    [Serializable]
    public struct VisibleFacesBufferElement : IBufferElementData {
        /// <summary>
        ///     VisibleFacesBufferElement with none visible.
        /// </summary>
        public static readonly VisibleFacesBufferElement None =
            new VisibleFacesBufferElement {
                west  = TextureID.AIR,
                east  = TextureID.AIR,
                up    = TextureID.AIR,
                down  = TextureID.AIR,
                north = TextureID.AIR,
                south = TextureID.AIR,
            };
 
        /// <summary>
        /// The left face, -1 is invisible otherwise texture id.
        /// </summary>
        public TextureID west;
 
        /// <summary>
        /// The right face, -1 is invisible otherwise texture id.
        /// </summary>
        public TextureID east;
 
        /// <summary>
        /// The top face, -1 is invisible otherwise texture id.
        /// </summary>
        public TextureID up;
 
        /// <summary>
        /// The bottom face, -1 is invisible otherwise texture id.
        /// </summary>
        public TextureID down;
 
        /// <summary>
        /// The front face, -1 is invisible otherwise texture id.
        /// </summary>
        public TextureID north;
 
        /// <summary>
        /// The back face, -1 is invisible otherwise texture id.
        /// </summary>
        public TextureID south;
 
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
        public TextureID this[Direction face] {
            get {
                return face switch {
                    Direction.WEST  => west,
                    Direction.EAST  => east,
                    Direction.UP    => up,
                    Direction.DOWN  => down,
                    Direction.NORTH => north,
                    Direction.SOUTH => south,
                    _               => throw new ArgumentOutOfRangeException("Out of index:" + face)
                };
            }
        }
    }
}
