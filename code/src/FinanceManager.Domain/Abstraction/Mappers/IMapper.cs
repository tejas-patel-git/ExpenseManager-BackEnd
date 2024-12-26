namespace FinanceManager.Domain.Abstraction.Mappers
{
    /// <summary>
    /// Defines a contract for mapping between a source type and a destination type.
    /// </summary>
    /// <typeparam name="TSource">The source type to map from.</typeparam>
    /// <typeparam name="TDestination">The destination type to map to.</typeparam>
    public interface IMapper<TSource, TDestination>
        where TSource : class
        where TDestination : class
    {
        /// <summary>
        /// Maps an object of type <typeparamref name="TSource"/> to an object of type <typeparamref name="TDestination"/>.
        /// </summary>
        /// <param name="source">The source object to be mapped.</param>
        /// <returns>The mapped object of type <typeparamref name="TDestination"/>.</returns>
        TDestination Map(TSource source);

        /// <summary>
        /// Maps a collection of objects of type <typeparamref name="TSource"/> to a collection of objects of type <typeparamref name="TDestination"/>.
        /// </summary>
        /// <param name="source">The collection of source objects to be mapped.</param>
        /// <returns>The collection of mapped objects of type <typeparamref name="TDestination"/>.</returns>
        IEnumerable<TDestination> Map(IEnumerable<TSource> source);
    }
}
