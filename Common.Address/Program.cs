using Common.Address.Work;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddHttpClient();
builder.Services.AddSingleton<HerePlatformValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config => { config.Title = "Address Validation API"; });


var app = builder.Build();
app.UseOpenApi();
app.UseSwaggerUi();

//app.MapPost("/validateAddress", async (UserAddress userAddress, IAddressValidator<NormalizedAddress> validator) =>
//{
//    var validated = await validator.ValidateAsync(userAddress);
//    return validated is not null ? Results.Ok(validated) : Results.BadRequest("Invalid address");
//});

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.MapOpenApi();

//}

//app.UseHttpsRedirection();
app.MapControllers();

app.UseAuthorization();

app.Run();
