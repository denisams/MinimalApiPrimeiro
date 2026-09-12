using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var todos = app.MapGroup("v1/todos").WithTags("Todos");

todos.MapGet("", async (AppDbContext context) =>
{
    var result = await context.Todos.AsNoTracking().ToListAsync();
    return Results.Ok(result);
})
.Produces<List<Todo>>();

todos.MapPost("", async (
    AppDbContext context,
    CreateTodosViewModel model) =>
{
    var todo = model.MapTo();

    if (!model.IsValid)
        return Results.BadRequest(model.Notifications);

    context.Todos.Add(todo);
    await context.SaveChangesAsync();

    return Results.Created($"/v1/todos/{todo.Id}", todo);
})
.Produces<Todo>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

app.Run();
