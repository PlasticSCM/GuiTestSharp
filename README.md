# GUI Testing examples
In this repository you can find an example of how do we do multiplatform GUI testing at Códice Software.

For now, this example only covers Windows (using WinForms, although the same concepts can be effortlessly ported to WPF).

A blogpost will be available soon at [our blog](http://blog.plasticscm.com) explaining this code in detail.

## About the application
The application is really simple: it consist on a single window, with a text entry, two buttons (`Add` and `Remove`), a list, and a progress text at the bottom.

When the user introduces some text and clicks the `Add` button, the application launches a background opperation. While the operation runs, the application displays a progress text at the bottom describing what is happening under the hood, but the only thing said operation really does is sleeping a random ammount of time to simulate a heavy computation or a blocking I/O operation. When the operation ends, the text introduced by the user will be added to the list in alphabetical order, and the text input will be cleared. Something like that happens if the user clicks the `Remove` button, but the text is removed from the list instead of being removed.

If the text entry is empty when the user clicks a button, the progress text at the bottom displays an error message.

If the text being added to the list was already there, or if the text being removed from it was not there in the first place, the application will show an error message once the background operation ends.

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

## How to compile and run the application and the tests
You can compile the solutions within this repository with Visual Studio, MonoDevelop, MSBuild, or XBuild depending on your preferences and the platform you're working in.

### Windows with Visual Studio
For compiling directly with Visual Studio, you would need to:

1) Open `/src/lib/pnunit/pnunit.sln` and compile it (`Ctrl + Shift + B` by default).
    * This will compile the PNUnit framework to `/bin/pnunit`.
2) Open `/src/application/gui/windows/windows.sln` and compile it.
    * This will compile the application and its dependencies to `/bin/application`.
    * It will also compile the testing library because it is in the same Visual Studio solution, but for it the output directory will be `/bin/pnunit`.
3) Go to `/bin/pnunit` in two different terminals and launch the following:
    * `> agent.exe agent.conf`
        * This launches the PNUnit agent, which will stay idle until the launcher issues a command.
    * `> launcher.exe wintest.conf`
        * This will launch all of the tests defined in the _wintest.conf_ suite, one after the other. Once all the test are executed, it will print the output and generate a report file in the same directory.

### Windows with MSBuild
For compiling from CMD, PowerShell, or the like, you would need to:

1) Make sure MSBuild.exe is in your `PATH`. It normally isn't.
2) Execute the following from the root directory of the repository:
    * `MSBuild.exe lib/pnunit/pnunit.sln`
    * `MSBuild.exe src/application/gui/windows/windows.sln`
3) Go to `/bin/pnunit` in two different terminals and launch the following:
* `> agent.exe agent.conf`
    * This launches the PNUnit agent, which will stay idle until the launcher issues a command.
* `> launcher.exe wintest.conf`
    * This will launch all of the tests defined in the _wintest.conf_ suite, one after the other. Once all thetest are executed, it will print the output and generate a report file in the same directory.

### Other ways of compiling
You could integrate the entire application and test building, and test running, using NAnt. If this is something you have interest in, please feel free to request it, or contribute it yourself through GitHub's pull requests :-)
