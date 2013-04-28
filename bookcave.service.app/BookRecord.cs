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
    
    public partial class BookRecord
    {
        public long Isbn13 { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Isbn { get; set; }
        public string Publisher { get; set; }
        public Nullable<short> PageCount { get; set; }
        public string DocType { get; set; }
        public string Series { get; set; }
        public string Summary { get; set; }
        public string Awards { get; set; }
    
        public virtual RatingRecord Rating { get; set; }
        public virtual SkillRecord Skill { get; set; }
        public virtual ContentRecord Content { get; set; }
    }
}