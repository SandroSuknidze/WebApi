using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Packages;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController(IPKG_CARDS pKG_CARDS) : ControllerBase
    {
        readonly IPKG_CARDS cards_package = pKG_CARDS;

        [HttpGet("{id}")]
        public ActionResult<Card> GetCardById(int id)
        {
            try
            {
                Card card;
                card = cards_package.GetCardById(id);
                return Ok(card);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Card with ID {id} not found.");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }

        }

        [HttpGet]
        public ActionResult<List<Card>> GetCards()
        {
            List<Card> cards = [];
            cards = cards_package.GetCards();
            return Ok(cards);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCard(int id)
        {
            try
            {
                cards_package.DeleteCard(id);
                return Ok($"Card with ID {id} deleted successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Card with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult CreateCard(Card card)
        {
            if (card == null)
            {
                return BadRequest("Card info required");
            }
            else
            {
                try
                {
                    cards_package.CreateCard(card);
                    return Ok("");

                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
                }
            }
        }

        [HttpPut]
        public ActionResult UpdateCard(Card card)
        {
            try
            {
                cards_package.UpdateCard(card);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }
    }
}
