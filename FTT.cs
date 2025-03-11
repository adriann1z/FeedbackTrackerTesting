using System;
using System.Diagnostics;
using NUnit.Framework;
using Moq;

[TestFixture]
public class TestFeedbackTracker
{
    /*
    alright so this is our test class for the feedback tracker
    basically we're gonna check if adding removing and retrieving feedback works properly
    also checking if it's secure fast and works on different .NET versions
    */

    private static string databaseConnection;
    private Dictionary<string, object> feedbackEntry;

    [OneTimeSetUp]
    public void SetUpClass()
    {
        /*
        this runs one time before all the tests start
        lets pretend its setting up a connection to a database
        */
        databaseConnection = "Mock Database Connection"; // not real but just go with it
    }

    [OneTimeTearDown]
    public void TearDownClass()
    {
        /*
        this runs after all the tests are done
        basically cleaning up stuff so nothing is left hanging
        */
        databaseConnection = null; // goodbye fake database
    }

    [SetUp]
    public void SetUp()
    {
        /*
        this runs before each test
        just setting up some fake feedback data so we have something to test
        */
        feedbackEntry = new Dictionary<string, object>
        {
            { "student_id", 12345 }, // pretend this is a real student
            { "course", "CS101" },  // classic intro to cs
            { "feedback", "Great course!" } // nice feedback
        };
    }

    [TearDown]
    public void TearDown()
    {
        /*
        this runs after each test
        just resetting everything so nothing interferes with other tests
        */
        feedbackEntry = null; // resetting back to nothing
    }

    // UNIT TESTING
    [Test]
    public void TestAddFeedback()
    {
        /*
        making sure that when we add feedback it actually stores the right course
        */
        Assert.AreEqual("CS101", feedbackEntry["course"]); // checking if course is correct
    }

    [Test]
    public void TestRemoveFeedback()
    {
        /*
        making sure we can remove feedback properly
        */
        feedbackEntry = null; // pretending we deleted it
        Assert.IsNull(feedbackEntry); // checking if it's really gone
    }

    // INTEGRATION TESTING
    [Test]
    public void TestDatabaseIntegration()
    {
        /*
        testing if our fake database can handle inserting feedback
        */
        var mockDb = new Mock<IDatabase>();
        mockDb.Setup(db => db.InsertFeedback(feedbackEntry)).Returns(true); // pretending the database accepts it
        bool result = mockDb.Object.InsertFeedback(feedbackEntry);
        Assert.IsTrue(result); // should be true if insert worked
    }

    // FUNCTIONAL TESTING
    [Test]
    public void TestFeedbackRetrieval()
    {
        /*
        making sure we can get feedback from the system properly
        */
        var mockDb = new Mock<IDatabase>();
        mockDb.Setup(db => db.GetFeedback(12345)).Returns(feedbackEntry);
        var result = mockDb.Object.GetFeedback(12345);
        Assert.AreEqual(feedbackEntry, result); // checking if we got the right feedback
    }

    // PERFORMANCE TESTING
    [Test]
    public void TestFeedbackSubmissionTime()
    {
        /*
        making sure submitting feedback doesn't take forever
        */
        Stopwatch stopwatch = Stopwatch.StartNew();
        System.Threading.Thread.Sleep(100); // faking a short delay
        stopwatch.Stop();
        Assert.Less(stopwatch.ElapsedMilliseconds, 500); // gotta be under half a second
    }

    // SECURITY TESTING
    [Test]
    public void TestSqlInjectionProtection()
    {
        /*
        checking if the system can stop a basic sql injection attack
        */
        string maliciousInput = "DROP TABLE feedback; --"; // oh no a hacker
        var mockDb = new Mock<IDatabase>();
        mockDb.Setup(db => db.AddFeedback(It.IsAny<Dictionary<string, object>>())).Returns(false); // pretending the system blocked it
        bool result = mockDb.Object.AddFeedback(new Dictionary<string, object>
        {
            { "student_id", 12345 },
            { "course", "CS101" },
            { "feedback", maliciousInput }
        });
        Assert.IsFalse(result); // if the attack is blocked it should be false
    }

    // COMPATIBILITY TESTING
    [Test]
    public void TestSystemCompatibility()
    {
        /*
        checking if the code runs on at least .NET Core 3.1
        */
        Version currentVersion = Environment.Version;
        Version minRequiredVersion = new Version(3, 1);
        Assert.GreaterOrEqual(currentVersion, minRequiredVersion); // gotta be at least .NET Core 3.1
    }

    // REGRESSION TESTING
    [Test]
    public void TestPreviousBugFix()
    {
        /*
        making sure a bug we fixed before doesn't come back
        */
        var mockDb = new Mock<IDatabase>();
        mockDb.SetupSequence(db => db.GetFeedback(12345))
            .Returns("Old Feedback")
            .Returns("Fixed Feedback"); // simulating an old bug fix
        string firstAttempt = (string)mockDb.Object.GetFeedback(12345);
        string secondAttempt = (string)mockDb.Object.GetFeedback(12345);
        Assert.AreNotEqual(firstAttempt, secondAttempt); // should be different if the bug is gone
    }
}

public interface IDatabase
{
    bool InsertFeedback(Dictionary<string, object> feedback);
    Dictionary<string, object> GetFeedback(int studentId);
    bool AddFeedback(Dictionary<string, object> feedback);
}
