using System;
using Microsoft.Extensions.Configuration;

namespace EMG.Extensions.Configuration
{
    public static class ECSMetadataExtensions
    {
        public const string ECSContainerMetadataFileKey = "ECS_CONTAINER_METADATA_FILE";

        public static IConfigurationBuilder AddECSMetadataFile(this IConfigurationBuilder builder)
        {
            var metadataFilePath = Environment.GetEnvironmentVariable(ECSContainerMetadataFileKey);

            if (metadataFilePath != null)
            {
                builder.AddJsonFile(metadataFilePath, false);
            }

            return builder;
        }
    }
}