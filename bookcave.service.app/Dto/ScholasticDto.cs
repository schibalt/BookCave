using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BookCave.Service.Dto
{
    public class ScholasticDto
    {
        [DataMember]
        public long Isbn13 { get; set; }

        [DataMember]
        public string ScholasticGradeHigher { get; set; } //interest range by grade

        [DataMember]
        public sbyte ScholasticGradeLower { get; set; } //interest range by grade

        [DataMember]
        public double ScholasticGrade { get; set; } //scholastic level

        [DataMember]
        public string Dra { get; set; } // DRA dev reading assess level

        [DataMember]
        public string GuidedReading { get; set; } //guided reading level?
    }
}
