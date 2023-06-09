using CatchSubscriber;
using CatchSubscriber.Extentions;
using CatchSubscriber.Interfaces;
using CatchSubscriber.Models;
using Example.Api;
using Example.Api.Interfaces;
using FluentEmail.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//------------ EXAMPLE ADD AWS ------------
//builder.Services.AddOpenTelemetryForAWS("ThisApplication", "1.0");

//------------ EXAMPLE ADD Azure ------------
//builder.Services.AddOpenTelemetryForAzure("ThisApplication", "1.0", "[Azure connecitonstring]");

//------------ EXAMPLE ADD SLACK ------------
//You must first enable the Webhooks integration for your Slack Account to get the Token. You can enable it here: https://slack.com/services/new/incoming-webhook
//builder.Services.AddScoped<IErrorProcesser, ErrorProcessor>(
//    serviceProvider => new ErrorProcessor()
//    .RegisterSlack("MyApplication", "1.0","https://hooks.slack.com/services/[YOUR-TOKEN]", "Testing", userName: "ThisApplication")
// );

//------------EXAMPLE ADD EMAIL using GMAIL SMTP and with CC support NOTE can not use this due to google removing "lesssecureapps" support------------
//builder.Services.AddEmail("smtp.gmail.com", 465, "peter.developer@mymail.se", "MYPASSWORD", "peter.developer@mymail.se", "Peter");

builder.Services.AddScoped<IErrorProcesser, ErrorProcessor>(
    serviceProvider => new ErrorProcessor(fluentEmail: builder.Services.BuildServiceProvider().GetService<IFluentEmail>())
    .RegisterEmail("MyApplication", "1.0", "peter.developer@emailhost.se", "ToAlice")
 );

//------------ EXAMPLE ADD EXAMPLEPROVIDER/Service ------------
//builder.Services.AddOpenTelemetryForConsole("ThisApplication", "1.0");
//builder.Services.AddScoped<IErrorProcesser, ErrorProcessor>();
builder.Services.AddScoped<IExampleProvider, ExampleProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/DoSomething", async (string message, IExampleProvider provider, IErrorProcesser errorProcessor) =>
{
    try
    {
        await provider.DoSomething(message);
    }
    catch (Exception ex)
    {
        await errorProcessor.ProcessError($"Error! {ex.Message}", LogLevel.Critical, CatchAction.Console, CatchAction.AWS);
    }
}).WithName("DoSomething")
.WithOpenApi();

app.Run();

public partial class Program
{ }