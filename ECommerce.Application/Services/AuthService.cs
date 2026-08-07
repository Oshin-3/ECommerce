using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using ECommerce.Domain.Contants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<IdentityResult> RegisterAsync(RegisterRequestDto request)
        {
            //check the user exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Email already exists."
                    });
            }

            //if not create the user
            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email
            };

            //user created
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return result;

            //Console.WriteLine($"Password entered: {request.Password}");
            //Console.WriteLine($"Generated Hash: {user.PasswordHash}");
            //Console.WriteLine($"Succeeded: {result.Succeeded}");

            //var verify = new PasswordHasher<ApplicationUser>()
            //    .VerifyHashedPassword(user, user.PasswordHash!, request.Password);

            //Console.WriteLine($"Verification: {verify}");

            //add role
            var resultRole = await _userManager.AddToRoleAsync(user, Roles.Customer);
            if (!resultRole.Succeeded)
                return resultRole;

            return IdentityResult.Success;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            //check if the user exists
            var user = await _userManager.FindByEmailAsync(request.Email);
            if(user == null)
            {
                //if not exists return error
                //return IdentityResult.Failed(new IdentityError
                //{
                //    Description = "User does not exists please register first!"
                //});
                return null;
            }
            //var hasher = new PasswordHasher<ApplicationUser>();

            //var verificationResult = hasher.VerifyHashedPassword(
            //    user,
            //    user.PasswordHash!,
            //    request.Password);

            //check password
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordCorrect)
            {
                //return IdentityResult.Failed(new IdentityError
                //{
                //    Description = "Invalid email or password."
                //});
                return null;
            }

            //fetch roles
            var roles = await _userManager.GetRolesAsync(user);
            //generate token
            var token = _jwtTokenGenerator.GenerateToken(user, roles);



            return new LoginResponseDto { Token = token };
        }
    }
}
