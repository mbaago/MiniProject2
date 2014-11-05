using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace miniproject2
{
    class Classifier
    {
        private Dictionary<string, double> goodWords { get; set; }
        private Dictionary<string, double> badWords { get; set; }
        public Dictionary<string, Tuple<double, double>> wordProbability { get; set; }

        public int positiveReviews { get; set; }
        public int negativeReviews { get; set; }
        public double propGood { get; set; }
        public double propNeg{ get; set; }

        public Classifier()
        {
            goodWords = new Dictionary<string, double>();
            badWords = new Dictionary<string, double>();
            wordProbability = new Dictionary<string, Tuple<double, double>>();

        }



        public Dictionary<string, Tuple<double, bool>> clasify(List<Review> list)
        {
            int tenth = list.Count / 10;
            List<Dictionary<string, Tuple<double, bool>>> result = new List<Dictionary<string,Tuple<double,bool>>>();
            

            for (int i = 0; i < 10; i++)
            {
                var firstLearnList = list.Take(i * tenth);
                var testList = list.Skip(i * tenth).Take(tenth);
                var learnList = list.Skip(tenth * (i + 1));

                calculateWordProbability(learnList.ToList());



                return rateReviews(testList.ToList(), learnList.Count());
            }

            return null;
           
        }


        private void addWordsToDictionary(string s, Dictionary<string, double> dict)
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
                wordProbability.Add(item.Key, new Tuple<double, double>(maxValue(1,(item.Value / list.Count)), 0));
            }
            foreach (var item in badWords)
            {
                if (wordProbability.ContainsKey(item.Key))
                {
                    double posValue = wordProbability[item.Key].Item1;
                    wordProbability[item.Key] = new Tuple<double, double>(posValue,  maxValue(1,(item.Value / list.Count)));
                }
            }
            var temp = new Dictionary<string, Tuple<double, double>>();
            //foreach (var item in wordProbability)
            //{
            //    double posValues = Math.Log10(positiveReviews) + item.Value.Item1;
            //    double negVAlue = Math.Log10(negativeReviews) + item.Value.Item2;
            //    temp[item.Key] = new Tuple<double, double>(posValues, negVAlue);
            //}

            //wordProbability = temp;
        }

        private double maxValue(int p1, double p2)
        {
            if (p2 > p1)
            {
                return 1;
            }
            return p2;
        }


        private Dictionary<string, Tuple<double, bool>> rateReviews(List<Review> list, int total)
        {
            double emptyGood = calculateGoodEmpty(list.Count, total);
            double emptyBad = calculateBadEmpty(list.Count, total);
            var result = new Dictionary<string, Tuple<double, bool>>();

            foreach (var item in list)
            {
                result[item.productID + item.userID] = new Tuple<double,bool>(item.score, reteReview(item, emptyGood, emptyBad));
            }


            return result;
        }

        private bool reteReview(Review item, double emptyGood, double emptyBad)
        {
            double propGood = 1;
            double propBad = 1;
            foreach (var s in Tokenizer.Tokenize(item.review))
            {
                if (wordProbability.ContainsKey(s))
                {
                    double t = (wordProbability[s].Item1 / (1 - wordProbability[s].Item1));
                    propGood *= t;
                    propBad *= (wordProbability[s].Item2 / (1 - wordProbability[s].Item2));
                }
                
            }

            propGood *= emptyGood;
            propBad *= emptyBad;

            return propGood > propBad;

        }

        private double calculateGoodEmpty(int good, int total)
        {
            double result = 1;
            foreach (var item in wordProbability)
            {
                result = result * (1 - item.Value.Item1);
            }
            result *= ((double)good/total);
            return result;
        }

        private double calculateBadEmpty(int neg, int total)
        {
            double result = 1;
            foreach (var item in wordProbability)
            {
                result = result * (1 - item.Value.Item2);
            }
            return result * ((double)neg / total);
        }



    }
}
