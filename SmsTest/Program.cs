using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using SmsTest;
using System.Net.Http.Headers;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Vonage;
using Vonage.Messages.Sms;
using Vonage.Messaging;
using Vonage.Request;
using Vonage.Verify;



var apiKey = "89bd2c7b";
var apiSecret = "4lu7b5VmZNVUus22";

var vonageClient = new VonageClient(new Vonage.ApiRequest
{
    ApiKey = apiKey,
    ApiSecret = apiSecret
});


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<VonageClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var key = config.GetValue<string>("Vonage_key");
    var secret = config.GetValue<string>("Vonage_Secret");
    var credentials = Credentials.FromApiKeyAndSecret(key, secret);

    return new VonageClient(credentials);
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
Dictionary<string, (string code, DateTime expiration)> verificationCodes = new Dictionary<string, (string, DateTime)>();

app.MapPost("/send-verification-code", async (HttpContext context, [FromBody] Vonage.Verify.VerifyRequest verifyRequest) =>
{
    // Initialize Vonage client with your API key and secret
    var apiKey = "89bd2c7b";
    var apiSecret = "4lu7b5VmZNVUus22";
    var vonageClient = new VonageClient(new Credentials(apiKey, apiSecret));

    // Generate a random verification code (you can implement your own logic)
    var verificationCode = GenerateRandomCode(); // Implement this function

    // Send the verification code via SMS
    var response = vonageClient.SmsClient.SendAnSms(new Vonage.Messaging.SendSmsRequest
    {
        To = "94701871526",
        From = "YourVonageNumber",
        Text = $"Your verification code is: {verificationCode}"
    });

    // Store the verification code and its expiration time
    var expiration = DateTime.UtcNow.AddMinutes(15); // Set an expiration time (e.g., 15 minutes)
    verificationCodes["94701871526"] = (verificationCode, expiration);

    // Return a response
    if (response.Messages[0].Status == "0")
    {
        await context.Response.WriteAsync("Verification code sent successfully.");
    }
    else
    {
        await context.Response.WriteAsync("Failed to send verification code.");
    }
});

app.MapPost("/verify-code", async (HttpContext context, [FromBody] VerifyCodeRequest codeRequest) =>
{
    if (verificationCodes.TryGetValue(codeRequest.PhoneNumber, out var storedCode))
    {
        if (DateTime.UtcNow <= storedCode.expiration && codeRequest.Code == storedCode.code)
        {
            // Code is valid and within the expiration time
            await context.Response.WriteAsync("Verification code is valid.");
        }
        else
        {
            await context.Response.WriteAsync("Invalid verification code.");
        }
    }
    else
    {
        await context.Response.WriteAsync("Verification code not found.");
    }
});




app.Run();

string GenerateRandomCode()
{
    Random random = new Random();
    const string chars = "0123456789";
    return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
}

