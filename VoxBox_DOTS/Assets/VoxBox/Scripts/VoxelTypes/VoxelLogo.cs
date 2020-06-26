using System;

namespace VoxBox.Scripts.VoxelTypes {
    [Serializable]
    public class VoxelLogo : Voxel {
        public VoxelLogo() : base() {
            VoxelID = VoxelID.LOGO;
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
                Direction.NORTH => TextureID.LOGO,
                Direction.EAST  => TextureID.LOGO,
                Direction.SOUTH => TextureID.LOGO,
                Direction.WEST  => TextureID.LOGO,
                Direction.UP    => TextureID.LOGO,
                Direction.DOWN  => TextureID.LOGO,
                _               => TextureID.NULL
            };
        }
    }
}