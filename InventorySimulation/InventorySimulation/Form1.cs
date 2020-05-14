using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventoryTesting;
using InventoryModels;

namespace InventorySimulation
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

    
        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            d.sim();

            DataGridView dataGridView1 = new DataGridView();
            dataGridView1.Size = new System.Drawing.Size(900, 450);
            dataGridView1.ScrollBars = ScrollBars.Both;
            tabControl1.Controls[0].Controls.Clear();
            tabControl1.Controls[0].Controls.Add(dataGridView1);
            dataGridView1.ScrollBars = ScrollBars.Both;

            MessageBox.Show(TestingManager.Test(d, filename));

            button2.Hide();

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dataGridView1.Columns.Add("Day", "Day");
            dataGridView1.Columns.Add("Cycle", "Cycle");
            dataGridView1.Columns.Add("DayWithinCycle", "DayWithinCycle");
            dataGridView1.Columns.Add("BeginningInventory", "BeginningInventory");
            dataGridView1.Columns.Add("RandomDemand", "RandomDemand");
            dataGridView1.Columns.Add("Demand", "Demand");
            dataGridView1.Columns.Add("EndingInventory", "EndingInventory");
            dataGridView1.Columns.Add("ShortageQuantity", "ShortageQuantity");
            dataGridView1.Columns.Add("OrderQuantity", "OrderQuantity");
            dataGridView1.Columns.Add("RandomLeadDays", "RandomLeadDays");
            dataGridView1.Columns.Add("LeadDays", "LeadDays");

            dataGridView1.Rows.Add(d.SimulationTable.Count());
            for (int i = 0; i < d.SimulationTable.Count(); i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = d.SimulationTable[i].Day;
                dataGridView1.Rows[i].Cells[1].Value = d.SimulationTable[i].Cycle;
                dataGridView1.Rows[i].Cells[2].Value = d.SimulationTable[i].DayWithinCycle;
                dataGridView1.Rows[i].Cells[3].Value = d.SimulationTable[i].BeginningInventory;
                dataGridView1.Rows[i].Cells[4].Value = d.SimulationTable[i].RandomDemand;
                dataGridView1.Rows[i].Cells[5].Value = d.SimulationTable[i].Demand;
                dataGridView1.Rows[i].Cells[6].Value = d.SimulationTable[i].EndingInventory;
                dataGridView1.Rows[i].Cells[7].Value = d.SimulationTable[i].ShortageQuantity;
                dataGridView1.Rows[i].Cells[8].Value = d.SimulationTable[i].OrderQuantity;
                dataGridView1.Rows[i].Cells[9].Value = d.SimulationTable[i].RandomLeadDays;
                dataGridView1.Rows[i].Cells[10].Value = d.SimulationTable[i].LeadDays   ;
            }


            textBox1.Text = d.PerformanceMeasures.EndingInventoryAverage.ToString();
            textBox2.Text = d.PerformanceMeasures.ShortageQuantityAverage.ToString();

        }

        private void button1_Click_1(object sender, EventArgs e)
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

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}