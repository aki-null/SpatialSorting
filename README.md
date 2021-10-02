SpatialSorting
===

Unity C# library to sort a sequence of coordinates considering their spatial locality.

Overview
---
Both CPU and GPU improves performance by introducing caching to their design. For example, each CPU core has a cache to optimize memory access. However, these kinds of optimizations often do not work effectively if the memory access patterns are random. One way to improve this issue is to make sure that memory access is as sequential as possible.

Games place various data in a 3D scene, which is hard to make memory access sequential. Developing a solution to sort these data spatially can potentially improve memory access patterns.

Features
---
- Array/List extension methods
- GameObject and Vector comparer
- [Z order](https://en.wikipedia.org/wiki/Z-order_curve) (Morton order) sorting
    - Fast and usable for runtime sorting
- [Hilbert order](https://en.wikipedia.org/wiki/Hilbert_curve) sorting
    - Slower then Z order but has better spatial locality
    - Recommended for data that can be sorted offline

Visualization
---
![Comparison](https://raw.githubusercontent.com/aki-null/SpatialSorting/image/comparison.jpg)

| Type     | Total Distance | Saving |
| -------- | -------------- | ------ |
| Unsorted |        1527173 |     0% |
| Morton   |          75458 |    95% |
| Hilbert  |          61123 |    96% |

<details closed>
<summary>Test Component Code</summary>

```c#
using System.Collections.Generic;
// Odin Inspector is REALLY good. Buy it now if you are not using it.
using Sirenix.OdinInspector;
using SpatialSorting;
using UnityEngine;

public class PointOrdering : MonoBehaviour
{
  [SerializeField, HideInInspector] private List<Vector3> positions = new List<Vector3>();

  [SerializeField] private int count = 40000;

  [SerializeField] private BoxCollider range;

  [SerializeField] private float holeRadius;

  [Button]
  private void Hilbert(SpaceMapping mapping)
  {
    // Array extension method
    positions.SpatialSort(mapping);
  }

  [Button]
  private void Morton(SpaceMapping mapping)
  {
    // Array extension method
    positions.SpatialSortFast(mapping);
  }

  [Button]
  private void GeneratePoints()
  {
    positions = new List<Vector3>();
    var holeRadiusSqr = holeRadius * holeRadius;
    var boundsExtents = range.size / 2;
    var boundsCenter = range.center;
    for (var i = 0; i < count; ++i)
    {
      var pos = new Vector3(
        Random.Range(-boundsExtents.x, boundsExtents.x),
        Random.Range(-boundsExtents.y, boundsExtents.y),
        Random.Range(-boundsExtents.z, boundsExtents.z)) + boundsCenter;
      if (pos.sqrMagnitude < holeRadiusSqr) continue;
      positions.Add(pos);
    }
  }

  private void OnDrawGizmosSelected()
  {
    if (positions == null || positions.Count == 0) return;
    var prevPoint = transform.TransformPoint(positions[0]);
    for (var i = 1; i < positions.Count; ++i)
    {
      Gizmos.color = new Color(0, ((float) i - 1) / positions.Count, 0);
      var currentPoint = transform.TransformPoint(positions[i]);
      Gizmos.DrawLine(prevPoint, currentPoint);
      prevPoint = currentPoint;
    }
  }
}
```
</details>

API
---

### Extension Methods

```c#
GameObject[] objects = ...; // can be a list as well

// Hilbert (slower)
objects.SpatialSort(SpaceMapping.XYZ);
// with explicit point bounds (faster if bounds are known)
objects.SpatialSort(SpaceMapping.XYZ, bounds);

// Morton (faster)
objects.SpatialSortFast(SpaceMapping.XYZ);
// with explicit point bounds (faster if bounds are known)
objects.SpatialSortFast(SpaceMapping.XYZ, bounds);
```

Extension methods are currently available for GameObject and Vector3

### Comaprer

```c#
var bounds = objects.Bounds(); // prepare this beforehand if performance matters
var comparer = new GameObjectComparer(SpaceMapping.XYZ, Curve.Hilbert, bounds);
Array.Sort(objects, comparer);
```

Please note that bounds do not need to be exact, although it has to encapsulate all input points.

Other Uses
---

For example, it incurs a significant performance penalty when branching to implement 3D point culling.

We can pass a Spatially sorted list of 3D points to the shader to have nearby points scheduled in the same warp, resulting in a higher chance of all threads taking the same branch.
