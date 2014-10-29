using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace miniproject2
{
    public sealed class txtParser
    {
        static readonly txtParser instance = new txtParser();

        public static txtParser Instance
        {
            get
            {
                return instance;
            }
        }

        public List<Person> parseTxt(string file)
        {
            FileInfo fi = new FileInfo(file);
            StreamReader reader = fi.OpenText();
            string [] lines = new string[5];
            List<Person> persons = new List<Person>();


            while (!reader.EndOfStream)
            {
                for (int i = 0; i < 5; i++)
                {
                    lines[i] = reader.ReadLine();
                }
                persons.Add(parsePerson(lines));
            }

            return persons;
        }


        private Person parsePerson(string[] person)
        {
            Person p = new Person((person[0].Split())[1]);

            string[] friends = person[1].Split();

            for (int i = 1; i < friends.Length; i++)
			{
			    p.friends.Add(friends[i]);
			}

            return p; 
        }

    }
}
