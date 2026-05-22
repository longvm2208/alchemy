using System.Reflection;
using System.ComponentModel;

public static class EnumExtensions
{
    public static string GetDescription(this System.Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());

        if (field != null)
        {
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

            if (attribute != null)
            {
                return attribute.Description;
            }
        }

        return value.ToString();
    }
}
