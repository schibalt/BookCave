using System;
using System.Runtime.Serialization;

namespace BookCave.Service.Dto
{
    public class CommonSenseDto
    {
        [DataMember]
        public string Isbn13 { get; set; }

        [DataMember]
        public Nullable<byte> CommonSensePause { get; set; }

        [DataMember]
        public Nullable<byte> CommonSenseOn { get; set; }

        [DataMember]
        public Nullable<bool> CommonSenseNoKids { get; set; }
    }
}
