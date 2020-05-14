using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueModels;
using MultiQueueTesting;
using System.IO;

namespace MultiQueueSimulation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SimulationSystem mysystem = new SimulationSystem();
        private void Form1_Load(object sender, EventArgs e)
        {
            FileStream fs = new FileStream("TestCase1.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            while (sr.Peek() != -1)
            {
                string fineline = sr.ReadLine();
                if (fineline == "")
                {
                    continue;
                }
                else if (fineline == "NumberOfServers")
                {
                    mysystem.NumberOfServers = int.Parse(sr.ReadLine());
                }
                else if (fineline == "StoppingNumber")
                {
                    mysystem.StoppingNumber = int.Parse(sr.ReadLine());
                }
                else if (fineline == "StoppingCriteria")
                {
                    int x = int.Parse(sr.ReadLine());
                    if (x == 1)
                    {
                        mysystem.StoppingCriteria = Enums.StoppingCriteria.NumberOfCustomers;
                    }
                    else
                    {
                        mysystem.StoppingCriteria = Enums.StoppingCriteria.SimulationEndTime;
                    }
                }
                else if (fineline == "SelectionMethod")
                {
                    int y = int.Parse(sr.ReadLine());
                    if (y == 1)
                    {
                        mysystem.SelectionMethod = Enums.SelectionMethod.HighestPriority;
                    }
                    else if (y == 2)
                    {
                        mysystem.SelectionMethod = Enums.SelectionMethod.Random;
                    }
                    else
                    {
                        mysystem.SelectionMethod = Enums.SelectionMethod.LeastUtilization;
                    }
                }
                else if (fineline== "InterarrivalDistribution")
                {
                    int cp = 0;
                    while (true)
                    {
                        
                        string td = sr.ReadLine();
                        if (td=="")
                        {
                            break; 
                        }
                        string[] l = td.Split(',');
                        TimeDistribution x = new TimeDistribution();
                        x.Time = int.Parse(l[0]);
                        int pro = Convert.ToInt32(float.Parse(l[1]) * 100);
                        x.Probability = pro;
                        x.CummProbability =cp+pro ;
                        x.MinRange = cp+1;
                        cp += pro;
                        x.MaxRange = cp;
                        mysystem.InterarrivalDistribution.Add(x);
                    }


                }
                else if (fineline.Contains("ServiceDistribution_Server"))
                {
                    for (int i = 0; i < mysystem.NumberOfServers; i++)
                    {
                        Server min = new Server();
                        min.ID = i + 1;
                        min.FinishTime = 0;
                        int cp = 0;
                        while (true)
                        {

                            string td = sr.ReadLine();
                            if (td == ""||td == null)
                            {
                                sr.ReadLine();
                                break;
                            }

                            string[] l = td.Split(',');
                            TimeDistribution x = new TimeDistribution();
                            x.Time = int.Parse(l[0]);
                            int pro = Convert.ToInt32(float.Parse(l[1]) * 100);
                            x.Probability = pro;
                            x.CummProbability = cp + pro;
                            x.MinRange = cp + 1;
                            cp += pro;
                            x.MaxRange = cp;
                            min.TimeDistribution.Add(x);
                        }

                        mysystem.Servers.Add(min);
                    }

                }
                //string[] l = sr.ReadLine().Split(',');
                //TimeDistribution x = new TimeDistribution();
                //x.Time = int.Parse(l[0]);
                //x.Probability = int.Parse(l[1]);
                //x.CummProbability = int.Parse(l[2]);
                //x.MinRange = int.Parse(l[3]);
                //x.MaxRange = int.Parse(l[4]);
                //TimeDistribution x = new TimeDistribution();
                //List < TimeDistribution > inter = x.gettable();
                //List<TimeDistribution> able = x.gettable_able();
                //List<TimeDistribution> baker = x.gettable_baker();

            }
            fs.Close();
            if (mysystem.StoppingCriteria==Enums.StoppingCriteria.NumberOfCustomers)
            {
                Random rnd = new Random();
                Random rndforservice = new Random();
                Random rndservers = new Random();
                int artime = 0;
                for (int i = 0; i < mysystem.StoppingNumber; i++)
                {
                    SimulationCase row = new SimulationCase();
                    if (i == 0)
                    {
                        row.CustomerNumber = 1;
                        row.InterArrival = 0;
                        row.ArrivalTime = 0;
                        row.RandomInterArrival = 1;
                        int st = rndforservice.Next(1, 100);
                        row.RandomService = st;
                        int id = 0;
                        if (mysystem.SelectionMethod == Enums.SelectionMethod.HighestPriority)
                        {
                            id = 0;
                        }
                        else if (mysystem.SelectionMethod == Enums.SelectionMethod.Random)
                        {
                             id  = rndservers.Next(0, mysystem.NumberOfServers);
                           
                        }
                        row.ServiceTime = getim(st, mysystem.Servers[id].TimeDistribution);
                        row.AssignedServer = mysystem.Servers[id];
                        mysystem.Servers[id].FinishTime = row.ServiceTime;
                        mysystem.Servers[id].TotalWorkingTime += row.ServiceTime;
                        row.StartTime = 0;
                        row.EndTime = row.ServiceTime;
                        row.TimeInQueue = 0;
                        mysystem.SimulationTable.Add(row);
                    }
                    else
                    {

                        row.CustomerNumber = i+1;
                        row.RandomInterArrival = rnd.Next(1, 100);
                        row.InterArrival = getim(row.RandomInterArrival,mysystem.InterarrivalDistribution);
                        artime += row.InterArrival;
                        row.ArrivalTime = artime;
                        int st = rndforservice.Next(1, 100);
                        row.RandomService = st;
                        int sr_id = 0;
                        if (mysystem.SelectionMethod == Enums.SelectionMethod.HighestPriority)
                        {
                            sr_id = get_server(mysystem.Servers, row.ArrivalTime);
                        }
                        else if (mysystem.SelectionMethod == Enums.SelectionMethod.Random)
                        {
                            sr_id = get_server_rc(mysystem.Servers, row.ArrivalTime);
                        }
                        else if (mysystem.SelectionMethod == Enums.SelectionMethod.LeastUtilization)
                        {
                            sr_id = least_ut();
                        }
                        row.ServiceTime = getim(st, mysystem.Servers[sr_id].TimeDistribution);

                        row.StartTime = maxx(mysystem.Servers[sr_id].FinishTime, row.ArrivalTime);

                        row.EndTime = row.StartTime + row.ServiceTime;

                        row.TimeInQueue = row.StartTime - row.ArrivalTime;

                        row.AssignedServer = mysystem.Servers[sr_id];

                        mysystem.Servers[sr_id].FinishTime = row.EndTime;

                        mysystem.Servers[sr_id].TotalWorkingTime += row.ServiceTime;
                        mysystem.SimulationTable.Add(row);
                    }

                }
            }
            else if (mysystem.StoppingCriteria == Enums.StoppingCriteria.SimulationEndTime)
            {
                Random rnd = new Random();
                Random rndforservice = new Random();
                Random rndservers = new Random();
                int artime = 0;
                int ghg = 1;
                while (artime<mysystem.StoppingNumber)
                {
                    SimulationCase row = new SimulationCase();
                    if (artime == 0)
                    {
                        row.CustomerNumber = 1;
                        row.InterArrival = 0;
                        row.ArrivalTime = 0;
                        row.RandomInterArrival = 1;
                        int st = rndforservice.Next(1, 100);
                        row.RandomService = st;
                        if (mysystem.SelectionMethod == Enums.SelectionMethod.HighestPriority)
                        {
                            row.ServiceTime = getim(st, mysystem.Servers[0].TimeDistribution);
                            row.AssignedServer = mysystem.Servers[0];
                            mysystem.Servers[0].FinishTime = row.ServiceTime;
                            mysystem.Servers[0].TotalWorkingTime += row.ServiceTime;
                        }
                        else if (mysystem.SelectionMethod == Enums.SelectionMethod.Random)
                        {
                            int id = rndservers.Next(0, mysystem.NumberOfServers);
                            row.ServiceTime = getim(st, mysystem.Servers[id].TimeDistribution);
                            row.AssignedServer = mysystem.Servers[id];
                            mysystem.Servers[id].FinishTime = row.ServiceTime;
                            mysystem.Servers[id].TotalWorkingTime += row.ServiceTime;
                        }
                        row.StartTime = 0;
                        row.EndTime = row.ServiceTime;
                        row.TimeInQueue = 0;
                        mysystem.SimulationTable.Add(row);
                    }
                    else
                    {

                        row.CustomerNumber = ghg;
                        ghg += 1;
                        row.RandomInterArrival = rnd.Next(1, 100);
                        row.InterArrival = getim(row.RandomInterArrival, mysystem.InterarrivalDistribution);
                        artime += row.InterArrival;
                        row.ArrivalTime = artime;
                        int st = rndforservice.Next(1, 100);
                        row.RandomService = st;
                        int sr_id = 0;
                        if (mysystem.SelectionMethod == Enums.SelectionMethod.HighestPriority)
                        {
                            sr_id = get_server(mysystem.Servers, row.ArrivalTime);
                        }
                        else if (mysystem.SelectionMethod == Enums.SelectionMethod.Random)
                        {
                            sr_id = get_server_rc(mysystem.Servers, row.ArrivalTime);
                        }
                        else if (mysystem.SelectionMethod == Enums.SelectionMethod.LeastUtilization)
                        {
                            sr_id = least_ut();
                        }
                        row.ServiceTime = getim(st, mysystem.Servers[sr_id].TimeDistribution);

                        row.StartTime = maxx(mysystem.Servers[sr_id].FinishTime, row.ArrivalTime);

                        row.EndTime = row.StartTime + row.ServiceTime;

                        row.TimeInQueue = row.StartTime - row.ArrivalTime;

                        row.AssignedServer = mysystem.Servers[sr_id];

                        mysystem.Servers[sr_id].FinishTime = row.EndTime;

                        mysystem.Servers[sr_id].TotalWorkingTime += row.ServiceTime;
                        mysystem.SimulationTable.Add(row);
                    }

                }


            }
            int cou = 0;
            int wait_prob = 0;
            for (int v = 0; v < mysystem.SimulationTable.Count; v++)
            {
                if (mysystem.SimulationTable[v].TimeInQueue>0)
                {
                    wait_prob += 1;
                }
                cou += mysystem.SimulationTable[v].TimeInQueue;

            }
            decimal n = (decimal)cou / mysystem.SimulationTable.Count;
            mysystem.PerformanceMeasures.AverageWaitingTime = n;
            n = (decimal)wait_prob / mysystem.SimulationTable.Count;
            mysystem.PerformanceMeasures.WaitingProbability = n;
            mysystem.PerformanceMeasures.MaxQueueLength = max_q_len(mysystem.SimulationTable);
            List<int> myli = avg_st(mysystem.SimulationTable ,mysystem.NumberOfServers);
            for (int u =0; u <mysystem.NumberOfServers;u++)
            {
                if (myli[u] == 0)
                {
                    mysystem.Servers[u].AverageServiceTime = 0;
                }
                else
                    mysystem.Servers[u].AverageServiceTime = (decimal)mysystem.Servers[u].TotalWorkingTime / myli[u];

                int dif = max_finish(mysystem.Servers) - mysystem.Servers[u].TotalWorkingTime;

                mysystem.Servers[u].IdleProbability = (decimal)dif / max_finish(mysystem.Servers);

                mysystem.Servers[u].Utilization= (decimal)mysystem.Servers[u].TotalWorkingTime / max_finish(mysystem.Servers);
            }
            for (int s = 0; s<mysystem.NumberOfServers; s++)
            {
                comboBox1.Items.Add(s+1);
            }
            comboBox1.SelectedItem = comboBox1.Items[0];
            MessageBox.Show(TestingManager.Test(mysystem,Constants.FileNames.TestCase1));
            var binding =new  BindingList<SimulationCase>(mysystem.SimulationTable);
            var src = new BindingSource(binding,null);
            dataGridView1.DataSource = src;
        }
        public int least_ut()
        {
            decimal min = 200000;
            int id = 0;
            for (int u = 0; u < mysystem.NumberOfServers; u++)
            {

            decimal v = (decimal)mysystem.Servers[u].TotalWorkingTime / max_finish(mysystem.Servers);
                if (v<min)
                {
                    min = v;
                    id = u;
                }
            }
            return id;
        }
        public int max_finish(List <Server> c)
        {
            int max = 0;

            for (int i = 0; i < c.Count; i++)
            {
                if (c[i].FinishTime > max)
                {
                    max = c[i].FinishTime;
                }
            }
            return max; 
        }
        public List<int> avg_st(List<SimulationCase> ca , int no)
        {
            List<int> custs = new List<int>();
            for (int i =0; i <no; i++)
            {
                custs.Add(0);
            }
            for (int y = 0; y< ca.Count; y++)
            {
                custs[ca[y].AssignedServer.ID-1]+= 1;
            }
            return custs;
        }
        public int maxx(int x , int y)
        {

            if (x>y)
            {
                return x;
            }
            else if (y>=x)
            {
                return y;
            }
            return 0;
        }
        public int max_q_len(List<SimulationCase> h)
        {
            int len = 0;
            int cur_max = 0;
            int bol = 0;
            int holding = 0;
            for (int y = 0; y < h.Count; y++)
            {
                if (h[y].TimeInQueue > 0 )
                {
                    for (int i = y; i < h.Count; i++)
                    {
                        if (h[i].TimeInQueue > 0 && bol == 0)
                        {
                            holding = h[i].StartTime;
                            len += 1;
                            bol = 1;
                        }
                        else if (h[i].TimeInQueue > 0 && bol == 1 && h[i].ArrivalTime < holding)
                        {
                            len += 1;

                        }
                        else if (h[i].ArrivalTime > holding && bol == 1)
                        {
                
                            break;
                        }
                    }
                    if (bol == 1 && len > cur_max)
                    {
                        cur_max = len;

                    }
                    bol = 0;
                    holding = 0;
                    len = 0;
                }
            }
        
            return cur_max;
        }
        public int get_server(List<Server> b,int ar)
        {
            int sr_id = 0;
            int min = 10000;
            for (int i = 0; i < b.Count; i++)
            {
                if (b[i].FinishTime <= ar)
                {
                    sr_id = i;
                    break;
                }
                else if (b[i].FinishTime<min)
                {
                    min = b[i].FinishTime;
                    sr_id = i;
                }
            }
   
            return sr_id;
        }
        public int get_server_rc(List<Server> b , int ar)
        {
            List<Server> r_list = new List<Server>();
            for (int i = 0; i <b.Count; i++)
            {
                if (b[i].FinishTime<=ar)
                {
                    r_list.Add(b[i]);
                }

            }
            if (r_list.Count != 0)
            {
            Random rnd = new Random();
            int rn = rnd.Next(0,r_list.Count);
                return r_list[rn].ID-1;
            }
            int min = 1000000;
            int sr_id = 0;
            for (int y = 0; y < b.Count; y++)
            {

                  if (b[y].FinishTime < min)
                {
                    min = b[y].FinishTime;
                    sr_id = y;
                }
            }

            return sr_id;
        }
        public int getim(int cprob , List<TimeDistribution> b)
        {
            for (int i = 0; i < b.Count; i++)
            {
                if (cprob >= b[i].MinRange && cprob <= b[i].MaxRange)
                {
                    return b[i].Time;
                }
            }
            return 0;
        }    
        private void button1_Click_1(object sender, EventArgs e)
        {
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }
            int serv_nu = int.Parse(comboBox1.SelectedItem.ToString());
            List<SimulationCase> lst_chrt = mysystem.SimulationTable;
            for (int i = 0; i <lst_chrt.Count; i++)
            {
                if (lst_chrt[i].AssignedServer.ID==serv_nu)
                {
                    for (int y = lst_chrt[i].StartTime; y <= lst_chrt[i].EndTime; y++)
                    {
                        chart1.Series["Server Busy Time"].Points.AddXY(y, 1);

                    }
                }
            }
            
        }
    }
}
