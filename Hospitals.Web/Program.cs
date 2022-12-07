using Microsoft.EntityFrameworkCore;
using Serilog;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Hospitals.Web.Tests")]

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(); // needs more setup

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); //swagger available at /swagger
builder.Services.AddDbContext<HospitalContext>(options => options.UseInMemoryDatabase("Hospitals")); // b/c it's a demo

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    
}

// swagger normally in the development check
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseRouting();

app.UseSerilogRequestLogging();

/*
 *  No onion layers, vertical slice architecture, or clean architecture used since it's a simple api.
 *  My initial plan was to create the model and then right-click -> Scaffold Controller and Views,
 *  but since the UI was requested to be in react, I implemented that instead.
 *  
 *  I also generally believe that endpoints should be filterable and projectable, but it's not as
 *  straightforward/quick in .NET as it could be. There's OData, but it's not so straightforward with
 *  what's out of the box
 */
app.MapGet("/hospital", (HospitalContext context) => context.Hospitals.ToListAsync()).WithOpenApi();
app.MapGet("/hospital/{id}", (HospitalContext context, Guid id) => context.Hospitals.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id));
app.MapPost("/hospital", async (HospitalContext context, Hospital hospital) =>
{
    await context.Hospitals.AddAsync(hospital);
    await context.SaveChangesAsync();
    return Results.Created($"/hospital/{hospital.ID}", hospital);
}).WithOpenApi();
app.MapPut("/hospital/{id}", async (HospitalContext context, Hospital updateHospital, Guid id) =>
{
    var hospital = await context.Hospitals.FindAsync(id);
    if (hospital == null) return Results.NotFound();
    updateHospital = updateHospital with { ID = id };
    context.Entry(hospital).CurrentValues.SetValues(updateHospital);
    await context.SaveChangesAsync();
    return Results.NoContent();
}).WithOpenApi();
app.MapDelete("/hospital/{id}", async (HospitalContext context, Guid id) =>
{
    // one of the contrib libraries can make it easier to do deletes without
    // fetching first. It can be done with raw ef.
    var hospital = await context.Hospitals.FindAsync(id);
    if (hospital == null) return Results.NotFound();
    context.Hospitals.Remove(hospital);
    await context.SaveChangesAsync();
    return Results.NoContent();
}).WithOpenApi();

/*
 * other nice things to implement:
 * app.MapPatch("/hospital", ...);
 * app.MapHead("/hospital", ...);
 */


// add seed data
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetService<HospitalContext>().Database.EnsureCreatedAsync();
}

app.Run();

internal record Hospital(Guid? ID, string Name);

class HospitalContext : DbContext
{
    public HospitalContext(DbContextOptions options) : base(options) { }
    public DbSet<Hospital> Hospitals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hospital>().HasData(
            new Hospital(Guid.NewGuid(), "I'm a hospital!"),
            new Hospital(Guid.NewGuid(), "I'm a hospital too!"));
        base.OnModelCreating(modelBuilder);
    }
}

// for testing

public partial class Program { }