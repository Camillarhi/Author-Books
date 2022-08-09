using AuthorsAPI.DTOs;
using AuthorsAPI.Model;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthorController> _logger;
        private readonly UserManager<AuthorModel> _userManager;
        private readonly SignInManager<AuthorModel> _signInManager;

        public AuthorController(IMapper mapper, IConfiguration configuration, ILogger<AuthorController> logger, UserManager<AuthorModel> userManager, SignInManager<AuthorModel> signInManager)
        {
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateAuthorDTO authordto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var author = _mapper.Map<AuthorModel>(authordto);
                    author.UserName = authordto.Email;
                    var result = await _userManager.CreateAsync(author, authordto.Password);
                    if (!result.Succeeded)
                    {
                        return BadRequest($"User registration failed");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Something went wrong");
                    return StatusCode(500, "Internal server error");
                }
            }
            _logger.LogError($"Invalid Post attempt");
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] LoginDTO login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent: false, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        var user = await _userManager.FindByEmailAsync(login.Email);
                        var role = await _userManager.GetRolesAsync(user);
                        var logedIn = new AuthenticationResponse
                        {
                            Email = user.Email,
                            Id = user.Id
                        };
                        return BuildToken(logedIn);
                    }
                    else
                    {
                        return BadRequest(new { Error = "Invalid Username or Password" });
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        private AuthenticationResponse BuildToken(AuthenticationResponse login)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", login.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["keyjwt"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(1);
            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: creds);
            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = login.Email,
                Id = login.Id,
                Expiration = expiration
            };

        }

    }
}
