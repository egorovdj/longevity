using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace longevity_1._0.Data
{
    /// <summary>
    /// Учебные группы
    /// </summary>
    [Index(nameof(DictionaryId), IsUnique = false)]
    [Table("StudyGroups")]
    public class StudyGroup
    {
        public StudyGroup()
        {
        }

        /// <summary>
        /// Ид базы данных
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// уникальный номер словаря
        /// </summary>
        public int? DictionaryId { get; set; }

        /// <summary>
        /// Адрес площадки
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// административный округ
        /// </summary>
        public string? AdministrativeRegion { get; set; }

        /// <summary>
        /// муниципальный округ
        /// </summary>
        public string? MunicipalDistrict { get; set; }

        /// <summary>
        /// расписание в активных периодах
        /// </summary>
        public string? ActiveSchedule { get; set; }

        /// <summary>
        /// расписание в закрытых периодах
        /// </summary>
        public string? ClosedSchedule { get; set; }

        /// <summary>
        /// расписание в плановом периоде
        /// </summary>
        public string? PlannedSchedule { get; set; }
        public virtual Dictionary? DictionaryNavigation { get; set; }
    }
}
