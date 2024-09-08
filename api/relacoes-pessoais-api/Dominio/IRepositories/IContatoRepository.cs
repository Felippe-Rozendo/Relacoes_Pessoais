using relacoes_pessoais_api.Aplicacao.DTO;
using relacoes_pessoais_api.Dominio.Entidades;

namespace relacoes_pessoais_api.Dominio.IRepositories
{
    public interface IContatoRepository
    {
        public Task<IList<Contato>> AdicionarContatoAsync(IList<ContatoDto> model, CancellationToken ct);
        public Task<IList<ContatoDto>> EditarContatoAsync(IList<ContatoDto> model, CancellationToken ct);
        public Task<(bool excluido, string? mensagem)> RemoverContatoAsync(int codContato, CancellationToken ct);
    }
}
