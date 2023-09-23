using Microsoft.AspNetCore.Mvc;
using SmsTest;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;
using Vonage.Verify;

var accountSid = "AC280aa864dda71af9658694efdbc13f99";
var authToken = "763fa2f973eba2cd44f5ab22c72ea687";

TwilioClient.Init(accountSid, authToken);

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

app.MapPost("/send-verification-code", async (HttpContext context, [FromBody] Vonage.Verify.VerifyRequest verifyRequest) =>
{
    // Initialize Vonage client with your API key and secret
    var apiKey = "89bd2c7b";
    var apiSecret = "4lu7b5VmZNVUus22";
    var vonageClient = new VonageClient(new Credentials(apiKey, apiSecret));

    // Generate a random verification code (you can implement your own logic)
    var verificationCode="hello";

    // Send the verification code via SMS
    var response = vonageClient.SmsClient.SendAnSms(new Vonage.Messaging.SendSmsRequest
    {
        To = "94701871526",
        From = "YourVonageNumber",
        Text = $"Your verification code is: {verificationCode}"
    });

    // You can store the verification code and its expiration for future validation

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





app.Run();

