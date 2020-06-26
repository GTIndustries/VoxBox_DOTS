using System;

namespace VoxBox.Scripts.VoxelTypes {
    [Serializable]
    public class VoxelDirt : Voxel {
        public VoxelDirt() : base() {
            VoxelID = VoxelID.DIRT;
        }
    
        public override bool ShouldRotate(Direction direction) {
            return direction switch {
                Direction.NORTH => true,
                Direction.EAST  => true,
                Direction.SOUTH => true,
                Direction.WEST  => true,
                Direction.UP    => true,
                Direction.DOWN  => true,
                _               => false
            };
        }

        public override TextureID GetFaceTexture(Direction face) {
            return face switch {
                Direction.NORTH => TextureID.DIRT,
                Direction.EAST  => TextureID.DIRT,
                Direction.SOUTH => TextureID.DIRT,
                Direction.WEST  => TextureID.DIRT,
                Direction.UP    => TextureID.DIRT,
                Direction.DOWN  => TextureID.DIRT,
                _               => TextureID.NULL
            };
        }
    }
}
