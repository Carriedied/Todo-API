using MediatR;

namespace ToDo.Domain.Events;

public record TodoCreatedDomainEvent(int TodoId, string Title);