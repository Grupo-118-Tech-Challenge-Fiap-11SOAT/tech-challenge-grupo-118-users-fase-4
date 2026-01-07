using Domain.Employee.Dtos;

namespace Domain.Employee.Ports.In;

public interface IEmployeeManager
{
    Task<EmployeeResponseDto> CreateAsync(EmployeeRequestDto employeeRequestDto, CancellationToken cancellationToken);
    Task<EmployeeResponseDto> UpdateAsync(UpdateEmployeeDto employeeDto, CancellationToken cancellationToken);
    Task<int> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<EmployeeResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<EmployeeResponseDto?>> GetAllAsync(CancellationToken cancellationToken, int skip = 0, int take = 10);
}
