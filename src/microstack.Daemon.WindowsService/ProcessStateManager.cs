using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace microstack.Daemon.WindowsService
{
    public class ProcessStateManager
    {
        public IList<ProcessState> _states;
        public ProcessStateManager()
        {
            _states = new List<ProcessState>();
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
                            process.Kill(true);
                    }
                }
                _states.Remove(processState);
            }
        }

        public void AddProcess(int pid, int microStackPid)
        {
            var state = new ProcessState(pid, microStackPid, ProcessMarkers.Green);
            state.TerminateProcess += TerminateProcess;
            _states.Add(state);
        }
    }
}