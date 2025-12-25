namespace InterPlayers.Application.Exceptions;

public class NotFoundException(string name, string key)
    : ArgumentException($"{name} with key '{key}' was not found.");
