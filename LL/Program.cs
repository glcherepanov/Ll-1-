using LL.Ultilites;
using System;
using System.Collections.Generic;
using System.IO;

namespace LL
{
    class Program
    {
        static void Main( string[] args )
        {
            ITableGenerator generator = new TableGenerator();
            IGuideSetDefiner guideSetDefiner = new GuideSetDefiner();
            IReadOnlyList<string> grammar; 

            guideSetDefiner.DefineGuideSetToFile( "../../../test4.txt", "../../../outputGuideSet.txt" );
         
            using ( StreamReader sr = new StreamReader( "../../../outputGuideSet.txt" ) )
            {
                grammar = sr.ReadToEnd().Split( Environment.NewLine );
            }

            using ( StreamWriter sw = new StreamWriter( "../../../outputTable.txt" ) )
            {
                sw.WriteLine( generator.Generate( grammar ) );
            }
            Lexer.Program.Tokenize("../../../inputLine.txt");

            ISlider slider = new Slider( "../../../tokenizedInputLine.txt", "",  generator.GetTable() );
            slider.Execute();

        }
    }
}
