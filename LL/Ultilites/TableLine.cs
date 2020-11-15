using System.Collections.Generic;

namespace LL.Ultilites
{
    public class TableLine
    {
        public TableLine( int index, string ch, List<string> guideSet, bool shift, bool error, int pointer, bool toStack, bool end )
        {
            Index = index;
            Char = ch;
            GuideSet = guideSet;
            Shift = shift;
            Error = error;
            Pointer = pointer;
            ToStack = toStack;
            End = end;
        }

        public int Index { get; set; }
        public string Char { get; set; }
        public List<string> GuideSet { get; set; }
        public bool Shift { get; set; }
        public bool Error { get; set; }
        public int Pointer { get; set; }
        public bool ToStack { get; set; }
        public bool End { get; set; }
    }
}
