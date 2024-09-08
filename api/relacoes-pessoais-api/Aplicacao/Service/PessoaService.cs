using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using relacoes_pessoais_api.Aplicacao.DTO;
using relacoes_pessoais_api.Dominio.Entidades;
using relacoes_pessoais_api.Dominio.IRepositories;
using relacoes_pessoais_api.Infraestrutura.Repositories;

namespace relacoes_pessoais_api.Aplicacao.Service
{
    public class PessoaService
    {
        private readonly PessoaRepository _pessoaRepository;

        public PessoaService(PessoaRepository pessoaRepository)
        {
            _pessoaRepository = pessoaRepository;
        }

        public async Task<PessoaDto?> ObterPessoaAsync(int codPessoa)
        {
            try
            {
                return await _pessoaRepository.ObterPessoaAsync(codPessoa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(IList<PessoaDto>, int totalLista)> ObterListaAsync(string nome, int page)
        {
            try
            {
                return await _pessoaRepository.ObterListaAsync(nome, page);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Pessoa> AdicionarPessoaAsync(PessoaDto dto, CancellationToken ct)
        {
            try
            {
                return await _pessoaRepository.AdicionarPessoaAsync(dto, ct);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Pessoa> EditarPessoaAsync(PessoaDto dto, CancellationToken ct)
        {
            try
            {
                return await _pessoaRepository.EditarPessoaAsync(dto, ct);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool excluido, string? mensagem)> ExcluirPessoaAsync(int codPessoa, CancellationToken ct)
        {
            try
            {
                return await _pessoaRepository.RemoverPessoaAsync(codPessoa, ct);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
