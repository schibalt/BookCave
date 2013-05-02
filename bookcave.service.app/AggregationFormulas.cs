using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookCave.Service
{
    public class AggregationFunctions
    {
        private const double GRADEAGEOFFSET = 5.5;

        public static double AggregateContent(byte? scholasticGradeHigher, string scholasticGradeLower, byte? barnesAgeYoung, byte? barnesAgeOld)
        {
            double aggregateContent = double.NegativeInfinity;
            var contentMetrics = new List<double>();

            var scholasticOlder = (double)scholasticGradeHigher + GRADEAGEOFFSET;
            contentMetrics.Add(scholasticOlder);

            if (scholasticGradeLower.Equals("K"))
                scholasticGradeLower = "0";
            var scholasticYounger = Convert.ToDouble(scholasticGradeLower) + GRADEAGEOFFSET;
            contentMetrics.Add(scholasticYounger);

            if (barnesAgeOld != null)
                contentMetrics.Add((double)barnesAgeOld);

            if (barnesAgeYoung != null)
                contentMetrics.Add((double)barnesAgeYoung);

            if (contentMetrics.Count > 0)
                aggregateContent = contentMetrics.Average();

            return aggregateContent;
        }

        public static double AggregateSkill(double? scholasticGrade, string dra, short? lexScore, string guidedReading)
        {
            double aggregateAge = double.NegativeInfinity;

            var skillMetrics = new List<double>();

            if (dra != null)
            {
                var aggregateDra = ComputeDra(dra);
                if (aggregateDra != double.NegativeInfinity)
                    skillMetrics.Add(aggregateDra);
            }

            if (scholasticGrade != null)
                skillMetrics.Add(ComputeScholasticGrade(scholasticGrade));

            if (guidedReading != null)
                skillMetrics.Add(ComputeGuidedReading(guidedReading));

            if (lexScore > 0)
                skillMetrics.Add((double)ComputeLexileQuadratic(lexScore));

            if (skillMetrics.Count > 0)
                aggregateAge = skillMetrics.Average();

            return aggregateAge;
        }

        private static double ComputeScholasticGrade(double? scholasticGrade)
        {
            if (scholasticGrade != null)
            {
                var age = (double)scholasticGrade + GRADEAGEOFFSET;
                return age;
            }
            return double.NegativeInfinity;
        }

        private static double ComputeGuidedReading(string level)
        {
            if (string.IsNullOrEmpty(level)) throw new ArgumentNullException("columnName");

            char[] characters = level.ToUpperInvariant().ToCharArray();

            int sum = 0;

            for (int i = 0; i < characters.Length; i++)
            {
                sum *= 26;
                sum += (characters[i] - 'A' + 1);
            }

            var grade = 0.2297 * sum + 0.047; //linear formula from scholastic leveling chart
            //http://teacher.scholastic.com/products/guidedreading/leveling_chart.htm
            var age = Convert.ToDouble(grade) + GRADEAGEOFFSET;
            return age;
        }

        //convert developmental reading assessment score (range) to grade + then age
        private static double ComputeDra(string dra)
        {
            var re1 = "(\\d+)";	// lower dra bound
            var re2 = "(-)";	// hyphen
            var re3 = "(\\d+)";	// upper dra bound
            double nvDra = double.NegativeInfinity;

            var r = new Regex(re1 + re2 + re3, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var m = r.Match(dra);

            if (m.Success) //if the parameter is a range 
            {
                var lowerBound = m.Groups[1].ToString();
                var upperBound = m.Groups[3].ToString();
                int[] range = { Convert.ToSByte(lowerBound), Convert.ToSByte(upperBound) };
                nvDra = range.Average(); //average of range is dra
            }
            //parameter is just the dra score
            else
            {
                r = new Regex("(\\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                m = r.Match(dra);

                if (m.Success)
                    nvDra = Convert.ToInt32(m.Groups[1].ToString());
            }
            var grade = 0.07 * nvDra + 1.2394; //linear formula from scholastic leveling chart
            var age = grade + GRADEAGEOFFSET;
            return age;
        }

        private static double? ComputeLexileExponential(short? lexScore)
        {
            //exponential curve derived from lexile map
            //http://www.lexile.com/m/cms_page_media/135/Lexile%20Map_8.5x11_FINAL_1.pdf

            double? mappedLexileGrade = 0.5974 * Math.Exp(0.0022 * (double)lexScore);
            var age = mappedLexileGrade + GRADEAGEOFFSET;
            return age;
        }

        private static double? ComputeLexileQuadratic(short? lexScore)
        {
            //exponential curve derived from lexile map
            //http://www.lexile.com/m/cms_page_media/135/Lexile%20Map_8.5x11_FINAL_1.pdf

            double? mappedLexileGrade = 7 * Math.Pow(10, -6) * Math.Pow((double)lexScore, 2) - 0.0011 * lexScore + 0.727;
            var age = mappedLexileGrade + GRADEAGEOFFSET;
            return age;
        }
    }
}
