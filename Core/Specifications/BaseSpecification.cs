using System.Linq.Expressions;
using Core.Entities.OrderAggregate;
using Core.Interfaces;

namespace Core.Specifications;

public class BaseSpecification<T>(Expression<Func<T, bool>>? criteria) : ISpecification<T>
{
    protected BaseSpecification() : this(null) { }
    public Expression<Func<T, bool>>? Criteria => criteria;
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    public bool IsDistinct { get; private set; }

    public int Skip { get; private set; }

    public int Take { get; private set; }

    public bool IsPagingEnabled { get; private set; }

    public List<Expression<Func<T, object>>> Includes { get; } = [];

    public List<string> IncludeStrings { get; } = [];

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes?.Add(includeExpression);
    }
    protected void AddInclude(string includeString)
    {
        IncludeStrings?.Add(includeString);
    }

    protected void SetOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }
    protected void SetOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }
    protected void SetIsDistinct()
    {
        IsDistinct = true;
    }
    protected void SetApplyPaging(int skip, int take)
    {
        IsPagingEnabled = true;
        Skip = skip;
        Take = take;
    }
    public IQueryable<T> ApplyCriteria(IQueryable<T> query)
    {
        if (Criteria != null)
        {
            query = query.Where(Criteria);
        }
        return query;
    }
}
public class BaseSpecification<T, TResult>(Expression<Func<T, bool>>? criteria) 
    : BaseSpecification<T> (criteria),ISpecification<T,TResult>
{
    protected BaseSpecification() : this(null) { } 
    public Expression<Func<T, TResult>>? Select { get; private set; }
    protected void SetSelect(Expression<Func<T,TResult>> selectExpression)
    {
        Select = selectExpression;
    }
}
