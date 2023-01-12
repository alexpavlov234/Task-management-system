using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace Task_management_system.Models
{
    [Table("KeyValue")]
    [Display(Name = "Стойности на номенклатура")]
    public class KeyValue
    {
        [Key]
        public int IdKeyValue { get; set; }

        public int IdEntity => this.IdKeyValue;

        [Required]
        [ForeignKey(nameof(KeyType))]
        public int IdKeyType { get; set; }
        public KeyType KeyType { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? NameEN { get; set; }

        [StringLength(255)]
        public string KeyValueIntCode { get; set; }


        [StringLength(1024)]
        public string? Description { get; set; }

        [StringLength(1024)]
        public string? DescriptionEN { get; set; }

        public int Order { get; set; }

        [StringLength(255)]
        public string? DefaultValue1 { get; set; }

        [StringLength(255)]
        public string? DefaultValue2 { get; set; }

        [StringLength(255)]
        public string? DefaultValue3 { get; set; }

        [StringLength(4000)]
        public string? FormattedText { get; set; }

        [StringLength(4000)]
        public string? FormattedTextEN { get; set; }


        [Comment("Определя дали стойността е активна")]
        public bool IsActive { get; set; }

    }
}
