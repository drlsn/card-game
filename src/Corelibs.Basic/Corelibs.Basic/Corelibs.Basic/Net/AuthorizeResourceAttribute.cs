namespace Corelibs.Basic.Net
{
    public class AuthorizeResourceAttribute : Attribute
    {
        public AuthorizeResourceAttribute(Type resourceType)
        {
            ResourceType = resourceType;
        }

        public Type ResourceType { get; }
    }
}
