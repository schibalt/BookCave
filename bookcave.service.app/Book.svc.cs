using Apprenda.Services.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using System.Text.RegularExpressions;
using BookCave.Service.Dto;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace BookCave.Service
{
    public class BookService : IBook
    {
        // Set the log source
        private readonly ILogger log = LogManager.Instance().GetLogger(typeof(BookService));

        /// <summary>
        ///     Adds or updates a single book in the books database
        /// </summary>
        /// <param name="lexileDto">book to add</param>
        /// <returns></returns>
        public ResultDto PostLexileData(LexileDto lexileDto)
        {
            Mapper.CreateMap<LexileDto, SkillRecord>();
            using (var context = new BookcaveEntities())
            {
                var currentSkillRecord = context.SkillRecords.Find(lexileDto.Isbn13);
                //SkillRecord modifiedSkillRecord;

                if (currentSkillRecord != null)
                    currentSkillRecord = Mapper.Map<LexileDto, SkillRecord>(lexileDto, currentSkillRecord);
                else
                {
                    //context.SkillRecords.Add(new SkillRecord{LexUpdate=lexileDto.LexUpdate,LexScore=lexileDto.LexScore,LexCode=lexileDto.LexCode});
                    var newSkillRecord = Mapper.Map<LexileDto, SkillRecord>(lexileDto);
                    context.SkillRecords.Add(newSkillRecord);
                }

                Mapper.CreateMap<LexileDto, BookRecord>();
                var currentBookRecord = context.BookRecords.Find(lexileDto.Isbn13);

                if (currentBookRecord != null)
                    currentBookRecord = Mapper.Map<LexileDto, BookRecord>(lexileDto, currentBookRecord);
                else
                {
                    var newBookRecord = Mapper.Map<LexileDto, BookRecord>(lexileDto);
                    context.BookRecords.Add(newBookRecord);
                }

                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e) { Console.WriteLine(e.Message); }
                catch (DbUpdateException e) { Console.WriteLine(e.Message); }
            }
            return new ResultDto { ResultDescription = "success", ResultCode = 1 };
        }

        /// <summary>
        ///     Retrieves a single book from the book database
        /// </summary>
        /// <param name="isbn13">exact isbn 13</param>
        /// <returns>single book</returns>
        public LexileDto GetBook(string isbn13)
        {
            log.Debug("Enter " + MethodBase.GetCurrentMethod().Name);
            var result = new LexileDto();
            try
            {
                using (var context = new BookcaveEntities())
                {
                    // apparently this is sql injection proof since, under the covers, it converts it to a dbparam, slick way of doing parameterized queries
                    var bookRecords = context.Database.SqlQuery(typeof(BookRecord), "Select * from Books where Isbn13 = {0}", isbn13);

                    // should only be 1 result since isbn13 is primary key
                    // if there are no results then nothing will be set
                    foreach (BookRecord book in bookRecords)
                    {
                        Mapper.CreateMap<BookRecord, LexileDto>();
                        result = Mapper.Map<BookRecord, LexileDto>(book);
                    }

                    // Entity Framework is throwing exception converting SaasGrid connectionstring to SqlClient connectionstring
                    // Direct query as well as using Entity Framework to do an insertion works though
                    // Also, this works if we just publish this web service outside of Apprenda in IIS
                    // This should be submitted as a bug report to Apprenda

                    /*          var query = from book in context.Books
                                          where book.Isbn13 == isbn13
                                          select book;

                              Book bookResult = query.First();
                              result = new BookDto(bookResult);*/
                }
            }
            catch (Exception e)
            {
                log.Error("Exception in GetBook.", e);
            }
            log.Debug("Exit " + MethodBase.GetCurrentMethod().Name);
            return result;
        }

        /// <summary>
        ///     Returns a group of books from the book database based on a set of optional
        ///     query parameters.  The lexile and age can be a single value or a range
        ///     (IE. /GetBooks?lexile=10:20 or /GetBooks?lexile=15).
        /// </summary>
        /// <param name="lexile">lexile range</param>
        /// <param name="age">age range</param>
        /// <param name="title">title query</param>
        /// <param name="author">title query</param>
        /// <returns>list of books</returns>
        public List<LexileDto> GetBooks(string age, string title, string lexile, string author, string summary)
        {
            log.Debug("Enter " + MethodBase.GetCurrentMethod().Name);
            // all of the incoming strings will be html encoded, so they must be decoded
            // example:
            // lexile = System.Net.WebUtility.HtmlDecode(lexile);

            var re1 = "(\\d+)";	// Integer Number 1
            var re2 = "(:)";	// Any Single Character 1
            var re3 = "(\\d+)";	// Integer Number 2

            var r = new Regex(re1 + re2 + re3, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m;
            IEnumerable<BookRecord> booksByLexile = new List<BookRecord>();

            using (var context = new BookcaveEntities())
            {
                if (lexile != null)
                {
                    log.Debug("lexile: " + lexile);
                    // lexile should be in one of 2 formats:
                    // min:max (10:20)
                    // score (30)
                    m = r.Match(lexile);
                    if (m.Success)
                    {
                        var int1 = m.Groups[1].ToString();
                        var int2 = m.Groups[3].ToString();

                        //swap the range values if necessary
                        var lower = Convert.ToInt32(int1);
                        var higher = Convert.ToInt32(int2);

                        if (lower > higher) //if the values are in the wrong order
                        {
                            var temp = higher;//save what's actually hte lower value
                            higher = lower;//overwrite it with what's actually the higher value
                            lower = temp;//assign the correct,lower value
                        }

                        booksByLexile = context.Database.SqlQuery<BookRecord>("Select * from Books where LexScore>={0} and LexScore<={1}", lower, higher);
                        Console.Write(int1.ToString() + ":" + int2.ToString() + "\n");
                    }
                    else
                        booksByLexile = context.Database.SqlQuery<BookRecord>("Select * from Books where LexScore={0}", lexile);
                }

                IEnumerable<BookRecord> booksByAge = new List<BookRecord>();

                if (age != null)
                {
                    log.Debug("age: " + age);
                    // age should be in one of 2 formats:
                    // min:max (15:16)
                    // age (12)
                    m = r.Match(age);
                    if (m.Success)
                    {
                        var int1 = m.Groups[1].ToString();
                        var int2 = m.Groups[3].ToString();

                        booksByAge = context.Database.SqlQuery<BookRecord>("Select * from Books where AgeRating>={0} and AgeRating<={1}", int1, int2);
                        Console.Write(int1.ToString() + ":" + int2.ToString() + "\n");
                    }
                    else
                        booksByAge = context.Database.SqlQuery<BookRecord>("Select * from Books where AgeRating={0}", age);
                }

                IEnumerable<BookRecord> booksByTitle = new List<BookRecord>();

                if (title != null)
                {
                    var annotatedTitle = '%' + title + '%';

                    booksByTitle = context.Database.SqlQuery<BookRecord>("Select * from Books where title like {0}", annotatedTitle);
                    log.Debug("title: " + title);
                }

                IEnumerable<BookRecord> booksByAuthor = new List<BookRecord>();

                if (author != null)
                {
                    var annotatedAuthor = '%' + author + '%';
                    booksByAuthor = context.Database.SqlQuery<BookRecord>("Select * from Books where author like {0}", annotatedAuthor);
                    log.Debug("author: " + author);
                }

                IEnumerable<BookRecord> booksBySummary = new List<BookRecord>();

                if (author != null)
                {
                    var annotatedSummary = '%' + summary + '%';
                    booksBySummary = context.Database.SqlQuery<BookRecord>("Select * from Books where summary like {0}", annotatedSummary);
                    log.Debug("summary: " + summary);
                }

                // combine query parameters and execute query against the database
                // return the result
                log.Debug("Exit " + MethodBase.GetCurrentMethod().Name);

                /*
                 * this method has to return the intersection of the various result sets.
                 * 1. they're added to a list
                 * 2. they're iterated through
                 * 
                 *      a. if there's something in that set the contents have to be 
                 *  incorporated into the overall set
                 *  
                 *          i. if there are already elements in the master set 
                 *          then intersect them with those matching the parameter (author, lexscore,...)
                 *          
                 *          ii. if there's nothing in the master set yet then union that empty set with 
                 *          whatever's in the parameter-specific set
                 * */
                var resultSetsPerCriterion = new List<IEnumerable<BookRecord>>();
                resultSetsPerCriterion.Add(booksByAge);
                resultSetsPerCriterion.Add(booksByAuthor);
                resultSetsPerCriterion.Add(booksByLexile);
                resultSetsPerCriterion.Add(booksByTitle);
                resultSetsPerCriterion.Add(booksBySummary);

                IEnumerable<BookRecord> dalResultSet = new List<BookRecord>();

                foreach (var set in resultSetsPerCriterion)
                    if (set.Count() > 0)
                    {
                        if (dalResultSet.Count() > 0)
                            dalResultSet = from a in dalResultSet
                                           join b in set on a.Isbn13 equals b.Isbn13
                                           select (BookRecord)a;
                        else
                            dalResultSet = dalResultSet.Union(set);
                    }

                var bllResultSet = new List<LexileDto>();

                Mapper.CreateMap<BookRecord, LexileDto>();

                foreach (BookRecord book in dalResultSet)
                    bllResultSet.Add(Mapper.Map<BookRecord, LexileDto>(book));

                return bllResultSet;
            }
        }

        /// <summary>
        ///     Adds or updates several books in the books database
        /// </summary>
        /// <param name="books">books to add</param>
        /// <returns></returns>
        public ResultDto PostBatchLexileData(List<LexileDto> books)
        {
            // TODO: implement basic authentication (don't worry about this until later)
            // add or update books in the books database
            return new ResultDto { ResultCode = 0, ResultDescription = "Success" };
        }

        public ResultDto PostBarnesData(BarnesDto barnesDto)
        {
            using (var context = new BookcaveEntities())
            {
                var currentRatingRecord = context.RatingRecords.Find(barnesDto.Isbn13);
                //Mapper.CreateMap<BarnesDto, RatingRecord>();

                if (currentRatingRecord != null)
                    currentRatingRecord = Mapper.Map<BarnesDto, RatingRecord>(barnesDto, currentRatingRecord);
                else
                {
                    var newRatingRecord = Mapper.Map<BarnesDto, RatingRecord>(barnesDto);
                    context.RatingRecords.Add(newRatingRecord);
                }

                var currentContentRecord = context.ContentRecords.Find(barnesDto.Isbn13);
                //Mapper.CreateMap<BarnesDto,ContentRecord>();

                if (currentContentRecord != null)
                    currentContentRecord = Mapper.Map(barnesDto, currentContentRecord);
                else
                {
                    var newContentRecord = Mapper.Map<BarnesDto, ContentRecord>(barnesDto);
                    context.ContentRecords.Add(newContentRecord);
                }

                context.SaveChanges();
                try
                {
                }
                catch (Exception e) { return new ResultDto { ResultCode = 0, ResultDescription = e.Message }; }
            }

            return new ResultDto { ResultCode = 1, ResultDescription = "Updated" };
        }

        public ResultDto PostScholasticData(ScholasticDto scholasticDto)
        {
            using (var context = new BookcaveEntities())
            {
                var currentSkillData = context.SkillRecords.Find(scholasticDto.Isbn13);

                if (currentSkillData != null)
                    currentSkillData = Mapper.Map<ScholasticDto, SkillRecord>(scholasticDto, currentSkillData);
                else
                {
                    var newSkillRecord = Mapper.Map<ScholasticDto, SkillRecord>(scholasticDto);
                    context.SkillRecords.Add(newSkillRecord);
                }

                var currentContentData = context.ContentRecords.Find(scholasticDto.Isbn13);

                if (currentContentData != null)
                    currentContentData = Mapper.Map<ScholasticDto, ContentRecord>(scholasticDto, currentContentData);
                else
                {
                    var newContentRecord = Mapper.Map<ScholasticDto, ContentRecord>(scholasticDto);
                    context.ContentRecords.Add(newContentRecord);
                }
                context.SaveChanges();
            }
            return new ResultDto { ResultDescription = "Updated", ResultCode = 1 };
        }
    }
}