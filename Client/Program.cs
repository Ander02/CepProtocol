using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Run();
            //RunTest();
            //RunMultThread();
        }

        public static void Run()
        {
            Console.WriteLine("Olá, bem vindo ao CepProtocol - Client");

            string serverIp;
            var ipRegexp = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
            do
            {
                serverIp = ReadLine("Digite o ip do servidor que deseja acessar");
            } while (!ipRegexp.Match(serverIp).Success);

            int serverPort;
            do
            {
                serverPort = ReadIntLine("Digite a porta do servidor que deseja acessar");
            } while (serverPort <= 0);

            var client = new Client(serverIp, serverPort);

            while (true)
            {
                int option;

                Console.WriteLine($"Você agora está conectado à {serverIp}:{serverPort}");

                Console.WriteLine("O que gostaria de fazer à seguir:");

                Console.WriteLine("1 - AJUDA");
                Console.WriteLine("2 - ENVIAR MENSAGEM");
                Console.WriteLine("9 - SAIR");

                option = ReadIntLine(string.Empty);

                switch (option)
                {
                    case 1:
                        HandleHelp();
                        break;
                    case 2:
                        var message = ReadLine("Digite sua mensagem no formato correto");

                        client.Send(message, (bytes) =>
                        {
                            var response = Encoding.UTF8.GetString(bytes);
                            Console.WriteLine($"O servidor respondeu:\n {response}");
                            Console.WriteLine();
                        });
                        break;
                    case 9:
                        Console.WriteLine("Bye");
                        return;
                    default:
                        Console.WriteLine("Comando inválido, tente novamente");
                        break;
                }
            }
        }

        private static void HandleHelp()
        {
            Console.WriteLine(@"Os campos do protocolo podem ser definidos da seguinte forma:
MENSAGEM=valor|CAMPO1=valor|CAMPO2=valor|...|CAMPOn=valor

Sendo que para o uso de todas as mensagens é necessário obter um token de autenticação, através da mensagem de Login, especificada abaixo:

LOGIN
Campos: Usuario <string>, Senha <string>
Exemplo:
MENSAGEM=LOGIN|Usuario=User|Senha=UmaSenhaSeguraEncriptada
Resposta: Sucesso <bool>, Token <string> (virá vazio caso sucesso for falso) 
Exemplo:
Sucesso=true|Token=sahasdhawiwqhosfbjdfadiufnjsafkoasduoadnasdkçauad

As outras mensagens cobertas pelo protocolo são:

CADASTRAR
Campos: Usuario <string>, Senha <string>, TempoDeConexao <int> (em segundos)
Exemplo:
MENSAGEM=CADASTRAR|Usuario=User|Senha=UmaSenhaSeguraEncriptada|TempoDeConexao=30
	Resposta: Sucesso <bool>, Descricao <string>
Sucesso=true|Descricao=Cadastrado com Sucesso

CEP
Campos: CEP <string>, Token <string> (Token de autenticação obtido pelo Login)
Exemplo:
MENSAGEM=CEP|CEP=01001000|Token=sahasdhawiwqhosfbjdfadiufnjsafkoasduoadnasdkçauadsbjasds
Resposta: CEP <string>, Logradouro<string>, Complemento <string>, Bairro <string>, Cidade <string>, UF <string>
CEP=01001000|Logradouro=Praça da Sé|Complemento=Lado Ímpar|Bairro=Sé|Cidade=São Paulo|UF=SP

HISTÓRICO
Campos: Token <string>
Exemplo:
MENSAGEM=HISTORICO|Token=sahasdhawiwqhosfbjdfadiufnjsafkoasduoadnasdkçauadsbjasds
	Resposta: Lista de Resultados da Consulta CEP, na seguinte forma:
Index<int>, CEP <string>, Logradouro<string>, Complemento <string>, Bairro <string>, Cidade <string>, UF <string>, Data <DateTime>
Index=0|CEP=01001000|Logradouro=Praça da Sé|Complemento=Lado Ímpar|Bairro=Sé|Cidade=São Paulo|UF=SP|Data=01/10/2019 12:59:00
Index=1|CEP=01001001|Logradouro=Praça da Sé|Complemento=Lado Par|Bairro=Sé|Cidade=São Paulo|UF=SP|Data=01/10/2019 12:59:00
");
        }

        private static int ReadIntLine(string message)
        {
            var serverPortString = ReadLine(message);
            Int32.TryParse(serverPortString, out int serverPortConverted);
            return serverPortConverted;
        }

        private static string ReadLine(string message)
        {
            Console.Write($"{message} > ");
            return Console.ReadLine();
        }












        private static void RunTest()
        {
            var client = new Client("127.0.0.1", 13000);

            while (true)
            {
                Console.WriteLine("Write your message");
                var message = Console.ReadLine();

                if (message.Equals("Quit", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Bye");
                    break;
                }

                client.Send(message, onReceive: (bytes) =>
                {
                    var response = ClientConstants.DefaultEncoding.GetString(bytes);
                    Console.WriteLine($"Client Received: {response.Trim()}");
                });
            }
            client.Close();
        }

        private static void RunMultThread()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                var client = new Client("127.0.0.1", 13000);

                for (var i = 0; i < 5; i++)
                {
                    client.Send($"Hello, I'm Device 1 sending i = {i}", onReceive: (bytes) =>
                    {
                        var response = Encoding.UTF8.GetString(bytes);
                        Console.WriteLine($"Received: {response}");
                        Thread.Sleep(2000);
                    });
                }
                client.Close();
            }).Start();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                var client = new Client("127.0.0.1", 13000);

                for (var i = 0; i < 5; i++)
                {
                    client.Send($"Hello, I'm Device 2 sending i = {i}", onReceive: (bytes) =>
                    {
                        var response = Encoding.UTF8.GetString(bytes);
                        Console.WriteLine($"Received: {response}");
                        Thread.Sleep(2000);
                    });
                }
                client.Close();
            }).Start();
            Console.ReadLine();
        }
    }
}
