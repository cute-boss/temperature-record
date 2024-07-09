/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                                                                          */
/* DALProfile.cs                                                                                                                                       */
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
using System.Data.SqlClient;
using System.Data;
using System.Collections;

namespace TRS
{
    class DALProfile
    {
        /* Get All profile list */
        public DataTable GetAllProfileList()
        {
            string conStr = Common.GetSQLDBStrCon();

            if (conStr == null)
            {
                return null;
            }

            DataTable dataTbl = new DataTable();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                try
                {
                    con.Open();
                    string sqlcmd = "SELECT * FROM tbl_profile ORDER BY staff_name";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlcmd, con))
                    {
                        adapter.Fill(dataTbl);
                    }
                }
                catch (Exception ex)
                {
                    //Write error to log
                    Common.WriteToLog(ex.ToString());
                    return null;
                }
            }

            return dataTbl;
        }

        /* Delete profile data by profile Id */
        public bool DelProfileById(int profile_id)
        {
            string conStr = Common.GetSQLDBStrCon();

            if (conStr == null)
            {
                return false;
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                try
                {
                    con.Open();
                    string sqlcmd = "DELETE FROM tbl_profile WHERE profile_id = @profile_id";
                    using (SqlCommand cmd = new SqlCommand(sqlcmd, con))
                    {
                        cmd.Parameters.AddWithValue("@profile_id", profile_id);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    //Write error to log
                    Common.WriteToLog(ex.ToString());
                    return false;
                }
            }

            return true;
        }

        /* Get profile list by staff Id*/
        public DataTable GetProfileListByStaffId(string staff_id)
        {
            string conStr = Common.GetSQLDBStrCon();

            if (conStr == null)
            {
                return null;
            }

            DataTable dataTbl = new DataTable();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                try
                {
                    con.Open();
                    string sqlcmd = "SELECT * FROM tbl_profile WHERE staff_id = @staff_id";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlcmd, con))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@staff_id", staff_id);
                        adapter.Fill(dataTbl);
                    }
                }
                catch (Exception ex)
                {
                    Common.WriteToLog(ex.ToString());
                    return null;
                }
            }

            return dataTbl;
        }

        /* Check duplicate profile */
        public int DuplicateProfile(string staff_id)
        {
            int count = 0;
            string conStr = Common.GetSQLDBStrCon();

            if (conStr == null)
            {
                return -1;
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM tbl_profile WHERE staff_id = @staff_id", con))
                    {
                        cmd.Parameters.AddWithValue("@staff_id", staff_id);
                        count = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                catch (Exception ex)
                {
                    Common.WriteToLog(ex.ToString());
                    return -1;
                }
            }

            return count;
        }

        /* Add profile */
        public bool AddProfile(string staff_id, string staff_name)
        {
            string conStr = Common.GetSQLDBStrCon();

            if (conStr == null)
            {
                return false;
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO tbl_profile VALUES(@staff_id, @staff_name)", con))
                    {
                        cmd.Parameters.AddWithValue("@staff_id", staff_id);
                        cmd.Parameters.AddWithValue("@staff_name", staff_name);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Common.WriteToLog(ex.ToString());
                    return false;
                }
            }

            return true;
        }

        /* Update profile */
        public bool UpdProfile(int profile_id, string staff_id, string staff_name)
        {
            string conStr = Common.GetSQLDBStrCon();

            if (conStr == null)
            {
                return false;
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE tbl_profile SET staff_id = @staff_id, staff_name = @staff_name WHERE profile_id = @profile_id", con))
                    {
                        cmd.Parameters.AddWithValue("@profile_id", profile_id);
                        cmd.Parameters.AddWithValue("@staff_id", staff_id);
                        cmd.Parameters.AddWithValue("@staff_name", staff_name);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Common.WriteToLog(ex.ToString());
                    return false;
                }
            }

            return true;
        }

        /* Add profile by batch */
        public bool AddBatchProfile(ArrayList staffList, bool overwrite)
        {
            string conStr = Common.GetSQLDBStrCon();

            if (conStr == null)
            {
                return false;
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                try
                {
                    con.Open();

                    string sqlCmd = "";
                    bool update = false;

                    for (int i = 0; i < staffList.Count; i++)
                    {
                        Profile profile = (Profile)staffList[i];

                        if (overwrite)
                        {
                            /* Get data from DB */
                            SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_profile WHERE staff_id = @staff_id", con);

                            cmd.Parameters.AddWithValue("@staff_id", profile.staffId);

                            SqlDataReader reader = cmd.ExecuteReader();

                            if (reader.Read())
                            {
                                update = true;
                            }
                            else
                            {
                                update = false;
                            }

                            if (reader != null)
                            {
                                reader.Close();
                            }
                        }
                        else
                        {
                            update = false;
                        }

                        if (update)
                        {
                            sqlCmd = "UPDATE tbl_profile SET staff_name = @staff_name WHERE staff_id = @staff_id";
                        }
                        else
                        {
                            sqlCmd = "INSERT INTO tbl_profile VALUES (@staff_id, @staff_name)";
                        }

                        using (SqlCommand cmd = new SqlCommand(sqlCmd, con))
                        {
                            cmd.Parameters.AddWithValue("@staff_id", profile.staffId);
                            cmd.Parameters.AddWithValue("@staff_name", profile.staffName);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.WriteToLog(ex.ToString());
                    return false;
                }
            }

            return true;
        }
    }
}
