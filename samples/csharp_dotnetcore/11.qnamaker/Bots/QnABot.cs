﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class QnABot : ActivityHandler
    {
        private IConfiguration _configuration;
        private ILogger<QnABot> _logger;

        public QnABot(IConfiguration configuration, ILogger<QnABot> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var qnaMaker = new QnAMaker(new QnAMakerEndpoint
            {
                KnowledgeBaseId = _configuration["QnA-sample-qna-kbId"],
                EndpointKey = _configuration["QnA-sample-qna-endpointKey"],
                Host = _configuration["QnA-sample-qna-hostname"]
            });

            _logger.LogInformation("Calling QnA Maker");

            // The actual call to the QnA Maker service.
            var response = await qnaMaker.GetAnswersAsync(turnContext);
            if (response != null && response.Length > 0)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(response[0].Answer), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("No QnA Maker answers were found."), cancellationToken);
            }
        }
    }
}
