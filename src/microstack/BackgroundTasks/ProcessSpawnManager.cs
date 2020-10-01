using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace microstack.BackgroundTasks
{
    public class ProcessSpawnManager : IDisposable
    {
        private BlockingCollection<(string Name, ProcessStartInfo ProcessObject)> _bc;

        public ProcessSpawnManager()
        {
            _bc = new BlockingCollection<(string Name, ProcessStartInfo ProcessObject)>();
        }

        public void QueueToSpawn(IList<(string Name, ProcessStartInfo ProcessObjects)> processObjects)
        {
            // Validate arguments in event
            if (processObjects == null || processObjects.Count == 0)
                throw new ArgumentNullException("ProcessObjects is null or empty");
            
            foreach(var procObj in processObjects)
            {
                _bc.Add(procObj);
            }
        }

        public (string Name, ProcessStartInfo ProcessInfoObject) Dequeue()
        {
            var processObjectTuple = new List<(string Name, ProcessStartInfo ProcessObject)>();
            if (_bc?.Count > 0)
                return _bc.Take();
                
            return (null, null);
        }

        public void StopProcessing()
        {
            _bc.CompleteAdding();
        }

        public void Dispose()
        {
            _bc.Dispose();
        }
    }
}