using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Probes
{
    public class ComNameComparer : StringComparer
    {
        public override int Compare(string x, string y)
        {
            return this.TryParseNumber(x) - this.TryParseNumber(y);
        }

        public override bool Equals(string x, string y)
        {
            return this.TryParseNumber(x) == this.TryParseNumber(y);
        }

        public override int GetHashCode(string obj)
        {
            return obj != null ? this.TryParseNumber(obj) : 0;
        }

        private int TryParseNumber(string name)
        {
            int n = -1;
            if (!string.IsNullOrEmpty(name) && name.Length > 3)
            {
                if (int.TryParse(name.Substring(3), out n))
                {

                }
            }
            return n;
        }
    }
}
