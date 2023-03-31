using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Task_management_system.Models
{
    /// </summary>
    [Table("KeyType")]
    [Display(Name = "Тип номенклатура")]
    public class KeyType
    {
        [Key]
        public int IdKeyType { get; set; }
        [StringLength(255)]
        public string KeyTypeName { get; set; }
        [StringLength(255)]
        public string KeyTypeIntCode { get; set; }
        [StringLength(1024)]
        public string Description { get; set; }
        public bool IsSystem { get; set; }
        public IEnumerable<KeyValue> KeyValues { get; set; }
    }
}
