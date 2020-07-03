namespace DynamicPixelCloud
{
    using Unity.Entities;

    /// <summary>
    /// The CloudPerlinNoiseData struct.
    /// </summary>
    [GenerateAuthoringComponent]
    public struct CloudPerlinNoiseData : IComponentData
    {
        /// <summary>
        /// The row number of the cloud unit.
        /// </summary>
        public int row;

        /// <summary>
        /// The column number of the cloud unit.
        /// </summary>
        public int col;

        /// <summary>
        /// The scale of the cloud unit.
        /// </summary>
        public float scale;

        /// <summary>
        /// The offset in x direction of the cloud unit.
        /// </summary>
        public float offsetX;

        /// <summary>
        /// The offset in z direction of the cloud unit.
        /// </summary>
        public float offsetZ;

        /// <summary>
        /// The chaos of the cloud.
        /// </summary>
        public float chaos;
    }
}