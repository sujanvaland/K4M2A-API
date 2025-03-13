using Microsoft.IdentityModel.Tokens;
using K4M2A.Entities.CommonModel;
using K4M2A.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using K4M2A.Common;
using K4M2A.AdminApi.Model;

namespace K4M2A.AdminApi.Services
{
    public interface IUserService
    {
        Task<JsonResponse> SignIn(string username, string password);
    }
    
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IConfiguration _configuration;
        public UserService(IRepository<User> userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        public async Task<JsonResponse> SignIn(string username, string password)
        {
            User user = await _userRepository.Table.Where(x=>x.UserName == "sagar").FirstOrDefaultAsync();
            if (user == null)
            {
                return new JsonResponse(204, true, "Not Exist", null);
            }

            bool isAuthenticated = false;

            if (user.UserName == username)
            {
                user = await Authenticate(username, password);
                if (user != null)
                {
                    isAuthenticated = true;
                }
                else
                {
                    return new JsonResponse(200, true, "UnAuthenticated", new LoginResponse());
                }
            }
           

            if (isAuthenticated)
            {
                //var profileModal = _profileService.GetUserProfile(user);
                var authClaims = new List<Claim>
                {
                    new Claim("Username", username),
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Exp", DateTime.Now.AddMonths(1).ToString())
                };
                var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMonths(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature)
                );

                return new JsonResponse(200, true, "Success", new LoginResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                   // Profile = profileModal
                });
            }

            return new JsonResponse(200, true, "UnAuthenticated", new LoginResponse());
        }

        private async Task<User> Authenticate(string username,string password)
        {
            try
            {
                var data = await _userRepository.Table
                    .Where(x => x.UserName.ToLower() == username.ToLower()
                    || x.Email.ToLower() == username.ToLower()).FirstOrDefaultAsync();

                if (data != null)
                {
                    var passwordMatch = false;
                    passwordMatch = PasswordHelper.VerifyPassword(password, data.Password);
                    
                    if (passwordMatch)
                    {
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
