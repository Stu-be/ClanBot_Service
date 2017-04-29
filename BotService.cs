using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace ClanBot_Service
{
    public partial class BotService : ServiceBase
    {
        Thread th;
        bool isRunning = false;
        private ClasherDynBot bot { get; set; } 

        public BotService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                bot = new ClasherDynBot();
                th = new Thread(DoThis);
                th.Start();
                isRunning = true;          
            }
            catch (Exception ex){}
        }

        public void DoThis()
        {
            while (isRunning)
            {
                
            }
        }

        protected override void OnStop()
        {
            isRunning = false;
        }
    }
}
