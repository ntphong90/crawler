Crawler App
The Crawler App is a high-performance web crawling application designed to scrape, process, and extract data from websites efficiently. It supports multi-threading, logging, and metrics visualization using Prometheus and Grafana.

Features
Parallel Processing: Supports multiple threads to optimize crawling speed.
Metrics Integration: Exposes Prometheus-compatible metrics for monitoring performance.
Dockerized Deployment: Easily deployable using Docker and Docker Compose.
Customizable: Flexible configuration options for URLs, concurrency, and more.
Error Handling: Robust handling of HTTP errors and retries.
Getting Started
Prerequisites
.NET SDK: Ensure you have .NET 6 or later installed.
Docker: Required for containerized deployment.
Prometheus and Grafana: For metrics and visualization.
Installation

dotnet restore
Build the application:
dotnet build

Configuration


Usage
Run Locally
Start the application:
dotnet run

Access the metrics at:
http://localhost:8080/metrics
View logs in the console output.

Docker Deployment
Build the Docker image:

docker build -t crawler-app .
Run the container:

docker run -d -p 8080:8080 crawler-app
Verify the app:

Metrics: http://localhost:8080/metrics
Docker Compose
To deploy with Prometheus and Grafana, use the included docker-compose.yml file:

docker-compose up -d
This starts the crawler app alongside Prometheus and Grafana.

Monitoring and Visualization
Access the Prometheus UI:

http://localhost:9090
Access the Grafana UI:

http://localhost:3000
Default credentials: admin/admin
Add Prometheus as a data source and import the provided Grafana dashboard JSON.
Development
Debugging
Run the application with Visual Studio or the .NET CLI in debug mode:

dotnet run --environment Development
Adding Custom Metrics
Use the prometheus-net library to define and expose custom metrics in Program.cs.


