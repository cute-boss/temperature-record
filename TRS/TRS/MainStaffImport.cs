/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                                                                          */
/* MainStaffImport.cs                                                                                                                                  */
/* 									  	                                                                                                               */
/* Date: 13/05/2020							                                                                                                           */
/* Author: Azmir 					                                                                                                                   */
/*										                                                                                                               */
/* Modify History:							                                                                                                           */
/* 		Date        Comment			                                	                                                                  Name         */
/*-----------------------------------------------------------------------------------------------------------------------------------------------------*/
/*      13/05/2020  Initial version                                                                                                       Azmir        */
/*                                                                                                                                                     */
/*-----------------------------------------------------------------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace TRS
{
    public partial class MainStaffImport : Form
    {
        const int numCol = 2;                           // staff id, staff name
        const int maxName = 50;
        const int maxStaffId = 10;
        bool overwrite = false;
        String[] colHeader = null;
        ArrayList staffList = null;
        List<String> errorList = null;
        List<String[]> dataList = null;

        public MainStaffImport()
        {
            InitializeComponent();
        }

        private void MainStaffImport_Load(object sender, EventArgs e)
        {
            ClearField();
        }

        private void ClearField()
        {
            MainStaffImport_txt_result.Text = "";
            MainStaffImport_btn_export.Enabled = false;
            MainStaffImport_btn_import.Enabled = false;

            MainStaffImport_txt_file.Focus();
        }

        private bool Check_CSV(Profile profile)
        {
            /* Check empty */
            if ((profile.staffId == "") || (profile.staffName == ""))
            {
                return false;
            }

            /* Check length staff no. */
            if (profile.staffId.Length > maxStaffId)
            {
                return false;
            }

            /* Accept 0-9, A-Z, a-z, & - only */
            for (int i = 0; i < profile.staffId.Length; i++)
            {
                char c = profile.staffId[i];

                if (!(((c >= 48) && (c <= 57)) || ((c >= 65) && (c <= 90)) || ((c >= 97) && (c <= 122)) || (c == 45)))
                {
                    return false;
                }
            }

            /* Check staff id when not overwrite */
            if (!overwrite)
            {
                int cntDup = Common.dalProfile.DuplicateProfile(profile.staffId);

                /* Check db error */
                if (cntDup == -1)
                {
                    Common.SQLErrorMsg();
                    return false;
                }

                /* Check duplicate staff no. */
                if (cntDup > 0)
                {
                    return false;
                }
            }

            /* Check max length of staff name */
            if (profile.staffName.Length > maxName)
            {
                return false;
            }

            return true;
        }

        private void MainStaffImport_btn_browse_Click(object sender, EventArgs e)
        {
            // Reset result
            MainStaffImport_txt_result.Text = "";
            MainStaffImport_btn_export.Enabled = false;

            OpenFileDialog openFile = new OpenFileDialog();

            openFile.FileName = "";
            openFile.Filter = "CSV Files|*.csv";
            openFile.Title = "Select a CSV file to import";

            // Show selected path in the textbox
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                MainStaffImport_txt_file.Text = openFile.FileName;
                MainStaffImport_btn_import.Enabled = true;
            }
            else
            {
                MainStaffImport_txt_file.Focus();
                return;
            }
        }

        private void MainStaffImport_btn_import_Click(object sender, EventArgs e)
        {
            // Reset result
            MainStaffImport_txt_result.Text = "";
            MainStaffImport_btn_export.Enabled = false;

            #region File validation

            // Check file path
            if (MainStaffImport_txt_file.Text == "")
            {
                // Check overwrite checkbox
                if (MainStaffImport_chkbox_overwrite.Checked)
                {
                    MainStaffImport_chkbox_overwrite.Checked = false;
                }

                Common.BatchFilePathEmptyMsg();
                MainStaffImport_txt_file.Focus();
                return;
            }

            // Check .csv file exist
            if (File.Exists(MainStaffImport_txt_file.Text))
            {
                // Get extension file
                string ext = Path.GetExtension(MainStaffImport_txt_file.Text);

                // Check file extension
                if (string.Compare(ext, ".csv", true) != 0)
                {
                    Common.InvalidFileTypeMsg(ext);
                    MainStaffImport_txt_file.Focus();
                    MainStaffImport_txt_file.SelectAll();
                    return;
                }
            }
            else
            {
                Common.InvalidFilePathMsg();
                MainStaffImport_txt_file.Focus();
                MainStaffImport_txt_file.SelectAll();
                return;
            }

            #endregion

            #region CSV validation

            FileStream fs = null;

            // Check for opened file
            try
            {
                fs = new FileStream(MainStaffImport_txt_file.Text, FileMode.Open, FileAccess.ReadWrite);
            }
            catch
            {
                Common.BatchFileOpenMsg();
                return;
            }

            StreamReader sr = new StreamReader(fs);
            String[] dataCol = null;
            dataList = new List<String[]>();

            // Read column header
            colHeader = sr.ReadLine().Split(',');

            while (!sr.EndOfStream)
            {
                dataCol = sr.ReadLine().Split(',');

                // Check number of column
                if (dataCol.Length != numCol)
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }

                    Common.BatchColumnNotMatchMsg();
                    return;
                }

                // Insert data into the list
                dataList.Add(dataCol);
            }

            if (fs != null)
            {
                fs.Close();
            }

            #endregion

            if (MainStaffImport_chkbox_overwrite.Checked)
            {
                overwrite = true;
            }
            else
            {
                overwrite = false;
            }

            string dataRow = "";
            int ttlRow = 0;

            staffList = new ArrayList();
            errorList = new List<String>();

            foreach (String[] col in dataList)
            {
                // Create class profile
                Profile profile = new Profile();

                profile.staffId = col[0];
                profile.staffName = col[1];

                dataRow = profile.staffId + "," + profile.staffName;

                if (Check_CSV(profile))
                {
                    // Add staff in register list
                    staffList.Add(profile);
                }
                else
                {
                    // Add staff in error list
                    errorList.Add(dataRow);
                }

                ttlRow++;
            }

            // Register staff
            if (staffList.Count > 0)
            {
                if (!Common.dalProfile.AddBatchProfile(staffList, overwrite))
                {
                    Common.BatchFailRegMsg();
                    return;
                }
            }

            #region Display result

            string result = "";

            /* Register complete without error */
            if (ttlRow == staffList.Count)
            {
                if (staffList.Count == 1)
                {
                    result = staffList.Count + " staff profile has been successfully imported.";
                }
                else
                {
                    result = "A total of " + staffList.Count + " staff profiles have been successfully imported.";
                }
            }
            /* Register complete with error */
            else
            {
                result = "A total of " + (ttlRow - errorList.Count).ToString() + " out of " + ttlRow.ToString() + " staff profile ";

                if (errorList.Count == 1)
                {
                    result += "has";
                }
                else
                {
                    result += "have";
                }

                result += " been imported.\r\n\r\nThe following was not imported :\r\n";

                int i = 1;

                foreach (string error in errorList)
                {
                    // Last row number \r\n
                    if (i == errorList.Count)
                    {
                        result += error;
                    }
                    else
                    {
                        result += error + "\r\n";
                    }

                    i++;
                }

                MainStaffImport_btn_export.Enabled = true;
            }

            MainStaffImport_txt_result.Text = result;

            /* Register log */
            //string logDesc = "Batch registration has completed : " + (ttlRow - errorList.Count).ToString() + " out of " + staffList.Count.ToString() + " have been imported";
            //Common.dalLog.RegLog(logDesc, (int)EnumEX.LogType.OPELOG_BATCH_REG, Common.username);

            #endregion Display result
        }

        private void MainStaffImport_btn_export_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveCSVFile = new SaveFileDialog();

            saveCSVFile.FileName = "TRS_BatchFailData";
            saveCSVFile.Filter = "CSV file|*.csv";
            saveCSVFile.Title = "Save to CSV";
            saveCSVFile.DefaultExt = ".csv";

            if (saveCSVFile.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                /* Get file extension */
                String fileExt = Path.GetExtension(saveCSVFile.FileName);

                // Check file extension. Error if file type is not CSV
                if ((fileExt != ".csv") && (fileExt != ".CSV"))
                {
                    Common.InvalidFileTypeMsg(fileExt);
                    return;
                }

                try
                {
                    StreamWriter sw = new StreamWriter(saveCSVFile.FileName);
                    string header = null;

                    // Column header to the exported CSV file
                    for (int i = 0; i < colHeader.Length; i++)
                    {
                        header = header + colHeader[i];

                        if (i < numCol - 1)
                        {
                            header = header + ",";
                        }

                        if (i == numCol - 1)
                        {
                            header = header + "\r\n";
                        }
                    }

                    // '4' = number of characters in ' \r\n' after ':' + 1
                    int startExport = MainStaffImport_txt_result.Text.IndexOf(':') + 4;   

                    // To export only staff info which was not imported by batch
                    string export = header + MainStaffImport_txt_result.Text.Substring(startExport, MainStaffImport_txt_result.Text.Length - startExport);

                    sw.WriteLine(export);

                    sw.Flush();
                    sw.Close();

                    Cursor.Current = Cursors.Default;
                    Common.ExpCSVSuccessMsg();
                }
                catch
                {
                    Cursor.Current = Cursors.Default;
                    Common.ExpCSVFailMsg();
                }
            }
        }

        private void MainStaffImport_txt_file_TextChanged(object sender, EventArgs e)
        {
            ClearField();
        }

        private void MainStaffImport_chkbox_overwrite_CheckedChanged(object sender, EventArgs e)
        {
            MainStaffImport_txt_result.Text = "";
            MainStaffImport_btn_export.Enabled = false;
        }
    }
}
