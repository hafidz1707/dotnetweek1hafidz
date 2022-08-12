using Microsoft.EntityFrameworkCore;
using System.Text;
using WeekOneApi.Infrastructure.Data.Models;
using WeekOneApi.Infrastructure.Services;
using WeekOneApi.Infrastructure.Data;
using WeekOneApi.Infrastructure;
using WeekOneApi.Infrastructure.Shared;
//using Microsoft.DotNet.Scaffolding.Shared.Messaging;
//using NuGet.Protocol.Plugins;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;

var securityScheme = new OpenApiSecurityScheme()
{
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "JSON Web Token based security",
};

var securityReq = new OpenApiSecurityRequirement()
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
};

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(securityReq);
});

Depedencies.ConfigureService(builder.Configuration, builder.Services);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<OtpSettings>(builder.Configuration.GetSection("OtpSetting"));
builder.Services.AddSingleton<IVerifyAccount, VerifyAccount>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = false,
        ValidateAudience = false,
        RequireExpirationTime = true
    };
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// Get All Users
app.MapGet("/auth/list", [Authorize] async(AppDbContext db, HttpContext httpContext) =>
{
    Token tokenData = new Jwt().GetTokenClaim(httpContext);
    return await db.Users.Select(user => new UserDTO(user)).ToListAsync();
});
// Edit Users Based on ID
app.MapPut("/auth/list/{id}", [Authorize] async (int id, User editUser, AppDbContext db) =>
{
    User? user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();
    user.name = editUser.name;
    user.dealer_code = editUser.dealer_code;
    user.dealer_name = editUser.dealer_name;
    user.position_code = editUser.position_code;
    user.position_name = editUser.position_name;
    ResponseNoData responseNoData = new ResponseNoData{success = true, message = "Profil Berhasil diupdate!"};
    await db.SaveChangesAsync();
    return Results.Ok(responseNoData);
});
// Delete Users Based on ID
app.MapDelete("/auth/list/{id}", async (int id, AppDbContext db) =>
{
    User? user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();
    db.Users.Remove(user);
    await db.SaveChangesAsync();
    ResponseNoData responseNoData = new ResponseNoData{success = true, message = "Akun Berhasil dihapus!"};
    return Results.Ok();
});

