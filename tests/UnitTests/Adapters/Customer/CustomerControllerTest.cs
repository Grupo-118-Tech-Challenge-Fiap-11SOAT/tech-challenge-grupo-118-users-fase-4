using Domain.Customer.Dtos;
using Domain.Customer.Ports.In;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TechChallengeUsers.API.Controllers;

namespace UnitTests.Adapters.Customer;

public class CustomerControllerTests
{
    private readonly Mock<ICustomerManager> _customerManagerMock;
    private readonly CustomerController _controller;

    public CustomerControllerTests()
    {
        _customerManagerMock = new Mock<ICustomerManager>();
        _controller = new CustomerController(_customerManagerMock.Object);
    }

    #region PUT (Update)

    [Fact]
    public async Task PutAsync_ShouldReturnOk_WhenUpdateIsSuccessful()
    {
        // Arrange
        var updateDto = new CustomerUpdateDto();
        // Simulando resposta de sucesso (Error = false)
        var responseDto = new CustomerResponseDto { Error = false };

        _customerManagerMock
            .Setup(x => x.UpdateAsync(updateDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        var result = await _controller.PutAsync(updateDto, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(responseDto);
    }

    [Fact]
    public async Task PutAsync_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        var updateDto = new CustomerUpdateDto();
        // Simulando resposta de erro de negócio (Error = true)
        var responseDto = new CustomerResponseDto { Error = true };

        _customerManagerMock
            .Setup(x => x.UpdateAsync(updateDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        var result = await _controller.PutAsync(updateDto, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(responseDto);
    }

    #endregion

    #region POST (Create)

    [Fact]
    public async Task PostAsync_ShouldReturnOk_WhenCreationIsSuccessful()
    {
        // Arrange
        var requestDto = new CustomerRequestDto();
        var responseDto = new CustomerResponseDto { Error = false };

        _customerManagerMock
            .Setup(x => x.CreateAsync(requestDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        var result = await _controller.PostAsync(requestDto, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(responseDto);
    }

    [Fact]
    public async Task PostAsync_ShouldReturnBadRequest_WhenCreationFails()
    {
        // Arrange
        var requestDto = new CustomerRequestDto();
        var responseDto = new CustomerResponseDto { Error = true };

        _customerManagerMock
            .Setup(x => x.CreateAsync(requestDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        var result = await _controller.PostAsync(requestDto, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(responseDto);
    }

    #endregion

    #region GET (ById - int)

    [Fact]
    public async Task GetByIdAsync_Int_ShouldReturnOk_WhenCustomerExists()
    {
        // Arrange
        int customerId = 1;
        var responseDto = new CustomerResponseDto();

        _customerManagerMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        var result = await _controller.GetByIdAsync(customerId, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(responseDto);
    }

    [Fact]
    public async Task GetByIdAsync_Int_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        int customerId = 99;

        _customerManagerMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerResponseDto?)null);

        // Act
        var result = await _controller.GetByIdAsync(customerId, CancellationToken.None);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var problemDetails = notFoundResult.Value.Should().BeOfType<ProblemDetails>().Subject;

        problemDetails.Title.Should().Be("Customer not found");
        problemDetails.Status.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion

    #region GET (ByCpf - string)

    [Fact]
    public async Task GetByIdAsync_Cpf_ShouldReturnOk_WhenCustomerExists()
    {
        // Arrange
        string cpf = "12345678900";
        var responseDto = new CustomerResponseDto();

        _customerManagerMock
            .Setup(x => x.GetByCpfAsync(cpf, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseDto);

        // Act
        var result = await _controller.GetByIdAsync(cpf, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(responseDto);
    }

    [Fact]
    public async Task GetByIdAsync_Cpf_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        string cpf = "00000000000";

        _customerManagerMock
            .Setup(x => x.GetByCpfAsync(cpf, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerResponseDto?)null);

        // Act
        var result = await _controller.GetByIdAsync(cpf, CancellationToken.None);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var problemDetails = notFoundResult.Value.Should().BeOfType<ProblemDetails>().Subject;

        problemDetails.Title.Should().Be("Customer not found");
    }

    #endregion
}