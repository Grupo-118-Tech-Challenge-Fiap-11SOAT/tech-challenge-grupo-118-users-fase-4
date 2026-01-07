using Domain.Employee.Dtos;
using Domain.Employee.Exceptions;
using Domain.Employee.Ports.In;
using Domain.Employee.Ports.Out;

namespace Application.Employee;

public class EmployeeManager : IEmployeeManager
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPasswordManager _passwordManager;

    public EmployeeManager(IEmployeeRepository employeeRepository, IPasswordManager passwordManager)
    {
        _employeeRepository = employeeRepository;
        _passwordManager = passwordManager;
    }

    /// <summary>
    /// Creates a new employee based on the provided <see cref="EmployeeRequestDto"/>.
    /// </summary>
    /// <param name="employeeRequestDto">The data transfer object containing employee details.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the created <see cref="EmployeeResponseDto"/>.
    /// If an exception occurs, the returned <see cref="EmployeeResponseDto"/> will contain error details.
    /// </returns>
    /// <exception cref="CpfNullOrEmptyException">Thrown when the CPF is null or empty.</exception>
    /// <exception cref="NameNullOrEmptyException">Thrown when the name is null or empty.</exception>
    /// <exception cref="SurnameNullOrEmptyException">Thrown when the surname is null or empty.</exception>
    /// <exception cref="EmailNullOrEmptyException">Thrown when the email is null or empty.</exception>
    /// <exception cref="BirthDayMinValueException">Thrown when the birth date is invalid.</exception>
    /// <exception cref="PasswordNullOrEmptyException">Thrown when the password is null or empty.</exception>
    public async Task<EmployeeResponseDto> CreateAsync(EmployeeRequestDto employeeRequestDto, CancellationToken cancellationToken)
    {
        try
        {
            var employee = EmployeeRequestDto.ToEntity(employeeRequestDto);
            
            _passwordManager.CreatePasswordHash(employeeRequestDto.Password, out var passwordHash);
            employee.SetPassword(passwordHash.ToString());
            await _employeeRepository.CreateAsync(employee, cancellationToken);

            return EmployeeResponseDto.ToDto(employee);
        }
        catch (Exception ex) when (ex is CpfNullOrEmptyException ||
                                   ex is NameNullOrEmptyException ||
                                   ex is SurnameNullOrEmptyException ||
                                   ex is EmailNullOrEmptyException ||
                                   ex is BirthDayMinValueException ||
                                   ex is PasswordNullOrEmptyException)
        {
            return new EmployeeResponseDto
            {
                ErrorMessage = $"Message: {ex.Message}",
                Error = true
            };
        }
    }

    /// <summary>
    /// Deletes an employee by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to delete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains:
    /// <list type="bullet">
    /// <item><description>0 if the employee does not exist.</description></item>
    /// <item><description>The number of records affected if the employee is successfully deleted.</description></item>
    /// </list>
    /// </returns>
    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee == null)
        {
            return 0;
        }

        return await _employeeRepository.DeleteAsync(employee, cancellationToken);
    }

    /// <summary>  
    /// Retrieves all employees from the repository.  
    /// </summary>  
    /// <returns>  
    /// A task that represents the asynchronous operation. The task result contains a list of <see cref="EmployeeResponseDto"/> objects.  
    /// </returns>  
    public async Task<List<EmployeeResponseDto>> GetAllAsync(CancellationToken cancellationToken, int skip = 0, int take = 10)
    {
        var employeeList = await _employeeRepository.GetAllAsync(cancellationToken, skip, take);

        var result = new List<EmployeeResponseDto>(employeeList.Count);

        foreach (var employee in employeeList)
        {
            result.Add(new EmployeeResponseDto(employee.Id, employee.Cpf, employee.Name, employee.Surname, employee.Email, employee.BirthDay, employee.Role, employee.IsActive));
        }

        return result;
    }

    /// <summary>  
    /// Retrieves an employee by their unique identifier.  
    /// </summary>  
    /// <param name="id">The unique identifier of the employee to retrieve.</param>  
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>  
    /// <returns>  
    /// A task that represents the asynchronous operation. The task result contains the <see cref="EmployeeResponseDto"/> object  
    /// representing the employee if found, or <c>null</c> if the employee does not exist.  
    /// </returns>  
    public async Task<EmployeeResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee == null)
        {
            return null;
        }

        return EmployeeResponseDto.ToDto(employee);
    }

    /// <summary>  
    /// Updates an existing employee based on the provided <see cref="UpdateEmployeeDto"/>.  
    /// </summary>  
    /// <param name="updateEmployeeDto">The data transfer object containing updated employee details.</param>  
    /// <returns>  
    /// A task that represents the asynchronous operation. The task result contains the updated <see cref="EmployeeResponseDto"/>.  
    /// If an exception occurs, the returned <see cref="EmployeeResponseDto"/> will contain error details.  
    /// </returns>  
    /// <exception cref="CpfNullOrEmptyException">Thrown when the CPF is null or empty.</exception>  
    /// <exception cref="NameNullOrEmptyException">Thrown when the name is null or empty.</exception>  
    /// <exception cref="SurnameNullOrEmptyException">Thrown when the surname is null or empty.</exception>  
    /// <exception cref="EmailNullOrEmptyException">Thrown when the email is null or empty.</exception>  
    /// <exception cref="BirthDayMinValueException">Thrown when the birth date is invalid.</exception>  
    /// <exception cref="PasswordNullOrEmptyException">Thrown when the password is null or empty.</exception>  
    public async Task<EmployeeResponseDto> UpdateAsync(UpdateEmployeeDto updateEmployeeDto, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdAsync(updateEmployeeDto.Id, cancellationToken);

            if (employee == null)
            {
                return new EmployeeResponseDto
                {
                    ErrorMessage = "Employee not found.",
                    Error = true
                };
            }

            employee.UpdateEmployee(updateEmployeeDto.Cpf,
                updateEmployeeDto.Name,
                updateEmployeeDto.Surname,
                updateEmployeeDto.Email,
                updateEmployeeDto.BirthDate,
                updateEmployeeDto.Password,
                updateEmployeeDto.Role,
                updateEmployeeDto.IsActive);

            var updatedEmployee = await _employeeRepository.UpdateAsync(employee, cancellationToken);

            return EmployeeResponseDto.ToDto(updatedEmployee);
        }
        catch (Exception ex) when (ex is CpfNullOrEmptyException ||
                                   ex is NameNullOrEmptyException ||
                                   ex is SurnameNullOrEmptyException ||
                                   ex is EmailNullOrEmptyException ||
                                   ex is BirthDayMinValueException ||
                                   ex is PasswordNullOrEmptyException)
        {
            return new EmployeeResponseDto
            {
                ErrorMessage = $"Message: {ex.Message}",
                Error = true
            };
        }
    }
}
