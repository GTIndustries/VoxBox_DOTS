using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VoxBox.Scripts.Components.Buffers;
using VoxBox.Scripts.Components.Tags;

namespace VoxBox.Scripts.Systems {
    public class ChunkRenderingSystem : SystemBase {
        private static EndSimulationEntityCommandBufferSystem _commandsBuffer;

        protected override void OnCreate() {
            _commandsBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            var ecb = _commandsBuffer.CreateCommandBuffer().ToConcurrent();


            Entities.WithAll<ChunkTag, UpdateChunkTag, RenderChunkTag>()
                    .ForEach(
                         (
                             Entity                                 e,
                             int                                    entityInQueryIndex,
                             ref DynamicBuffer<VoxelBufferElement>  voxelBuffer,
                             ref DynamicBuffer<EntityBufferElement> entityBuffer,
                             in  Translation                        translation
                         ) => {
                             // Update and render complete
                             ecb.RemoveComponent<UpdateChunkTag>(entityInQueryIndex, e);
                             ecb.RemoveComponent<RenderChunkTag>(entityInQueryIndex, e);
                         }
                     )
                    .ScheduleParallel();

            _commandsBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}