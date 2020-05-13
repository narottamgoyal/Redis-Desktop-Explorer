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
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string connectionString = $"{serverTextBox.Text}:{portTextBox.Text}";
            ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(options);
            _redisDb = connection.GetDatabase();
            EndPoint endPoint = connection.GetEndPoints().First();
            RedisKey[] keys = connection.GetServer(endPoint).Keys(pattern: "*").ToArray();
            keysListBox.DataSource = keys;
        }

        private void KeysListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the currently selected item in the ListBox.
            string currentItem = keysListBox.SelectedItem.ToString();
            if (currentItem != null)
                resultTextBox.Text = _redisDb.StringGet(currentItem);
        }
    }
}
