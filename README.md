# dotnet GUI test automation platform
A very simple platform to implement GUI tests on WinForms, WPF, GTK# and Xamarin.Mac applications.

Click on the following images to watch the videos of this system working!

[![Automated GUI testing on Linux | YouTube](http://img.youtube.com/vi/YuYjPMirlh0/0.jpg)](http://www.youtube.com/watch?v=YuYjPMirlh0 "Automated GUI testing on Linux | YouTube")

[![Automated GUI testing on macOS | YouTube](http://img.youtube.com/vi/7mEqkUaMxJI/0.jpg)](http://www.youtube.com/watch?v=7mEqkUaMxJI "Automated GUI testing on macOS | YouTube")

## Motivation
This is the automated GUI testing system we use internally to develop Plastic SCM, and we thought it would be a good idea to share it for anyone to benefit from it.

It heavily depends on our own [PNUnit](http://nunit.org/docs/2.5/pnunit.html), a modification to NUnit to run distributed tests we contributed more than a decade ago.

We develop GUIs for Windows, GNU/Linux, and macOS, and while we used a well-known commercial tool on Windows for GUI automated testing, we didn't have a solution for Linux and macOS. The existing solutions were not cross-platform.

So, we decided to go ahead and implement our own GUI testing system.

The beautiful parts are:
* You can write tests in C#, using the same editors and debuggers you use to write your application. After years of scripting languages without proper debuggers and editors, this was a blessing for us.
* The same test code can actually test applications written for different operating systems and even having different aesthetics and layouts.
* Most solutions based on UI Automation on Windows end up in trouble sooner or later. If you heavily tested GUIs on Windows, you probably faced that at some point the "object inspectors" can't find any object. It might happen only once in a while, or repeatedly. **Tests end up being fragile**. We removed this with our implementation.

The less beautiful parts are:
* Purists will say you can easily trick these tests to make them something different to actual GUI tests. For example, when a test _clicks_ a button with a `ClickOkButton()` call, it all depends on how this method is implemented. If it invokes some GUI `button.Click()` code, then it will be almost as good as actually sending the event to the app. If the implementer wants to fool you, it could be invoking the underlying logic, and then the test would be pointless. More on this in the _how does it work_ section.

## How does it work
The PNUnit framework, apart from the `pnunit.framework.dll` library itself, consists on two applications: `agent.exe` and `launcher.exe`. The `launcher` is responsible of telling the `agent` which tests should run, and of gathering and reporting the results. The `agent` is in charge of launching the application being tested, and terminating it if it doesn't exit on its own.

Because the tests need access to the _actual application_ (the entire GUI, and not only the classes that make up your bussiness logic, the tests must run inside the application's context, and not inside the typical NUnit test runner. So, the `agent` is not only in charge of launching the target application, but of serializing the test data and using the right arguments to pass it to the application.

This way, the application is not entirely _test agnostic_, as it ends up depending on the testing framework, but this is a small price to pay compared to the benefits.

While your application runs on the GUI's thread (and on any additional thread it might create, for example for I/O bound work), the test runs on another thread, and it manipulates UI elements through `Invoke` on WinForms, and the like on other platforms. It is the developer's duty to write the code that manipulates the UI as faithful as possible to what the GUI framework would do / behave as with a _real user_.

## How test code looks
The test code is written in C#, taking advantage of the NUnit platform, along with the custom `WaitingAssert` class.

A typical section of a text looks at follows:

```csharp
Assert.AreEqual(
    "Goodbye", window.GetItemInListAt(0),
    "The item 0 in the list does not match the expected one.");

Assert.AreEqual(
    "Hello", window.GetItemInListAt(1),
    "The item 1 in the list does not match the expected one.");

window.ChangeText("Hello");

window.ClickRemoveButton();

WaitingAssert.AreEqual(
    "Removing element Hello...",
    window.GetProgressMessage,
    "The progress message does not match the expected one while" +
        "removing 'Hello'.");

WaitingAssert.IsTrue(
    window.AreButtonsEnabled,
    "The window buttons were not re-enabled in a reasonable " +
        "time after removing 'Hello'.");
```

As you can see, there are very specific interfaces like `ChangeText` and `ClickRemoveButton`. This framework is not a general purpose test framework, but something very simple to develop and maintain.

## Sample application
We built a sample GUI application to explain how testing works. In fact, we built 3 different ones: [WinForms](https://docs.microsoft.com/es-es/dotnet/framework/winforms/) on Windows, [GTKSharp](https://www.mono-project.com/docs/gui/gtksharp/) on GNU/Linux, and [Xamarin.Mac](https://docs.microsoft.com/es-es/xamarin/mac/user-interface/) for macOS, but it is extensible to any GUI framework, as long as there are bindings to work with it from C#. You'd only need to adapt the UI specific code of the testing stack.

![Windows application](https://github.com/PlasticSCM/gui-testing-examples/raw/develop-windows-application/img/windows-application.png)

The sample application is really simple. It consist on a single window that has:
* A text entry.
* Two buttons (`Add` and `Remove`).
* A list.
* A progress text at the bottom.

1) When the user types text and clicks the `Add` button, the application launches a background operation. While the operation runs, the application displays a progress text at the bottom describing what is happening under the hood (it is really just a `Sleep` simulating some CPU bound or I/O). When the operation ends, the text introduced by the user is added to the list in alphabetical order. Then the text input is cleared.
2) If the user clicks the `Remove` button the text is removed from the list.
3) If the text entry is empty when the user clicks a button, the progress text at the bottom displays an error message.
4) If the new text to add already exists in the list, or if the text to remove can't be found, the application shows an error message on a dialog once the background operation ends.

In short, we simply simulate what a regular GUI application does.

![System in action](https://github.com/PlasticSCM/gui-testing-examples/raw/develop-windows-application/img/gui-testing-in-action.gif)

## Repository structure
* `/src`: the main code of the example.
    * `/application`: the source code of the application.
        * `/lib`: the multiplatform bussiness logic. This library does not have code that depends on any UI framework or that is OS-specific.
        * `/gui`: the GUI code.
            * `/windows`: the Windows GUI application.
    * `/testing`: the testing related source code. On this repository there is only GUI testing code, but unit tests would be here too.
        * `/guitest`: the library that contains the tests.
        * `/guitestinterfaces`: the interfaces (and some utility classes) that all of the applications (Windows, macOS & GNU/Linux) need to implement so the tests can interact with them.
* `/lib`: the _third party_ libraries (source code or libraries).
    * `/log4net`: the logging library used by the `pnunit` framework.
    * `/mono`: the library that the testing framework needs to identify the specific OS version and hardware platform when running under the Mono Runtime.
    * `/nunit`: the well known NUnit testing framework, in its last 2.x release.
    * `/pnunit`: Códice's contribution to the NUnit framework, stands for [_Parallel NUnit_](http://nunit.org/docs/2.5/pnunit.html). It allows you to run tests in parallel both between them and to the application that is being tested. This version is a little bit different to the one you can find in the NUnit 2.6 release, with changes to make GUI testing work. Maybe in the future we'll release it as a standalone library, but for now you can use this version.

## How to build
You can build the solutions in this repository with Visual Studio, MonoDevelop, MSBuild, or XBuild.

### Windows with Visual Studio
1) Open `/src/lib/pnunit/pnunit.sln` and build it (`Ctrl + Shift + B` by default).
    * This will build the PNUnit framework to `/bin/pnunit`, including the `agent` and the `launcher`.
2) Open `/src/application/gui/windows/windows.sln` and build it.
    * This will build the application and its dependencies to `/bin/application`.
    * It will also build the testing library because it is in the same Visual Studio solution, but it will output to `/bin/pnunit`.
3) Go to `/bin/pnunit` in two different terminals and launch the following:
    * `> agent.exe agent.conf`
        * This launches the PNUnit agent, which will stay idle until the `launcher` issues a command.
    * `> launcher.exe wintest.conf`
        * This will launch all of the tests defined in the _wintest.conf_ suite, one after the other. Once all the test are executed, it will print the output and generate a report file in the same directory.

### Windows with MSBuild
1) Make sure `MSBuild.exe` is in your `%PATH%`. It normally isn't.
2) Execute the following from the root directory of the repository:
    * `MSBuild.exe lib/pnunit/pnunit.sln`
    * `MSBuild.exe src/application/gui/windows/windows.sln`
3) Go to `/bin/pnunit` in two different terminals and launch the following:
* `> agent.exe agent.conf`
    * This launches the PNUnit agent, which will stay idle until the `launcher` issues a command.
* `> launcher.exe wintest.conf`
    * This will launch all of the tests defined in the _wintest.conf_ suite, one after the other. Once all the tests finish, it will print the output and generate a report file in the same directory.
