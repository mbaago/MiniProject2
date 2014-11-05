using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

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
            string[] lines = new string[5];
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
            Person p = new Person((person[0].Split(new char[] { ' ' }, 2))[1]);

            string[] friends = person[1].Split(new char[] { '\t' });

            for (int i = 1; i < friends.Length; i++)
            {
                p.friends.Add(friends[i]);
            }

            return p;
        }

        public List<Review> parseReview(string file, int count)
        {
            FileInfo fi = new FileInfo(file);
            StreamReader reader = fi.OpenText();

            int n = count;
            int i = 0;
            string[] delimiter = new string[] { "review/" };

            List<Review> reviews = new List<Review>();
            while (!reader.EndOfStream && ((n == 0) || (i < n)))
            {
                string product = parseToNewLine(reader);
                //Console.WriteLine(product);

                string[] lines = product.Split(delimiter, StringSplitOptions.None);
                Review review = new Review();


                foreach (string l in lines)
                {
                    string firstWord = l.Substring(0, l.IndexOf(" "));
                    switch (firstWord)
                    {
                        case "product/productId:":
                            review.productID = trimString(l.Substring(l.IndexOf(" ")));
                            break;
                        case "userId:":
                            review.userID = trimString(l.Substring(l.IndexOf(" ")));
                            break;
                        case "profileName:":
                            review.profileName = trimString(l.Substring(l.IndexOf(" ")));
                            break;
                        case "helpfulness:":
                            string[] s = Regex.Replace(l, @"\s+", "").Substring(l.IndexOf(" ")).Split('/');

                            review.helpfulness = new Tuple<int, int>(Convert.ToInt32(s[0].Trim()), Convert.ToInt32(s[1].Trim()));
                            break;
                        case "score:":
                            review.score = double.Parse(l.Substring(l.IndexOf(" ")), CultureInfo.InvariantCulture);
                            break;
                        case "time:":
                            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0);
                            review.time = time.AddSeconds(Convert.ToDouble(l.Substring(l.IndexOf(" "))));
                            break;
                        case "summary:":
                            review.summary = trimString(l.Substring(l.IndexOf(" ")));
                            break;
                        case "text:":
                            review.review = trimString(l.Substring(l.IndexOf(" ")));
                            break;
                        default:
                            break;
                    }


                }
                reviews.Add(review);
                i++;

            }

            return reviews;
        }

        private string trimString(string s)
        {
            char[] trimChars = { '\r', '\n', '\t', ' ' };
            return s.Trim(trimChars);
        }


        private string parseToNewLine(StreamReader reader)
        {
            bool found = false;
            string line = "";
            StringBuilder sb = new StringBuilder();
            while (!found && !reader.EndOfStream)
            {
                if (!String.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    sb.AppendLine(line);
                }
                else
                {
                    found = true;
                }
            }

            return sb.ToString();
        }
    }
}
