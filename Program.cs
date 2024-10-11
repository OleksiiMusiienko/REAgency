using Azure;
using REAgency.BLL.Infrastructure;
using REAgency.BLL.Interfaces;
using REAgency.BLL.Interfaces.Locations;
using REAgency.BLL.Interfaces.Persons;
using REAgency.BLL.Services;
using REAgency.BLL.Services.Persons;
using REAgency.BLL.Services.Locations;
using REAgency.BLL.Interfaces.Object;
using REAgency.BLL.Services.Objects;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession();

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddREAgencyContext(connection);
builder.Services.AddUnitOfWorkService(); 
builder.Services.AddTransient<IClientService, ClientService>();
builder.Services.AddTransient<IOperationService, OperationService>();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<ILocalityService, LocalityService>();
builder.Services.AddTransient<IFlatService, FlatService>();
builder.Services.AddTransient<IEstateObjectService, EstateObjectService>();
builder.Services.AddTransient<IHouseSevice, HouseService>();
builder.Services.AddTransient<IOfficeService, OfficeService>();
builder.Services.AddTransient<IGarageService, GarageService>();
builder.Services.AddTransient<ISteadService, SteadService>();
builder.Services.AddTransient<IAreaService, AreaService>();
builder.Services.AddTransient<ICurrencyService, CurrencyService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<ILocationService, LocationService>();
builder.Services.AddTransient<IRegionService, RegionService>();
builder.Services.AddTransient<IDistrictService, DistrictService>();
builder.Services.AddTransient<IRoomService, RoomService>();
builder.Services.AddTransient<IPremisService, PremisService>();
builder.Services.AddTransient<IParkingService, ParkingService>();
builder.Services.AddTransient<IStorageService, StorageService>();

builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
