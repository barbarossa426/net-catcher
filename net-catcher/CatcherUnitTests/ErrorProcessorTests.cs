using CatchSubscriber;
using CatchSubscriber.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace CatcherUnitTests;

public class ErrorProcessorTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task ShouldProcessUsingDefaultAction()
    {
        ErrorProcessor ProcessError = new();
        LogLevel expectedLogLevel = LogLevel.Debug;

        await ProcessError.ProcessError("This usnig defalut action to log a message", expectedLogLevel);
    }

    [Test]
    public async Task ShouldProcessMessageUsingMultiplyActions()
    {
        ErrorProcessor ProcessError = new();

        await ProcessError.ProcessError("This is my log message", LogLevel.Critical, CatchAction.Azure, CatchAction.Console);
    }

    [Test]
    public void ShouldProcessMessageUsingAzureDiagnostics()
    {
        //Given
        ErrorProcessor ProcessError = new();
        LogLevel expectedLogLevel = LogLevel.Critical;

        //When
        Func<Task> outcome = async () => await ProcessError.ProcessError("This is my message for azure diagnostics", expectedLogLevel, CatchAction.Azure);
        Action act = () => outcome();

        //Then
        act.Should().NotThrow();
    }

    [Test]
    public void ShouldProcessMessageUsingAWSXRay()
    {
        //Given
        ErrorProcessor ProcessError = new();
        LogLevel expectedLogLevel = LogLevel.Critical;

        //When
        Func<Task> outcome = async () => await ProcessError.ProcessError("This is my message for AWSXRay", expectedLogLevel, CatchAction.AWS);
        Action act = () => outcome();

        //Then
        act.Should().NotThrow();
    }

    [Test]
    public void ShouldProcessMessageUsingSlackWebbHook()
    {
        //Given
        ErrorProcessor ProcessError = new();
        ProcessError.RegisterSlack("https://hooks.slack.com/services/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", "Testing", "ThisApplication");

        var expectedActions = CatchAction.Slack;

        //When
        Func<Task> outcome = async () => await ProcessError.ProcessError("This is a test message", LogLevel.Critical, expectedActions);
        Action act = () => outcome();

        //Then
        act.Should().NotThrow();
    }

    [Test]
    public void ShouldRegisterEmail()
    {
        //Given
        ErrorProcessor ProcessError = new();

        //When
        Func<ErrorProcessor> outcome = () => ProcessError.RegisterEmail();
        Action act = () => outcome();

        //Then
        act.Should().NotThrow();
    }
}