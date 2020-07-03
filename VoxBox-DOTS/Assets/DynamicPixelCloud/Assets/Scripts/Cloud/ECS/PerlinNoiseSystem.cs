namespace DynamicPixelCloud
{
    using System;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using UnityEngine;

    /// <summary>
    /// The PerlinNoiseSystem class.
    /// </summary>
    public class PerlinNoiseSystem : JobComponentSystem
    {
        private float totalTime = 0f;

        /// <summary>
        /// Updates jobs.
        /// </summary>
        /// <param name="inputDeps">The inputDeps.</param>
        /// <returns>The job handle.</returns>
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var rows = CloudManager.inst.rows;
            var columns = CloudManager.inst.columns;
            var maximumSize = CloudManager.inst.maximumSize;
            var realDensity = 1 - CloudManager.inst.density;
            var expandSpeed = CloudManager.inst.expandSpeed;
            var contractSpeed = CloudManager.inst.contractSpeed;
            var chaos = CloudManager.inst.chaos;
            var dir = CloudManager.inst.windDirection.normalized;

            float totalTimeTmp = totalTime;
            if (totalTime >= 100f)
            {
                totalTime = 0f;
            }

            totalTime += Time.DeltaTime * CloudManager.inst.moveSpeed;

            var jobHandler = Entities.ForEach((ref CloudPerlinNoiseData data) =>
            {
                float x = data.row / (float)rows;
                float y = data.col / (float)columns;
                float frequency = chaos + data.chaos * 0.3f;
                float x1 = totalTimeTmp * dir.x + x * frequency;
                float y1 = totalTimeTmp * dir.y + y * frequency;
                float x2 = totalTimeTmp * dir.x + x * frequency * 0.5f;
                float y2 = totalTimeTmp * dir.y + y * frequency * 0.5f;
                float scale = Mathf.PerlinNoise(x1, y1);
                float visible = Mathf.PerlinNoise(x2, y2);

                float targetScale = 0f;
                if (scale > realDensity && visible > 0.5f)
                {
                    targetScale = maximumSize * scale;
                }

                if (data.scale < targetScale)
                {
                    data.scale += totalTimeTmp == 0 ? targetScale : expandSpeed * 0.02f;
                    data.scale = Mathf.Min(targetScale, data.scale);
                }
                else
                {
                    data.scale -= totalTimeTmp == 0 ? targetScale : contractSpeed * 0.02f;
                    data.scale = Mathf.Max(targetScale, data.scale);
                }

                data.offsetX = Mathf.Min(Mathf.Max(data.offsetX, -0.5f), 0.5f);
                data.offsetZ = Mathf.Min(Math.Max(data.offsetZ, -0.5f), 0.5f);
            }).Schedule(inputDeps);

            return jobHandler;
        }
    }
}