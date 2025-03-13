using System;

namespace Aspirate.Tests.ActionsTests;

public abstract class BaseActionTests<TSystemUnderTest> : AspirateTestBase
    where TSystemUnderTest : class, IAction
{
   protected TSystemUnderTest? GetSystemUnderTest(IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredKeyedService<IAction>(typeof(TSystemUnderTest).Name) as TSystemUnderTest;
}
