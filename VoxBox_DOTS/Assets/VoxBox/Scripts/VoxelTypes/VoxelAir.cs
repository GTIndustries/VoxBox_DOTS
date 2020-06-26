using System;

namespace VoxBox.Scripts.VoxelTypes {
    [Serializable]
    public class VoxelAir : Voxel {
        public VoxelAir() : base() {
            VoxelID = VoxelID.AIR;
        }

        // public override MeshData VoxelData(Chunk chunk, int x, int y, int z, MeshData meshData) {
        //     meshData.useRenderDataForCollider = true;
        //     return meshData;
        // }

        public override bool IsSolid(Direction direction) {
            return false;
        }
    
        public override bool IsOpaque(Direction direction) { 
            return false;
        }
    }
}
