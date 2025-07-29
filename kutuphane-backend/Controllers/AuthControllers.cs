using Microsoft.AspNetCore.Mvc;
using Kutuphane.Models;
using Kutuphane.Models.DTOs;
using Kutuphane.Repositories.Interfaces;
using Kutuphane.Services;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Kutuphane.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IKullaniciRepository _kullaniciRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IKullaniciRepository kullaniciRepository,
            IJwtService jwtService,
            ILogger<AuthController> logger)
        {
            _kullaniciRepository = kullaniciRepository;
            _jwtService = jwtService;
            _logger = logger;
        }
        private string HashPassword(string password)
        {
            using (var hmac = new HMACSHA256())
            {
               var salt = Convert.ToBase64String(hmac.Key);
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
                return Convert.ToBase64String(hash) + ":" + salt;
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                var parts = hashedPassword.Split(':');
                if (parts.Length != 2) return false;
                
                var hash = parts[0];
                var salt = parts[1];
                
                using (var hmac = new HMACSHA256(Convert.FromBase64String(salt)))
                {
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
                    return Convert.ToBase64String(computedHash) == hash;
                }
            }
            catch
            {
                return false;
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
        {
            _logger.LogInformation("Giriş denemesi: {Email}", request.Email);

            try
            {
                var kullanicilar = await _kullaniciRepository.GetAllAsync();
                var kullanici = kullanicilar.FirstOrDefault(k => k.Email == request.Email);

                if (kullanici == null)
                {
                    _logger.LogWarning("Kullanıcı bulunamadı: {Email}", request.Email);
                    return Unauthorized("Email veya şifre hatalı");
                }
                if (!VerifyPassword(request.Password, kullanici.PasswordHash))
                {
                    _logger.LogWarning("Yanlış şifre: {Email}", request.Email);
                    return Unauthorized("Email veya şifre hatalı");
                }


                // JWT token oluştur
                var token = _jwtService.GenerateToken(
                kullanici.Id.ToString(),
                kullanici.Email,
                kullanici.Role);

                 _logger.LogInformation("Başarılı giriş: {Email}, Role: {Role}", request.Email, kullanici.Role);

                return Ok(new LoginResponseDto
                {
                    Token = token,
                    Email = kullanici.Email,
                    Ad = kullanici.Ad,
                    Soyad = kullanici.Soyad,
                    Role = kullanici.Role
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Giriş sırasında hata: {Email}", request.Email);
                return StatusCode(500, "Giriş işlemi sırasında bir hata oluştu");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto request)
        {
            _logger.LogInformation("Kayıt denemesi: {Email}", request.Email);

            try
            {
                
                var mevcutKullanicilar = await _kullaniciRepository.GetAllAsync();
                var mevcutKullanici = mevcutKullanicilar.FirstOrDefault(k => k.Email == request.Email);

                if (mevcutKullanici != null)
                {
                    _logger.LogWarning("Email zaten kayıtlı: {Email}", request.Email);
                    return BadRequest("Bu email zaten kayıtlı");
                }

                var hashedPassword = HashPassword(request.Password);


                // Yeni kullanıcı oluştur
                var kullanici = new Kullanici
                {
                    Ad = request.Ad,
                    Soyad = request.Soyad,
                    Email = request.Email,
                    Telefon = request.Telefon,
                    DogumTarihi = request.DogumTarihi,
                    PasswordHash = hashedPassword,
                    Role = "User",
                    ToplamOduncSayisi = 0,
                    AktifMi = true
                };

                var createdKullanici = await _kullaniciRepository.AddAsync(kullanici);
                _logger.LogInformation("Yeni kullanıcı kaydedildi: {Email}, ID: {Id},  Rol: {Role}" , request.Email, createdKullanici.Id, createdKullanici.Role);

                return Ok(new RegisterResponseDto
                {
                    Success = true,
                    Message = "Kayıt başarılı",
                    UserId = createdKullanici.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kayıt sırasında hata: {Email}", request.Email);
                return StatusCode(500, "Kayıt işlemi sırasında bir hata oluştu");
            }
        }
    }
}