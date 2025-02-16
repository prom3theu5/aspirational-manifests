using System;

namespace Aspirate.Tests.ServiceTests;

public abstract class BaseServiceTests<TSystemUnderTest> : AspirateTestBase
    where TSystemUnderTest : class
{
    protected TSystemUnderTest GetSystemUnderTest(IServiceProvider serviceProvider) => serviceProvider.GetRequiredService<TSystemUnderTest>();
}
