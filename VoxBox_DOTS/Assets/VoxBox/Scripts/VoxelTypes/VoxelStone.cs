using System;

namespace VoxBox.Scripts.VoxelTypes {
    [Serializable]
    public class VoxelStone : Voxel {
        public VoxelStone() : base() {
            VoxelID = VoxelID.LIMESTONE;
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
                Direction.NORTH => TextureID.LIMESTONE,
                Direction.EAST  => TextureID.LIMESTONE,
                Direction.SOUTH => TextureID.LIMESTONE,
                Direction.WEST  => TextureID.LIMESTONE,
                Direction.UP    => TextureID.LIMESTONE,
                Direction.DOWN  => TextureID.LIMESTONE,
                _               => TextureID.NULL
            };
        }
    }
}
