/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                                                                          */
/* MainStaff.cs                                                                                                                                         */
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
    public partial class MainStaff : Form
    {
        DataTable profileTbl;
        int row;

        public MainStaff()
        {
            InitializeComponent();
        }

        private void MainStaff_Load(object sender, EventArgs e)
        {
            ClearField();
            ShowGridData();
        }

        private void ClearField()
        {
            MainStaff_btn_save.Visible = true;
            MainStaff_btn_upd.Visible = false;
            MainStaff_btn_delete.Enabled = false;

            MainStaff_txt_id.Text = "";
            MainStaff_txt_name.Text = "";

            MainStaff_txt_id.Focus();
            MainStaff_gv_result.ClearSelection();

            this.AcceptButton = MainStaff_btn_save;
        }

        // Show gridview data
        private void ShowGridData()
        {
            profileTbl = new DataTable();
            profileTbl = Common.dalProfile.GetAllProfileList();

            if (profileTbl == null)
            {
                Common.SQLErrorMsg();
                return;
            }

            if (profileTbl.Rows.Count > 0)
            {
                MainStaff_gv_result.DataSource = profileTbl;

                // setup staff grid view
                MainStaff_gv_result.Columns[0].Visible = false;
                MainStaff_gv_result.Columns[1].Width = 80;
                MainStaff_gv_result.Columns[2].Width = 160;

                MainStaff_gv_result.Columns[1].HeaderText = "Staff Id";
                MainStaff_gv_result.Columns[2].HeaderText = "Staff Name";

                MainStaff_gv_result.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                MainStaff_gv_result.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;

                // Header become center
                MainStaff_gv_result.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                MainStaff_gv_result.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MainStaff_gv_result.ClearSelection();
            }
            else
            {
                MainStaff_gv_result.DataSource = null;
            }
        }

        private void MainStaff_btn_save_Click(object sender, EventArgs e)
        {
            string staff_id = MainStaff_txt_id.Text;
            string staff_name = MainStaff_txt_name.Text;

            if (staff_id == "")
            {
                Common.NoStaffIdMsg();
                MainStaff_txt_id.Focus();
                return;
            }

            if (staff_name == "")
            {
                Common.NoStaffNameMsg();
                MainStaff_txt_name.Focus();
                return;
            }

            // Check for duplicate profile
            if (Common.dalProfile.DuplicateProfile(staff_id) > 0)
            {
                Common.StaffIsRegistered(staff_id);
                MainStaff_txt_id.Focus();
                return;
            }

            // Save staff
            if (Common.dalProfile.AddProfile(staff_id, staff_name))
            {
                ShowGridData();
                ClearField();
                Common.StaffRegSuccessMsg(staff_id);

                /* set Log */
                //string logDesc = "Staff Id = [" + staff_id + "] | Staff Name = [" + staff_name + "]";
                //Common.dalLog.SetLog(logDesc, (int)Common.logType.OPELOG_PROFILE_REG, Common.username);
            }
            else
            {
                Common.StaffRegFailMsg(staff_id);
            }
        }

        private void MainStaff_btn_upd_Click(object sender, EventArgs e)
        {
            string staff_name = MainStaff_txt_name.Text;
            string old_staff_name = MainStaff_gv_result.Rows[row].Cells[2].Value.ToString();
            string old_staff_id = MainStaff_gv_result.Rows[row].Cells[1].Value.ToString();
            int profile_id = Int32.Parse(MainStaff_gv_result.Rows[row].Cells[0].Value.ToString());

            /*if (staff_name != old_staff_name)
            {
                // Check for duplicate profile
                if (Common.dalProfile.DuplicateProfile(old_staff_id) > 0)
                {
                    Common.StaffIsRegistered(staff_name);
                    MainStaff_txt_name.Focus();
                    return;
                }
            }*/

            if (Common.StaffUpdConfirmMsg() == DialogResult.Yes)
            {
                if (Common.dalProfile.UpdProfile(profile_id, old_staff_id, staff_name))
                {
                    ShowGridData();
                    ClearField();
                    Common.StaffUpdSuccessMsg(old_staff_id);

                    /* set Log */
                    //string logDesc = "Staff Id = [" + old_staff_id + "] | Staff Name = [" + staff_name + "]";
                    //Common.dalLog.SetLog(logDesc, (int)Common.logType.OPELOG_PROFILE_UPD, Common.username);
                }
                else
                {
                    Common.StaffUpdFailMsg(old_staff_id);
                }
            }
        }

        private void MainStaff_btn_clear_Click(object sender, EventArgs e)
        {
            ClearField();
        }

        private void MainStaff_btn_delete_Click(object sender, EventArgs e)
        {
            if (Common.ProfileDelConfirmMsg() == DialogResult.Yes)
            {
                string staffName = MainStaff_gv_result.Rows[row].Cells[2].Value.ToString();
                int profile_id = Int32.Parse(MainStaff_gv_result.Rows[row].Cells[0].Value.ToString());

                if (Common.dalProfile.DelProfileById(profile_id))
                {
                    ShowGridData();
                    ClearField();
                    Common.ProfileDelSuccessMsg(staffName);

                    /* set Log */
                    //string logDesc = "Staff Name = [" + staffName + "]";
                    //DALLog dalLog = new DALLog();
                    //dalLog.SetLog(logDesc, (int)Common.logType.OPELOG_PROFILE_DEL, Common.username);
                }
                else
                {
                    Common.ProfileDelFailMsg(staffName);
                }
            }
        }

        private void MainStaff_gv_result_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            try
            {
                row = MainStaff_gv_result.CurrentRow.Index;
            }
            catch
            {
                return;
            }

            MainStaff_txt_name.Focus();

            MainStaff_txt_id.Text = MainStaff_gv_result.Rows[row].Cells[1].Value.ToString();
            MainStaff_txt_name.Text = MainStaff_gv_result.Rows[row].Cells[2].Value.ToString();

            MainStaff_btn_save.Visible = false;
            MainStaff_btn_upd.Visible = true;
            MainStaff_btn_delete.Enabled = true;
            MainStaff_txt_id.Enabled = false;
        }
    }
}
