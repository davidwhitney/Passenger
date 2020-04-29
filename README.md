# Passenger - C# Page Object Models for your Selenium tests
[![Build status](https://ci.appveyor.com/api/projects/status/hbd96aw13hoxejud/branch/master?svg=true)](https://ci.appveyor.com/project/DavidWhitney/passenger/branch/master)

Passenger is a C# page object model library to keep your Selenium code clean and dry.

You can use Passenger to create lightweight page object models that describe the bindings between your tests and the Selenium selectors used for navigation.

Ideally, you'll replace any occurrences of `FindByXXX(someId)` with a strongly typed model.

** Features**
* Page object model support
* Auto-selection and location of page elements based on attributes
* Bindings to Selenium vLatest
* Page component support
* Attribute-based selection
* Raw access to selenium drivers inside your Page objects

## Installation

Add Passenger to your test projects using NuGet

		PM> Install-Package Passenger


## Documentation

* Example usage
* Why would I use Passenger?
* Why would I use this instead of a library like Coypu?
* Writing your first test
  * PassengerConfiguration
  * The PageObjectTestContext
* Creating your first Page Object
  * The UrlAttribute
	* Collections
  * Navigation attributes and selectors
	* Accessing WebDriver from a page object
	* Methods
* Creating your first Page Component
  * PageComponentAttribute
* Extended features
	* Page transitions and method chaining
	* Building UI abstractions with IPassengerElements

## Example usage

Given a POCO Page Object that looks like this:


```csharp
[Uri("/")]
public class Homepage
{
  // Magically wired up.
  protected virtual RemoteWebDriver YayWebDriver { get; set; }

  [Id("middleWrapper")]
  public virtual IWebElement MiddleWrapper { get; set; }

  [LinkText]
  public virtual IWebElement Blog { get; set; }

  public void FillInForm(string user)
  {
    var ele = YayWebDriver.FindElementById("someForm"); // Or some other driver operation
  }
}

[Uri("/Blog")]
public class Blog
{
  [CssSelector(".blog-post-title-on-index")]
  public virtual IEnumerable<IWebElement> Posts { get; set; }
}
```

Consider the following C# test

```csharp
[TestFixture]
public class ExampleUsage
{
  private PassengerConfiguration _testConfig;
  private PageObjectTestContext<Homepage> _ctx;

  [SetUp]
  public void Setup()
  {
    var chromeOptions = new ChromeOptions();
    chromeOptions.AddArgument("--headless");
    chromeOptions.AddArgument("--no-sandbox");
    chromeOptions.AddArgument("window-size=1400,2100");
    var driver = new ChromeDriver(Environment.CurrentDirectory, chromeOptions);

    _testConfig = new PassengerConfiguration
    {
        WebRoot = "http://www.davidwhitney.co.uk"
    }.WithDriver(driver);
  }

    [Test]
    public void BrowseToTheHomepage_ClickADiv_FillInAForm_ThenGoToTheBlog()
    {
      _ctx = _testConfig.StartTestAt<Homepage>();

      _ctx.Page<Homepage>().MiddleWrapper.Click();
      _ctx.Page<Homepage>().FillInForm("abc");

      _ctx.Page<Homepage>().Blog.Click();
      _ctx.VerifyRedirectionTo<Blog>();

      foreach (var post in _ctx.Page<Blog>().Posts)
      {
        Console.WriteLine(post.Text);
      }
    }

    [TearDown]
    public void Teardown()
    {
      _ctx.Dispose();
    }
  }
  ```


## Why would I use Passenger?

Simply, when your markup changes, your tests have to change, so it's important that you can change your tests in one place only.
The benefits of the page object model are well documents elsewhere: http://martinfowler.com/bliki/PageObject.html

This is not a replacement for a BDD or Unit Testing framework, just a way to make your tests a little better.

## Why would I use this instead of a library like Coypu?

There are a couple of good and well maintained "Selenium wrappers" out there that try and wrap, better and hide selenium from your tests.
Passenger sees that approach, however valid, as a barrier to entry - and is to be used by teams that want to stay closer to "native selenium code".
All the types that you're exposed to, with the exception of your page objects themselves, are native, unwrapped calls to WebDriver.

Passenger is an addition to Seleniums existing APIs, not a replacement.

If you use a wrapping library, we'd more than welcome you to provide an implementation of IDriverBindings for your driver wrapping library.

## Writing your first test

* You need a SetUp creating an `PassengerConfiguration` object for your site.
* You need to add a `WebDriver` instance (like the ChromeDriver or FirefoxDriver) to the configuration.
* You need to create a `PassengerTestContext` by calling ```_testConfig.StartTestAt<TMyPageObjectType>()```
* You need to add a TearDown method that Disposes of the `PassengerTestContext`

**Example**


```csharp
[TestFixture]
public class ExampleUsage
{
  private PassengerConfiguration _testConfig;
  private PageObjectTestContext<Homepage> _ctx;

  [SetUp]
  public void Setup()
  {
    var chromeOptions = new ChromeOptions();
    chromeOptions.AddArgument("--headless");
    chromeOptions.AddArgument("--no-sandbox");
    chromeOptions.AddArgument("window-size=1400,2100");
    var driver = new ChromeDriver(Environment.CurrentDirectory, chromeOptions);

    _testConfig = new PassengerConfiguration
    {
        WebRoot = "http://tempuri.org"
    }.WithDriver(driver);

    _ctx = _testConfig.StartTestAt<MyPageObject>();
  }

    [TearDown]
    public void Teardown()
    {
      _ctx.Dispose();
    }
  }
```


&nbsp;

## Creating your first Page Object

Page objects are POCOs with a few important attributes applied to them.
The simplest possible Page object looks like this

```csharp
[Uri("http://tempuri.org")]
public class MyPageObject
{  
}
```

That's a Page object for the site located at the Url provided in the attribute applied to the class. You can use it in your tests, but it doesn't have any behaviour.

In order to make use of the Page object, you'll want to add properties representing parts of your page. For example...


```csharp
[Uri("http://tempuri.org")]
public class MyPageObject
{  
  [Id]
  public virtual IWebElement Title { get; set; }
}
```

There are a few important things here
  1. The page element **must be declared as a public/protected virtual** property.
  2. The `Type` of the property must match the `Type` Selenium would return when `FindByXXX`-ing.

## Creating an instance of your page object

When you ask your PassengerConfiguration for an instance using

    PassengerTestContext.StartTestAt<MyPageObject>();

Selenium will go and fetch the page using the Url in the page object attribute, and the library will hand you an instance of your page object that you can start to interact with.

## The Uri attribute

The Uri attribute supports either **fully qualified Urls** or **relative paths**. Relative paths are preferred, but when used, the PassengerConfiguration must have it's **WebRoot** property set. An exception will be thrown if you forget to do this.

The Uri attribute supports an optional verification pattern regular expression - if you provide one, any calls to verify page transitions will validate against this regex, allowing you finer grained control over checking for correct page transitions. Setting up a verification pattern is easy, just provide it as a second parameter in your Uri attribute declaration.

```csharp
[Uri("http://tempuri.org", ".+tempuri\\.[a-z]+")]
```

## Collections

Collections of elements are supported for selectors that return multiple items:

```csharp
[Uri("http://tempuri.org")]
public class MyPageObject
{  
  [CssSelector(".title")]
  public virtual List<IWebElement> Titles { get; set; }
}
```

## Navigation attributes and selectors

We support all the navigation attributes available in selenium:

* XPathAttribute
* TagNameAttribute
* ClassNameAttribute
* NameAttribute
* IdAttribute
* CssSelectorAttribute
* LinkTextAttribute
* PartialLinkTextAttribute

By default, these attributes use the **case sensitive property name** as their selection criteria - but you can override this by providing the selector as parameters. For example:


```csharp
[Uri("http://tempuri.org")]
public class MyPageObject
{  
  [Id("thisIsTheIdOfTheTitle")]
  public virtual IWebElement Title { get; set; }
}
```

## Accessing the WebDriver from a page object

You can access the native WebDriver from inside your page objects by providing a public/protected virtual property of the type `IWebDriver` or `RemoteWebDriver`.

```csharp
[Uri("http://tempuri.org")]
public class MyPageObject
{  
  protected virtual IWebDriver CurrentDriver { get; set; }

  public void FillInMyForm()
  {
    CurrentDriver.SelectBy....
  }
}
```

The driver that gets returned when you access that property will be the current driver from the test context you are currently executing it - basically - it'll "just work".

## Methods

Apart from these "magical" properties, your Page objects behave like normal objects, so you can and should write methods to perform your page interactions in the objects themselves


```csharp
[Uri("http://tempuri.org")]
public class MyPageObject
{  
  [Id("someForm")]
  public virtual IWebElement Form { get; set; }

  public void FillInMyForm()
  {
    var theForm = Form;
    // Do something with the form here...
  }
}
```

Remember - the idea is to encapsulate any page operations inside the object - and leave your tests or BDD scenarios only orchestrating calls to the page object.


## Creating your first Page Component

Page components represent re-usable portions of your Page object model, like a consistent navigation menu. They work in exactly the same way, with exactly the same features, as the "root" page object.

To create a page component, just create a POCO with the `PageComponentAttribute` on the class. The library will correctly hook up your page components for you, and you can use them like this:

```csharp
[Uri("http://tempuri.org")]
public class MyPageObject
{  
  public virtual MyNav Navigation { get; set; }

  public void GoHome()
  {
    Navigation.HomeLink.Click();
  }
}

[PageComponent]
public class MyNav
{  
  [Id("homeLink")]
  public virtual IWebElement HomeLink { get; set; }
}
```

# Extended features

## Page transitions and method chaining

Page objects support `Page transitions` - it's common for a method on a page object to drive the browser to another Uri - page transitions are a way of capturing this behaviour in your model.

To implement a method that leads to another page - you must:
* Mark the method as `virtual`
* Set the return type of the method to be the page object you're transitioning to
* Use the helper method `Arrives.At<TDestinationPageObject>()` as your return statement.

This will create the subsequent page object for you, imbued with Passenger magic.

Given this page object for searching:

```csharp
[Uri("/")]
public class Homepage
{
    public virtual RemoteWebDriver Driver { get; set; }

    [Id("twotabsearchtextbox")]
    public virtual IWebElement SearchBox { get; set; }

    [CssSelector("nav-searchbar")]
    public virtual IWebElement SearchForm { get; set; }

    public virtual SearchResultsPage SearchFor(string thing)
    {
        SearchBox.Click();
        SearchBox.SendKeys(thing);
        SearchForm.Submit();

        return Arrives.At<SearchResultsPage>();
    }
}
```

You can now use method chaining to write tests that look like this:

```csharp
using (var context = testConfig.StartTestAt<Homepage>())
{
    context
        .Page<Homepage>()
        .SearchFor("Game of thrones")
        .SomeMethodOnSearchResultsPage();
}
```

If you need access to the initial `PageObject<TYourPageObject>` you can use the method `Arrives.AtPageObject<TYourPageObject>` as your method return.

### Navgating between domains

Both the .GoTo<T> and Arrive.At<T> methods contain an optional `rebaseOn` parameter.
Providing a value will switch the WebRoot you're currently navigating across to support
scenarios where the same relative, attribute based Uris are split across multiple domains.

You can also manually manipulate the WebRoot on your configuration object at any time.

## Building UI abstractions with IPassengerElements

In addition to your Page Objects and Page Components you may find you need to test web applications with small repeating UI elements that you need to write some WebDriver code to maniuplate - it could be something as small as a button, or as complex as a specific type of menu or javascript driven control. We have provided a hook - the `IPassengerElement` - to help you capture these interactions.

`IPassengerElement` is a simple interface you can implement that you can use in place of a standard Selenium IWebElement. Consider the following example:

```csharp
public class MyPageObject
{
		[Id]
		public virtual MyButton Button { get; set; }

		[CssSelector]
		public virtual List<MyButton> Buttons { get; set; }
}

public class MyButton : IPassengerElement
{
		public IWebElement Inner { get; set; }
}
```
The `IPassengerElement` interface forces you to implement a single public property which can be get/set by the library. If you implement this interface, you can use the implementing class anywhere a normal `IWebElement` would work - the `Inner` property will be set with the underlying `IWebElement` from the selenium selection.

You can use `IPassengerElement`s with collections, or on their own, to build richer DSLs using Passenger.
