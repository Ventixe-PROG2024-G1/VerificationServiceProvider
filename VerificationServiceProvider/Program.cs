using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using VerificationServiceProvider;
using VerificationServiceProvider.Middlewares;
using VerificationServiceProvider.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

//builder.Services.AddGrpc();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton(x => new EmailClient(builder.Configuration["ACS:ConnectionString"]));
builder.Services.AddScoped<VerificationService>();
builder.Services.AddSingleton(new ServiceBusClient(builder.Configuration["ASB:ConnectionString"]));

//builder.Services.AddGrpcClient<EmailContract.EmailContractClient>(x =>
//{
//    x.Address = new Uri(builder.Configuration["EmailServiceProvider"]!);
//});

var app = builder.Build();

//app.MapGrpcService<VerificationService>();
//app.MapGet("/", () => "VerificationServiceProvider is running.");
app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthentication();
app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();