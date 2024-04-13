namespace Unshackled.Fitness.Core.Models;

public class CommandResult
{
	public bool Success { get; set; }
	public string? Message { get; set; }

	public CommandResult()
	{

	}

	public CommandResult(bool success, string? message)
	{
		Success = success;
		Message = message;
	}

	public CommandResult(Exception e)
	{
		Success = false;
		Message = "Something went wrong. We could not complete that request";
	}
}

public class CommandResult<T> : CommandResult
{

	public T? Payload { get; set; }

	public CommandResult() { }

	public CommandResult(bool success, string? message) : base(success, message) { }

	public CommandResult(bool success, string? message, T? payload) : base(success, message)
	{
		Payload = payload;
	}

	public CommandResult(Exception e) : base(e)
	{
		Payload = default;
	}

}
