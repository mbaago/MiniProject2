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
            //Clustering();
            //Classifier classifier = runClasifier();
            //parsetxt();

            var classifier = learn("SentimentTrainingData.txt", 0);

            Clusterer clusterer = new Clusterer(@"friendships.reviews.txt", @"../../../cliques.txt", @"../../../clusters.txt");
            var clusters = clusterer.DoClustering(2, 2);

            var det = new DetermineIfLikelyToBuy(clusterer, classifier, clusters);

            var res = det.WillUsersBuy();

            if (System.IO.File.Exists(@"../../../result.txt"))
            {
                System.IO.File.Delete(@"../../../result.txt");
            }

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"../../../result.txt"))
            {
                foreach (var item in res)
                {
                    string it = item.Item1 + "\t" + item.Item2 + "\t" + item.Item3;
                    writer.WriteLine(it);
                }
            }

            //learn("SentimentTrainingData.txt", 0);

        }

        private static void parsetxt()
        {
            txtParser parser = new txtParser();
            parser.parseTxt("friendships.reviews.txt");
        }

        private static void Clustering()
        {
            Clusterer clusterer = new Clusterer(@"../../../friendships.reviews.txt", @"../../../cliques.txt", @"../../../clusters.txt");
            var clusters = clusterer.DoClustering(2, 2);


            Console.WriteLine("done");


            Console.ReadKey();
        }

        private static Classifier learn(string file, int n)
        {
            var list = txtParser.Instance.parseReview(file, n);
            Classifier c = new Classifier();
            c.learn(list);

            return c;
        }

        private static void runClasifier()
        {
            DateTime start = DateTime.Now;
            var list = txtParser.Instance.parseReview("SentimentTrainingData.txt", 10000);
            Classifier c = new Classifier();

            var result = c.clasify(list);

            string review = "";
            int correct = 0;
            int neutral = 0;
            //foreach (var item in result)
            //{
            //    if ((item.Value.Item1 == 3))
            //    {
            //        Console.ForegroundColor = ConsoleColor.Yellow;
            //        if (item.Value.Item2)
            //        {
            //            review = "Good review";
            //        }
            //        else
            //        {
            //            review = "Bad review";
            //        }
            //        neutral++;

            //    }
            //    else if ((item.Value.Item1 > 3) == item.Value.Item2)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Green;
            //        review = "Good review";
            //        correct++;
            //    }
            //    else if ((item.Value.Item1 < 3) != item.Value.Item2)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Green;
            //        review = "Bad review";
            //        correct++;
            //    }

            //    else
            //    {
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        if (item.Value.Item2)
            //        {
            //            review = "Good review";
            //        }
            //        else
            //        {
            //            review = "Bad review";
            //        }

            //    }
            //    Console.WriteLine(item.Key + " Score" + item.Value.Item1 + review);
            //}

            double avgScore = 0;
            Console.WriteLine("*********************************************");
            Console.WriteLine("Started: " + start.ToString());
            Console.WriteLine("Finished: " + DateTime.Now.ToString());
            foreach (var item in result)
            {
                Console.WriteLine("Part " + item.Key + " correctness: " + item.Value + "%");
                avgScore += item.Value;
            }
            Console.WriteLine("Average score: " + avgScore / 10);
            Console.WriteLine("*********************************************");
            Console.ReadKey();
        }
    }
}
