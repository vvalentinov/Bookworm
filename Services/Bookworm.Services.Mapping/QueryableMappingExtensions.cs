namespace Bookworm.Services.Mapping
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using AutoMapper.QueryableExtensions;

    using static Bookworm.Services.Mapping.AutoMapperConfig;

    public static class QueryableMappingExtensions
    {
        public static IQueryable<TDestination> To<TDestination>(
            this IQueryable source,
            params Expression<Func<TDestination,
            object>>[] membersToExpand)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.ProjectTo(MapperInstance.ConfigurationProvider, null, membersToExpand);
        }

        public static IQueryable<TDestination> To<TDestination>(this IQueryable source, object parameters)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.ProjectTo<TDestination>(MapperInstance.ConfigurationProvider, parameters);
        }
    }
}
