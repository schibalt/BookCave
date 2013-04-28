using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using BookCave.Service.Dto;
using BookCave.Service;
using System.Net;
using System.Runtime.Serialization.Json;

namespace BookcaveUnitTests
{
    [TestClass]
    public class BookcaveUnitTests
    {
        [TestMethod]
        public void TestPostBarnesNobleData()
        {
            // Create an instance of StreamReader to read from a file. 
            // The using statement also closes the StreamReader. 
            using (var sr = new StreamReader(@"C:\Users\tiliska\Downloads\bn-sample.csv"))
            {
                string svLine;
                // Read and display lines from the file until the end of  
                // the file is reached. 
                while ((svLine = sr.ReadLine()) != null)
                {
                    var barnesNobleData = svLine.Split(',');
                    var bnDto = new BarnesDto();

                    var isbn13 = Convert.ToInt64(barnesNobleData[0]);
                    bnDto.Isbn13 = isbn13;

                    var ageLow = Convert.ToByte(barnesNobleData[1]);
                    bnDto.BarnesAgeYoung = ageLow;

                    var ageHigh = Convert.ToByte(barnesNobleData[2]);
                    bnDto.BarnesAgeOld = ageHigh;

                    var rating = Convert.ToDouble(barnesNobleData[3]);
                    bnDto.BarnesAvg = rating;

                    try
                    {
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("data in csv aren't valid");
                    }

                    var service = new BookService();
                    service.PostBarnesData(bnDto);
                }
            }
        }

        [TestMethod]
        public void TestPostLexileData()
        {
            ParseLexileDB("testinstance");
        }

        private void ParseLexileDB(string method)
        {
            // Create an instance of StreamReader to read from a file. 
            // The using statement also closes the StreamReader. 
            using (var sr = new StreamReader(@"C:\Users\tiliska\Documents\lexiletitles.txt"))
            {
                sr.ReadLine();//eat first line
                string svLine;
                var line = 1;

                // Read and display lines from the file until the end of  
                // the file is reached. 
                while ((svLine = sr.ReadLine()) != null)
                {
                    if (line % 1000 == 0)
                        Console.WriteLine(line);
                    line++;
                    var bookParams = svLine.Split('\t');
                    var lexileData = new LexileDto();

                    lexileData.Title = bookParams[0];
                    lexileData.Author = bookParams[1];

                    try
                    {
                        lexileData.Isbn = bookParams[2];

                        try
                        {
                            lexileData.Isbn13 = Convert.ToInt64(bookParams[3]);

                            if (bookParams[5].Length > 0)
                                lexileData.LexScore = Convert.ToInt16(bookParams[5]);

                            if (bookParams[7].Length > 0)
                                lexileData.PageCount = Convert.ToInt16(bookParams[7]);

                            lexileData.LexUpdate = Convert.ToDateTime(bookParams[12]);
                        }
                        catch (FormatException) { Console.WriteLine("metametrics messed up the row in the text db on line " + line); }

                        lexileData.LexCode = bookParams[4];
                        lexileData.Publisher = bookParams[6];
                        lexileData.DocType = bookParams[8];
                        lexileData.Series = bookParams[9];
                        lexileData.Awards = bookParams[10];
                        lexileData.Summary = bookParams[11];

                        if (method.Equals("post"))
                        {
                            var webClient = new WebClient();
                            webClient.Headers["Content-type"] = "application/json";

                            var memoryStream = new MemoryStream();
                            var serializedJson = new DataContractJsonSerializer(typeof(LexileDto));

                            serializedJson.WriteObject(memoryStream, lexileData);

                            var uri = new Uri(@"http://apps.apprenda.local/api/services/json/r/bookcave(v1)/BookService/IBook/books");
                            byte[] res1 = webClient.UploadData(uri.ToString(), "POST", memoryStream.ToArray());
                        }
                        else
                        {
                            var service = new BookService();
                            service.PostLexileData(lexileData);
                        }
                    }
                    catch (IndexOutOfRangeException) { Console.WriteLine("metametrics messed up the row in the text db on line " + line); }
                }
            }
        }

        [TestMethod]
        public void PostLexileData()
        {
            ParseLexileDB("post");
        }
    }
}
