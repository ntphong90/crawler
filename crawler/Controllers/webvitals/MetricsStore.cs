using Prometheus;

namespace crawler.Controllers.webvitals
{
    public class MetricsStore
    {
        private static readonly Gauge WebVitalMetrics = Metrics.CreateGauge(
        "web_vital_metric",
        "Tracks Web Vitals metrics",
        new GaugeConfiguration
        {
            LabelNames = new[] { "name" }
        });

        public static void AddMetric(MetricData metric)
        {
            WebVitalMetrics.WithLabels(metric.Name).Set(metric.Value);
        }
    }
}
