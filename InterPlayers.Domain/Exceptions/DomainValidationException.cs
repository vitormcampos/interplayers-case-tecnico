namespace InterPlayers.Domain.Exceptions;

public class DomainValidationException(string message = "") : ArgumentException(message);
