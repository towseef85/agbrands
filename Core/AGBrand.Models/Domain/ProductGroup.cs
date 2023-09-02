////using System;
////using System.Collections.Generic;
////using System.ComponentModel.DataAnnotations;

////namespace AGBrand.Models.Domain
////{
////    public class ProductGroup
////    {
////        public ProductGroup()
////        {
////            ProductGroupItems = new List<ProductGroupItem>();
////        }

////        [Key]
////        public Guid Id { get; set; }

////        [MaxLength(100)]
////        public string Title { get; set; }

////        public virtual List<ProductGroupItem> ProductGroupItems { get; set; }
////    }
////}
