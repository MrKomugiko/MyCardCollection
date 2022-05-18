using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCardCollection.Models;
using MyCardCollection.Repository;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MyCardCollection.Controllers
{
    public class LikesController : ControllerBase
    {
        //string _userId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private readonly IDeckLikeRepository _deckLikeRepository;

        public LikesController(IDeckLikeRepository deckLikeRepository)
        {
            _deckLikeRepository = deckLikeRepository;
        }

        [HttpPost]
        [Route("api/Like/Deck")]
        public async Task<IActionResult> AddDeckLike([FromBody]LikeDeckRequest likeRequest)
        {
            if (ModelState.IsValid)
            {
                DeckLike? like = await _deckLikeRepository.AddLike(likeRequest.DeckId, likeRequest.UserId);

                if(like == null)
                {
                    return NotFound();
                }

                return Ok(like.Id);

            }
            else
                return BadRequest();
        }

        [HttpDelete]
        [Route("api/Like/Deck")]
        public async Task<IActionResult> RemoveLike([FromBody] LikeDeckRequest likeRequest)
        {
            if (ModelState.IsValid)
            {
                await _deckLikeRepository.RemoveLike(likeRequest.DeckId, likeRequest.UserId);
                
                return Ok();
            }
            else
                return BadRequest();
        }


        public class LikeDeckRequest
        {
           [Required] public int DeckId { get; set; }
           [Required] public string? UserId { get; set; }
        }

    }
}
