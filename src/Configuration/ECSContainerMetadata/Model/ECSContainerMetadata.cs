namespace EMG.Extensions.Configuration.Model
{
    public class ECSContainerMetadata
    {
        public string Cluster { get; set; }
        
        public string ContainerInstanceARN { get; set; }
        
        public string TaskARN { get; set; }
        
        public string TaskDefinitionFamily { get; set; }
        
        public string TaskDefinitionRevision { get; set; }
        
        public string ContainerID { get; set; }
        
        public string ContainerName { get; set; }
        
        public string DockerContainerName { get; set; }
        
        public string ImageID { get; set; }
        
        public string ImageName { get; set; }
        
        public PortMapping[] PortMappings { get; set; }
        
        public Network[] Networks { get; set; }
        
        public string MetadataFileStatus { get; set; }
        
        public string AvailabilityZone { get; set; }
        
        public string HostPrivateIPv4Address { get; set; }
    }
}