using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Messages
{
    public class MessageResult
    {
        public string MessageType { get; set; }
        public IEnumerable<ValueResult> Values { get; set; }

        public string GetFieldValue(string field)
            => this.Values.FirstOrDefault(d => d.Field.Equals(field, StringComparison.InvariantCultureIgnoreCase))?.Value;

        public class ValueResult
        {
            public string Field { get; set; }
            public string Value { get; set; }
        }
    }
}
