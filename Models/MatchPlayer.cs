using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScraperApp.Models
{    
    public class MatchPlayer
    {
        public string matchid { get; set; }
        public string playername { get; set; }
        public string playercategory { get; set; }
        public int playercategoryid { get; set; }
        public string originalmatchid { get; set; }
        public int point { get; set; }
        public double credit { get; set; }
        public int isannnouce { get; set; }
        public string playerurl { get; set; }
    }
    public class PlayerCategory
    {
     public int playercategoryId { get; set; }
     public string playercategory { get; set; }
      public string playercategoryshortform { get; set; }
    }
}
