using AutoFixture;
using FinanceManager.Application.Services;
using FinanceManager.Data;
using FinanceManager.Data.Models;
using FinanceManager.Data.Repository;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using FinanceManager.Models.Request;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Linq.Expressions;

namespace FinanceManager.UnitTest;

public class TransactionServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IMapper<TransactionRequest, TransactionDomain>> _requestDomainMapperMock;
    private readonly NullLogger<TransactionService> _logger;
    private readonly TransactionService _transactionService;

    public TransactionServiceTests()
    {
        // Setup AutoFixture
        _fixture = new Fixture();

        // Replace the ThrowingRecursionBehavior with OmitOnRecursionBehavior
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Customize decimal range for Amount to avoid overflow
        _fixture.Customize<TransactionRequest>(c => c
            .With(tr => tr.Amount, _fixture.Create<decimal>() % 10000m + 0.01m)); // Ensure values are within a safe range


        // Mock repository
        _transactionRepositoryMock = new();
        _unitOfWorkMock = new();
        _unitOfWorkMock.SetupGet(un => un.TransactionRepository).Returns(_transactionRepositoryMock.Object);

        // Mock services and mappers
        _userServiceMock = new();
        _requestDomainMapperMock = new();

        // Mock Logger
        _logger = new NullLogger<TransactionService>();

        // Inject mock into service
        _transactionService = new TransactionService(
            _unitOfWorkMock.Object,
            _logger,
            _requestDomainMapperMock.Object,
            _userServiceMock.Object
        );
    }

    [Fact]
    public async Task GetTransactionByIdAsync_ShouldReturnNull_WhenTransactionNotFound()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        _transactionRepositoryMock
            .Setup(repo => repo.GetByIdAsync(transactionId))
            .ReturnsAsync((TransactionDomain?)null);

        // Act
        var result = await _transactionService.GetUserTransactionAsync(transactionId, " ");

        // Assert
        result.Should().BeNull();
        _transactionRepositoryMock.Verify(repo => repo.GetByIdAsync(transactionId), Times.Once);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_ShouldReturnTransaction_WhenTransactionExists()
    {
        // Arrange
        var transaction = _fixture.Create<TransactionDomain>();
        _transactionRepositoryMock
            .Setup(repo => repo.GetByIdAsync(transaction.Id))
            .ReturnsAsync(transaction);

        // Act
        var result = await _transactionService.GetUserTransactionAsync(transaction.Id, transaction.UserId);

        // Assert
        result.Should().Be(transaction);
        _transactionRepositoryMock.Verify(repo => repo.GetByIdAsync(transaction.Id), Times.Once);
    }

    [Fact]
    public async Task GetAllTransactionsAsync_ShouldReturnEmpty_WhenNoTransactionsExist()
    {
        // Arrange
        var userId = _fixture.Create<string>();
        _transactionRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Transaction, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<TransactionDomain>());

        // Act
        var result = await _transactionService.GetUserTransactionsAsync(userId);

        // Assert
        result.Should().BeEmpty();
        _transactionRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Transaction, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task AddTransactionAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var transaction = _fixture.Create<TransactionDomain>();
        _userServiceMock
            .Setup(service => service.UserExistsAsync(transaction.UserId))
            .ReturnsAsync(false);

        // Act
        var result = await _transactionService.AddTransactionAsync(transaction);

        // Assert
        result.Should().BeFalse();
        _userServiceMock.Verify(service => service.UserExistsAsync(transaction.UserId), Times.Once);
    }

    [Fact]
    public async Task AddTransactionAsync_ShouldAddTransaction_WhenUserExists()
    {
        // Arrange
        var transaction = _fixture.Create<TransactionDomain>();
        _userServiceMock
            .Setup(service => service.UserExistsAsync(transaction.UserId))
            .ReturnsAsync(true);

        _transactionRepositoryMock
            .Setup(repo => repo.AddAsync(transaction))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _transactionService.AddTransactionAsync(transaction);

        // Assert
        result.Should().BeTrue();
        _transactionRepositoryMock.Verify(repo => repo.AddAsync(transaction), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateTransactionAsync_ShouldUpdateTransaction()
    {
        // Arrange
        var transactionDomain = _fixture.Create<TransactionDomain>();
        
        _transactionRepositoryMock
            .Setup(repo => repo.UpdateAsync(transactionDomain))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _transactionService.UpdateTransactionAsync(transactionDomain);

        // Assert
        _transactionRepositoryMock.Verify(repo => repo.UpdateAsync(transactionDomain), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteTransactionAsync_ShouldDeleteTransaction()
    {
        // Arrange
        var transactionId = Guid.NewGuid();

        _transactionRepositoryMock
            .Setup(repo => repo.DeleteByIdAsync(transactionId))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _transactionService.DeleteTransactionAsync(transactionId);

        // Assert
        _transactionRepositoryMock.Verify(repo => repo.DeleteByIdAsync(transactionId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
