using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NuLibrary.Migration.Mappings
{
    public class SqlTableStats
    {
        Timer timer = new Timer(1000);
        TimeSpan elapsed = new TimeSpan();

        public SqlTableStats()
        {
            timer.Elapsed += OnTimerElapsed;
            GC.KeepAlive(timer);
        }

        public string Tablename { get; set; }
        public int PreSaveCount { get; set; }
        public int PostSaveCount { get; set; }
        public TimeSpan TotalSaveTime { get { return elapsed; } }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            elapsed = e.SignalTime.TimeOfDay;
        }

        public void StartTimer()
        {
            timer.Start();
        }

        public void StopTimer()
        {
            timer.Stop();
        }

        public override string ToString()
        {
            return $"{Tablename} Presave={PreSaveCount} - Postsave={PostSaveCount} Save Time={TotalSaveTime}";
        }
    }
}
