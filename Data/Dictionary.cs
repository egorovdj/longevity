using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace longevity_1._0.Data
{
    /// <summary>
    /// Словарь
    /// </summary>
    [Index(nameof(ParentId), nameof(Scope), IsUnique = false)]
    [Table("Dictionaries")]
    public class Dictionary
    {
        public Dictionary()
        {
            Items = new HashSet<Dictionary>();
        }

        /// <summary>
        /// Ид базы данных
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Ид старшего уровня - для верхнего уровня null
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Область
        /// </summary>
        public scope? Scope { get; set; }

        public virtual Dictionary? ParentNavigation { get; set; }
        public virtual ICollection<Dictionary> Items { get; set; }
    }

    public enum scope
    {
        Для_ума, Для_души, Для_тела
    }
}
