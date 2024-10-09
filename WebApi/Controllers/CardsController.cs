using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
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

        [HttpGet("sortedByNameASC")]
        public ActionResult<List<Card>> GetCardsSortedByNameASC()
        {
            try
            {
                List<Card> cards = cards_package.GetCards();
                cards = cards.OrderBy(card => card.Name).ToList();
                return Ok(cards);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpGet("sortedByNameDESC")]
        public ActionResult<List<Card>> GetCardsSortedByNameDESC()
        {
            try
            {
                List<Card> cards = cards_package.GetCards();
                cards = cards.OrderByDescending(card => card.Name).ToList();
                return Ok(cards);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpGet("filter")]
        public ActionResult<List<Card>> GetFilteredCards([FromQuery] string? name, [FromQuery] bool? wifi)
        {
            try
            {
                List<Card> cards = cards_package.GetCards();
                if (!string.IsNullOrEmpty(name))
                {
                    cards = cards.Where(card => card.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (wifi.HasValue)
                {
                    cards = cards.Where(card => card.Wifi == wifi.Value).ToList();
                }

                return Ok(cards);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost("uploadCSV")]
        //public ActionResult UploadCSV(IFormFile file)
        public ActionResult UploadCSV()
        {
            try
            {
                var dataFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data");
                var filePath = Path.Combine(dataFolder, "bk.csv");

                using var reader = new StreamReader(filePath);
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    BadDataFound = null // Ignore bad data
                };
                using var csv = new CsvReader(reader, csvConfig);

                var records = csv.GetRecords<Card>().ToList();

                foreach (var record in records)
                {
                    cards_package.CreateCard(record);
                }

                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }




    }
}
