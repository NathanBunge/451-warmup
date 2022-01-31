using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Npgsql;

namespace Milesstone1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private string bid = "";
        public Window1(string bid)
        {
            InitializeComponent();
            this.bid = String.Copy(bid);
            loadBusinessDetails();
            loadBusinessNumbers();
        }



        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = milestone1db; password = HockeysaintP4!";
        }

        private void executeQuery(string sqlstr, Action<NpgsqlDataReader> myf)
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = sqlstr;
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        reader.Read();
                        myf(reader);
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                        System.Windows.MessageBox.Show(ex.Message); 
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

        }


        void setNumInState(NpgsqlDataReader R)
        {
            numInState.Content = R.GetInt16(0).ToString();
        }

        void setNumInCity(NpgsqlDataReader R)
        {
            numInCity.Content = R.GetInt16(0).ToString();
        }

        private void setBusinessDetails(NpgsqlDataReader R)
        {
            bname.Text = R.GetString(0);
            cname.Text = R.GetString(1);
            sname.Text = R.GetString(2);
        }

        private void loadBusinessNumbers()
        {
            string sqlStr1 = "SELECT count(*) from buisiness WHERE state = (SELECT state FROM buisiness WHERE business_id = '" + this.bid + "');";
            executeQuery(sqlStr1, setNumInState);
            string sqlStr2 = "SELECT count(*) from buisiness WHERE city = (SELECT city FROM buisiness WHERE business_id = '" + this.bid + "');";
            executeQuery(sqlStr2, setNumInCity);
        }

        private void loadBusinessDetails()
        {
            string sqlStr = "SELECT name, state, city FROM buisiness WHERE business_id = '" + this.bid + "';";
            executeQuery(sqlStr, setBusinessDetails);
        }
    }

}
