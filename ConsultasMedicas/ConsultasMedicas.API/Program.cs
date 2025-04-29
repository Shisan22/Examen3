using ConsultasMedicas.Application.Commands;
using ConsultasMedicas.Domain.Interfaces;
using ConsultasMedicas.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SolicitarConsultaCommand).Assembly));
builder.Services.AddSingleton<IPacienteRepository, InMemoryPacienteRepository>();
builder.Services.AddSingleton<IMedicoRepository, InMemoryMedicoRepository>();
builder.Services.AddSingleton<IConsultaRepository, InMemoryConsultaRepository>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();