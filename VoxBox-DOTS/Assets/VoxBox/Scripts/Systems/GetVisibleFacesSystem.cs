using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using VoxBox.Scripts.Components.Buffers;
using VoxBox.Scripts.Components.Tags;

namespace VoxBox.Scripts.Systems {
    public class GetVisibleFacesSystem : SystemBase {
        private static EndSimulationEntityCommandBufferSystem _commandsBuffer;
        private const  int                                    ChunkSize = GameWorld.ChunkSize;

        protected override void OnCreate() {
            _commandsBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            CheckFaceVisibility();
        }

        private void CheckFaceVisibility() {
            var ecb = _commandsBuffer.CreateCommandBuffer().ToConcurrent();
            var voxelNameMap = World.GetOrCreateSystem<VoxelRegistrationSystem>().GetVoxelNameMap();

            Entities.WithAll<ChunkTag, UpdateChunkTag, CalculateFacesTag>()
                    .WithReadOnly(voxelNameMap)
                    .ForEach(
                         (
                             Entity                                       e,
                             int                                          entityInQueryIndex,
                             ref DynamicBuffer<VoxelBufferElement>        voxelBuffer,
                             ref DynamicBuffer<VisibleFacesBufferElement> visibleFacesBuffer
                         ) => {
                             for (var x = 0; x < ChunkSize; ++x) {
                                 for (var y = 0; y < ChunkSize; ++y) {
                                     for (var z = 0; z < ChunkSize; ++z) {
                                         VisibleFacesBufferElement face;

                                         var voxel = voxelBuffer[GetIndex(x, y, z)].value;
                                         var voxelName = voxelNameMap.GetName(voxel);

                                         if (!IsOpaque(voxel)) {
                                             face = VisibleFacesBufferElement.None;
                                         }
                                         else {
                                             var left =
                                                 x >= 1
                                              && IsOpaque(voxelBuffer[GetIndex(x - 1, y, z)].value);
                                             var bottom =
                                                 y >= 1
                                              && IsOpaque(voxelBuffer[GetIndex(x, y - 1, z)].value);
                                             var front =
                                                 z >= 1
                                              && IsOpaque(voxelBuffer[GetIndex(x, y, z - 1)].value);
                                             var right =
                                                 x < ChunkSize - 1
                                              && IsOpaque(voxelBuffer[GetIndex(x + 1, y, z)].value);
                                             var top =
                                                 y < ChunkSize - 1
                                              && IsOpaque(voxelBuffer[GetIndex(x, y + 1, z)].value);
                                             var back =
                                                 z < ChunkSize - 1
                                              && IsOpaque(voxelBuffer[GetIndex(x, y, z + 1)].value);

                                             // TODO: Convert to face-specific visibility instead of voxel-specific
                                             // inverted because the face is only visible if there *isn't* anything opaque
                                             face = new VisibleFacesBufferElement {
                                                 west  = !left,
                                                 east  = !right,
                                                 down  = !bottom,
                                                 up    = !top,
                                                 north = !front,
                                                 south = !back,
                                             };
                                         }

                                         visibleFacesBuffer[GetIndex(x, y, z)] = face;
                                     }
                                 }
                             }

                             // Mark as ready for render
                             ecb.RemoveComponent<CalculateFacesTag>(entityInQueryIndex, e);
                             ecb.AddComponent<CreateMeshChunkTag>(entityInQueryIndex, e);
                         }
                     )
                    .ScheduleParallel();

            _commandsBuffer.AddJobHandleForProducer(Dependency);
        }

        private static int GetIndex(int x, int y, int z) {
            return x + (y * ChunkSize * ChunkSize) + (z * ChunkSize);
        }

        private static bool IsSolid(VoxelID voxelID) {
            return voxelID switch {
                VoxelID.LOGO      => true,
                VoxelID.AIR       => false,
                VoxelID.BEDROCK   => true,
                VoxelID.NULL      => false,
                VoxelID.GRASS     => true,
                VoxelID.COBBLE    => true,
                VoxelID.LIMESTONE => true,
                VoxelID.DIRT      => true,
                VoxelID.LOG       => true,
                _                 => false
            };
        }

        private static bool IsOpaque(VoxelID voxelID) {
            return voxelID switch {
                VoxelID.LOGO      => true,
                VoxelID.AIR       => false,
                VoxelID.BEDROCK   => true,
                VoxelID.NULL      => false,
                VoxelID.GRASS     => true,
                VoxelID.COBBLE    => true,
                VoxelID.LIMESTONE => true,
                VoxelID.DIRT      => true,
                VoxelID.LOG       => true,
                _                 => false
            };
        }
    }
}