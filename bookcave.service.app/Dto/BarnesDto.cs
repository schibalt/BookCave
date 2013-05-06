using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BookCave.Service.Dto
{
    public class BarnesDto
    {
        [DataMember]
        public string Isbn13 { get; set; }

        [DataMember]
        public Nullable<byte> BarnesAgeYoung { get; set; }  // interest range by age

        [DataMember]
        public Nullable<byte> BarnesAgeOld { get; set; } // interest range by age

        [DataMember]
        public double? BarnesAvg { get; set; } //user review by percent (.9 = 90%)
    }
}
