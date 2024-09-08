using Microsoft.EntityFrameworkCore;
using relacoes_pessoais_api.Aplicacao.Service;
using relacoes_pessoais_api.Infraestrutura.Context;
using relacoes_pessoais_api.Infraestrutura.Repositories;

namespace relacoes_pessoais_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<RelacoesPessoaisDB>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddTransient<PessoaService>();
            builder.Services.AddTransient<PessoaRepository>();

            builder.Services.AddTransient<ContatoService>();
            builder.Services.AddTransient<ContatoRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.UseCors("AllowAll");

            app.MapControllers();

            app.Run();
        }
    }
}
