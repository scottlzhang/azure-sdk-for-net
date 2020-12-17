// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DigitalTwins.Tests.Helpers;
using Microsoft.Azure.Management.DigitalTwins;
using Microsoft.Azure.Management.DigitalTwins.Models;
using Microsoft.Rest.ClientRuntime.Azure.TestFramework;
using Xunit;
using FluentAssertions;

namespace DigitalTwins.Tests.ScenarioTests
{
    public class DigitalTwinsLifecycleTests : DigitalTwinsTestBase
    {
        [Fact]
        public void TestDigitalTwinsLifecycle()
        {
            using var context = MockContext.Start(GetType());

            var digitalTwinsDescription = new DigitalTwinsDescription
            {
                Location = Location,
            };

            Initialize(context);

            // Create Resource Group
            var resourceGroup = CreateResourceGroup(DigitalTwinsTestUtilities.DefaultResourceGroupName);

            // Check if instance exists and delete
            var digitalTwinsAvailability = DigitalTwinsClient.DigitalTwins.CheckNameAvailability(
                DigitalTwinsTestUtilities.DefaultLocation,
                DigitalTwinsTestUtilities.DefaultInstanceName);

            if (!(bool)digitalTwinsAvailability.NameAvailable)
            {
                DigitalTwinsClient.DigitalTwins.Delete(
                    DigitalTwinsTestUtilities.DefaultResourceGroupName,
                    DigitalTwinsTestUtilities.DefaultInstanceName);

                digitalTwinsAvailability = DigitalTwinsClient.DigitalTwins.CheckNameAvailability(
                    DigitalTwinsTestUtilities.DefaultLocation,
                    DigitalTwinsTestUtilities.DefaultInstanceName);
                Assert.True(digitalTwinsAvailability.NameAvailable);
            }

            // Create DigitalTwins resource
            var digitalTwinsInstance = CreateDigitalTwinsInstance(
                resourceGroup,
                DigitalTwinsTestUtilities.DefaultLocation,
                DigitalTwinsTestUtilities.DefaultInstanceName);

            digitalTwinsInstance.Should().NotBeNull();
            digitalTwinsInstance.Name.Should().Be(DigitalTwinsTestUtilities.DefaultInstanceName);
            digitalTwinsInstance.Location.Should().Be(DigitalTwinsTestUtilities.DefaultLocation);

            // Add and Get Tags
            const string key2 = "key2";
            const string value2 = "value2";
            var patch = new DigitalTwinsPatchDescription(
                tags: new Dictionary<string, string>
                {
                    { "key1", "value1" },
                    { key2, value2 },
                });
            digitalTwinsInstance = DigitalTwinsClient.DigitalTwins.Update(
                DigitalTwinsTestUtilities.DefaultResourceGroupName,
                DigitalTwinsTestUtilities.DefaultInstanceName,
                patch);

            Assert.NotNull(digitalTwinsInstance);
            digitalTwinsInstance.Tags.Count().Should().Be(2);
            digitalTwinsInstance.Tags[key2].Should().Be(value2);

            // List DigitalTwins instances in Resource Group
            var twinsResources = DigitalTwinsClient.DigitalTwins.ListByResourceGroup(DigitalTwinsTestUtilities.DefaultResourceGroupName);
            twinsResources.Count().Should().BeGreaterThan(0);

            // Get all of the available operations, ensure CRUD
            var operationList = DigitalTwinsClient.Operations.List();
            operationList.Count().Should().BeGreaterThan(0);
            Assert.Contains(operationList, e => e.Name.Equals($"Microsoft.DigitalTwins/digitalTwinsInstances/read", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(operationList, e => e.Name.Equals($"Microsoft.DigitalTwins/digitalTwinsInstances/write", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(operationList, e => e.Name.Equals($"Microsoft.DigitalTwins/digitalTwinsInstances/delete", StringComparison.OrdinalIgnoreCase));

            // Get other operations

            // Register Operation
            var registerOperations = operationList.Where(e => e.Name.Contains($"Microsoft.DigitalTwins/register"));
            registerOperations.Count().Should().BeGreaterThan(0);

            // Twin Operations
            var twinOperations = operationList.Where(e => e.Name.Contains($"Microsoft.DigitalTwins/digitaltwins"));
            twinOperations.Count().Should().BeGreaterThan(0);

            // Event Route Operations
            var eventRouteOperations = operationList.Where(e => e.Name.Contains($"Microsoft.DigitalTwins/eventroutes"));
            eventRouteOperations.Count().Should().BeGreaterThan(0);

            // Model operations
            var modelOperations = operationList.Where(e => e.Name.Contains($"Microsoft.DigitalTwins/models"));
            modelOperations.Count().Should().BeGreaterThan(0);

            // Delete instance
            var deleteOp = DigitalTwinsClient.DigitalTwins.BeginDelete(
                DigitalTwinsTestUtilities.DefaultResourceGroupName,
                DigitalTwinsTestUtilities.DefaultInstanceName);
            deleteOp.ProvisioningState.Should().Be(ProvisioningState.Deleting);
        }
    }
}
