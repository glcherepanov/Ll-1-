using System.Collections.Generic;

namespace LL.Ultilites
{
    public interface ITableGenerator
    {
        string Generate( IReadOnlyList<string> grammar );
        IReadOnlyList<TableLine> GetTable();
    }
}
