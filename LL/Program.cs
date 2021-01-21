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

            using ( StreamReader sr = new StreamReader( "../../../test2.txt" ) )
            {
                grammar = sr.ReadToEnd().Split( Environment.NewLine );
            }

            //Console.WriteLine( generator.Generate( grammar ) );

            guideSetDefiner.DefineGuideSetToFile( "../../../test3.txt", "../../../outputGuideSet.txt" );
        }
    }
}
