using Flunt.Notifications;
using Flunt.Validations;

public class CreateTodosViewModel : Notifiable<Notification>
{
    public required string Title { get; set; }

    public Todo MapTo()
    {
        var contract = new Contract<Notification>()
                        .Requires()
                        .IsNotNull(Title, "Informe o título da Tarefa")
                        .IsGreaterThan(Title, 5, "O título deve conter mais de 5 caracteres");

        AddNotifications(contract);

        return new Todo(Guid.CreateVersion7(), Title, false);
    }
}
