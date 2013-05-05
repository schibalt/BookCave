using Apprenda.Services.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using System.Text.RegularExpressions;
using BookCave.Service.Dto;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using BookCave.Service.Entities;
using System.Text;

namespace BookCave.Service
{
    public class BookService : IBook
    {
        // Set the log source
        private readonly ILogger log = LogManager.Instance().GetLogger(typeof(BookService));

        private bool LexileRecordVerified(LexileDto lexileDto)
        {
            var isbn13Regex = @"(\d{13})"; //needs to be a 13 digit character
            var r = new Regex(isbn13Regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var m = r.Match(lexileDto.Isbn13.ToString());

            if (m.Success /*&& lexileDto.Isbn.Length == 10*/)
            {
                if (lexileDto.Isbn.Length != 10)
                    lexileDto.Isbn = null;

                var notAllowed = new Hashtable();
                notAllowed["DocType"] = new List<object> { "None" };
                notAllowed["Series"] = new List<object> { "None" };
                notAllowed["Awards"] = new List<object> { "" };
                notAllowed["Author"] = new List<object> { "" };
                notAllowed["LexCode"] = new List<object> { "" };
                notAllowed["Summary"] = new List<object> { "" };
                notAllowed["PageCount"] = new List<object> { 0 };
                notAllowed["Isbn"] = new List<object> { "None" };
                var dtoMembers = lexileDto.GetType().GetMembers();

                foreach (var member in dtoMembers)
                {
                    var memberName = member.Name;
                    var property = lexileDto.GetType().GetProperty(memberName);

                    if (property == null)
                        continue;
                    var propertyValue = property.GetValue(lexileDto);
                    var badValueList = notAllowed[memberName];

                    if (badValueList == null)
                        continue;
                    if (((List<object>)badValueList).Contains(propertyValue))
                        property.SetValue(lexileDto, null);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Adds or updates a single book in the books database
        /// </summary>
        /// <param name="lexileDto">book to add</param>
        /// <returns></returns>
        public ResultDto PostLexileData(LexileDto lexileDto)
        {
            if (!LexileRecordVerified(lexileDto))
                return new ResultDto { ResultDescription = "bad object" };

            Mapper.CreateMap<LexileDto, SkillRecord>();
            using (var context = new BookcaveEntities())
            {
                //var recordQuery = context.SkillRecords.Where(book => book.Isbn13.Equals(lexileDto.Isbn13));
                var recordQuery = context.Database.SqlQuery<SkillRecord>("Select * from SkillRecords where Isbn13 = {0}", lexileDto.Isbn13);
                //var currentSkillRecord = skillRecords.FirstOrDefault();
                var skillRecords = new List<SkillRecord>(recordQuery);
                SkillRecord currentSkillRecord = null;

                if (skillRecords.Count > 0)
                    currentSkillRecord = skillRecords.First();

                if (currentSkillRecord != null)
                {
                    var modifiedSkillRecord = Mapper.Map<LexileDto, SkillRecord>(lexileDto, currentSkillRecord);
                    var aggregateSkill = DataFunctions.GetAverageSkillAge(modifiedSkillRecord);
                    context.Database.ExecuteSqlCommand
                    (
                        "update dbo.SkillRecords set LexScore={0},LexCode={1},LexUpdate={2},AverageSkillAge={3} where Isbn13={4}",
                        lexileDto.LexScore,
                        lexileDto.LexCode,
                        lexileDto.LexUpdate,
                        aggregateSkill,
                        lexileDto.Isbn13
                    );
                }
                else
                {
                    var newSkillRecord = Mapper.Map<LexileDto, SkillRecord>(lexileDto);
                    newSkillRecord.AverageSkillAge = DataFunctions.GetAverageSkillAge(newSkillRecord);
                    context.SkillRecords.Add(newSkillRecord);
                }

                Mapper.CreateMap<LexileDto, BookRecord>();
                var bookQuery = context.Database.SqlQuery<BookRecord>("Select * from BookRecords where Isbn13 = {0}", lexileDto.Isbn13);
                //var bookQuery = context.BookRecords.Where(book => book.Isbn13.Equals(lexileDto.Isbn13));
                var bookRecords = new List<BookRecord>(bookQuery);
                BookRecord currentBookRecord = null;

                if (bookRecords.Count > 0)
                    currentBookRecord = bookRecords[0];
                //var currentBookRecord = bookRecords.FirstOrDefault();

                if (currentBookRecord != null)
                {
                    //currentBookRecord = Mapper.Map<LexileDto, BookRecord>(lexileDto, currentBookRecord);
                    context.Database.ExecuteSqlCommand
                    (
                        "update dbo.BookRecords set Title={0},Author={1},Publisher={2},PageCount={3},DocType={4},Series={5},Awards={6},Summary={7}",
                        lexileDto.Title,
                        lexileDto.Author,
                        lexileDto.Publisher,
                        lexileDto.PageCount,
                        lexileDto.DocType,
                        lexileDto.Series,
                        lexileDto.Awards,
                        lexileDto.Summary
                    );
                }
                else
                {
                    var newBookRecord = Mapper.Map<LexileDto, BookRecord>(lexileDto);
                    context.BookRecords.Add(newBookRecord);
                }

                context.SaveChanges();
                //try
                //{
                //}
                //catch (DbEntityValidationException e) { Console.WriteLine(e.Message); }
                //catch (DbUpdateException e) { Console.WriteLine(e.Message); }
            }
            return new ResultDto { ResultDescription = "success" };
        }

        /// <summary>
        ///     Retrieves a single book from the book database
        /// </summary>
        /// <param name="isbn13">exact isbn 13</param>
        /// <returns>single book</returns>
        public SkillDto GetSkillMetrics(string isbn13)
        {
            using (var context = new BookcaveEntities())
            {
                // apparently this is sql injection proof since, under the covers, it converts it to a dbparam, slick way of doing parameterized queries
                var skillRecords = context.Database.SqlQuery<SkillRecord>("Select * from SkillRecords where Isbn13 = {0}", isbn13);

                // Entity Framework is throwing exception converting SaasGrid connectionstring to SqlClient connectionstring
                // Direct query as well as using Entity Framework to do an insertion works though
                // Also, this works if we just publish this web service outside of Apprenda in IIS
                // This should be submitted as a bug report to Apprenda

                //var query = from book in context.SkillRecords
                //            where book.Isbn13 == isbn13
                //            select book;

                //var skillRecord = context.SkillRecords.First(book => book.Isbn13.Equals(isbn13));

                var skillRecord = skillRecords.FirstOrDefault();

                if (skillRecord == null)
                    return new SkillDto();
                var scholasticGrade = skillRecord.ScholasticGrade;
                var dra = skillRecord.Dra;
                var lexscore = skillRecord.LexScore;
                var guidedReading = skillRecord.GuidedReading;

                Mapper.CreateMap<SkillRecord, SkillDto>();
                var skillDto = Mapper.Map<SkillRecord, SkillDto>(skillRecord);
                var aggregateSkill = DataFunctions.GetAverageSkillAge(skillRecord);
                skillDto.AggregateSkill = aggregateSkill;
                return skillDto;
            }
        }

        /// <summary>
        ///     Retrieves a single book from the book database
        /// </summary>
        /// <param name="isbn13">exact isbn 13</param>
        /// <returns>single book</returns>
        public ContentDto GetContentMetrics(string isbn13)
        {
            using (var context = new BookcaveEntities())
            {
                var contentRecords = context.Database.SqlQuery<ContentRecord>("Select * from ContentRecords where Isbn13 = {0}", isbn13);
                var contentRecord = contentRecords.FirstOrDefault();

                if (contentRecord == null)
                    return new ContentDto();

                var scholasticGradeHigher = contentRecord.ScholasticGradeHigher;
                var scholasticGradeLower = contentRecord.ScholasticGradeLower;
                var barnesAgeYoung = contentRecord.BarnesAgeYoung;
                var barnesAgeOld = contentRecord.BarnesAgeOld;
                var commonSenseNoKids = contentRecord.CommonSenseNoKids;
                var commonSensePause = contentRecord.CommonSensePause;
                var commonSenseOn = contentRecord.CommonSenseOn;

                //make DTO
                Mapper.CreateMap<ContentRecord, ContentDto>();
                var contentDto = Mapper.Map<ContentRecord, ContentDto>(contentRecord);

                //get aggregated content score, assign to DTO property, then return
                var aggregateContent = DataFunctions.GetContentAverageAge(contentRecord);
                contentDto.AggregateContent = aggregateContent;
                return contentDto;
            }
        }

        /// <summary>
        ///     Returns a group of books from the book database based on a set of optional
        ///     query parameters.  The lexile and age can be a single value or a range
        ///     (IE. /GetBooks?skill=10:20 or /GetBooks?skill=15).
        /// </summary>
        /// <param name="skill">skill range or target</param>
        /// <param name="content">content range or target</param>
        /// <param name="title">desired title</param>
        /// <param name="author">desired author</param>
        /// <returns>list of books</returns>
        public List<SuperDto> GetBooks(string content, string title, string skill, string author, string summary)
        {
            log.Debug("Enter " + MethodBase.GetCurrentMethod().Name);
            // all of the incoming strings will be html encoded, so they must be decoded
            // example:
            // skill = System.Net.WebUtility.HtmlDecode(skill);

            var re1 = "(\\d+)";	// Integer Number 1
            var re2 = "(:)";	// Any Single Character 1
            var re3 = "(\\d+)";	// Integer Number 2

            var r = new Regex(re1 + re2 + re3, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m;

            using (var context = new BookcaveEntities())
            {
                var query = new StringBuilder();
                query.Append("select * from BookRecords");
                query.Append(" full outer join SkillRecords on BookRecords.Isbn13=SkillRecords.Isbn13");
                query.Append(" full outer join ContentRecords on BookRecords.Isbn13=ContentRecords.Isbn13");
                query.Append(" full outer join RatingRecords on BookRecords.Isbn13=RatingRecords.Isbn13");

                const double window = 1;

               var booksBySkill = new List<SuperRecord>();

               var queryBySkill = new StringBuilder(query.ToString());
                queryBySkill.Append(" where SkillRecords.AverageSkillAge>={0} and SkillRecords.AverageSkillAge<{1}");
                if (skill != null)
                {
                    log.Debug("skill: " + skill);
                    // lexile should be in one of 2 formats:
                    // min:max (10:20)
                    // score (30)
                    m = r.Match(skill);
                    if (m.Success)
                    {
                        var int1 = m.Groups[1].ToString();
                        var int2 = m.Groups[3].ToString();

                        //swap the range values if necessary
                        var lower = Convert.ToDouble(int1);
                        var higher = Convert.ToDouble(int2);

                        if (lower > higher) //if the values are in the wrong order
                        {
                            var temp = higher;//save what's actually hte lower value
                            higher = lower;//overwrite it with what's actually the higher value
                            lower = temp;//assign the correct,lower value
                        }
                        booksBySkill = context.Database.SqlQuery<SuperRecord>(queryBySkill.ToString(), lower, higher).ToList();
                        //booksBySkill = context.SkillRecords.SqlQuery("Select * from SkillRecords where AverageSkillAge>={0} and AverageSkillAge<={1}", lower, higher).ToList();
                        Console.Write(int1.ToString() + ":" + int2.ToString() + "\n");
                    }
                    else
                    {
                        var nvSkill = Convert.ToDouble(skill);
                        booksBySkill = context.Database.SqlQuery<SuperRecord>(queryBySkill.ToString(), nvSkill , nvSkill + window).ToList();
                    }
                }

                var booksByContent= new List<SuperRecord>();
                var queryByContent = new StringBuilder(query.ToString()); ;
                queryByContent.Append(" where ContentRecords.AverageContentAge>={0} and ContentRecords.AverageContentAge<{1}");
                if (content != null)
                {
                    var nvContent = Convert.ToDouble(content);
                    log.Debug("age: " + content);
                    // age should be in one of 2 formats:
                    // min:max (15:16)
                    // age (12)
                    m = r.Match(content);
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
                        booksByContent = context.Database.SqlQuery<SuperRecord>(queryByContent.ToString(), lower, higher).ToList();
                        Console.Write(int1.ToString() + ":" + int2.ToString() + "\n");
                    }
                    else
                    {
                        booksByContent = context.Database.SqlQuery<SuperRecord>(queryByContent.ToString(), nvContent , nvContent + window).ToList();
                    }
                }

                var booksByTitle= new List<SuperRecord>();
                var queryByTitle = new StringBuilder(query.ToString()); ;
                queryByTitle.Append(" where title like {0}");

                if (title != null)
                {
                    var annotatedTitle = '%' + title + '%';

                    booksByTitle = context.Database.SqlQuery<SuperRecord>(queryByTitle.ToString(), annotatedTitle).ToList();
                    log.Debug("title: " + title);
                }

               var booksByAuthor= new List<SuperRecord>();
               var queryByAuthor = new StringBuilder(query.ToString()); ;
                queryByAuthor.Append(" where author like {0}");

                if (author != null)
                {
                    var annotatedAuthor = '%' + author + '%';
                    booksByAuthor = context.Database.SqlQuery<SuperRecord>(queryByAuthor.ToString(), annotatedAuthor).ToList();
                    log.Debug("author: " + author);
                }

                var booksBySummary= new List<SuperRecord>();
                var queryBySummary = new StringBuilder(query.ToString()); ;
                queryBySummary.Append(" where summary like {0}");

                if (author != null)
                {
                    var annotatedSummary = '%' + summary + '%';
                    booksBySummary = context.Database.SqlQuery<SuperRecord>(queryBySummary.ToString(), annotatedSummary).ToList();
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

                var resultSetsPerCriterion = new List<List<SuperRecord>>();
                resultSetsPerCriterion.Add(booksByContent);
                resultSetsPerCriterion.Add(booksByAuthor);
                resultSetsPerCriterion.Add(booksBySkill);
                resultSetsPerCriterion.Add(booksByTitle);
                resultSetsPerCriterion.Add(booksBySummary);

                IEnumerable<SuperRecord> dalResultSet = new List<SuperRecord>();

                foreach (var set in resultSetsPerCriterion)
                    if (set.Count() > 0)
                    {
                        if (dalResultSet.Count() > 0)

                            dalResultSet = from result in dalResultSet
                                           join record in set on result.Isbn13 equals record.Isbn13
                                           select (SuperRecord)result;
                        else
                            dalResultSet = dalResultSet.Union(set) ;
                    }

                var results = new List<SuperDto>();
                Mapper.CreateMap<SuperRecord, SuperDto>();

                foreach (var record in dalResultSet)
                    results.Add(Mapper.Map<SuperRecord, SuperDto>(record));
                return results;

                //foreach (BookRecord book in dalResultSet)
                //    bllResultSet.Add(Mapper.Map<BookRecord, LexileDto>(book));

                //return bllResultSet;
            }//using
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
            return new ResultDto { ResultDescription = "Success" };
        }

        public ResultDto PostBarnesData(BarnesDto barnesDto)
        {
            using (var context = new BookcaveEntities())
            {
                var ratingRecords = context.Database.SqlQuery<RatingRecord>("Select * from RatingRecords where Isbn13 = {0}", barnesDto.Isbn13);
                var currentRatingRecord = ratingRecords.FirstOrDefault();
                Mapper.CreateMap<BarnesDto, RatingRecord>();

                if (currentRatingRecord != null)
                    currentRatingRecord = Mapper.Map<BarnesDto, RatingRecord>(barnesDto, currentRatingRecord);
                else
                {
                    var newRatingRecord = Mapper.Map<BarnesDto, RatingRecord>(barnesDto);
                    context.RatingRecords.Add(newRatingRecord);
                }

                var contentRecords = context.Database.SqlQuery<ContentRecord>("Select * from ContentRecords where Isbn13 = {0}", barnesDto.Isbn13);
                var currentContentRecord = contentRecords.FirstOrDefault();
                Mapper.CreateMap<BarnesDto, ContentRecord>();

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
                catch (DbUpdateException e)
                {
                    Console.WriteLine("book metadata wasn't found in primary table.");
                    return new ResultDto { ResultDescription = e.Message };
                }
            }

            return new ResultDto { ResultDescription = "Updated" };
        }

        private ResultDto VerifyScholasticDto(ScholasticDto dto)
        {
            if (dto.GuidedReading.Length != 1 || !Char.IsLetter(Convert.ToChar(dto.GuidedReading)))
                return new ResultDto { ResultDescription = "incorrect format for guided reading level property", ResultCode = -2 };

            var re1 = "(\\d+)";	// lower dra bound
            var re2 = "(-)";	// hyphen
            var re3 = "(\\d+)";	// upper dra bound

            var regexScoreRange = new Regex(re1 + re2 + re3, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matchScoreRange = regexScoreRange.Match(dto.Dra);
            var regexScore = new Regex("(\\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matchScore = regexScore.Match(dto.Dra);

            if (!matchScoreRange.Success && !matchScore.Success) //if the parameter is a range 
                return new ResultDto { ResultDescription = "incorrect dra format ", ResultCode = -1 };

            return new ResultDto { ResultDescription = "success" };
        }

        public ResultDto PostScholasticData(ScholasticDto scholasticDto)
        {
            scholasticDto.GuidedReading = scholasticDto.GuidedReading.Trim();
            scholasticDto.Dra = scholasticDto.Dra.Trim();

            var verificationResult = VerifyScholasticDto(scholasticDto);

            if (verificationResult.ResultCode < 0)
                return verificationResult;

            using (var context = new BookcaveEntities())
            {
                var skillRecords = context.Database.SqlQuery<SkillRecord>("Select * from SkillRecords where Isbn13 = {0}", scholasticDto.Isbn13);
                var currentSkillRecord = skillRecords.FirstOrDefault();
                Mapper.CreateMap<ScholasticDto, SkillRecord>();

                if (currentSkillRecord != null)
                    currentSkillRecord = Mapper.Map<ScholasticDto, SkillRecord>(scholasticDto, currentSkillRecord);
                else
                {
                    var newSkillRecord = Mapper.Map<ScholasticDto, SkillRecord>(scholasticDto);

                    context.SkillRecords.Add(newSkillRecord);
                }

                var contentRecords = context.Database.SqlQuery<ContentRecord>("Select * from ContentRecords where Isbn13 = {0}", scholasticDto.Isbn13);
                var currentContentRecord = contentRecords.FirstOrDefault();
                Mapper.CreateMap<ScholasticDto, ContentRecord>();

                if (currentContentRecord != null)
                    currentContentRecord = Mapper.Map<ScholasticDto, ContentRecord>(scholasticDto, currentContentRecord);
                else
                {
                    var newContentRecord = Mapper.Map<ScholasticDto, ContentRecord>(scholasticDto);

                    context.ContentRecords.Add(newContentRecord);
                }

                context.SaveChanges();
                //try
                //{
                //}
                //catch (DbEntityValidationException e) { Console.WriteLine(e.Message); }
                //catch (DbUpdateException e) { Console.WriteLine(e.Message); }
            }
            return new ResultDto { ResultDescription = "Updated" };
        }

        public ResultDto PostCommonSenseData(CommonSenseDto commonSenseDto)
        {
            using (var context = new BookcaveEntities())
            {
                var contentRecords = context.Database.SqlQuery<ContentRecord>("Select * from ContentRecords where Isbn13 = {0}", commonSenseDto.Isbn13);
                var currentContentRecord = contentRecords.FirstOrDefault();
                Mapper.CreateMap<CommonSenseDto, ContentRecord>();

                if (currentContentRecord != null)
                {
                    var modifiedContentRecord = Mapper.Map<CommonSenseDto, ContentRecord>(commonSenseDto, currentContentRecord);
                    var aggregateContent = DataFunctions.GetContentAverageAge(modifiedContentRecord);
                    context.Database.ExecuteSqlCommand
                    (
                        "update dbo.ContentRecords set CommonSensePause={0},CommonSenseOn={1},CommonSenseNoKids={2},AverageContentAge={3} where Isbn13={4}",
                        commonSenseDto.CommonSensePause,
                        commonSenseDto.CommonSenseOn,
                        commonSenseDto.CommonSenseNoKids,
                        aggregateContent,
                        commonSenseDto.Isbn13
                    );
                }
                else
                {
                    var newContentRecord = Mapper.Map<CommonSenseDto, ContentRecord>(commonSenseDto);
                    newContentRecord.AverageContentAge = DataFunctions.GetContentAverageAge(newContentRecord);
                    context.ContentRecords.Add(newContentRecord);
                }

                context.SaveChanges();
                return new ResultDto { ResultDescription = "success" };
            }
        }
    }
}