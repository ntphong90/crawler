namespace crawler.Controllers.webvitals
{
    public class MetricData
    {
        public string Name { get; set; } // Metric name (e.g., LCP, INP)
        public double Value { get; set; } // Metric value (e.g., milliseconds)
        public string Id { get; set; } // Unique metric ID
        public string Timestamp { get; set; } // Timestamp
    }
}
