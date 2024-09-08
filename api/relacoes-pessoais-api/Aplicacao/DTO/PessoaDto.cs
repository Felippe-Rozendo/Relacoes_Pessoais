using relacoes_pessoais_api.Dominio.Entidades;

namespace relacoes_pessoais_api.Aplicacao.DTO
{
    public class PessoaDto
    {
        public int CodPessoa { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataModificacao { get; set; }
        public string? ContatosDsc { get; set; } = string.Empty;
        public ICollection<ContatoDto> Contatos { get; set; } = new List<ContatoDto>();
    }
}
