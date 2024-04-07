using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Название категории")]
        [MaxLength(25)]
        public string? Name { get; set; }
        [DisplayName("Порядковый номер")]
        [Range(1,100, ErrorMessage = "Значение вне диапазона")]
        public int DisplayOrder { get; set; }
        public string? ImageURL { get; set; }
    }
}
