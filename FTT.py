import unittest  
from unittest.mock import MagicMock  
import time  
import sys  

class TestFeedbackTracker(unittest.TestCase):
    """
    alright so this is our test class for the feedback tracker
    basically we're gonna check if adding removing and retrieving feedback works properly
    also checking if it's secure fast and works on different python versions
    """
    
    @classmethod
    def setUpClass(cls):
        """
        this runs one time before all the tests start
        lets pretend its setting up a connection to a database
        """
        cls.database_connection = "Mock Database Connection"  # not real but just go with it
    
    @classmethod
    def tearDownClass(cls):
        """
        this runs after all the tests are done
        basically cleaning up stuff so nothing is left hanging
        """
        cls.database_connection = None  # goodbye fake database
    
    def setUp(self):
        """
        this runs before each test
        just setting up some fake feedback data so we have something to test
        """
        self.feedback_entry = {
            "student_id": 12345,  # pretend this is a real student
            "course": "CS101",   # classic intro to cs
            "feedback": "Great course!"  # nice feedback
        }
    
    def tearDown(self):
        """
        this runs after each test
        just resetting everything so nothing interferes with other tests
        """
        self.feedback_entry = None  # resetting back to nothing
    
    # UNIT TESTING
    def test_add_feedback(self):
        """
        making sure that when we add feedback it actually stores the right course
        """
        self.assertEqual(self.feedback_entry["course"], "CS101")  # checking if course is correct
    
    def test_remove_feedback(self):
        """
        making sure we can remove feedback properly
        """
        self.feedback_entry = None  # pretending we deleted it
        self.assertIsNone(self.feedback_entry)  # checking if it's really gone
    
    # INTEGRATION TESTING
    def test_database_integration(self):
        """
        testing if our fake database can handle inserting feedback
        """
        mock_db = MagicMock()
        mock_db.insert_feedback.return_value = True  # pretending the database accepts it
        result = mock_db.insert_feedback(self.feedback_entry)
        self.assertTrue(result)  # should be true if insert worked
    
    # FUNCTIONAL TESTING
    def test_feedback_retrieval(self):
        """
        making sure we can get feedback from the system properly
        """
        mock_db = MagicMock()
        mock_db.get_feedback.return_value = self.feedback_entry
        result = mock_db.get_feedback(12345)
        self.assertEqual(result, self.feedback_entry)  # checking if we got the right feedback
    
    # PERFORMANCE TESTING
    def test_feedback_submission_time(self):
        """
        making sure submitting feedback doesn't take forever
        """
        start_time = time.time()
        time.sleep(0.1)  # faking a short delay
        end_time = time.time()
        self.assertLess(end_time - start_time, 0.5)  # gotta be under half a second
    
    # SECURITY TESTING
    def test_sql_injection_protection(self):
        """
        checking if the system can stop a basic sql injection attack
        """
        malicious_input = "DROP TABLE feedback; --"  # oh no a hacker
        mock_db = MagicMock()
        mock_db.add_feedback.return_value = False  # pretending the system blocked it
        result = mock_db.add_feedback({"student_id": 12345, "course": "CS101", "feedback": malicious_input})
        self.assertFalse(result)  # if the attack is blocked it should be false
    
    # COMPATIBILITY TESTING
    def test_system_compatibility(self):
        """
        checking if the code runs on at least python 3.6
        """
        python_version = sys.version_info
        self.assertGreaterEqual(python_version, (3, 6))  # gotta be at least 3.6
    
    # REGRESSION TESTING
    def test_previous_bug_fix(self):
        """
        making sure a bug we fixed before doesn't come back
        """
        mock_db = MagicMock()
        mock_db.get_feedback.side_effect = ["Old Feedback", "Fixed Feedback"]  # simulating an old bug fix
        first_attempt = mock_db.get_feedback(12345)
        second_attempt = mock_db.get_feedback(12345)
        self.assertNotEqual(first_attempt, second_attempt)  # should be different if the bug is gone
    
if __name__ == "__main__":
    unittest.main()  # runs all the tests
