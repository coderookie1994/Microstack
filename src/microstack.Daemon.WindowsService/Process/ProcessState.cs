using System;
using System.Diagnostics;
using System.Threading;

namespace microstack.Daemon.WindowsService
{
    public class ProcessState
    {
        private Timer _timer;
        public int MicroStackPID { get; set; }
        public int PID { get; set; }
        public ProcessMarkers ProcessMarker { get; set; }
        public int Ticks { get; private set; }

        public ProcessState()
        {
            _timer = new Timer(CheckHealth, null, 0, 5);
            Ticks = 0;
        }

        private void CheckHealth(object state)
        {
            // Check if Microstack is alive
            var process = Process.GetProcessById(MicroStackPID);
            if (process == null)
            {
                // Raise event to terminate associated PID
                ProcessMarker = ProcessMarkers.Black;
                OnProcessTerminationRequest(new ProcessEventArgs(){ PID = PID, ProcessMarker = ProcessMarker });
            }
            Ticks++;
        }

        protected void OnProcessTerminationRequest(ProcessEventArgs args)
        {
            EventHandler<ProcessEventArgs> handler = TerminateProcess;
            if (handler != null)
            {
                handler(null, args);
            }
        }

        public event EventHandler<ProcessEventArgs> TerminateProcess;
    }
}