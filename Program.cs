using NLog;
using NLog.Config;

LogManager.Configuration = new XmlLoggingConfiguration(@"NLog.config");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(
//    builder.Configuration.GetConnectionString("DefaultConnection")
//    ));

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//перехватывает исключения и выводит информацию о них разработчику.
app.UseDeveloperExceptionPage();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseFileServer();


app.UseRouting();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
