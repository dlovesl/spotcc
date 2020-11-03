using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spotcc.Services;
using Spotcc.Services.Models;

namespace SpotCC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly IRepository<Artist> _artistRepo;
        private readonly IRepository<Account> _accountRepo;
        private readonly ILogger<ArtistController> _logger;

        public ArtistController(ILogger<ArtistController> logger, IRepository<Artist> artistRepo, IRepository<Account> accountRepo)
        {
            _artistRepo = artistRepo;
            _accountRepo = accountRepo;

            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public Artist GetArtist(int id)
        {
            var artist = _artistRepo.FindFirst(a => a.Id == id);
            return artist;
        }

        [HttpGet("spotify/{spotifyId}")]
        public Artist GetArtist(string spotifyId)
        {
            var artist = _artistRepo.FindFirst(a => a.SpotifyId == spotifyId);
            return artist;
        }

        [HttpPost]
        public IActionResult CreateArtist(Artist artist)
        {
            var error = string.Empty;
            if (artist == null)
            {
                error = $"Parameter '{nameof(artist)}' should not be null.";
            } else if (string.IsNullOrEmpty(artist.Name))
            {
                error = $"Artist name is required.";
            } else if (_accountRepo.FindFirst(a => a.Id == artist.AccountId) == null)
            {
                error = $"Account with Id: '{artist.AccountId}' is not existed.";
            } else if (GetArtist(artist.Id) != null || GetArtist(artist.SpotifyId) != null)
            {
                error = $"Artist with Id: '{ artist.Id }' - SpotifyId: '{ artist.SpotifyId?? string.Empty }' is already existed.";
            }

            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogDebug(error);
                return BadRequest(new { error });
            }

            artist = _artistRepo.Insert(artist);
            return CreatedAtAction(nameof(CreateArtist), new { artist }, artist);
        }
    }
}