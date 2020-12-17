// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.DigitalTwins;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Rest.ClientRuntime.Azure.TestFramework;
using System.Net;

namespace DigitalTwins.Tests.Helpers
{
    internal class DigitalTwinsTestUtilities
    {
        public static string DefaultLocation = "westus2";
        public static string DefaultInstanceName = "DigitalTwinsSDK";
        public static string DefaultEndpointName = "DigitalTwinsSDKEndpoint";
        public static string DefaultResourceGroupName = "DigitalTwinsSDKResourceGroup";

        public static AzureDigitalTwinsManagementClient GetDigitalTwinsClient(MockContext context, RecordedDelegatingHandler handler = null)
        {
            if (handler != null)
            {
                handler.IsPassthrough = true;
            }
 
            return context.GetServiceClient<AzureDigitalTwinsManagementClient>(
                handlers: handler ?? new RecordedDelegatingHandler { StatusCodeToReturn = HttpStatusCode.OK });
        }

        public static ResourceManagementClient GetResourceManagementClient(MockContext context, RecordedDelegatingHandler handler)
        {
            handler.IsPassthrough = true;
            var client = context.GetServiceClient<ResourceManagementClient>(handlers: handler);
            return client;
        }
    }
}
