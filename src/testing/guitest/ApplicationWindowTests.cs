using System;

using log4net;
using NUnit.Framework;

using Codice.Examples.GuiTesting.GuiTestInterfaces;

namespace GuiTest
{
    [TestFixture]
    public class ApplicationWindowTests
    {
        [Test]
        public void BasicApplicationWindowTest()
        {
            TestOperations.RunTest(
                "BasicApplicationWindowTest",
                DoBasicApplicationWindowTest);
        }

        [Test]
        public void AddExistingElementTest()
        {
            TestOperations.RunTest(
                "AddExistingElementTest",
                DoAddExistingElementTest);
        }

        [Test]
        public void RemoveUnexistingElementTest()
        {
            TestOperations.RunTest(
                "RemoveUnexistingElementTest",
                DoRemoveUnexistingElementTest);
        }

        [Test]
        public void ThisTestWillFailAtTheEndTest()
        {
            TestOperations.RunTest(
                "ThisTestWillFailAtTheEndTest",
                DoThisTestWillFailAtTheEndTest);
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
                mLog.Error(ex.Message);
                mLog.Error(ex.StackTrace);
            }
            finally
            {
                // here you'd do your cleanup!
            }
        }

        void DoAddExistingElementTest(string testName)
        {
            try
            {
                ITesteableApplicationWindow window =
                    GuiTesteableServices.GetApplicationWindow();

                window.ChangeText("Hello");
                window.ClickAddButton();

                WaitingAssert.AreEqual(
                    "Adding element Hello...",
                    window.GetProgressMessage,
                    "The progress message does not match the expected one " +
                        "while adding 'Hello'.");

                WaitingAssert.IsTrue(
                    window.AreButtonsEnabled,
                    "The window buttons were not re-enabled in a reasonable time.");

                Assert.IsEmpty(
                    window.GetText(),
                    "The window's text should be empty after the buttons are re-enabled.");

                window.ChangeText("Hello");
                window.ClickAddButton();

                WaitingAssert.AreEqual(
                    "Adding element Hello...",
                    window.GetProgressMessage,
                    "The progress message does not match the expected one " +
                        "while adding 'Hello'.");

                // We don't know at which point the background operation will
                // end and the error message will show up, so we'll have to
                // wait for the error message to be different than null.
                WaitingAssert.IsNotNull(
                    window.GetErrorDialog,
                    "The error message did not show up in a reasonable time.");

                ITesteableErrorDialog dialog = window.GetErrorDialog();

                Assert.AreEqual(
                    "Error", dialog.GetTitle(),
                    "The error dialog's title does not match the expected one.");

                Assert.AreEqual(
                    "The element Hello is already in the list, and thus it can't be added!",
                    dialog.GetMessage(),
                    "The error dialog's text does not match the expected one.");

                dialog.ClickOkButton();
            }
            catch (Exception ex)
            {
                mLog.Error(ex.Message);
                mLog.Error(ex.StackTrace);
            }
        }

        void DoRemoveUnexistingElementTest(string testName)
        {
            try
            {
                ITesteableApplicationWindow window =
                    GuiTesteableServices.GetApplicationWindow();

                window.ChangeText("Hello");
                window.ClickRemoveButton();

                WaitingAssert.AreEqual(
                    "Removing element Hello...",
                    window.GetProgressMessage,
                    "The progress message did not match the expected one " +
                        "while removing 'Hello'.");

                WaitingAssert.IsNotNull(
                    window.GetErrorDialog,
                    "The error message did not show up in a reasonable time.");

                ITesteableErrorDialog dialog = window.GetErrorDialog();

                Assert.AreEqual(
                    "Error", dialog.GetTitle(),
                    "The error dialog's title does not match the expected one.");

                Assert.AreEqual(
                    "The element Hello is not in the list, and thus it can't be removed!",
                    dialog.GetMessage(),
                    "The error dialog's text does not match the expected one.");

                dialog.ClickOkButton();
            }
            catch (Exception ex)
            {
                mLog.Error(ex.Message);
                mLog.Error(ex.StackTrace);
            }
        }

        void DoThisTestWillFailAtTheEndTest(string testName)
        {
            try
            {
                ITesteableApplicationWindow window =
                    GuiTesteableServices.GetApplicationWindow();

                window.ChangeText("Hello");
                window.ClickAddButton();

                WaitingAssert.AreEqual(
                    "Adding element Hello...",
                    window.GetProgressMessage,
                    "The progress message does not match the expected one " +
                        "while adding 'Hello'.");

                WaitingAssert.IsTrue(
                    window.AreButtonsEnabled,
                    "The window buttons were not re-enabled in a reasonable time.");

                Assert.IsEmpty(
                    window.GetText(),
                    "The window's text should be empty after the buttons are re-enabled.");

                window.ChangeText("Hello");
                window.ClickAddButton();

                WaitingAssert.AreEqual(
                    "Adding element Hello...",
                    window.GetProgressMessage,
                    "The progress message does not match the expected one " +
                        "while adding 'Hello'.");

                WaitingAssert.IsTrue(
                    window.AreButtonsEnabled,
                    "The window buttons were not re-enabled in a reasonable time.");

                // This test is exactly the same as AddExistingElementTest,
                // but because we __didn't__ expect any dialog, we won't dismiss it.

                // WaitingAssert.IsNotNull(
                //     window.GetErrorDialog,
                //     "The error message did not show in a reasonable time.");

                // ITesteableErrorDialog dialog = window.GetErrorDialog();

                // Assert.AreEqual(
                //     "Error", dialog.GetTitle(),
                //     "The error dialog's title does not match the expected one.");

                // Assert.AreEqual(
                //     "The element Hello is already in the list, and thus it can't be added!",
                //     dialog.GetMessage(),
                //     "The error dialog's text does not match the expected one.");

                // dialog.ClickOkButton();
            }
            catch (Exception ex)
            {
                mLog.Error(ex.Message);
                mLog.Error(ex.StackTrace);
            }
        }

        static readonly ILog mLog = LogManager.GetLogger("RunTest");
    }
}
