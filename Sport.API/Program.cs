using Microsoft.EntityFrameworkCore;
using Sport.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SportDbContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
   {
      options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
      options.RoutePrefix = string.Empty;
   });
}

app.UseHttpsRedirection();

#region Config. CORS

app.UseCors(options => options
   .AllowAnyOrigin()
   .AllowAnyMethod()
   .AllowAnyHeader());

#endregion

app.MapControllers();

app.Run();