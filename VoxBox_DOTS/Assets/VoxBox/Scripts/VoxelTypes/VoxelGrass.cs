using System;

namespace VoxBox.Scripts.VoxelTypes {
    [Serializable]
    public class VoxelGrass : Voxel {
        public VoxelGrass() : base() {
            VoxelID = VoxelID.GRASS;
        }
    
        public override bool ShouldRotate(Direction direction) {
            return direction switch {
                Direction.NORTH => false,
                Direction.EAST  => false,
                Direction.SOUTH => false,
                Direction.WEST  => false,
                Direction.UP    => true,
                Direction.DOWN  => true,
                _               => false
            };
        }

        public override TextureID GetFaceTexture(Direction face) {
            return face switch {
                Direction.NORTH => TextureID.GRASS_SIDE,
                Direction.EAST  => TextureID.GRASS_SIDE,
                Direction.SOUTH => TextureID.GRASS_SIDE,
                Direction.WEST  => TextureID.GRASS_SIDE,
                Direction.UP    => TextureID.GRASS,
                Direction.DOWN  => TextureID.DIRT,
                _               => TextureID.NULL
            };
        }
    }
}
