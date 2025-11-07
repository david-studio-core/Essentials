using System.Linq.Expressions;

namespace DavidStudio.Core.DataIO.Helpers;

/// <summary>
/// Provides helper methods for expressions.
/// </summary>
public static class ExpressionsHelper
{
    /// <summary>
    /// Extracts the dot-separated property path from a lambda expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the object containing the property.</typeparam>
    /// <param name="expression">An expression representing the property to extract.</param>
    /// <returns>A string representing the property path, including nested properties (e.g., "Parent.Child.Name").</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the expression is not a valid <see cref="MemberExpression"/> or
    /// a <see cref="UnaryExpression"/> wrapping a <see cref="MemberExpression"/>.
    /// </exception>
    /// <remarks>
    /// The method supports expressions like:
    /// <code>
    /// ExpressionPropertyHelper.GetPropertyPath&lt;Person&gt;(p => p.Name);           // returns "Name"
    /// ExpressionPropertyHelper.GetPropertyPath&lt;Person&gt;(p => p.Address.City);  // returns "Address.City"
    /// </code>
    /// </remarks>
    public static string GetPropertyPath<TEntity>(Expression<Func<TEntity, object>> expression)
    {
        var body = expression.Body;

        if (body is UnaryExpression { NodeType: ExpressionType.Convert } unary)
            body = unary.Operand;

        if (body is not MemberExpression member)
            throw new InvalidOperationException($"Unsupported expression: {expression}");

        var members = new Stack<string>();
        var currentMember = member;

        while (currentMember is not null)
        {
            members.Push(currentMember.Member.Name);
            currentMember = currentMember.Expression as MemberExpression;
        }

        return string.Join('.', members);
    }
}