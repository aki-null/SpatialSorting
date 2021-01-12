using System.Collections.Generic;
using UnityEngine;

namespace SpatialSorting
{
    public class Comparer<T> : IComparer<T>
    {
        private readonly SpaceMapper _mapper;
        private readonly System.Func<T, Vector3> _getPosition;
        private readonly Curve _curve;

        public Comparer(SpaceMapping mapping, Curve curve, Bounds bounds, System.Func<T, Vector3> getPosition)
        {
            var mapBits = 32;
            if (curve == Curve.Morton && mapping == SpaceMapping.XYZ)
            {
                mapBits = 21;
            }

            _mapper = new SpaceMapper(bounds, mapping, mapBits);
            _curve = curve;
            _getPosition = getPosition;
        }

        public int Compare(T x, T y)
        {
            switch (_curve)
            {
                case Curve.Morton:
                    if (_mapper.Mapping == SpaceMapping.XYZ)
                    {
                        var lhsIdx = Morton.ToIndex3D(_getPosition(x), in _mapper);
                        var rhsIdx = Morton.ToIndex3D(_getPosition(y), in _mapper);
                        return lhsIdx.CompareTo(rhsIdx);
                    }
                    else
                    {
                        var lhsIdx = Morton.ToIndex2D(_getPosition(x), in _mapper);
                        var rhsIdx = Morton.ToIndex2D(_getPosition(y), in _mapper);
                        return lhsIdx.CompareTo(rhsIdx);
                    }
                case Curve.Hilbert:
                    if (_mapper.Mapping == SpaceMapping.XYZ)
                    {
                        var lhsIdx = Hilbert.ToIndex3D(_getPosition(x), in _mapper);
                        var rhsIdx = Hilbert.ToIndex3D(_getPosition(y), in _mapper);
                        return lhsIdx.CompareTo(rhsIdx);
                    }
                    else
                    {
                        var lhsIdx = Hilbert.ToIndex2D(_getPosition(x), in _mapper);
                        var rhsIdx = Hilbert.ToIndex2D(_getPosition(y), in _mapper);
                        return lhsIdx.CompareTo(rhsIdx);
                    }
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}