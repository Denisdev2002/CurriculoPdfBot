using CVPdfBot.API.GraphQL;
using CVPdfBot.Domain.Entities;
using CVPdfBot.Domain.Interfaces;
using CVPdfBot.Domain.Services;
using CVPdfBot.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<TelegramBotService>();
builder.Services.AddSingleton<Dictionary<long, ConversationState>>(); // Estado por usuário
builder.Services
    .AddSingleton<IConversationStateRepository, ConversationStateRepository>()
    .AddGraphQLServer()
    .AddQueryType<Query>();

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
