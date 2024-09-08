using relacoes_pessoais_api.Aplicacao.DTO;
using relacoes_pessoais_api.Dominio.Entidades;
using relacoes_pessoais_api.Infraestrutura.Repositories;
using System.Text.RegularExpressions;

namespace relacoes_pessoais_api.Aplicacao.Service
{
    public class ContatoService
    {
        private readonly ContatoRepository _contatoRepository;

        public ContatoService(ContatoRepository contatoRepository)
        {
            _contatoRepository = contatoRepository;
        }

        public async Task<IList<Contato>> AdicionarContatoAsync(IList<ContatoDto> contatos, CancellationToken ct)
        {
            try
            {
                var (ehValido, mensagem) = ValidarContato(contatos);
                
                if (ehValido)
                    return await _contatoRepository.AdicionarContatoAsync(contatos, ct);
                
                throw new Exception(mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<ContatoDto>> EditarContatoAsync(IList<ContatoDto> dto, CancellationToken ct)
        {
            try
            {
                var (ehValido, mensagem) = ValidarContato(dto);

                if (ehValido)
                    return await _contatoRepository.EditarContatoAsync(dto, ct);
                throw new Exception(mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool excluido, string? mensagem)> ExcluirContatoAsync(int codContato, CancellationToken ct)
        {
            try
            {
                return await _contatoRepository.RemoverContatoAsync(codContato, ct);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private (bool, string) ValidarContato(IList<ContatoDto> contatos)
        {
            var contatosComErro = new List<ContatoDto>();
            var telefoneRegex = new Regex(@"^\(\d{2}\)\d{4,5}-\d{4}$"); // regex para telefone
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$"); // regex para e-mail

            foreach (var contato in contatos)
            {
                bool ehValido = true;
                string erro = string.Empty;

                switch (contato.TipoContato)
                {
                    case 1: // Telefone
                    case 3: // Whatsapp
                        if (!telefoneRegex.IsMatch(contato.ValorContato))
                            ehValido = false;
                        break;

                    case 2: // E-mail
                        if (!emailRegex.IsMatch(contato.ValorContato))
                            ehValido = false;
                        break;
                }

                if (!ehValido)
                {
                    contatosComErro.Add(contato);
                }
            }

            bool sucesso = !contatosComErro.Any();
            string mensagem = sucesso ? "Todos os contatos são válidos." : "Alguns contatos são inválidos.";

            return (sucesso, mensagem);
        }

    }
}
