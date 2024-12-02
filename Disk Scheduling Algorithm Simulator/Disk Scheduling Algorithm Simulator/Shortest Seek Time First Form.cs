﻿using System;
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
            //Checks if all input in the requested tracks is integer and within range
            if (!Utilities.IsTrackInputValid(tracksTable)) return;
            if (tracksTable.RowCount == 1)
            {
                MessageBox.Show("Please enter atleast one valid track", "Error");
                return;
            }

            //if(initialHeadTrack_txt.Text)
            switch (Utilities.InitialTrackHeadValidation(initialHeadTrack_txt))
            {
                case "empty":
                    //if the textbox is empty
                    //Set random current head track
                    Utilities.ResetVariables();
                    Utilities.ResetLineGraph(sstf_chart);
                    Utilities.ResetInitialHeadTrack(initialTrack_lbl);
                    Utilities.ResetOutputHeadMovements(panel5, panel6, panel7);

                    Utilities.initialCurrentHeadPosition = new Random().Next(0, Utilities.numberOfTracks);
                    Utilities.currentHeadPosition = Utilities.initialCurrentHeadPosition;
                    break;
                case "integer":
                    //if the textbox input is integer
                    //Set the textbox input as current head track
                    Utilities.ResetVariables();
                    Utilities.ResetLineGraph(sstf_chart);
                    Utilities.ResetInitialHeadTrack(initialTrack_lbl);
                    Utilities.ResetOutputHeadMovements(panel5, panel6, panel7);

                    Utilities.initialCurrentHeadPosition = Convert.ToInt32(initialHeadTrack_txt.Text);
                    Utilities.currentHeadPosition = Utilities.initialCurrentHeadPosition;
                    break;
                case "not integer":
                    //if the textbox input is not integer
                    //Make a pop up error msg
                    MessageBox.Show("Invalid input: Only integer values are accepted.", "Error");
                    return;
            }

            //Store the input to a list
            for (int i = 0; i < tracksTable.RowCount - 1; i++)
            {
                int track = Convert.ToInt32(tracksTable.Rows[i].Cells[1].Value);
                Utilities.requestedTracks.Add(new Track(track));
            }

            //Do The main calculation
            Calculate();

            //Draw line graph
            Utilities.DrawLineGraph(sstf_chart);

            //Print the initial head track
            Utilities.PrintInitialHeadTrack(initialTrack_lbl);

            //Output the number of head movements in the table below
            Utilities.OutputHeadMovements(panel5, panel6, panel7);
        }

        private void Calculate()
        {
            while(Utilities.IsFinished() == false)
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

        private void confirm_btn_Click(object sender, EventArgs e)
        {
            Utilities.NoOfTracksValidation(noOfTracks_txt, confirm_btn, calculate_btn, reset_btn, tracksTable, label5);
            Utilities.EnableInitialTraackHeadTB(initialHeadTrack_txt, true);
        }

        private void reset_btn_Click(object sender, EventArgs e)
        {
            Utilities.ResetVariables();
            Utilities.ResetNoOfTracks();
            Utilities.ResetNoOfTracksInputUI(noOfTracks_txt, confirm_btn);
            Utilities.ResetInitialTrackHeadTB(initialHeadTrack_txt);
            Utilities.EnableInitialTraackHeadTB(initialHeadTrack_txt, false);
            Utilities.EnableCalcAndResBtn(calculate_btn, reset_btn, false);
            Utilities.ResetReqTracksTable(tracksTable);
            Utilities.ResetLineGraph(sstf_chart);
            Utilities.ResetInitialHeadTrack(initialTrack_lbl);
            Utilities.ResetOutputHeadMovements(panel5,panel6,panel7);
            Utilities.ResetTracksRangeIndicator(label5);
        }
    }
}
