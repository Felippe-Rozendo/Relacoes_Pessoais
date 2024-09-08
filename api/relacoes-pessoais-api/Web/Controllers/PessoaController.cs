using Microsoft.AspNetCore.Mvc;
using relacoes_pessoais_api.Aplicacao.DTO;
using relacoes_pessoais_api.Aplicacao.Service;

namespace relacoes_pessoais_api.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PessoaController : ControllerBase
    {

        [HttpGet("obter-pessoa")]
        public async Task<ActionResult> ObterPessoaAsync([FromServices] PessoaService service, [FromQuery] int codPessoa)
        {
            try
            {
                var pessoa = await service.ObterPessoaAsync(codPessoa);

                if(pessoa is null)
                    return BadRequest("Pessoa não encontrada");

                return Ok(pessoa);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("obter-lista-pessoas")]
        public async Task<ActionResult> ObterListaAsync([FromServices] PessoaService service, [FromQuery] string? nome, [FromQuery] int page)
        {
            try
            {
                var (lista, total )= await service.ObterListaAsync(nome, page-1);
                return Ok(new {lista, total});
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("incluir-pessoa")]
        public async Task<ActionResult> IncluirPessoaAsync([FromServices] PessoaService service, [FromBody] PessoaDto model, CancellationToken ct)
        {
            try
            {
                var response = await service.AdicionarPessoaAsync(model, ct);

                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("editar-pessoa")]
        public async Task<ActionResult> EditarPessoaAsync([FromServices] PessoaService service, [FromBody] PessoaDto model, CancellationToken ct)
        {
            try
            {
                var response = await service.EditarPessoaAsync(model, ct);

                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("excluir-pessoa")]
        public async Task<ActionResult> ExcluirPessoaAsync([FromServices] PessoaService service, [FromQuery] int codPessoa, CancellationToken ct)
        {
            try
            {
                var (excluido, mensagem) = await service.ExcluirPessoaAsync(codPessoa, ct);

                if (!excluido)
                    return BadRequest(mensagem);

                return Ok(mensagem);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
