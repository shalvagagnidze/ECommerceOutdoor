﻿using DomainLayer.Entities;
using DomainLayer.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
    }
}
