using AsyncDataServices;
using CommandsService.Data;
using EventProcessing;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
    cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODA1MDY4ODAwIiwiaWF0IjoiMTc3MzU4MTAwOSIsImFjY291bnRfaWQiOiIwMTljZjFhYTBmMTY3MDQ3YWM1YzYxOGI0ZDFiMDQ5NiIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2tydG4wY3p0dmtodHI5eTg4anN0a3I1Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.AMu108nYfpL0tYnrll4PfNo1aNRJUZts_y_E9sJ0rNEh4dNPRbKAb0kiJtMEOIhBPovqWyY-ZrMpqq8SciywpkedLXP1rMNYWPqlSxmy_ftHQUS33YfNj0d-dLPqVPxKnHjyLfvD84Y2eNuw1eYZDlwDSV7ZyMRu-DY4LB2I7IEG6EYH9R1oWXFYfYd6QeV_UL4AOQWgl_NKFIHaj4Qv4yIlJ6brIKBYrEB-QIa-bo0xV_Igs2f0wVTmAr0g_wCC2lntyXF95F3nZN-RHxV5kyShp-1yirmkdCQI8LgCi5Lbuii_j7skvph4Xx0MOsregD4G_pb73vWNLq7RKbCxBw";
}

);
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMen"));
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();