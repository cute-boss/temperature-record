/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                                                                          */
/* DALRecord.cs                                                                                                                                        */
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

namespace TRS
{
    class DALRecord
    {
        /* Get all record list */
        public DataTable GetAllRecordList()
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
                    string sqlcmd = "SELECT * FROM tbl_record ORDER BY datetime_in DESC";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlcmd, con))
                    {
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


        /* Add record */
        public bool AddRecord(string staff_id, string staff_name, string temperature)
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
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO tbl_record VALUES(@staff_id, @staff_name, @temperature, GETDATE())", con))
                    {
                        cmd.Parameters.AddWithValue("@staff_id", staff_id);
                        cmd.Parameters.AddWithValue("@staff_name", staff_name);
                        cmd.Parameters.AddWithValue("@temperature", temperature);
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

        /* Get all staff list by date/time */
        public DataTable GetAllStaffListByDT(DateTime start, DateTime end)
        {
            string sqlcmd, conStr = Common.GetSQLDBStrCon();

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
                    sqlcmd = "SELECT staff_id, staff_name, temperature, datetime_in FROM tbl_record WHERE datetime_in BETWEEN @start AND @end ORDER BY record_id";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlcmd, con))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@start", start);
                        adapter.SelectCommand.Parameters.AddWithValue("@end", end);
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
    }
}
