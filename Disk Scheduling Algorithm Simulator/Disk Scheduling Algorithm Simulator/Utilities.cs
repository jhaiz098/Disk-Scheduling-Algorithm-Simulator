using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Disk_Scheduling_Algorithm_Simulator
{
    internal class Utilities
    {
        public static List<Track> requestedTracks = new List<Track>();
        public static List<int> trackPath = new List<int>();
        public static int numberOfTracks;
        public static int initialCurrentHeadPosition;
        public static int currentHeadPosition;

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

        public static void ResetVariables()
        {
            //Resets global variables
            requestedTracks.Clear();
            trackPath.Clear();
            initialCurrentHeadPosition = 0;
            currentHeadPosition = 0;
        }

        public static void ResetNoOfTracks()
        {
            numberOfTracks = 0;
        }

        public static void ResetNoOfTracksInputUI(TextBox noOfTracks_txt, Button confirm_btn)
        {
            //Resets no of tracks text field and button
            noOfTracks_txt.Text = string.Empty;
            noOfTracks_txt.ReadOnly = false;
            confirm_btn.Enabled = true;
        }

        public static void ResetInitialTrackHeadTB(TextBox tb)
        {
            tb.Text = string.Empty;
        }

        public static void EnableInitialTraackHeadTB(TextBox tb, bool enable)
        {
            tb.Enabled = enable;
        }

        public static void EnableCalcAndResBtn(Button calculate_btn, Button reset_btn, bool enable)
        {
            //Enable or disable calc and reset btn
            calculate_btn.Enabled = enable;
            reset_btn.Enabled = enable;
        }

        public static void ResetReqTracksTable(DataGridView tracksTable)
        {
            tracksTable.ReadOnly = true;
            tracksTable.Rows.Clear();
            tracksTable.DefaultCellStyle.BackColor = SystemColors.WindowFrame;
        }

        public static void ResetLineGraph(Chart sstf_chart)
        {
            sstf_chart.Series.Clear();
        }

        public static void ResetInitialHeadTrack(Label initialTrack_lbl)
        {
            initialTrack_lbl.Text = string.Empty;
        }

        public static void ResetOutputHeadMovements(Panel panel5, Panel panel6, Panel panel7)
        {
            panel5.Controls.Clear();
            panel6.Controls.Clear();
            panel7.Controls.Clear();
        }

        public static void ResetTracksRangeIndicator(Label label5)
        {
            label5.Text = "Enter requested tracks";
        }

        public static bool IsFinished()
        {
            for (int i = 0; i < requestedTracks.Count; i++)
            {
                if (requestedTracks[i].passed == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsTrackInputValid(DataGridView tracksTable)
        {
            bool isValid = true;
            //Checks if all input in the requested tracks is integer and within range
            for (int i = 0; i < tracksTable.RowCount - 1; i++)
            {
                var cellValue = tracksTable.Rows[i].Cells[1].Value;

                if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
                {
                    MessageBox.Show("Cell is empty or null at row " + (i + 1), "Error");

                    if (isValid) isValid = false;
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
                        if (Convert.ToInt32(cellValue) < 0 || Convert.ToInt32(cellValue) >= Utilities.numberOfTracks)
                        {
                            MessageBox.Show("Row " + (i + 1) + " is outside the valid track range.", "Error");

                            if (isValid) isValid = false; isValid = false;
                        }
                    }
                }
            }
            return isValid;
        }

        public static void DrawLineGraph(Chart chart)
        {
            //Create new series
            Series trackSeries = new Series
            {
                Name = "Tracks",
                ChartType = SeriesChartType.Line,
                IsXValueIndexed = true,
                Color = Color.Red
            };

            ChartArea trackArea = chart.ChartAreas[0];


            //Draw the line graph
            for (int i = 0; i < Utilities.trackPath.Count; i++)
            {
                //Adding points in the line graph
                trackSeries.Points.AddXY(i, Utilities.trackPath[i]);

                //Styling the points
                trackSeries.Points[i].Label = trackSeries.Points[i].YValues[0].ToString();
                trackSeries.Points[i].LabelForeColor = Color.White;
                trackSeries.Points[i].LabelBackColor = Color.Black;
                trackSeries.Points[i].Font = new Font(trackSeries.Points[i].Font.FontFamily, 10, FontStyle.Bold);
                trackSeries.Points[i].MarkerStyle = MarkerStyle.Circle;
                trackSeries.Points[i].MarkerSize = 10;
            }

            trackArea.AxisY.Maximum = Utilities.numberOfTracks - 1;
            trackArea.AxisY.Minimum = 0;
            trackArea.AxisY.Interval = 1;

            // Set Y-axis labels to only correspond to the track points
            trackArea.AxisY.LabelStyle.Interval = 0; // Disable automatic label intervals
            trackArea.AxisY.LabelStyle.IsStaggered = false;

            // Enable grid lines only at points of interest (this will be removed)
            trackArea.AxisY.MajorGrid.Enabled = false;  // Disable grid lines for Y-axis
            trackArea.AxisX.MajorGrid.Enabled = false;  // Disable grid lines for X-axis

            //Set the series of the chart
            chart.Series.Add(trackSeries);
        }

        public static void PrintInitialHeadTrack(Label initialTrack_lbl)
        {
            initialTrack_lbl.Text = Utilities.initialCurrentHeadPosition.ToString();
        }

        public static void OutputHeadMovements(Panel panel5, Panel panel6, Panel panel7)
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
            for (int i = 0; i < Utilities.trackPath.Count; i++)
            {
                if (i == Utilities.trackPath.Count - 1) break;

                if (Utilities.trackPath[i] > Utilities.trackPath[i + 1])
                {
                    label.Text += "(" + Utilities.trackPath[i] + " - " + Utilities.trackPath[i + 1] + ")";
                    trackPathDif.Add(Utilities.trackPath[i] - Utilities.trackPath[i + 1]);
                }
                else
                {
                    label.Text += "(" + Utilities.trackPath[i + 1] + " - " + Utilities.trackPath[i] + ")";
                    trackPathDif.Add(Utilities.trackPath[i + 1] - Utilities.trackPath[i]);
                }

                if (i != Utilities.trackPath.Count - 2)
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

        public static string InitialTrackHeadValidation(TextBox tb)
        {
            //Error if textbox is empty
            if (tb.Text == string.Empty || string.IsNullOrWhiteSpace(tb.Text.ToString()))
            {
                return "empty";
            }
            else
            {
                //Check if textbox input is integer
                int val;
                if (int.TryParse(tb.Text, out val))
                {
                    // If integer
                    return "integer";
                }
                else
                {
                    // If not integer
                    return "not integer";
                }
            }
        }

        public static void NoOfTracksValidation(TextBox noOfTracks_txt, Button confirm_btn, Button calculate_btn, Button reset_btn, DataGridView tracksTable, Label label5)
        {
            //Error if number of tracks is empty
            if (noOfTracks_txt.Text == string.Empty || string.IsNullOrWhiteSpace(noOfTracks_txt.Text.ToString()))
            {
                MessageBox.Show("Invalid input: Please enter a value.", "Error");
                noOfTracks_txt.Text = string.Empty;
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
                label5.Text += ": Enter tracks (0 - " + (Utilities.numberOfTracks - 1) + ")";
            }
            else
            {
                //If not integer.
                MessageBox.Show("Invalid input: Only integer values are accepted.", "Error");
                noOfTracks_txt.Text = string.Empty;
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
