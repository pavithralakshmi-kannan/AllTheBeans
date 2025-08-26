using AllTheBeans.Data;
using AllTheBeans.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add EF Core DbContext (SQL Server)
builder.Services.AddDbContext<BeansDb>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// Simple CORS for demo/testing
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

// Ensure database exists and seed from JSON on first run
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BeansDb>();
    // creates DB if it does not exist (simple for test/demo)
    db.Database.EnsureCreated();
    // seed from Data/AllTheBeans.json if the Beans table is empty
    await SeedData.EnsureSeedAsync(db);
}

// ----------------------------
// Beans endpoints (CRUD + search)
// ----------------------------
app.MapGet("/api/beans", async (BeansDb db, string? q) =>
{
    var beans = db.Beans.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
    {
        q = q.Trim();
        beans = beans.Where(b =>
            b.Name.Contains(q) ||
            b.Country.Contains(q) ||
            b.Colour.Contains(q) ||
            b.Description.Contains(q));
    }
    return await beans.OrderBy(b => b.Name).ToListAsync();
});

app.MapGet("/api/beans/{id:int}", async (BeansDb db, int id) =>
    await db.Beans.FindAsync(id) is Bean b ? Results.Ok(b) : Results.NotFound());

app.MapPost("/api/beans", async (BeansDb db, Bean bean) =>
{
    db.Beans.Add(bean);
    await db.SaveChangesAsync();
    return Results.Created($"/api/beans/{bean.Id}", bean);
});

app.MapPut("/api/beans/{id:int}", async (BeansDb db, int id, Bean updated) =>
{
    var existing = await db.Beans.FindAsync(id);
    if (existing is null) return Results.NotFound();
    existing.ExternalId = updated.ExternalId;
    existing.Index = updated.Index;
    existing.IsBOTD = updated.IsBOTD;
    existing.Name = updated.Name;
    existing.Colour = updated.Colour;
    existing.Country = updated.Country;
    existing.Description = updated.Description;
    existing.Image = updated.Image;
    existing.Cost = updated.Cost;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/beans/{id:int}", async (BeansDb db, int id) =>
{
    var existing = await db.Beans.FindAsync(id);
    if (existing is null) return Results.NotFound();
    db.Beans.Remove(existing);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ----------------------------
// Bean of the Day (BOTD)
// ----------------------------
app.MapGet("/api/botd", async (BeansDb db) =>
{
    var today = DateTime.UtcNow.Date;
    var yday = today.AddDays(-1);

    var todays = await db.BotdAssignments.Include(x => x.Bean)
        .FirstOrDefaultAsync(x => x.Date == today);

    if (todays is not null)
        return Results.Ok(todays);

    var yesterday = await db.BotdAssignments.FirstOrDefaultAsync(x => x.Date == yday);
    var yesterdayBeanId = yesterday?.BeanId;

    var candidates = await db.Beans
        .Where(b => b.Id != yesterdayBeanId)
        .ToListAsync();

    if (candidates.Count == 0)
        return Results.Problem("No candidate beans to choose from.");

    var rnd = Random.Shared.Next(candidates.Count);
    var chosen = candidates[rnd];

    var assignment = new BotdAssignment { Date = today, BeanId = chosen.Id };
    db.BotdAssignments.Add(assignment);
    await db.SaveChangesAsync();

    var hydrated = await db.BotdAssignments.Include(x => x.Bean)
        .FirstAsync(x => x.Id == assignment.Id);

    return Results.Ok(hydrated);
});

// ----------------------------
// Orders (simple)
// ----------------------------
app.MapPost("/api/orders", async (BeansDb db, Order order) =>
{
    if (order.Quantity <= 0) return Results.BadRequest("Quantity must be > 0");
    var bean = await db.Beans.FindAsync(order.BeanId);
    if (bean is null) return Results.BadRequest("Bean not found.");

    order.UnitPrice = bean.Cost;
    order.CreatedUtc = DateTime.UtcNow;

    db.Orders.Add(order);
    await db.SaveChangesAsync();

    return Results.Created($"/api/orders/{order.Id}", order);
});

app.Run();
