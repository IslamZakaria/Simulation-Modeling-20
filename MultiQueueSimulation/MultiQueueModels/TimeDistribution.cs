using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace MultiQueueModels
{
    public class TimeDistribution
    {
        public int Time { get; set; }
        public decimal Probability { get; set; }
        public decimal CummProbability { get; set; }
        public int MinRange { get; set; }
        public int MaxRange { get; set; }

        public List<TimeDistribution> gettable()
        {

            List<TimeDistribution> inter = new List<TimeDistribution>();
            FileStream fs = new FileStream("C:/Users/Eslam Yehia/Desktop/interarrival.txt",FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            while (sr.Peek()!=-1)
            {
                string[] l =  sr.ReadLine().Split(',');
                TimeDistribution x = new TimeDistribution();
                x.Time =  int.Parse (l[0]) ;
                x.Probability = int.Parse( l[1]);
                x.CummProbability = int.Parse (l[2]);
                x.MinRange = int.Parse(l[3]);
                x.MaxRange = int.Parse (l[4]);
                inter.Add(x);
            }
            fs.Close();
            return inter;
        }
        public List<TimeDistribution> gettable_able()
        {

            List<TimeDistribution> inter = new List<TimeDistribution>();
            FileStream fs = new FileStream("C:/Users/Eslam Yehia/Desktop/able.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            while (sr.Peek() != -1)
            {
                string[] l = sr.ReadLine().Split(',');
                TimeDistribution x = new TimeDistribution();
                x.Time = int.Parse(l[0]);
                x.Probability = int.Parse(l[1]);
                x.CummProbability = int.Parse(l[2]);
                x.MinRange = int.Parse(l[3]);
                x.MaxRange = int.Parse(l[4]);
                inter.Add(x);
            }
            fs.Close();
            return inter;
        }
        public List<TimeDistribution> gettable_baker()
        {

            List<TimeDistribution> inter = new List<TimeDistribution>();
            FileStream fs = new FileStream("C:/Users/Eslam Yehia/Desktop/baker.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            while (sr.Peek() != -1)
            {
                string[] l = sr.ReadLine().Split(',');
                TimeDistribution x = new TimeDistribution();
                x.Time = int.Parse(l[0]);
                x.Probability = int.Parse(l[1]);
                x.CummProbability = int.Parse(l[2]);
                x.MinRange = int.Parse(l[3]);
                x.MaxRange = int.Parse(l[4]);
                inter.Add(x);
            }
            fs.Close();
            return inter;
        }
    }
}
