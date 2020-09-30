using System;
using System.Diagnostics;
using System.Management;
using System.Threading;
// using Timer = System.Timers.Timer;

namespace microstack.Daemon.WindowsService
{
    public class ProcessState
    {
        private Timer _timer;
        private CancellationTokenSource _cts;
        private CancellationToken _cancellationToken;

        public int MicroStackPID { get; set; }
        public int PID { get; set; }
        public ProcessMarkers ProcessMarker { get; set; }
        public int Ticks { get; private set; }
        private object _lock = new object();
        public ProcessState(int pid, int microStackPid, ProcessMarkers marker)
        {
            PID = pid;
            MicroStackPID = microStackPid;
            ProcessMarker = marker;

            _timer = new Timer(CheckHealth, null, 0, 5);
            _cts = new CancellationTokenSource();
            _cancellationToken = _cts.Token;
            
            Ticks = 0;
        }

        private void CheckHealth(object state)
        {
            // Check if Microstack is alive
            // Check WMI Database
            lock(_lock)
            {
                if (!_cts.IsCancellationRequested)
                {
                    var query = string.Format("SELECT * FROM Win32_Process WHERE ProcessId = {0}", MicroStackPID);
                    var search = new ManagementObjectSearcher("root\\CIMV2", query);
                    var results = search.Get().GetEnumerator();
                    var next = results.MoveNext();
                    // var queryObj = results.Current;
                    // var pidInWmi = (uint)queryObj["ProcessId"];
                    Ticks++;

                    if (!next)
                    {
                        // Raise event to terminate associated PID
                        ProcessMarker = ProcessMarkers.Black;
                        OnProcessTerminationRequest(new ProcessEventArgs(){ PID = PID, ProcessMarker = ProcessMarker });
                        _cts.Cancel();
                        _timer.Dispose();
                    }
                }
            }
            
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