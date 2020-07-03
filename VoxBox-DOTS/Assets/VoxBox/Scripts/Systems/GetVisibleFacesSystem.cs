using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VoxBox.Scripts.Components.Buffers;
using VoxBox.Scripts.Components.Tags;

namespace VoxBox.Scripts.Systems {
    public class GetVisibleFacesSystem : SystemBase {
        private static EndSimulationEntityCommandBufferSystem _commandsBuffer;
        private const  int                                    ChunkSize = GameWorld.ChunkSize;
        private        Dictionary<int3, Entity>               chunkMap  = new Dictionary<int3, Entity>();

        protected override void OnCreate() {
            chunkMap.Clear();
            _commandsBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            var entityQuery1 = GetEntityQuery(
                ComponentType.ReadOnly<ChunkTag>(),
                ComponentType.ReadOnly<UpdateChunkTag>(),
                ComponentType.Exclude<CreateChunkMeshTag>(),
                ComponentType.Exclude<RenderChunkTag>(),
                ComponentType.Exclude<SetChunkNeighborsTag>(),
                ComponentType.Exclude<CalculateFacesTag>()
            );
            var entityQuery2 = GetEntityQuery(
                ComponentType.ReadOnly<ChunkTag>(),
                ComponentType.ReadOnly<UpdateChunkTag>(),
                ComponentType.ReadOnly<SetChunkNeighborsTag>()
            );
            var entityCount1 = entityQuery1.CalculateEntityCount();
            var entityCount2 = entityQuery2.CalculateEntityCount();

            SetChunkNeighbors();
            CheckFaceVisibility();

            if (entityCount1 == 0 && entityCount2 == 0 && chunkMap.Count > 0) {
                chunkMap.Clear();
            }
        }

        private void SetChunkNeighbors() {
            var ecb = _commandsBuffer.CreateCommandBuffer();
            //var ecbNC = _commandsBuffer.CreateCommandBuffer();
            //Debug.Log(group.CalculateEntityCount());
            //Debug.Log(group2.CalculateEntityCount());
            //var voxelNameMap = World.GetOrCreateSystem<VoxelRegistrationSystem>().GetVoxelNameMap();
            //var chunkMap = new NativeHashMap<int3, Entity>(entityCount, Allocator.TempJob);

            Entities.WithAll<ChunkTag, UpdateChunkTag>()
                    .WithNone<CreateChunkMeshTag>()
                    .WithNone<RenderChunkTag>()
                    .WithNone<SetChunkNeighborsTag>()
                    .WithNone<CalculateFacesTag>()
                    .WithoutBurst()
                    .ForEach(
                         (
                             Entity         e,
                             int            entityInQueryIndex,
                             in Translation translation
                         ) => {
                             if (!chunkMap.ContainsKey((int3)translation.Value))
                                 chunkMap.Add((int3)translation.Value, e);
                             ecb.AddComponent<SetChunkNeighborsTag>(e);
                         }
                     )
                    .Run();

            _commandsBuffer.AddJobHandleForProducer(Dependency);

            Entities.WithAll<ChunkTag, UpdateChunkTag, SetChunkNeighborsTag>()
                    .WithoutBurst()
                    .ForEach(
                         (
                             Entity                                   e,
                             int                                      entityInQueryIndex,
                             ref DynamicBuffer<NeighborBufferElement> neighborBuffer,
                             in  Translation                          translation
                         ) => {
                             for (var face = 0; face < 6; ++face) {
                                 var neighbor = (int3)translation.Value;
                                 neighbor += WorldData.Directions[face] * ChunkSize;

                                 //Debug.Log($"{translation.Value}->{(int3)translation.Value} |  + {WorldData.Directions[face] * ChunkSize}");
                                 //Debug.Log(chunkMap.Count());
                                 if (chunkMap.ContainsKey(neighbor)) {
                                     //Debug.Log("Found neighbor");
                                     neighborBuffer[face] = chunkMap[neighbor];
                                 }
                                 else {
                                     //Debug.Log($"No neighbor on {(Direction)face}");
                                     neighborBuffer[face] = null;
                                 }
                             }

                             ecb.RemoveComponent<SetChunkNeighborsTag>(e);
                             ecb.AddComponent<CalculateFacesTag>(e);
                         }
                     )
                    .Run();
            //Debug.Log(chunkMap.Count());
            //chunkMap.Dispose();

            _commandsBuffer.AddJobHandleForProducer(Dependency);
        }

