/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                                                                          */
/* RepStaff.cs                                                                                                                                       */
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
using OfficeOpenXml;
using System.IO;

namespace TRS
{
    public partial class RepStaff : Form
    {
        //DALLog dalLog = new DALLog();
        DALRecord dalRecord = new DALRecord();
        DataTable staffTbl;

        public RepStaff()
        {
            InitializeComponent();
        }

        private void RepStaff_Load(object sender, EventArgs e)
        {
            ClearField();
        }

        public void ClearField()
        {
            RepStaff_gv_result.DataSource = null;
            RepStaff_btn_export.Enabled = false;
        }

        /* Load and display staff report */
        private void ShowData()
        {
            DateTime dtFrom = RepStaff_picker_dtFrm.Value.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
            DateTime dtTo = RepStaff_picker_dtTo.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            staffTbl = new DataTable();
            staffTbl = dalRecord.GetAllStaffListByDT(dtFrom, dtTo);

            if (staffTbl == null)
            {
                Common.SQLErrorMsg();
                return;
            }

            if (staffTbl.Rows.Count > 0)
            {
                RepStaff_btn_export.Enabled = true;

                RepStaff_gv_result.DataSource = staffTbl;

                RepStaff_gv_result.Columns[0].HeaderText = "Staff Id";
                RepStaff_gv_result.Columns[1].HeaderText = "Staff Name";
                RepStaff_gv_result.Columns[2].HeaderText = "Temperature";
                RepStaff_gv_result.Columns[3].HeaderText = "Date/Time";

                RepStaff_gv_result.Columns[0].Width = 70;
                RepStaff_gv_result.Columns[1].Width = 160;
                RepStaff_gv_result.Columns[2].Width = 90;
                RepStaff_gv_result.Columns[3].Width = 160;

                RepStaff_gv_result.Columns[3].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";

                RepStaff_gv_result.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Disabled sort & align center for row data //
                for (int rowData = 0; rowData < RepStaff_gv_result.Columns.Count; rowData++)
                {
                    RepStaff_gv_result.Columns[rowData].SortMode = DataGridViewColumnSortMode.NotSortable;

                    if ((rowData != 0) && (rowData != 1))
                    {
                        RepStaff_gv_result.Columns[rowData].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }

                RepStaff_gv_result.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                RepStaff_gv_result.ClearSelection();
            }
            else
            {
                Common.EmptyRecordMsg();
                RepStaff_gv_result.DataSource = null;
            }
        }

        /* Load data based on datatime validation when button is clicked */
        private void RepStaff_btn_view_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            // Date start bigger than date end //
            if (RepStaff_picker_dtFrm.Value.Date > RepStaff_picker_dtTo.Value.Date)
            {
                Common.InvalidDate1Msg();
                RepStaff_picker_dtFrm.Focus();
                return;
            }

            TimeSpan span = RepStaff_picker_dtTo.Value.Date.Subtract(RepStaff_picker_dtFrm.Value.Date);
            // View should not exceeds 365 days
            if (span.TotalDays > 365)
            {
                Common.InvalidDayMsg();
                RepStaff_picker_dtTo.Focus();
                return;
            }

            ShowData();

            Cursor.Current = Cursors.Default;
        }

