namespace EMG.Extensions.Configuration.Model
{
    public class PortMapping
    {
        public int ContainerPort { get; set; }
        
        public int HostPort { get; set; }
        
        public string BindIp { get; set; }
        
        public string Protocol { get; set; }
    }
}