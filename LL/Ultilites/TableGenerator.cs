using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;

namespace LL.Ultilites
{
    public class TableGenerator : ITableGenerator
    {
        private List<TableLine> _lines = new List<TableLine>();
        private bool _isLeftSide = true;
        private bool _isLast = false;
        private int _index = 0;
        private int _currentLine = 0;

        public string Generate( IReadOnlyList<string> grammar )
        {
            CheckLeftPartOfGrammar( grammar );
            _isLeftSide = false;
            CheckRightPartOfGrammar( grammar );

            UpdateNonterminalElements( grammar.Count );

            return CreateTableString();
        }

        public IReadOnlyList<TableLine> GetTable()
        {
            return _lines;
        }

        private void CheckLeftPartOfGrammar( IReadOnlyList<string> grammar )
        {

            foreach ( var grammarLine in grammar )
            {
                if ( grammarLine == string.Empty )
                {
                    break;
                }

                IReadOnlyList<string> lineElements = grammarLine.Split( ' ' );
                bool isGuideSet = false;
                List<string> guideSet = new List<string>();
                _lines.Add( GenerateTableLine( lineElements.First() ) );

                foreach ( var element in lineElements )
                {
                    if ( isGuideSet )
                    {
                        guideSet.Add( element ); 
                    }

                    if ( element == "/" )
                    {
                        isGuideSet = true;
                    }
                }

                _lines[ _lines.Count - 1 ].GuideSet = guideSet;
            }
        }

        private void CheckRightPartOfGrammar( IReadOnlyList<string> grammar )
        {
            foreach ( var grammarLine in grammar )
            {
                if ( grammarLine == string.Empty )
                {
                    break;
                }

                string line = grammarLine.Remove( 0, grammarLine.IndexOf( "->" ) + 3 );
                line = line.Remove( line.IndexOf( '/' ) - 1, line.Length - line.IndexOf( '/' ) + 1 );
                IReadOnlyList<string> lineElements = line.Split( ' ' );
                int i = 1;
                _isLast = false;

                foreach ( var element in lineElements )
                {
                    _isLast = i == lineElements.Count;
                    _lines.Add( GenerateTableLine( element ) );
                    i++;
                }
                ++_currentLine;
            }
        }

        private void UpdateNonterminalElements( int size )
        {
            for ( int i = 0; i < size; i++ )
            {
                _lines[ i ].Pointer = _lines.FirstOrDefault( item => item.Char == _lines[ i ].GuideSet.First() )?.Index ?? -1;
            }
        }

        private TableLine GenerateTableLine( string element )
        {
            if ( _isLeftSide )
            {
                string ch = element.Remove( 0, 1 );
                ch = ch.Remove( ch.Length - 1, 1 );

                TableLine prevElementLine = _lines.LastOrDefault( item => item.Char == ch );

                if ( prevElementLine != null )
                {
                    _lines[ prevElementLine.Index - 1 ].Error = false;
                }

                return new TableLine( ++_index, ch, null, false, true, -1, false, false );
            }
            else
            {
                if ( element.StartsWith( '<' ) && element.EndsWith( '>' ) )
                {
                    HashSet<string> guideSymbols = new HashSet<string>();
                    string ch = element.Remove( 0, 1 );
                    ch = ch.Remove( ch.Length - 1, 1 );

                    var nonterminalLines = _lines.FindAll( item => item.Char == ch );
                    foreach ( var line in nonterminalLines )
                    {
                        guideSymbols.UnionWith( line.GuideSet.ToHashSet() );
                    }

                    return new TableLine( ++_index, ch, guideSymbols.ToList(), false, false, nonterminalLines.First().Index, _isLast ? false : true, false );
                }
                else if ( element == "e" )
                {
                    return new TableLine( ++_index, element, _lines[ _currentLine ].GuideSet, false, true, -1, false, false );
                }
                else if ( element == "#" )
                {
                    return new TableLine( ++_index, element, new List<string>() { element }, true, true, -1, false, true );
                }
                else
                {
                    if ( element != null )
                    {
                        return new TableLine( ++_index, element, new List<string>() { element }, true, true, _isLast ? -1 : _index + 1, false, false );
                    }
                    else
                    {
                        return new TableLine( ++_index, element, null, false, true, -1, false, true );
                    }
                }
            }

        }

        private string CreateTableString()
        {
            string result = string.Format( "{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10} {6,-10} {7,-10} \n", "№",  "Сдвиг", "Ошибка", "Указатель", "В стек", "Конец", "Символ", "Множество" );

            foreach ( var line in _lines )
            {
                result += string.Format( "{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10} {6,-10} {7,-10} \n", line.Index, line.Shift, line.Error, line.Pointer, line.ToStack, line.End, line.Char, string.Join( ", ", line.GuideSet.ToArray() ) ); 
            }

            return result;
        }
    }
}
