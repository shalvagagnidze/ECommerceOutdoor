﻿using MediatR;
using ServiceLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Features.Queries.FacetQueries
{
    public record GetSubFacetValuesQuery(Guid id) : IRequest<IEnumerable<FacetValueModel>>;
}
