using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using WeekOneApi.Infrastructure.Data.Models;
using WeekOneApi.Infrastructure.Data;

namespace WeekOneApi.Infrastructure.Services;

class VerifyAccount : IVerifyAccount
{
    private readonly OtpSettings _otpSettings;
    public VerifyAccount(IOptions<OtpSettings> otpSettings)
    {
        _otpSettings = otpSettings.Value;
    }
    private readonly AppDbContext db;
    
    public async Task SendOTPAsync(int id, string pin_otp, User user)
    {
        //Console.WriteLine("OTP Request ID:" + id);
        //Console.WriteLine("Pin OTP:" + pin_otp);
        Console.WriteLine(user);

        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://graph.facebook.com/v14.0/102428365909334/messages"),
            Headers =
            {
                { "Authorization", "Bearer "+_otpSettings.Token },
            },
            Content = new StringContent("{ \"messaging_product\": \"whatsapp\", \"recipient_type\": \"individual\", \"to\": \"6283863385061\", \"type\": \"text\", \"text\": {\"body\": \"Kode OTP Anda: " + pin_otp + "\" } }")
            {
                Headers =
                {
                    ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
                }
            }

        };
        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(body);
        }

        //Console.WriteLine(_otpSettings.Host);
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(_otpSettings.From);
        email.To.Add(MailboxAddress.Parse(user.email));
        if (pin_otp.Length == 6) email.Subject = "KODE OTP";
        else email.Subject = "Reset Password";
        
        var builder = new BodyBuilder();
        if (pin_otp.Length == 6) builder.HtmlBody = "Kode OTP Anda: "+pin_otp;
        else builder.HtmlBody = "Password Baru Anda: "+pin_otp;
        
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        smtp.Connect(_otpSettings.Host, _otpSettings.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_otpSettings.Username, _otpSettings.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
}