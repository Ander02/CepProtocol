using Server.HttpClient;
using Shared.Messages;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Messages
{
    public class MessageHandler
    {
        public static async Task<string> Handle(MessageResult result)
        {
            var builder = new MessageBuilder('|');

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

                    builder.AddField("ADDRESS", address);

                    //TODO: Salvar Historico

                    return builder.BuildValues();

                case "HISTORICO":
                    //TODO: Validador do token gerado e buscar CEP do usuário 
                    break;

                default:

                    break;
            }

            return string.Empty;
        }

    }
}
