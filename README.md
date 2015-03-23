# Ariane - C# Page Object Models for your Selenium tests
[![Build status](https://ci.appveyor.com/api/projects/status/kticwdj98vfp4roc/branch/master?svg=true)](https://ci.appveyor.com/project/DavidWhitney/ariane/branch/master)

Ariane is a C# page object model library to keep your Selenium code clean and dry.

You can use Ariane to create lightweight page object models that describe the bindings between your tests and the Selenium selectors used for navigation.

Ideally, you'll replace any occurrences of `FindByXXX(someId)` with a strongly typed model.

** Features**
* Page object model support
* Auto-selection and location of page elements based on attributes
* Bindings to Selenium vLatest
* Page component support
* Attribute-based selection
* Raw access to selenium drivers inside your Page objects

## Documentation

* Example usage
* Why would I use Ariane?
* Why would I use this instead of a library like Coypu?
* Writing your first test
  * ArianeConfiguration
  * The PageObjectTestContext
* Creating your first Page Object
  * UrlAttribute
  * Navigation attributes
* Creating your first Page Component
  * PageComponentAttribute

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
  private ArianeConfiguration _testConfig;
  private PageObjectTestContext<Homepage> _ctx;

  [SetUp]
  public void Setup()
  {
    _testConfig = new ArianeConfiguration
    {
      WebRoot = "http://www.davidwhitney.co.uk"
      }.WithDriver(new PhantomJSDriver());
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


## Why would I use Ariane?

Simply, when your markup changes, your tests have to change, so it's important that you can change your tests in one place only.
The benefits of the page object model are well documents elsewhere: http://martinfowler.com/bliki/PageObject.html

This is not a replacement for a BDD or Unit Testing framework, just a way to make your tests a little better.

## Why would I use this instead of a library like Coypu?

There are a couple of good and well maintained "Selenium wrappers" out there that try and wrap, better and hide selenium from your tests.
Ariane sees that approach, however valid, as a barrier to entry - and is to be used by teams that want to stay closer to "native selenium code".
All the types that you're exposed to, with the exception of your page objects themselves, are native, unwrapped calls to WebDriver.

Ariane is an addition to Seleniums existing APIs, not a replacement.

If you use a wrapping library, we'd more than welcome you to provide an implementation of IDriverBindings for your driver wrapping library.

## Writing your first test

* You need a SetUp creating an `ArianeConfiguration` object for your site.
* You need to add a `WebDriver` instance (like the PhantomJsDriver or FirefoxDriver) to the configuration.
* You need to create a `ArianeTestContext` by calling


    _testConfig.StartTestAt<TMyPageObjectType>()

* You need to add a TearDown method that Disposes of the `ArianeTestContext`

**Example**


```csharp
[TestFixture]
public class ExampleUsage
{
  private ArianeConfiguration _testConfig;
  private PageObjectTestContext<Homepage> _ctx;

  [SetUp]
  public void Setup()
  {
    _testConfig = new ArianeConfiguration
    {
      WebRoot = "http://tempuri.org"
      }.WithDriver(new PhantomJSDriver());

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

### Creating an instance of your page object

When you ask your ArianeConfiguration for an instance using `ArianeTestContext.StartTestAt<MyPageObject>();`, Selenium will go and fetch the page using the Url in the page object attribute, and the library will hand you an instance of your page object that you can start to interact with.

### The Uri attribute

The Uri attribute supports either **fully qualified Urls** or **relative paths**. Relative paths are prefered, but when used, the ArianeConfiguration must have it's **WebRoot** property set. An exception will be thrown if you forget to do this.

### Collections

Collections of elements are supported for selectors that return multiple items:

```csharp
[Uri("http://tempuri.org")]
public class MyPageObject
{  
  [CssSelector(".title")]
  public virtual List<IWebElement> Titles { get; set; }
}
```

### Navigation attributes and selectors

We support all the navigation attributes available in selenium:

* XPathAttribute
* TagNameAttribute
* ClassNameAttribute
* NameAttribute
* IdAttribute
* CssSelectorAttribute
* LinkTextAttribute
* PartialLinkTextAttribute

By default, these attributes use the **case sensitive property name* as their selection criteria - but you can override this by providing the selector as parameters. For example:


```csharp
[Uri("http://tempuri.org")]
public class MyPageObject
{  
  [Id("thisIsTheIdOfTheTitle")]
  public virtual IWebElement Title { get; set; }
}
```

### Other methods

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

### Accessing the WebDriver from a page object

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
