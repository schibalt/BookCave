using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using BookCave.Service.Dto;
using BookCave.Service;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

namespace BookcaveUnitTests
{
    [TestClass]
    public class BookcaveUnitTests
    {
        [TestMethod]
        public void TestPostBarnesDataInMemory()
        {
            // Create an instance of StreamReader to read from a file. 
            // The using statement also closes the StreamReader. 
            using (var sr = new StreamReader(@"C:\Users\tiliska\Documents\bn-sample.csv"))
            {
                string svLine;
                // Read and display lines from the file until the end of  
                // the file is reached. 
                while ((svLine = sr.ReadLine()) != null)
                {
                    var barnesNobleData = svLine.Split(',');
                    var barnesDto = new BarnesDto();

                    var isbn13 = Convert.ToInt64(barnesNobleData[0]);
                    barnesDto.Isbn13 = isbn13;

                    try
                    {
                        var ageLow = Convert.ToByte(barnesNobleData[1]);
                        barnesDto.BarnesAgeYoung = ageLow;

                        var ageHigh = Convert.ToByte(barnesNobleData[2]);
                        barnesDto.BarnesAgeOld = ageHigh;

                        var rating = Convert.ToDouble(barnesNobleData[3]);
                        barnesDto.BarnesAvg = rating;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("data in csv aren't valid");
                    }

                    var service = new BookService();
                    service.PostBarnesData(barnesDto);
                }
            }
        }

        [TestMethod]
        public void TestPostLexileDataInMemory()
        {
            ParseLexileDB("testinstance");
        }

        [TestMethod]
        public void TestPostLexileDataOverWire()
        {
            ParseLexileDB("post");
        }

        private void ParseLexileDB(string method)
        {
            // Create an instance of StreamReader to read from a file. 
            // The using statement also closes the StreamReader. 
            //using (var sr = new StreamReader(@"C:\Users\tiliska\Documents\problematiclexile.txt"))
            using (var sr = new StreamReader(@"C:\Users\tiliska\Documents\lexiletitles.txt"))
            {
                sr.ReadLine();//eat first line
                string svLine;
                var line = 1;

                // Read and display lines from the file until the end of  
                // the file is reached. 
                while ((svLine = sr.ReadLine()) != null)
                {
                    if (svLine.StartsWith("#"))
                    {
                        Console.WriteLine("still a # on line " + line);
                        continue;
                    }

                    line++;
                    var bookParams = svLine.Split('\t');
                    var lexileDto = new LexileDto();

                    lexileDto.Title = bookParams[0];
                    lexileDto.Author = bookParams[1];

                    try
                    {
                        lexileDto.Isbn = bookParams[2];

                        try
                        {
                            lexileDto.Isbn13 = bookParams[3];

                            if (bookParams[5].Length > 0)
                                lexileDto.LexScore = Convert.ToInt16(bookParams[5]);

                            if (bookParams[7].Length > 0)
                                lexileDto.PageCount = Convert.ToInt16(bookParams[7]);

                            lexileDto.LexUpdate = Convert.ToDateTime(bookParams[12]);
                        }
                        catch (FormatException) { Console.WriteLine("metametrics messed up the row in the text db on line " + line); }

                        lexileDto.LexCode = bookParams[4];
                        lexileDto.Publisher = bookParams[6];
                        lexileDto.DocType = bookParams[8];
                        lexileDto.Series = bookParams[9];
                        lexileDto.Awards = bookParams[10];
                        lexileDto.Summary = bookParams[11];

                        if (method.Equals("post"))
                        {
                            var uri = new Uri(@"http://apps.apprenda.local/api/services/json/r/bookcavetest(v1)/BookService/IBook/books");
                            //var uri = new Uri(@"http://apps.my.apprendacloud.com/api/services/json/r/bookcavetest(v1)/BookService/IBook/books");
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

                            var webClient = new WebClient();
                            webClient.Headers["Content-type"] = "application/json";

                            var memoryStream = new MemoryStream();
                            var serializedJson = new DataContractJsonSerializer(typeof(LexileDto));

                            serializedJson.WriteObject(memoryStream, lexileDto);

                            byte[] res1 = webClient.UploadData(uri.ToString(), "POST", memoryStream.ToArray());
                        }
                        else if (method.Equals("testinstance"))
                        {
                            var service = new BookService();
                            service.PostLexileData(lexileDto);
                        }
                        else if (method.Equals("skillmetrics"))
                        {
                            var service = new BookService();
                            var skillAggregate = service.GetSkillMetrics(lexileDto.Isbn13);
                        }
                    }
                    catch (IndexOutOfRangeException) { Console.WriteLine("metametrics messed up the row in the text db on line " + line); }
                }
            }
        }

        private void ParseScholasticData(string test)
        {
            // Create an instance of StreamReader to read from a file. 
            // The using statement also closes the StreamReader. 
            using (var sr = new StreamReader(@"C:\Users\tiliska\documents\sch-output2.csv"))
            {
                sr.ReadLine();
                string svLine;
                // Read and display lines from the file until the end of  
                // the file is reached. 
                while ((svLine = sr.ReadLine()) != null)
                {
                    var scholasticData = svLine.Split(',');
                    var scholasticDto = new ScholasticDto();

                    //isbn13
                    var isbn13 = scholasticData[0];
                    scholasticDto.Isbn13 = isbn13;

                    var service = new BookService();

                    switch (test)
                    {
                        case "scholasticData":
                            //scholastic lower interest grade
                            var scholasticGradeLower = scholasticData[1];
                            scholasticDto.ScholasticGradeLower = scholasticGradeLower.Trim();

                            //scholastic higher interest grade
                            var scholasticGradeHigher = scholasticData[2];

                            if (scholasticGradeHigher.Trim().Length > 0) scholasticDto.ScholasticGradeHigher = (byte?)Convert.ToByte(scholasticGradeHigher);

                            //scholastic equivalent grade
                            try
                            {
                                var scholasticGrade = scholasticData[3];
                                scholasticDto.ScholasticGrade = Convert.ToDouble(scholasticGrade);
                            }
                            catch (FormatException) { Console.WriteLine("parameter not found"); }

                            //dra
                            var dra = scholasticData[4];
                            scholasticDto.Dra = dra;

                            //guided reading level
                            var guidedReading = scholasticData[5];
                            scholasticDto.GuidedReading = guidedReading;

                            service.PostScholasticData(scholasticDto);
                            break;
                        case "skillmetrics":
                            service.GetSkillMetrics(isbn13);
                            break;
                        case "contentmetrics":
                            service.GetContentMetrics(isbn13);
                            break;
                    }
                }
            }
        }

        [TestMethod]
        public void TestAggregateSkill()
        {
            var aboutSeven = AggregationFunctions.AggregateSkill(2, "13-15", 520, "H");
            Assert.IsTrue(aboutSeven > 7 && aboutSeven < 8);
            var aboutEleven = AggregationFunctions.AggregateSkill(6, "60", 1000, "V");
            Assert.IsTrue(aboutEleven > 11 && aboutEleven < 12);
        }

        [TestMethod]
        public void TestPostScholasticDataInMemory()
        {
            ParseScholasticData("scholasticData");
        }

        [TestMethod]
        public void TestGetSkillMetricsWithScholastic()
        {
            ParseScholasticData("skillmetrics");
        }

        [TestMethod]
        public void TestGetContentMetricsWithScholastic()
        {
            ParseScholasticData("contentmetrics");
        }

        [TestMethod]
        public void TestGetSkillMetricsWithLexile()
        {
            ParseLexileDB("skillmetrics");
        }
    }
}
