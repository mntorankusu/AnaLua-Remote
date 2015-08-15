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
        public bool workerActive = false;
        
        public List<GenericController> controllers;

        public MainWindow()
        {
            InitializeComponent();
            config = new ConfigWindow(this);
            controllers = GenericController.FindControllers();
            workerActive = true;
            backgroundWorker1.RunWorkerAsync();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("Init background thread");

            network = new Network(outip, outport);

            while (true)
            {
                if (!workerActive) { break; }

                Thread.Sleep(1);

                controllers[controllerindex].Update();
            }

            network.Close();

            Console.WriteLine("End background thread");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            workerActive = false;
            config.ShowDialog(this);

            workerActive = true;
            backgroundWorker1.RunWorkerAsync();

            textBox1.Text = controllers[controllerindex].type;
        }
    }
}
