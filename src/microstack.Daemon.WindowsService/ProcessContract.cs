using ProtoBuf;

namespace microstack.Daemon.WindowsService
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