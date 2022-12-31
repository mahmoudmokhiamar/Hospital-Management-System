﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EasyHealthcare
{
    public partial class patient : System.Web.UI.Page
    {
        
        SqlConnection con = new SqlConnection(@"Data Source=localhost;Initial Catalog=HospitalManagement;Integrated Security=True");
        protected void Page_Load(object sender, EventArgs e)
        {
            string user = Session["userid"].ToString();
            con.Open();
            //string user = Session["userid"].ToString();
            
            string getId = "SELECT pid FROM PATIENT WHERE pid='" + user + "'";
            string getFName = "SELECT pFirstname FROM PATIENT WHERE pid='" + user + "'";
            string getLName = "SELECT pLastname FROM PATIENT WHERE  pid='" + user + "'";
            string getAddress = "SELECT pAddress FROM PATIENT WHERE pid='" + user + "'";
            string getPhone = "SELECT pPhone FROM PATIENT WHERE pid='" + user + "'";
            SqlCommand cmd1 = new SqlCommand(getId, con);
            SqlCommand cmd2 = new SqlCommand(getFName, con);
            SqlCommand cmd3 = new SqlCommand(getLName, con);
            SqlCommand cmd4 = new SqlCommand(getAddress, con);
            SqlCommand cmd5 = new SqlCommand(getPhone, con);
            pIDlabel.Text = cmd1.ExecuteScalar().ToString();
            firstName.Text = cmd2.ExecuteScalar().ToString();
            lastName.Text = cmd3.ExecuteScalar().ToString();
            address.Text = cmd4.ExecuteScalar().ToString();
            phone.Text = cmd5.ExecuteScalar().ToString();


            SqlCommand appDates = new SqlCommand("SELECT COUNT(*) FROM hasAppointment WHERE pid='" + user + "'", con);
            int rows = (int)appDates.ExecuteScalar();

            for (int i = 0; i < rows; i++)
            {
                SqlCommand getDate = new SqlCommand("select appointmentDate from hasAppointment order by appointmentDate offset @Offset rows fetch next 1 rows only", con);
                getDate.Parameters.Add("@Offset", SqlDbType.Int);
                getDate.Parameters["@Offset"].Value = i;
                DateTime date = DateTime.Parse(getDate.ExecuteScalar().ToString());
                SqlCommand getTime = new SqlCommand("select appointmentTime from hasAppointment order by appointmentTime offset @Offset2 rows fetch next 1 rows only", con);
                getTime.Parameters.Add("@Offset2", SqlDbType.Int);

                TableRow r = new TableRow();
                for (int j = 0; j < 1; j++)
                {
                    getTime.Parameters["@Offset2"].Value = j;
                    DateTime time = DateTime.Parse(getTime.ExecuteScalar().ToString());
                    TableCell c = new TableCell();
                    c.Controls.Add(new LiteralControl(date.ToString("MMMM dd, yyyy") + " at " + time.ToString("HH:mm")));
                    r.Cells.Add(c);
                }
                appointments.Rows.Add(r);
            }

            SqlCommand medicalNotes = new SqlCommand("SELECT COUNT(*) FROM attachNote WHERE pid='" + user + "'", con);
            rows = (int)medicalNotes.ExecuteScalar();
            for (int i = 0; i < rows; i++)
            {
                SqlCommand getNote = new SqlCommand("select medical_note from attachNote order by noteId offset @Offset rows fetch next 1 rows only", con);
                getNote.Parameters.Add("@Offset", SqlDbType.Int);
                getNote.Parameters["@Offset"].Value = i;
                string note = (string)getNote.ExecuteScalar();
                TableRow r = new TableRow();
                TableCell c = new TableCell();
                c.Controls.Add(new LiteralControl(note));
                r.Cells.Add(c);
                noteView.Rows.Add(r);
            }


        }

        protected void reserve_Click(object sender, EventArgs e)
        {
            string user = Session["userid"].ToString();
            try
            {
                string date = DateTime.Parse(datePicker.Text).ToString("MMMM dd, yyyy");
                string time = DateTime.Parse(timePicker.Text).ToString("HH:mm");
                string doc = did.Text;
                SqlCommand check = new SqlCommand("SELECT COUNT(*) FROM hasAppointment WHERE pid = '" + user + "' and appointmentDate = @date and appointmentTime = @time and did=@did", con);
                check.Parameters.Add("@date", SqlDbType.Date);
                check.Parameters["@date"].Value = date;
                check.Parameters.Add("@time", SqlDbType.Time);
                check.Parameters["@time"].Value = time;
                check.Parameters.Add("@did", SqlDbType.Int);
                check.Parameters["@did"].Value = doc;
                int checkExist = (int)check.ExecuteScalar();
                if (checkExist == 0)
                {
                    SqlCommand reserve = new SqlCommand("INSERT INTO hasAppointment(pid, did, appointmentDate, appointmentTime) values('" + user + "', @did, @date, @time)", con);
                    reserve.Parameters.Add("@date", SqlDbType.Date);
                    reserve.Parameters["@date"].Value = date;
                    reserve.Parameters.Add("@time", SqlDbType.Time);
                    reserve.Parameters["@time"].Value = time;
                    reserve.Parameters.Add("@did", SqlDbType.Int);
                    reserve.Parameters["@did"].Value = doc;
                    reserve.ExecuteNonQuery();
                    success.Visible = true;
                    existDate.Visible = false;
                    docUn.Visible = false;
                }
                else
                {
                    SqlCommand docCheck = new SqlCommand("SELECT COUNT(*) FROM hasAppointment WHERE did = @did", con);
                    docCheck.Parameters.Add("@did", SqlDbType.Int);
                    docCheck.Parameters["@did"].Value = doc;
                    int docExist = (int)docCheck.ExecuteScalar();
                    if (docExist > 0)
                    {
                        docUn.Visible = true;
                        success.Visible = false;
                        existDate.Visible = false;
                    }
                    else
                    {
                        docUn.Visible = false;
                        success.Visible = false;
                        existDate.Visible = true;
                    }
                }
            } 
            catch(Exception)
            {
                //do nothing 
            }
        }
    }
}