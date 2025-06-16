using Azure.Identity;

using GraphT.IoC;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
	builder.Configuration.AddUserSecrets<Program>();
}

if (builder.Environment.IsProduction())
{
	string? keyVaultUri = builder.Configuration["AzureKeyVault:Uri"];
	
	if (!string.IsNullOrEmpty(keyVaultUri))
	{
		builder.Configuration.AddAzureKeyVault(
			new Uri(keyVaultUri),
			new DefaultAzureCredential()
		);
	}
}

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGraphTServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
	    string[]? origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>();
	    string[]? methods = builder.Configuration.GetSection("Cors:Methods").Get<string[]>();
	    string[]? headers = builder.Configuration.GetSection("Cors:Headers").Get<string[]>();

	    policy
		    .WithOrigins(origins ?? Array.Empty<string>())
		    .WithMethods(methods ?? new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS", "PATCH" })
		    .WithHeaders(headers ?? new[] { "Authorization", "Content-Type", "Accept" })
		    .AllowCredentials();
    });

    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("x-pagination");
    });
});

builder.Services.ConfigureSwaggerGen(options =>
{
	options.CustomSchemaIds(x => x.FullName);
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentPolicy");
    app.UseExceptionHandler("/error-development");
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

if (builder.Environment.IsProduction())
{
    app.UseCors("DefaultPolicy");
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

