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
            ResetVariables();
            ResetLineGraph();
            ResetInitialHeadTrack();
            ResetOutputHeadMovements();

            //Checks if all input in the requested tracks is integer and within range
            if (!IsTrackInputValid()) return;

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

            //Print the initial head track
            PrintInitialHeadTrack();

            //Output the number of head movements in the table below
            OutputHeadMovements();
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
                IsXValueIndexed = true,
                Color = Color.Red
            };

            ChartArea trackArea = sstf_chart.ChartAreas[0];

            //Draw the line graph
            for (int i = 0; i < trackPath.Count; i++)
            {
                //Adding points in the line graph
                trackSeries.Points.AddXY(i, trackPath[i]);

                //Styling the points
                trackSeries.Points[i].Label = trackSeries.Points[i].YValues[0].ToString();
                trackSeries.Points[i].LabelForeColor = Color.Red;
                trackSeries.Points[i].LabelBackColor = Color.White;
                trackSeries.Points[i].Font = new Font(trackSeries.Points[i].Font.FontFamily, 10, FontStyle.Bold);
                trackSeries.Points[i].MarkerStyle = MarkerStyle.Circle;
                trackSeries.Points[i].MarkerSize = 10;
            }

            trackArea.AxisY.Maximum = numberOfTracks - 1;
            trackArea.AxisY.Minimum = 0;

            //Set the series of the chart
            sstf_chart.Series.Add(trackSeries);
        }

        private void PrintInitialHeadTrack()
        {
            initialTrack_lbl.Text = initialCurrentHeadPosition.ToString();
        }

        private void OutputHeadMovements()
        {
            List<int> trackPathDif = new List<int>();
            int trackPathSum = 0;

            Label label = new Label
            {
                Font = new Font("Consolas", 11),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };

            Label label1 = new Label
            {
                Font = new Font("Consolas", 11),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };

            Label label2 = new Label
            {
                Font = new Font("Consolas", 11),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };

            label.Text = "= ";
            label1.Text = "= ";
            label2.Text = "= ";

            //Display the subtraction of the tracks movements
            for (int i = 0; i < trackPath.Count; i++)
            {
                if (i == trackPath.Count - 1) break;

                if (trackPath[i] > trackPath[i + 1])
                {
                    label.Text += "(" + trackPath[i] + " - " + trackPath[i + 1] + ")";
                    trackPathDif.Add(trackPath[i] - trackPath[i + 1]);
                }
                else
                {
                    label.Text += "(" + trackPath[i + 1] + " - " + trackPath[i] + ")";
                    trackPathDif.Add(trackPath[i + 1] - trackPath[i]);
                }

                if (i != trackPath.Count - 2)
                {
                    label.Text += " + ";
                }
            }

            //Display the addition of the track movements
            for (int i = 0; i < trackPathDif.Count; i++)
            {
                label1.Text += trackPathDif[i];

                if (i != trackPathDif.Count - 1)
                {
                    label1.Text += " + ";
                }

                //Get the sum of the difference
                trackPathSum += trackPathDif[i];
            }

            label2.Text += trackPathSum.ToString() + " Tracks";

            panel5.Controls.Add(label);
            panel6.Controls.Add(label1);
            panel7.Controls.Add(label2);

        }

        private bool IsTrackInputValid()
        {
            bool isValid = true;
            //Checks if all input in the requested tracks is integer and within range
            for (int i = 0; i < tracksTable.RowCount - 1; i++)
            {
                var cellValue = tracksTable.Rows[i].Cells[1].Value;

                if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
                {
                    MessageBox.Show("Cell is empty or null at row " + (i + 1), "Error");

                    if(isValid) isValid = false;
                }
                else
                {
                    int val;
                    if (!int.TryParse(cellValue.ToString(), out val))
                    {
                        // If the value is not an integer
                        MessageBox.Show("Invalid integer at row " + (i + 1) + ": \n" +
                                        "Value is: " + cellValue.ToString(), "Error");

                        if (isValid) isValid = false; isValid = false;
                    }
                    else
                    {
                        if (Convert.ToInt32(cellValue) < 0 || Convert.ToInt32(cellValue) >= numberOfTracks)
                        {
                            MessageBox.Show("Row " + (i + 1) + " is outside the valid track range.", "Error");

                            if (isValid) isValid = false; isValid = false;
                        }
                    }
                }
            }
            return isValid;
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

                //Change the text of the label below
                label5.Text += ": Enter tracks (0 - " + (numberOfTracks - 1) + ")";
            }
            else
            {
                //If not integer.
                MessageBox.Show("Invalid input: Only integer values are accepted.", "Error");
                noOfTracks_txt.Text = "";
            }

        }

        private void reset_btn_Click(object sender, EventArgs e)
        {
            ResetVariables();
            ResetNoOfTracks();
            ResetNoOfTracksInputUI();
            EnableCalcAndResBtn(false);
            ResetReqTracksTable();
            ResetLineGraph();
            ResetOutputHeadMovements();
            ResetTracksRangeIndicator();
            ResetInitialHeadTrack();
        }

        private void ResetVariables()
        {
            //Resets global variables
            requestedTracks.Clear();
            trackPath.Clear();
            initialCurrentHeadPosition = 0;
            currentHeadPosition = 0;
        }

        private void ResetNoOfTracks()
        {
            numberOfTracks = 0;
        }

        private void ResetNoOfTracksInputUI()
        {
            //Resets no of tracks text field and button
            noOfTracks_txt.Text = "";
            noOfTracks_txt.ReadOnly = false;
            confirm_btn.Enabled = true;
        }
           
        private void EnableCalcAndResBtn(bool enable)
        {
            //Enable or disable calc and reset btn
            calculate_btn.Enabled = enable;
            reset_btn.Enabled = enable;
        }

        private void ResetReqTracksTable()
        {
            tracksTable.ReadOnly = true;
            tracksTable.Rows.Clear();
            tracksTable.DefaultCellStyle.BackColor = SystemColors.WindowFrame;
        }

        private void ResetLineGraph()
        {
            sstf_chart.Series.Clear();
        }

        private void ResetInitialHeadTrack()
        {
            initialTrack_lbl.Text = "";
        }

        private void ResetOutputHeadMovements()
        {
            panel5.Controls.Clear();
            panel6.Controls.Clear();
            panel7.Controls.Clear();
        }

        private void ResetTracksRangeIndicator()
        {
            label5.Text = "Enter requested tracks";
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
