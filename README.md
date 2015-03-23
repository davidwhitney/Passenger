# Ariane - C# Page Object Models for your Selenium tests

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
