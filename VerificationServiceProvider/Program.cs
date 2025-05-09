using Azure.Communication.Email;
using VerificationServiceProvider;
using VerificationServiceProvider.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddGrpc();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton(x => new EmailClient(builder.Configuration["ACS:ConnectionString"]));
builder.Services.AddScoped<VerificationService>();

builder.Services.AddGrpcClient<EmailContract.EmailContractClient>(x =>
{
    x.Address = new Uri(builder.Configuration["EmailServiceProvider"]!);
});

var app = builder.Build();

app.MapGrpcService<VerificationService>();
app.MapGet("/", () => "VerificationServiceProvider is running.");
app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();