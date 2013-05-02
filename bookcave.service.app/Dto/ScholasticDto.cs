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
        public string Isbn13 { get; set; }

        [DataMember]
        public string ScholasticGradeLower { get; set; } //interest range by grade

        [DataMember]
        public Nullable<byte> ScholasticGradeHigher { get; set; } //interest range by grade

        [DataMember]
        public Nullable<double> ScholasticGrade { get; set; } //scholastic level

        [DataMember]
        public string Dra { get; set; } // DRA dev reading assess level

        [DataMember]
        public string GuidedReading { get; set; } //guided reading level?
    }
}
