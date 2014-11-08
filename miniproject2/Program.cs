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

            Clusterer cl = new Clusterer(@"../../../friendships.reviews.txt");

            var clusters = cl.DoClustering(2);

            Console.WriteLine(clusters);

            Console.ReadKey();
        }

        private static void runClasifier()
        {
            DateTime start = DateTime.Now;
            var list = txtParser.Instance.parseReview("SentimentTrainingData.txt", 0);
            Classifier c = new Classifier();

            var result = c.clasify(list);

            string review = "";
            int correct = 0;
            int neutral = 0;
            foreach (var item in result)
            {
                if ((item.Value.Item1 == 3))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    if (item.Value.Item2)
                    {
                        review = "Good review";
                    }
                    else
                    {
                        review = "Bad review";
                    }
                    neutral++;

                }
                else if ((item.Value.Item1 > 3) == item.Value.Item2)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    review = "Good review";
                    correct++;
                }
                else if ((item.Value.Item1 < 3) != item.Value.Item2)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    review = "Bad review";
                    correct++;
                }

                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (item.Value.Item2)
                    {
                        review = "Good review";
                    }
                    else
                    {
                        review = "Bad review";
                    }

                }
                Console.WriteLine(item.Key + " Score" + item.Value.Item1 + review);
            }
            Console.WriteLine("*********************************************");
            Console.WriteLine("Started: " + start.ToString());
            Console.WriteLine("Finished: " + DateTime.Now.ToString());
            Console.WriteLine("Correctness: " + ((double)correct / (result.Count - neutral)) * 100 + "%");
            Console.WriteLine("*********************************************");
            Console.ReadKey();
        }
    }
}
