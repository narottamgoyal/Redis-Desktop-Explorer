using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedisDesktopExplorer
{
    public partial class Form1 : Form
    {
        private IDatabase _redisDb;
        private string currentItem;
        public Form1()
        {
            InitializeComponent();
            dbList.SelectedIndex = 0;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Start();
                string connectionString = $"{serverTextBox.Text}:{portTextBox.Text}";
                ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);
                ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(options);
                _redisDb = connection.GetDatabase(dbList.SelectedIndex);
                EndPoint endPoint = connection.GetEndPoints().First();
                RedisKey[] keys = connection.GetServer(endPoint).Keys(dbList.SelectedIndex, pattern: "*").ToArray();
                keysListBox.DataSource = keys;
                connectButton.Text = "Refresh";
                addButton.Enabled = true;
                editButton.Enabled = true;
            }
            catch
            { }
        }

        private void KeysListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the currently selected item in the ListBox.
            currentItem = keysListBox.SelectedItem.ToString();
            if (currentItem != null)
                resultTextBox.Text = _redisDb.StringGet(currentItem);

            resultTextBox.ReadOnly = true;
            editButton.Text = "Edit";
        }

        private void Add_Click(object sender, EventArgs e)
        {

        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (resultTextBox.ReadOnly && !string.IsNullOrWhiteSpace(currentItem))
            {
                resultTextBox.ReadOnly = false;
                editButton.Text = "Update";
            }
            else
            {
                //update
                _redisDb.StringSet(currentItem, resultTextBox.Text);
                resultTextBox.ReadOnly = true;
                editButton.Text = "Edit";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Increment(100);
        }
    }
}
