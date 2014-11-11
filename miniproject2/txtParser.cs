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
            FileInfo fi = new FileInfo("../../../" + file);
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

            p.review = person[3].Substring(person[3].IndexOf(':') + 2);

            return p;
        }

        

        public List<Review> parseReview(string file, int count)
        {
            FileInfo fi = new FileInfo(file);
            StreamReader reader = fi.OpenText();

            int n = count;
            int i = 0;
            string[] delimiter = new string[] { ":" };

            List<Review> reviews = new List<Review>();
            while (!reader.EndOfStream && ((n == 0) || (i < n)))
            {
                List<string> lines = parseToNewLine(reader);
                //Console.WriteLine(product);

                //string[] lines = product.Split(delimiter, StringSplitOptions.None);
                Review review = new Review();


                for (int z = 0; z < lines.Count(); z++)
                {
                    string line = trimString(lines[z].Substring(lines[z].IndexOf(": ")+ 1));
                    //string firstWord = l.Substring(0, l.IndexOf(" "));
                    switch (z)
                    {
                        case 0:
                            review.productID = line;
                            break;
                        case 1:
                            review.userID = line;
                            break;
                        case 2:
                            review.profileName = line;
                            break;
                        case 3:
                            //string[] s = Regex.Replace(lines[z], @"\s+", "").Substring(lines[z].IndexOf(":")).Split('/');
                            string[] s = line.Split('/');
                            
                            review.helpfulness = new Tuple<int, int>(Convert.ToInt32(s[0].Trim()), Convert.ToInt32(s[1].Trim()));
                            break;
                        case 4:
                            review.score = double.Parse((line), CultureInfo.InvariantCulture);
                            break;
                        case 5:
                            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0);
                            review.time = time.AddSeconds(Convert.ToDouble(line));
                            break;
                        case 6:
                            review.summary = trimString(line);
                            break;
                        case 7:
                            review.review = trimString(line);
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


        private List<string> parseToNewLine(StreamReader reader)
        {
            bool found = false;
            string line = "";
            List<string> result = new List<string>();
            while (!found && !reader.EndOfStream)
            {
                if (!String.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    result.Add(line);
                }
                else
                {
                    found = true;
                }
            }

            return result;
        }
    }
}
