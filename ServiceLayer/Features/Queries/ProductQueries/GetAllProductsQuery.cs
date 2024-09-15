﻿using MediatR;
using ServiceLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Features.Queries.ProductQueries
{
    public record GetAllProductsQuery : IRequest<IEnumerable<ProductResponseModel>>;

}
