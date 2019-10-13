using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.HttpClient
{
    /// <summary>
    /// Cliente TCP para o serviço do viaCep de busca de endereços
    /// </summary>
    public class ViaCepClient
    {
        private readonly string endLine = "\r\n\r\n";
        private readonly string viaCepBaseUri = "viacep.com.br";
        private readonly int port = 443;
        private readonly string responseContentType = "application/json";

        private int GetEndLineBytes() => Encoding.ASCII.GetBytes(endLine).Length;

        /// <summary>
        /// Busca o endereço completo baseado no CEP
        /// </summary>
        /// <param name="cep"></param>
        /// <returns></returns>
        public async Task<string> GetAddressByCep(string cep)
        {
            using (var tcp = new TcpClient(viaCepBaseUri, port))
            {
                using (var stream = new SslStream(tcp.GetStream(), false))
                {
                    stream.AuthenticateAsClient(viaCepBaseUri, null, System.Security.Authentication.SslProtocols.Tls, false);

                    tcp.SendTimeout = 1000;
                    tcp.ReceiveTimeout = 1000;

                    // Send request
                    var builder = new StringBuilder();
                    builder.AppendLine($"GET /ws/{cep}/json HTTP/1.1");
                    builder.AppendLine($"Host: {viaCepBaseUri}");
                    builder.AppendLine("Connection: keep-alive");
                    builder.AppendLine("Upgrade-Insecure-Requests: 1");
                    builder.AppendLine("Sec-Fetch-Site: none");
                    builder.AppendLine();
                    var requestHeaders = Encoding.ASCII.GetBytes(builder.ToString());
                    await stream.WriteAsync(requestHeaders, 0, requestHeaders.Length);
                    await stream.FlushAsync();

                    //Get response
                    stream.ReadTimeout = 1000;
                    var data = new byte[2 << 15];
                    await stream.ReadAsync(data, 0, data.Length);

                    var endOfHeadersIndex = BinarySearch(data, Encoding.ASCII.GetBytes(this.endLine)) + this.GetEndLineBytes();
                    string response = Encoding.UTF8.GetString(data, endOfHeadersIndex, data.Length - endOfHeadersIndex);

                    //Verify response
                    var responseHeaders = Encoding.ASCII.GetString(data, 0, endOfHeadersIndex);
                    if (!responseHeaders.Contains(responseContentType, StringComparison.InvariantCultureIgnoreCase))
                        response = $"Invalid Content Type but, the response is:\n {response}";

                    return response;
                }
            }
        }

        private static int BinarySearch(byte[] input, byte[] pattern)
        {
            int length = input.Length - pattern.Length + 1;
            for (int i = 0; i < length; ++i)
            {
                bool search = true;
                for (int j = 0; j < pattern.Length; ++j)
                {
                    if (input[i + j] != pattern[j])
                    {
                        search = false;
                        break;
                    }
                }
                if (search) return i;
            }
            return -1;
        }
    }
}
