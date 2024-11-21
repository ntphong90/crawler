using crawler.Controllers.webvitals;
using Microsoft.AspNetCore.Mvc;

namespace crawler.Controllers
{
    public class WebVitalsController : Controller
    {
        [HttpPost]
        [Route("/api/webvital")]
        public IActionResult PostMetric([FromBody] MetricData metric)
        {
            // Log or process the metric
            Console.WriteLine($"Received Metric: {metric.Name}, Value: {metric.Value}");

            // Optionally, store metrics in memory or a database
            MetricsStore.AddMetric(metric);

            return Ok("Metric received");
        }
    }
}
