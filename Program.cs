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
        Int32 expiredAt = (Int32)DateTime.UtcNow.AddDays(1).Subtract(new DateTime(2022, 08, 25)).TotalSeconds;
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

// Register From Booking
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

// Get Service Booking Statistic
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
        // Add to Circle Check
        CircleCheck circleCheck = new CircleCheck{
            service_registration_id = result.id,
            service_date = DateTime.Now,
            plate_number = result.plate_number,
            customer_name = result.name
            };
        db.CircleChecks.Add(circleCheck);
        await db.SaveChangesAsync();
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

// Folder 'Circle Check'
// Get Circle Check
app.MapGet("/circle-check/get-circle-check", [Authorize] async (int? UserRegistrationId, AppDbContext db) =>
{
    int userRegistrationId = UserRegistrationId ?? 0;
    CircleCheck? result = await db.CircleChecks.Where(item => item.service_registration_id == userRegistrationId)
        .Include(item => item.interior_view)
        .Include(item => item.complaint_notes_view)
        .Include(item => item.exterior_view)
        .Include(item => item.tire_view)
        .FirstOrDefaultAsync();
    if (result == null) return Results.NotFound();
    return Results.Ok(result);
});

// Save Circle Check
app.MapPost("/circle-check/save-circle-check", [Authorize] async (SaveCircleCheck saveCircleCheck, AppDbContext db) =>
{
    CircleCheck? result = await db.CircleChecks.Where(item => item.service_registration_id == saveCircleCheck.service_registration_id).FirstOrDefaultAsync();
    if (result != null)
    {
        result.customer_name = saveCircleCheck.customer_info?.customer_name;
        result.vin = saveCircleCheck.customer_info?.vin;
        result.phone = saveCircleCheck.customer_info?.phone;
        InteriorView interiorView = new InteriorView{
            circle_check_header_id = result.id,
            service_registration_id = result.service_registration_id,
            stnk = saveCircleCheck.interior.stnk,
            service_booklet = saveCircleCheck.interior.service_booklet,
            spare_tire = saveCircleCheck.interior.spare_tire,
            safety_kit = saveCircleCheck.interior.safety_kit,
            fuel_gauge = saveCircleCheck.interior.fuel_gauge,
            other_stuff = saveCircleCheck.interior.other_stuff,
            other_stuff_notes = saveCircleCheck.interior.other_stuff_notes
        };
        db.InteriorViews.Add(interiorView);
        ComplaintView complaintView = new ComplaintView{
            circle_check_header_id = result.id,
            service_registration_id = result.service_registration_id,
            notes = saveCircleCheck.complaint_notes
        };
        db.ComplaintViews.Add(complaintView);
        await db.SaveChangesAsync();
        return Results.Ok();
    }
    return Results.NotFound();
});

// Save Interior View Photo
app.MapPost("/circle-check/save-interior-view-photo", [Authorize] async (AppDbContext db, HttpRequest rtx) =>
{
    int.TryParse(rtx.Form["service_registration_id"], out int service_registration_id);
    var photo_1 = rtx.Form.Files["photo_1"];
    var photo_2 = rtx.Form.Files["photo_2"];
    var photo_3 = rtx.Form.Files["photo_3"];
    InteriorView? result = await db.InteriorViews.Where(item => item.service_registration_id == service_registration_id).FirstOrDefaultAsync();
    var extension_1 = new FileInfo(rtx.Form.Files["photo_1"].FileName);
    var filePath_1 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_Interior1_{extension_1.Extension}");
    
    using (var stream = System.IO.File.Create(filePath_1))
    {
        await rtx.Form.Files["photo_1"].CopyToAsync(stream);
    }
    result.interior_photo_1 = filePath_1;
    if (photo_2 != null)
    {
        var extension_2 = new FileInfo(rtx.Form.Files["photo_2"].FileName);
        var filePath_2 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_Interior2_{extension_2.Extension}");
        using (var stream = System.IO.File.Create(filePath_2))
        {
            await rtx.Form.Files["photo_2"].CopyToAsync(stream);
        }
        result.interior_photo_2 = filePath_2;
    }
    if (photo_3 != null) 
    {
        var extension_3 = new FileInfo(rtx.Form.Files["photo_3"].FileName);
        var filePath_3 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_Interior3_{extension_3.Extension}");
        using (var stream = System.IO.File.Create(filePath_3))
        {
            await rtx.Form.Files["photo_3"].CopyToAsync(stream);
        }
        result.interior_photo_3 = filePath_3;
    }
    await db.SaveChangesAsync();
    return Results.Ok(result);
});

