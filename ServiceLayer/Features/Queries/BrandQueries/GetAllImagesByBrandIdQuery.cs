using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Features.Queries.BrandQueries;

public record GetAllImagesByBrandIdQuery(Guid Id) : IRequest<List<string>>;
