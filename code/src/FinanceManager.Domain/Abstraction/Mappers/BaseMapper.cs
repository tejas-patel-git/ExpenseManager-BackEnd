namespace FinanceManager.Domain.Abstraction.Mappers
{
    public abstract class BaseMapper<TSource, TDestination> : IMapper<TSource, TDestination>
        where TSource : class
        where TDestination : class
    {
        private readonly Func<TSource, TDestination> _mapFunc;

        protected BaseMapper(Func<TSource, TDestination> mapFunc)
        {
            _mapFunc = mapFunc ?? throw new ArgumentNullException(nameof(mapFunc));
        }

        public TDestination Map(TSource source) {
            ArgumentNullException.ThrowIfNull(source, nameof(source));

            return _mapFunc(source);
        }

        public IEnumerable<TDestination> Map(IEnumerable<TSource> source)
        {
            ArgumentNullException.ThrowIfNull(source, nameof(source));
            
            return source.Select(_mapFunc);
        }
    }
}