// Delete Interior View
app.MapPost("/circle-check/delete-interior-view", [Authorize] async (int? ServiceRegistrationId, AppDbContext db) =>
{
    int serviceRegistrationId = ServiceRegistrationId ?? 0;
    InteriorView? result = await db.InteriorViews.Where(item => item.service_registration_id == serviceRegistrationId).FirstOrDefaultAsync();
    if (result == null) return Results.NotFound();
    db.InteriorViews.Remove(result);
    await db.SaveChangesAsync();
    return Results.Ok(result);
});

// Delete Complaint Note
app.MapPost("/circle-check/delete-complaint-note", [Authorize] async (int? ServiceRegistrationId, AppDbContext db) =>
{
    int serviceRegistrationId = ServiceRegistrationId ?? 0;
    ComplaintView? result = await db.ComplaintViews.Where(item => item.service_registration_id == serviceRegistrationId).FirstOrDefaultAsync();
    if (result == null) return Results.NotFound();
    db.ComplaintViews.Remove(result);
    await db.SaveChangesAsync();
    return Results.Ok(result);
});

// Create Update Exterior View
app.MapPost("/circle-check/create-update-exterior-view", [Authorize] async (AppDbContext db, HttpRequest rtx) =>
{
    int.TryParse(rtx.Form["service_registration_id"], out int service_registration_id);
    int.TryParse(rtx.Form["type"], out int type);
    int.TryParse(rtx.Form["vehicle_condition"], out int vehicle_condition);
    string? notes = rtx.Form["notes"];
    var image_path = rtx.Form.Files["capture_file"];
   
    int circle_check_header_id = await db.CircleChecks
        .Where(item => item.service_registration_id == service_registration_id)
        .Select(item => item.id)
        .FirstOrDefaultAsync();
    ExteriorView? result = await db.ExteriorViews
        .Where(item => item.circle_check_header_id == service_registration_id)
        .Where(item => item.data_type == type)
        .FirstOrDefaultAsync();
    if (result != null)
    {  
        result.vehicle_condition = vehicle_condition;
        result.notes = notes;
        var extension = new FileInfo(rtx.Form.Files["capture_file"].FileName);
        var filePath = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_Exterior_{extension.Extension}");
        using (var stream = System.IO.File.Create(filePath))
        {
            await rtx.Form.Files["capture_file"].CopyToAsync(stream);
        }
        result.image_path = filePath;
    }
    else
    {
        String? data_type_text = "";
        if (type == 1) data_type_text = "Front View";
        else if (type == 2) data_type_text = "Right View";
        else if (type == 3) data_type_text = "Left View";
        else if (type == 4) data_type_text = "Back View";
        else if (type == 5) data_type_text = "Top View";
        var extension = new FileInfo(rtx.Form.Files["capture_file"].FileName);
        var filePath = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_Exterior_{extension.Extension}");
        ExteriorView exteriorView = new ExteriorView
        {
            circle_check_header_id = circle_check_header_id,
            service_registration_id = service_registration_id,
            data_type = type,
            data_type_text = data_type_text,
            vehicle_condition = vehicle_condition,
            notes = notes,
            image_path = filePath
        };
        using (var stream = System.IO.File.Create(filePath))
        {
            await rtx.Form.Files["capture_file"].CopyToAsync(stream);
        }
        db.ExteriorViews.Add(exteriorView);
        await db.SaveChangesAsync();
        return Results.Ok(exteriorView);
    }
    await db.SaveChangesAsync();
    return Results.Ok(result);
});

