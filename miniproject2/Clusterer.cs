using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace miniproject2
{
    public class Clusterer
    {
        public Clusterer(string fileName)
        {
            PersonNameIndex = new Dictionary<string, int>();
            People = new List<Person>();
            NeighBours = new Dictionary<int, List<int>>();

            LoadPeople(fileName);
            LoadNamesToIndex();
            LoadGraphArray();
        }

        private List<Person> People { get; set; }
        private Dictionary<string, int> PersonNameIndex { get; set; }
        public double[,] GraphArray { get; private set; }
        public bool[,] BoolGraphArray { get; private set; }
        private Dictionary<int, List<int>> NeighBours { get; set; }

        private double Jaccard(IEnumerable<Person> p1, IEnumerable<Person> p2)
        {
            var cap = p1.Intersect(p2);
            var cup = p1.Union(p2);
            var capSize = cap.Count();
            var cupSize = cup.Count();

            return capSize / cupSize;
        }

        private void LoadPeople(string fileName)
        {
            People = txtParser.Instance.parseTxt(fileName);

        }

        public IEnumerable<int> Neighbours(string personName)
        {
            return Neighbours(PersonNameIndex[personName]);
        }
        public IEnumerable<int> Neighbours(int personIndex)
        {
            //List<int> friends = new List<int>();

            //for (int i = 0; i < PersonNameIndex.Count; i++)
            //{
            //    if (BoolGraphArray[personIndex, i])
            //    {
            //        friends.Add(i);
            //    }
            //}

            //return friends;
            return NeighBours[personIndex];
        }

        private void LoadNamesToIndex()
        {
            int i = 0;
            foreach (var person in People)
            {
                PersonNameIndex.Add(person.name, i++);
            }
        }

        private void LoadGraphArray()
        {
            //GraphArray = new double[PersonNameIndex.Count, PersonNameIndex.Count];
            BoolGraphArray = new bool[PersonNameIndex.Count, PersonNameIndex.Count];

            foreach (var person in People)
            {
                int index = PersonNameIndex[person.name];
                NeighBours[index] = new List<int>();

                foreach (var friend in person.friends)
                {
                    var friendIndex = PersonNameIndex[friend];

                    //GraphArray[index, friendIndex] = 1;
                    BoolGraphArray[index, friendIndex] = true;
                    BoolGraphArray[friendIndex, index] = true;
                    NeighBours[index].Add(friendIndex);
                }
            }

        }

        public List<List<int>> DoClustering(int kcliqueness, int k)
        {
            // find all cliques (relaxed?) of size k

            var cliquesOfSize = CalculateQliquesV2(kcliqueness);
            var adjacents = CliquePercolationMethod(cliquesOfSize, k);


            //StringBuilder sb = new StringBuilder();
            //foreach (var item in cliquesOfSize.OrderBy(t => t.Count))
            //{
            //    sb.Append("," + item.Count());
            //}

            //Console.WriteLine(sb.ToString());

            return adjacents;
        }

        private List<List<int>> CliquePercolationMethod(List<List<int>> cliques, int k)
        {
            List<List<int>> adjacentCliques = new List<List<int>>();

            for (int i = 0; i < PersonNameIndex.Count; i++)
            {
                var thisClique = cliques[i];
                var adjacent = new List<int>(thisClique);

                for (int j = i + 1; j < PersonNameIndex.Count; j++)
                {
                    var nextClicque = cliques[j];

                    if (thisClique.Intersect(nextClicque).Count() > (k - 1))
                    {
                        adjacent.AddRange(nextClicque);
                    }
                }

                adjacentCliques.Add(new List<int>(adjacent.Distinct()));
            }

            return adjacentCliques;
        }

        private List<List<int>> CalculateCliques()
        {
            List<List<int>> cliques = new List<List<int>>();

            for (int v = 0; v < PersonNameIndex.Count; v++)
            {
                var clique = new List<int>();
                clique.Add(v);

                var toBetried = new Stack<int>();
                foreach (var u in Neighbours(v))
                {
                    toBetried.Push(u);
                }


                while (toBetried.Count > 0)
                {
                    var next = toBetried.Pop();

                    if (IsNeighbourWithAll(clique, next))
                    {
                        foreach (var neighbour in Neighbours(next))
                        {
                            toBetried.Push(neighbour);
                        }
                        clique.Add(next);
                    }
                }

                cliques.Add(clique);
            }

            return cliques;
        }

        private List<List<int>> CalculateQliquesV2(int k)
        {
            List<List<int>> cliques = new List<List<int>>();
            System.IO.File.Delete(@"../../../cliques.txt");
            //System.IO.TextWriter writer = new System.IO.StreamWriter(@"../../../cliques.txt", true);

            for (int v = 0; v < PersonNameIndex.Count; v++)
            {
                COUNTER = 0;
                var clique = new List<int>();
                clique.Add(v);
                var toBetried = new Queue<int>(Neighbours(v));


                while (toBetried.Count > 0)
                {
                    var next = toBetried.Dequeue();
                    if (clique.Contains(next))
                    {
                        continue;
                    }

                    if (CanReachAllInkHops(next, clique, k))
                    {
                        clique.Add(next);
                        foreach (var item in Neighbours(next))
                        {
                            toBetried.Enqueue(item);
                        }
                    }
                }

                cliques.Add(clique);

                Console.WriteLine("Clique " + v + " created, size: " + clique.Count);
                var line = string.Join(",", clique);
                //writer.WriteLine(line);
                System.IO.File.AppendAllText(@"../../../cliques.txt", line + Environment.NewLine);
            }

            return cliques;
        }


        private bool IsNeighbourWithAll(IEnumerable<int> all, int v)
        {
            var neighbours = Neighbours(v);

            foreach (var u in all)
            {
                if (!neighbours.Contains(u))
                {
                    return false;
                }
            }

            return true;
        }

        int COUNTER = 0;
        bool CanReachAllInkHops(int v, IEnumerable<int> all, int k)
        {
            Debug.WriteLine("CANREACH: " + COUNTER++);
            foreach (var u in all)
            {
                int sp = ShortestPath(v, u, k);
                if (sp < 0)
                {
                    return false;
                }
            }

            return true;
        }

        private int ShortestPath(int v, int u, int k)
        {
            if (v == u)
            {
                return 0;
            }

            int depth = 1;
            Queue<int> thisLevel = new Queue<int>(Neighbours(v));
            Queue<int> nextLevel = new Queue<int>();

            while (depth <= k)
            {
                while (thisLevel.Count > 0)
                {
                    var i = thisLevel.Dequeue();
                    if (i == u)
                    {
                        return depth;
                    }
                    else
                    {
                        foreach (var item in Neighbours(i))
                        {
                            nextLevel.Enqueue(item);
                        }
                    }
                }

                thisLevel = nextLevel;
                nextLevel = new Queue<int>();
                depth++;
            }

            return -1;
        }

        //public void cluster()
        //{
        //    LoadGraphArray("../../../friendships.reviews.txt");
        //    List<DocumentVector> vecs = new List<DocumentVector>();
        //    int totalIt = 0;
        //    foreach (var p in People)
        //    {
        //        DocumentVector d = new DocumentVector();
        //        d.Content = p.name;
        //        d.VectorSpace = new float[p.friends.Count];
        //        for (int i = 0; i < p.friends.Count; i++)
        //        {
        //            d.VectorSpace[i] = PersonNameIndex[p.friends[i]];
        //        }
        //        vecs.Add(d);
        //    }

        //    var result = DocumnetClustering.PrepareDocumentCluster(7, vecs, ref totalIt);

        //    string s;
        //}
    }
}
