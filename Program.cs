var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS (Ensure it allows the deployed React URL)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins(
            "http://localhost:3000",
            "https://your-deployed-react-app.com") // Change this to your actual deployed frontend URL
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS (Place before routing)
app.UseCors("AllowReactApp");

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/", () => "API is running!");

app.Run();
