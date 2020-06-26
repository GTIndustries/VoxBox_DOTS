using System;

namespace VoxBox.Scripts.VoxelTypes {
    [Serializable]
    public class VoxelCobble : Voxel {
        public VoxelCobble() : base() {
            VoxelID = VoxelID.COBBLE;
        }
    
        public override bool ShouldRotate(Direction direction) {
            return direction switch {
                Direction.NORTH => false,
                Direction.EAST  => false,
                Direction.SOUTH => false,
                Direction.WEST  => false,
                Direction.UP    => false,
                Direction.DOWN  => false,
                _               => false
            };
        }

        public override TextureID GetFaceTexture(Direction face) {
            return face switch {
                Direction.NORTH => TextureID.COBBLE,
                Direction.EAST  => TextureID.COBBLE,
                Direction.SOUTH => TextureID.COBBLE,
                Direction.WEST  => TextureID.COBBLE,
                Direction.UP    => TextureID.COBBLE,
                Direction.DOWN  => TextureID.COBBLE,
                _               => TextureID.NULL
            };
        }
    }
}
