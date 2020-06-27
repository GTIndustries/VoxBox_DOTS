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
    public struct CalculateFacesTag : IComponentData { }

    [GenerateAuthoringComponent]
    [Serializable]
    public struct MeshedChunkTag : IComponentData { }
    
    [GenerateAuthoringComponent]
    [Serializable]
    public struct RenderChunkTag : IComponentData { }
}