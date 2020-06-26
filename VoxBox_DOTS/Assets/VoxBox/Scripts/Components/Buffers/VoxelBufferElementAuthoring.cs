using Unity.Entities;
using UnityEngine;

namespace VoxBox.Scripts.Components.Buffers {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class VoxelBufferElementAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
        // Add fields to your component here. Remember that:
        //
        // * The purpose of this class is to store data for authoring purposes - it is not for use while the game is
        //   running.
        // 
        // * Traditional Unity serialization rules apply: fields must be public or marked with [SerializeField], and
        //   must be one of the supported types.
        //
        // For example,
        //    public float scale;
        // public VoxelId[] valueArray = new VoxelId[1000];
        private static int VoxelArrayLength { get; } = GameWorld.ChunkSize * GameWorld.ChunkSize * GameWorld.ChunkSize;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            // Call methods on 'dstManager' to create runtime components on 'entity' here. Remember that:
            //
            // * You can add more than one component to the entity. It's also OK to not add any at all.
            //
            // * If you want to create more than one entity from the data in this class, use the 'conversionSystem'
            //   to do it, instead of adding entities through 'dstManager' directly.
            //
            // For example,
            //   dstManager.AddComponentData(entity, new Unity.Transforms.Scale { Value = scale });
            var voxelBuffer = dstManager.AddBuffer<VoxelBufferElement>(entity);
            // if (voxelBuffer.Length >= voxelArrayLength) return;
            for (var i = 0; i < VoxelArrayLength; ++i) {
                voxelBuffer.Add(VoxelID.NULL);
            }
            // foreach (var value in valueArray) {
            //     voxelBuffer.Add(value);
            // }
        }
    }
}
