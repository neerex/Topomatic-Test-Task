using System.Collections.Generic;
using Utility;

namespace DataComparers
{
    public class DoubleComparer : IEqualityComparer<double>
    {
        public bool Equals(double x, double y) => x.EqualsWithEpsilon(y);
        public int GetHashCode(double value) => 0;
    }
}