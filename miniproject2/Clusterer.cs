using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace miniproject2
{
    public class Clusterer
    {
        public Clusterer(string fileName)
        {
            PersonNameIndex = new Dictionary<string, int>();
            People = new List<Person>();

            LoadPeople(fileName);
            LoadNamesToIndex();
            LoadGraphArray();
        }

        private List<Person> People { get; set; }
        private Dictionary<string, int> PersonNameIndex { get; set; }
        public double[,] GraphArray { get; private set; }
        public bool[,] BoolGraphArray { get; private set; }

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
            List<int> friends = new List<int>();

            for (int i = 0; i < PersonNameIndex.Count; i++)
            {
                if (BoolGraphArray[personIndex, i])
                {
                    friends.Add(i);
                }
            }

            return friends;
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

                foreach (var friend in person.friends)
                {
                    var friendIndex = PersonNameIndex[friend];

                    //GraphArray[index, friendIndex] = 1;
                    BoolGraphArray[index, friendIndex] = true;
                    BoolGraphArray[friendIndex, index] = true;
                }
            }
        }

        public List<List<int>> DoClustering(int k)
        {
            // find all cliques (relaxed?) of size k

            var cliquesOfSize = CalculateCliques();
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