// Get Exterior View
app.MapGet("/circle-check/get-exterior-view", [Authorize] async (int? exteriorId, AppDbContext db) =>
{
    int exteriorViewId = exteriorId ?? 0;
    ExteriorView? result = await db.ExteriorViews.Where(item => item.id == exteriorViewId).FirstOrDefaultAsync();
    if (result == null) return Results.NotFound();
    return Results.Ok(result);
});

// Get Exterior View List
app.MapGet("/circle-check/get-exterior-view-list", [Authorize] async (int? ServiceRegistrationId, AppDbContext db) =>
{
    int serviceRegistrationId = ServiceRegistrationId ?? 0;
    List<ExteriorView>? result = await db.ExteriorViews.Where(item => item.service_registration_id == serviceRegistrationId).ToListAsync();
    if (result == null) return Results.NotFound();
    return Results.Ok(result);
});

// Delete Exterior View
app.MapPost("/circle-check/delete-exterior-view", [Authorize] async (int? exteriorId, AppDbContext db) =>
{
    int exteriorViewId = exteriorId ?? 0;
    ExteriorView? result = await db.ExteriorViews.Where(item => item.id == exteriorViewId).FirstOrDefaultAsync();
    if (result == null) return Results.NotFound();
    db.ExteriorViews.Remove(result);
    await db.SaveChangesAsync();
    return Results.Ok(result);
});

