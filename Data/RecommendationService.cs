using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;
using static longevity_1._0.Pages.Index;

namespace longevity_1._0.Data
{
    public class RecommendationService
    {
        public static readonly LongevityDbContext dc = new(new DbContextOptions<LongevityDbContext>());
        public Person Person { get; set; } = new Person();
        public int UID { get; set; } = 101349211;
        public DateTime CurrentDate { get; set; } = DateTime.Today;
        public List<StudyGroup>? ClassesContinuation { get;set; }
        public List<StudyGroup>? SoulOnline { get; set; }
        public List<StudyGroup>? MindOnline { get; set; }
        public List<StudyGroup>? BodyOnline { get; set; }
        public List<StudyGroup>? SoulLive { get; set; }
        public List<StudyGroup>? MindLive { get; set; }
        public List<StudyGroup>? BodyLive { get; set; }

        public Task<Dictionary<string, Dictionary<string, List<GroupRating>>>> GetRecommendationAsync(DateOnly startDate)
        {
            DateTime start = DateTime.Now;
            DateTime stop = DateTime.Now;
            Console.WriteLine(start);
            List<StudyGroup> sel = new();
            var grps = dc.StudyGroups.Include(g => g.DictionaryNavigation);
            Regex regex = new Regex(@"\d{2}\.\d{2}\.\d{4}");

            foreach (var g in grps)
            {
                string shed = g.PlannedSchedule ?? "" + " " + g.ActiveSchedule ?? "";
                MatchCollection matches = regex.Matches(shed);
                for (int i = 1; i < matches.Count; i += 2)
                    if (DateTime.Parse(matches[i].Value) > CurrentDate)
                    {
                        g.DictionaryNavigation = dc.Dictionaries.SingleOrDefault(d => d.Id == g.DictionaryId);
                        if (g.DictionaryNavigation != null)
                        {
                            g.DictionaryNavigation.ParentNavigation = dc.Dictionaries.SingleOrDefault(d => d.Id == g.DictionaryNavigation.ParentId);
                            if (g.DictionaryNavigation.ParentNavigation != null)
                                g.DictionaryNavigation.ParentNavigation.ParentNavigation = dc.Dictionaries.SingleOrDefault(d => d.Id == g.DictionaryNavigation.ParentNavigation.ParentId);
                        }
                        sel.Add(g);
                    }
            }

            var statistics = (from att in dc.Attendance
                              group att by new
                              {
                                  Группа = att.GroupId,
                                  Словарь = att.DictionaryId,
                                  Возраст = (CurrentDate.Year - att.Visitor.BirthDate.Value.Year),
                                  Пол = att.Visitor.Gender,
                                  Область = dc.Dictionaries.Single(a => a.Id == att.DictionaryId).Scope,
                                  Вид = ((att.Online ?? false) ? "онлайн" : "вживую")
                              } into g
                              orderby g.Key.Возраст, g.Key.Пол, g.Key.Вид
                              select new
                              {
                                  g.Key.Группа,
                                  g.Key.Словарь,
                                  g.Key.Вид,
                                  g.Key.Пол,
                                  g.Key.Область,
                                  g.Key.Возраст,
                                  Посещений = g.Count()
                              }).ToArray();

            //List<int> tusr = new();
            //using (TextFieldParser parser = new TextFieldParser($@"{Directory.GetCurrentDirectory()}\wwwroot\data\test.csv"))
            //{
            //    parser.TextFieldType = FieldType.Delimited;
            //    parser.SetDelimiters(",");
            //    while (!parser.EndOfData)
            //    {
            //        //Processing row
            //        string[] fields = parser.ReadFields();
            //        tusr.Add(int.Parse(fields[0]));
            //    }
            //}
            Person? rndUser = null;
            Person = rndUser = dc.People.Single(us => us.Id == UID);
            //rndUser = usrs[System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, usrs.Length)];
            // вариант нового пользователя - без истории посещений
            //do
            //{
            //    if (dc.Attendance.FirstOrDefault(p => p.PersonId == rndUser.Id) != null) rndUser = null;
            //} while (rndUser == null);
            var age = Age(rndUser.Gender, CurrentDate.Year - rndUser.BirthDate.Value.Year);
            var prv = dc.Attendance.Where(p => p.PersonId == rndUser.Id).Select(p => p.LessonId).Distinct().ToArray();
            var generalStatistics = (from s in statistics
                                     where (!string.IsNullOrEmpty(s.Область.ToString()))
                                     group s by new { s.Группа, s.Словарь, Возраст = Age(s.Пол, s.Возраст), s.Пол, s.Область, s.Вид } into g
                                     orderby g.Key.Группа, g.Key.Возраст, g.Key.Пол, g.Key.Область, g.Key.Вид
                                     select new
                                     {
                                         g.Key.Группа,
                                         g.Key.Словарь,
                                         g.Key.Возраст,
                                         Область = g.Key.Область?.ToString().Replace("_", " "),
                                         g.Key.Пол,
                                         g.Key.Вид,
                                         Посещений = g.Select(p => p.Посещений).Sum()
                                     }).Where(it => it.Пол == rndUser.Gender && it.Возраст == age).ToList();
            Dictionary<string, Dictionary<string, List<GroupRating>>> groupRatings = new() { { "онлайн", new() { { "Для ума", new List<GroupRating>() }, { "Для души", new List<GroupRating>() }, { "Для тела", new List<GroupRating>() } } }, { "вживую", new() { { "Для ума", new List<GroupRating>() }, { "Для души", new List<GroupRating>() }, { "Для тела", new List<GroupRating>() } } } };
            foreach (var onl in groupRatings.Keys)
                foreach (var ar in groupRatings[onl].Keys)
                    foreach (var g in sel)
                    {
                        var grs = generalStatistics.Where(it => (it.Группа == g.Id) && (it.Возраст == age) && (it.Вид == onl) && (it.Область == ar));
                        foreach (var gi in grs)
                            groupRatings[onl][ar].Add(new GroupRating { Group = gi.Группа ?? 0, Dict = gi.Словарь ?? 0, Rating = gi.Посещений });
                    }
            foreach (var onl in groupRatings.Keys)
                foreach (var ar in groupRatings[onl].Keys)
                    foreach (var g in sel)
                    {
                        groupRatings[onl][ar].Sort((it1, it2) => it1.Rating.CompareTo(it2.Rating));
                        groupRatings[onl][ar].Reverse();
                    }
            foreach (var onl in groupRatings.Keys)
                foreach (var ar in groupRatings[onl].Keys)
                {
                    var temp = groupRatings[onl][ar].Take(2).ToList();
                    groupRatings[onl][ar] = temp;
                }
            List<Visiting> visit = new();
            List<GroupRating> rvisit = new();
            if (dc.Attendance.FirstOrDefault(p => p.PersonId == rndUser.Id) != null)
            {
                int oldid = 0;
                var acomp = new AttComparer();
                //visit = dc.Attendance.Where(p => p.PersonId == rndUser.Id).ToList().Distinct(acomp).ToList();
                visit = dc.Attendance.Where(p => p.PersonId == rndUser.Id).OrderBy(a => a.DictionaryId).ToList();
                foreach (var v in visit)
                {
                    if (oldid == v.DictionaryId) continue;
                    GroupRating? prsnt = null;
                    foreach (var onl in groupRatings.Keys)
                        foreach (var ar in groupRatings[onl].Keys)
                        {
                            prsnt = groupRatings[onl][ar].Find(g => v.DictionaryId == g.Dict);
                            if (prsnt != null)
                            {
                                rvisit.Add(prsnt);
                                oldid = v.DictionaryId ?? 0;
                                break;
                            }
                        }
                }
                rvisit.Sort((it1, it2) => it1.Rating.CompareTo(it2.Rating));
                rvisit.Reverse();
            }
            groupRatings.Add("продолжение", new Dictionary<string, List<GroupRating>>());
            groupRatings["продолжение"].Add("занятий", rvisit);
            stop=DateTime.Now;
            Console.WriteLine($"{stop}/{stop - start}");
            return Task.FromResult(groupRatings);
        }
        private string Age(gender? gender, int age)
        {
            if (gender == Data.gender.женщина)
                switch (age)
                {
                    case < 60:
                        return "до 60";
                    case < 74:
                        return "до 74";
                    case < 80:
                        return "до 80";
                    case >= 80:
                        return "с 80 и после";
                }
            if (gender == Data.gender.мужчина)
                switch (age)
                {
                    case < 64:
                        return "до 64";
                    case < 78:
                        return "до 78";
                    case < 87:
                        return "до 87";
                    case >= 87:
                        return "с 87 и после";
                }
            return "";
        }

        public record GroupRating
        {
            public int Group { get; set; }
            public int Dict { get; set; }
            public int Rating { get; set; }
        }
    }
}