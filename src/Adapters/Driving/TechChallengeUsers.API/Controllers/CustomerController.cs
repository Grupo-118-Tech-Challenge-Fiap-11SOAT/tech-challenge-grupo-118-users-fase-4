using Domain.Customer.Dtos;
using Domain.Customer.Ports.In;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechChallengeUsers.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerManager _customerManager;

    private readonly ProblemDetails CUSTOMER_NOT_FOUND = new ProblemDetails
    {
        Title = "Customer not found",
        Status = StatusCodes.Status404NotFound,
        Detail = "The requested customer could not be found."
    };

    /// <summary>
    /// Customer constructor
    /// </summary>
    /// <param name="customerManager"></param>
    public CustomerController(ICustomerManager customerManager) => _customerManager = customerManager;

    /// <summary>
    /// Update a Customer
    /// </summary>
    /// <param name="customerUpdateDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(CustomerResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpPut]
    public async Task<ActionResult<CustomerResponseDto>> PutAsync([FromBody] CustomerUpdateDto customerUpdateDto, CancellationToken cancellationToken)
    {
        var result = await _customerManager.UpdateAsync(customerUpdateDto, cancellationToken);

        return result.Error ? BadRequest(result) : Ok(result);
    }
    /// <summary>
    /// Create a new Customer
    /// </summary>
    /// <param name="custormerRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(CustomerResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<ActionResult<CustomerResponseDto>> PostAsync([FromBody] CustomerRequestDto custormerRequestDto, CancellationToken cancellationToken)
    {
        var result = await _customerManager.CreateAsync(custormerRequestDto, cancellationToken);

        return result.Error ? BadRequest(result) : Ok(result);
    }

    /// <summary>
    /// Return a Customer filtering by Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(CustomerResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponseDto>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var customer = await _customerManager.GetByIdAsync(id, cancellationToken);

        if (customer == null)
        {
            return NotFound(CUSTOMER_NOT_FOUND);
        }

        return Ok(customer);
    }

    /// <summary>
    /// Return a Customer filtering by CPF
    /// </summary>
    /// <param name="cpf"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(CustomerResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpGet("/cpf/{cpf}")]
    public async Task<ActionResult<CustomerResponseDto>> GetByIdAsync(string cpf, CancellationToken cancellationToken)
    {
        var customer = await _customerManager.GetByCpfAsync(cpf, cancellationToken);

        if (customer == null)
        {
            return NotFound(CUSTOMER_NOT_FOUND);
        }

        return Ok(customer);
    }
}
