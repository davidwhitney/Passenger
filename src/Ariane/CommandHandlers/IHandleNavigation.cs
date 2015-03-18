using System.Reflection;

namespace Ariane.CommandHandlers
{
    public interface IHandleNavigation
    {
        object InvokeSeleniumSelection(PropertyInfo property);
    }
}