using ProtoBuf;

namespace Microstack.CLI.BackgroundTasks.Models
{
    [ProtoContract]
    public class ProcessContract
    {
        [ProtoMember(1)]
        public int MicroStackPID { get; set; }
        
        [ProtoMember(2)]
        public int ProcessId { get; set; }
    }
}