using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace miniproject2
{
    public class Person
    {
        public string name { get; set; }
        public string review { get; set; }
        public string summery { get; set; }

        public List<String> friends { get; set; }


        public Person(string name)
        {
            this.name = name;
            friends = new List<string>();
        }


        public override string ToString()
        {
            return name;
        }
    }
}
