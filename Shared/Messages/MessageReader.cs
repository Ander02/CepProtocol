using System.Collections.Generic;
using System.Linq;

namespace Shared.Messages
{
    public class MessageReader
    {
        private readonly char separator;
        public MessageReader(char separator)
        {
            this.separator = separator;
        }

        public MessageResult Read(string message)
        {
            var result = new MessageResult();

            var separatedFields = message.Split(this.separator);

            var firstParamMessage = separatedFields.FirstOrDefault();

            if (!firstParamMessage.Contains("MESSAGE="))
                return null;

            result.MessageType = firstParamMessage.Replace("MESSAGE=", "");
            result.Values = separatedFields.Except(new List<string> { firstParamMessage })
                                           .Select(d => d.Split('='))
                                           .Select(d => new MessageResult.ValueResult
                                           {
                                               Field = d[0],
                                               Value = d[1]
                                           })
                                           .ToList();

            return result;
        }
    }
}
