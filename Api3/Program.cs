var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(options =>
    {
        options.InputFormatters.Clear();
        options.OutputFormatters.Clear();
    })
    .AddXmlSerializerFormatters();

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(options =>
{
    options.MapDefaultControllerRoute();
});

app.Run();