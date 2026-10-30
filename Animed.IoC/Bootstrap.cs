using Animed.Application.Interfaces;
using Animed.Application.UseCases;
using Animed.Domain.Interfaces;
using Animed.Infrastructure.Data;
using Animed.Infrastructure.Data.Repositories;
using Animed.Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.EntityFrameworkCore.Infrastructure;

namespace Animed.IoC
{
    public static class Bootstrap
    {
        public static void AddIoC(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationContext>(options => {

                // O Oracle da FIAP é 19c: sem tipo BOOLEAN, booleanos viram NUMBER(1)
                options.UseOracle(configuration.GetConnectionString("Oracle"), oracle => oracle.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19));
            });

            services.AddDbContext<MongoApplicationContext>(options => {
                options.UseMongoDB(configuration.GetConnectionString("Mongodb") ?? "", configuration["MongoDatabase"] ?? "animed");
            });


            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserUseCase, UserUseCase>();


            services.AddTransient<ITutorRepository, TutorRepository>();
            services.AddTransient<ITutorUseCase, TutorUseCase>();


            services.AddTransient<IPetRepository, PetRepository>();
            services.AddTransient<IPetUseCase, PetUseCase>();


            services.AddTransient<IHistoricoCuidadoRepository, HistoricoCuidadoRepository>();
            services.AddTransient<IHistoricoCuidadoUseCase, HistoricoCuidadoUseCase>();


            services.AddTransient<IEnderecoService, EnderecoService>();


            services.AddTransient<IPackageAnimedRepository, PackageAnimedRepository>();
            services.AddTransient<IPackageAnimedUseCase, PackageAnimedUseCase>();
        }
    }
}
