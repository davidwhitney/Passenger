using System;
using System.Linq;
using System.Reflection;
using Ariane.Drivers;

namespace Ariane.CommandHandlers
{
    /// <summary>
    /// Populates virtual properties with Driver-specific instances of types
    /// </summary>
    public class TypeSubstitutionHandler
    {
        private readonly IDriverBindings _driver;

        public TypeSubstitutionHandler(IDriverBindings driver)
        {
            _driver = driver;
        }

        public DriverBindings.TypeSubstitution FindSubstituteFor(Type type)
        {
            if (_driver == null || _driver.Substitutes == null)
            {
                return null;
            }

            if (!_driver.Substitutes.ToList().Any())
            {
                return null;
            }

            return _driver.Substitutes.ToList().SingleOrDefault(map => type == map.Type);
        }
    }
}