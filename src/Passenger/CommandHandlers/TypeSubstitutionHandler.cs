using System;
using System.Linq;
using System.Reflection;
using Passenger.Drivers;

namespace Passenger.CommandHandlers
{
    /// <summary>
    /// Populates virtual properties with Driver-specific instances of types
    /// </summary>
    public class TypeSubstitutionHandler
    {
        private readonly PassengerConfiguration _cfg;

        public TypeSubstitutionHandler(PassengerConfiguration cfg)
        {
            _cfg = cfg;
        }

        public DriverBindings.TypeSubstitution FindSubstituteFor(Type type)
        {
            if (_cfg?.Driver.Substitutes == null)
            {
                return null;
            }

            if (!_cfg.Driver.Substitutes.ToList().Any())
            {
                return null;
            }

            return _cfg.Driver.Substitutes.ToList().SingleOrDefault(map => type == map.Type);
        }
    }
}