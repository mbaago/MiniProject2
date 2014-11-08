using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace miniproject2
{
    class DetermineIfLikelyToBuy
    {
        public DetermineIfLikelyToBuy(Clusterer clusterMachine, List<List<int>> communities, List<int> userIndexes, List<string> userNames, List<Review> reviews)
        {
            ClusterMachine = clusterMachine;
            Communities = communities;
            UserIndexes = userIndexes;
            UserNames = userNames;
            Reviews = reviews;
        }

        private Clusterer ClusterMachine { get; set; }
        private List<List<int>> Communities { get; set; }
        private List<int> UserIndexes { get; set; }
        private List<string> UserNames { get; set; }
        private List<Review> Reviews { get; set; }

        public List<Tuple<string, int, string>> WillUsersBuy()
        {
            var result = new List<Tuple<string, int, string>>();
            foreach (var index in UserIndexes)
            {
                // has bought?
                // calc sentiment

                // else
                // calc sentiment
                // answer: likely to buy?
                var neighbours = ClusterMachine.NeighBours[index];
                foreach (var neighbour in neighbours)
                {
                    // har nabo reviewet? fortsæt
                    // er nabo i ANDET community? * 10
                    // er nabo kyle? * 10
                    // summer op
                    // divider med antal?
                }
            }

            return result;
        }
    }
}
