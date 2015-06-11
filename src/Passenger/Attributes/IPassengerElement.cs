using OpenQA.Selenium;

namespace Passenger.Attributes
{
    public interface IPassengerElement
    {
        IWebElement Inner { get; set; }
    }
}