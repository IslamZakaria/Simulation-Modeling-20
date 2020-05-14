using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NewspaperSellerModels;
using NewspaperSellerTesting;

namespace NewspaperSellerSimulation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string filename = "";
        private void Form1_Load(object sender, EventArgs e)
        {
            SimulationSystem mysysteem = new SimulationSystem();
            openFileDialog1.ShowDialog();
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "All Files (*.*)|*.*";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;
            filename = openFileDialog1.SafeFileName ;



            mysysteem.simulate(filename);
            MessageBox.Show(TestingManager.Test(mysysteem,filename));



            var binding = new BindingList<SimulationCase>(mysysteem.SimulationTable);
            var src = new BindingSource(binding, null);
            dataGridView1.DataSource = src;

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            
        }
    }
}
