using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Tests
{
    public class CaseInsensitiveStringComparer : System.Collections.Generic.IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return string.Equals(x, y, System.StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            if (obj == null) return 0;
            return obj.ToLowerInvariant().GetHashCode();
        }
    }
}
