using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using relacoes_pessoais_api.Aplicacao.DTO;
using relacoes_pessoais_api.Dominio.Entidades;
using relacoes_pessoais_api.Dominio.Enums;
using relacoes_pessoais_api.Dominio.Extensions;
using relacoes_pessoais_api.Dominio.IRepositories;
using relacoes_pessoais_api.Infraestrutura.Context;
using System.Drawing;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace relacoes_pessoais_api.Infraestrutura.Repositories
{
    public class PessoaRepository : IPessoaRepository
    {
        private readonly RelacoesPessoaisDB _db;

        public PessoaRepository(RelacoesPessoaisDB db)
        {
            _db = db;
        }

        public async Task<(IList<PessoaDto>, int totalLista)> ObterListaAsync(string nome, int page)
        {
            var query = from pessoa in _db.Pessoas
                        join contato in _db.Contatos
                        on pessoa.CodPessoa equals contato.CodPessoa into pessoaContatos
                        orderby pessoa.DataModificacao descending
                        select new PessoaDto
                        {
                            CodPessoa = pessoa.CodPessoa,
                            Nome = pessoa.Nome,
                            DataModificacao = pessoa.DataModificacao,
                            ContatosDsc = string.Join(" | ", pessoaContatos.Select(c => $"{EnumExtensions.GetDescription<TipoContatoEnum>(c.TipoContato)}: {c.ValorContato}"))
                        };


            if (!string.IsNullOrWhiteSpace(nome))
            {
                query = query.Where(p => p.Nome.Trim().Contains(nome.Trim()));
            }

            var totalLista = await query.CountAsync();

            var lista = await query
            .Skip(page * 9).Take(9)
                .ToListAsync();

            return (lista, totalLista);
        }


        public async Task<PessoaDto?> ObterPessoaAsync(int codPessoa)
        {
            return await _db.Pessoas
                .AsNoTracking()
                .Where(p => p.CodPessoa == codPessoa)
                .Select(p => new PessoaDto
                {
                    CodPessoa = p.CodPessoa,
                    Nome = p.Nome,
                    DataModificacao = p.DataModificacao,
                    Contatos = _db.Contatos
                        .Where(c => c.CodPessoa == p.CodPessoa)
                        .Select(c => new ContatoDto
                        {
                            CodContato = c.CodContato,
                            TipoContato = c.TipoContato,
                            TipoContatoDsc = EnumExtensions.GetDescription<TipoContatoEnum>(c.TipoContato),
                            ValorContato = c.ValorContato
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }


        public async Task<Pessoa> AdicionarPessoaAsync(PessoaDto model, CancellationToken ct)
        {
            var pessoa = new Pessoa();

            pessoa.Nome = model.Nome;
            pessoa.DataModificacao = DateTime.Now;

            await _db.Pessoas.AddAsync(pessoa, ct);
            await _db.SaveChangesAsync(ct);
            return pessoa;
        }

        public async Task<Pessoa> EditarPessoaAsync(PessoaDto model, CancellationToken ct)
        {
            var pessoa = await _db.Pessoas.FirstAsync(x => x.CodPessoa == model.CodPessoa);

            if (pessoa == null)
                throw new Exception("Pessoa não encontrada.");

            pessoa.Nome = model.Nome;
            pessoa.DataModificacao = DateTime.Now;

            _db.Pessoas.Update(pessoa);
            await _db.SaveChangesAsync(ct);
            return pessoa;
        }

        public async Task<(bool excluido, string? mensagem)> RemoverPessoaAsync(int codPessoa, CancellationToken ct)
        {
            try
            {
                var pessoa = await _db.Pessoas.FirstAsync(x => x.CodPessoa == codPessoa);

                if (pessoa == null)
                    return (false, "Pessoa não encontrada.");

                _db.Pessoas.Remove(pessoa);
                await _db.SaveChangesAsync(ct);
                return (true, "Pessoa excluída com sucesso.");
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }

        }
    }
}
