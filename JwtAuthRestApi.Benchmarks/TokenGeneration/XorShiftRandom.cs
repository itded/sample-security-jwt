using System;

namespace JwtAuthRestApi.Benchmarks.TokenGeneration
{
    /// <summary>
    /// http://codingha.us/2018/12/17/xorshift-fast-csharp-random-number-generator/
    /// </summary>
    public class XorShiftRandom
    {
        private ulong _x;
        private ulong _y;

        public XorShiftRandom()
        {
            _x = ( ulong ) Guid.NewGuid().GetHashCode();
            _y = ( ulong ) Guid.NewGuid().GetHashCode();
        }

        public unsafe void NextBytes( byte[] buffer )
        {
            // Localize state for stack execution
            ulong x = _x, y = _y, tempX, tempY, z;

            fixed ( byte* pBuffer = buffer )
            {
                ulong* pIndex = ( ulong* ) pBuffer;
                ulong* pEnd = ( ulong* ) ( pBuffer + buffer.Length );

                // Fill array in 8-byte chunks
                while ( pIndex <= pEnd - 1 )
                {
                    tempX = y;
                    x ^= x << 23; tempY = x ^ y ^ ( x >> 17 ) ^ ( y >> 26 );

                    *( pIndex++ ) = tempY + y;

                    x = tempX;
                    y = tempY;
                }

                // Fill remaining bytes individually to prevent overflow
                if ( pIndex < pEnd )
                {
                    tempX = y;
                    x ^= x << 23; tempY = x ^ y ^ ( x >> 17 ) ^ ( y >> 26 );
                    z = tempY + y;

                    byte* pByte = ( byte* ) pIndex;
                    while ( pByte < pEnd ) *( pByte++ ) = ( byte ) ( z >>= 8 );
                }
            }

            // Store modified state in fields.
            _x = x;
            _y = y;
        }
    }
}
