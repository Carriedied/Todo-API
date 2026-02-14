using MediatR;
using Microsoft.AspNetCore.Mvc;
using ToDo.Application.Commands;
using ToDo.Application.Commands.Queries;

namespace ToDo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TodosController> _logger;

    public TodosController(IMediator mediator, ILogger<TodosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<TodoItemDto>>> GetTodos(
        [FromQuery] bool? isCompleted,
        [FromQuery] Domain.Enums.Priority? priority,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting todos with filters: IsCompleted={IsCompleted}, Priority={Priority}",
            isCompleted, priority);

        var query = new GetTodosQuery(isCompleted, priority, page, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to retrieve todos: {Error}", result.Error.Message);
            return StatusCode(500, new ProblemDetails { Title = "Retrieval failed", Detail = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItemDto>> GetTodo(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting todo with ID: {Id}", id);

        var query = new GetTodoByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning("Todo with ID {Id} not found", id);
            return NotFound(new ProblemDetails { Title = "Not Found", Detail = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItemDto>> CreateTodo(
        [FromBody] CreateTodoCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new todo with title: {Title}", command.Title);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to create todo: {Error}", result.Error.Message);
            return BadRequest(new ProblemDetails { Title = "Creation failed", Detail = result.Error.Message });
        }

        return CreatedAtAction(
            nameof(GetTodo),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoItemDto>> UpdateTodo(
        int id,
        [FromBody] UpdateTodoCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating todo with ID: {Id}", id);

        var updatedCommand = command with { Id = id };

        var result = await _mediator.Send(updatedCommand, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Code == "Todo.NotFound")
            {
                _logger.LogWarning("Todo with ID {Id} not found for update", id);
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = result.Error.Message });
            }

            _logger.LogWarning("Failed to update todo: {Error}", result.Error.Message);
            return BadRequest(new ProblemDetails { Title = "Update failed", Detail = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPatch("{id:int}/complete")]
    public async Task<ActionResult<TodoItemDto>> CompleteTodo(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing todo with ID: {Id}", id);

        var command = new CompleteTodoCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Code == "Todo.NotFound")
            {
                _logger.LogWarning("Todo with ID {Id} not found for completion", id);
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = result.Error.Message });
            }

            _logger.LogWarning("Failed to complete todo: {Error}", result.Error.Message);
            return BadRequest(new ProblemDetails { Title = "Completion failed", Detail = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTodo(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting todo with ID: {Id}", id);

        var command = new DeleteTodoCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Code == "Todo.NotFound")
            {
                _logger.LogWarning("Todo with ID {Id} not found for deletion", id);
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = result.Error.Message });
            }

            _logger.LogWarning("Failed to delete todo: {Error}", result.Error.Message);
            return BadRequest(new ProblemDetails { Title = "Deletion failed", Detail = result.Error.Message });
        }

        return NoContent();
    }
}