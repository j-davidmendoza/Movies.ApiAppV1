﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieRepository)
        {
            _movieService = movieRepository;
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request,
            CancellationToken token)
        {
            var movie = request.MapToMovie();
            await _movieService.CreateAsync(movie, token);

            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
        }

        [AllowAnonymous]
        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug,
            CancellationToken token)
        {
            var userId = HttpContext.GetUserId();

            var movie = Guid.TryParse(idOrSlug, out var id) ? 
                await _movieService.GetByIdAsync(id, userId, token)
                : await _movieService.GetBySlugAsync(idOrSlug, userId, token);
            if (movie is null)
            { return NotFound(); }

            var response = movie.MapToResponse();
            return Ok(response);
        }
        
        [AllowAnonymous]
        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll(CancellationToken token)
        {
            var userId = HttpContext.GetUserId();

            var movies = await _movieService.GetAllAsync(userId, token);

            var moviesResponse = movies.MapToResponse();
            return Ok(moviesResponse);
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request,
            CancellationToken token)
        {           
            var movie = request.MapToMovie(id);

            var userId = HttpContext.GetUserId();

            var updatedMovie = await _movieService.UpdateAsync(movie, userId, token);
            if(updatedMovie is null)
            {  return NotFound(); }

            var response = updatedMovie.MapToResponse();
            return Ok(response);

        }
        
        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id,
            CancellationToken token)
        {
            var deleted = await _movieService.DeleteByIdAsync(id, token);

            if (!deleted)
            { return NotFound(); }

            return Ok();
        }
    }
}
