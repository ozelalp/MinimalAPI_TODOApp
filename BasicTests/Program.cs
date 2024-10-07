using BasicTests.EFContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoContext>(options => options.UseInMemoryDatabase("TodoDB"));
// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/todos", async (TodoContext db) =>
{
    return await db.Todos.ToListAsync();
});

app.MapGet("/todos/{id}", async (int id, TodoContext db) =>
{
    return await db.Todos.FindAsync(id);
});

app.MapPost("/todos", async (Todo newItem, TodoContext db) =>
{
    await db.AddAsync<Todo>(newItem);
    await db.SaveChangesAsync();

    return Results.Created($"/todos/{newItem.Id}", newItem);
});

app.MapPut("/todos/{id}", async (int id, Todo item, TodoContext db) =>
{
    var td = await db.FindAsync<Todo>(id);
    if (td == null)
    {
        return Results.NotFound();
    }
    td.Name = item.Name;
    td.ExpireInDays = item.ExpireInDays;
    db.Update(td);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todos/{id}", async (int id, TodoContext db) =>
{
    var td = await db.FindAsync<Todo>(id);
    if (td == null)
    {
        return Results.NotFound();
    }

    db.Remove<Todo>(td);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();