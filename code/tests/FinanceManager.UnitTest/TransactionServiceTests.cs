using AutoFixture;
using FinanceManager.Data;
using FinanceManager.Data.Models;
using FinanceManager.Data.Repository;
using FinanceManager.Models;
using FinanceMangement.Application.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FinananceManager.UnitTest;
public class TransactionServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
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

        // Custom builder for creating controlled circular references
        _fixture.Customize<User>(c => c
            .Without(u => u.Transactions) // Prevent auto-population of Transactions
            .Do(u =>
            {
                // Manually create transactions
                var transactions = new List<Transaction>();
                for (int i = 0; i < 3; i++)
                {
                    var transaction = _fixture.Build<Transaction>()
                        .Without(t => t.User) // Prevent recursion during initial creation
                        .Create();

                    transaction.User = u; // Set circular reference manually
                    transactions.Add(transaction);
                }

                u.Transactions = transactions;
            }));

        // Mock repository
        _transactionRepositoryMock = new();
        _unitOfWorkMock = new();
        _unitOfWorkMock.SetupGet(un => un.TransactionRepository).Returns(_transactionRepositoryMock.Object);

        // Mock Logger
        _logger = new NullLogger<TransactionService>();

        // Inject mock into service
        _transactionService = new TransactionService(_unitOfWorkMock.Object, _logger);
    }

    // TODO : Update the test after service update
    //[Fact]
    //public async Task GetAllTransactionsAsync_ShouldReturnEmpty_WhenNoTransactionsExist()
    //{
    //    // Arrange
    //   _repositoryMock.Setup(set => set.GetAllTransactionsAsync()).ReturnsAsync(Enumerable.Empty<Transaction>());

    //    // Act
    //    var result = await _transactionService.GetAllTransactionsAsync();

    //    // Assert
    //    result.Should().BeEmpty();
    //    _repositoryMock.Verify(repo => repo.GetAllTransactionsAsync(), Times.Once);
    //}

    // TODO : Update the test after service update
    //[Fact]
    //public async Task GetAllTransactionsAsync_ShouldReturnMappedDtos_WhenTransactionsExist()
    //{
    //    // Arrange
    //    var transactions = _fixture.CreateMany<Transaction>(5).ToList(); // Generate 5 mock transactions
    //    _repositoryMock
    //        .Setup(repo => repo.GetAllTransactionsAsync())
    //        .ReturnsAsync(transactions);

    //    // Act
    //    var result = await _transactionService.GetAllTransactionsAsync();

    //    // Assert
    //    result.Should().NotBeNull()
    //          .And.HaveCount(transactions.Count)
    //          .And.AllBeOfType<TransactionDto>();

    //    _repositoryMock.Verify(repo => repo.GetAllTransactionsAsync(), Times.Once);
    //}

    [Fact]
    public async Task GetAllTransactionsAsync_ByUserId_ShouldReturnEmpty_WhenNoTransactionsExistForUser()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        _transactionRepositoryMock
            .Setup(repo => repo.GetAllTransactionsAsync(userId))
            .ReturnsAsync(Enumerable.Empty<Transaction>());

        // Act
        var result = await _transactionService.GetAllTransactionsAsync(userId);

        // Assert
        result.Should().BeEmpty();
        _transactionRepositoryMock.Verify(repo => repo.GetAllTransactionsAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetAllTransactionsAsync_ByUserId_ShouldReturnMappedDtos_WhenTransactionsExistForUser()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var transactions = _fixture.CreateMany<Transaction>(3).ToList(); // Generate 3 mock transactions
        _transactionRepositoryMock
            .Setup(repo => repo.GetAllTransactionsAsync(userId))
            .ReturnsAsync(transactions);

        // Act
        var result = await _transactionService.GetAllTransactionsAsync(userId);

        // Assert
        result.Should().NotBeNull()
              .And.HaveCount(transactions.Count)
              .And.AllBeOfType<TransactionDto>();

        _transactionRepositoryMock.Verify(repo => repo.GetAllTransactionsAsync(userId), Times.Once);
    }
}