using System;

using log4net;
using NUnit.Framework;

using Codice.Examples.GuiTesting.GuiTestInterfaces;
using PNUnit.Framework;

namespace GuiTest
{
    [TestFixture]
    public class ApplicationWindowTests
    {
        [Test]
        public void BasicApplicationWindowTest()
        {
            PNUnitServices.Get().WriteLine("The test is running!");

            TestOperations.RunTest(
                "BasicApplicationWindowTest",
                DoBasicApplicationWindowTest);
        }

        void DoBasicApplicationWindowTest(string testName)
        {
            try
            {
                ITesteableApplicationWindow window =
                    GuiTesteableServices.GetApplicationWindow();

                window.ChangeText("Hello");

                window.ClickAddButton();

                // We could use a regular Assert.AreEqual here, as in our example
                // the progress text is updated immediately on the UI thread
                // (see ApplicationOperations class), but if this progress text
                // was updated for example depending on the phase of the operation,
                // this should be the way to go.
                WaitingAssert.AreEqual(
                    "Adding element Hello...",
                    window.GetProgressMessage,
                    "The progress message does not match the expected one " +
                        "while adding 'Hello'.");

                // We could NOT use a regular Assert.IsTrue here. We don't know for
                // how long does the background job run, so we'll have to wait
                // 'a reasonable amount of time' until the buttons are re-enabled.
                WaitingAssert.IsTrue(
                    window.AreButtonsEnabled,
                    "The window buttons were not re-enabled in a reasonable time.");

                Assert.IsEmpty(
                    window.GetText(),
                    "The window's text should be empty after the buttons are re-enabled.");

                Assert.AreEqual(
                    1, window.GetItemsInListCount(),
                    "There should be exactly 1 item after adding 'Hello'.");

                Assert.AreEqual(
                    "Hello", window.GetItemInListAt(0),
                    "The item 0 in the list does not match the expected one.");

                window.ChangeText("Goodbye");

                window.ClickAddButton();

                WaitingAssert.AreEqual(
                    "Adding element Goodbye...",
                    window.GetProgressMessage,
                    "The progress message does not match the expected one " +
                        "while adding 'Goodbye'.");

                WaitingAssert.IsTrue(
                    window.AreButtonsEnabled,
                    "The window buttons were not re-enabled in a reasonable " +
                        "time after adding 'Goodbye'.");

                Assert.IsEmpty(
                    window.GetText(),
                    "The window's text should be empty after the buttons are re-enabled.");

                Assert.AreEqual(
                    2, window.GetItemsInListCount(),
                    "There should be exactly 2 items after adding 'Goodbye'.");

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

                Assert.IsEmpty(
                    window.GetText(),
                    "The window's text should be empty after the buttons are re-enabled.");

                Assert.AreEqual(
                    1, window.GetItemsInListCount(),
                    "There should be exactly 1 item after removing 'Hello'.");

                Assert.AreEqual(
                    "Goodbye", window.GetItemInListAt(0),
                    "The item 0 in the list does not match the expected one.");

                window.ClickAddButton();

                Assert.AreEqual(
                    "The element can't be empty!", window.GetErrorMessage(),
                    "The error message does not match the expected one while "
                        + "adding an empty element.");
            }
            catch (Exception ex)
            {
                mLog.ErrorFormat(ex.Message);
                mLog.ErrorFormat(ex.StackTrace);
            }
            finally
            {
                // here you'd do your cleanup!
            }
        }

        static readonly ILog mLog = LogManager.GetLogger("RunTest");
    }
}
