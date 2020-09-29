using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
                Process.GetProcessById(processState.PID)?.Kill(true);
                _states.Remove(processState);
            }
        }

        public void AddProcess(int pid, int microStackPid)
        {
            var state = new ProcessState()
            {
                PID = pid,
                MicroStackPID = microStackPid,
                ProcessMarker = ProcessMarkers.Green,
            };
            state.TerminateProcess += TerminateProcess;
            _states.Add(state);
        }
    }
}