var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// app.MapGet("/", () => {
//     var todo = new Todo(Guid.NewGuid(), "Testando Minimal Api", false);
//     return Results.Ok(todo);
// });

app.UseSwagger();

app.MapGet("v1/todos", (AppDbContext context) => {
    var todos = context.Todos.ToList();
    return Results.Ok(todos);
});

app.MapPost("v1/todos", (
                        AppDbContext context,
                        CreateTodosViewModel model) => {
    var todo = model.MapTo();

    if(!model.IsValid)
        return Results.BadRequest(model.Notifications);

    context.Todos.Add(todo);
    context.SaveChanges();

    return Results.Created($"/v1/todos/{todo.Id}", todo);
}).Produces<Todo>();

app.UseSwaggerUI();

app.Run();
