﻿using FinanceManager.Data.Models;
using FinanceManager.Models;

namespace FinanceManager.Application.Mappers
{
    /// <summary>
    /// Extension methods for mapping between <see cref="Transaction"/> and <see cref="TransactionDto"/>.
    /// </summary>
    public static class TransactionMapperExtension
    {
        /// <summary>
        /// Maps a <see cref="Transaction"/> entity to a <see cref="TransactionDto"/>.
        /// </summary>
        /// <param name="transaction">The <see cref="Transaction"/> entity to map.</param>
        /// <returns>A <see cref="TransactionDto"/> representing the mapped transaction.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="transaction"/> is <c>null</c>.</exception>
        public static TransactionDto MapToDto(this Transaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction, nameof(transaction));

            return new()
            {
                TransactionID = transaction.TransactionID,
                IsExpense = transaction.IsExpense,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Description = transaction.Description
            };
        }

        /// <summary>
        /// Maps a collection of <see cref="Transaction"/> entity to a collection of <see cref="TransactionDto"/>.
        /// </summary>
        /// <param name="transactions">The collection of <see cref="Transaction"/> entities to map.</param>
        /// <returns>An <see cref="IEnumerable{TransactionDto}"/> representing the mapped transactions.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="transactions"/> is <c>null</c>.</exception>
        public static IEnumerable<TransactionDto> MapToDto(this IEnumerable<Transaction> transactions)
        {
            ArgumentNullException.ThrowIfNull(transactions, nameof(transactions));

            var transactionsDto = new List<TransactionDto>();

            foreach (var transaction in transactions)
                transactionsDto.Add(transaction.MapToDto());

            return transactionsDto;
        }

        /// <summary>
        /// Maps a <see cref="TransactionDto"/> to a <see cref="Transaction"/> entity.
        /// </summary>
        /// <param name="transactionDto">The DTO to map.</param>
        /// <returns>The mapped <see cref="Transaction"/> entity.</returns>
        public static Transaction MapToEntity(this TransactionDto transactionDto)
        {
            ArgumentNullException.ThrowIfNull(transactionDto, nameof(transactionDto));

            return new Transaction
            {
                TransactionID = transactionDto.TransactionID,
                IsExpense = transactionDto.IsExpense,
                Amount = transactionDto.Amount,
                Date = transactionDto.Date,
                Description = transactionDto.Description,
                UpdatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Maps a collection of <see cref="TransactionDto"/> objects to a collection of <see cref="Transaction"/> entities.
        /// </summary>
        /// <param name="transactionDtos">The collection of DTOs to map.</param>
        /// <returns>A collection of mapped <see cref="Transaction"/> entities.</returns>
        public static IEnumerable<Transaction> MapToEntity(this IEnumerable<TransactionDto> transactionDtos)
        {
            ArgumentNullException.ThrowIfNull(transactionDtos, nameof(transactionDtos));

            var transactions = new List<Transaction>();

            foreach (var transactionDto in transactionDtos)
                transactions.Add(transactionDto.MapToEntity());

            return transactions;
        }
    }
}
