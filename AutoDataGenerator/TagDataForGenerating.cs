using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDataGenerator
{
    public class TagDataForGenerating
    {
        public TagDataForGenerating(TagDataResponse data)
        {
            if (String.IsNullOrWhiteSpace(data.tag))
            {
                throw new ArgumentNullException(nameof(data.tag));
            }

            this.Tag = data.tag;

            if (!string.IsNullOrWhiteSpace(data.signalType))
            {
                this.AnalogSignal = data.signalType.ToLower().StartsWith("a");
            }
            else
            {
                this.AnalogSignal = false;
            }

            if (!String.IsNullOrWhiteSpace(data.signalRange) && data.signalRange.Contains('-'))
            {
                var signalRanges = data.signalRange.Split('-');

                if (float.TryParse(signalRanges[0], out float lowerRange) && float.TryParse(signalRanges[1], out float upperRange))
                {
                    this.UpperRange = Math.Max(lowerRange, upperRange);
                    this.LowerRange = Math.Min(lowerRange, upperRange);
                }
                else
                {
                    this.LowerRange = 0;
                    this.UpperRange = 100;
                }
            }
            if (!this.AnalogSignal)
            {
                this.LowerRange = 0;
                this.UpperRange = 1;
            }
            else if (this.AnalogSignal && this.LowerRange == this.UpperRange)
            {
                this.LowerRange = 0;
                this.UpperRange = 100;
            }
        }

        public string Tag { get; }
        public float LowerRange { get; }
        public float UpperRange { get; }
        public bool AnalogSignal { get; }
    }
}
