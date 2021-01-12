using UnityEngine;

namespace SpatialSorting
{
    /// <summary>
    /// Provides an algorithm to convert coordinates to a Morton index.
    /// </summary>
    internal static class Morton
    {
        public static unsafe ulong ToIndex3D(Vector3 pos, in SpaceMapper mapper)
        {
            var mappedIndices = stackalloc uint[3];
            mapper.Map(pos, mappedIndices);
            // Split bits and interleave them
            return Split3D(mappedIndices[0]) | (Split3D(mappedIndices[1]) << 1) | (Split3D(mappedIndices[2]) << 2);
        }

        public static unsafe ulong ToIndex2D(Vector3 pos, in SpaceMapper mapper)
        {
            var mappedIndices = stackalloc uint[2];
            mapper.Map(pos, mappedIndices);
            // Split bits and interleave them
            return Split2D(mappedIndices[0]) | (Split2D(mappedIndices[1]) << 1);
        }

        private static ulong Split2D(ulong x)
        {
            // Split bits (32-bits)
            // 0b0000000000000000111111111111111100000000000000001111111111111111
            x = (x | (x << 16)) & 0x0000FFFF0000FFFFul;
            // 0b0000000011111111000000001111111100000000111111110000000011111111
            x = (x | (x << 8)) & 0x00FF00FF00FF00FFul;
            // 0b0000111100001111000011110000111100001111000011110000111100001111
            x = (x | (x << 4)) & 0x0F0F0F0F0F0F0F0Ful;
            // 0b0011001100110011001100110011001100110011001100110011001100110011
            x = (x | (x << 2)) & 0x3333333333333333ul;
            // 0b0101010101010101010101010101010101010101010101010101010101010101
            x = (x | (x << 1)) & 0x5555555555555555ul;
            return x;
        }

        private static ulong Split3D(ulong x)
        {
            // Split bits (21-bits) - Leftmost bit unused
            // 0b0000000000011111000000000000000000000000000000001111111111111111
            x = (x | (x << 32)) & 0x001F00000000FFFFul;
            // 0b0000000000011111000000000000000011111111000000000000000011111111
            x = (x | (x << 16)) & 0x001F0000FF0000FFul;
            // 0b0001000000001111000000001111000000001111000000001111000000001111
            x = (x | (x << 8)) & 0x100F00F00F00F00Ful;
            // 0b0001000011000011000011000011000011000011000011000011000011000011
            x = (x | (x << 4)) & 0x10C30C30C30C30C3ul;
            // 0b0001001001001001001001001001001001001001001001001001001001001001
            x = (x | (x << 2)) & 0x1249249249249249ul;
            return x;
        }
    }
}