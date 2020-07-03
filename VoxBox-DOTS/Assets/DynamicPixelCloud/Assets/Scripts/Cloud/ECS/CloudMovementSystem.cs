namespace DynamicPixelCloud
{
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using Unity.Transforms;

    /// <summary>
    /// The CloudMovementSystem class.
    /// </summary>
    public class CloudMovementSystem : JobComponentSystem
    {
        /// <summary>
        /// /Updates jobs.
        /// </summary>
        /// <param name="inputDeps">The inputDeps.</param>
        /// <returns>The jobHandle.</returns>
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var transformScaleMultiplier = (float3)CloudManager.inst.transformScaleMultiplier;
            var originOffset = (float3)CloudManager.inst.gameObject.transform.position;

            var jobHandler = Entities.ForEach((ref Translation translation, ref NonUniformScale scale, in CloudPerlinNoiseData data) =>
            {
                scale.Value = data.scale * transformScaleMultiplier;
                translation.Value = new float3(data.row, 0f, data.col) + originOffset + new float3(data.offsetX, 0, data.offsetZ) * 0.5f;
            }).Schedule(inputDeps);

            return jobHandler;
        }
    }
}