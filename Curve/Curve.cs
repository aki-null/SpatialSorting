namespace SpatialSorting
{
    /// <summary>
    /// A type of space filling curve.
    /// </summary>
    public enum Curve
    {
        /// <summary>
        /// Morton order, or a Z-order curve.
        /// </summary>
        Morton,

        /// <summary>
        /// Hilbert curve.
        /// </summary>
        Hilbert
    }
}