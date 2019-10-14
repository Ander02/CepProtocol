using Server.HttpClient;
using Shared;
using Shared.Messages;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Messages
{
    public class MessageHandler
    {
        public static async Task<string> Handle(MessageResult result)
        {
            var builder = new MessageBuilder(Constants.DefaultSeparator);

            switch (result.MessageType.ToUpper())
            {
                case "CADASTRAR":
                    //TODO: Cadastrar usuário no CSV

                    break;

                case "LOGIN":
                    //TODO: Gerar token para o usuario baseado no usuário do CSV
                    break;
                case "CEP":
                    //TODO: Validador do token gerado

                    var cep = result.Values.FirstOrDefault(d => d.Field == "CEP")?.Value;
                    var client = new ViaCepClient();
                    var address = await client.GetAddressByCep(cep);

                    if (address.Erro)
                    {
                        builder.AddField("SUCESSO", address.Erro.ToString());
                        builder.AddField("DESCRICAO", "Endereço não encontrado");
                        break;
                    }

                    builder.AddField("CEP", address.Cep);
                    builder.AddField("LOGRADOURO", address.Logradouro);
                    builder.AddField("BAIRRO", address.Bairro);
                    builder.AddField("COMPLEMENTO", address.Complemento);
                    builder.AddField("CIDADE", address.Localidade);
                    builder.AddField("UF", address.Uf);

                    //TODO: Salvar no Historico

                    break;
                case "HISTORICO":
                    //TODO: Validador do token gerado e buscar CEP do usuário 
                    break;

                default:
                    builder.AddField("SUCESSO", false.ToString());
                    builder.AddField("DESCRICAO", "Mensagem inválida");
                    break;
            }

            return builder.BuildValues();
        }

    }
}
