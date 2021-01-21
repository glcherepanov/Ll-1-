using System.Collections.Generic;

namespace LL.Ultilites
{
    public class GrammaLine
    {
        public GrammaLine()
        {
            RightSide = new List<string>();
            GuideSet = new List<string>();
        }

        public string LeftSide { get; set; }
        public List<string> RightSide { get; set; }
        public List<string> GuideSet { get; set; }
    }
}
