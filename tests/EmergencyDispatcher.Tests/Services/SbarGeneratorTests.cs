using EmergencyDispatcher.Web.Services;

namespace EmergencyDispatcher.Tests.Services;

public class SbarGeneratorTests
{
    private readonly SbarGenerator _sut;

    public SbarGeneratorTests()
    {
        _sut = new SbarGenerator(TestHelpers.CreateMockConfig());
    }

    [Fact]
    public void GenerateCallScript_WithMember_IncludesAllergies()
    {
        var testCase = TestHelpers.CreateTestCase();
        var member = TestHelpers.CreateTestMember();
        var dispatcher = TestHelpers.CreateTestUser();

        var result = _sut.GenerateCallScript(testCase, member, dispatcher);

        Assert.Contains("Penicillin", result);
    }

    [Fact]
    public void GenerateCallScript_WithoutMember_ShowsNoneReported()
    {
        var testCase = TestHelpers.CreateTestCase();
        var dispatcher = TestHelpers.CreateTestUser();

        var result = _sut.GenerateCallScript(testCase, null, dispatcher);

        Assert.Contains("None reported", result);
    }

    [Fact]
    public void GenerateCallScript_IncludesDispatcherName()
    {
        var testCase = TestHelpers.CreateTestCase();
        var dispatcher = TestHelpers.CreateTestUser();

        var result = _sut.GenerateCallScript(testCase, null, dispatcher);

        Assert.Contains("Test Dispatcher", result);
    }

    [Fact]
    public void GenerateCallScript_IncludesServiceName()
    {
        var testCase = TestHelpers.CreateTestCase();
        var dispatcher = TestHelpers.CreateTestUser();

        var result = _sut.GenerateCallScript(testCase, null, dispatcher);

        Assert.Contains("Test Dispatch Service", result);
    }

    [Fact]
    public void GenerateCallScript_IncludesPatientInfo()
    {
        var testCase = TestHelpers.CreateTestCase();
        var dispatcher = TestHelpers.CreateTestUser();

        var result = _sut.GenerateCallScript(testCase, null, dispatcher);

        Assert.Contains("Juan Dela Cruz", result);
        Assert.Contains("38", result);
        Assert.Contains("Male", result);
        Assert.Contains("Chest Pain", result);
    }

    [Fact]
    public void GenerateCallScript_WithMember_IncludesMedications()
    {
        var testCase = TestHelpers.CreateTestCase();
        var member = TestHelpers.CreateTestMember();
        var dispatcher = TestHelpers.CreateTestUser();

        var result = _sut.GenerateCallScript(testCase, member, dispatcher);

        Assert.Contains("Metformin 500mg", result);
    }

    [Fact]
    public void GenerateCallScript_WithMember_IncludesConditions()
    {
        var testCase = TestHelpers.CreateTestCase();
        var member = TestHelpers.CreateTestMember();
        var dispatcher = TestHelpers.CreateTestUser();

        var result = _sut.GenerateCallScript(testCase, member, dispatcher);

        Assert.Contains("Type 2 Diabetes", result);
    }

    [Fact]
    public void GenerateMessage_IncludesPatientNameAndEta()
    {
        var testCase = TestHelpers.CreateTestCase();
        var dispatcher = TestHelpers.CreateTestUser();

        var result = _sut.GenerateMessage(testCase, null, dispatcher);

        Assert.Contains("Juan Dela Cruz", result);
        Assert.Contains("15", result);
    }

    [Fact]
    public void GenerateMessage_IncludesSbarStructure()
    {
        var testCase = TestHelpers.CreateTestCase();
        var dispatcher = TestHelpers.CreateTestUser();

        var result = _sut.GenerateMessage(testCase, null, dispatcher);

        Assert.Contains("SBAR PRE-NOTIFICATION", result);
        Assert.Contains("S:", result);
        Assert.Contains("B:", result);
        Assert.Contains("A:", result);
        Assert.Contains("R:", result);
    }

    [Fact]
    public void GenerateEmail_IncludesSubjectLine()
    {
        var testCase = TestHelpers.CreateTestCase();
        var dispatcher = TestHelpers.CreateTestUser();

        var result = _sut.GenerateEmail(testCase, null, dispatcher);

        Assert.Contains("Subject: SBAR Pre-Notification", result);
    }

    [Fact]
    public void GenerateEmail_IncludesAllSections()
    {
        var testCase = TestHelpers.CreateTestCase();
        var member = TestHelpers.CreateTestMember();
        var dispatcher = TestHelpers.CreateTestUser();

        var result = _sut.GenerateEmail(testCase, member, dispatcher);

        Assert.Contains("S —", result);
        Assert.Contains("B —", result);
        Assert.Contains("A —", result);
        Assert.Contains("R —", result);
        Assert.Contains("Penicillin", result);
    }

    [Fact]
    public void GenerateCallScript_NullTransport_ShowsUnknown()
    {
        var testCase = TestHelpers.CreateTestCase();
        testCase.TransportMethod = null;
        var dispatcher = TestHelpers.CreateTestUser();

        var result = _sut.GenerateCallScript(testCase, null, dispatcher);

        Assert.Contains("Unknown", result);
    }

    [Fact]
    public void GenerateCallScript_NullLocation_ShowsUnknown()
    {
        var testCase = TestHelpers.CreateTestCase();
        testCase.LocationText = null;
        var dispatcher = TestHelpers.CreateTestUser();

        var result = _sut.GenerateCallScript(testCase, null, dispatcher);

        Assert.Contains("Unknown", result);
    }
}
