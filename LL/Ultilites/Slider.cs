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
            List<string> elements = _inputStream.ReadLine().Trim().Split( " " ).ToList();
            _stack.Push( RETURN_INDEX );
            elements.Add( "#" );
            int currentRule = 1;

            foreach ( var element in elements )
            {
                currentRule = CheckPoiners( currentRule, element );
            }

            if ( _stack.Count() != 0 )
            {
                Console.WriteLine( "Stack not empty" );
            }
            else
            {
                Console.WriteLine( "Stack empty" );
            }
            _movesTrack.ForEach( item => Console.WriteLine( item ) );
        }

        private bool ElementInSet( int ruleId, string element )
        {
            return _table[ ruleId - 1 ].GuideSet.Contains( element );
        }

        private int CheckPoiners( int ruleId, string element )
        {
            bool check = true;

            while ( check )
            {
                if ( ruleId == RETURN_INDEX )
                {
                    break;
                }

                if ( _table[ ruleId - 1 ].Shift && !_table[ ruleId - 1 ].End )
                {
                    check = false;
                }

                _movesTrack.Add( string.Format( "rule( {0} ), element( {1} ), stack( {2} )", ruleId, element, string.Join( " ", _stack.ToList() ) ) );

                if ( ElementInSet( ruleId, element ) )
                {
                    if ( _table[ ruleId - 1 ].ToStack )
                    {
                        _stack.Push( ruleId + 1 );
                    }

                    if ( _table[ ruleId - 1 ].Pointer == RETURN_INDEX )
                    {
                        ruleId = _stack.Pop();
                    }
                    else
                    {
                        ruleId = _table[ ruleId - 1 ].Pointer;
                    }
                }
                else if ( _table[ ruleId - 1 ].Pointer == RETURN_INDEX )
                {
                    ruleId = _stack.Pop();
                }
                else
                {
                    if ( _table[ ruleId - 1 ].Error )
                    {
                        Console.WriteLine( string.Format( "Unexepted symbol: {0}", element ) );
                        _movesTrack.ForEach( item => Console.WriteLine( item ) );
                    }
                    ++ruleId;

                }

            }

            return ruleId;
        }
    }
}
