using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BookStore.Unitily;
using BookStore.Models.Authentication.SignUp;
using BookStore.Models.Serive;
using UserManagementService.Models;
using BookStore.Models.Authentication.Login;
using BookStore.Models.ModelsToRequest;

namespace BookStoreApi.Controllers.Admin
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser>? UserManager;
        private readonly RoleManager<IdentityRole>? RoleManager;
        private readonly SignInManager<IdentityUser>? SignInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public AuthenticationController(UserManager<IdentityUser>? userManager, RoleManager<IdentityRole>? roleManager, SignInManager<IdentityUser>? signInManager, IConfiguration configuration, IEmailService emailService)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            SignInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("Role")]
        public async Task<IActionResult> RegisterAccount([FromBody] RegisterUser registerUser, string Role)
        {
            try
            {
                // check user exits
                var userExits = await UserManager.FindByEmailAsync(registerUser.Email);
                if (userExits != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "User already exits!" });
                }
                else
                {
                    IdentityUser user = new IdentityUser
                    {
                        Email = registerUser.Email,
                        UserName = registerUser.UserName,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        TwoFactorEnabled = false
                    };
                    // check role exits
                    if (await RoleManager.RoleExistsAsync(Role))
                    {
                        var result = await UserManager.CreateAsync(user, registerUser.Password);
                        if (!result.Succeeded)
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Failed", Message = "Register is Failed" });
                        }
                        // Add role to the user
                        await UserManager.AddToRoleAsync(user, Role);

                        // Add Token to Verified the email
                        var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = user.Email }, Request.Scheme); // Get link to Transfer to action ConfirmEmail
                        var message = new Message(new string[] { user.Email! }, "Confirmation email link", confirmationLink);
                        _emailService.SendEmail(message);

                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "User created & Send Email is SuccessFully" });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Failed", Message = "Role is not fould" });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAccount([FromBody] Login login)
        {
            // checking the user && password
            var user = await UserManager.FindByNameAsync(login.UserName);
            try
            {
                if (user != null)
                {
                    if (user.TwoFactorEnabled)
                    {
                        await SignInManager.SignOutAsync();
                        await SignInManager.PasswordSignInAsync(user, login.Password, false, true);
                        var token = await UserManager.GenerateTwoFactorTokenAsync(user, "Email");
                        //  string twoFactorAuthLink = Url.Action("Verifie", "Authentication", new { token, email = user.Email }, Request.Scheme);
                        var message = new Message(new string[] { user.Email }, "OTP To Confirmation", token);
                        _emailService.SendEmail(message);

                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}" });
                    }
                    if (user != null && await UserManager.CheckPasswordAsync(user, login.Password))
                    {
                        // claim list creation
                        var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                        // we add roles to the list
                        var userRoles = await UserManager.GetRolesAsync(user);
                        foreach (var role in userRoles)
                        {
                            authClaims.Add(new Claim(ClaimTypes.Role, role));
                        }
                        // Generation the token with the claims
                        var jwtToken = GetToken(authClaims);

                        // Return Token
                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                            expiration = jwtToken.ValidTo
                        });
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

            // returning the token
            return Unauthorized();
        }

        [HttpPost]
        [Route("login2FA")]
        public async Task<IActionResult> LoginWithOTP([FromBody]Login2FARequest login2FA)
        {
            var user = await UserManager.FindByNameAsync(login2FA.UserName);
            var signIn = await SignInManager.TwoFactorSignInAsync("Email", login2FA.OtpCode, false, false);

            if (signIn.Succeeded)
            {
                if (user != null)
                {
                    // claim list creation
                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                    // we add roles to the list
                    var userRoles = await UserManager.GetRolesAsync(user);
                    foreach (var role in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    // Generation the token with the claims
                    var jwtToken = GetToken(authClaims);

                    // Return Token
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        expiration = jwtToken.ValidTo
                    });
                }

            }
            return NotFound();
        }
        [HttpGet]
        [Route("Token")]
        public async Task<IActionResult> GetTokenLogin2FA(string UserName)
        {
            var user = await UserManager.FindByNameAsync(UserName);
            if (user != null)
                {
                    // claim list creation
                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                    // we add roles to the list
                    var userRoles = await UserManager.GetRolesAsync(user);
                    foreach (var role in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    // Generation the token with the claims
                    var jwtToken = GetToken(authClaims);

                    // Return Token
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        expiration = jwtToken.ValidTo
                    });
                }
            return NotFound();
        }
        [HttpPost]
        [Route("TwoFactorAuth")]
        public async Task<IActionResult> Verifie(string token, string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user != null)
            {
                user.TwoFactorEnabled = true;
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Enable Two Factor Successfully" });
            }
            return NotFound();
        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail(MessageRequest message)
        {
            try
            {
                var mess = new Message(new string[] { message.To }, message.Subject, message.Content);

                _emailService.SendEmail(mess);

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email Sent SuccessFully" });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await UserManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email verified is Successfully" });
                }
            }
            return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "This user Does Not Exit!" });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }

    }
}
