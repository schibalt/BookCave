//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookCave.Service
{
    using System;
    using System.Collections.Generic;
    
    public partial class ContentRecord
    {
        public long Isbn13 { get; set; }
        public string ScholasticGradeHigher { get; set; }
        public short ScholasticGradeLower { get; set; }
        public byte BarnesAgeYoung { get; set; }
        public byte BarnesAgeOld { get; set; }
    
        public virtual BookRecord Books { get; set; }
    }
}
