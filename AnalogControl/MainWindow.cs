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
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Console.Title = "AnaLua Controller Server";
            comboBox1.SelectedIndex = 0;
            textBox1.Text = "3478";
            textBox2.Text = "127.0.0.1";
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
            //Console.WriteLine(comboBox1.SelectedIndex);
            
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
                Program.XInpPlayerIndex = comboBox1.SelectedIndex;
                this.Close();
            }
        }
    }
}
