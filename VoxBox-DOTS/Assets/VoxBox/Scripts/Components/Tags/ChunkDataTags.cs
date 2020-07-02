using System;
using Unity.Entities;

namespace VoxBox.Scripts.Components.Tags {
    [GenerateAuthoringComponent]
    [Serializable]
    public struct UpdateChunkTag : IComponentData { }
    
    [GenerateAuthoringComponent]
    [Serializable]
    public struct LoadChunkTag : IComponentData { }

    [GenerateAuthoringComponent]
    [Serializable]
    public struct GenerateTerrainTag : IComponentData { }

    [GenerateAuthoringComponent]
    [Serializable]
    public struct SetChunkNeighborsTag : IComponentData { }

    [GenerateAuthoringComponent]
    [Serializable]
    public struct FinishChunkNeighborsTag : IComponentData { }

    [GenerateAuthoringComponent]
    [Serializable]
    public struct CalculateFacesTag : IComponentData { }

    [GenerateAuthoringComponent]
    [Serializable]
    public struct CreateChunkMeshTag : IComponentData { }
    
    [GenerateAuthoringComponent]
    [Serializable]
    public struct RenderChunkTag : IComponentData { }
    
    [GenerateAuthoringComponent]
    [Serializable]
    public struct DestroyChunkTag : IComponentData { }
}