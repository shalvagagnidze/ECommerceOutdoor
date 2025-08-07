using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Features.Commands.ImageCommands
{
    public record UploadImageCommand(Guid productId,List<IFormFile> files,bool IsBrand = false) : IRequest<List<string>>;
}
