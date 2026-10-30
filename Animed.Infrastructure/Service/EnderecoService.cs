using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Animed.Infrastructure.Service
{
    public class EnderecoService : IEnderecoService
    {
        private readonly ILogger<EnderecoService> _logger;
        private readonly RestClient _restClient;

        public EnderecoService(ILogger<EnderecoService> logger)
        {
            _logger = logger;
            _restClient = new RestClient("https://viacep.com.br/ws/");
        }

        public async Task<EnderecoModel?> ObterEnderecoPorCepAsync(string cep)
        {
            var request = new RestRequest($"{cep}/json/", Method.Get);
            var response = await _restClient.ExecuteAsync<EnderecoModel>(request);

            if (!response.IsSuccessful || response.Data == null || string.IsNullOrEmpty(response.Data.Cep))
            {
                _logger.LogWarning("ViaCEP não encontrou o cep {Cep}", cep);
                throw new NoContentException($"Não foi localizado o endereco para esta cep: {cep}");
            }

            return response.Data;
        }
    }
}
