using System.ComponentModel;

namespace relacoes_pessoais_api.Dominio.Enums
{
    public enum TipoContatoEnum
    {
        [Description("Telefone")]
        Telefone = 1,

        [Description("E-mail")]
        Email = 2,

        [Description("Whatsapp")]
        Outro = 3
    }
}
