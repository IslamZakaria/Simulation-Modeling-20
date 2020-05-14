using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BearingMachineTesting;
using BearingMachineModels;
namespace BearingMachineSimulation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string filename = "";
        SimulationSystem d;
        public void Form1_Load(object sender, EventArgs e)
        {
            button2.Hide();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            d = new SimulationSystem();
            openFileDialog1.ShowDialog();
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "All Files (*.*)|*.*";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;
            filename = openFileDialog1.SafeFileName;
            d.readfile(filename);
            button2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            DataGridView dataGridView1 = new DataGridView();
            DataGridView dataGridView2 = new DataGridView();
    
            dataGridView1.Size = new System.Drawing.Size(700,450);
            dataGridView2.Size = new System.Drawing.Size(1000,450);
            dataGridView1.ScrollBars = ScrollBars.Both;
            dataGridView2.ScrollBars = ScrollBars.Both;
            tabControl1.Controls[0].Controls.Clear();
            tabControl1.Controls[1].Controls.Clear();

            tabControl1.Controls[0].Controls.Add(dataGridView1);
            tabControl1.Controls[1].Controls.Add(dataGridView2);
        
            d.simulate();
            MessageBox.Show(TestingManager.Test(d, filename));
            button2.Hide();

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dataGridView1.Columns.Add("Index", "Index");
            dataGridView1.Columns.Add("RandomHours", "RandomHours");
            dataGridView1.Columns.Add("Hours", "Hours");
            dataGridView1.Columns.Add("AccumulatedHours", "AccumulatedHours");
            dataGridView1.Columns.Add("RandomDelay", "RandomDelay");
            dataGridView1.Columns.Add("Delay", "Delay");

            dataGridView1.Rows.Add(d.CurrentSimulationTable.Count());
            for (int i = 0; i < d.CurrentSimulationTable.Count(); i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = d.CurrentSimulationTable[i].Bearing.Index;
                dataGridView1.Rows[i].Cells[1].Value = d.CurrentSimulationTable[i].Bearing.RandomHours;
                dataGridView1.Rows[i].Cells[2].Value = d.CurrentSimulationTable[i].Bearing.Hours;
                dataGridView1.Rows[i].Cells[3].Value = d.CurrentSimulationTable[i].AccumulatedHours;
                dataGridView1.Rows[i].Cells[4].Value = d.CurrentSimulationTable[i].RandomDelay;
                dataGridView1.Rows[i].Cells[5].Value = d.CurrentSimulationTable[i].Delay ;
            }

            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            for (int j = 0; j < d.NumberOfBearings; j++)
                dataGridView2.Columns.Add("Bearing"+(j+1), "Bearing"+(j+1));

            dataGridView2.Columns.Add("FirstFailure", "FirstFailure");
            dataGridView2.Columns.Add("AccumulatedHours", "AccumulatedHours");
            dataGridView2.Columns.Add("RandomDelay", "RandomDelay");
            dataGridView2.Columns.Add("Delay", "Delay");

            dataGridView2.Rows.Add(d.ProposedSimulationTable.Count());

            for (int i = 0; i < d.ProposedSimulationTable.Count(); i++)
            {
                int j = 0;
                for (j = 0; j < d.NumberOfBearings; j++)
                        dataGridView2.Rows[i].Cells[j].Value = d.ProposedSimulationTable[i].Bearings[j].Hours;

                dataGridView2.Rows[i].Cells[j].Value = d.ProposedSimulationTable[i].FirstFailure;
                dataGridView2.Rows[i].Cells[j+1].Value = d.ProposedSimulationTable[i].AccumulatedHours;
                dataGridView2.Rows[i].Cells[j+2].Value = d.ProposedSimulationTable[i].RandomDelay;
                dataGridView2.Rows[i].Cells[j+3].Value = d.ProposedSimulationTable[i].Delay;
            }
            textBox1.Text = d.CurrentPerformanceMeasures.BearingCost.ToString();
            textBox2.Text = d.CurrentPerformanceMeasures.DelayCost.ToString();
            textBox3.Text = d.CurrentPerformanceMeasures.DowntimeCost.ToString();
            textBox4.Text = d.CurrentPerformanceMeasures.RepairPersonCost.ToString();
            textBox5.Text = d.CurrentPerformanceMeasures.TotalCost.ToString();

            textBox10.Text = d.ProposedPerformanceMeasures.BearingCost.ToString();
            textBox9.Text = d.ProposedPerformanceMeasures.DelayCost.ToString();
            textBox8.Text = d.ProposedPerformanceMeasures.DowntimeCost.ToString();
            textBox7.Text = d.ProposedPerformanceMeasures.RepairPersonCost.ToString();
            textBox6.Text = d.ProposedPerformanceMeasures.TotalCost.ToString();

        }
    }
}
