using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace AnalogControl
{
    public partial class MainWindow : Form
    {

        Network network;
        public ConfigWindow config;
        public string outip = "127.0.0.1";
        public UInt16 outport = 3478;
        public int controllerindex = -0;

        public List<GenericController> controllers;

        public MainWindow()
        {
            InitializeComponent();
            network = new Network();
            config = new ConfigWindow(this);
            backgroundWorker1_DoWork(this, new DoWorkEventArgs(null));
            controllers = GenericController.FindControllers();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (controllerindex > 0)
            { 
                textBox1.Text = String.Format("XInput Controller {0}", controllerindex);
            } else
            {
                textBox1.Text = "No controller";
            }

            textBox2.Text = outip;
            textBox3.Text = outport.ToString();
            Thread.Sleep(8);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            config.ShowDialog(this);
            textBox1.Text = controllers[controllerindex].type;
        }
    }
}
