using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using birds_shop_api.Models.BirdsShop;
using NuGet.Protocol;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using birds_shop_api.Models;
using Microsoft.Extensions.Options;
using System.Text;
using birds_shop_api.Utils;
using Microsoft.AspNetCore.Authorization;

namespace birds_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BirdsContext _context;
        private readonly JWTSettings authOptions;

        public UsersController(BirdsContext context, IOptions<JWTSettings> AuthOptions)
        {
            _context = context;
            authOptions = AuthOptions.Value;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (_context.users == null)
            {
                return NotFound();
            }
            return await _context.users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUsers(long id)
        {
            if (_context.users == null)
            {
                return NotFound();
            }
            User? user = await _context.users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.password = "";

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUsers(long id, User user)
        {
            if (id != user.id)
            {
                return BadRequest();
            }

            bool haveErrors = false;
            string[] errorsArray = new string[2];
            List<User> usersList = await _context.users.ToListAsync();

            User? foundUser = usersList.FirstOrDefault((User item) => item.id == id);
            

            User? foundUserEmail = usersList.FirstOrDefault((User item) => item.email == user.email);
            User? foundUserPhone = usersList.FirstOrDefault((User item) => item.phone_number == user.phone_number);

            if (foundUser == null)
            {
                return BadRequest();
            }
            if (foundUserEmail != null && foundUserEmail.id != id)
            {
                haveErrors = true;
                errorsArray[0] = "email";
            }

            if (foundUserPhone != null && foundUserPhone.id != id)
            {
                haveErrors = true;
                errorsArray[1] = "phoneNumber";
            }

            if (haveErrors)
            {
                return BadRequest(new { errors = errorsArray });
            }

            if (user.password != "")
            {
                foundUser.password = PasswordHasher.HashPassword(user.password);
            }

            foundUser.email = user.email;
            foundUser.fullname = user.fullname;
            foundUser.address = user.address;
            foundUser.phone_number = user.phone_number;

            _context.Entry(foundUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context.users == null)
            {
                return Problem("Entity set 'BirdsContext.Users'  is null.");
            }

            bool haveErrors = false;
            string[] errorsArray = new string[2];
            List<User> usersList = await _context.users.ToListAsync();

            User? foundUserEmail = usersList.Find((User item) => item.email == user.email);
            User? foundUserPhone = usersList.Find((User item) => item.phone_number == user.phone_number);

            if (foundUserEmail != null)
            {
                haveErrors = true;
                errorsArray[0] = "email";
            }

            if (foundUserPhone != null)
            {
                haveErrors = true;
                errorsArray[1] = "phoneNumber";
            }

            if (haveErrors)
            {
                return BadRequest(new { errors = errorsArray });
            }

            user.password = PasswordHasher.HashPassword(user.password);

            try
            {
                _context.users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Problem("User not created");
            }


            return Ok("User created");
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> LoginUser(User user)
        {
            if (_context.users == null)
            {
                return Problem("Entity set 'BirdsContext.Users'  is null.");
            }

            User? foundUser = await _context.users.FirstOrDefaultAsync((item) => item.phone_number == user.phone_number);

            if (foundUser == null)
            {
                return BadRequest("Wrong user");
            }

            if (!PasswordHasher.VerifyHashedPassword(foundUser.password, user.password))
            {
                return BadRequest("Wrong password");
            }

            var claims = new List<Claim>
            {
                new Claim("id", foundUser.id.ToString()),
                new Claim("phone_number", foundUser.phone_number),
            };

            var signingkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.SecretKey));
            var jwt = new JwtSecurityToken(
                    issuer: authOptions.Issuer,
                    audience: authOptions.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
                    signingCredentials: new SigningCredentials(signingkey, SecurityAlgorithms.HmacSha256));

            return CreatedAtAction(nameof(LoginUser), new JwtSecurityTokenHandler().WriteToken(jwt));
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsers(long id)
        {
            if (_context.users == null)
            {
                return NotFound();
            }
            var users = await _context.users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            _context.users.Remove(users);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsersExists(long id)
        {
            return (_context.users?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
