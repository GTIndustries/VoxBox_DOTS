using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VoxBox.Scripts.Components.Buffers;
using VoxBox.Scripts.Components.Tags;

namespace VoxBox.Scripts.Systems {
    public class TerrainGenerationSystem : SystemBase {
        private static EndSimulationEntityCommandBufferSystem _commandsBuffer;
        private const  int                                    ChunkSize = GameWorld.ChunkSize;

        protected override void OnCreate() {
            Debug.Log("Created System: TerrainGeneration");
            _commandsBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            var ecb = _commandsBuffer.CreateCommandBuffer().ToConcurrent();

            Entities.WithAll<ChunkTag, GenerateTerrainTag>()
                    .ForEach(
                         (
                             Entity                                 e,
                             int                                    entityInQueryIndex,
                             ref DynamicBuffer<VoxelBufferElement>  voxelBuffer,
                             //ref DynamicBuffer<EntityBufferElement> entityBuffer,
                             in  Translation                        translation
                         ) => {
                             // Generate terrain
                             //Debug.Log($"Generating chunk {translation.Value}");
                             voxelBuffer = GenerateTerrain(voxelBuffer, translation);

                             // Set ready for update and facing
                             ecb.RemoveComponent<GenerateTerrainTag>(entityInQueryIndex, e);
                             ecb.AddComponent<UpdateChunkTag>(entityInQueryIndex, e);
                             ecb.AddComponent<CalculateFacesTag>(entityInQueryIndex, e);
                         }
                     )
                    .ScheduleParallel();

            _commandsBuffer.AddJobHandleForProducer(Dependency);
        }

        private static DynamicBuffer<VoxelBufferElement> GenerateTerrain(
            DynamicBuffer<VoxelBufferElement> voxelBuffer,
            in Translation                    translation
        ) {
            for (var y = 0; y < ChunkSize; ++y) {
                for (var x = 0; x < ChunkSize; ++x) {
                    for (var z = 0; z < ChunkSize; ++z) {
                        var currentIndex = y * ChunkSize * ChunkSize + x * ChunkSize + z;

                        voxelBuffer[currentIndex] = new VoxelBufferElement {
                            value = SelectVoxelType(x, y, z, translation)
                        };
                    }
                }
            }

            return voxelBuffer;
        }

        private static VoxelID SelectVoxelType(in int x, in int y, in int z, in Translation translation) {
            VoxelID voxel;
            var     trueY = (int)math.floor(translation.Value.y) + y;

            if (trueY > 0) {
                voxel = VoxelID.AIR;
            }
            else if (trueY == 0) {
                voxel = VoxelID.GRASS;
            }
            else {
                voxel = VoxelID.DIRT;

                if (trueY < -2) {
                    voxel = VoxelID.LIMESTONE;
                }
            }

            return voxel;
        }
    }
}