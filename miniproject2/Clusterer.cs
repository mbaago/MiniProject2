using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace miniproject2
{
    public class Clusterer
    {
        private double Jaccard(IEnumerable<Person> p1, IEnumerable<Person> p2)
        {
            var cap = p1.Intersect(p2);
            var cup = p1.Union(p2);
            var capSize = cap.Count();
            var cupSize = cup.Count();

            return capSize / cupSize;
        }


    }
}
