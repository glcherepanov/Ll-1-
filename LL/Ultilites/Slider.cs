using System;
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
        private Stack<int> _stack = new Stack<int>();
        private readonly int RETURN_INDEX = -1;
        private List<string> _movesTrack = new List<string>();

        public Slider( string input, string output, IReadOnlyList<TableLine> table )
        {
            _inputStream = new StreamReader( input );
            //_outputStream = new StreamWriter( output );
            _table = table;
        }

        public void Execute()
        {
            _stack.Push( 0 );
            List<string> elements = _inputStream.ReadLine().Split( " " ).ToList();
            int currentRule = 1;

            foreach ( var element in elements )
            {
                CheckPoiners( currentRule, element );
            }

            if ( _stack.Count() != 0 )
            {
                Console.WriteLine( "Stack not empty" );
            }
            else
            {
                Console.WriteLine( "Stack empty" );
            }
        }

        private bool ElementInSet( int ruleId, string element )
        {
            return _table[ ruleId - 1 ].GuideSet.Contains( element );
        }

        private void CheckPoiners( int ruleId, string element )
        {
            bool check = true;

            while ( check )
            {
                if ( _table[ ruleId - 1 ].Shift )
                {
                    check = false;
                }

                _movesTrack.Add( string.Format( "rule( {0} ), element( {1} )", ruleId, element ) );

                if ( ElementInSet( ruleId, element ) )
                {
                    if ( _table[ ruleId - 1 ].ToStack )
                    {
                        _stack.Push( ruleId + 1 );
                    }

                    if ( _table[ ruleId - 1].Pointer == RETURN_INDEX )
                    {
                        _stack.Pop();
                    }
                }
                else
                {
                    Console.WriteLine( string.Format( "Unexepted symbol: {0}", element ) );
                    ++ruleId;

                    _movesTrack.ForEach( item => Console.WriteLine( item ) );
                }

                ruleId = _table[ ruleId - 1 ].Pointer;
            }

            ruleId = _table[ ruleId - 1 ].Pointer;
        }
    }
}
