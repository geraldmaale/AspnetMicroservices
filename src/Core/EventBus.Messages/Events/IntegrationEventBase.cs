namespace EventBus.Messages.Events;

public record IntegrationEventBase
{
    public IntegrationEventBase()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    public IntegrationEventBase(Guid id, DateTime creationDate)
    {
        Id = id;
        CreationDate = creationDate;
    }

    public Guid Id { get; }
    public DateTime CreationDate { get; }
}
