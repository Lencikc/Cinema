using CinemaAPI.Connection;
using CinemaAPI.Interfaces;
using CinemaAPI.Models;
using CinemaAPI.Requests;
using CinemaAPI.UniversalMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CinemaAPI.Services
{
    public class LoginService : ILoginService
    {
        private static readonly Regex _emailRegex = new(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+$", RegexOptions.Compiled);

        private readonly ContextDb _context;
        private readonly PasswordService _passwordService;
        private readonly JwtGenerator _jwtGenerator;

        public LoginService(ContextDb context, PasswordService passwordService, JwtGenerator jwtGenerator)
        {
            _context = context;
            _passwordService = passwordService;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (string.IsNullOrWhiteSpace(model.LastName) || string.IsNullOrWhiteSpace(model.FirstName) ||
                string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Заполните обязательные поля"
                });
            }

            if (!_emailRegex.IsMatch(model.Email))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Некорректный email"
                });
            }

            if (model.Password.Length < 6)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Пароль должен содержать минимум 6 символов"
                });
            }

            var existing = await _context.Logins.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (existing != null)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Пользователь с таким email уже зарегистрирован"
                });
            }

            var newUser = new User
            {
                LastName = model.LastName,
                FirstName = model.FirstName,
                Patronymic = model.Patronymic,
                Phone = model.Phone,
                BirthDate = model.BirthDate,
                RoleID = 3,
                Login = new Login
                {
                    Email = model.Email,
                    Password = _passwordService.HashPassword(model.Password)
                }
            };

            await _context.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Регистрация прошла успешно"
            });
        }

        public async Task<IActionResult> SignIn(SignInModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Введите email и пароль"
                });
            }

            var login = await _context.Logins.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (login == null)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Неверный email или пароль"
                });
            }

            if (!_passwordService.VerifyPassword(model.Password, login.Password))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Неверный email или пароль"
                });
            }

            var user = await _context.Users
                .Include(x => x.Role)
                .Include(x => x.Login)
                .FirstOrDefaultAsync(x => x.LoginID == login.LoginID);

            if (user == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Профиль не найден"
                });
            }

            var session = new Session
            {
                Token = _jwtGenerator.GenerateToken(user.UserID, user.RoleID),
                UserID = user.UserID,
                LogedAt = DateTime.UtcNow
            };

            await _context.AddAsync(session);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Успешная авторизация",
                token = session.Token,
                logedUser = new
                {
                    user.UserID,
                    user.LastName,
                    user.FirstName,
                    user.Patronymic,
                    user.Phone,
                    user.RoleID,
                    RoleName = user.Role.RoleName,
                    Email = user.Login.Email
                }
            });
        }

        public async Task<IActionResult> GetProfile(int userID)
        {
            var user = await _context.Users
                .Include(x => x.Role)
                .Include(x => x.Login)
                .FirstOrDefaultAsync(x => x.UserID == userID);

            if (user == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Профиль не найден"
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                profile = new
                {
                    user.UserID,
                    user.LastName,
                    user.FirstName,
                    user.Patronymic,
                    user.Phone,
                    user.RoleID,
                    RoleName = user.Role.RoleName,
                    Email = user.Login.Email
                }
            });
        }
    }
}
