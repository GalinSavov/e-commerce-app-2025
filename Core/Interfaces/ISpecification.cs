using System.Linq.Expressions;
namespace Core.Interfaces;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; } //Where query, this is passed into a specification evaluator class
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    bool IsDistinct { get; }
    //pagination properties
    public int Skip { get; }
    public int Take { get; }
    public bool IsPagingEnabled { get; }
    IQueryable<T> ApplyCriteria(IQueryable<T> query);
}
public interface ISpecification<T, TResult> :ISpecification<T>
{
    Expression<Func<T,TResult>>? Select{ get; }
}
