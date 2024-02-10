using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTrackingApp
{
    public class Book
    {
        public int BookID { get; set; }
        public string BookName { get; set; }
        public int AuthorID { get; set; }
        public bool Read { get; set; }
        public bool ToRead { get; set; }
        public bool NowReading { get; set; }
    }
}