        /* Export and save report to excel when button is clicked */
        private void RepStaff_btn_export_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            using (ExcelPackage eP = new ExcelPackage())
            {
                try
                {
                    #region Sheet design

                    // Document properties 
                    eP.Workbook.Properties.Author = "Temperature Record System";

                    // Excess the first sheet named Staff Record
                    ExcelWorksheet wSheet = eP.Workbook.Worksheets.Add("Staff_Report");
                    wSheet.View.ZoomScale = 97;
                    wSheet.View.ShowGridLines = false;
                    wSheet.Cells.Style.Font.Size = 11;          // Default size 
                    wSheet.Cells.Style.Font.Name = "Calibri";   // Default font name 

                    // Resize column width 
                    int targetColumn = 9;

                    for (int i = 1; i < targetColumn; i++)
                    {
                        wSheet.Column(i).Width = 20;
                    }

                    // Adjust column 
                    wSheet.Column(1).Width = 3.10;

                    // Adjust row 
                    wSheet.Row(10).Height = 20;

                    // Freeze pane 
                    wSheet.View.FreezePanes(11, 1);

                    #endregion Sheet design

                    #region Header design

                    wSheet.Cells["B2"].Value = "TEMPERATURE RECORD SYSTEM";
                    wSheet.Cells["B3"].Value = "Report Name :";
                    wSheet.Cells["B4"].Value = "Report Date :";
                    wSheet.Cells["B5"].Value = "Report Generated On :";
                    wSheet.Cells["B6"].Value = "Report Generated By :";
                    wSheet.Cells["B8"].Value = "Staff Details";
                    wSheet.Cells["B10"].Value = "Staff Id";
                    wSheet.Cells["C10"].Value = "Staff Name";
                    wSheet.Cells["D10"].Value = "Temperature";
                    wSheet.Cells["E10"].Value = "Date/Time";

                    string dT = String.Format("{0:dd/MM/yyyy hh:mm tt}", DateTime.Now);

                    wSheet.Cells["C3"].Value = "Staff Report";
                    wSheet.Cells["C4"].Value = RepStaff_picker_dtFrm.Value.ToString("dd/MM/yyyy") + " to " + RepStaff_picker_dtTo.Value.ToString("dd/MM/yyyy");
                    wSheet.Cells["C5"].Value = dT;
                    wSheet.Cells["C6"].Value = "User";

                    // Title style 
                    ExcelRange excelRange = wSheet.Cells["B2:I2"];
                    excelRange.Style.Font.SetFromFont(new Font("Calibri", 20, FontStyle.Regular));
                    excelRange.Merge = true;
                    excelRange.Style.Font.Bold = true;

                    #endregion Header design

                    #region Table header design

                    wSheet.Cells["B8"].Style.Font.Size = 14;
                    wSheet.Cells["B10:E10"].Style.Font.Size = 12;

                    wSheet.Cells["B8"].Style.Font.Bold = true;
                    wSheet.Cells["B10:E10"].Style.Font.Bold = true;

                    wSheet.Cells["B8"].Style.Font.UnderLine = true;

                    wSheet.Cells["B10:E10"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                    wSheet.Cells["B10:E10"].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                    wSheet.Cells["B10:E10"].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    wSheet.Cells["B10:E10"].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    wSheet.Cells["B10:E10"].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    wSheet.Cells["B10:E10"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    wSheet.Cells["B10:E10"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    wSheet.Cells["B10:E10"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    #endregion Table header design

                    #region Table content

                    excelRange = wSheet.Cells["B10:E10"];

                    for (int r = 0; r < staffTbl.Rows.Count; r++)
                    {
                        for (int c = 0; c < staffTbl.Columns.Count; c++)
                        {
                            if (c == 3)
                            {
                                wSheet.Cells[(r + 11), (c + 2), (r + 11), (c + 2)].Value = DateTime.Parse(staffTbl.Rows[r][c].ToString()).ToString("dd/MM/yyyy hh:mm tt");
                            }
                            else
                            {
                                wSheet.Cells[(r + 11), (c + 2), (r + 11), (c + 2)].Value = staffTbl.Rows[r][c].ToString();
                            }

                            // Change background color for even number 
                            if ((r + 1) % 2 == 0)
                            {
                                excelRange = wSheet.Cells[(r + 11), (c + 2), (r + 11), (c + 2)];
                                excelRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                excelRange.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
                            }

                            wSheet.Cells[(r + 11), (c + 2), (r + 11), (c + 2)].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            wSheet.Cells[(r + 11), (c + 2), (r + 11), (c + 2)].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            wSheet.Cells[(r + 11), (c + 2), (r + 11), (c + 2)].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            wSheet.Cells[(r + 11), (c + 2), (r + 11), (c + 2)].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            //wSheet.Cells[(r + 11), (c + 2), (r + 11), (c + 2)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                    }

                    // Fit the columns according to its content //
                    wSheet.Column(1).AutoFit();
                    wSheet.Column(2).AutoFit();
                    wSheet.Column(3).AutoFit();
                    wSheet.Column(4).AutoFit();

                    #endregion Table content

                }
                catch
                {
                    Common.ExportRptFailMsg("Staff");
                    return;
                }

                #region Save file

                SaveFileDialog saveFile = new SaveFileDialog();
                string Date = String.Format("{0: yyyyMMdd}", DateTime.Now);
                saveFile.FileName = "StaffReport_" + Date;
                saveFile.Filter = "Excel file (*.xlsx)|.xlsx";
                saveFile.DefaultExt = ".xlsx";

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (FileStream fS = new FileStream(saveFile.FileName, FileMode.Create))
                        {
                            eP.SaveAs(fS);
                            // Set Log 
                            //string strMsg = "Staff report has been exported";
                            //dalLog.SetLog(strMsg, (int)EnumEX.LogType.OPELOG_VMS_REPORTSTAFF_EXP, Common.username);
                            Common.ExportRptSuccessMsg("Staff");
                        }
                    }
                    catch
                    {
                        Common.OverwriteRptFailMsg();
                        return;
                    }

                    #region openExcel

                    // Check if excel is installed
                    bool isExcelInstalled = Type.GetTypeFromProgID("Excel.Application") != null ? true : false;

                    // Open excel file
                    // Only after excel is installed
                    // And Yes CONFIRMATION is clicked
                    if (isExcelInstalled)
                    {
                        if (Common.OpenFileConfirmationMsg() == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFile.FileName);
                        }
                    }

                    #endregion openExcel
                }

                #endregion Save file
            }
        }

        /* Load when picker value changed */
        private void RepStaff_picker_dtFrm_ValueChanged(object sender, EventArgs e)
        {
            ClearField();
        }

        /* Load when picker value changed */
        private void RepStaff_picker_dtTo_ValueChanged(object sender, EventArgs e)
        {
            ClearField();
        }
    }
}
