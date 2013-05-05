using System.Runtime.Serialization;

namespace BookCave.Service.Dto
{
    [DataContract]
    public class ResultDto
    {
        [DataMember]
        public int ResultCode { get; set; }

        [DataMember]
        public string ResultDescription { get; set; }
    }
}