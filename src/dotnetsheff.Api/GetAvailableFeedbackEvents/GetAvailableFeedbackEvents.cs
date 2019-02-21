using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace dotnetsheff.Api.GetAvailableFeedbackEvents
{
    public class GetAvailableFeedbackEvents
    {
        private const int EVENT_COUNT = 3;

        [FunctionName("GetAvailableFeedbackEvents")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "feedback/available-events")] HttpRequestMessage req,
            TraceWriter log)
        {
            log.Info("Getting feedback for available event");

            var lastEvents = await Container.Instance.Resolve<ILastEventsQuery>(log).Execute(EVENT_COUNT);

            var talkParsers = Container.Instance.Resolve<IEnumerable<ITalkParser>>(log).ToArray();

            var eventTalks = new List<EventTalks>();

            foreach (var lastEvent in lastEvents)
            {
                foreach (var parser in talkParsers)
                {
                    var talks = parser.Parse(lastEvent).ToArray();
                    if (talks.Any())
                    {
                        eventTalks.Add(new EventTalks
                        {
                            Title = lastEvent.Name,
                            Talks = talks
                        });
                        break;
                    }

                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(eventTalks), Encoding.UTF8, "application/json")
            };
        }

        private class EventTalks
        {
            public string Title { get; set; }
            public Talk[] Talks { get; set; }
        }
    }
}