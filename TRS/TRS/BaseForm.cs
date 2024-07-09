/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                                                                          */
/* BaseForm.cs                                                                                                                                         */
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
using FontAwesome.Sharp;
using System.Runtime.InteropServices;

namespace TRS
{
    public partial class BaseForm : Form
    {
        private IconButton currentBtn;
        //private Panel leftBorderBtn;
        private Form currentChildForm;

        public BaseForm()
        {
            InitializeComponent();

            // Create menu panel
            //leftBorderBtn = new Panel();
            //leftBorderBtn.Size = new Size(7, 60);
            //BaseForm_panel_menu.Controls.Add(leftBorderBtn);

            // Handle form control
            this.Text = string.Empty;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;

            HideSubMenu();
        }

        // Hide subpanel 
        private void HideSubMenu()
        {
            BaseForm_subPanel_maintenance.Visible = false;
        }

        // Set visibility of subpanel
        private void ShowSubMenu(Panel subMenu)
        {
            if (subMenu.Visible == false)
            {
                HideSubMenu();
                subMenu.Visible = true;
            }
            else
            {
                subMenu.Visible = false;
            }
        }

        private void BaseForm_btn_shutdown_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BaseForm_btn_minimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void BaseForm_btn_normal_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                WindowState = FormWindowState.Normal;
            }
        }

        // Drag form control
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void BaseForm_panel_titleBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void BaseForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                FormBorderStyle = FormBorderStyle.None;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Sizable;
            }
        }

        private void OpenChildForm(Form childForm)
        {
            // Open only form
            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }

            currentChildForm = childForm;
            // End
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            BaseForm_panel_childForm.Controls.Add(childForm);
            BaseForm_panel_childForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            BaseForm_lbl_titleChildForm.Text = childForm.Text;
        }

        private void Reset()
        {
            DisableButton();
            //leftBorderBtn.Visible = false;
            BaseForm_iconPictureBox_currentChildForm.IconChar = IconChar.Home;
            BaseForm_lbl_titleChildForm.Text = "Home";
        }

        private void ActivateButton(object senderBtn, Color color)
        {
            if (senderBtn != null)
            {
                DisableButton();
                // Button
                currentBtn = (IconButton)senderBtn;
                currentBtn.BackColor = Color.FromArgb(63, 81, 181);
                currentBtn.ForeColor = color;
                //currentBtn.TextAlign = ContentAlignment.MiddleCenter;
                currentBtn.IconColor = color;
                currentBtn.TextImageRelation = TextImageRelation.TextBeforeImage;
                currentBtn.ImageAlign = ContentAlignment.MiddleRight;
                // Left border button
                //leftBorderBtn.BackColor = color;
                //leftBorderBtn.Location = new Point(0, currentBtn.Location.Y);
                //leftBorderBtn.Visible = true;
                //leftBorderBtn.BringToFront();
                // Current Child Form Icon
                BaseForm_iconPictureBox_currentChildForm.IconChar = currentBtn.IconChar;
                BaseForm_iconPictureBox_currentChildForm.IconColor = color;
            }
        }

        private void DisableButton()
        {
            if (currentBtn != null)
            {
                currentBtn.BackColor = Color.FromArgb(63, 81, 181);
                currentBtn.ForeColor = Color.Gainsboro;
                currentBtn.TextAlign = ContentAlignment.MiddleLeft;
                currentBtn.IconColor = Color.Gainsboro;
                currentBtn.TextImageRelation = TextImageRelation.ImageBeforeText;
                currentBtn.ImageAlign = ContentAlignment.MiddleLeft;
            }
        }

        private void BaseForm_iconPictureBox_logo_Click(object sender, EventArgs e)
        {
            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }

            Reset();
        }

        private void BaseForm_iconButton_record_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, Color.FromArgb(255, 255, 255));
            OpenChildForm(new BaseRecord());
            HideSubMenu();
        }

        private void BaseForm_iconButton_maintenance_Click(object sender, EventArgs e)
        {
            ShowSubMenu(BaseForm_subPanel_maintenance);
        }

        private void BaseForm_iconButton_staff_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, Color.FromArgb(255, 255, 255));
            OpenChildForm(new MainStaff());
        }

        private void BaseForm_iconButton_batchImport_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, Color.FromArgb(255, 255, 255));
            OpenChildForm(new MainStaffImport());
        }

        private void BaseForm_iconButton_report_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, Color.FromArgb(255, 255, 255));
            OpenChildForm(new RepStaff());
            HideSubMenu();
        }
    }
}
