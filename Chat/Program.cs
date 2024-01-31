using ServiceStack;
using Chat;
using Chat.ServiceInterface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServiceStack(typeof(ServerEventsServices).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseServiceStack(new AppHost(), options => {
    options.MapEndpoints();
});
app.MapRazorPages();

app.Run();
