using System;
using System.Collections.Generic;

using Codice.Examples.GuiTesting.Lib.Interfaces;
using Codice.Examples.GuiTesting.Lib.Threading;

namespace Codice.Examples.GuiTesting.Lib
{
    public class ApplicationOperations
    {
        public void AddElement(
            string element,
            IApplicationWindow window,
            IProgressControls progressControls)
        {
            if (string.IsNullOrEmpty(element))
            {
                progressControls.ShowError(
                    Localization.GetText(
                        Localization.Name.ElementCantBeEmptyErrorMessage));
                return;
            }

            progressControls.ShowProgress(
                Localization.GetText(
                    Localization.Name.AddingElementProgressText,
                    element));

            IThreadWaiter waiter = ThreadWaiter.GetWaiter();
            waiter.Execute(
                threadOperationDelegate: () =>
                {
                    DoHeavyWork();

                    if (mModel.Contains(element))
                        throw new Exception("The element is already in the list!");

                    mModel.Add(element);
                    mModel.Sort(StringComparer.Ordinal);
                },
                afterOperationDelegate: () =>
                {
                    progressControls.HideProgress();

                    if (waiter.Exception != null)
                    {
                        progressControls.ShowError(waiter.Exception.Message);
                        return;
                    }

                    window.UpdateItems(mModel);
                    window.ClearInput();
                });
        }

        public void RemoveElement(
            string element,
            IApplicationWindow window,
            IProgressControls progressControls)
        {
            if (string.IsNullOrEmpty(element))
            {
                progressControls.ShowError(
                    Localization.GetText(
                        Localization.Name.ElementCantBeEmptyErrorMessage));
                return;
            }

            progressControls.ShowProgress(
                Localization.GetText(
                    Localization.Name.RemovingElementProgressText,
                element));

            IThreadWaiter waiter = ThreadWaiter.GetWaiter();
            waiter.Execute(
                threadOperationDelegate: () =>
                {
                    DoHeavyWork();

                    if (!mModel.Contains(element))
                        throw new Exception("The element is not in the list!");

                    mModel.Remove(element);
                },
                afterOperationDelegate: () =>
                {
                    progressControls.HideProgress();

                    if (waiter.Exception != null)
                    {
                        progressControls.ShowError(waiter.Exception.Message);
                        return;
                    }

                    window.UpdateItems(mModel);
                    window.ClearInput();
                });
        }

        void DoHeavyWork()
        {
            System.Threading.Thread.Sleep(
                mRandom.Next(MIN_SLEEP_TIME, MAX_SLEEP_TIME));
        }

        readonly List<string> mModel = new List<string>();
        readonly Random mRandom = new Random(Environment.TickCount);

        const int MIN_SLEEP_TIME = 1500;
        const int MAX_SLEEP_TIME = 3000;
    }
}
