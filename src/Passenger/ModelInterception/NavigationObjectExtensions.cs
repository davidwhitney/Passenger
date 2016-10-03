namespace Passenger.ModelInterception
{
    public static class NavigationObjectExtensions
    {
        public static void SetRebaseOn(this object obj, string rebaseOn)
        {
            obj.GetType().GetProperty("RebaseOn").SetValue(obj, rebaseOn);
        }

        public static string GetRebaseOn(this object obj)
        {
            return (string)obj.GetType().GetProperty("RebaseOn").GetValue(obj);
        }
    }
}