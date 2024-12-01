using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.VisualStyles;

namespace Disk_Scheduling_Algorithm_Simulator
{
    public partial class Shortest_Seek_Time_First_Form : Form
    {

        private List<Track> requestedTracks = new List<Track>();
        private List<int> trackPath = new List<int>();
        private int numberOfTracks;

        private int highestTrack;
        private int minBoundTrack = 0;
        private int maxBoundTrack;
        private int initialCurrentHeadPosition;
        private int currentHeadPosition;

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
            //Checks if all input in the requested tracks is integer
            for (int i = 0; i < tracksTable.RowCount - 1; i++)
            {
                var cellValue = tracksTable.Rows[i].Cells[1].Value;

                if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
                {
                    MessageBox.Show("Cell is empty or null at row " + (i + 1), "Error");
                }
                else
                {
                    int val;
                    if (!int.TryParse(cellValue.ToString(), out val))
                    {
                        // If the value is not an integer
                        MessageBox.Show("Invalid integer at row " + (i + 1) + ": \n" +
                                        "Value is: " + cellValue.ToString(), "Error");
                    }
                }
            }

            //Store the input to a list
            for (int i = 0; i < tracksTable.RowCount - 1; i++)
            {
                int track = Convert.ToInt32(tracksTable.Rows[i].Cells[1].Value);
                requestedTracks.Add(new Track(track));
            }

            //Set the current head track
            initialCurrentHeadPosition = new Random().Next(0, numberOfTracks);
            currentHeadPosition = initialCurrentHeadPosition;

            //Do The main calculation
            Calculate();

            //Draw line graph
            DrawLineGraph();
        }

        private void Calculate()
        {
            while(IsFinished() == false)
            {
                //Find the nearest track
                int nearestTrack = 0;
                int nearestTrackIndex = 0;
                int lowestDifference = int.MaxValue;
                for (int i = 0; i < requestedTracks.Count; i++)
                {
                    if (requestedTracks[i].passed) continue;

                    int difference = currentHeadPosition - requestedTracks[i].track;
                    difference = Math.Abs(difference);

                    if (difference < lowestDifference)
                    {
                        lowestDifference = difference;
                        nearestTrackIndex = i;
                        nearestTrack = requestedTracks[i].track;
                    }
                }

                //Record the track path
                trackPath.Add(currentHeadPosition);

                //Mark the tracks already passed
                requestedTracks[nearestTrackIndex].passed = true;

                //Update the current head position
                currentHeadPosition = nearestTrack;
            }

            //Add the last track
            trackPath.Add(currentHeadPosition);
        }

        private void DrawLineGraph()
        {
            //Create new series
            Series trackSeries = new Series
            {
                Name = "Tracks",
                ChartType = SeriesChartType.Line,
                IsXValueIndexed = true
            };

            //Draw the line graph
            for (int i = 0; i < trackPath.Count; i++)
            {
                trackSeries.Points.AddXY(i,trackPath[i]);
            }

            //Set the series of the chart
            sstf_chart.Series.Add(trackSeries);
            MessageBox.Show(initialCurrentHeadPosition.ToString() + " : " + trackPath[0]);
        }

        private bool IsFinished()
        {
            for(int i = 0; i < requestedTracks.Count; i++)
            {
                if (requestedTracks[i].passed == false)
                {
                    return false;
                }
            }

            return true;
        }

        private void confirm_btn_Click(object sender, EventArgs e)
        {
            //Error if number of tracks is empty
            if(noOfTracks_txt.Text == string.Empty || string.IsNullOrWhiteSpace(noOfTracks_txt.Text.ToString()))
            {
                MessageBox.Show("Invalid input: Please enter a value.", "Error");
                noOfTracks_txt.Text = "";
                return;
            }

            //Checks if number of tracks input is integer
            int noOfTracks;
            if (int.TryParse(noOfTracks_txt.Text, out noOfTracks))
            {
                //If integer.
                noOfTracks_txt.ReadOnly = true;
                confirm_btn.Enabled = false;

                calculate_btn.Enabled = true;
                reset_btn.Enabled = true;

                tracksTable.ReadOnly = false;
                tracksTable.Columns[0].ReadOnly = true;
                tracksTable.DefaultCellStyle.BackColor = SystemColors.Window;

                numberOfTracks = noOfTracks;
            }
            else
            {
                //If not integer.
                MessageBox.Show("Invalid input: Only integer values are accepted.", "Error");
                noOfTracks_txt.Text = "";
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