// Create Update Tire View
app.MapPost("/circle-check/create-update-tire-view", [Authorize] async (AppDbContext db, HttpRequest rtx) =>
{
    int.TryParse(rtx.Form["service_registration_id"], out int service_registration_id);
    string front_right = rtx.Form["front_right"];
    string front_left = rtx.Form["front_left"];
    string back_right = rtx.Form["back_right"];
    string back_left = rtx.Form["back_left"];
    var extension = new FileInfo(rtx.Form.Files["front_right_photo_1"].FileName);

    int circle_check_header_id = await db.CircleChecks
        .Where(item => item.service_registration_id == service_registration_id)
        .Select(item => item.id)
        .FirstOrDefaultAsync();
    TireView? result = await db.TireViews
        .Where(item => item.circle_check_header_id == service_registration_id)
        .FirstOrDefaultAsync();
    
    
    if (result == null)
    {
        TireView tireView = new TireView
        {
            circle_check_header_id = circle_check_header_id,
            service_registration_id = service_registration_id,
            front_right = front_right,
            front_left = front_left,
            back_right = back_right,
            back_left = back_left
        };
        var filePath_frontRight_1 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireFR1_{extension.Extension}");
        var filePath_frontLeft_1 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireFL1_{extension.Extension}");
        var filePath_backRight_1 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireBR1_{extension.Extension}");
        var filePath_backLeft_1 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireBL1_{extension.Extension}");
        // Front Right
        using (var stream = System.IO.File.Create(filePath_frontRight_1))
        {
            await rtx.Form.Files["front_right_photo_1"].CopyToAsync(stream);
        }
        tireView.front_right_photo_1 = filePath_frontRight_1;
        if (rtx.Form.Files["front_right_photo_2"] != null)
        {
            var filePath_frontRight_2 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireFR2_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_frontRight_2))
            {
                await rtx.Form.Files["front_right_photo_2"].CopyToAsync(stream);
            }
            tireView.front_right_photo_2 = filePath_frontRight_2;
        }
        if (rtx.Form.Files["front_right_photo_3"] != null)
        {
            var filePath_frontRight_3 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireFR3_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_frontRight_3))
            {
                await rtx.Form.Files["front_right_photo_3"].CopyToAsync(stream);
            }
            tireView.front_right_photo_3 = filePath_frontRight_3;
        }
        // Front Left
        using (var stream = System.IO.File.Create(filePath_frontLeft_1))
        {
            await rtx.Form.Files["front_left_photo_1"].CopyToAsync(stream);
        }
        tireView.front_left_photo_1 = filePath_frontLeft_1;
        if (rtx.Form.Files["front_left_photo_2"] != null)
        {
            var filePath_frontLeft_2 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireFL2_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_frontLeft_2))
            {
                await rtx.Form.Files["front_left_photo_2"].CopyToAsync(stream);
            }
            tireView.front_left_photo_2 = filePath_frontLeft_2;
        }
        if (rtx.Form.Files["front_left_photo_3"] != null)
        {
            var filePath_frontLeft_3 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireFL3_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_frontLeft_3))
            {
                await rtx.Form.Files["front_left_photo_3"].CopyToAsync(stream);
            }
            tireView.front_left_photo_3 = filePath_frontLeft_3;
        }
        // Back Right
        using (var stream = System.IO.File.Create(filePath_backRight_1))
        {
            await rtx.Form.Files["back_right_photo_1"].CopyToAsync(stream);
        }
        tireView.back_right_photo_1 = filePath_backRight_1;
        if (rtx.Form.Files["back_right_photo_2"] != null)
        {
            var filePath_backRight_2 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireBR2_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_backRight_2))
            {
                await rtx.Form.Files["back_right_photo_2"].CopyToAsync(stream);
            }
            tireView.back_right_photo_2 = filePath_backRight_2;
        }
        if (rtx.Form.Files["back_right_photo_3"] != null)
        {
            var filePath_backRight_3 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireBR3_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_backRight_3))
            {
                await rtx.Form.Files["back_right_photo_3"].CopyToAsync(stream);
            }
            tireView.back_right_photo_3 = filePath_backRight_3;
        }
        // Back Left
        using (var stream = System.IO.File.Create(filePath_backLeft_1))
        {
            await rtx.Form.Files["back_left_photo_1"].CopyToAsync(stream);
        }
        tireView.back_left_photo_1 = filePath_backLeft_1;
        if (rtx.Form.Files["back_left_photo_2"] != null)
        {
            var filePath_backLeft_2 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireBL2_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_backLeft_2))
            {
                await rtx.Form.Files["back_left_photo_2"].CopyToAsync(stream);
            }
            tireView.back_left_photo_2 = filePath_backLeft_2;
        }
        if (rtx.Form.Files["back_left_photo_3"] != null)
        {
            var filePath_backLeft_3 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireBL3_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_backLeft_3))
            {
                await rtx.Form.Files["back_left_photo_3"].CopyToAsync(stream);
            }
            tireView.back_left_photo_3 = filePath_backLeft_3;
        }
        db.TireViews.Add(tireView);
        await db.SaveChangesAsync();
        return Results.Ok(tireView);
    }
    if (result != null)
    {
        var filePath_frontRight_1 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireFR1_{extension.Extension}");
        var filePath_frontLeft_1 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireFL1_{extension.Extension}");
        var filePath_backRight_1 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireBR1_{extension.Extension}");
        var filePath_backLeft_1 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireBL1_{extension.Extension}");
        // Front Right
        using (var stream = System.IO.File.Create(filePath_frontRight_1))
        {
            await rtx.Form.Files["front_right_photo_1"].CopyToAsync(stream);
        }
        result.front_right_photo_1 = filePath_frontRight_1;
        if (rtx.Form.Files["front_right_photo_2"] != null)
        {
            var filePath_frontRight_2 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireFR2_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_frontRight_2))
            {
                await rtx.Form.Files["front_right_photo_2"].CopyToAsync(stream);
            }
            result.front_right_photo_2 = filePath_frontRight_2;
        }
        if (rtx.Form.Files["front_right_photo_3"] != null)
        {
            var filePath_frontRight_3 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireFR3_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_frontRight_3))
            {
                await rtx.Form.Files["front_right_photo_3"].CopyToAsync(stream);
            }
            result.front_right_photo_3 = filePath_frontRight_3;
        }
        // Front Left
        using (var stream = System.IO.File.Create(filePath_frontLeft_1))
        {
            await rtx.Form.Files["front_left_photo_1"].CopyToAsync(stream);
        }
        result.front_left_photo_1 = filePath_frontLeft_1;
        if (rtx.Form.Files["front_left_photo_2"] != null)
        {
            var filePath_frontLeft_2 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireFL2_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_frontLeft_2))
            {
                await rtx.Form.Files["front_left_photo_2"].CopyToAsync(stream);
            }
            result.front_left_photo_2 = filePath_frontLeft_2;
        }
        if (rtx.Form.Files["front_left_photo_3"] != null)
        {
            var filePath_frontLeft_3 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireFL3_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_frontLeft_3))
            {
                await rtx.Form.Files["front_left_photo_3"].CopyToAsync(stream);
            }
            result.front_left_photo_3 = filePath_frontLeft_3;
        }
        // Back Right
        using (var stream = System.IO.File.Create(filePath_backRight_1))
        {
            await rtx.Form.Files["back_right_photo_1"].CopyToAsync(stream);
        }
        result.back_right_photo_1 = filePath_backRight_1;
        if (rtx.Form.Files["back_right_photo_2"] != null)
        {
            var filePath_backRight_2 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireBR2_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_backRight_2))
            {
                await rtx.Form.Files["back_right_photo_2"].CopyToAsync(stream);
            }
            result.back_right_photo_2 = filePath_backRight_2;
        }
        if (rtx.Form.Files["back_right_photo_3"] != null)
        {
            var filePath_backRight_3 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireBR3_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_backRight_3))
            {
                await rtx.Form.Files["back_right_photo_3"].CopyToAsync(stream);
            }
            result.back_right_photo_3 = filePath_backRight_3;
        }
        // Back Left
        using (var stream = System.IO.File.Create(filePath_backLeft_1))
        {
            await rtx.Form.Files["back_left_photo_1"].CopyToAsync(stream);
        }
        result.back_left_photo_1 = filePath_backLeft_1;
        if (rtx.Form.Files["back_left_photo_2"] != null)
        {
            var filePath_backLeft_2 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireBL2_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_backLeft_2))
            {
                await rtx.Form.Files["back_left_photo_2"].CopyToAsync(stream);
            }
            result.back_left_photo_2 = filePath_backLeft_2;
        }
        if (rtx.Form.Files["back_left_photo_3"] != null)
        {
            var filePath_backLeft_3 = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_TireBL3_{extension.Extension}");
            using (var stream = System.IO.File.Create(filePath_backLeft_3))
            {
                await rtx.Form.Files["back_left_photo_3"].CopyToAsync(stream);
            }
            result.back_left_photo_3 = filePath_backLeft_3;
        }
    }

    await db.SaveChangesAsync();
    return Results.Ok(result);
});

