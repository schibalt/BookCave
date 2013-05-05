using BookCave.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCave.Service.Entities
{
    public class SuperRecord
    {
        public string Isbn13 { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Isbn { get; set; }
        public string Publisher { get; set; }
        public Nullable<short> PageCount { get; set; }
        public string DocType { get; set; }
        public string Series { get; set; }
        public string Awards { get; set; }
        public string Summary { get; set; }
        public string ScholasticGradeLower { get; set; }
        public Nullable<byte> ScholasticGradeHigher { get; set; }
        public Nullable<byte> BarnesAgeYoung { get; set; }
        public Nullable<byte> BarnesAgeOld { get; set; }
        public Nullable<byte> CommonSensePause { get; set; }
        public Nullable<byte> CommonSenseOn { get; set; }
        public Nullable<bool> CommonSenseNoKids { get; set; }
        public Nullable<double> AverageContentAge { get; set; }
        public Nullable<double> BarnesAvg { get; set; }
        public Nullable<double> ScholasticGrade { get; set; }
        public string Dra { get; set; }
        public Nullable<short> LexScore { get; set; }
        public string GuidedReading { get; set; }
        public string LexCode { get; set; }
        public Nullable<System.DateTime> LexUpdate { get; set; }
        public Nullable<double> AverageSkillAge { get; set; }
    }
}
