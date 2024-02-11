using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
_db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        async Task<LoginResponseDto> IAuthService.Login(LoginRequestDto loginRequestDto)
        {
            var user= _db.ApplicationUsers.FirstOrDefault(u=> u.UserName.ToLower()==loginRequestDto.Username.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null || isValid == false)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""

                };
            }
            UserDto userDto = new()
                {
                    Email = user.Email,
                    ID = user.Id,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber

                };
             LoginResponseDto loginResponseDto = new LoginResponseDto()
                {
                    User = userDto,
                    Token = "",
                };
                return loginResponseDto;
            
        }

        async Task<String> IAuthService.Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber,
            };
            try {
            var result=await  _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u =>  u.UserName == registrationRequestDto.Email);
                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber

                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
                    }
            catch(Exception ex) 
            {
                return ex.Message;
            }
            return "Error found";
        }
    }
}
