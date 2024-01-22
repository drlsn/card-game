namespace Corelibs.Basic.Collections;

public static class TreeExtensions
{
    public static TNode FindOrDefault<TNode, TProperty>(
        this TNode node,
        TProperty property,
        Func<TNode, IEnumerable<TNode>> getChildren,
        Func<TNode, TProperty> getProperty)
        where TNode : class
        where TProperty : class
    {
        if (node == default || property == default)
            return default;

        var children = getChildren(node);
        foreach (var child in children)
        {
            var childProperty = getProperty(child);
            if (childProperty.Equals(property))
                return child;
        }

        return default;
    }
}
