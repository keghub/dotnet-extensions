using Moq;
using NUnit.Framework;
using System;
using EMG.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Tests
{
    public class ECSMetadataExtensionsTests
    {
        [Test, CustomAutoData]
        public void No_config_source_is_added_if_fileKey_not_found(IConfigurationBuilder builder)
        {
            Environment.SetEnvironmentVariable(ECSMetadataExtensions.ECSContainerMetadataFileKey, string.Empty);

            builder.AddECSMetadataFile();

            Mock.Get(builder).Verify(p => p.Add(It.IsAny<IConfigurationSource>()), Times.Never());
        }

        [Test, CustomAutoData]
        public void Json_config_source_is_added_if_fileKey_is_found(IConfigurationBuilder builder, string fileName)
        {
            Environment.SetEnvironmentVariable(ECSMetadataExtensions.ECSContainerMetadataFileKey, fileName);

            builder.AddECSMetadataFile();

            Mock.Get(builder).Verify(p => p.Add(It.Is<IConfigurationSource>(cs => ((JsonConfigurationSource) cs).Path == fileName)), Times.Once());

            Environment.SetEnvironmentVariable(ECSMetadataExtensions.ECSContainerMetadataFileKey, string.Empty);
        }
    }
}