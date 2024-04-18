using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Название товара")]
        [Required]
        public string? Name { get; set; }
        [DisplayName("Описание товара")]
        public string? Description { get; set; }
        [DisplayName("Цена")]
        public decimal Price { get; set; }
        [DisplayName("Название категории")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        [DisplayName("Добавить изображения")]
        public List<ProductImage>? ProductImages { get; set; }
    }
}
