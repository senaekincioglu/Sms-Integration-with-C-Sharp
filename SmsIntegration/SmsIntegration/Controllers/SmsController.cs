using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmsIntegration.Models;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using SmsIntegration.Data;

namespace SmsIntegration.Controllers
{
    public class SmsController : Controller

    {
        private readonly ApplicationDbContext _context;

        public SmsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Telefon numarasını normalize eden yardımcı yöntem
        private string NormalizePhoneNumber(string phoneNumber)
        {
            if (phoneNumber.StartsWith("0"))
            {
                phoneNumber = "+90" + phoneNumber.Substring(1);
            }
            else if (phoneNumber.StartsWith("5"))
            {
                phoneNumber = "+90" + phoneNumber;
            }
            return phoneNumber;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                // Telefon numarasını normalize et
                user.PhoneNumber = NormalizePhoneNumber(user.PhoneNumber);

                // Kullanıcı zaten veritabanında varsa
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);
                if (existingUser != null)
                {
                    // Kullanıcı doğrulanmadıysa, yeni bir doğrulama kodu gönder
                    if (!existingUser.IsVerified)
                    {
                        existingUser.VerificationCode = new Random().Next(100000, 999999).ToString();
                        await _context.SaveChangesAsync();
                        user = existingUser; // var olan kullanıcıyı güncelle
                    }
                    else
                    {
                        // Kullanıcı zaten doğrulanmış, yeniden kod göndermeye gerek yok
                        return RedirectToAction("VerifiedNumber");
                    }
                }
                else
                {
                    // Yeni kullanıcı oluşturuluyor
                    var verificationCode = new Random().Next(100000, 999999).ToString();
                    user.VerificationCode = verificationCode;
                    user.IsVerified = false;

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                // Twilio ayarlarını al
                var settings = await _context.TwilioSettings.FirstOrDefaultAsync();
                if (settings == null)
                {
                    return Content("Twilio settings not found.");
                }

                // SMS gönder
                TwilioClient.Init(settings.AccountSid, settings.AuthToken);

                var to = new PhoneNumber(user.PhoneNumber);
                var from = new PhoneNumber(settings.TwilioPhoneNumber);
                var message = MessageResource.Create(
                    to: to,
                    from: from,
                    body: $"Your verification code is: {user.VerificationCode}"
                );

                return RedirectToAction("Verify", new { phoneNumber = user.PhoneNumber });
            }
            catch (Exception ex)
            {
                return Content($"An error occurred: {ex.Message}");
            }
        }

        // GET: /Sms/Verify
        public IActionResult Verify(string phoneNumber)
        {
            ViewBag.PhoneNumber = phoneNumber;
            return View();
        }


        // POST: /Sms/Verify
        [HttpPost]
        public async Task<IActionResult> Verify(string phoneNumber, string verificationCode)
        {
            // Telefon numarasını normalize et
            phoneNumber = NormalizePhoneNumber(phoneNumber);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found.";
                return View();
            }

            if (string.IsNullOrEmpty(verificationCode))
            {
                ViewBag.ErrorMessage = "Verification code cannot be empty.";
                return View();
            }

            if (user.VerificationCode == verificationCode)
            {
                user.IsVerified = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Success");
            }

            ViewBag.ErrorMessage = "Invalid verification code.";
            return View();
        }


        public IActionResult Success() 
        {
            return View();
        }

    }


}

    

