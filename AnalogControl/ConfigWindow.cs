using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace AnalogControl
{
    public partial class ConfigWindow : Form
    {
        MainWindow mainwindow;

        public ConfigWindow(MainWindow main)
        {
            mainwindow = main;
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Console.Title = "AnaLua Controller Configuration";

            comboBox1.Items.Clear();

            textBox1.Text = mainwindow.outport.ToString();
            textBox2.Text = mainwindow.outip.ToString();

            foreach (GenericController controller in mainwindow.controllers)
            {
                AddItemToComboBox(controller.type);
            }

            try
            {
                comboBox1.SelectedIndex = 0;
            }
            catch
            {
                MessageBox.Show("No controller detected", "No controller detected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        public void AddItemToComboBox(string item)
        {
            comboBox1.Items.Add(item);
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(comboBox1.SelectedIndex);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!UInt16.TryParse(textBox1.Text, out Program.Port))
            {
                MessageBox.Show("Invalid Port", "Invalid Port", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else if (!IPAddress.TryParse(textBox2.Text, out Program.outaddress))
            {
                MessageBox.Show("Invalid IP Address", "Invalid IP Address", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else if (comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("No controller selected", "No controller selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                mainwindow.controllerindex = comboBox1.SelectedIndex;
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
