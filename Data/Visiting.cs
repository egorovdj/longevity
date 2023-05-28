using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace longevity_1._0.Data
{
    /// <summary>
    /// Посещение занятия
    /// </summary>
    [Index(nameof(GroupId), nameof(PersonId), IsUnique = false)]
    [Table("Attendance")]
    public class Visiting
    {
        public Visiting()
        {
        }

        /// <summary>
        /// Ид базы данных
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// уникальный номер группы
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// уникальный номер занятия
        /// </summary>
        public int? LessonId { get; set; }

        /// <summary>
        /// уникальный номер участника
        /// </summary>
        public int? PersonId { get; set; }

        /// <summary>
        /// уникальный номер словаря
        /// </summary>
        public int? DictionaryId { get; set; }

        /// <summary>
        /// Режим занятия - онлайн/вживую
        /// </summary>
        public bool? Online { get; set; }

        /// <summary>
        /// Дата занятия
        /// </summary>
        public DateTime? LessonDate { get; set; }

        /// <summary>
        /// Начало занятия
        /// </summary>
        public TimeSpan? LessonStarts { get; set; }

        /// <summary>
        /// Окончание занятия
        /// </summary>
        public TimeSpan? LessonEnd { get; set; }

        public virtual Person Visitor { get; set; }
    }
}
