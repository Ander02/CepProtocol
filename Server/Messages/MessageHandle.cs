using DataBase;
using Server.HttpClient;
using Shared;
using Shared.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace Server.Messages
{
    public class MessageHandler
    {
        public static async Task<string> Handle(MessageResult result)
        {
            var builder = new MessageBuilder(Constants.DefaultSeparator);
            var userDb = new DbWriter($"{AppDomain.CurrentDomain.BaseDirectory}User.csv", "Username;Password");
            var cepDb = new DbWriter($"{AppDomain.CurrentDomain.BaseDirectory}Cep.csv", "UserId;Cep;Logradouro;Bairro;Complemento;Cidade;Uf;DataBusca");

            switch (result.MessageType.ToUpper())
            {
                case "CADASTRAR":
                    {
                        //TODO: Cadastrar usuário no CSV
                        Console.WriteLine("AQUI");
                        
                        var username = result.Values.FirstOrDefault(d => d.Field == "USERNAME")?.Value;
                        String password = result.Values.FirstOrDefault(d=> d.Field == "PASSWORD")?.Value;
                        Console.WriteLine("Password= "+password);
                        Console.WriteLine("UserName= "+username);
                        Regex RE=new Regex("@*.com");
                        if(password.Length<8)
                        { 
                            builder.AddFailure("A senha deve possuir pelo menos 8 caracteres");
                        }
                        else if (!(RE.IsMatch(username))){
                            builder.AddFailure("Email invalido");
                        }
                        else if (userDb.GetLines("UserId",username).Count()>0)
                        {
                            builder.AddFailure("Usuario ja cadastrado");
                        } 
                        else{
                            password=UserHelper.Encrypt(password);
                            Console.WriteLine(password);
                            String [] line= {username,password};
                            Console.WriteLine(userDb.InsertLine(line));
                            builder.AddSucess();
                        }
                        break;
                    }
                case "LOGIN":
                    {
                        var username = result.Values.FirstOrDefault(d => d.Value == "USERNAME")?.Value;
                        var password = result.Values.FirstOrDefault(d => d.Value == "PASSWORD")?.Value;
                        var tempoDeConexaoStr = result.Values.FirstOrDefault(d => d.Value == "TEMPO_DE_CONEXAO")?.Value;

                        Int32.TryParse(tempoDeConexaoStr, out int tempoDeConexao);
                        if (tempoDeConexao <= 0)
                        {
                            builder.AddFailure("Tempo de conexão inválido");
                            break;
                        }

                        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                        {
                            builder.AddFailure("Usuário ou senha inválidos");
                            break;
                        }

                        var userLines = userDb.GetLines("Username", username);

                        if (userLines.Count != 1)
                        {
                            builder.AddFailure("Usuário ou senha inválidos");
                            break;
                        }

                        var user = userLines.FirstOrDefault();

                        if (user == null || !UserHelper.PasswordEquals(user[2], UserHelper.Encrypt(password)))
                        {
                            builder.AddFailure("Usuário ou senha inválidos");
                            break;
                        }

                        builder.AddSucess();

                        builder.AddField("TOKEN", UserHelper.GenerateToken(user[0], 30));

                        break;
                    }
                case "CEP":
                    {
                        var token = result.Values.FirstOrDefault(d => d.Field == "TOKEN")?.Value;

                        var userId = UserHelper.ValidateToken(token);

                        if (string.IsNullOrWhiteSpace(userId))
                        {
                            builder.AddFailure("Token inválido");
                            break;
                        }

                        var cep = result.Values.FirstOrDefault(d => d.Field == "CEP")?.Value;
                        var client = new ViaCepClient();
                        var address = await client.GetAddressByCep(token);

                        if (address.Erro)
                        {
                            builder.AddFailure("Endereço não encontrado");
                            break;
                        }

                        builder.AddSucess();
                        builder.AddField("CEP", address.Cep);
                        builder.AddField("LOGRADOURO", address.Logradouro);
                        builder.AddField("BAIRRO", address.Bairro);
                        builder.AddField("COMPLEMENTO", address.Complemento);
                        builder.AddField("CIDADE", address.Localidade);
                        builder.AddField("UF", address.Uf);

                        //Salvar no Historico
                        cepDb.InsertLine(new string[] { userId, address.Cep, address.Logradouro, address.Bairro, address.Complemento, address.Localidade, address.Uf, DateTime.Now.ToString() });
                        break;
                    }
                case "HISTORICO":
                    {
                        var token = result.Values.FirstOrDefault(d => d.Field == "TOKEN")?.Value;

                        var userId = UserHelper.ValidateToken(token);

                        if (string.IsNullOrWhiteSpace(userId))
                        {
                            builder.AddFailure("Token inválido");
                            break;
                        }

                        var results = cepDb.GetLines("UserId", userId);

                        if (!results.Any())
                        {
                            builder.AddFailure("Histórico vazio");
                            break;
                        }

                        builder.AddSucess();

                        int i = 0;
                        foreach (var line in results)
                        {
                            builder.AddField("INDEX", i.ToString());
                            builder.AddField("USER_ID", line[0]);
                            builder.AddField("CEP", line[1]);
                            builder.AddField("LOGRADOURO", line[2]);
                            builder.AddField("BAIRRO", line[3]);
                            builder.AddField("COMPLEMENTO", line[4]);
                            builder.AddField("CIDADE", line[5]);
                            builder.AddField("UF", line[6]);
                            builder.AddField("DATA_DA_BUSCA", line[7]);
                            i++;
                        }
                        break;
                    }
                default:
                    {
                        builder.AddFailure("Mensagem inválida");
                        break;
                    }
            }

            return builder.BuildValues();
        }
    }
}
