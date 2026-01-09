using Domain.Employee.Dtos;
using Domain.Employee.Ports.In;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechChallengeUsers.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeManager _employeeManager;

    private readonly ProblemDetails EMPLOYEE_NOT_FOUND = new ProblemDetails
    {
        Title = "Employee not found",
        Status = StatusCodes.Status404NotFound,
        Detail = "The requested employee could not be found."
    };

    /// <summary>  
    /// Initializes a new instance of the <see cref="EmployeeController"/> class.  
    /// </summary>  
    /// <param name="employeeManager">The employee manager service used to handle employee-related operations.</param>  
    public EmployeeController(IEmployeeManager employeeManager)
    {
        _employeeManager = employeeManager;
    }

    /// <summary>  
    /// Creates a new employee in the system.  
    /// </summary>  
    /// <param name="employeeDto">The data transfer object containing the details of the employee to be created.</param>  
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>  
    /// <returns>An object representing the created employee, or a 400 status if the creation fails.</returns>  
    [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<ActionResult<EmployeeResponseDto>> PostAsync([FromBody] EmployeeRequestDto employeeRequestDto, CancellationToken cancellationToken)
    {
        var result = await _employeeManager.CreateAsync(employeeRequestDto, cancellationToken);

        return result.Error ? BadRequest(result) : CreatedAtAction("GetByIdAsync", new { result.Id }, result);
    }

    /// <summary>  
    /// Updates an existing employee.  
    /// </summary>  
    /// <param name="id">The unique identifier of the employee to update.</param>  
    /// <param name="employeeDto">The updated employee data.</param>  
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>  
    /// <returns>An object representing the updated employee, or a 400 status if the update fails.</returns>  
    [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status400BadRequest)]
    [HttpPut("{id}")]
    public async Task<ActionResult<EmployeeResponseDto>> PutAsync(int id, [FromBody] UpdateEmployeeDto employeeDto, CancellationToken cancellationToken)
    {
        employeeDto.Id = id;
        var result = await _employeeManager.UpdateAsync(employeeDto, cancellationToken);

        return result.Error ? BadRequest(result) : Ok(result);
    }

    /// <summary>  
    /// Deletes an employee by their unique identifier.  
    /// </summary>  
    /// <param name="id">The unique identifier of the employee to delete.</param>  
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>  
    /// <returns>The number of records deleted if successful.</returns>  
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpDelete("id")]
    public async Task<ActionResult<int>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _employeeManager.DeleteAsync(id, cancellationToken);

        return Ok(result);
    }

    /// <summary>  
    /// Retrieves a paginated list of all employees.  
    /// </summary>  
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>  
    /// <param name="skip">The number of records to skip for pagination. Default is 0.</param>  
    /// <param name="take">The number of records to take for pagination. Default is 10.</param>  
    /// <returns>A list of objects representing the employees.</returns>  
    [ProducesResponseType(typeof(List<EmployeeResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpGet]
    public async Task<ActionResult<List<EmployeeResponseDto>>> GetAllPaginatedAsync(CancellationToken cancellationToken, int skip = 0, int take = 10)
    {
        var employees = await _employeeManager.GetAllAsync(cancellationToken, skip, take);

        return employees is { Count: > 0 } ? Ok(employees) : NoContent();
    }

    /// <summary>  
    /// Retrieves an employee by their unique identifier.  
    /// </summary>  
    /// <param name="id">The unique identifier of the employee to retrieve.</param>  
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>  
    /// <returns>An object representing the employee, or a 404 status if not found.</returns>  
    [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}"), ActionName("GetByIdAsync")]
    public async Task<ActionResult<EmployeeResponseDto>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await _employeeManager.GetByIdAsync(id, cancellationToken);

        if (employee is null)
        {
            return NotFound(EMPLOYEE_NOT_FOUND);
        }

        return Ok(employee);
    }
}
