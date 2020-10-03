using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace microstack.BackgroundTasks
{
    public class ProcessSpawnManager : IDisposable
    {
        private BlockingCollection<(string Name, ProcessStartInfo ProcessObject)> _spawnQueue;
        private BlockingCollection<string> _killQueue;
        public ProcessSpawnManager()
        {
            _spawnQueue = new BlockingCollection<(string Name, ProcessStartInfo ProcessObject)>();
            _killQueue = new BlockingCollection<string>();
        }

        public void QueueToSpawn(IList<(string Name, ProcessStartInfo ProcessObjects)> processObjects)
        {
            // Validate arguments in event
            if (processObjects == null || processObjects.Count == 0)
                throw new ArgumentNullException("ProcessObjects is null or empty");
            
            foreach(var procObj in processObjects)
            {
                _spawnQueue.Add(procObj);
            }
        }
        
        internal void SigKill(IEnumerable<string> enumerable)
        {
            foreach(var projName in enumerable)
                _killQueue.Add(projName);
        }

        internal string DequeueKillRequests()
        {
            if (_killQueue.Count > 0)
                return _killQueue.Take();
            return null;
        }

        public (string Name, ProcessStartInfo ProcessInfoObject) Dequeue()
        {
            var processObjectTuple = new List<(string Name, ProcessStartInfo ProcessObject)>();
            if (!_spawnQueue.IsCompleted && _spawnQueue.Count > 0 && !_killQueue.IsCompleted && _killQueue.Count == 0)
                return _spawnQueue.Take();
                
            return (null, null);
        }

        public void Dispose()
        {
            _spawnQueue.CompleteAdding();
            _killQueue.CompleteAdding();
        }
    }
}