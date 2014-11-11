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
        public Clusterer(string peopleFile, string cliquesFile, string clustersFile)
        {
            PersonNameIndex = new Dictionary<string, int>();
            People = new List<Person>();
            NeighBours = new Dictionary<int, List<int>>();

            LoadPeople(peopleFile);
            LoadNamesToIndex();
            LoadGraphArray();

            CLiquesFile = cliquesFile;
            ClustersFile = clustersFile;
        }

        private string CLiquesFile { get; set; }
        private string ClustersFile { get; set; }
        public List<Person> People { get; set; }
        public Dictionary<string, int> PersonNameIndex { get; set; }
        public double[,] GraphArray { get; private set; }
        public bool[,] BoolGraphArray { get; private set; }
        public Dictionary<int, List<int>> NeighBours { get; set; }

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

        //public IEnumerable<int> Neighbours(string personName)
        //{
        //    return Neighbours(PersonNameIndex[personName]);
        //}
        //public IEnumerable<int> Neighbours(int personIndex)
        //{
        //    //List<int> friends = new List<int>();

        //    //for (int i = 0; i < PersonNameIndex.Count; i++)
        //    //{
        //    //    if (BoolGraphArray[personIndex, i])
        //    //    {
        //    //        friends.Add(i);
        //    //    }
        //    //}

        //    //return friends;
        //    return NeighBours[personIndex];
        //}

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

        public List<List<int>> DoClustering(int k, int cpm)
        {
            // Find all k-plexes
            var cliquesOfSize = CalculateClustersOrReadFromFile(k);

            // Use CPM
            // Little different from slides
            // We use another value of k for the clique graph
            var CPM = CPMOrReadFromFile(cliquesOfSize, k);

            return CPM;
        }

        private List<List<int>> CPMOrReadFromFile(List<List<int>> cliques, int k)
        {
            if (System.IO.File.Exists(ClustersFile))
            {
                return ReadListOfCliquesOrClusters(ClustersFile);
            }
            else
            {
                return CliquePercolationMethod(cliques, k);
            }
        }

        private List<List<int>> CliquePercolationMethod(List<List<int>> cliques, int k)
        {
            System.IO.File.Delete(ClustersFile);
            var adjacentCliques = new List<List<List<int>>>();

            // walk over the remaining
            for (int i = 0; i < PersonNameIndex.Count; i++)
            {
                if (i % 100 == 0) { Debug.WriteLine(i); }

                var thisClique = cliques[i];

                // add to all? seems to add to literally all
                //var indexes = AllFitsOrEmpty(adjacentCliques, thisClique, k-1);

                //if (indexes.Count == 0)
                //{
                //    adjacentCliques.Add(new List<List<int>>());
                //    adjacentCliques.Last().Add(thisClique);
                //}
                //else
                //{
                //    foreach (var index in indexes)
                //    {
                //        adjacentCliques[index].Add(thisClique);
                //    }
                //}

                // add to first fit
                int index = FirstFitOrNegative(adjacentCliques, thisClique, k);
                if (index < 0)
                {
                    adjacentCliques.Add(new List<List<int>>());
                    adjacentCliques.Last().Add(thisClique);
                }
                else
                {
                    adjacentCliques[index].Add(thisClique);
                }
            }

            var communities = new List<List<int>>();
            foreach (var connected in adjacentCliques)
            {
                var thing = connected.SelectMany(x => x).Distinct().OrderBy(x => x);
                communities.Add(thing.ToList());
            }

            foreach (var community in communities)
            {
                var line = string.Join(",", community);
                System.IO.File.AppendAllText(ClustersFile, line + Environment.NewLine);
            }

            // one person might be shared in communites
            return communities;
        }

        private List<int> AllFitsOrEmpty(List<List<List<int>>> connectedCliques, List<int> clique, int k)
        {
            var result = new List<int>();

            for (int i = 0; i < connectedCliques.Count; i++)
            {
                for (int j = 0; j < connectedCliques[i].Count; j++)
                {
                    if (connectedCliques[i][j].Intersect(clique).Count() >= k)
                    {
                        result.Add(i);
                        break;
                    }
                }
            }

            return result;
        }

        private int FirstFitOrNegative(List<List<List<int>>> connectedCliques, List<int> clique, int k)
        {
            for (int i = 0; i < connectedCliques.Count; i++)
            {
                for (int j = 0; j < connectedCliques[i].Count; j++)
                {
                    if (connectedCliques[i][j].Intersect(clique).Count() >= k)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }


        private List<List<int>> CalculateClustersOrReadFromFile(int kcliqueness)
        {
            if (System.IO.File.Exists(CLiquesFile))
            {
                return ReadListOfCliquesOrClusters(CLiquesFile);
            }
            else
            {
                return CalculateQliquesV2(kcliqueness);
            }
        }

        private List<List<int>> ReadListOfCliquesOrClusters(string file)
        {
            var values = new List<List<int>>();

            char[] splitChars = new char[] { ',' };
            using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    string str = reader.ReadLine();
                    var splitted = str.Split(splitChars);
                    var read = splitted
                        .Select(c => int.Parse(c));
                    values.Add(new List<int>(read));
                }
            }

            return values;
        }

        private List<List<int>> CalculateQliquesV2(int k)
        {
            List<List<int>> cliques = new List<List<int>>();
            System.IO.File.Delete(CLiquesFile);
            Stopwatch watch = new Stopwatch();

            for (int v = 0; v < PersonNameIndex.Count; v++)
            {
                watch.Restart();
                var clique = new List<int>();
                clique.Add(v);
                var toBetried = new Queue<int>(NeighBours[v]);


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
                        foreach (var item in NeighBours[next])
                        {
                            toBetried.Enqueue(item);
                        }
                    }
                }

                Console.WriteLine("Clique " + v.ToString().PadLeft(4) + " created, size: " + clique.Count.ToString().PadLeft(3) + " Time: " + (int)watch.Elapsed.TotalSeconds);
                var line = string.Join(",", clique);
                //writer.WriteLine(line);
                System.IO.File.AppendAllText(CLiquesFile, line + Environment.NewLine);
            }

            return cliques;
        }


        private bool IsNeighbourWithAll(IEnumerable<int> all, int v)
        {
            var neighbours = NeighBours[v];

            foreach (var u in all)
            {
                if (!neighbours.Contains(u))
                {
                    return false;
                }
            }

            return true;
        }

        bool CanReachAllInkHops(int v, IEnumerable<int> all, int k)
        {
            //Debug.WriteLine("CANREACH: " + COUNTER++);
            foreach (var u in all)
            {
                int sp = ShortestPath(v, u, k);
                if (sp < 0)
                {
                    return false;
                }
            }

            //var canReach = RecursiveGetNeighbours(v, k).ToList();

            //foreach (var any in all)
            //{
            //    if (!canReach.Contains(any))
            //    {
            //        return false;
            //    }
            //}

            return true;
        }

        private IEnumerable<int> RecursiveGetNeighbours(int v, int d)
        {
            var neighbours = NeighBours[v].AsEnumerable();
            if (d >= 1)
            {
                foreach (var neigh in neighbours)
                {
                    neighbours = neighbours.Union(RecursiveGetNeighbours(v, d - 1));
                }
            }

            neighbours = neighbours.Distinct();

            return neighbours;
        }

        private int ShortestPath(int v, int u, int k)
        {
            if (v == u)
            {
                return 0;
            }

            int depth = 1;
            Queue<int> thisLevel = new Queue<int>(NeighBours[v]);
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
                        foreach (var item in NeighBours[i])
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
