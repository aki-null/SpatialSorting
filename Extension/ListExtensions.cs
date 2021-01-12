using System.Collections.Generic;
using UnityEngine;

namespace SpatialSorting
{
    public static class ListExtensions
    {
        /// <summary>
        /// Sorts the coordinates of a sequence considering their spatial locality.
        /// </summary>
        /// <remarks>
        /// This operation is slow, so it is recommended that sorting takes place offline if the number of points to
        /// sort is large.
        /// Use <c>SpatialSortFast</c> if fast runtime sorting is required, although the spatial locality is worse.
        /// </remarks>
        /// <param name="obj">An array to sort.</param>
        /// <param name="mapping">How coordinates are mapped to sort spatially.</param>
        /// <param name="bounds">Bounds that encapsulates all points in a sequence.</param>
        public static void SpatialSort(this List<Vector3> obj, SpaceMapping mapping = SpaceMapping.XYZ,
            Bounds? bounds = null)
        {
            obj.Sort(new VectorComparer(mapping, Curve.Hilbert, bounds ?? obj.Bounds()));
        }

        /// <summary>
        /// Sorts the coordinates of a sequence considering their spatial locality.
        /// </summary>
        /// <remarks>
        /// This operation is faster than <c>SpatialSort</c>, but spatial locality is worse.
        /// Consider using <c>SpatialSort</c> if sorting can take place offline.
        /// </remarks>
        /// <param name="obj">An array to sort.</param>
        /// <param name="mapping">How coordinates are mapped to sort spatially.</param>
        /// <param name="bounds">Bounds that encapsulates all points in a sequence.</param>
        public static void SpatialSortFast(this List<Vector3> obj, SpaceMapping mapping = SpaceMapping.XYZ,
            Bounds? bounds = null)
        {
            obj.Sort(new VectorComparer(mapping, Curve.Morton, bounds ?? obj.Bounds()));
        }

        /// <summary>
        /// Sorts the coordinates of a sequence considering their spatial locality.
        /// </summary>
        /// <remarks>
        /// This operation is slow, so it is recommended that sorting takes place offline if the number of points to
        /// sort is large.
        /// Use <c>SpatialSortFast</c> if fast runtime sorting is required, although the spatial locality is worse.
        /// </remarks>
        /// <param name="obj">An array to sort.</param>
        /// <param name="mapping">How coordinates are mapped to sort spatially.</param>
        /// <param name="bounds">Bounds that encapsulates all points in a sequence.</param>
        public static void SpatialSort(this List<GameObject> obj, SpaceMapping mapping = SpaceMapping.XYZ,
            Bounds? bounds = null)
        {
            obj.Sort(new GameObjectComparer(mapping, Curve.Hilbert, bounds ?? obj.Bounds()));
        }

        /// <summary>
        /// Sorts the coordinates of a sequence considering their spatial locality.
        /// </summary>
        /// <remarks>
        /// This operation is faster than <c>SpatialSort</c>, but spatial locality is worse.
        /// Consider using <c>SpatialSort</c> if sorting can take place offline.
        /// </remarks>
        /// <param name="obj">An array to sort.</param>
        /// <param name="mapping">How coordinates are mapped to sort spatially.</param>
        /// <param name="bounds">Bounds that encapsulates all points in a sequence.</param>
        public static void SpatialSortFast(this List<GameObject> obj, SpaceMapping mapping = SpaceMapping.XYZ,
            Bounds? bounds = null)
        {
            obj.Sort(new GameObjectComparer(mapping, Curve.Morton, bounds ?? obj.Bounds()));
        }
    }
}