        private void CheckFaceVisibility() {
            var ecb = _commandsBuffer.CreateCommandBuffer().ToConcurrent();
            //var voxelNameMap = World.GetOrCreateSystem<VoxelRegistrationSystem>().GetVoxelNameMap();
            var lookup = GetBufferFromEntity<VoxelBufferElement>();

            Entities.WithAll<ChunkTag, UpdateChunkTag, CalculateFacesTag>()
                    .ForEach(
                         (
                             Entity e,
                             int    entityInQueryIndex,
                             //ref DynamicBuffer<VoxelBufferElement>        voxelBuffer,
                             ref DynamicBuffer<VisibleFacesBufferElement> visibleFacesBuffer,
                             ref DynamicBuffer<NeighborBufferElement>     neighborBuffer
                         ) => {
                             for (var x = 0; x < ChunkSize; ++x) {
                                 for (var y = 0; y < ChunkSize; ++y) {
                                     for (var z = 0; z < ChunkSize; ++z) {
                                         VisibleFacesBufferElement face;
                                         var                       position = new int3(x, y, z);
                                         var                       voxel    = lookup[e][GetIndex(position)].value;
                                         //var voxelName = voxelNameMap.GetName(voxel);

                                         if (!IsOpaque(voxel) && voxel != VoxelID.WATER) {
                                             face = VisibleFacesBufferElement.None;
                                         }
                                         else {
                                             // TODO: Convert to face-specific visibility instead of voxel-specific
                                             // inverted because the face is only visible if there *isn't* anything opaque
                                             face = new VisibleFacesBufferElement {
                                                 north = IsFaceVisible(
                                                     voxel,
                                                     position,
                                                     Direction.NORTH,
                                                     e,
                                                     //ref voxelBuffer,
                                                     ref neighborBuffer,
                                                     lookup
                                                 ),
                                                 east = IsFaceVisible(
                                                     voxel,
                                                     position,
                                                     Direction.EAST,
                                                     e,
                                                     //ref voxelBuffer,
                                                     ref neighborBuffer,
                                                     lookup
                                                 ),
                                                 south = IsFaceVisible(
                                                     voxel,
                                                     position,
                                                     Direction.SOUTH,
                                                     e,
                                                     //ref voxelBuffer,
                                                     ref neighborBuffer,
                                                     lookup
                                                 ),
                                                 west = IsFaceVisible(
                                                     voxel,
                                                     position,
                                                     Direction.WEST,
                                                     e,
                                                     //ref voxelBuffer,
                                                     ref neighborBuffer,
                                                     lookup
                                                 ),
                                                 up = IsFaceVisible(
                                                     voxel,
                                                     position,
                                                     Direction.UP,
                                                     e,
                                                     //ref voxelBuffer,
                                                     ref neighborBuffer,
                                                     lookup
                                                 ),
                                                 down = IsFaceVisible(
                                                     voxel,
                                                     position,
                                                     Direction.DOWN,
                                                     e,
                                                     //ref voxelBuffer,
                                                     ref neighborBuffer,
                                                     lookup
                                                 )
                                             };
                                         }

                                         visibleFacesBuffer[GetIndex(position)] = face;
                                     }
                                 }
                             }

                             // Mark as ready for render
                             ecb.RemoveComponent<CalculateFacesTag>(entityInQueryIndex, e);
                             ecb.AddComponent<CreateChunkMeshTag>(entityInQueryIndex, e);
                         }
                     )
                    .Schedule();

            _commandsBuffer.AddJobHandleForProducer(Dependency);
        }

        private static int GetIndex(int3 position) {
            return (position.y * ChunkSize * ChunkSize) + (position.x * ChunkSize) + position.z;
        }

        private static bool IsInRange(int3 position) {
            return (position.x >= 0 && position.x < ChunkSize)
                && (position.y >= 0 && position.y < ChunkSize)
                && (position.z >= 0 && position.z < ChunkSize);
        }

        private static bool IsFaceVisible(
            VoxelID   voxelID,
            int3      voxelPosition,
            Direction direction,
            Entity    chunkEntity,
            //ref DynamicBuffer<VoxelBufferElement>    voxelBuffer,
            ref DynamicBuffer<NeighborBufferElement> neighborBuffer,
            BufferFromEntity<VoxelBufferElement>     lookup
        ) {
            var neighborVoxelPosition = voxelPosition + WorldData.Directions[(int)direction];

            if (IsInRange(neighborVoxelPosition)) {
                var neighborVoxelID = lookup[chunkEntity][GetIndex(neighborVoxelPosition)].value;

                // exception for two water blocks next to each other. they shouldn't show interior faces
                if (voxelID == VoxelID.WATER && neighborVoxelID == VoxelID.WATER) {
                    return false;
                }

                return !IsOpaque(neighborVoxelID);
            }

            if (neighborBuffer[(int)direction].value.HasValue) {
                var neighborChunkEntity = neighborBuffer[(int)direction].value.Value;
                var neighborVoxelBuffer = lookup[neighborChunkEntity];

                // flip the direction vector so it is something like 1,0,1 rather than 0,1,0
                var modifier = math.abs(WorldData.Directions[(int)direction]);
                modifier -= new int3(1, 1, 1);
                modifier =  math.abs(modifier);

                neighborVoxelPosition = new int3(
                    voxelPosition.x * modifier.x,
                    voxelPosition.y * modifier.y,
                    voxelPosition.z * modifier.z
                );

                switch (direction) {
                    case Direction.NORTH:
                        neighborVoxelPosition.z = 0;
                        break;
                    case Direction.EAST:
                        neighborVoxelPosition.x = 0;
                        break;
                    case Direction.SOUTH:
                        neighborVoxelPosition.z = ChunkSize - 1;
                        break;
                    case Direction.WEST:
                        neighborVoxelPosition.x = ChunkSize - 1;
                        break;
                    case Direction.UP:
                        neighborVoxelPosition.y = 0;
                        break;
                    case Direction.DOWN:
                        neighborVoxelPosition.y = ChunkSize - 1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }

                var neighborVoxelID = neighborVoxelBuffer[GetIndex(neighborVoxelPosition)].value;

                // exception for two water blocks next to each other. they shouldn't show interior faces
                if (voxelID == VoxelID.WATER && neighborVoxelID == VoxelID.WATER) {
                    return false;
                }

                return !IsOpaque(neighborVoxelID);
            }

            // neighbor voxel isn't in chunk and there is no neighbor chunk
            // true  - sides of chunks are rendered on world edge (slower)
            // false - sides of chunks are not rendered on world edge (faster)
            return false;
        }

        private static bool IsSolid(VoxelID voxelID) {
            return voxelID switch {
                VoxelID.LOGO      => true,
                VoxelID.AIR       => false,
                VoxelID.WATER     => false,
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
                VoxelID.WATER     => false,
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