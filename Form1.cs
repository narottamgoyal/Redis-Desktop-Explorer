using Microsoft.VisualBasic;
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
        private BindingList<string> keys;
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
                resultTextBox.Text = string.Empty;
                string connectionString = $"{serverTextBox.Text}:{portTextBox.Text}";
                ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);
                ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(options);
                _redisDb = connection.GetDatabase(dbList.SelectedIndex);
                EndPoint endPoint = connection.GetEndPoints().First();
                var ke = connection.GetServer(endPoint).Keys(dbList.SelectedIndex, pattern: "*").ToList();
                keys = new BindingList<string>();
                foreach (var k in ke)
                    keys.Add(k);
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
            currentItem = keysListBox.SelectedItem?.ToString();
            if (currentItem != null)
            {
                resultTextBox.Text = _redisDb.StringGet(currentItem);
                ShowTime();
            }
            resultTextBox.ReadOnly = true;
            editButton.Text = "Edit";
        }

        private void ShowTime()
        {
            try
            {
                expireTimeLabel.Text = _redisDb.KeyTimeToLive(currentItem).Value.ToString();
            }
            catch { expireTimeLabel.Text = "NA"; }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            var response = Interaction.InputBox("Enter Key name", "New Key", "");
            keys.Add(response);
            keysListBox.SelectedIndex = keys.Count - 1;
            currentItem = response;
            resultTextBox.ReadOnly = false;
            editButton.Text = "Save";
            addButton.Enabled = false;
            resultTextBox.Focus();
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
                addButton.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Increment(100);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            resultTextBox.Text = string.Empty;
            if (string.IsNullOrWhiteSpace(currentItem)) return;
            keys.Remove(currentItem);
            if (string.IsNullOrWhiteSpace(currentItem)) return;
            _redisDb.KeyDelete(currentItem);
            if (keys.Count > 0)
                keysListBox.SelectedIndex = keys.Count - 1;
        }
    }
}
