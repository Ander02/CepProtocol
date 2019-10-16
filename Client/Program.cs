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

            Client client = default;
            var tryAgain = false;
            do
            {
                try
                {
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

                    client = new Client(serverIp, serverPort);
                    Console.WriteLine($"Você agora está conectado à {serverIp}:{serverPort}");
                    tryAgain = false;
                }
                catch (Exception)
                {
                    Console.WriteLine("Não foi possível conectar-se ao servidor, tente novamente");
                    tryAgain = true;
                }
            } while (tryAgain);

            while (true)
            {
                int option;
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
            Console.WriteLine(@"As mensagens do protocolo seguem o seguinte padrão:
	                            MESSAGE:valor|CAMPO1:valor|CAMPO2:valor| … |CAMPOn:valor

LOGIN
Campos: USERNAME <String>, PASSWORD<String>, CONNECTION_DURATION<int>
Exemplo:
MESSAGE:LOGIN|USERNAME:”User”|PASSWORD: “UmaSenhaSegura”|CONNECTION_DURATION:300

A MESSAGE de resposta segue o seguinte padrão:
Resposta: Sucesso <bool>, Token <String> (vazio caso sucesso for falso)
Exemplo:
Sucesso:true|TOKEN:”SequênciaDeAlfanuméricos”

CADASTRAR
Campos: USERNAME<String>, PASSWORD<String>
Exemplo:
MESSAGE:CADASTRAR|USERNAME:”email@email.com”|Senha:”UmaSenhaSegura”
Padrão de resposta: Sucesso <bool>
Exemplo:
Sucesso:true

CEP
Campos: CEP <String>, TOKEN <String> (Token de autenticação obtido pelo Login)
Exemplo:
MESSAGE:CEP|CEP:”01001000”|TOKEN:”SequênciaDeAlfanuméricos”
Padrão de resposta: CEP <String>, LOGRADOURO<String>, COMPLEMENTO<String>, BAIRRO<String>, CIDADE<String>, UF <String>
Exemplo: 
CEP:”01001000”|LOGRADOURO:”Praça da Sé”|COMPLEMENTO:”Lado Ímpar”|BAIRRO:”Sé”|CIDADE:”São Paulo”|UF:”SP”

HISTÓRICO
Campos: Token <String>
Exemplo:
MESSAGE:HISTORICO|TOKEN:”SequênciaDeAlfanuméricos”
Padrão de resposta: Lista de resultados da consulta CEP, na seguinte forma: Index <int>, CEP <String>, Logradouro <String>, Complemento <String>, Bairro <String>, Cidade <String>, UF <String>, Data <DateTime>
Exemplo:
Index:0|CEP:”01001000”|Logradouro:”Praça da Sé”|Complemento:”Lado Ímpar”|Bairro:”Sé”|Cidade:”São Paulo”|UF:”SP”|Data:01/10/2019 12:59:00
Index:1|CEP:”01001001”|Logradouro:”Praça da Sé”|Complemento:”Lado Par”|Bairro:”Sé”|Cidade:”São Paulo”|UF:”SP”|Data:01/10/2019 12:59:00
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








        //Tests code



        private static void RunTest()
        {
            var client = new Client("127.0.0.1", 4242);

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
                var client = new Client("127.0.0.1", 4242);

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
                var client = new Client("127.0.0.1", 4242);

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
