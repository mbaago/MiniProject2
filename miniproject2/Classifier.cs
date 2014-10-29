using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace miniproject2
{
    class Classifier
    {
        private Dictionary<string, int> goodWords { get; set; }
        private Dictionary<string, int> badWords { get; set; }
        public Dictionary<string, Tuple<double, double>> wordProbability { get; set; }

        public int positiveReviews { get; set; }
        public int negativeReviews { get; set; }




        public Dictionary<string, Tuple<double, bool>> clasify(List<Review> list)
        {
            var learnList = list.Take(list.Count / 10);
            var testList = list.Skip(list.Count / 10).Take(list.Count / 20);

            calculateWordProbability(learnList.ToList());

            

            return rateReviews(testList.ToList());
        }


        private void addWordsToDictionary(string s, Dictionary<string, int> dict)
        {
            foreach (string item in Tokenizer.Tokenize(s))
            {
                if (dict.ContainsKey(item))
                {
                    dict[item]++;
                }
                else
                {
                    dict.Add(item, 1);
                }
            }
        }


        private void countWords(List<Review> list)
        {
            foreach (var item in list)
	        {
                if (item.score < 3)
                {
                    addWordsToDictionary(item.review, badWords);
                    negativeReviews++;
                }
                else if(item.score > 3)
                {
                    addWordsToDictionary(item.review, goodWords);
                    positiveReviews++;
                }
            }
        }

        private void calculateWordProbability(List<Review> list)
        {
            countWords(list);

            foreach (var item in goodWords)
            {
                wordProbability.Add(item.Key, new Tuple<double, double>((item.Value / list.Count), 0));
            }
            foreach (var item in badWords)
            {
                if (wordProbability.ContainsKey(item.Key))
                {
                    double posValue = wordProbability[item.Key].Item1;
                    wordProbability[item.Key] = new Tuple<double, double>(posValue, (item.Value / list.Count));
                }
            }
        }


        private Dictionary<string, Tuple<double, bool>> rateReviews(List<Review> list)
        {
            double emptyGood = calculateGoodEmpty(list.Count);
            double emptyBad = calculateBadEmpty(list.Count);

            foreach (var item in list)
            {
                reteReview(item, emptyGood, emptyBad);
            }


            return null;
        }

        private bool reteReview(Review item, double emptyGood, double emptyBad)
        {
            double propGood = 1;
            double propBad = 1;
            foreach (var s in Tokenizer.Tokenize(item.review))
            {
                propGood *= (wordProbability[s].Item1 / (1 - wordProbability[s].Item1));
                propBad *= (wordProbability[s].Item2 / (1 - wordProbability[s].Item2));
            }

            propGood *= emptyGood;
            propBad *= emptyBad;

            return propGood > propBad;

        }

        private double calculateGoodEmpty(int total)
        {
            double result = 1;
            foreach (var item in wordProbability)
            {
                result = result * item.Value.Item1;
            }
            return result * (positiveReviews/total);
        }

        private double calculateBadEmpty(int total)
        {
            double result = 1;
            foreach (var item in wordProbability)
            {
                result = result * item.Value.Item2;
            }
            return result * (negativeReviews / total);
        }



    }
}
