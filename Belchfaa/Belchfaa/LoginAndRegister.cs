﻿ using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Types;
using Oracle.DataAccess.Client;
using System.Windows.Forms;

namespace Belchfaa
{
    
    internal class LoginAndRegister
    {
        //class LoginAndRegister
        string ordb = "Data Source=orcl;User Id=scott;Password=tiger;";
        public static OracleConnection conn;
        OracleCommand cmd;
        OracleCommand loginCmd;
        SharedPreference addDa;

        public void signUp(TextBox textBox1, TextBox textBox2, TextBox textBox3, TextBox textBox4,ComboBox comboBox1)
        {
            conn = new OracleConnection(ordb);
            conn.Open();
            cmd = new OracleCommand();
            int userId = 0;
            cmd.Connection = conn;
            OracleCommand cmd2 = new OracleCommand();
            cmd2.Connection = conn;
            cmd2.CommandText = "select max (userId) from users";
            OracleDataReader dr = cmd2.ExecuteReader();
            cmd.CommandText = @"insert into users
                                values(:id,:gender,:name,:age,:pass)";
            if (dr.Read())
            {
                if (!(dr[0].ToString().Equals("")))
                {
                    userId = int.Parse(dr[0].ToString()) + 1;
                    cmd.Parameters.Add("id", int.Parse(dr[0].ToString()) + 1);
                }
                else
                {
                    userId = 1;
                    cmd.Parameters.Add("id", 1);
                }
                dr.Close();
            }
            cmd.Parameters.Add("gender", comboBox1.Text);
            cmd.Parameters.Add("Name", textBox1.Text);
            cmd.Parameters.Add("age", (textBox2.Text));
            cmd.Parameters.Add("pass", textBox3.Text);
            int r = cmd.ExecuteNonQuery();
            if (r != -1)
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                comboBox1.Text = "";

                msg mg = new msg();
                mg.Load("You have been registerd successfully \n Your ID is: " + userId);
                mg.ShowDialog();
            }
            conn.Dispose();
        }

        public bool signIn(TextBox id, TextBox password)
        {
           
            
                conn = new OracleConnection(ordb);
                conn.Open();
                cmd = new OracleCommand();
                cmd.Connection = conn;
                addDa = new SharedPreference();

                cmd.CommandText = "select * from users where userId=:id and userPassword=:pass";
                cmd.Parameters.Add("id", id.Text);
                cmd.Parameters.Add("pass", password.Text);

                OracleDataReader dr = cmd.ExecuteReader();
            
                if (dr.Read())
                {
                    if (!(dr[0].ToString().Equals("")))
                    {
                        CurrentUserClass.userId = int.Parse(dr[0].ToString());
                        CurrentUserClass.userAge = int.Parse(dr[3].ToString());
                        CurrentUserClass.userName = dr[2].ToString();
                        CurrentUserClass.gender = dr[1].ToString();
                        CurrentUserClass.userPassword = dr[4].ToString();
                        addDa.AddData(int.Parse(dr[0].ToString()), dr[1].ToString(), dr[2].ToString(),
                        dr[4].ToString(), int.Parse(dr[3].ToString()));
                        return true;
                        //MessageBox.Show("Login successfuly");
                        // MessageBox.Show("your name is: " + dr[1]);
                    }
                    else
                    {
                    msg mg = new msg();
                    mg.Load(" Invalid Information");
                    mg.ShowDialog();
                        return false;
                    }
                    dr.Close();
                }
                else
                {
                   msg mg = new msg();
                    mg.Load(" Invalid Information");
                    mg.ShowDialog();
                    dr.Close();
                    return false;
                }

                conn.Dispose();
            
        }

    }
}
