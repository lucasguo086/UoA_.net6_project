using Microsoft.AspNetCore.Authentication;
using A2.Handler;
using A2.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//authentication 给不同的claim
builder.Services
    .AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, MyAuthHandler>("MyAuthentication", null);
    //.AddScheme<AuthenticationSchemeOptions, AdminHandler>("AdminAuthentication", null);
builder.Services.AddDbContext<A2DBContext>(
    options => options.UseSqlite(builder.Configuration["A2DbConnection"])
);
builder.Services.AddControllers();
builder.Services.AddScoped<IA2Repo, A2Repo>();
builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("AdminOnly", policy => policy.RequireClaim("admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireClaim("user"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
