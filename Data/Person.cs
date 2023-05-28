using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Formats.Asn1.AsnWriter;

namespace longevity_1._0.Data
{
    /// <summary>
    /// Пользователь
    /// </summary>
    [Index(nameof(Gender), nameof(BirthDate), IsUnique = false)]
    [Table("People")]
    public class Person
    {
        public Person()
        {
            Visits = new HashSet<Visiting>();
        }

        /// <summary>
        /// Ид базы данных
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Пол
        /// </summary>
        public gender? Gender { get; set; }

        /// <summary>
        /// Дата создания личного дела
        /// </summary>
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// адрес проживания
        /// </summary>
        public string? ResidentialAddress { get; set; }

        /// <summary>
        /// ФИО
        /// </summary>
        public string? Name { get; set; }

        public virtual ICollection<Visiting> Visits { get; set; }

    }

    public enum gender
    {
        женщина, мужчина
    }
}
