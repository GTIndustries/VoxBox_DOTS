using Unity.Entities;

namespace VoxBoxAssets.Examples {
    [GenerateAuthoringComponent]
    public struct Rotate : IComponentData {
        public float DegreesPerSecond;
    }
}
