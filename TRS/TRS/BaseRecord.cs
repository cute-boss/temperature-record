/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                                                                          */
/* BaseRecord.cs                                                                                                                                       */
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

namespace TRS
{
    public partial class BaseRecord : Form
    {
        DataTable recordTbl;

        public BaseRecord()
        {
            InitializeComponent();
        }

        private void BaseRecord_Load(object sender, EventArgs e)
        {
            ClearField();
            ShowGridData();
        }

        private void ClearField()
        {
            BaseRecord_txt_id.Text = "";
            BaseRecord_txt_name.Text = "";

            BaseRecord_txt_id.Focus();
            BaseRecord_gv_result.ClearSelection();
        }

        private void ShowGridData()
        {
            recordTbl = new DataTable();
            recordTbl = Common.dalRecord.GetAllRecordList();

            if (recordTbl == null)
            {
                Common.SQLErrorMsg();
                return;
            }

            if (recordTbl.Rows.Count > 0)
            {
                BaseRecord_gv_result.DataSource = recordTbl;

                BaseRecord_gv_result.Columns[0].Visible = false;
                BaseRecord_gv_result.Columns[1].Width = 60;
                BaseRecord_gv_result.Columns[2].Width = 160;
                BaseRecord_gv_result.Columns[3].Width = 90;
                BaseRecord_gv_result.Columns[4].Visible = false;

                BaseRecord_gv_result.Columns[1].HeaderText = "Staff Id";
                BaseRecord_gv_result.Columns[2].HeaderText = "Staff Name";
                BaseRecord_gv_result.Columns[3].HeaderText = "Temperature";

                BaseRecord_gv_result.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Disabled sort & align center for row data //
                for (int rowData = 0; rowData < BaseRecord_gv_result.Columns.Count; rowData++)
                {
                    BaseRecord_gv_result.Columns[rowData].SortMode = DataGridViewColumnSortMode.NotSortable;

                    if (rowData != 2)
                    {
                        BaseRecord_gv_result.Columns[rowData].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }

                BaseRecord_gv_result.Columns[BaseRecord_gv_result.ColumnCount - 3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                BaseRecord_gv_result.ClearSelection();
            }
            else
            {
                BaseRecord_gv_result.DataSource = null;
            }
        }

        private void BaseRecord_txt_id_TextChanged(object sender, EventArgs e)
        {
            string staffId = BaseRecord_txt_id.Text;
            string staffName = BaseRecord_txt_name.Text;

            // Get selected staff by staff id
            DataTable profileTbl = new DataTable();
            profileTbl = Common.dalProfile.GetProfileListByStaffId(staffId);

            if (profileTbl == null)
            {
                Common.SQLErrorMsg();
                return;
            }

            if (profileTbl.Rows.Count > 0)
            {
                staffId = profileTbl.Rows[0]["staff_id"].ToString();
                staffName = profileTbl.Rows[0]["staff_name"].ToString();

                BaseRecord_txt_id.Text = staffId;
                BaseRecord_txt_name.Text = staffName;
            }
            else
            {
                BaseRecord_txt_name.Text = "";
                BaseRecord_txt_id.Focus();
            }
        }

        private void tempBtn_Click(object sender, EventArgs e)
        {
            string staffId = BaseRecord_txt_id.Text;
            string staffName = BaseRecord_txt_name.Text;

            Button btn = (Button)sender;

            if (staffId == "")
            {
                //Common.NoInputStaffIdMsg();
                RecordStatus("INVALID");
                BaseRecord_txt_id.Clear();
                BaseRecord_txt_id.Focus();
                return;
            }

            if (staffName == "")
            {
                //Common.NoInputStaffNameMsg();
                RecordStatus("INVALID");
                BaseRecord_txt_id.Clear();
                BaseRecord_txt_id.Focus();
                return;
            }

            if (Common.dalRecord.AddRecord(staffId, staffName, btn.Text))
            {
                ShowGridData();
                ClearField();
                RecordStatus("OK");
                //Common.StaffRegSuccessMsg(staffName);
            }
            else
            {
                RecordStatus("NG");
                //Common.StaffRegFailMsg(staffName);
            }
        }

        private void RecordStatus(string status)
        {
            if (status == "OK")
            {
                BaseRecord_lbl_status.Text = "OK";
                BaseRecord_lbl_status.BackColor = Color.MediumSeaGreen;
            }
            else if (status == "NG")
            {
                BaseRecord_lbl_status.Text = "NG";
                BaseRecord_lbl_status.BackColor = Color.IndianRed;
            }
            else
            {
                BaseRecord_lbl_status.Text = "INVALID";
                BaseRecord_lbl_status.BackColor = Color.DarkOrange;
            }

            Timer t = new System.Windows.Forms.Timer();
            t.Interval = 3000; // Tick in 3 seconds
            t.Tick += (s, e) =>
            {
                BaseRecord_lbl_status.Text = "-";
                BaseRecord_lbl_status.BackColor = Color.FromArgb(248, 248, 248);
                t.Stop();
            };

            t.Start();
        }
    }
}
