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

        [HttpGet]
        public ActionResult<List<Card>> GetCards() 
        {
            List<Card> cards = [];
            cards = cards_package.GetCards();
            if (cards == null || cards.Count == 0)
            {
                return NotFound("No cards found.");
            }
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
    }
}
