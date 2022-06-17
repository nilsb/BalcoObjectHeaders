using System.Collections.Generic;

namespace SPLib.Shared
{
    public class SPSimpleDataRow
    {
        public SPSimpleDataRow(dynamic row)
        {
            Cells = new Dictionary<string, dynamic>();

            foreach(var cell in row.Cells.results)
            {
                Cells.Add((string)cell.Key.Value, cell.Value);
            }
        }

        public Dictionary<string, dynamic> Cells { get; set; }
    }
}
