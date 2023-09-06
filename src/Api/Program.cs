using Api.OpenApi;
using Asp.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    builder.Services.AddSwaggerGen(opts => opts.OperationFilter<SwaggerDefaultValues>());
    builder.Services.AddApiVersioning(opts =>
        {
            opts.DefaultApiVersion = new ApiVersion(1);
            opts.AssumeDefaultVersionWhenUnspecified = true;
            opts.ApiVersionReader = new UrlSegmentApiVersionReader();
            opts.ReportApiVersions = true;
        })
        .AddApiExplorer(opts =>
        {
            opts.GroupNameFormat = "'v'VVV";
            opts.SubstituteApiVersionInUrl = true;
        });
}

var app = builder.Build();
{
    var greetings = app.NewVersionedApi("Greetings");
    RouteGroupBuilder? greetingsV1 = greetings.MapGroup("api/v{version:apiVersion}/greetings")
        .HasApiVersion(new ApiVersion(1));

    greetingsV1.MapGet("/", () => "Hello World");

    var greetingsV2 = greetings.MapGroup("api/v{version:apiVersion}/greetings")
        .HasApiVersion(new ApiVersion(2));

    greetingsV2.MapGet("/", () => "Hello World v2");

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var descriptions = app.DescribeApiVersions();
            foreach (var description in descriptions)
            {
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
            }
        });
    }

    app.Run();
}
