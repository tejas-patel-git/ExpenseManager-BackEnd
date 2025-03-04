using FinanceManager.FunctionalTest.Abstraction;
using FinanceManager.FunctionalTest.TestData;
using FinanceManager.Models.Request;
using FinanceManager.Models.Response;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace FinanceManager.FunctionalTest.Tests.SavingsTests
{
    public class SavingsTest : BaseSavingsTest
    {
        public SavingsTest(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact(DisplayName = "Should create savings goal successfully with valid request")]
        public async Task ShouldCreateSavingsGoal_WhenPostingValidRequest()
        {
            // Arrange
            var savingsRequest = TestDataGenerator.Generate<SavingsRequest>();
            savingsRequest.TargetAmount = 1000m;
            savingsRequest.CurrentBalance = 0m;

            // Act
            var createResponse = await PostSavingsGoal(savingsRequest);

            // Assert
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdSavings = await createResponse.Content.ReadFromJsonAsync<Response<SavingsResponse>>();

            createdSavings.Should().NotBeNull();
            createdSavings!.Data.Should().NotBeNull();
            createdSavings.Data!.Goal.Should().Be(savingsRequest.Goal);
            createdSavings.Data.TargetAmount.Should().Be(savingsRequest.TargetAmount);
            createdSavings.Data.CurrentBalance.Should().Be(savingsRequest.CurrentBalance);

            // Verify in DB
            var dbSavings = await Context.SavingsGoals.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == createdSavings.Data.Id);
            dbSavings.Should().NotBeNull();
            dbSavings!.Goal.Should().Be(savingsRequest.Goal);
        }

        [Theory(DisplayName = "Should update savings goal details when PUTting valid changes")]
        [InlineData(500, 100)]
        [InlineData(2000, 0)]
        public async Task ShouldUpdateSavingsGoal_WhenPuttingValidChanges(decimal newTarget, decimal newBalance)
        {
            // Arrange
            var savingsRequest = await SetupSavingsGoal();
            var createdSavings = await PostSavingsGoal(savingsRequest);
            var createdResponse = await createdSavings.Content.ReadFromJsonAsync<Response<SavingsResponse>>();
            var savingsId = createdResponse!.Data!.Id;

            var updatedRequest = savingsRequest.Clone();
            updatedRequest.TargetAmount = newTarget;
            updatedRequest.CurrentBalance = newBalance;

            // Act
            var updateResponse = await UpdateSavingsGoalAsync(savingsId, updatedRequest);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify update
            var getResponse = await GetSavingsGoalByIdAsync(savingsId);
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedSavings = await getResponse.Content.ReadFromJsonAsync<Response<SavingsResponse>>();

            updatedSavings!.Data!.TargetAmount.Should().Be(newTarget);
            updatedSavings.Data.CurrentBalance.Should().Be(newBalance);
            updatedSavings.Data.Goal.Should().Be(savingsRequest.Goal); // Goal unchanged
        }

        [Fact(DisplayName = "Should return 404 when updating non-existent savings goal")]
        public async Task ShouldReturn404_WhenUpdatingNonExistentSavingsGoal()
        {
            // Arrange
            var savingsRequest = TestDataGenerator.Generate<SavingsRequest>();
            var nonExistentId = Guid.NewGuid();

            // Act
            var updateResponse = await UpdateSavingsGoalAsync(nonExistentId, savingsRequest);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorResponse = await updateResponse.Content.ReadFromJsonAsync<Response>();
            errorResponse!.ErrorMessage.Should().Contain("Savings goal does not exist");
        }

        [Fact(DisplayName = "Should retrieve all savings goals for user when no ID provided")]
        public async Task ShouldRetrieveAllSavingsGoals_WhenNoIdProvided()
        {
            // Arrange
            var savings1 = await SetupSavingsGoal();
            var savings2 = await SetupSavingsGoal();
            await PostSavingsGoal(savings1);
            await PostSavingsGoal(savings2);

            // Act
            var getResponse = await GetSavingsGoalsAsync();

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var savingsList = await getResponse.Content.ReadFromJsonAsync<Response<List<SavingsResponse>>>();

            savingsList!.Data.Should().HaveCountGreaterOrEqualTo(2);
            savingsList.Data!.Should().Contain(s => s.Goal == savings1.Goal);
            savingsList.Data!.Should().Contain(s => s.Goal == savings2.Goal);
        }

        [Fact(DisplayName = "Should delete savings goal successfully when no transactions exist")]
        public async Task ShouldDeleteSavingsGoal_WhenNoTransactionsExist()
        {
            // Arrange
            var savingsRequest = await SetupSavingsGoal();
            var createResponse = await PostSavingsGoal(savingsRequest);
            var createdSavings = await createResponse.Content.ReadFromJsonAsync<Response<SavingsResponse>>();
            var savingsId = createdSavings!.Data!.Id;

            // Act
            var deleteResponse = await DeleteSavingsGoalAsync(savingsId);

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var deleteResult = await deleteResponse.Content.ReadFromJsonAsync<Response>();
            deleteResult!.Message.Should().Contain(savingsId.ToString());

            // Verify deletion
            var getResponse = await GetSavingsGoalByIdAsync(savingsId);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "Should return 400 when creating savings with negative target amount")]
        public async Task ShouldReturn400_WhenCreatingSavingsWithNegativeTarget()
        {
            // Arrange
            var savingsRequest = TestDataGenerator.Generate<SavingsRequest>();
            savingsRequest.TargetAmount = -100m;

            // Act
            var createResponse = await PostSavingsGoal(savingsRequest);

            // Assert
            createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            //var errorResponse = await createResponse.Content.ReadFromJsonAsync<Response>();
            //errorResponse!.ErrorMessage.Should().NotBeNull();
        }

        // Helper Methods
        private async Task<SavingsRequest> SetupSavingsGoal()
        {
            var savingsRequest = TestDataGenerator.Generate<SavingsRequest>();
            savingsRequest.TargetAmount = 1000m;
            savingsRequest.CurrentBalance = 0m;
            return savingsRequest;
        }
    }
    public static class SavingsExtensions
    {
        public static SavingsRequest Clone(this SavingsRequest savings)
        {
            return new SavingsRequest
            {
                Goal = savings.Goal,
                TargetAmount = savings.TargetAmount,
                InitialBalance = savings.InitialBalance,
                CurrentBalance = savings.CurrentBalance
            };
        }
    }
}
