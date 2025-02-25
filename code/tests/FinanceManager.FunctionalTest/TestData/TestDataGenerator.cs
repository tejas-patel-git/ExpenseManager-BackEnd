using Bogus;

namespace FinanceManager.FunctionalTest.TestData
{
    public static class TestDataGenerator
    {
        private static readonly Dictionary<Type, object> _fakerRegistry = [];
        static TestDataGenerator()
        {
            RegisterFaker(TestDataFakers.UserFaker());

            RegisterFaker(TestDataFakers.UserBankAccountsFaker());
            RegisterFaker(TestDataFakers.AccountsRequestFaker());

            RegisterFaker(TestDataFakers.TransactionRequestFaker());
            
            RegisterFaker(TestDataFakers.SavingsGoalFaker());
        }

        public static void RegisterFaker<T>(Faker<T> faker) where T : class
        {
            _fakerRegistry[typeof(T)] = faker;
        }

        private static Faker<T> GetFaker<T>() where T : class
        {
            if (!_fakerRegistry.TryGetValue(typeof(T), out var faker))
            {
                throw new InvalidOperationException($"No Faker registered for type {typeof(T).Name}.");
            }
            return (Faker<T>)faker;
        }

        public static T Generate<T>(Action<Faker<T>>? customRules = null) where T : class
        {
            var faker = GetFaker<T>().Clone(); // Clone to avoid modifying the original Faker

            customRules?.Invoke(faker);
            return faker.Generate();
        }

        public static List<T> GenerateMany<T>(int count = 1, Action<Faker<T>>? customRules = null) where T : class
        {
            var faker = GetFaker<T>().Clone();
            customRules?.Invoke(faker);
            return faker.Generate(count);
        }
    }
}