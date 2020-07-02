using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.U2D;
using VoxBox.Scripts.Voxels;

/*
 * Credit to sarkahn on GitHub for his implementation of a voxel engine in DOTS
 * assisting me in improving my own
 */

namespace VoxBox.Scripts.Systems {
    public struct VoxelUVMapBlobAsset {
        // first level is the voxel type index, next is voxel face index, final is the 4 uvs
        public BlobArray<BlobArray<BlobArray<float2>>> uvs;
    }

    /// <summary>
    /// A struct that can be passed in to jobs to
    /// retrieve voxel IDs from voxel names. This should be passed 
    /// as read-only. Should be retrieved via <see cref="VoxelRegistrationSystem.GetVoxelIDMap"/>.
    /// </summary>
    public struct VoxelNameMap {
        private NativeHashMap<int, FixedString32> _voxelIDToNameMap;

        public VoxelNameMap(NativeHashMap<int, FixedString32> hashMap) {
            _voxelIDToNameMap = hashMap;
        }

        public FixedString32 GetName(int voxelID) => _voxelIDToNameMap[voxelID];

        public FixedString32 GetName(VoxelID voxelID) => GetName((int)voxelID);
    }

    /// <summary>
    /// A struct that can be passed in to jobs to retrieve voxel UV data from
    /// voxel IDs. This should be retrieved via <see cref="VoxelRegistrationSystem.GetVoxelUVMap"/>
    /// </summary>
    public struct VoxelUVMap {
        private BlobAssetReference<VoxelUVMapBlobAsset> _voxelUVBlob;

        public VoxelUVMap(BlobAssetReference<VoxelUVMapBlobAsset> blobAsset) {
            _voxelUVBlob = blobAsset;
        }

        public ref BlobArray<float2> GetUVs(VoxelID voxelID, Direction faceID) =>
            ref _voxelUVBlob.Value.uvs[(int)voxelID][(int)faceID];
    }
    
    public class VoxelRegistrationSystem : SystemBase {
        public struct VoxelData {
            private NativeList<FixedList64<float2>> _uvMap;
            private NativeHashMap<int, FixedString32> _idToNameMap;
        }
        
        private UnsafeBitArray _voxelOpacity = default;
        private NativeHashMap<int, FixedString32> _idToNameMap = default;
        private BlobAssetReference<VoxelUVMapBlobAsset> _voxelUVMapBlobAsset = default;
        
        public SpriteAtlas voxelAtlas;

        protected override void OnCreate() {
            base.OnCreate();
            //Debug.Log("Created System: ChunkMeshing");
            
            voxelAtlas = Resources.Load<SpriteAtlas>("Sprites/Atlas_Voxel");
            var voxels = Resources.LoadAll<VoxelBase>("VoxelAssets");

            InitContainers(voxels.Length);

            var builder = new BlobBuilder(Allocator.Temp);
            ref var uvRoot = ref builder.ConstructRoot<VoxelUVMapBlobAsset>();
            // Allocate base array-of-arrays
            var voxelUVsBaseArray = builder.Allocate(ref uvRoot.uvs, voxels.Length);
            
            //Debug.Log("System|VoxelRegistration: Registering voxels");

            // foreach (var voxelID in (int[])Enum.GetValues(typeof(VoxelID))) {
            //     if (voxelID >= voxels.Length) {
            //         
            //     }
            // }
            for (var i = 0; i < voxels.Length; ++i) {
                var voxel = voxels[i];

                RegisterVoxelName(voxel, (int)voxel.voxelID);
                RegisterVoxelUVs(voxel, ref builder, ref voxelUVsBaseArray[(int)voxel.voxelID]);
            }

            _voxelUVMapBlobAsset = builder.CreateBlobAssetReference<VoxelUVMapBlobAsset>(Allocator.Persistent);
        }

        protected override void OnDestroy() {
            base.OnDestroy();

            _voxelOpacity.Dispose();
            _idToNameMap.Dispose();
        }

        // Nothing needs to be done per frame
        protected override void OnUpdate() { }

        public FixedString32 GetVoxelName(int voxelID) => _idToNameMap[voxelID];

        public FixedString32 GetVoxelName(VoxelID voxelID) => GetVoxelName((int)voxelID);

        public ref BlobArray<float2> GetVoxelFaceUVs(VoxelID voxelID, Direction faceID) =>
            ref _voxelUVMapBlobAsset.Value.uvs[(int)voxelID][(int)faceID];
        
        public VoxelNameMap GetVoxelNameMap() => new VoxelNameMap(_idToNameMap);
        
        public VoxelUVMap GetVoxelUVMap() => new VoxelUVMap(_voxelUVMapBlobAsset);

        private void InitContainers(int length) {
            _voxelOpacity = new UnsafeBitArray(length, Allocator.Persistent);
            _idToNameMap = new NativeHashMap<int, FixedString32>(length, Allocator.Persistent);
        }

        private void RegisterVoxelName(VoxelBase voxel, int voxelID) {
            _idToNameMap[voxelID] = voxel.name;
        }

        private void RegisterVoxelUVs(
            VoxelBase                        voxel,
            ref BlobBuilder                  builder,
            ref BlobArray<BlobArray<float2>> voxelUVsBaseArray
        ) {
            var voxelFaces = voxel.faces;
            var voxelBlobFaces = builder.Allocate(ref voxelUVsBaseArray, voxelFaces.Length);

            for (var faceIndex = 0; faceIndex < voxelFaces.Length; ++faceIndex) {
                var voxelFace = voxelFaces[faceIndex];
                //Debug.Log($"Binding sprite: {voxel.name}|{voxelFace.name}");
                if (voxelAtlas.CanBindTo(voxelFace)) {
                    var voxelUVs = voxelAtlas.GetSprite(voxelFace.name).uv;
                    var voxelBlobUVs = builder.Allocate(ref voxelBlobFaces[faceIndex], voxelUVs.Length);
                
                    for (var uvIndex = 0; uvIndex < voxelUVs.Length; ++uvIndex) {
                        voxelBlobUVs[uvIndex] = voxelUVs[uvIndex];
                    }
                }
                else {
                    Debug.Log($"Sprite not found in atlas: {voxel.name}|{voxelFace.name}");
                }
            }
        }
    }
}