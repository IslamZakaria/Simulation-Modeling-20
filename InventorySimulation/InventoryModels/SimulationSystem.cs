using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace InventoryModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            DemandDistribution = new List<Distribution>();
            LeadDaysDistribution = new List<Distribution>();
            SimulationTable = new List<SimulationCase>();
            PerformanceMeasures = new PerformanceMeasures();
        }

        ///////////// INPUTS /////////////

        public int OrderUpTo { get; set; }
        public int ReviewPeriod { get; set; }
        public int NumberOfDays { get; set; }
     
        public int StartInventoryQuantity { get; set; }
        public int StartLeadDays { get; set; }
        public int StartOrderQuantity { get; set; }
        
        public List<Distribution> DemandDistribution { get; set; }
        public List<Distribution> LeadDaysDistribution { get; set; }

        ///////////// OUTPUTS /////////////

        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }
        public int get_dist(int rnd,List<Distribution> dis)
        {
            for (int i = 0; i < dis.Count; i++)
                if (rnd <= dis[i].MaxRange)
                    return dis[i].Value;
            return 0;
        }

        public void readfile(string fna)
        {
            FileStream fs = new FileStream(fna, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            while (sr.Peek() != -1)
            {
                string fineline = sr.ReadLine();
                if (fineline == "")
                {
                    continue;
                }
                else if (fineline.Contains("OrderUpTo"))
                {
                    OrderUpTo = int.Parse(sr.ReadLine());
                }
                else if (fineline.Contains("ReviewPeriod"))
                {
                    ReviewPeriod = int.Parse(sr.ReadLine());
                }
                else if (fineline.Contains("StartInventoryQuantity"))
                {
                    StartInventoryQuantity = int.Parse(sr.ReadLine());
                }
                else if (fineline.Contains("StartLeadDays"))
                {
                    StartLeadDays = int.Parse(sr.ReadLine());
                }
                else if (fineline.Contains("StartOrderQuantity"))
                {
                    StartOrderQuantity = int.Parse(sr.ReadLine());
                }
                else if (fineline.Contains("NumberOfDays"))
                {
                    NumberOfDays = int.Parse(sr.ReadLine());
                }
                else if (fineline.Contains("DemandDistribution"))
                {
                    int cp = 0;
                    while (true)
                    {
                        string td = sr.ReadLine();
                        if (td == "")
                        {
                            break;
                        }
                        string[] l = td.Split(',');
                        Distribution x = new Distribution();
                        x.Value = int.Parse(l[0]);
                        int pro = Convert.ToInt32(float.Parse(l[1]) * 100);
                        x.Probability = pro;
                        x.CummProbability = cp + pro;
                        x.MinRange = cp + 1;
                        cp += pro;
                        x.MaxRange = cp;
                        DemandDistribution.Add(x);
                    }
                }
                else if (fineline.Contains("LeadDaysDistribution"))
                {
                    int cp = 0;
                    while (true)
                    {
                        string td = sr.ReadLine();
                        if (td == "" || td == null)
                        {
                            break;
                        }
                        string[] l = td.Split(',');
                        Distribution x = new Distribution();
                        x.Value = int.Parse(l[0]);
                        int pro = Convert.ToInt32(float.Parse(l[1]) * 100);
                        x.Probability = pro;
                        x.CummProbability = cp + pro;
                        x.MinRange = cp + 1;
                        cp += pro;
                        x.MaxRange = cp;
                        LeadDaysDistribution.Add(x);
                    }
                }
            }
            fs.Close();
        }

        public void sim()
        {
            int ncycle = NumberOfDays / ReviewPeriod ;
            int da = 1;
            Random rnd = new Random ();
            Random rndd = new Random ();
            int startshortq = 0;
            for (int i = 1; i <= ncycle; i++)
            {
                for (int j = 1; j <= ReviewPeriod; j++)
                {
                    if (j ==(StartLeadDays+1))
                    {
                        StartInventoryQuantity += StartOrderQuantity;      
                    }
                    SimulationCase c = new SimulationCase();
                    c.Day = da;
                    da++;
                    c.Cycle = i;
                    c.DayWithinCycle = j ;
                    c.BeginningInventory = StartInventoryQuantity;
                    c.RandomDemand = rnd.Next(1, 101);
                    c.Demand = get_dist(c.RandomDemand, DemandDistribution);
                    if (c.Demand+startshortq <= c.BeginningInventory)
                    {  
                            c.EndingInventory = c.BeginningInventory - c.Demand - startshortq;
                            startshortq = 0;
                            StartInventoryQuantity = c.EndingInventory;   
                    }
                    else if (c.Demand + startshortq > c.BeginningInventory)
                    {
                        c.EndingInventory = 0;
                        StartInventoryQuantity =0;
                        c.ShortageQuantity = c.Demand - c.BeginningInventory + startshortq;
                        startshortq = c.ShortageQuantity;
                    }
                    if (j==ReviewPeriod)
                    {
                        c.OrderQuantity = OrderUpTo - c.EndingInventory + c.ShortageQuantity;
                        StartOrderQuantity = c.OrderQuantity;
                        c.RandomLeadDays = rndd.Next(1,101);
                        c.LeadDays = get_dist(c.RandomLeadDays, LeadDaysDistribution);
                        StartLeadDays = c.LeadDays;
                    }
                    PerformanceMeasures.EndingInventoryAverage += c.EndingInventory;
                    PerformanceMeasures.ShortageQuantityAverage += c.ShortageQuantity;
                    SimulationTable.Add(c);
                }
            }
            PerformanceMeasures.EndingInventoryAverage = (decimal)(PerformanceMeasures.EndingInventoryAverage/NumberOfDays);
            PerformanceMeasures.ShortageQuantityAverage = (decimal)(PerformanceMeasures.ShortageQuantityAverage/NumberOfDays);
        }

    }
}
