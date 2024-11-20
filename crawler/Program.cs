using crawler.Controllers.crawler;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
// Register the crawlers
builder.Services.AddSingleton<IHtmlCrawler, VNExpressCrawler>();
builder.Services.AddSingleton<IHtmlCrawler, TuoiTreCrawler>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting(); // Ensure routing is added before MapMetrics.
app.UseHttpMetrics(); // Middleware to track HTTP request metrics.
app.UseEndpoints(endpoints =>
{
    // Add Prometheus scraping endpoint
    endpoints.MapMetrics(); // Exposes /metrics endpoint for Prometheus.

    // Other endpoints
    endpoints.MapControllers(); // If using controllers
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
