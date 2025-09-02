using System.Linq.Expressions;
namespace Core.Interfaces;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; } //Where query, this is passed into a specification evaluator class
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    bool IsDistinct{ get; }
}
public interface ISpecification<T, TResult> :ISpecification<T>
{
    Expression<Func<T,TResult>>? Select{ get; }
}
