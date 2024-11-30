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

        public List<Track> requestedTracks = new List<Track>();
        public List<int> trackPath = new List<int>();
        public int highestTrack;
        public int minBoundTrack = 0;
        public int maxBoundTrack;
        public int initialCurrentHeadTrack;
        public int currentHeadTrack;

        public Shortest_Seek_Time_First_Form()
        {
            InitializeComponent();
        }

        private void Shortest_Seek_Time_First_Form_Load(object sender, EventArgs e)
        {
            //Resize the first column of the input table to 20%
            Utilities.ResizeNoOfReqFirstColumn(tracksTable);
        }

        private void tracksTable_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //Update the no. of requested tracks column 
            Utilities.UpdateNoOfReqTracks(tracksTable);
        }

        private void tracksTable_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            //Update the no. of requested tracks column 
            Utilities.UpdateNoOfReqTracks(tracksTable);
        }

        private void calculate_btn_Click(object sender, EventArgs e)
        {
            //Store the input to a list
            for (int i = 0; i < tracksTable.Rows.Count - 1; i++)
            {
                int track = Convert.ToInt32(tracksTable.Rows[i].Cells[1].Value);
                requestedTracks.Add(new Track(track));
            }

            //Store the highest track 
            highestTrack = Utilities.GetHighestTrack(requestedTracks);

            //Set the max track
            maxBoundTrack = highestTrack - 1;

            //Set the current head track
            initialCurrentHeadTrack = new Random().Next(0, highestTrack);
            currentHeadTrack = initialCurrentHeadTrack;

            //Inside is the main calculations
            Calculate();
        }

        private void Calculate()
        {
            //Main loop
            for(int i = 0; i < requestedTracks.Count + 1; i++)
            {
                /*Will check which track is nearest to the current head track
                and store it to variable*/
                int nearestTrack = currentHeadTrack;
                for(int o = 0; o < requestedTracks.Count; o++)
                {
                    if (requestedTracks[i].track < nearestTrack && 
                        requestedTracks[i].passed == false)
                    {
                        nearestTrack = requestedTracks[i].track;
                    }
                }

                /*Add the current head track to a variable to be use for 
                the line graph later*/
                trackPath.Add(currentHeadTrack);

                //Set the current head track to the nearest track
                currentHeadTrack = nearestTrack;
            }
        }
    }

    public class Track
    {
        public int track;
        public bool passed = false;

        public Track(int track)
        {
            this.track = track;
        }
    }
}
