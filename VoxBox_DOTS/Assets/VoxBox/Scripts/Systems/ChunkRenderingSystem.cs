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
            
            
            Entities.WithAll<ChunkTag, FacesTag, RenderTag>().ForEach((Entity                                 e,
                                                                      int                                    nativeThreadIndex,
                                                                      ref DynamicBuffer<VoxelBufferElement>  voxelBuffer,
                                                                      ref DynamicBuffer<EntityBufferElement> entityBuffer,
                                                                      in Translation                         translation) => {
                
                
                // Update and render complete
                if (HasComponent<UpdateTag>(e)) {
                    ecb.RemoveComponent<UpdateTag>(nativeThreadIndex, e);
                }
                if (HasComponent<FacesTag>(e)) {
                    ecb.RemoveComponent<FacesTag>(nativeThreadIndex, e);
                }
                if (HasComponent<RenderTag>(e)) {
                    ecb.RemoveComponent<RenderTag>(nativeThreadIndex, e);
                }
            }).ScheduleParallel();
            
            _commandsBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}
