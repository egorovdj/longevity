using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace longevity_1._0.Data
{
    public partial class LongevityDbContext : DbContext
    {
        public LongevityDbContext(DbContextOptions<LongevityDbContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Dictionary> Dictionaries { get; set; }
        public DbSet<Visiting> Attendance { get; set; }
        public DbSet<StudyGroup> StudyGroups { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-53bc9b9d-9d6a-45d4-8429-2a2761773502;Trusted_Connection=True;MultipleActiveResultSets=true");
                //optionsBuilder.UseSqlServer("Data Source=ms-sql-9.in-solve.ru;Initial Catalog=1gb_ordersunit;User ID=1gb_egorovdj;Password=3zzbbc78bqwr;MultipleActiveResultSets=true;Encrypt=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            return; // Для ускорения очень долгая инициализация БД заблокирована
            DateTime start = DateTime.Now;
            DateTime stop = DateTime.Now;
            Console.WriteLine(start);
            //if (stop < start)
            //{
                int maxId = 0;
                List<Person>? us = new();
                using (TextFieldParser parser = new TextFieldParser($@"{Directory.GetCurrentDirectory()}\wwwroot\data\users.csv"))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        //Processing row
                        string[] fields = parser.ReadFields();
                        if (maxId == 0)
                        {
                            maxId = 1;
                            continue;
                        }
                        int id = int.Parse(fields[0]);
                        us.Add(new Person
                        {
                            BirthDate = DateTime.Parse(fields[3]),
                            CreationDate = DateTime.Parse(fields[1]),
                            Gender = fields[2] == "Женщина" ? gender.женщина : gender.мужчина,
                            Id = id,
                            ResidentialAddress = fields[4]
                        });
                    }
                }
                //modelBuilder.Entity<Person>().HasData(us);

                int dc = 0;
                Dictionary<string, scope> sc = new() { { "Для ума", scope.Для_ума }, { "Для души", scope.Для_души }, { "Для тела", scope.Для_тела } };
                Dictionary<int, Dictionary>? dict = new();
                using (TextFieldParser parser = new TextFieldParser($@"{Directory.GetCurrentDirectory()}\wwwroot\data\dict.csv"))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");
                    while (!parser.EndOfData)
                    {
                        //Processing row
                        string[] fields = parser.ReadFields();
                        if (dc == 0)
                        {
                            dc = 1;
                            continue;
                        }
                        else dc++;
                        if (!sc.ContainsKey(fields[0])) Console.WriteLine($"Ошибка области в строке {dc}");
                        int id = int.Parse(fields[1]);
                        int pid = id;
                        if (!dict.ContainsKey(id)) dict.Add(id, new Dictionary { Id = id, Scope = sc[fields[0]], Title = fields[2] });
                        id = int.Parse(fields[3]);
                        if (!dict.ContainsKey(id)) dict.Add(id, new Dictionary { Id = id, ParentId = pid, Scope = sc[fields[0]], Title = fields[4] });
                        pid = id;
                        id = int.Parse(fields[5]);
                        if (!dict.ContainsKey(id)) dict.Add(id, new Dictionary { Id = id, ParentId = pid, Scope = sc[fields[0]], Title = fields[6], Description = fields[7] });
                        else Console.WriteLine($"Дубликат ключа {id} в строке {dc}");
                    }
                }
                //modelBuilder.Entity<Dictionary>().HasData(dict.Values.ToList<Dictionary>());

                //int tc = 0;
                //List<Visiting> visitings = new();
                //using (TextFieldParser parser = new TextFieldParser($@"{System.IO.Directory.GetCurrentDirectory()}\wwwroot\data\attend.csv"))
                //{
                //    parser.TextFieldType = FieldType.Delimited;
                //    parser.SetDelimiters(",");
                //    while (!parser.EndOfData)
                //    {
                //        //Processing row
                //        string[] fields = parser.ReadFields();
                //        if (tc == 0)
                //        {
                //            tc = 1;
                //            continue;
                //        }
                //        else tc++;
                //        int uid = int.Parse(fields[2]);
                //        int id = int.Parse(fields[0]);
                //        int gid = int.Parse(fields[1]);
                //        int did = 0;
                //        foreach (var it in dict.Values.Where(d => d.Title == fields[3]))
                //            foreach (var nit in dict.Values.Where(dict => dict.Title == fields[4]))
                //                if (it.Id == nit.ParentId)
                //                {
                //                    did = nit.Id;
                //                    break;
                //                }
                //        var ol = fields[5] == "Да";
                //        DateOnly date = DateOnly.Parse(fields[6]);
                //        TimeOnly timeFrom = TimeOnly.Parse(fields[7]);
                //        TimeOnly timeTo = TimeOnly.Parse(fields[8]);
                //        visitings.Add(new Visiting { DictionaryId = did, GroupId = gid, LessonId = id, Id = tc, Online = ol, PersonId = uid, LessonDate = date.ToDateTime(TimeOnly.MinValue), LessonEnd = timeTo.ToTimeSpan(), LessonStarts = timeFrom.ToTimeSpan() });
                //    }
                //}
                //us = null;
                //dict = null;
                //GC.Collect();

                //using (var writer = new StreamWriter($@"{System.IO.Directory.GetCurrentDirectory()}\wwwroot\data\attendProto.csv"))
                //using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
                //{
                //    csv.WriteRecords(visitings);
                //}

                //    modelBuilder.Entity<Visiting>().HasData(visitings);
            //}

            List<StudyGroup> groups = new();
            using (TextFieldParser parser = new TextFieldParser($@"{Directory.GetCurrentDirectory()}\wwwroot\data\groups.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                int count = 0;
                int len = 0;
                int len23 = 0;
                int cl1 = 0;
                int cl2 = 0;
                int cl3 = 0;
                int cl0 = 0;
                while (!parser.EndOfData)
                {
                    //Processing row
                    string[] fields = parser.ReadFields();
                    if (count == 0)
                    {
                        count++;
                        continue;
                    }
                    len++;
                    int id = int.Parse(fields[0]);
                    //var les = visitings.FirstOrDefault(i => i.GroupId == id);
                    //if (les == null)
                    //{
                    //    count++;
                    //}
                    var l1 = dict.Values.ToList<Dictionary>().FirstOrDefault(l => l.Title == fields[1] && l.ParentId == null);
                    var l2 = dict.Values.ToList<Dictionary>().FirstOrDefault(l => l.Title == fields[2] && l1 != null && l.ParentId == l1.Id);
                    var l3 = dict.Values.ToList<Dictionary>().FirstOrDefault(l => l.Title == fields[3] && l2 != null && l.ParentId == l2.Id);
                    if (l1 == null) cl1++;
                    if (l2 == null)
                        cl2++;
                    if (l3 == null)
                        cl3++;
                    groups.Add(new StudyGroup
                    {
                        Id = id,
                        DictionaryId = l3?.Id,
                        Address = fields[4],
                        AdministrativeRegion = fields[5],
                        MunicipalDistrict = fields[6],
                        ActiveSchedule = fields[7],
                        ClosedSchedule = fields[8],
                        PlannedSchedule = fields[9]
                    });
                }
                using (var writer = new StreamWriter($@"{System.IO.Directory.GetCurrentDirectory()}\wwwroot\data\groupsProto.csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
                {
                    csv.WriteRecords(groups);
                }

                Console.WriteLine($"{count - 1}/{len23}({cl1},{cl2},{cl3} - {cl0})/{len}");
                //modelBuilder.Entity<StudyGroup>().HasData(() => groups);
            }
            stop = DateTime.Now;
            Console.WriteLine($"{stop} {stop - start}");
        }
    }
}