// Folder 'Auth'
// Register Account
app.MapPost("/auth/register", async (User user, AppDbContext db) =>
{
    User? result = await db.Users.Where(item => item.username == user.username).FirstOrDefaultAsync();
    if (result == null)
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.password);
        user.password = passwordHash;
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Results.Created($"/auth/list/{user.id}", user);
    }
    else
    {
        ResponseNoData responseNoData = new ResponseNoData{success = false, message = "Username Sudah Digunakan!"};
        return Results.BadRequest(responseNoData);
    }
});
// Login Account Get Token
app.MapPost("/auth/login", async (User user, AppDbContext db) =>
{
    User? result = await db.Users.Where(item => item.username == user.username).FirstOrDefaultAsync();
    if (result == null)
    {
        return Results.BadRequest("Username Tidak Ditemukan!");
    }
    else
    {
        bool verifyPassword = BCrypt.Net.BCrypt.Verify(user.password, result.password);
        if (!verifyPassword)
        {
            return Results.BadRequest("Password Salah!");
        }

        string token = new Jwt().GenerateJwtToken(result);
        Int32 expiredAt = (Int32)DateTime.UtcNow.AddDays(1).Subtract(new DateTime(2022, 08, 12)).TotalSeconds;
        AuthToken authToken = new AuthToken
        {
            userId = result.id,
            role = "admin",
            expiredAt = expiredAt,
            token = token,
        };
        db.AuthTokens.Add(authToken);
        await db.SaveChangesAsync();

        Profile profile = new Profile{name = result.name, email = result.email, phone = result.no_hp};
        Access_token access_Token = new Access_token{auth_token = token, type = "Bearer", expires_at = authToken.expiredAt};
        Data data = new Data {id = result.id, is_registered = result.is_registered, profile = profile, access_token = access_Token};
        Response response = new Response {data = data, success = true, message = "Login Berhasil!"};
        return Results.Ok(response);
    }
});
// Forgot Password sent to Email & Phone
app.MapPost("/auth/forgot-password", async (User user, IVerifyAccount verification, AppDbContext db) =>
{
    User? result = await db.Users.Where(item => item.username == user.username).FirstOrDefaultAsync();
    if (result == null)
    {
        return Results.BadRequest("Username Tidak Ditemukan");
    }
    else
    {
        Encoding ASCII = Encoding.ASCII;
        string generatedPassword = Convert.ToBase64String(ASCII.GetBytes("" + new Random().Next(100000,999999)));
        string randomPassword = generatedPassword.Substring(0,8);
        Console.WriteLine(generatedPassword);
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(randomPassword);
        result.password = passwordHash;
        await db.SaveChangesAsync();
        await verification.SendOTPAsync(result.id, randomPassword, result);

        ResponseNoData responseNoData = new ResponseNoData{success = true, message = "Password berhasil dirubah menjadi: "+ randomPassword};
        return Results.Ok(responseNoData);
    }
});
// Verify Account
app.MapPost("/auth/verify/", async (RequestId requestId, IVerifyAccount verification, AppDbContext db) =>
{
    int pin_otp = new Random().Next(100000, 999999);
    User? user = await db.Users.FindAsync(requestId.id);
    if (user is null) return Results.NotFound();

    user.pin_otp = pin_otp;
    await db.SaveChangesAsync();
    await verification.SendOTPAsync(requestId.id, pin_otp.ToString(), user);
    
    ResponseNoData responseNoData = new ResponseNoData{success = true, message = "Kode OTP Telah Dikirim!"};
    return Results.Ok(responseNoData);
});
// Resend OTP
app.MapPost("/auth/resend-otp/", async (RequestId requestId, IVerifyAccount verification, AppDbContext db) =>
{
    int pin_otp = new Random().Next(100000, 999999);
    User? user = await db.Users.FindAsync(requestId.id);
    if (user is null) return Results.NotFound();

    user.pin_otp = pin_otp;
    await db.SaveChangesAsync();
    await verification.SendOTPAsync(requestId.id, pin_otp.ToString(), user);
    
    ResponseNoData responseNoData = new ResponseNoData{success = true, message = "Kode OTP Telah Dikirim Ulang!"};
    return Results.Ok(responseNoData);
});
// Confirmation OTP
app.MapPost("/auth/otp", async (OtpRequest otpRequest, AppDbContext db) =>
{
    User? result = await db.Users.Where(item => item.id == otpRequest.id).FirstOrDefaultAsync();
    if (result != null)
    {
        if (result.pin_otp == otpRequest.pin_otp)
        {
            result.is_registered = true;
            result.pin_otp = 0;
            await db.SaveChangesAsync();
            ResponseNoData responseNoData = new ResponseNoData{success = true, message = "Akun Berhasil Diaktifkan!"};
            return Results.Ok(responseNoData);
        }
        else 
        {
            ResponseNoData responseNoData = new ResponseNoData{success = false, message = "Kode OTP Salah!"};
            return Results.Ok(responseNoData);
        }
    }
    return Results.BadRequest("User Tidak Ditemukan!");
});

