﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Models
{
    public class BrandModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Origin { get; set; }
        public string? Description { get; set; }
    }
}
