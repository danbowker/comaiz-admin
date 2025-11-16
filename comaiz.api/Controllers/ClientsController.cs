using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace comaiz.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly ComaizContext dbContext;

        public ClientsController(ComaizContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClientsAsync()
        {
            if(dbContext.Clients == null) return StatusCode(StatusCodes.Status500InternalServerError);

            return await dbContext.Clients.ToListAsync();

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClientAsync(int id)
        {
            if(dbContext.Clients == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var client = await dbContext.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        [HttpPut]
        public async Task<IActionResult> PutClient(Client client)
        {
            if (dbContext.Clients == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.Entry(client).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(client.Id))
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            if (dbContext.Clients == null) return StatusCode(StatusCodes.Status500InternalServerError);

            dbContext.Clients.Add(client);
            await dbContext.SaveChangesAsync();
            return CreatedAtAction("GetClient", new { id = client.Id }, client);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            if (dbContext.Clients == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var client = await dbContext.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            dbContext.Clients.Remove(client);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/duplicate")]
        public async Task<ActionResult<Client>> DuplicateClient(int id)
        {
            if (dbContext.Clients == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var client = await dbContext.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            var duplicatedClient = new Client
            {
                ShortName = client.ShortName,
                Name = client.Name != null ? $"{client.Name} (Copy)" : null
            };

            dbContext.Clients.Add(duplicatedClient);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetClient", new { id = duplicatedClient.Id }, duplicatedClient);
        }

        private bool ClientExists(int id)
        {
            if (dbContext.Clients == null) return false;

            return dbContext.Clients.Any(e => e.Id == id);
        }
    }
}
