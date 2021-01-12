using UnityEngine;

namespace SpatialSorting
{
    public class GameObjectComparer : Comparer<GameObject>
    {
        public GameObjectComparer(SpaceMapping mapping, Curve curve, Bounds bounds) : base(mapping, curve, bounds,
            go => go.transform.position)
        {
        }
    }
}