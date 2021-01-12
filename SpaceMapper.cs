using UnityEngine;

namespace SpatialSorting
{
    /// <summary>
    /// How a 3D coordinate gets mapped in space.
    /// </summary>
    public enum SpaceMapping
    {
        XYZ,
        XZ,
        XY,
        YZ
    }

    /// <summary>
    /// Provides an space mapping algorithm.
    /// </summary>
    internal readonly struct SpaceMapper
    {
        private readonly Vector3 _boundsMin;
        private readonly double _mapFactor;

        public SpaceMapping Mapping { get; }

        /// <summary>
        /// Initializes a new instance of the SpaceMapper struct.
        /// </summary>
        /// <param name="bounds">Bounds in 3D space that encapsulates points to be mapped.</param>
        /// <param name="mapping">A space mapping configuration.</param>
        /// <param name="mapBits">A number of bits to use for mapped integer value (maximum of 32)</param>
        public SpaceMapper(Bounds bounds, SpaceMapping mapping, int mapBits)
        {
            _boundsMin = bounds.min;
            var boundsMax = bounds.max;

            var xRange = boundsMax.x - _boundsMin.x;
            var yRange = boundsMax.y - _boundsMin.y;
            var zRange = boundsMax.z - _boundsMin.z;

            float range;
            if (mapping == SpaceMapping.XYZ)
            {
                range = Mathf.Max(Mathf.Max(xRange, yRange), zRange);
            }
            else
            {
                var firstRange = mapping == SpaceMapping.YZ ? yRange : xRange;
                var secondRange = mapping == SpaceMapping.XY ? yRange : zRange;

                range = Mathf.Max(firstRange, secondRange);
            }

            _mapFactor = (mapBits >= 32 ? uint.MaxValue : (1u << mapBits) - 1) / (double) range;
            Mapping = mapping;
        }

        /// <summary>
        /// Maps a floating point coordinates into an integer coordinates
        /// </summary>
        /// <param name="pos">An original 3D coordinates to map</param>
        /// <param name="result">Unsafe pointer to an integer coordinates buffer</param>
        public unsafe void Map(Vector3 pos, uint* result)
        {
            if (Mapping == SpaceMapping.XYZ)
            {
                result[0] = (uint) ((pos.x - _boundsMin.x) * _mapFactor);
                result[1] = (uint) ((pos.y - _boundsMin.y) * _mapFactor);
                result[2] = (uint) ((pos.z - _boundsMin.z) * _mapFactor);
            }
            else
            {
                result[0] = (uint) ((Mapping == SpaceMapping.YZ ? pos.y - _boundsMin.y : pos.x - _boundsMin.x) *
                                    _mapFactor);
                result[1] = (uint) ((Mapping == SpaceMapping.XY ? pos.y - _boundsMin.y : pos.z - _boundsMin.z) *
                                    _mapFactor);
            }
        }
    }
}