using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSorting
{
    public static class IListExtensions
    {
        public static Bounds Bounds(this IList<Vector3> obj)
        {
            return obj.Bounds(vec => vec);
        }

        public static Bounds Bounds(this IList<GameObject> obj)
        {
            return obj.Bounds(go => go.transform.position);
        }

        public static Bounds Bounds<T>(this IList<T> obj, Func<T, Vector3> getPosition)
        {
            if (obj.Count == 0)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            var bounds = new Bounds {center = getPosition(obj[0]), extents = Vector3.zero};
            for (var i = 1; i < obj.Count; ++i)
            {
                bounds.Encapsulate(getPosition(obj[i]));
            }

            return bounds;
        }
    }
}