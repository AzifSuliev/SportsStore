﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string? AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        [ValidateNever]
        public ApplicationUser? AppUser { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal OrderTotal { get; set; }
        public string? OrderStatus { get; set; }

        public string? PaymentStatus { get; set; }
        public string? PaymentIntentId { get; set; }

        public string? TrackingNumber { get; set; } 
        public string? Carrier { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? StreetAddress { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? PostalCode { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? SessionId { get; set; }
    }
}
