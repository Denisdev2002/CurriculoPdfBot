using CVPdfBot.API.Services;
using CVPdfBot.Domain.Entities;
using CVPdfBot.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<TelegramBotService>();
builder.Services.AddSingleton<Dictionary<long, ConversationState>>();

// Add services api
builder.Services.AddSingleton<Dictionary<long, ConversationState>>();
builder.Services.AddSingleton<TelegramBotService>();
builder.Services.AddHostedService<SessionCleanerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("Erro na inicialização: " + ex.Message);
}
