using TreeEditor;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VoxBox.Scripts.Components.Buffers;
using VoxBox.Scripts.Components.Tags;
using VoxelEngine;

namespace VoxBox.Scripts.Systems {
    public class TerrainGenerationSystem : SystemBase {
        private static EndSimulationEntityCommandBufferSystem _commandsBuffer;
        private const  int                                    ChunkSize = GameWorld.ChunkSize;

        protected override void OnCreate() {
            //Debug.Log("Created System: TerrainGeneration");
            _commandsBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            var ecb = _commandsBuffer.CreateCommandBuffer().ToConcurrent();

            Entities.WithAll<ChunkTag, GenerateTerrainTag>()
                    .ForEach(
                         (
                             Entity                                e,
                             int                                   entityInQueryIndex,
                             ref DynamicBuffer<VoxelBufferElement> voxelBuffer,
                             //ref DynamicBuffer<EntityBufferElement> entityBuffer,
                             in Translation translation
                         ) => {
                             // Generate terrain
                             //Debug.Log($"Generating chunk {translation.Value}");
                             voxelBuffer = GenerateTerrain(
                                 ref voxelBuffer,
                                 translation
                             );

                             // Set ready for update and facing
                             ecb.RemoveComponent<GenerateTerrainTag>(entityInQueryIndex, e);
                             ecb.AddComponent<UpdateChunkTag>(entityInQueryIndex, e);
                             //ecb.AddComponent<CalculateFacesTag>(entityInQueryIndex, e);
                         }
                     )
                    .ScheduleParallel();

            _commandsBuffer.AddJobHandleForProducer(Dependency);
        }

        private static DynamicBuffer<VoxelBufferElement> GenerateTerrain(
            ref DynamicBuffer<VoxelBufferElement> voxelBuffer,
            in  Translation                       translation
        ) {
            for (var y = 0; y < ChunkSize; ++y) {
                for (var x = 0; x < ChunkSize; ++x) {
                    for (var z = 0; z < ChunkSize; ++z) {
                        var currentIndex = y * ChunkSize * ChunkSize + x * ChunkSize + z;

                        voxelBuffer[currentIndex] = new VoxelBufferElement {
                            value = SelectVoxelType(
                                new int3(x, y, z),
                                translation
                            )
                        };
                    }
                }
            }

            return voxelBuffer;
        }

        private static VoxelID SelectVoxelType(
            int3           localPosition,
            in Translation translation
        ) {
            VoxelID voxel;
            var     trueY = (int)math.floor(translation.Value.y) + localPosition.y;
            var surfaceLevel = (int)math.floor(
                (noise.snoise(
                     new float2(
                         localPosition.x + translation.Value.x + 8f,
                         localPosition.z + translation.Value.z + 8f
                     )
                   * 0.01f
                 )
               * 4f) * 0.5f
              + (noise.snoise(
                     new float2(
                         localPosition.x + translation.Value.x - 32f,
                         localPosition.z + translation.Value.z - 32f
                     )
                   * 0.005f
                 )
               * 8f) * 0.5f
            );

            // Basic generation pass
            if (trueY > surfaceLevel) {
                voxel = VoxelID.AIR;
            }
            else if (trueY == surfaceLevel) {
                voxel = VoxelID.GRASS;
            }
            else {
                voxel = VoxelID.DIRT;

                if (trueY < surfaceLevel - 2) {
                    voxel = VoxelID.LIMESTONE;
                }
            }
            
            // Water pass
            if (trueY < 1 && voxel == VoxelID.AIR) {
                voxel = VoxelID.WATER;
            }

            // Mountain pass
            if (trueY > 5 && voxel != VoxelID.AIR) {
                voxel = VoxelID.LIMESTONE;
            }

            return voxel;
        }
    }
}