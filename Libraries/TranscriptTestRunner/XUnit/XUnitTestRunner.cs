﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;

namespace TranscriptTestRunner.XUnit
{
    /// <summary>
    /// XUnit extension of <see cref="TestRunner"/>.
    /// </summary>
    public class XUnitTestRunner : TestRunner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XUnitTestRunner"/> class.
        /// </summary>
        /// <param name="client">Test client to use.</param>
        /// <param name="logger">Optional. Instance of <see cref="ILogger"/> to use.</param>
        public XUnitTestRunner(TestClientBase client, ILogger logger = null)
            : base(client, logger)
        {
        }

        /// <summary>
        /// Validates an <see cref="Activity"/> according to an expected activity <see cref="TestScriptItem"/> using XUnit.
        /// </summary>
        /// <param name="expectedActivity">The expected activity of type <see cref="TestScriptItem"/>.</param>
        /// <param name="actualActivity">The actual response <see cref="Activity"/> recieved.</param>
        /// <param name="cancellationToken">Optional. A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        protected override Task AssertActivityAsync(TestScriptItem expectedActivity, Activity actualActivity, CancellationToken cancellationToken = default)
        {
            foreach (var assertion in expectedActivity.Assertions)
            {
                var (result, error) = Expression.Parse(assertion).TryEvaluate<bool>(actualActivity);

                Assert.True(result, $"The bot's response was different than expected. The assertion: \"{assertion}\" was evaluated as false.\nExpected Activity:\n{JsonConvert.SerializeObject(expectedActivity, Formatting.Indented)}\nActual Activity:\n{JsonConvert.SerializeObject(actualActivity, Formatting.Indented)}");
            }

            return Task.CompletedTask;
        }
    }
}