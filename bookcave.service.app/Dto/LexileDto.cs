using System;
using System.Runtime.Serialization;

namespace BookCave.Service.Dto
{
    public class LexileDto
    {
        [DataMember]
        public string Title { set; get; }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public string Isbn { get; set; }

        [DataMember]
        public string Isbn13 { get; set; }

        [DataMember]
        public string LexCode { get; set; }

        [DataMember]
        public Nullable<short> LexScore { get; set; }

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
        public DateTime LexUpdate { get; set; }
    }
}
