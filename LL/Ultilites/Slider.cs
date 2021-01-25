using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LL.Ultilites
{
    public class Slider : ISlider
    {
        private StreamReader _inputStream;
        private StreamWriter _outputStream;
        private IReadOnlyList<TableLine> _table;
        private Stack<string> stack = new Stack<string>();

        public Slider( string text, StreamWriter outputStream, IReadOnlyList<TableLine> table )
        {
            _inputStream = new StreamReader( text );
            _outputStream = outputStream;
            _table = table;
        }

        public void Execute()
        {
            stack.Push( _table.First().Char );
        }
    }
}
