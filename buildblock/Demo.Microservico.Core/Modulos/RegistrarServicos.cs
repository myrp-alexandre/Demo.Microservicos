using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.Configuration
{
    public static class RegistrarServicos
    {
        /// <summary>
        /// Responsável por configurar onde o serviço vai se auto-registrar, ou seja,
        /// Será registrado o Consul Client (Cliente Consul) que o microserviço vai
        /// utilizar para se auto-registrar
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AdicionarConfiguracaoConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = configuration.GetValue<string>("Consul:ServiceDiscoveryAddress");
                consulConfig.Address = new Uri(address);
            }));
            return services;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UsarConsul(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("Agente de serviço Consul");
            var lifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();

            var registration = new AgentServiceRegistration
            {
                ID = configuration.GetValue<string>("Consul:ServiceId"),
                Name = configuration.GetValue<string>("Consul:ServiceName"),
                Address = configuration.GetValue<string>("Consul:ServiceHost"),
                Port = configuration.GetValue<int>("Consul:ServicePort")
            };

            logger.LogInformation($"Descoberta do serviço: {registration.ID}. Registrando serviço {registration.Name} no Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true); //desregistrando caso ele já exista (se não existir não faz nada)
            consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);      //registrando o serviço novamente (atualiza alguma nova informação)

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation($"Parando o registro: {registration.ID}. Desregistrando serviço {registration.Name} do Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            });

            return app;
        }
    }
}