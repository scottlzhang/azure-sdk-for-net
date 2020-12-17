// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using DigitalTwins.Tests.Helpers;
using Microsoft.Azure.Management.DigitalTwins;
using Microsoft.Azure.Management.DigitalTwins.Models;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.ClientRuntime.Azure.TestFramework;
using System;
using System.Net;

namespace DigitalTwins.Tests.ScenarioTests
{
    public class DigitalTwinsTestBase
    {
        private readonly object _initializeLock = new object();

        protected bool IsInitialized { get; private set; } = false;
        protected ResourceManagementClient ResourcesClient { get; private set; }
        protected AzureDigitalTwinsManagementClient DigitalTwinsClient { get; private set; }
        protected string Location { get; private set; }
        protected TestEnvironment TestEnv { get; private set; }

        protected void Initialize(MockContext context)
        {
            if (IsInitialized)
            {
                return;
            }

            lock (_initializeLock)
            {
                if (IsInitialized)
                {
                    return;
                }

                TestEnv = TestEnvironmentFactory.GetTestEnvironment();
                
                ResourcesClient = DigitalTwinsTestUtilities.GetResourceManagementClient(
                    context,
                    new RecordedDelegatingHandler { StatusCodeToReturn = HttpStatusCode.OK });

                DigitalTwinsClient = DigitalTwinsTestUtilities.GetDigitalTwinsClient(
                    context,
                    new RecordedDelegatingHandler { StatusCodeToReturn = HttpStatusCode.OK });

                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AZURE_LOCATION")))
                {
                    Location = DigitalTwinsTestUtilities.DefaultLocation;
                }
                else
                {
                    Location = Environment.GetEnvironmentVariable("AZURE_LOCATION").Replace(" ", "").ToLower();
                }

                IsInitialized = true;
            }
        }

        protected DigitalTwinsDescription CreateDigitalTwinsInstance(ResourceGroup resourceGroup, string location, string digitalTwinsInstanceName)
        {
            var digitalTwinsDescription = new DigitalTwinsDescription
            {
                Location = location,
            };

            return DigitalTwinsClient.DigitalTwins.CreateOrUpdate(
                resourceGroup.Name,
                digitalTwinsInstanceName,
                digitalTwinsDescription);
        }

        protected DigitalTwinsDescription UpdateDigitalTwinsInstance(ResourceGroup resourceGroup, DigitalTwinsDescription digitalTwinsDescription, string digitalTwinsInstanceName)
        {
            return DigitalTwinsClient.DigitalTwins.CreateOrUpdate(
               resourceGroup.Name,
               digitalTwinsInstanceName,
               digitalTwinsDescription);
        }

        protected ResourceGroup CreateResourceGroup(string resourceGroupName)
        {
            return ResourcesClient.ResourceGroups.CreateOrUpdate(
                resourceGroupName,
                new ResourceGroup
                {
                    Location = DigitalTwinsTestUtilities.DefaultLocation
                });
        }

        protected void DeleteResourceGroup(string resourceGroupName)
        {
            ResourcesClient.ResourceGroups.Delete(resourceGroupName);
        }
    }
}
