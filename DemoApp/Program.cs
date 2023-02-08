using Azure.Identity;
using DemoApp;
using Microsoft.Extensions.Configuration.AzureAppConfiguration.FeatureManagement;
using Microsoft.FeatureManagement;




var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(options =>
{
    // Provide a list of replica endpoints in the order of preference for nth endpoint to fallback on (n+1)th endpoint
    var endpoints = new Uri[] {
        new Uri("OriginEndpointUrl"),
        new Uri("Replica1EndpointUrl") };

// Environment.GetEnvironmentVariable("ConnectionString")

options.Connect(endpoints, new DefaultAzureCredential())
        //Select all keys with prefix "MyPage:" and no label
        .Select("MyPage:*")
        // Configure to reload configuration if the registered key "WebDemo:Settings:MessageMessage" is modified.
        // Use the default cache expiration of 30 seconds. It can be overriden via AzureAppConfigurationRefreshOptions.SetCacheExpiration.
        .ConfigureRefresh(refreshOptions =>
        {
            refreshOptions.Register("MyPage:Sentinel", refreshAll: true);

        })

       // Load all feature flags with no label. To load specific feature flags and labels, set via FeatureFlagOptions.Select.
       // Use the default cache expiration of 30 seconds. It can be overriden via FeatureFlagOptions.CacheExpirationInterval.
       .UseFeatureFlags(FeatureFlagOptions => FeatureFlagOptions.CacheExpirationInterval = TimeSpan.FromSeconds(120));
        
    // Other changes to options
});

// Add services to the container.

builder.Services.AddRazorPages();

//Add AppConfig and FeatureManagement services to the container
builder.Services.AddAzureAppConfiguration()
                .AddFeatureManagement();


// Bind configuration to the Settings object
builder.Services.Configure<Settings>(builder.Configuration.GetSection("MyPage:Settings"));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//refreshes the app config values
app.UseAzureAppConfiguration();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
