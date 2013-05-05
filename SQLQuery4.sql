select * 
from BookRecords
full outer join SkillRecords on SkillRecords.Isbn13=BookRecords.Isbn13
full outer join ContentRecords on BookRecords.Isbn13=ContentRecords.Isbn13
full outer join RatingRecords on BookRecords.Isbn13=RatingRecords.Isbn13
where SkillRecords.AverageSkillAge>=6 and SkillRecords.AverageSkillAge<7