// Folder 'Profile'
// Get Detail of Profile
app.MapGet("/profile/detail/", [Authorize] async (AppDbContext db, HttpContext httpContext) =>
{
    Token tokenData = new Jwt().GetTokenClaim(httpContext);
    User? result = await db.Users.Where(item => item.id.ToString() == tokenData.id).FirstOrDefaultAsync();
    if (result == null) return Results.NotFound("Data Tidak Ditemukan!");
    return Results.Ok(result);
});
// Update Profile
app.MapPut("/profile/update", [Authorize] async (UserChanger editUser, AppDbContext db, IVerifyAccount verification, HttpContext httpContext) =>
{
    Token tokenData = new Jwt().GetTokenClaim(httpContext);
    User? result = await db.Users.Where(item => item.id.ToString() == tokenData.id).FirstOrDefaultAsync();
    if (result is null) return Results.NotFound();
    if (result == null) return Results.NotFound("Data Tidak Ditemukan!");
    
    bool verifyPassword = BCrypt.Net.BCrypt.Verify(editUser.password, result.password);
    if (!verifyPassword) return Results.BadRequest("Password Salah!");

    UserChanger? checkChanger = await db.UsersChanger.Where(item => item.id == result.id).FirstOrDefaultAsync();
    if (checkChanger != null ) db.UsersChanger.Remove(checkChanger);

    UserChanger? userChanger = new UserChanger{id = result.id, email = editUser.email, no_hp = editUser.no_hp};

    db.UsersChanger.Add(userChanger);
    await db.SaveChangesAsync();

    int pin_otp = new Random().Next(100000, 999999);
    result.pin_otp = pin_otp;
    await db.SaveChangesAsync();
    await verification.SendOTPAsync(result.id, pin_otp.ToString(), result);
    ResponseNoData responseNoData = new ResponseNoData{success = true, message = "Kode OTP Telah Dikirim!"};
    return Results.Ok(responseNoData);
});
// Resend OTP Profile
app.MapPost("/profile/otp-resend", [Authorize] async (RequestId requestId, AppDbContext db, IVerifyAccount verification, HttpContext httpContext) =>
{
    Token tokenData = new Jwt().GetTokenClaim(httpContext);
    User? result = await db.Users.Where(item => item.id.ToString() == tokenData.id).FirstOrDefaultAsync();
    if (result is null) return Results.NotFound();
    if (result == null) return Results.NotFound("Data Tidak Ditemukan!");

    int pin_otp = new Random().Next(100000, 999999);
    result.pin_otp = pin_otp;
    await db.SaveChangesAsync();
    await verification.SendOTPAsync(result.id, pin_otp.ToString(), result);
    ResponseNoData responseNoData = new ResponseNoData{success = true, message = "Kode OTP Telah Dikirim Ulang!"};
    return Results.Ok(responseNoData);
});
// Confirmation OTP to Change Profile
app.MapPost("/profile/otp", [Authorize] async (OtpRequest otpRequest, AppDbContext db) =>
{
    User? result = await db.Users.Where(item => item.id == otpRequest.id).FirstOrDefaultAsync();
    UserChanger? userChanger = await db.UsersChanger.Where(item => item.id == otpRequest.id).FirstOrDefaultAsync();
    Console.WriteLine(userChanger);
    if (userChanger != null)
    {
        if (result.pin_otp == otpRequest.pin_otp)
        {
            result.pin_otp = 0;
            result.email = userChanger.email;
            result.no_hp = userChanger.no_hp;
            db.UsersChanger.Remove(userChanger);
            await db.SaveChangesAsync();
            ResponseNoData responseNoData = new ResponseNoData{success = true, message = "Akun Berhasil Diupdate!"};
            return Results.Ok(responseNoData);
        }
        else 
        {
            ResponseNoData responseNoData = new ResponseNoData{success = false, message = "Kode OTP Salah!"};
            return Results.Ok(responseNoData);
        }
    }
    return Results.BadRequest("Data Update Profile Tidak Ditemukan!");
});
// Change Profile Password
app.MapPost("/profile/change-password", async (ChangePassword changePassword, IVerifyAccount verification, AppDbContext db, HttpContext httpContext) =>
{
    Token tokenData = new Jwt().GetTokenClaim(httpContext);
    User? result = await db.Users.Where(item => item.id.ToString() == tokenData.id).FirstOrDefaultAsync();
    if (result == null) return Results.BadRequest("Username Tidak Ditemukan");

    bool verifyPassword = BCrypt.Net.BCrypt.Verify(changePassword.current_password, result.password);
    if (!verifyPassword) return Results.BadRequest("Password Salah!");

    Encoding ASCII = Encoding.ASCII;
    if (changePassword.new_password != changePassword.new_password_confirmation) return Results.BadRequest("Password Baru Tidak Cocok!");
    string passwordHash = BCrypt.Net.BCrypt.HashPassword(changePassword.new_password);
    result.password = passwordHash;
    await db.SaveChangesAsync();

    ResponseNoData responseNoData = new ResponseNoData{success = true, message = "Password Berhasil Dirubah!"};
    return Results.Ok(responseNoData);
});

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.Run();

public interface IVerifyAccount
{
    Task SendOTPAsync(int id, string pin_otp, User user);
}
