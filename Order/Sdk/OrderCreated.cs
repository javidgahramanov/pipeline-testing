namespace Order.Sdk;

public record OrderCreated(Guid Id, IList<Guid> Products, Guid UserId);
