using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace miniproject2
{
    public class Review
    {
        public string productID { get; set; }
        public string userID { get; set; }
        public string profileName { get; set; }
        public Tuple<int, int> helpfulness { get; set; }
        public double score { get; set; }
        public DateTime time { get; set; }
        public string summary { get; set; }
        public string review { get; set; }

        
    }
}
