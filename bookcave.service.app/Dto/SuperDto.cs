using System;
using System.Runtime.Serialization;

namespace BookCave.Service.Dto
{
    public class SuperDto
    {
        [DataMember]
        public string Isbn13 { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public string Isbn { get; set; }

        [DataMember]
        public string Publisher { get; set; }

        [DataMember]
        public Nullable<short> PageCount { get; set; }

        [DataMember]
        public string DocType { get; set; }

        [DataMember]
        public string Series { get; set; }

        [DataMember]
        public string Awards { get; set; }

        [DataMember]
        public string Summary { get; set; }

        [DataMember]
        public string ScholasticGradeLower { get; set; }

        [DataMember]
        public Nullable<byte> ScholasticGradeHigher { get; set; }

        [DataMember]
        public Nullable<byte> BarnesAgeYoung { get; set; }

        [DataMember]
        public Nullable<byte> BarnesAgeOld { get; set; }

        [DataMember]
        public Nullable<byte> CommonSensePause { get; set; }

        [DataMember]
        public Nullable<byte> CommonSenseOn { get; set; }

        [DataMember]
        public Nullable<bool> CommonSenseNoKids { get; set; }

        [DataMember]
        public Nullable<double> AverageContentAge { get; set; }

        [DataMember]
        public Nullable<double> BarnesAvg { get; set; }

        [DataMember]
        public Nullable<double> ScholasticGrade { get; set; }

        [DataMember]
        public string Dra { get; set; }

        [DataMember]
        public Nullable<short> LexScore { get; set; }

        [DataMember]
        public string GuidedReading { get; set; }

        [DataMember]
        public string LexCode { get; set; }

        [DataMember]
        public Nullable<System.DateTime> LexUpdate { get; set; }

        [DataMember]
        public Nullable<double> AverageSkillAge { get; set; }
    }
}
