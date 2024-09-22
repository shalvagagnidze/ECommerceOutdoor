﻿using DomainLayer.Common.Enums;

namespace ServiceLayer.Models
{
    public class ProductModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public StockStatus Status { get; set; }
        public Condition Condition { get; set; }
        public string? Description { get; set; }
        public List<string>? Images { get; set; }
        public ICollection<ProductFacetValueModel> ProductFacetValues { get; set; } = new List<ProductFacetValueModel>();
    }
}
