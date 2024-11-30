using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Disk_Scheduling_Algorithm_Simulator
{
    internal class Utilities
    {
        public static void UpdateNoOfReqTracks(DataGridView tracksTable)
        {
            for (int i = 0; i < tracksTable.Rows.Count - 1; i++)
            {
                tracksTable.Rows[i].Cells[0].Value = i + 1;
            }
        }

        public static void ResizeNoOfReqFirstColumn(DataGridView tracksTable)
        {
            tracksTable.Columns[0].Width = Convert.ToInt32(tracksTable.Width * 0.2f);
        }
    }
}
