using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LL.Ultilites
{
    public class GuideSetDefiner : IGuideSetDefiner
    {
        private List<GrammaLine> _gramma = new List<GrammaLine>();
        private readonly string EMPTY = "e";
        private readonly string END = "#";
        private readonly Dictionary<string, List<string>> _followDictionary = new Dictionary<string, List<string>>();

        public void DefineGuideSetToFile( string inputGrammaFile, string outFile )
        {
            ReadGrammaFromFile( inputGrammaFile );
            DelLefthandRecursion();
            UnionLines();

            foreach ( var line in _gramma )
            {
                line.GuideSet = GetGuideSet( line );
            }

            using ( StreamWriter sw = new StreamWriter( outFile ) )
            {
                foreach ( var line in _gramma )
                {
                    sw.WriteLine( string.Format( "{0} -> {1} / {2}", line.LeftSide, string.Join( " ", line.RightSide.ToArray() ), string.Join( " ", line.GuideSet.ToArray() ) ) );
                }
            }
        }

        private void ReadGrammaFromFile( string inputFile )
        {
            IReadOnlyList<string> grammar;

            using ( StreamReader sr = new StreamReader( inputFile ) )
            {
                grammar = sr.ReadToEnd().Split( Environment.NewLine );
            }

            foreach ( var grammarLine in grammar )
            {
                bool isLeftSide = true;
                GrammaLine line = new GrammaLine();
                IReadOnlyList<string> lineElements = grammarLine.Split( ' ' );

                foreach ( var element in lineElements )
                {
                    if ( element == "=>" )
                    {
                        isLeftSide = false;
                    }
                    else if ( isLeftSide )
                    {
                        line.LeftSide = element;
                    }
                    else
                    {
                        line.RightSide.Add( element );
                    }
                }

                _gramma.Add( line );
            }
        }

        private void DelLefthandRecursion()
        {
            List<GrammaLine> newGramma = new List<GrammaLine>();

            foreach ( var line in _gramma )
            {
                List<List<string>> bN = new List<List<string>>();
                List<List<string>> aN = new List<List<string>>();

                if ( line.LeftSide == line.RightSide.First() )
                {
                    newGramma.RemoveAll( item => item.LeftSide == line.LeftSide );

                    foreach ( var newLine in _gramma )
                    {
                        if ( newLine.LeftSide == line.LeftSide && newLine.RightSide.First() != line.LeftSide )
                        {
                            bN.Add( new List<string>( newLine.RightSide ) );
                        }
                        else if ( newLine.LeftSide == line.LeftSide && newLine.RightSide.First() == line.LeftSide )
                        {
                            aN.Add( new List<string>( newLine.RightSide.Skip( 1 ) ) );
                        }
                    }

                    string newNontermonal = new string( line.LeftSide.SkipLast( 1 ).ToArray() ) + "`>";

                    foreach ( var b in bN )
                    {
                        b.Add( newNontermonal );
                        newGramma.Add( new GrammaLine()
                        {
                            LeftSide = line.LeftSide,
                            RightSide = b
                        } );
                    }

                    foreach ( var a in aN )
                    {
                        a.Add( newNontermonal );
                        newGramma.Add( new GrammaLine()
                        {
                            LeftSide = newNontermonal,
                            RightSide = a
                        } );
                    }

                    newGramma.Add( new GrammaLine
                    {
                        LeftSide = newNontermonal,
                        RightSide = new List<string>() { EMPTY }
                    } );
                }
                else
                {
                    newGramma.Add( line );
                }
            }

            _gramma = newGramma;
            _gramma.Where( item => item.LeftSide == "<E>" ).ToList().ForEach( item => item.RightSide.Add( END ) );
        }

        private void UnionLines()
        {
            var temp = new List<GrammaLine>( _gramma );

            foreach ( var line in _gramma )
            {
                if ( line.RightSide.Last().Contains( "`" ) )
                {
                    temp.RemoveAll( item => item.LeftSide == line.LeftSide && item.RightSide.SequenceEqual( line.RightSide.SkipLast( 1 ).ToList() ) );
                }
            }

            _gramma = temp;
        }

        private List<string> GetGuideSet( GrammaLine line )
        {
            if ( line.RightSide.First() == EMPTY )
            {
                return Follow( line.LeftSide );
            }
            else if ( line.RightSide.First().StartsWith( "<" ) )
            {
                return First( line.RightSide.First() );
            }
            else
            {
                return new List<string>() { line.RightSide.First() };
            }
        }

        private List<string> Follow( string nonterminal )
        {
            HashSet<string> guideSet = new HashSet<string>();

            if ( _followDictionary.ContainsKey( nonterminal ) )
            {
                return _followDictionary[ nonterminal ];
            }

            foreach ( var line in _gramma )
            {
                if ( line.RightSide.Contains( nonterminal ) )
                {
                    string element = string.Empty;
                    int elementIndex = line.RightSide.FindIndex( item => item == nonterminal );
                    try
                    {
                        element = line.RightSide[ elementIndex + 1 ];
                    }
                    catch ( Exception )
                    { }


                    if ( ( element == string.Empty || element == END || element.Contains( "`" ) ) && elementIndex == 1 )
                    {
                        guideSet.UnionWith( Follow( line.LeftSide ).ToHashSet() );
                    }

                    if ( element != string.Empty )
                    {
                        if ( element.StartsWith( "<" ) )
                        {
                            guideSet.UnionWith( First( element ).ToHashSet() );
                            guideSet.RemoveWhere( item => item == EMPTY );
                        }
                        else
                        {
                            guideSet.Add( element );
                        }
                    }
                }
            }

            if ( !_followDictionary.ContainsKey( nonterminal ) )
            {
                _followDictionary.Add( nonterminal, guideSet.ToList() );
            }

            return guideSet.ToList();
        }

        private List<string> First( string Nonterminal )
        {
            List<string> guideSet = new List<string>();

            foreach ( var line in _gramma )
            {
                if ( line.LeftSide == Nonterminal )
                {
                    if ( line.RightSide.First() != Nonterminal && line.RightSide.First().StartsWith( "<" ) )
                    {
                        guideSet.AddRange( First( line.RightSide.First() ) );
                    }
                    else if ( !line.RightSide.First().StartsWith( "<" ) )
                    {
                        guideSet.Add( line.RightSide.First() );
                    }
                }
            }

            return guideSet;
        }
    }
}
