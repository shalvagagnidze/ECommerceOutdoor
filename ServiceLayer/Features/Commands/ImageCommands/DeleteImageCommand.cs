using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Features.Commands.ImageCommands
{
    public record DeleteImageCommand(Guid productId, string key,bool IsBrand) : IRequest;
}
