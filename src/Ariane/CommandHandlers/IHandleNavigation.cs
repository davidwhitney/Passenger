using System.Reflection;

namespace Ariane.CommandHandlers
{
    public interface IHandleNavigation
    {
        object InvokeDriver(PropertyInfo property);
    }
}