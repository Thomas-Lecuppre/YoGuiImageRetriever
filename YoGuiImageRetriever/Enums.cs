using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoGuiImageRetriever
{
    public enum ImageRatio
    {
        [Description("Custom size")]
        [Ratio(512,512)]
        Undefine,
        [Description("Large image size (421x614 px)")]
        [Ratio(421, 614)]
        FullCard,
        [Description("Small image size (169x246 px)")]
        [Ratio(169, 246)]
        SmallCard,
        [Description("Yu gi oh character size (624x624 px)")]
        [Ratio(624, 624)]
        CroppedCard
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class RatioAttribute : Attribute
    {
        public int Width { get; }
        public int Height { get; }

        public RatioAttribute(int numerator, int denominator)
        {
            Width = numerator;
            Height = denominator;
        }
    }
}
