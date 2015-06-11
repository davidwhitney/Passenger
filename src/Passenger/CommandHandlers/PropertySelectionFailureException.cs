using System;
using System.Reflection;

namespace Passenger.CommandHandlers
{
    public class PropertySelectionFailureException : Exception
    {
        public PropertyInfo Info { get; set; }

        public PropertySelectionFailureException(PropertyInfo info, Exception ex)
            :base("Attempting to match property '" + info + "' failed. " +
                  "Make sure your selector is correct. " +
                  "Collection returned fro WebDriver was empty.", ex)
        {
            Info = info;
        }
    }
}