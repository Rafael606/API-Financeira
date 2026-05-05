using PixApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Registra o suporte a controllers, permitindo usar classes de API.
builder.Services.AddControllers();

// Registrar serviços
builder.Services.AddScoped<IContaService, ContaService>();
builder.Services.AddScoped<ITransacaoPixService, TransacaoPixService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Força redirecionamento de HTTP para HTTPS para mais segurança.
app.UseHttpsRedirection();

// Mapear controllers
app.MapControllers();

app.Run();
