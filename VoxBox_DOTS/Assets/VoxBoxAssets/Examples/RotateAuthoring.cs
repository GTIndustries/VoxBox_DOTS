using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace VoxBoxAssets.Examples {
    public class RotateAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
        [SerializeField] private float degreesPerSecond = 0f;
    
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            dstManager.AddComponentData(entity, new Rotate {
                DegreesPerSecond = math.radians(degreesPerSecond)
            });
            dstManager.AddComponentData(entity, new RotationEulerXYZ());
        }
    }
}
