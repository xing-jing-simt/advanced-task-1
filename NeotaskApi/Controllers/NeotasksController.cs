using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeotaskApi.Models;

namespace NeotaskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NeotasksController : ControllerBase
    {
        private readonly NeotaskContext _context;

        public NeotasksController(NeotaskContext context)
        {
            _context = context;
        }

        // GET: api/Neotasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Neotask>>> GetNeotasks()
        {
            if (_context.Neotasks == null)
            {
                return NotFound();
            }
            return await _context.Neotasks.ToListAsync();
        }

        // GET: api/Neotasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Neotask>> GetNeotask(long id)
        {
            if (_context.Neotasks == null)
            {
                return NotFound();
            }
            var neotask = await _context.Neotasks.FindAsync(id);

            if (neotask == null)
            {
                return NotFound();
            }

            return neotask;
        }

        // PUT: api/Neotasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNeotask(long id, Neotask neotask)
        {
            if (id != neotask.Id)
            {
                return BadRequest();
            }

            _context.Entry(neotask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NeotaskExists(id))
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

        // POST: api/Neotasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Neotask>> PostNeotask(Neotask neotask)
        {
            if (_context.Neotasks == null)
            {
                return Problem("Entity set 'NeotaskContext.Neotasks'  is null.");
            }
            Console.WriteLine(System.Environment.GetEnvironmentVariable("RABBITMQ_HOST"));
            Console.WriteLine(System.Environment.GetEnvironmentVariable("RABBITMQ_PORT"));
            var factory = new ConnectionFactory()
            {
                HostName = System.Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                Port = Convert.ToInt32(System.Environment.GetEnvironmentVariable("RABBITMQ_PORT"))
            };
            var contentToSend = new StringContent(JsonSerializer.Serialize(neotask), Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: "tasks",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null

                );

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(neotask));

                channel.BasicPublish(
                    exchange: "",
                    routingKey: "tasks",
                    basicProperties: null,
                    body: body
                );
            }
            _context.Neotasks.Add(neotask);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNeotask", new { id = neotask.Id }, neotask);
        }

        // DELETE: api/Neotasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNeotask(long id)
        {
            if (_context.Neotasks == null)
            {
                return NotFound();
            }
            var neotask = await _context.Neotasks.FindAsync(id);
            if (neotask == null)
            {
                return NotFound();
            }

            _context.Neotasks.Remove(neotask);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NeotaskExists(long id)
        {
            return (_context.Neotasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
