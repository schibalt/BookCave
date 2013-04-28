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
        public Int64 Isbn13 { get; set; }

        [DataMember]
        public byte BarnesAgeYoung { get; set; }  // interest range by age

        [DataMember]
        public byte BarnesAgeOld { get; set; } // interest range by age

        [DataMember]
        public double? BarnesAvg { get; set; } //user review by percent (.9 = 90%)
    }
}
