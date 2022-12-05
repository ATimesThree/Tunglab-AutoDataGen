using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDataGenerator
{
    public class CustomKeyValuePair
    {
        public string Tag { get; set; }
        public double NormalValue { get; set; }
        public double AbnormalValue { get; set; }
        public SignalType SignalType { get; set; } = SignalType.Analog;
    }
}
