using System.Runtime.Serialization;

namespace Didox.Infrastructure.Client.Extensions;

public static class EnumExtensions
{
    public static string GetEnumMemberValue(this Enum value)
    {
        var member = value.GetType()
            .GetMember(value.ToString())
            .FirstOrDefault();

        var enumMemberValue = member?
            .GetCustomAttributes(typeof(EnumMemberAttribute), false)
            .Cast<EnumMemberAttribute>()
            .FirstOrDefault()?
            .Value;
        
        if (enumMemberValue != null)
            return enumMemberValue;
        
        return value.ToString();
    }
}
