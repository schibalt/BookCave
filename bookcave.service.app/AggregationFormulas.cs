using BookCave.Service.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookCave.Service
{
    public class DataFunctions
    {
        private const double GRADEAGEOFFSET = 5.5;

        public static double GetContentAverageAge(ContentRecord contentRecord)
        {
            var averageAge = double.NegativeInfinity;
            var contentMetrics = new List<double>();

            if (contentRecord.ScholasticGradeHigher != null)
            {
                var scholasticOlder = (double)contentRecord.ScholasticGradeHigher + GRADEAGEOFFSET;
                contentMetrics.Add(scholasticOlder);
            }

            if (contentRecord.ScholasticGradeLower != null)
            {
                if (contentRecord.ScholasticGradeLower.Equals("K"))
                    contentRecord.ScholasticGradeLower = "0";

                var scholasticYounger = Convert.ToDouble(contentRecord.ScholasticGradeLower) + GRADEAGEOFFSET;
                contentMetrics.Add((double)scholasticYounger);
            }

            if (contentRecord.BarnesAgeOld != null) contentMetrics.Add((double)contentRecord.BarnesAgeOld);

            if (contentRecord.BarnesAgeYoung != null) contentMetrics.Add((double)contentRecord.BarnesAgeYoung);

            if (contentRecord.CommonSensePause != null) contentMetrics.Add((double)contentRecord.CommonSenseOn);

            if (contentRecord.CommonSensePause != null) contentMetrics.Add((double)contentRecord.CommonSensePause);

            if (contentRecord.CommonSenseNoKids != null) if ((bool)contentRecord.CommonSenseNoKids) contentMetrics.Add(double.PositiveInfinity);

            if (contentMetrics.Count > 0)
                averageAge = contentMetrics.Average();

            return averageAge;
        }

        public static double? GetAverageSkillAge(SkillRecord skillRecord)
        {
            double? averageAge = null;

            var skillMetrics = new List<double>();

            if (skillRecord.Dra != null)
            {
                var draAge = ComputeDra(skillRecord.Dra);
                if (draAge != double.NegativeInfinity)
                    skillMetrics.Add(draAge);
            }

            if (skillRecord.ScholasticGrade != null) skillMetrics.Add(ComputeScholasticGrade(skillRecord.ScholasticGrade));

            if (skillRecord.GuidedReading != null) skillMetrics.Add(ComputeGuidedReading(skillRecord.GuidedReading));

            if (skillRecord.LexScore > 0) skillMetrics.Add((double)ComputeLexileQuadratic(skillRecord.LexScore));

            if (skillMetrics.Count > 0) averageAge = skillMetrics.Average();

            return averageAge;
        }

        private static double ComputeScholasticGrade(double? scholasticGrade)
        {
            if (scholasticGrade != null) { var age = (double)scholasticGrade + GRADEAGEOFFSET; return age; }
            return double.NegativeInfinity;
        }

        private static double ComputeGuidedReading(string level)
        {
            if (string.IsNullOrEmpty(level)) throw new ArgumentNullException("columnName");

            char[] characters = level.ToUpperInvariant().ToCharArray();

            int sum = 0;

            for (int i = 0; i < characters.Length; i++) { sum *= 26; sum += (characters[i] - 'A' + 1); }

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

                if (m.Success) nvDra = Convert.ToInt32(m.Groups[1].ToString());
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
