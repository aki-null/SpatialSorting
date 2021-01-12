using UnityEngine;

namespace SpatialSorting
{
    public class VectorComparer : Comparer<Vector3>
    {
        public VectorComparer(SpaceMapping mapping, Curve curve, Bounds bounds) : base(mapping, curve, bounds,
            vec => vec)
        {
        }
    }
}