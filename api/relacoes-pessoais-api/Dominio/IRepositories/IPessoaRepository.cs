using Microsoft.AspNetCore.Mvc;
using relacoes_pessoais_api.Aplicacao.DTO;
using relacoes_pessoais_api.Dominio.Entidades;

namespace relacoes_pessoais_api.Dominio.IRepositories
{
    public interface IPessoaRepository
    {
        public Task<PessoaDto?> ObterPessoaAsync(int codPessoa);
        public Task<(IList<PessoaDto>, int totalLista)> ObterListaAsync(string nome, int page);
        public Task<Pessoa> AdicionarPessoaAsync(PessoaDto model, CancellationToken ct);
        public Task<Pessoa> EditarPessoaAsync(PessoaDto model, CancellationToken ct);
        public Task<(bool excluido, string? mensagem)> RemoverPessoaAsync(int codPessoa, CancellationToken ct);
    }
}
