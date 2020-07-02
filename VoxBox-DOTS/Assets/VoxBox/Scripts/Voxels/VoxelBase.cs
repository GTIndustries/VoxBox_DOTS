using UnityEngine;

namespace VoxBox.Scripts.Voxels {
    [CreateAssetMenu(fileName = "Voxel", menuName = "Voxel Asset", order = 10)]
    public class VoxelBase : ScriptableObject {
        public new string name = "Voxel";
        public VoxelID voxelID = VoxelID.NULL;
        
        [Tooltip("0 - North"
               + "\n1 - East"
               + "\n2 - South"
               + "\n3 - West"
               + "\n4 - Up"
               + "\n5 - Down")]
        [SerializeField] public Sprite[] faces = new Sprite[6];
    }
}
