using System.Collections.Generic;
using System.Linq;

namespace Shared.Messages
{
    public class MessageBuilder
    {
        private string messageType;
        private char separator;
        private IList<(string fieldName, string fieldValue)> values;

        public MessageBuilder() : this(string.Empty, Constants.DefaultSeparator) { }

        public MessageBuilder(char separator) : this(string.Empty, separator) { }

        public MessageBuilder(string messageType, char separator)
        {
            this.messageType = messageType;
            this.separator = separator;
            this.values = new List<(string fieldName, string fieldValue)>();
        }

        public void AddField(string fieldName, string fieldValue)
        {
            this.values.Add((fieldName, fieldValue));
        }

        public void AddSucess()
        {
            this.AddField("SUCESSO", true.ToString());
        }

        public void AddFailure(string descricao)
        {
            this.AddField("SUCESSO", false.ToString());
            this.AddField("DESCRICAO", descricao);
        }

        public string BuildMessage()
        {
            if (string.IsNullOrWhiteSpace(messageType))
                return string.Empty;

            return $"MESSAGE={this.messageType}{separator}{BuildValues()}";
        }

        public string BuildValues()
        {
            if (!this.values.Any())
                return string.Empty;

            return $"{this.values.Select(d => $"{d.fieldName}={d.fieldValue}").Aggregate((s1, s2) => $"{s1}{separator}{s2}")}";
        }

        public void CleanValue()
        {
            this.values = new List<(string fieldName, string fieldValue)>();
        }

        public void SetMessage(string newMessageType)
        {
            this.messageType = newMessageType;
        }

        public void SetSeparator(char separator)
        {
            this.separator = separator;
        }
    }
}
