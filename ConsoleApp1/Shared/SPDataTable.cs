using System.Collections.Generic;

namespace SPLib.Shared
{
    public class SPSimpleDataTable
    {
        public SPSimpleDataTable(dynamic table)
        {
            Rows = new List<SPSimpleDataRow>();

            foreach(var row in table.Rows.results)
            {
                Rows.Add(new SPSimpleDataRow(row));
            }
        }

        public int RowCount { get; set; }
        public List<SPSimpleDataRow> Rows { get; set; }
    }
}
