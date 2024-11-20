
# **Crawler App**

The **Crawler App** is a high-performance web crawling application designed to scrape, process, and extract data from websites efficiently. It supports multi-threading, logging, and metrics visualization using Prometheus and Grafana.

---

## **Features**
- **Parallel Processing**: Supports multiple threads to optimize crawling speed.
- **Metrics Integration**: Exposes Prometheus-compatible metrics for monitoring performance.
- **Dockerized Deployment**: Easily deployable using Docker and Docker Compose.
- **Customizable**: Flexible configuration options for URLs, concurrency, and more.
- **Error Handling**: Robust handling of HTTP errors and retries.

---

## **Getting Started**

### Prerequisites
1. **.NET SDK**: Ensure you have .NET 6 or later installed.
2. **Docker**: Required for containerized deployment.
3. **Prometheus and Grafana**: For metrics and visualization.

### Installation

2. Install dependencies:
   ```bash
   dotnet restore
   ```

3. Build the application:
   ```bash
   dotnet build
   ```

---


## **Usage**

### Run Locally
1. Start the application:
   ```bash
   dotnet run
   ```

2. Access the metrics at:
   ```
   http://localhost:8080/metrics
   ```

3. View logs in the console output.

### Docker Deployment
1. Build the Docker image:
   ```bash
   docker build -t crawler-app .
   ```

2. Run the container:
   ```bash
   docker run -d -p 8080:8080 crawler-app
   ```

3. Verify the app:
   - Metrics: [http://localhost:8080/metrics](http://localhost:8080/metrics)

### Docker Compose
To deploy with Prometheus and Grafana, use the included `docker-compose.yml` file:
```bash
docker-compose up -d
```
This starts the crawler app alongside Prometheus and Grafana.

---

## **Monitoring and Visualization**
1. Access the Prometheus UI:
   ```
   http://localhost:9090
   ```

2. Access the Grafana UI:
   ```
   http://localhost:3000
   ```
   - Default credentials: `admin/admin`
   - Add Prometheus as a data source and import the provided Grafana dashboard JSON.

---

