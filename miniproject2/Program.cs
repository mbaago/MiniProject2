using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace miniproject2
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<Person> persons = txtParser.Instance.parseTxt("friendships.txt");


            //foreach (Person p in persons)
            //{
            //    Console.WriteLine("***********************************************");
            //    Console.WriteLine("Username: " + p.name);
            //    Console.WriteLine("Friends");
            //    foreach (string s in p.friends)
            //    {
            //        Console.WriteLine(s);
            //    }
            //    Console.WriteLine("***********************************************");
            //    Console.ReadKey();
            //}

            dynamic list = txtParser.Instance.parseReview("SentimentTrainingData.txt", 1000);
            Classifier c = new Classifier();

            var result = c.clasify(list);

            foreach (var item in result)
            {
            }
            Console.ReadKey();
        }
    }
}
