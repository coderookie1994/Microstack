using System;

namespace microstack.Daemon.WindowsService
{
    public class ProcessEventArgs : EventArgs
    {
        public int PID { get; set; }
        public ProcessMarkers ProcessMarker { get; set; }
    }
}