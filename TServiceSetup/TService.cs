using System.ComponentModel;
using System.ServiceProcess;
using TwitterManager;
using Logger;
using System.Timers;
using Timer = System.Timers.Timer;

namespace TServiceSetup
{
    partial class TService : ServiceBase
    {
        //Initialize the timer
        readonly Timer timer = new Timer();

        readonly EventLogWriter logWriter = new EventLogWriter("TServiceSetup");
        
        public TService()
        {
            ServiceName = "TServiceNew";
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //handle Elapsed event
            timer.Elapsed += OnElapsedTime;

            //This statement is used to set interval to 1 minute (= 60,000 milliseconds)
            timer.Interval = 900000;//20 min

            //enabling the timer
            timer.Enabled = true;

            Read();

        }

        protected override void OnStop()
        {
            logWriter.WriteToEventLog("OnStop");
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            Read();
        }

        private void Read()
        {
            BackgroundWorker bw = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = false };
            bw.DoWork += (delegate(object o, DoWorkEventArgs args) { new Reader().Read(); });
            bw.RunWorkerAsync();           
        }
    }
}
