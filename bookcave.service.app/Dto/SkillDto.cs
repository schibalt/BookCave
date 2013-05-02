using System;
using System.Runtime.Serialization;

namespace BookCave.Service.Dto
{
    public class SkillDto
    {
        [DataMember]
        public Nullable<double> ScholasticGrade { get; set; }

        [DataMember]
        public string Dra { get; set; }

        [DataMember]
        public Nullable<short> LexScore { get; set; }

        [DataMember]
        public string GuidedReading { get; set; }

        [DataMember]
        public long Isbn13 { get; set; }

        [DataMember]
        public string LexCode { get; set; }

        [DataMember]
        public System.DateTime LexUpdate { get; set; }

        [DataMember]
        public double AggregateSkill { get; set; }
    }
}
