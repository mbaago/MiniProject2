using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace miniproject2
{
    public class Clusterer
    {
        public Clusterer()
        {
            PersonNameIndex = new Dictionary<string, int>();
            People = new List<Person>();
        }

        private List<Person> People { get; set; }
        private Dictionary<string, int> PersonNameIndex { get; set; }
        public double[,] GraphArray { get; private set; }

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
                if (GraphArray[personIndex, i] > 0)
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

        public void LoadGraphArray(string fileName)
        {
            LoadPeople(fileName);
            LoadNamesToIndex();

            GraphArray = new double[PersonNameIndex.Count, PersonNameIndex.Count];

            foreach (var person in People)
            {
                int index = PersonNameIndex[person.name];

                foreach (var friend in person.friends)
                {
                    var friendIndex = PersonNameIndex[friend];

                    GraphArray[index, friendIndex] = 1;
                }
            }
        }
    }
}
