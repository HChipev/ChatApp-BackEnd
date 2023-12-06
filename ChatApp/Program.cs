using Back_End.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterDbContext(builder.Configuration, builder.Environment);
builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.ConfigureServices(builder.Configuration, builder.Environment);
builder.Services.RegisterFilters();
builder.Services.RegisterSwagger();
builder.Services.RegisterAutoMapper(builder.Configuration);
builder.Services.RegisterRabbitMQBus(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();