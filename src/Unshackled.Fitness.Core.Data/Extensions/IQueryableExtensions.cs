using System.Linq.Expressions;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.Core.Data.Extensions;

public static class IQueryableExtensions
{
	public static IQueryable<T> AddSorts<T>(this IQueryable<T> query, List<SearchSortModel> sorts)
	{
		if (sorts != null && sorts.Count > 0)
		{
			IQueryable<T> newQuery = query.OrderBy(sorts[0].Member, sorts[0].SortDirection);
			if (sorts.Count > 1)
				for (int i = 1; i < sorts.Count; i++)
					newQuery = newQuery.ThenBy(sorts[i].Member, sorts[i].SortDirection);
			return newQuery;
		}
		return query;
	}

	public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, int direction)
	{
		if (source == null) throw new ArgumentNullException("source");
		if (string.IsNullOrEmpty(propertyName)) return source;

		// Create a parameter to pass into the Lambda expression
		//(Entity => Entity.OrderByField).
		var parameter = Expression.Parameter(typeof(T), "Entity");

		//  create the selector part, but support child properties (it works without . too)
		string[] childProperties = propertyName.Split('.');
		MemberExpression property = Expression.Property(parameter, childProperties[0]);
		for (int i = 1; i < childProperties.Length; i++)
		{
			property = Expression.Property(property, childProperties[i]);
		}

		LambdaExpression selector = Expression.Lambda(property, parameter);

		string methodName = direction > 0 ? "OrderByDescending" : "OrderBy";

		MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName,
										new Type[] { source.ElementType, property.Type },
										source.Expression, Expression.Quote(selector));

		return source.Provider.CreateQuery<T>(resultExp);

	}

	public static IQueryable<T> ThenBy<T>(this IQueryable<T> source, string propertyName, int direction)
	{
		if (source == null) throw new ArgumentNullException("source");
		if (string.IsNullOrEmpty(propertyName)) return source;

		// Create a parameter to pass into the Lambda expression
		//(Entity => Entity.OrderByField).
		var parameter = Expression.Parameter(typeof(T), "Entity");

		//  create the selector part, but support child properties (it works without . too)
		string[] childProperties = propertyName.Split('.');
		MemberExpression property = Expression.Property(parameter, childProperties[0]);
		for (int i = 1; i < childProperties.Length; i++)
		{
			property = Expression.Property(property, childProperties[i]);
		}

		LambdaExpression selector = Expression.Lambda(property, parameter);

		string methodName = direction > 0 ? "ThenByDescending" : "ThenBy";

		MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName,
										new Type[] { source.ElementType, property.Type },
										source.Expression, Expression.Quote(selector));

		return source.Provider.CreateQuery<T>(resultExp);
	}
}
