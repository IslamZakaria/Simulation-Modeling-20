using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace NewspaperSellerModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            DayTypeDistributions = new List<DayTypeDistribution>();
            DemandDistributions = new List<DemandDistribution>();
            SimulationTable = new List<SimulationCase>();
            PerformanceMeasures = new PerformanceMeasures();
        }
        ///////////// INPUTS /////////////
        public int NumOfNewspapers { get; set; }
        public int NumOfRecords { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal ScrapPrice { get; set; }
        public decimal UnitProfit { get; set; }
        public List<DayTypeDistribution> DayTypeDistributions { get; set; }
        public List<DemandDistribution> DemandDistributions { get; set; }

        ///////////// OUTPUTS /////////////
        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }


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
                else if (fineline == "NumOfNewspapers")
                {
                    NumOfNewspapers = int.Parse(sr.ReadLine());
                }
                else if (fineline == "NumOfRecords")
                {
                    NumOfRecords = int.Parse(sr.ReadLine());
                }
                else if (fineline == "PurchasePrice")
                {
                    PurchasePrice = decimal.Parse(sr.ReadLine());
                }
                else if (fineline == "ScrapPrice")
                {
                    ScrapPrice = decimal.Parse(sr.ReadLine());
                }
                else if (fineline == "SellingPrice")
                {
                    SellingPrice = decimal.Parse(sr.ReadLine());
                    UnitProfit = SellingPrice - PurchasePrice;
                }
                else if (fineline == "DayTypeDistributions")
                {
                    string[] g = sr.ReadLine().Split(',');
                    int cp = 0;
                    for (int f = 0; f < g.Length; f++)
                    {
                        DayTypeDistribution xy = new DayTypeDistribution();
                        int prob = Convert.ToInt32(decimal.Parse(g[f]) * 100);
                        xy.Probability = prob;
                        xy.MinRange = cp+1;
                        cp += prob;
                        xy.CummProbability = cp;
                        xy.MaxRange = cp;
                        if (f==0)
                            xy.DayType = Enums.DayType.Good;
                        else if (f == 1)
                            xy.DayType = Enums.DayType.Fair;
                        else if (f == 2)
                            xy.DayType = Enums.DayType.Poor;
                        DayTypeDistributions.Add(xy);

                    }
                }
                else if (fineline == "DemandDistributions")
                {
                    int []cp = new int[3];
                    while (sr.Peek() != -1)
                    {

                        string td = sr.ReadLine();
                        if (td == "" || td==null)
                        {
                            break;
                        }
                        string[] l = td.Split(',');
                        DemandDistribution u = new DemandDistribution();
                        u.Demand = int.Parse(l[0]);
                        for (int f = 0; f < 3; f++)
                        {
                            DayTypeDistribution xy = new DayTypeDistribution();
                            int prob = Convert.ToInt32(decimal.Parse(l[f+1]) * 100);
                            xy.Probability = prob;
                            xy.MinRange = cp[f] + 1;
                            cp[f] += prob;
                            xy.CummProbability = cp[f];
                            xy.MaxRange = cp[f];
                            if (f == 0)
                                xy.DayType = Enums.DayType.Good;
                            else if (f == 1)
                                xy.DayType = Enums.DayType.Fair;
                            else if (f == 2)
                                xy.DayType = Enums.DayType.Poor;
                            u.DayTypeDistributions.Add(xy);

                        }
                        DemandDistributions.Add(u);
                    }


                }
              
            }
            fs.Close();

        }
        public int get_demand(int ran,Enums.DayType da)
        {
            for (int i = 0; i < DemandDistributions.Count; i++)
            {
                for (int j = 0; j < DemandDistributions[i].DayTypeDistributions.Count; j++)
                {
                    if (DemandDistributions[i].DayTypeDistributions[j].MinRange <= ran && DemandDistributions[i].DayTypeDistributions[j].MaxRange >= ran && DemandDistributions[i].DayTypeDistributions[j].DayType ==da)
                    {
                        return DemandDistributions[i].Demand;
                    }

                }
         
            }
            return 0;
        }
        public void simulate(string fnam)
        {
            readfile(fnam);
            Random day = new Random();
            Random demand = new Random();
            for (int i = 1; i <= NumOfRecords; i++)
            {
                SimulationCase sc = new SimulationCase();
                sc.DayNo = i;
                sc.RandomNewsDayType = day.Next(1,100);
                for (int h = 0; h < DayTypeDistributions.Count; h++)
                {
                    if (DayTypeDistributions[h].MinRange <= sc.RandomNewsDayType && DayTypeDistributions[h].MaxRange >= sc.RandomNewsDayType)
                    {
                        sc.NewsDayType = DayTypeDistributions[h].DayType;
                        break;
                    }

                }
                sc.RandomDemand = demand.Next(1,100);
                sc.Demand = get_demand(sc.RandomDemand, sc.NewsDayType);
                sc.DailyCost = NumOfNewspapers * PurchasePrice;
                sc.SalesProfit = Math.Min(sc.Demand,NumOfNewspapers) * SellingPrice;
                
                if (sc.Demand < NumOfNewspapers)
                {
                    sc.ScrapProfit = (NumOfNewspapers-sc.Demand) * ScrapPrice;
                    sc.DailyNetProfit = sc.SalesProfit + sc.ScrapProfit - sc.DailyCost;
                }
                else if (sc.Demand > NumOfNewspapers)
                {
                    sc.LostProfit = (sc.Demand - NumOfNewspapers) * UnitProfit;
                    sc.DailyNetProfit = sc.SalesProfit - sc.LostProfit - sc.DailyCost;
                }
                else
                    sc.DailyNetProfit = sc.SalesProfit - sc.DailyCost;

                if (sc.ScrapProfit != 0)
                    PerformanceMeasures.DaysWithUnsoldPapers += 1;
                if (sc.LostProfit != 0)
                    PerformanceMeasures.DaysWithMoreDemand += 1;
                PerformanceMeasures.TotalLostProfit += sc.LostProfit;
                PerformanceMeasures.TotalScrapProfit += sc.ScrapProfit;
                PerformanceMeasures.TotalCost += sc.DailyCost;
                PerformanceMeasures.TotalNetProfit += sc.DailyNetProfit;
                PerformanceMeasures.TotalSalesProfit += sc.SalesProfit;

                SimulationTable.Add(sc);
            }

        }
        public void calc_mss()
        {
            for (int i = 0; i < SimulationTable.Count; i++)
            {
                if (SimulationTable[i].ScrapProfit != 0)
                    PerformanceMeasures.DaysWithUnsoldPapers += 1;
                if (SimulationTable[i].LostProfit != 0)
                    PerformanceMeasures.DaysWithMoreDemand += 1;
                PerformanceMeasures.TotalLostProfit += SimulationTable[i].LostProfit;
                PerformanceMeasures.TotalScrapProfit += SimulationTable[i].ScrapProfit;
                PerformanceMeasures.TotalCost += SimulationTable[i].DailyCost;
                PerformanceMeasures.TotalNetProfit += SimulationTable[i].DailyNetProfit;
                PerformanceMeasures.TotalSalesProfit += SimulationTable[i].SalesProfit;
            }

        }
    }
}
