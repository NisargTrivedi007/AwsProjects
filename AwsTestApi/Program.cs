using AwsTestApi.Endpoints;
using Scalar.AspNetCore;
using Amazon.S3;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions("AWSS3"));
builder.Services.AddAWSService<IAmazonS3>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        //options.with
    });
}
app.MapS3Endpoints();

//app.UseHsts();
app.UseHttpsRedirection();

app.MapGet("testurl", () =>
{
    return "This is a test URL";
});

app.Run();