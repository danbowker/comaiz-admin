using Microsoft.AspNetCore.Mvc;
using comaiz.data;
using comaiz.data.Models;
using Microsoft.EntityFrameworkCore;


namespace comaiz.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            //retreive all clients from db asynchronously
            return await dbContext.Clients.ToListAsync();

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClientAsync(int id)
        {
            var client = await dbContext.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, Client client)
        {
            if (id != client.Id)
            {
                return BadRequest();
            }

            dbContext.Entry(client).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            dbContext.Clients.Add(client);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("GetClient", new { id = client.Id }, client);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await dbContext.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            dbContext.Clients.Remove(client);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return dbContext.Clients.Any(e => e.Id == id);
        }
    }
}
