using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Disk_Scheduling_Algorithm_Simulator
{
    public partial class Shortest_Seek_Time_First_Form : Form
    {
        public Shortest_Seek_Time_First_Form()
        {
            InitializeComponent();
        }

        private void Shortest_Seek_Time_First_Form_Load(object sender, EventArgs e)
        {
            Utilities.ResizeNoOfReqFirstColumn(tracksTable);
        }

        private void tracksTable_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            Utilities.UpdateNoOfReqTracks(tracksTable);
        }

        private void tracksTable_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            Utilities.UpdateNoOfReqTracks(tracksTable);
        }
    }
}
