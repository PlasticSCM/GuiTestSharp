# GUI Testing examples

In this repository you can find an example of how do we do multiplatform GUI testing at Códice Software.

For now, this example only covers Windows (using WinForms, although the same concepts can be effortlessly ported to WPF).

A blogpost will be available soon at [our blog](http://blog.plasticscm.com) explaining this code in detail.

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
    * `/pnunit`: Códice's contribution to the NUnit framework, stands for _Parallel NUnit_. It allows you to run tests in parallel both between them and to the application that is being tested. This version is a little bit different to the one you can find in the NUnit 2.6 release, with changes to make GUI testing work. Maybe in the future we'll release it as a standalone library, but for now you can use this version.

## How to compile and run the application and the tests

You can compile the solutions within this repository with Visual Studio, MonoDevelop, MSBuild, or XBuild depending on your preferences and the platform you're working in.

### Windows with Visual Studio
For Visual Studio, you would need to:

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
