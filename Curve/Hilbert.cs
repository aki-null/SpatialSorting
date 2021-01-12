using UnityEngine;

namespace SpatialSorting
{
    /// <summary>
    /// Provides an algorithm to convert coordinates to a Hilbert index.
    /// </summary>
    internal static class Hilbert
    {
        /// <summary>
        /// A struct to represent a comparable index with 3 integer components.
        /// </summary>
        public readonly struct Index3D : System.IComparable<Index3D>
        {
            private readonly ulong _xy;
            private readonly uint _z;

            public unsafe Index3D(uint* coord)
            {
                _xy = ((ulong) coord[0] << 32) | coord[1];
                _z = coord[2];
            }

            public int CompareTo(Index3D other)
            {
                var xyComparison = _xy.CompareTo(other._xy);
                return xyComparison != 0 ? xyComparison : _z.CompareTo(other._z);
            }
        }

        public static unsafe Index3D ToIndex3D(Vector3 pos, in SpaceMapper mapper)
        {
            var mappedIndices = stackalloc uint[3];
            mapper.Map(pos, mappedIndices);
            var res = stackalloc uint[3];
            ToLinearIndex(mappedIndices, res, 3);
            return new Index3D(res);
        }

        public static unsafe ulong ToIndex2D(Vector3 pos, in SpaceMapper mapper)
        {
            var mappingIndices = stackalloc uint[2];
            mapper.Map(pos, mappingIndices);
            var res = stackalloc uint[2];
            ToLinearIndex(mappingIndices, res, 2);
            return ((ulong) res[0] << 32) | res[1];
        }

        // Uses an algorithm from the following paper:
        // John Skilling, Programming the Hilbert curve, AIP Conference Proceedings 707, 381 (2004)
        // http://dx.doi.org/10.1063/1.1751381 

        /// <summary>
        /// Maps an integer coordinate to N-dimensional Hilbert curve index
        /// </summary>
        /// <param name="x">Input integer coordinate buffer</param>
        /// <param name="res">Output integer index buffer</param>
        /// <param name="n">Number of dimensions</param>
        private static unsafe void ToLinearIndex(uint* x, uint* res, int n)
        {
            // Bits
            const int b = sizeof(uint) * 8;

            const uint m = 1u << (b - 1);

            uint t;
            // Inverse undo
            for (var q = m; q > 1; q >>= 1)
            {
                var p = q - 1;
                for (var i = 0; i < n; i++)
                {
                    if ((x[i] & q) > 0) x[0] ^= p; // invert
                    else
                    {
                        t = (x[0] ^ x[i]) & p;
                        x[0] ^= t;
                        x[i] ^= t;
                    }
                }
            } // exchange

            // Gray encode
            for (var i = 1; i < n; i++) x[i] ^= x[i - 1];
            t = 0;
            for (var q = m; q > 1; q >>= 1)
            {
                if ((x[n - 1] & q) > 0) t ^= q - 1;
            }

            for (var i = 0; i < n; i++) x[i] ^= t;

            // Transposes a transposed index values
            var currentN = 0;
            var currentReadBitMask = m;
            for (var i = 0; i < n; i++)
            {
                var transposedResult = 0u;
                for (var currentOutBitMask = m; currentOutBitMask > 0; currentOutBitMask >>= 1)
                {
                    transposedResult |= (x[currentN] & currentReadBitMask) > 0 ? currentOutBitMask : 0;

                    currentN++;
                    if (currentN >= n)
                    {
                        currentN = 0;
                        currentReadBitMask >>= 1;
                    }
                }

                res[i] = transposedResult;
            }
        }
    }
}