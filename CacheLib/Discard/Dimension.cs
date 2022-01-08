using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLib.Discard
{
    public readonly struct Dimension
    {
        public static explicit operator Dimension(int dimension) => new Dimension(dimension);

        public static implicit operator int(Dimension dimension) => dimension.Value;

        public static Dimension NoChange => new Dimension();

        public int Value { get; }

        public Dimension(int dimension)
        {
            if (dimension <= 0) throw new ArgumentOutOfRangeException(nameof(dimension));

            Value = dimension;
        }
    }
}
