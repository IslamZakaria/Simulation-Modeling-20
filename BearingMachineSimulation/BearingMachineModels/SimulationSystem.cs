using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace BearingMachineModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            DelayTimeDistribution = new List<TimeDistribution>();
            BearingLifeDistribution = new List<TimeDistribution>();

            CurrentSimulationTable = new List<CurrentSimulationCase>();
            CurrentPerformanceMeasures = new PerformanceMeasures();

            ProposedSimulationTable = new List<ProposedSimulationCase>();
            ProposedPerformanceMeasures = new PerformanceMeasures();
        }
        
        ///////////// INPUTS /////////////
        public int DowntimeCost { get; set; }
        public int RepairPersonCost { get; set; }
        public int BearingCost { get; set; }
        public int NumberOfHours { get; set; }
        public int NumberOfBearings { get; set; }
        public int RepairTimeForOneBearing { get; set; }
        public int RepairTimeForAllBearings { get; set; }
        public List<TimeDistribution> DelayTimeDistribution { get; set; }
        public List<TimeDistribution> BearingLifeDistribution { get; set; }

        ///////////// OUTPUTS /////////////
        public List<CurrentSimulationCase> CurrentSimulationTable { get; set; }
        public PerformanceMeasures CurrentPerformanceMeasures { get; set; }
        public List<ProposedSimulationCase> ProposedSimulationTable { get; set; }
        public PerformanceMeasures ProposedPerformanceMeasures { get; set; }
       

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
                else if (fineline == "DowntimeCost")
                {
                    DowntimeCost = int.Parse(sr.ReadLine());
                }
                else if (fineline == "RepairPersonCost")
                {
                    RepairPersonCost = int.Parse(sr.ReadLine());
                }
                else if (fineline == "BearingCost")
                {
                    BearingCost = int.Parse(sr.ReadLine());
                }
                else if (fineline == "NumberOfHours")
                {
                    NumberOfHours = int.Parse(sr.ReadLine());
                }
                else if (fineline == "NumberOfBearings")
                {
                    NumberOfBearings = int.Parse(sr.ReadLine());
                }
                
                else if (fineline == "RepairTimeForOneBearing")
                {
                    RepairTimeForOneBearing = int.Parse(sr.ReadLine());
                }
                else if (fineline == "RepairTimeForAllBearings")
                {
                    RepairTimeForAllBearings = int.Parse(sr.ReadLine());
                }
                else if (fineline == "DelayTimeDistribution")
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
                        TimeDistribution x = new TimeDistribution();
                        x.Time = int.Parse(l[0]);
                        int pro = Convert.ToInt32(float.Parse(l[1]) * 100);
                        x.Probability = pro;
                        x.CummProbability = cp + pro;
                        x.MinRange = cp + 1;
                        cp += pro;
                        x.MaxRange = cp;
                        DelayTimeDistribution.Add(x);
                    }

                }
                else if (fineline == "BearingLifeDistribution")
                {
                    int cp = 0;
                    while (true)
                    {

                        string td = sr.ReadLine();
                        if (td == ""|| td ==null)
                        {
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
                        BearingLifeDistribution.Add(x);
                    }


                }

            }
            fs.Close();

        }
        public void simulate()
        {
            Random rnd = new Random();
            List<Bearing> [] my_bears = new List<Bearing> [NumberOfBearings];
            
            for (int i = 0; i <NumberOfBearings;i++)
            {   
                int accum = 0;
                my_bears[i] = new List<Bearing>();
                while (accum < NumberOfHours)
                {
                    CurrentSimulationCase curbear = new CurrentSimulationCase();
                    curbear.Bearing.Index = i+1;
                    curbear.Bearing.RandomHours = rnd.Next(1, 100);
                    curbear.Bearing.Hours = life_dist(curbear.Bearing.RandomHours);

                    accum = accum + curbear.Bearing.Hours;

                    curbear.AccumulatedHours = accum;
                    curbear.RandomDelay = rnd.Next(1, 100);
                    curbear.Delay = delay_dist(curbear.RandomDelay);
                    CurrentPerformanceMeasures.DelayCost += (decimal)curbear.Delay * DowntimeCost;
                    my_bears[i].Add(curbear.Bearing);
                    CurrentSimulationTable.Add(curbear); 
                }

                
            }
            CurrentPerformanceMeasures.BearingCost = (decimal)CurrentSimulationTable.Count * BearingCost;
            CurrentPerformanceMeasures.DowntimeCost =(decimal) CurrentSimulationTable.Count * RepairTimeForOneBearing * DowntimeCost;
            decimal nrcost =(decimal) RepairPersonCost / 60;
            CurrentPerformanceMeasures.RepairPersonCost = (decimal)(CurrentSimulationTable.Count * RepairTimeForOneBearing)*nrcost;
            CurrentPerformanceMeasures.TotalCost = (decimal)CurrentPerformanceMeasures.BearingCost + CurrentPerformanceMeasures.DelayCost 
             + CurrentPerformanceMeasures.DowntimeCost + CurrentPerformanceMeasures.RepairPersonCost;

           sec_step(my_bears);
        }
        public void sec_step(List<Bearing>[] mylist)
        {   
            Random rnd = new Random();
            int acc = 0;
            for (int j = 0; acc < NumberOfHours; j++)
            {
                ProposedSimulationCase prp = new ProposedSimulationCase();
                prp.Bearings = new List<Bearing>();
                int first_fail = NumberOfHours;
                for (int k = 0; k < NumberOfBearings; k++)
                {
                    if (mylist[k].Count > j)
                    {
                        prp.Bearings.Add(mylist[k][j]);
                        if (first_fail>prp.Bearings[k].Hours)
                        {
                            first_fail = prp.Bearings[k].Hours;
                        }
                    }
                    else
                    {
                        Bearing curbear = new Bearing();
                        curbear.Index = k + 1;
                        curbear.RandomHours = rnd.Next(1, 100);
                        curbear.Hours = life_dist(curbear.RandomHours);
                        prp.Bearings.Add(curbear);
                        if (first_fail > prp.Bearings[k].Hours)
                        {
                            first_fail = prp.Bearings[k].Hours;
                        }
                    }

                }
                prp.AccumulatedHours = acc;
                prp.FirstFailure = first_fail;
                acc += first_fail;
                prp.AccumulatedHours = acc;
                prp.RandomDelay = rnd.Next(1, 100);
                prp.Delay = delay_dist(prp.RandomDelay);
                ProposedPerformanceMeasures.DelayCost += (decimal)prp.Delay * DowntimeCost;

                ProposedSimulationTable.Add(prp);
            }

            ProposedPerformanceMeasures.BearingCost = (decimal)ProposedSimulationTable.Count *NumberOfBearings*BearingCost ;
            ProposedPerformanceMeasures.DowntimeCost = (decimal)ProposedSimulationTable.Count * RepairTimeForAllBearings * DowntimeCost;
            decimal nrcost = (decimal)RepairPersonCost / 60;
            ProposedPerformanceMeasures.RepairPersonCost = (decimal)(ProposedSimulationTable.Count * RepairTimeForAllBearings )*nrcost;
            ProposedPerformanceMeasures.TotalCost = (decimal)ProposedPerformanceMeasures.BearingCost + ProposedPerformanceMeasures.DelayCost
            + ProposedPerformanceMeasures.DowntimeCost + ProposedPerformanceMeasures.RepairPersonCost;


        }
       
        public int delay_dist(int rnd)
        {
            for (int i = 0; i < DelayTimeDistribution.Count; i++)
            {
                if (rnd >= DelayTimeDistribution[i].MinRange && rnd <= DelayTimeDistribution[i].MaxRange)
                {
                    return DelayTimeDistribution[i].Time;
                }
            }

            return 0;
        }
        public int life_dist(int rnd)
        {
            for (int i = 0; i < BearingLifeDistribution.Count; i++)
            {
                if (rnd >= BearingLifeDistribution[i].MinRange && rnd <= BearingLifeDistribution[i].MaxRange)
                {
                    return BearingLifeDistribution[i].Time;
                }
            }
            return 0;
        }
    }
}
