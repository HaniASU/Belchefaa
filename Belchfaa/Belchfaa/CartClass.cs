﻿using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Belchfaa
{
    internal class CartClass
    {
        string ordb = "Data Source=orcl;User Id=scott;Password=tiger;";
        public static OracleConnection conn;
        OracleCommand cmd;
        public void addToCart(int userId, int medId,int medAmount, int amount)
        {
            conn = new OracleConnection(ordb);
            conn.Open();
            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"insert into cart
                                values(:userId,:medId,:medAmount)";
            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("medId", medId);
            cmd.Parameters.Add("medAmount", amount);
            int r = cmd.ExecuteNonQuery();
            if (r != -1)
            {
                updateMedAmount(userId, medId, medAmount-amount);
                MessageBox.Show("Item has been added successfuly");
            }
            conn.Dispose();

        }

        public void removefromCart(int userId, int medId ,int medAmount)
        {
            int oldAmount=0;
            oldAmount = getCartAmount(userId, medId);
            if(oldAmount != 0)
            {
                conn = new OracleConnection(ordb);
                conn.Open();
                cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = @"delete from cart
                                  where cartuserId=:userId and cartmedId=:medId";
                cmd.Parameters.Add("userId", userId);
                cmd.Parameters.Add("medId", medId);
               
                int r = cmd.ExecuteNonQuery();
                if (r != -1)
                {
                    updateMedAmount(userId, medId,oldAmount+medAmount );
                    MessageBox.Show("Item has been removed successfuly");
                }
                else
                {
                    MessageBox.Show("You don't have this item");
                }
                conn.Dispose();
            }
            else
            {
                MessageBox.Show("You don't have this item");
            }
           
        }

        public void displayCart(int userId, ListView listview ,TextBox total)
        {
            int totalPrice = 0;
            conn = new OracleConnection(ordb);
            conn.Open();
            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"select c.amount,m.*
                                from medicines m , cart c
                                WHERE c.cartuserid =:id and c.cartmedid= m.medid";
            cmd.Parameters.Add("userId", userId);
            OracleDataReader dr = cmd.ExecuteReader();
            while(dr.Read())
            {
                ListViewItem list = new ListViewItem(dr[2].ToString());

                list.SubItems.Add(dr[4].ToString() +" L.E.");
                list.SubItems.Add(dr[0].ToString() +" items");
                list.SubItems.Add(dr[6].ToString());
                totalPrice += int.Parse(dr[4].ToString())*int.Parse(dr[0].ToString());
                listview.Items.Add(list);
            }
            PaymentClass.subTotal=totalPrice;   
            dr.Close();
            conn.Dispose();
        }

        public void updateMedAmount(int userId, int medId, int amount)
        {
            conn = new OracleConnection(ordb);
            conn.Open();
            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "update Medicines set medAmount=:amount where medId=:medid";
            cmd.Parameters.Add("amount", amount);
            
            cmd.Parameters.Add("medid", medId);
            int r = cmd.ExecuteNonQuery();
            
            conn.Dispose();

        }

        public void increaseCartAmount(int userId, int medId,int medAmount, int amount)
        {
            conn = new OracleConnection(ordb);
            conn.Open();
            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "update cart set Amount=Amount+:amount where cartmedId=:medid and cartUserId=:userId";
            cmd.Parameters.Add("amount", amount);
            cmd.Parameters.Add("medid", medId);
            cmd.Parameters.Add("userid", userId);
            int r = cmd.ExecuteNonQuery();
            if (r != -1)
            {
                MessageBox.Show("Item amount has been increased by "+ amount);
                updateMedAmount(userId, medId, medAmount - amount);
            }
            conn.Dispose();

        }

        public void decreaseCartAmount(int userId, int medId, int medAmount, int amount)
        {
            int oldAmount=0;
            oldAmount = getCartAmount(userId, medId);
            conn = new OracleConnection(ordb);
            conn.Open();
            OracleCommand cmd2 = new OracleCommand();
            

           if(oldAmount!=0)
            {

                if(oldAmount==amount)
                {
                    removefromCart(userId,medId,medAmount);
                }
                else if(oldAmount>amount)
                {
                    cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = @"update cart set Amount=Amount-:amount where cartmedId=:medid and cartUserId=:userId";

                    cmd.Parameters.Add("amount", amount);
                    cmd.Parameters.Add("medid", medId);
                    cmd.Parameters.Add("userid", userId);

                    int r = cmd.ExecuteNonQuery();
                    if (r != -1)
                    {
                        MessageBox.Show("Item amount has been decreased by " + amount);
                        updateMedAmount(userId, medId, medAmount + amount);
                    }
                    else
                    {
                        MessageBox.Show("Invaild process");
                    }
                    conn.Dispose();
                }
                else
                {
                    MessageBox.Show("Invalid Amount");
                }
                
            }
            else
            {
                MessageBox.Show("You dont have this item");
            }
            

        }

        public int getCartAmount(int userId, int medId)
        {
            int oldAmount = 0;
            conn = new OracleConnection(ordb);
            conn.Open();
            OracleCommand cmd2 = new OracleCommand();
            cmd2.Connection = conn;
            cmd2.CommandText = "select amount from cart where cartmedId=:medid and cartUserId=:userId";

            cmd2.Parameters.Add("medid", medId);
            cmd2.Parameters.Add("userid", userId);
            OracleDataReader dr = cmd2.ExecuteReader();
            if (dr.Read())
            {
                if (dr[0].ToString() != "" && dr[0].ToString() != null)
                {
                    oldAmount = Convert.ToInt32(dr[0].ToString());
                }
            }

            conn.Dispose();
            return oldAmount;

        }

        public void clearCart(int userId)
        {
            conn = new OracleConnection(ordb);
            conn.Open();
            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"delete from cart
                                where cartuserId=:userId";
            cmd.Parameters.Add("userId", userId);
          
            int r = cmd.ExecuteNonQuery();
            if (r != -1)
            {
               
               
            }
            else
            {
                MessageBox.Show("You don't have this item");
            }
            conn.Dispose();
            
           

        }

    }


}