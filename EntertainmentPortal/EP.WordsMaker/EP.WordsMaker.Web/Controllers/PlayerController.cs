﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EP.WordsMaker.Logic.Commands;
using EP.WordsMaker.Logic.Models;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using EP.WordsMaker.Logic.Queries;
using NSwag.Annotations;

namespace EP.WordsMaker.Web.Controllers
{
	[ApiController]
	public class PlayerController : ControllerBase
	{
        private readonly IMediator _mediator;

        public PlayerController(IMediator mediator)
        {
            _mediator = mediator;
        }
  
		[HttpGet("api/players")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<Player>), Description = "Success")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "Books collection is empty")]
        public async Task<IActionResult> GetAllPlayersAsync()
        {
            var result = await _mediator.Send(new GetAllPlayers());
            return result.HasValue ? (IActionResult)Ok(result) : NotFound();
        }

        [HttpPost("api/players")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Player), Description = "Success")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "Invalid data")]
        public async Task<IActionResult> AddNewPlayerASync([FromBody] AddNewPlayerCommand model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(model);
            return result.IsFailure ?
                (IActionResult)BadRequest(result.Error)
                : Ok(result.Value);
        }
    }
}
