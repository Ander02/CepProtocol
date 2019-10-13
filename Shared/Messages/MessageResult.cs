using System.Collections.Generic;

namespace Shared.Messages
{
    public class MessageResult
    {
        public string MessageType { get; set; }
        public List<ValueResult> Values { get; set; }

        public class ValueResult
        {
            public string Field { get; set; }
            public string Value { get; set; }
        }
    }
}
