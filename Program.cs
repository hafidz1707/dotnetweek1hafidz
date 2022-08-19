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

using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

OpenApiSecurityScheme securityScheme = new OpenApiSecurityScheme()
{
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "JSON Web Token based security",
};

OpenApiSecurityRequirement securityReq = new OpenApiSecurityRequirement()
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

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis"];
    options.InstanceName = "local-redis";
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
// Cached Database
// get All Users
app.MapGet("/usersRedis", [Authorize] async Task<object> (IDistributedCache cache, AppDbContext db) =>
{
    var response = await cache.GetUsers(db);
    return response;
});
// get All Services List
app.MapGet("/servicesRedis", [Authorize] async Task<object> (IDistributedCache cache, AppDbContext db) =>
{
    var response = await cache.GetServiceLists(db);
    return response;
});

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

        Profile profile = new Profile{name = result.name, email = result.email, phone = result.phone};
        Access_token access_Token = new Access_token{auth_token = token, type = "Bearer", expires_at = authToken.expiredAt};
        Data data = new Data {id = result.id, is_registered = result.is_registered, profile = profile, access_token = access_Token};
        Response<Data> response = new Response<Data> {data = data, success = true, message = "Login Berhasil!"};
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

    UserChanger? userChanger = new UserChanger{id = result.id, email = editUser.email, phone = editUser.phone};

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
            result.phone = userChanger.phone;
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
app.MapPost("/profile/change-password", [Authorize] async (ChangePassword changePassword, IVerifyAccount verification, AppDbContext db, HttpContext httpContext) =>
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

// Folder 'Service Registration'
// Get All Service Lists
app.MapGet("/service-registration/all-list", [Authorize] async(AppDbContext db, HttpContext httpContext) =>
{
    Token tokenData = new Jwt().GetTokenClaim(httpContext);
    return await db.ServiceLists.ToListAsync();
});
// Register Walk In
app.MapPost("service-registration/register-walkin", [Authorize] async (RegisterWalkIn registerWalkIn, AppDbContext db) =>
{
    ServiceList serviceList = new ServiceList{
        reg_number = "100170-SREG-{idFromSecurity}",
        queue_number = "NonBook-{idFromToday}",
        plate_number = registerWalkIn.plate_number,
        input_source_id = 2,
        input_source = "Walk In",
        create_time = registerWalkIn.time_stamp,
        booking_date_time = registerWalkIn.time_stamp};
    db.ServiceLists.Add(serviceList);
    await db.SaveChangesAsync();
    Response<ServiceList> response = new Response<ServiceList>{data = serviceList, success = true,
    message = $"Plat Nomor {registerWalkIn.plate_number} Berhasil dimasukkan ke antrian service dengan Nomor Antrian NonBook-(idFromToday)"};
    return Results.Ok(response);
});

// // Register From Booking
app.MapPost("service-registration/register-from-booking", [Authorize] async (RegisterBooking registerBooking, AppDbContext db) =>
{
    ServiceList serviceList = new ServiceList{
        reg_number = "100170-SREG-{idFromSecurity}",
        queue_number = "Book-{idFromToday}",
        name = registerBooking.name,
        plate_number = registerBooking.plate_number,
        input_source_id = 1,
        input_source = "Booking",
        is_vip = registerBooking.is_vip,
        create_time = registerBooking.time_stamp,
        booking_date_time = registerBooking.time_stamp};
    db.ServiceLists.Add(serviceList);
    await db.SaveChangesAsync();
    Response<ServiceList> response = new Response<ServiceList>{data = serviceList, success = true,
    message = $"Plat Nomor {registerBooking.plate_number} Berhasil dimasukkan ke antrian service dengan Nomor Antrian Book-(idFromToday)"};
    return Results.Ok(response);
});

// Get Service Registration
app.MapGet("/service-registration/list", [Authorize] async (DateTime? filterDate, AppDbContext db) =>
{
    // Filter Configuration
    DateTime filteredDate = filterDate ?? DateTime.Now.Date;
    
    List<ServiceList> vip = await db.ServiceLists.Where(item => item.create_time.Date == filteredDate && item.is_vip == "1" && item.status_id == 1).ToListAsync();
    List<ServiceList> booking = await db.ServiceLists.Where(item => item.create_time.Date == filteredDate && item.is_vip != "1" && item.input_source_id == 1 && item.status_id == 1).ToListAsync();
    List<ServiceList> walkIn = await db.ServiceLists.Where(item => item.create_time.Date == filteredDate && item.is_vip != "1" && item.input_source_id == 2 && item.status_id == 1).ToListAsync();
    List<ServiceList> progress = await db.ServiceLists.Where(item => item.create_time.Date == filteredDate && (item.status_id == 2 || item.status_id == 3)).ToListAsync();
    ResponseSRList responseSRList = new ResponseSRList
    {
        vip = vip,
        booking = booking,
        walk_in = walkIn,
        progress = progress
    };
    Response<ResponseSRList> response = new Response<ResponseSRList> {data = responseSRList, success = true, message = "Data Successfully Retrieved"};
    return Results.Ok(response);
});

// // Get Service Booking Statistic
app.MapGet("/service-registration/statistic", [Authorize] async (DateTime? filterDateTime, AppDbContext db) =>
{
    // Filter Configuration
    DateTime filteredDateTime = filterDateTime ?? DateTime.Now;
    DateTime filteredDate = filteredDateTime.Date;

    //List<ServiceList> vip = await db.ServiceLists.Where(item => item.create_time.Date == filteredDate && item.is_vip == "1" && item.status_id <= 3).ToListAsync();
    //List<ServiceList> progress = await db.ServiceLists.Where(item => item.create_time.Date == filteredDate && (item.status_id == 2 || item.status_id == 3)).ToListAsync();
    List<ServiceList> bookingAndWalkIn = await db.ServiceLists.Where(item => item.create_time.Date == filteredDate && item.is_vip != "1" && (item.input_source_id == 1 || item.input_source_id == 2) && item.status_id == 1).ToListAsync();
    int special_customer = await db.ServiceLists.Where(item => item.create_time.Date == filteredDate && item.is_vip == "1" && item.status_id <= 3).CountAsync();
    int completed_customer = await db.ServiceLists.Where(item => item.create_time.Date == filteredDate && (item.status_id == 2 || item.status_id == 3)).CountAsync();

    // Count the Service List
    int customer_less15_min = 0;
    int customer_more15_min = 0;
    foreach (ServiceList element in bookingAndWalkIn)
    {
        if (filteredDateTime - element.create_time <= TimeSpan.FromMinutes(15)) customer_less15_min += 1;
        else if (filteredDateTime - element.create_time > TimeSpan.FromMinutes(15)) customer_more15_min += 1;
    }
    ResponseSRStatistic responseSRStatistic = new ResponseSRStatistic{
        special_customer = special_customer,
        customer_less15_min = customer_less15_min,
        customer_more15_min = customer_more15_min,
        completed_customer = completed_customer
    };
    Response<ResponseSRStatistic> response = new Response<ResponseSRStatistic>{data = responseSRStatistic, success = true, message = "Data Successfully Retrieved"};
    return Results.Ok(response);
});

// Update Service Status
app.MapPost("service-registration/update-service-status", [Authorize] async (UpdateServiceStatus updateServiceStatus, AppDbContext db) =>
{
    ServiceList? result = await db.ServiceLists.Where(item => item.id == updateServiceStatus.service_registration_id).FirstOrDefaultAsync();
    Console.WriteLine(updateServiceStatus.service_status);
    if (result is null) return Results.NotFound();
    if (result.status_id == (int)SRStatus.Waiting_For_Service)
    {
        if (updateServiceStatus.service_status != (int)SRStatus.SA_Checking && updateServiceStatus.service_status != (int)SRStatus.Canceled) return Results.BadRequest("Status ID is Not Correct!");
        result.waiting_time = DateTime.Now - result.booking_date_time;
        result.status_id = updateServiceStatus.service_status;
        result.status = "SA Checking";
    }
    else if (result.status_id == (int)SRStatus.SA_Checking)
    {
        if (updateServiceStatus.service_status != (int)SRStatus.In_Progress_Service) return Results.BadRequest("Status ID is Not Correct!");
        result.status_id = updateServiceStatus.service_status;
        result.status = "In Progress Service";
    }
    // Status 4 is commented because only Mechanic has the privilige to complete the service
    // else if (result.status_id == (int)SRStatus.In_Progress_Service)
    // {
    //     if (updateServiceStatus.service_status != 4) return Results.BadRequest("Status ID is Not Correct!");
    //     result.status_id = updateServiceStatus.service_status;
    //     result.status = "Completed";
    // }
    else {
        return Results.BadRequest("Status ID is Not Correct!");
    }
    await db.SaveChangesAsync();
    Response<ServiceList> response = new Response<ServiceList>{success = true, message = "Status DMS berhasil diupdate"}; 
    return Results.Ok(response);
});

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.Run();

public interface IVerifyAccount
{
    Task SendOTPAsync(int id, string pin_otp, User user);
}
internal static class DistribuitedCacheExtentions
{
    public static async Task SetRecordAsync<T>(
            this IDistributedCache cache,
            string recordId,
            T data,
            TimeSpan? absoluteExpireTime = null)
    {
        var options = new DistributedCacheEntryOptions();
        options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);
        var jsonData = JsonSerializer.Serialize(data);
        await cache.SetStringAsync(recordId, jsonData, options);
    }
    public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
    {
        var jsonData = await cache.GetStringAsync(recordId);
        if (jsonData is null) return default(T);
        var dataResponse = JsonSerializer.Deserialize<T>(jsonData);
        return dataResponse;
    }
}

internal static class LoadDatasInDatabase
{
    public static async Task<object> GetUsers(this IDistributedCache cache, AppDbContext db)
    {
        List<User>? users = null; //Declaring empty forecasts
        string recordKey = $"users_{DateTime.Now.ToString("yyyyMMdd_hhmm")}"; //Declaring a unit recordKey to set our get the data
        users = await cache.GetRecordAsync<List<User>>(recordKey);
        if (users != null)
        {
            return Results.Ok(users);
        }
        List<User>? result = await db.Users.ToListAsync();
        await cache.SetRecordAsync(recordKey, result);
        return Results.Ok(result);
    }
    public static async Task<object> GetServiceLists(this IDistributedCache cache, AppDbContext db)
    {
        List<ServiceList>? serviceLists = null;
        string recordKey = $"serviceLists_{DateTime.Now.ToString("yyyyMMdd_hhmm")}";
        serviceLists = await cache.GetRecordAsync<List<ServiceList>>(recordKey);
        if (serviceLists != null)
        {
            return Results.Ok(serviceLists);
        }
        List<ServiceList>? result = await db.ServiceLists.ToListAsync();
        await cache.SetRecordAsync(recordKey, result);
        return Results.Ok(result);
    }
}