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
        public bool[,] GraphArray { get; private set; }

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

        private IEnumerable<Person> Neighbours(Person p1)
        {
            throw new NotImplementedException();
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

            GraphArray = new bool[PersonNameIndex.Count, PersonNameIndex.Count];

            foreach (var person in People)
            {
                int index = PersonNameIndex[person.name];

                foreach (var friend in person.friends)
                {
                    var friendIndex = PersonNameIndex[friend];

                    GraphArray[index, friendIndex] = true;
                }
            }
        }
    }
}
