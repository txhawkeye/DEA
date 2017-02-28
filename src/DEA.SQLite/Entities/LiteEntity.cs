using System.ComponentModel.DataAnnotations;

namespace DEA.SQLite
{
    public abstract class LiteEntity<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
