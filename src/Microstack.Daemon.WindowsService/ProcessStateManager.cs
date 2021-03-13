using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;

namespace Microstack.Daemon.WindowsService
{
    public class ProcessStateManager : IDisposable
    {
        public List<ProcessState> _states;
        private BlockingCollection<int> _queueToRemove;
        private Timer _timer;
        private object _lockObj = new object();
        public ProcessStateManager()
        {
            _states = new List<ProcessState>();
            _queueToRemove = new BlockingCollection<int>();
            _timer = new Timer(RemoveState, null, 0, 5);
        }

        public void TerminateProcess(object sender, ProcessEventArgs args)
        {
            var processState = _states.FirstOrDefault(s => s.PID.Equals(args.PID));
            if (processState != null)
            {
                // Check WMI Database
                var query = string.Format("SELECT * FROM Win32_Process WHERE ProcessId = {0}", processState.PID);
                var search = new ManagementObjectSearcher("root\\CIMV2", query);
                var results = search.Get().GetEnumerator();
                var next = results.MoveNext();
                if (next)
                {
                    var queryObj = results.Current;
                    var pidInWmi = (uint)queryObj["ProcessId"];
                    Console.WriteLine(pidInWmi);
                    if (pidInWmi != 0)
                    {
                        var process = Process.GetProcessById(processState.PID);
                        if (process != null)
                        {
                            process.Kill(true);
                            Console.WriteLine($"Terminated {pidInWmi}");
                        }
                    }
                }
                
                var processStateIndex = _states.FindIndex(s => s.PID.Equals(args.PID));
                Console.WriteLine($"Marking {args.PID} for removal");

                if (processStateIndex >= 0)
                    _queueToRemove.Add(processStateIndex);                
            }
        }

        public void AddProcess(int pid, int microStackPid)
        {
            var state = new ProcessState(pid, microStackPid, ProcessMarkers.Green);
            state.TerminateProcess += TerminateProcess;
            _states.Add(state);
        }

        private void RemoveState(object state)
        {
            lock(_lockObj)
            {
                if (!_queueToRemove.IsAddingCompleted && _queueToRemove.Count > 0)
                {
                    var index = _queueToRemove.Take();
                    if (index < _states.Count)
                    {
                        _states.RemoveAt(index);
                    }
                }
            }
            
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}