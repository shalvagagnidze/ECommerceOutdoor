﻿using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceOutdoor.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        private ISender? _mediator;

        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    }
}
