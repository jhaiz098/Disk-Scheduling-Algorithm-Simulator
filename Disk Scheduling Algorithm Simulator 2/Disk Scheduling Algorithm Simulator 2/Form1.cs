using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Disk_Scheduling_Algorithm_Simulator_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
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

        private void confirm_btn_Click(object sender, EventArgs e)
        {
            Utilities.NoOfTracksValidation(noOfTracks_txt, confirm_btn, calculate_btn, reset_btn, tracksTable, label5);
            Utilities.EnableInitialTraackHeadTB(initialHeadTrack_txt, true);
            Utilities.EnableDiskAlgoSelectorCB(diskAlgoSelector_cb, true);
        }

        private void calculate_btn_Click(object sender, EventArgs e)
        {
            //Pop up an error msg if selected no disk algorithm
            if (diskAlgoSelector_cb.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a disk algorithm from the dropdown list.", "Error");
                return;
            }

            //Checks if all input in the requested tracks is integer
            if (!Utilities.IsTrackInputValid(tracksTable)) return;

            //Check if input is within range
            if (!Utilities.CheckInputRange(tracksTable)) return;

            //Validates the initial track head textbox
            switch (Utilities.InitialTrackHeadValidation(initialHeadTrack_txt))
            {
                case "empty":
                    //if the textbox is empty

                    //Reset things
                    Utilities.ResetVariables();
                    Utilities.ResetLineGraph(track_chart);
                    Utilities.ResetInitialHeadTrack(initialTrack_lbl);
                    Utilities.ResetOutputHeadMovements(panel5, panel6, panel7);

                    //Set random current head track
                    Utilities.initialCurrentHeadPosition = new Random().Next(0, Utilities.numberOfTracks);
                    Utilities.currentHeadPosition = Utilities.initialCurrentHeadPosition;
                    break;
                case "valid":
                    //if the textbox input is in valid range

                    //Reset things
                    Utilities.ResetVariables();
                    Utilities.ResetLineGraph(track_chart);
                    Utilities.ResetInitialHeadTrack(initialTrack_lbl);
                    Utilities.ResetOutputHeadMovements(panel5, panel6, panel7);

                    //Set the textbox input as current head track
                    Utilities.initialCurrentHeadPosition = Convert.ToInt32(initialHeadTrack_txt.Text);
                    Utilities.currentHeadPosition = Utilities.initialCurrentHeadPosition;
                    break;
                case "invalid":
                    //if the textbox input is not in valid range
                    //Make a pop up error msg

                    MessageBox.Show("Invalid input: Enter tracks (0 - " + (Utilities.numberOfTracks - 1) + ")", "Error");
                    return;
                case "not integer":
                    //if the textbox input is not integer
                    //Make a pop up error msg
                    MessageBox.Show("Invalid input: Only integer values are accepted.", "Error");
                    return;
            }

            //Store the input to a list
            Utilities.StoreInputToList(tracksTable);

            //Do The main calculation
            Calculate();

            //Draw line graph
            Utilities.DrawLineGraph(track_chart);

            //Print the initial head track
            Utilities.PrintInitialHeadTrack(initialTrack_lbl);

            //Output the number of head movements in the table below
            Utilities.OutputHeadMovements(panel5, panel6, panel7);
        }

        private void Calculate()
        {
            switch (diskAlgoSelector_cb.SelectedIndex)
            {
                case 0:
                        CalculateShortestSeekTimeFirst();
                    break;
                case 1:
                        CalculateScanDisk();
                    break;
                case 2:
                        CalculateCLook();
                    break;
            }
        }

        private void CalculateShortestSeekTimeFirst()
        {
            while (Utilities.IsFinished() == false)
            {
                //Find the nearest track
                int nearestTrack = 0;
                int nearestTrackIndex = 0;
                int lowestDifference = int.MaxValue;
                for (int i = 0; i < Utilities.requestedTracks.Count; i++)
                {
                    if (Utilities.requestedTracks[i].passed) continue;

                    int difference = Utilities.currentHeadPosition - Utilities.requestedTracks[i].track;
                    difference = Math.Abs(difference);

                    if (difference < lowestDifference)
                    {
                        lowestDifference = difference;
                        nearestTrackIndex = i;
                        nearestTrack = Utilities.requestedTracks[i].track;
                    }
                }

                //Record the track path
                Utilities.trackPath.Add(Utilities.currentHeadPosition);

                //Mark the tracks already passed
                Utilities.requestedTracks[nearestTrackIndex].passed = true;

                //Update the current head position
                Utilities.currentHeadPosition = nearestTrack;
            }

            //Add the last track
            Utilities.trackPath.Add(Utilities.currentHeadPosition);
        }

        private void CalculateScanDisk()
        {
            bool ascending = true;
            int nearestTrackIndex = 0;
            while (Utilities.IsFinished() == false)
            {
                int nearestTrack = -1 ;
                int nearestTrackDistance = int.MaxValue;
                
                if (ascending)
                {
                    // Checks which track is greater than and nearest to the current head track
                    for (int i = 0; i < Utilities.requestedTracks.Count; i++)
                    {
                        // if the current track is greater than the current track position
                        if (Utilities.requestedTracks[i].track >= Utilities.currentHeadPosition && Utilities.requestedTracks[i].passed == false)
                        {
                            int tempDistanceToCurrentHeadTrack = Utilities.requestedTracks[i].track - Utilities.currentHeadPosition;
                            tempDistanceToCurrentHeadTrack = Math.Abs(tempDistanceToCurrentHeadTrack);

                            if (tempDistanceToCurrentHeadTrack < nearestTrackDistance)
                            {
                                nearestTrack = Utilities.requestedTracks[i].track;
                                nearestTrackIndex = i;
                                nearestTrackDistance = tempDistanceToCurrentHeadTrack;
                            }
                        }
                    }

                    if (nearestTrack == -1)
                    {
                        Utilities.trackPath.Add(Utilities.currentHeadPosition);
                        Utilities.requestedTracks[nearestTrackIndex].passed = true;

                        Utilities.currentHeadPosition = Utilities.numberOfTracks - 1;
                        
                        ascending = false;
                        continue;
                    }
                }
                else
                {
                    // Checks which track is less than and nearest to the current head track
                    for (int i = 0; i < Utilities.requestedTracks.Count; i++)
                    {
                        // if the current track is less than the current track position
                        if (Utilities.requestedTracks[i].track <= Utilities.currentHeadPosition && Utilities.requestedTracks[i].passed == false)
                        {
                            int tempDistanceToCurrentHeadTrack = Utilities.requestedTracks[i].track - Utilities.currentHeadPosition;
                            tempDistanceToCurrentHeadTrack = Math.Abs(tempDistanceToCurrentHeadTrack);

                            if (tempDistanceToCurrentHeadTrack < nearestTrackDistance)
                            {
                                nearestTrack = Utilities.requestedTracks[i].track;
                                nearestTrackIndex = i;
                                nearestTrackDistance = tempDistanceToCurrentHeadTrack;
                            }
                        }
                    }
                }

                //Record the track path
                Utilities.trackPath.Add(Utilities.currentHeadPosition);

                //Mark the tracks already passed
                Utilities.requestedTracks[nearestTrackIndex].passed = true;

                //Update the current head position
                Utilities.currentHeadPosition = nearestTrack;
            }

            //Add the last track
            Utilities.trackPath.Add(Utilities.currentHeadPosition);
        }

        private void CalculateCLook()
        {
            while (Utilities.IsFinished() == false)
            {
                int nearestTrack = -1;
                int nearestTrackIndex = 0;
                int nearestTrackDistance = int.MaxValue;

                for (int i = 0; i < Utilities.requestedTracks.Count; i++)
                {
                    if (Utilities.requestedTracks[i].track > Utilities.currentHeadPosition && Utilities.requestedTracks[i].passed == false)
                    {
                        int tempDistanceToCurrentHeadTrack = Utilities.requestedTracks[i].track - Utilities.currentHeadPosition;
                        tempDistanceToCurrentHeadTrack = Math.Abs(tempDistanceToCurrentHeadTrack);

                        if (tempDistanceToCurrentHeadTrack < nearestTrackDistance)
                        {
                            nearestTrack = Utilities.requestedTracks[i].track;
                            nearestTrackIndex = i;
                            nearestTrackDistance = tempDistanceToCurrentHeadTrack;
                        }
                    }
                }

                int lowestTrack = 0;
                int lowestTrackIndex = 0;
                int lowestTrackDistance = int.MinValue;

                for (int i = 0; i < Utilities.requestedTracks.Count; i++)
                {
                    if (nearestTrack == -1)
                    {
                        for (int o = 0; o < Utilities.requestedTracks.Count; o++)
                        {
                            if (Utilities.requestedTracks[i].track <= Utilities.currentHeadPosition && Utilities.requestedTracks[i].passed == false)
                            {
                                int tempDistanceToCurrentHeadTrack = Utilities.requestedTracks[i].track - Utilities.currentHeadPosition;
                                tempDistanceToCurrentHeadTrack = Math.Abs(tempDistanceToCurrentHeadTrack);

                                if (tempDistanceToCurrentHeadTrack > lowestTrackDistance)
                                {
                                    lowestTrack = Utilities.requestedTracks[i].track;
                                    lowestTrackIndex = i;
                                    lowestTrackDistance = tempDistanceToCurrentHeadTrack;
                                }
                            }
                        }
                    }
                }

                if(nearestTrack != -1)
                {
                    //Record the track path
                    Utilities.trackPath.Add(Utilities.currentHeadPosition);

                    //Mark the tracks already passed
                    Utilities.requestedTracks[nearestTrackIndex].passed = true;

                    //Update the current head position
                    Utilities.currentHeadPosition = nearestTrack;
                }
                else
                {
                    //Record the track path
                    Utilities.trackPath.Add(Utilities.currentHeadPosition);

                    //set current head position to the lowest track
                    Utilities.currentHeadPosition = lowestTrack;

                    //Mark the tracks already passed
                    Utilities.requestedTracks[lowestTrackIndex].passed = true;
                }
            }

            //Add the last track
            Utilities.trackPath.Add(Utilities.currentHeadPosition);
        }

        private void reset_btn_Click(object sender, EventArgs e)
        {
            Utilities.ResetVariables();
            Utilities.ResetNoOfTracks();
            Utilities.ResetNoOfTracksInputUI(noOfTracks_txt, confirm_btn);
            Utilities.ResetInitialTrackHeadTB(initialHeadTrack_txt);
            Utilities.EnableInitialTraackHeadTB(initialHeadTrack_txt, false);
            Utilities.ResetDiskAlgoSelector(diskAlgoSelector_cb);
            Utilities.EnableDiskAlgoSelectorCB(diskAlgoSelector_cb, false);
            Utilities.EnableCalcAndResBtn(calculate_btn, reset_btn, false);
            Utilities.ResetReqTracksTable(tracksTable);
            Utilities.ResetLineGraph(track_chart);
            Utilities.ResetInitialHeadTrack(initialTrack_lbl);
            Utilities.ResetOutputHeadMovements(panel5, panel6, panel7);
            Utilities.ResetTracksRangeIndicator(label5);
        }
    }
}
