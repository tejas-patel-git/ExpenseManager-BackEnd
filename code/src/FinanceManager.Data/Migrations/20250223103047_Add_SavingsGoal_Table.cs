using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_SavingsGoal_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavingsTransaction_Transactions_TransactionId",
                table: "SavingsTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SavingsTransaction",
                table: "SavingsTransaction");

            migrationBuilder.RenameTable(
                name: "SavingsTransaction",
                newName: "SavingsTransactions");

            migrationBuilder.RenameIndex(
                name: "IX_SavingsTransaction_TransactionId",
                table: "SavingsTransactions",
                newName: "IX_SavingsTransactions_TransactionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavingsTransactions",
                table: "SavingsTransactions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SavingsGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InitialBalance = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingsGoals", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SavingsTransactions_Transactions_TransactionId",
                table: "SavingsTransactions",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavingsTransactions_Transactions_TransactionId",
                table: "SavingsTransactions");

            migrationBuilder.DropTable(
                name: "SavingsGoals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SavingsTransactions",
                table: "SavingsTransactions");

            migrationBuilder.RenameTable(
                name: "SavingsTransactions",
                newName: "SavingsTransaction");

            migrationBuilder.RenameIndex(
                name: "IX_SavingsTransactions_TransactionId",
                table: "SavingsTransaction",
                newName: "IX_SavingsTransaction_TransactionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavingsTransaction",
                table: "SavingsTransaction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavingsTransaction_Transactions_TransactionId",
                table: "SavingsTransaction",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
