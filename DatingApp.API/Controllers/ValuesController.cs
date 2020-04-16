using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace DatingApp.API.Controllers
{
    [Authorize] 
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
         private readonly DataContext _context;
    public ValuesController(DataContext context)
        {
            _context = context;
        }
            // GET api/values
        [HttpGet]
        // public ActionResult<IEnumerable<Value>> Get()
        // {
        //     return _context.Value.ToList();
        // }
        public async Task<IActionResult> Values()
        {
            return Ok(await _context.Value.ToListAsync());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        // public ActionResult<Value> Get(int id)
        // {
        //     return 
        // }
        [AllowAnonymous]
        public async Task<IActionResult> Values(int id)
        {
            return Ok(await _context.Value.FirstOrDefaultAsync(x=>x.Id==id));
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
