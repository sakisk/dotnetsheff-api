using System;

namespace dotnetsheff.Api.FunctionalTests
{
    public class Event
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime Time { get; set; }

        public int YesRsvpCount { get; set; }

        public string Link { get; set; }

        public string ShortDescription { get; set; }
    }
}