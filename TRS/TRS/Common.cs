/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                                                                          */
/* Common.cs                                                                                                                                           */
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
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Security.AccessControl;
using System.Security.Principal;

namespace TRS
{
    class Common
    {
        public static DALProfile dalProfile = new DALProfile();
        public static DALRecord dalRecord = new DALRecord();

        /* Get connection with MSSQL from static variable */
        public static String GetSQLDBStrCon()
        {
            string ResultStr = ConfigurationManager.ConnectionStrings["DBConStr"].ConnectionString;

            return ResultStr;
        }

        /* Check database connection with MSSQL */
        public static bool CheckDBStatus()
        {
            bool result = false;
            string conStr = Common.GetSQLDBStrCon();

            try
            {
                SqlConnection con = new SqlConnection(conStr);
                con.Open();

                result = true;

                con.Close();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /* Write error to log file */
        public static void WriteToLog(string errDesc)
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + "TRS" + "\\" + "Log";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string filename = dir + "\\" + "TRS_" + DateTime.Now.ToString("yyyyMM") + "." + "LOG";

            if (!File.Exists(filename))
            {
                using (var myFile = File.Create(filename))
                {
                    GrantAccess(filename);
                }
            }

            using (StreamWriter swLog = new StreamWriter(filename, true))
            {
                if (swLog != null)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append(DateTime.Now);
                    sb.Append("\r\n");

                    sb.Append(errDesc);
                    swLog.WriteLine(sb.ToString());
                    swLog.WriteLine();
                    swLog.Flush();
                }
            }
        }

        /* Grant access right to everyone */
        private static void GrantAccess(string fullPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(fullPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
            FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
            PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }

        /* SQL error msg */
        public static void SQLErrorMsg()
        {
            MessageBox.Show("SQL Server error.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /* Staff id is empty
        public static void NoInputStaffIdMsg()
        {
            MessageBox.Show("Please enter staff Id.", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }*/

        /* Staff name is empty
        public static void NoInputStaffNameMsg()
        {
            MessageBox.Show("Data not found.", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }*/

        /* Staff registration is successful 
        public static void StaffRegSuccessMsg(string staffName)
        {
            MessageBox.Show("[" + staffName + "] has been saved successfully.", "Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }*/

        /* Staff registration fails 
        public static void StaffRegFailMsg(string staffName)
        {
            MessageBox.Show("Failed to register [" + staffName + "].", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }*/

        /* No records msg */
        public static void EmptyRecordMsg()
        {
            MessageBox.Show("Record is not found.", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /* Invalid date msg */
        public static void InvalidDate1Msg()
        {
            MessageBox.Show("Invalid date. End date must be larger than the start date.", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /* Invalid day msg */
        public static void InvalidDayMsg()
        {
            MessageBox.Show("Date range should not exceed 365 days.", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /* Export report fail msg */
        public static void ExportRptFailMsg(string reportName)
        {
            MessageBox.Show("An error occurs during exporting " + reportName + ". Please try again.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /* Export report success msg */
        public static void ExportRptSuccessMsg(string reportName)
        {
            MessageBox.Show(reportName + " report has been exported successfully.", "Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /* Overwrite report fail msg */
        public static void OverwriteRptFailMsg()
        {
            MessageBox.Show("Unable to overwrite existing data. Please close the existing file if the file is open " +
                "and make sure the existing file is not set to hidden.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /* Open report confirmation msg */
        public static DialogResult OpenFileConfirmationMsg()
        {
            DialogResult result = MessageBox.Show("Do you want to open the file", "Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            return result;
        }

        /* No staff id */
        public static void NoStaffIdMsg()
        {
            MessageBox.Show("Staff Id is required.", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);  
        }

        /* No staff name */
        public static void NoStaffNameMsg()
        {
            MessageBox.Show("Staff name is required.", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /* Staff already registered msg */
        public static void StaffIsRegistered(string staff_id)
        {
            MessageBox.Show("[" + staff_id + "] has already been registered.", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /* Staff register success msg */
        public static void StaffRegSuccessMsg(string staff_id)
        {
            MessageBox.Show("[" + staff_id + "] has been registered successfully.", "Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /* Staff register fail msg */
        public static void StaffRegFailMsg(string staff_id)
        {
            MessageBox.Show("Failed to register [" + staff_id + "].", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /* Coonfirm update staff msg */
        public static DialogResult StaffUpdConfirmMsg()
        {
            DialogResult result = MessageBox.Show("Are you sure you want to update this staff?", "Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            return result;
        }

        /* Staff update success msg */
        public static void StaffUpdSuccessMsg(string staff_id)
        {
            MessageBox.Show("[" + staff_id + "] has been updated successfully.", "Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /* Staff update fail msg */
        public static void StaffUpdFailMsg(string staff_id)
        {
            MessageBox.Show("Failed to update [" + staff_id + "].", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /* Confirm delete profile msg */
        public static DialogResult ProfileDelConfirmMsg()
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete this staff?", "Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            return result;
        }

        /* Profile delete success msg */
        public static void ProfileDelSuccessMsg(string ProfileName)
        {
            MessageBox.Show("[" + ProfileName + "] has deleted successfully.", "Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /* Profile delete fails msg */
        public static void ProfileDelFailMsg(string ProfileName)
        {
            MessageBox.Show("Failed to delete [" + ProfileName + "].", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /* File path empty msg */
        public static void BatchFilePathEmptyMsg()
        {
            MessageBox.Show("Please select any CSV file.", "Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /* Invalid file extension msg */
        public static void InvalidFileTypeMsg(string fileExt)
        {
            MessageBox.Show("Invalid file type : " + fileExt + ". Please enter any CSV file.", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /* Invalid file path msg */
        public static void InvalidFilePathMsg()
        {
            MessageBox.Show("Invalid file path.", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // CSV file open
        public static void BatchFileOpenMsg()
        {
            MessageBox.Show("CSV file was opened. Please close the file.", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /* Column size not match msg */
        public static void BatchColumnNotMatchMsg()
        {
            MessageBox.Show("CSV file must consist of two columns.", "Alert", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /* Batch failed register msg */
        public static void BatchFailRegMsg()
        {
            MessageBox.Show("Failed to register staff by batch.", "Alert", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /* Export .csv success msg */
        public static void ExpCSVSuccessMsg()
        {
            MessageBox.Show("Success to export to CSV file.", "Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /* Export .csv fail msg */
        public static void ExpCSVFailMsg()
        {
            MessageBox.Show("Failed to export to CSV file.", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
