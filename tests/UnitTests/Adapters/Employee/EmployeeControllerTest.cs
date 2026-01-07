using Domain.Employee.Dtos;
using Domain.Employee.Ports.In;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TechChallengeUsers.API.Controllers;

namespace UnitTests.Adapters.Employee;

public class EmployeeControllerTests
{
    private readonly Mock<IEmployeeManager> _employeeManagerMock;
    private readonly EmployeeController _controller;

    public EmployeeControllerTests()
    {
        _employeeManagerMock = new Mock<IEmployeeManager>();
        _controller = new EmployeeController(_employeeManagerMock.Object);
    }

    #region POST (Create)

    [Fact]
    public async Task PostAsync_ShouldReturnCreated_WhenCreationIsSuccessful()
    {
        // Arrange
        var requestDto = new EmployeeRequestDto();
        var responseDto = new EmployeeResponseDto { Error = false, Id = 123 };

        _employeeManagerMock
            .Setup(x => x.CreateAsync(requestDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        var result = await _controller.PostAsync(requestDto, CancellationToken.None);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;

        createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdResult.ActionName.Should().Be("GetByIdAsync");
        createdResult.RouteValues["Id"].Should().Be(123);
        createdResult.Value.Should().BeEquivalentTo(responseDto);
    }

    [Fact]
    public async Task PostAsync_ShouldReturnBadRequest_WhenCreationFails()
    {
        // Arrange
        var requestDto = new EmployeeRequestDto();
        var responseDto = new EmployeeResponseDto { Error = true };

        _employeeManagerMock
            .Setup(x => x.CreateAsync(requestDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        var result = await _controller.PostAsync(requestDto, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(responseDto);
    }

    #endregion

    #region PUT (Update)

    [Fact]
    public async Task PutAsync_ShouldReturnOk_WhenUpdateIsSuccessful()
    {
        // Arrange
        int id = 5;
        var updateDto = new UpdateEmployeeDto(); // Id inicial é 0 ou null
        var responseDto = new EmployeeResponseDto { Error = false };

        _employeeManagerMock
            .Setup(x => x.UpdateAsync(It.IsAny<UpdateEmployeeDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto)
            .Callback<UpdateEmployeeDto, CancellationToken>((dto, token) =>
            {
                dto.Id.Should().Be(id);
            });

        // Act
        var result = await _controller.PutAsync(id, updateDto, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(responseDto);
    }

    [Fact]
    public async Task PutAsync_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        int id = 5;
        var updateDto = new UpdateEmployeeDto();
        var responseDto = new EmployeeResponseDto { Error = true };

        _employeeManagerMock
            .Setup(x => x.UpdateAsync(It.IsAny<UpdateEmployeeDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        var result = await _controller.PutAsync(id, updateDto, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(responseDto);
    }

    #endregion

    #region DELETE

    [Fact]
    public async Task DeleteAsync_ShouldReturnOk_WhenCalled()
    {
        // Arrange
        int id = 10;
        int recordsDeleted = 1;

        _employeeManagerMock
            .Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recordsDeleted);

        // Act
        var result = await _controller.DeleteAsync(id, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(recordsDeleted);

        _employeeManagerMock.Verify(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GET (All Paginated)

    [Fact]
    public async Task GetAllPaginatedAsync_ShouldReturnOk_WhenEmployeesExist()
    {
        // Arrange
        var employeesList = new List<EmployeeResponseDto>
            {
                new EmployeeResponseDto(),
                new EmployeeResponseDto()
            };

        _employeeManagerMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>(), 0, 10))
            .ReturnsAsync(employeesList);

        // Act
        var result = await _controller.GetAllPaginatedAsync(CancellationToken.None, 0, 10);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(employeesList);
    }

    [Fact]
    public async Task GetAllPaginatedAsync_ShouldReturnNoContent_WhenListIsEmpty()
    {
        // Arrange
        var emptyList = new List<EmployeeResponseDto>();

        _employeeManagerMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _controller.GetAllPaginatedAsync(CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetAllPaginatedAsync_ShouldReturnNoContent_WhenListIsNull()
    {
        // Arrange
        List<EmployeeResponseDto>? nullList = null;

        _employeeManagerMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(nullList);

        // Act
        var result = await _controller.GetAllPaginatedAsync(CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NoContentResult>();
    }

    #endregion

    #region GET (ById)

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOk_WhenEmployeeExists()
    {
        // Arrange
        int id = 1;
        var responseDto = new EmployeeResponseDto();

        _employeeManagerMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        var result = await _controller.GetByIdAsync(id, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(responseDto);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        int id = 99;

        _employeeManagerMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployeeResponseDto?)null);

        // Act
        var result = await _controller.GetByIdAsync(id, CancellationToken.None);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var problemDetails = notFoundResult.Value.Should().BeOfType<ProblemDetails>().Subject;

        problemDetails.Title.Should().Be("Employee not found");
    }

    #endregion
}
