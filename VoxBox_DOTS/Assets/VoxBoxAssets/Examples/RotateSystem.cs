using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace VoxBoxAssets.Examples {
    public class RotateSystem : JobComponentSystem {
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            float dT = Time.DeltaTime;
            
            var jobHandle = Entities.ForEach((ref Rotation rotation, in Rotate rotate) => {
                rotation.Value = math.mul(
                    rotation.Value,
                    quaternion.RotateY(
                        math.radians(rotate.DegreesPerSecond) * dT
                    )
                );
            }).Schedule(inputDeps);

            return jobHandle;
        }
    }
}
