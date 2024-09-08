using Microsoft.EntityFrameworkCore;
using relacoes_pessoais_api.Aplicacao.DTO;
using relacoes_pessoais_api.Dominio.Entidades;
using relacoes_pessoais_api.Dominio.IRepositories;
using relacoes_pessoais_api.Infraestrutura.Context;

namespace relacoes_pessoais_api.Infraestrutura.Repositories
{
    public class ContatoRepository : IContatoRepository
    {
        private readonly RelacoesPessoaisDB _db;

        public ContatoRepository(RelacoesPessoaisDB db)
        {
            _db = db;
        }

        public async Task<IList<Contato>> AdicionarContatoAsync(IList<ContatoDto> contatos, CancellationToken ct)
        {
            var contatoList = new List<Contato>();

            foreach (var item in contatos)
            {
                var contato = new Contato
                {
                    CodPessoa = item.CodPessoa,
                    TipoContato = item.TipoContato,
                    ValorContato = item.ValorContato
                };
                contatoList.Add(contato);
            }

            await _db.Contatos.AddRangeAsync(contatoList, ct);
            await _db.SaveChangesAsync(ct);
            return contatoList;
        }

        public async Task<IList<ContatoDto>> EditarContatoAsync(IList<ContatoDto> contatos, CancellationToken ct)
        {
            var novosContatos = new List<Contato>();

            foreach (var item in contatos)
            {
                if (item.CodContato == 0)
                {
                    // Adicionar novo contato
                    var novoContato = new Contato
                    {
                        CodPessoa = item.CodPessoa,
                        TipoContato = item.TipoContato,
                        ValorContato = item.ValorContato
                    };
                    novosContatos.Add(novoContato);
                }
                else
                {
                    // Atualizar contato existente
                    var contatoAtualizado = await _db.Contatos.FirstOrDefaultAsync(x => x.CodContato == item.CodContato);
                    if (contatoAtualizado != null)
                    {
                        contatoAtualizado.TipoContato = item.TipoContato;
                        contatoAtualizado.ValorContato = item.ValorContato;

                        _db.Contatos.Update(contatoAtualizado);
                    }
                }
            }

            if (novosContatos.Any())
                await _db.Contatos.AddRangeAsync(novosContatos, ct);

            await _db.SaveChangesAsync(ct);

            return contatos;
        }


        public async Task<(bool excluido, string? mensagem)> RemoverContatoAsync(int codContato, CancellationToken ct)
        {
            try
            {
                var contato = await _db.Contatos.FirstAsync(x => x.CodContato == codContato);

                if (contato == null)
                    return (false, "Contato não encontrado.");

                _db.Contatos.Remove(contato);
                await _db.SaveChangesAsync(ct);
                return (true, "Contato excluído com sucesso.");
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }
    }
}
