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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace Milesstone1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Business
        {
            public string bid { get; set; }
            public string name { get; set; }
            public string city { get; set; }
            public string state { get; set; }

        }


        public MainWindow()
        {
            InitializeComponent();
            addState();
            addColumnsGrid();
        }

        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = milestone1db; password = HockeysaintP4!";
        }
        private void addState()
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT distinct state FROM buisiness ORDER BY state";
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                            statelist.Items.Add(reader.GetString(0));
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                        System.Windows.MessageBox.Show("SQL Error: " + ex.Message.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void addColumnsGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "BusinessName";
            col1.Width = 255;
            col1.Binding = new Binding("name");
            businessgrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "State";
            col2.Width = 60;
            col2.Binding = new Binding("state");
            businessgrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "City";
            col3.Width = 150;
            col3.Binding = new Binding("city");
            businessgrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "";
            col4.Width = 0;
            col4.Binding = new Binding("bid");
            businessgrid.Columns.Add(col4);
        }

        private void statelist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            citylist.Items.Clear();
            if (statelist.SelectedIndex > -1)
            {


                using (var connection = new NpgsqlConnection(buildConnectionString()))
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand())
                    {

                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT distinct city FROM buisiness WHERE state = '" + statelist.SelectedItem.ToString() + "' ORDER BY city";
                        try
                        {
                            var reader = cmd.ExecuteReader();
                            while (reader.Read())
                                citylist.Items.Add(reader.GetString(0));
                        }
                        catch (NpgsqlException ex)
                        {
                            Console.WriteLine(ex.Message.ToString());
                            System.Windows.MessageBox.Show("SQL Error: " + ex.Message.ToString());
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
            
        }

        private void citylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            businessgrid.Items.Clear();
            if (citylist.SelectedIndex > -1)
            {


                using (var connection = new NpgsqlConnection(buildConnectionString()))
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand())
                    {

                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT name, state, city, business_id  FROM buisiness WHERE state = '" + statelist.SelectedItem.ToString() + "' AND city = '" + citylist.SelectedItem.ToString() + "' ORDER BY name;";
                        try
                        {
                            var reader = cmd.ExecuteReader();
                            while (reader.Read())
                                businessgrid.Items.Add(new Business() { name = reader.GetString(0), state = reader.GetString(1), city = reader.GetString(2), bid = reader.GetString(3) });
                        }
                        catch (NpgsqlException ex)
                        {
                            Console.WriteLine(ex.Message.ToString());
                            System.Windows.MessageBox.Show("SQL Error: " + ex.Message.ToString());
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }

        }

        private void businessgrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (businessgrid.SelectedIndex > -1)
            {
                Business B = businessgrid.Items[businessgrid.SelectedIndex] as Business;
                if((B.bid != null) && (B.bid.ToString().CompareTo("") != 0))
                {
                    Window1 businessWindow = new Window1(B.bid.ToString());
                    businessWindow.Show();
                }
            }
        }
    }
}
