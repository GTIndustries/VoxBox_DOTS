using System;

namespace VoxBox.Scripts.VoxelTypes {
    [Serializable]
    public class VoxelStone : Voxel {
        public VoxelStone() : base() {
            VoxelID = VoxelID.STONE;
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
                Direction.NORTH => TextureID.STONE,
                Direction.EAST  => TextureID.STONE,
                Direction.SOUTH => TextureID.STONE,
                Direction.WEST  => TextureID.STONE,
                Direction.UP    => TextureID.STONE,
                Direction.DOWN  => TextureID.STONE,
                _               => TextureID.NULL
            };
        }
    }
}
