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
    public partial class Scan_Disk_Form : Form
    {
        public Scan_Disk_Form()
        {
            InitializeComponent();
        }

        private void Scan_Disk_Form_Load(object sender, EventArgs e)
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
            Utilities.ResetOutputHeadMovements(panel5, panel6, panel7);
            Utilities.ResetTracksRangeIndicator(label5);
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

            //Validates the initial track head textbox
            switch (Utilities.InitialTrackHeadValidation(initialHeadTrack_txt))
            {
                case "empty":
                    //if the textbox is empty

                    //Reset things
                    Utilities.ResetVariables();
                    Utilities.ResetLineGraph(sstf_chart);
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
                    Utilities.ResetLineGraph(sstf_chart);
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
            Utilities.DrawLineGraph(sstf_chart);

            //Print the initial head track
            Utilities.PrintInitialHeadTrack(initialTrack_lbl);

            //Output the number of head movements in the table below
            Utilities.OutputHeadMovements(panel5, panel6, panel7);
        }

        private void Calculate()
        {
            while (Utilities.IsFinished() == false)
            {

            }
        }
    }
}