// Get Tire View
app.MapGet("/circle-check/get-tire-view", [Authorize] async (int? ServiceRegistrationId, AppDbContext db) =>
{
    int serviceRegistrationId = ServiceRegistrationId ?? 0;
    TireView? result = await db.TireViews.Where(item => item.service_registration_id == serviceRegistrationId).FirstOrDefaultAsync();
    if (result == null) return Results.NotFound();
    return Results.Ok(result);
});

// Finalize Circle Check
app.MapPost("/circle-check/finalize-circle-check", [Authorize] async (HttpRequest rtx, AppDbContext db) =>
{
    int.TryParse(rtx.Form["service_registration_id"], out int service_registration_id);
    
    CircleCheck? result = await db.CircleChecks
    .Where(item => item.service_registration_id == service_registration_id)
    .Include(item => item.interior_view)
    .Include(item => item.complaint_notes_view)
    .Include(item => item.exterior_view)
    .Include(item => item.tire_view)
    .FirstOrDefaultAsync();
    if (result == null) return Results.NotFound();
    var extension = new FileInfo(rtx.Form.Files["capture_sign"].FileName);
    var filePath = Path.Combine("image", $"{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}_signature_{extension.Extension}");
    using (var stream = System.IO.File.Create(filePath))
    {
        await rtx.Form.Files["capture_sign"].CopyToAsync(stream);
    }
    result.signature = filePath;
    await db.SaveChangesAsync();
    return Results.Ok(result);
});

// Get Receiving Check List
app.MapGet("/circle-check/get-receiving-check-list", [Authorize] async (AppDbContext db) =>
{
    List<CircleCheck>? result = await db.CircleChecks.ToListAsync();
    if (result == null) return Results.NotFound();
    return Results.Ok(result);
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