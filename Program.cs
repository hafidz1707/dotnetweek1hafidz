using Microsoft.EntityFrameworkCore;
using System.Text;
using WeekOneApi.Infrastructure.Data.Models;
using WeekOneApi.Infrastructure.Services;
using WeekOneApi.Infrastructure.Data;
using WeekOneApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Depedencies.ConfigureService(builder.Configuration, builder.Services);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<OtpSettings>(builder.Configuration.GetSection("OtpSetting"));
builder.Services.AddSingleton<IVerifyAccount, VerifyAccount>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.MapPost("/auth/register", async (User user, AppDbContext db) =>
{
    User? result = await db.Users.Where(item => item.username == user.username).FirstOrDefaultAsync();
    if (result == null)
    {
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

app.MapGet("/auth/list", async (AppDbContext db) => await db.Users.Select(user => new UserDTO(user)).ToListAsync());

app.MapGet("/auth/list/{id}", async (int id, AppDbContext db) =>
    await db.Users.FindAsync(id)
        is User user
            ? Results.Ok(user)
            : Results.NotFound()
);

app.MapPut("/auth/list/{id}", async (int id, User editUser, AppDbContext db) =>
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

app.MapDelete("/auth/list/{id}", async (int id, AppDbContext db) =>
{
    User? user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();
    db.Users.Remove(user);
    await db.SaveChangesAsync();
    ResponseNoData responseNoData = new ResponseNoData{success = true, message = "Akun Berhasil dihapus!"};
    return Results.Ok();
});

app.MapPost("/auth/login", async (User user, AppDbContext db) =>
{
    User? result = await db.Users.Where(item => item.username == user.username).FirstOrDefaultAsync();
    if (result == null)
    {
        return Results.BadRequest("Username Tidak Ditemukan");
    }
    else
    {
        if (result.password == user.password && result.is_registered == true)
        {
            Encoding ASCII = Encoding.ASCII;
            var dateNow = DateTime.UtcNow;
            var random = new Random();
            var token = Convert.ToBase64String(ASCII.GetBytes(
                "" + random.Next() + dateNow
            ));

            Profile profile = new Profile{name = result.name, email = result.email, phone = result.no_hp};
            Access_token access_Token = new Access_token{auth_token = token};
            Data data = new Data {id = result.id, is_registered = result.is_registered, profile = profile, access_token = access_Token};
            Response response = new Response {data = data, success = true, message = "Login Berhasil!"};

            return Results.Ok(response);
        }
        else
        {
            ResponseNoData responseNoData = new ResponseNoData {success = false, message = "Login Gagal!"};
            return Results.Ok(responseNoData);
        }
    }
});

app.MapPost("/auth/forgot-password", async (User user, AppDbContext db) =>
{
    User? result = await db.Users.Where(item => item.username == user.username).FirstOrDefaultAsync();
    if (result == null)
    {
        return Results.BadRequest("Username Tidak Ditemukan");
    }
    else
    {
        Encoding ASCII = Encoding.ASCII;
        var randomPassword = Convert.ToBase64String(ASCII.GetBytes("" + new Random().Next(1000,9999)));
        result.password = randomPassword;
        await db.SaveChangesAsync();
        ResponseNoData responseNoData = new ResponseNoData{success = true, message = "Password berhasil dirubah menjadi: "+ randomPassword};
        return Results.Ok(responseNoData);
    }
});

app.MapPost("/auth/verify/{id}", async (int id, IVerifyAccount verification, AppDbContext db) =>
{
    int pin_otp = new Random().Next(100000, 999999);
    User? user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();
    user.pin_otp = pin_otp;
    await db.SaveChangesAsync();
    await verification.SendOTPAsync(id, pin_otp, user);
    ResponseNoData responseNoData = new ResponseNoData{success = true, message = "Kode OTP Telah Dikirim!"};
    return Results.Ok(responseNoData);
});

app.MapPost("/auth/resend-otp/{id}", async (int id, IVerifyAccount verification, AppDbContext db) =>
{
    int pin_otp = new Random().Next(100000, 999999);
    User? user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();
    user.pin_otp = pin_otp;
    await db.SaveChangesAsync();
    await verification.SendOTPAsync(id, pin_otp, user);
    ResponseNoData responseNoData = new ResponseNoData{success = true, message = "Kode OTP Telah Dikirim Ulang!"};
    return Results.Ok(responseNoData);
});

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

app.Run();

public interface IVerifyAccount
{
    Task SendOTPAsync(int id, int pin_otp, User user);
}
