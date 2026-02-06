using ERP.Infrastructure;
using ERP.Services.User;
using ERP.Model;

var MyAllowSpecificOrigins = "MyAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(o => o.AddPolicy(MyAllowSpecificOrigins, policy => {
policy.WithOrigins("http://localhost:4200", "http://teammate.pl",
                    "https://teammate.pl")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
}));

builder.Services.AddModel(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddUserApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();
