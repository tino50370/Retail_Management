using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class JobProfile
    {
        public String CustomerId { set; get; }
        public String MobileNumber { set; get; }
        public String CustomerName { set; get; }
        public String JobCategory { set; get; }
        public String Description { set; get; }
        public String Image { set; get; }
        public String ProfileRating { set; get; }
        public Int32 RatingCount { set; get; }
        public Int32 JobsDone { set; get; }
        public DateTime ExpireryDate { set; get; }
        public Int32 CustomerRatings { set; get; }
        public IEnumerable<JobSubCategory> OtherSkills { set; get; }
    }
}