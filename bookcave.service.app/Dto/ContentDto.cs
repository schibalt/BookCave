using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BookCave.Service.Dto
{
    public class ContentDto
    {
        [DataMember]
        public string Isbn13 { get; set; }

        [DataMember]
        public Nullable<byte> ScholasticGradeHigher { get; set; }

        [DataMember]
        public string ScholasticGradeLower { get; set; }

        [DataMember]
        public Nullable<byte> BarnesAgeYoung { get; set; }

        [DataMember]
        public Nullable<byte> BarnesAgeOld { get; set; }

        [DataMember]
        public double AggregateContent { get; set; }
    }
}
