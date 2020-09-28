using ProtoBuf;

namespace microstack.Daemon.WindowsService
{
    [ProtoContract]
    public class ProcessContract
    {
        [ProtoMember(1)]
        public int ProcessId { get; set; }
    }
}