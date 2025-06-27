using atFrameWork2.BaseFramework;
using atFrameWork2.PageObjects; // For PortalHomePage
using atFrameWork2.TestEntities; // For User
using System;
using System.Collections.Generic;

namespace ATframework3demo.TestCases
{
    public class Case_Bitrix24_Calendar : CaseCollectionBuilder
    {
        protected override List<TestCase> GetCases()
        {
            var cases = new List<TestCase>
            {
                new TestCase("Проверка создания события календаря с новым участником", homePage => CreateEventWithNewUserAndVerify(homePage))
            };
            return cases;
        }

        public void CreateEventWithNewUserAndVerify(PortalHomePage homePage)
        {
            // Step 3a: User Creation
            User newUser = null;
            try
            {
                // Create a new intranet user.
                // TestCase.RunningTestCase is available because this method is executed by the TestCase instance.
                newUser = TestCase.RunningTestCase.CreatePortalTestUser(false);
                atFrameWork2.BaseFramework.LogTools.Log.Info($"Successfully created new user: {newUser.LoginAkaEmail} / {newUser.Password}");
            }
            catch (Exception e)
            {
                // CreatePortalTestUser throws an exception if on cloud or if user creation fails.
                // This will be caught by the TestCase's main execution block and mark the test as failed.
                atFrameWork2.BaseFramework.LogTools.Log.Error($"Failed to create new user: {e.Message}");
                throw; // Re-throw to ensure test fails if user creation is critical
            }

            // Step 4b, 5: Navigate to Calendar and Create New Event (as Admin)
            CalendarPage calendarPage = null;
            EventFormPage eventFormPage = null;
            string uniqueEventName = $"Test Event {DateTime.Now.Ticks}";
            DateTime eventStartDateTime = DateTime.Now.AddDays(1).AddHours(2); // Example: tomorrow, 2 hours from now

            try
            {
                atFrameWork2.BaseFramework.LogTools.Log.Info("Navigating to Calendar page...");
                calendarPage = homePage.LeftMenu.OpenCalendar();

                atFrameWork2.BaseFramework.LogTools.Log.Info("Clicking 'Add Event' button...");
                eventFormPage = calendarPage.ClickAddEventButton();

                atFrameWork2.BaseFramework.LogTools.Log.Info($"Setting event name: {uniqueEventName}");
                eventFormPage.SetEventName(uniqueEventName);

                atFrameWork2.BaseFramework.LogTools.Log.Info("Setting event description...");
                eventFormPage.SetEventDescription("This is a test event description created by an automated test.");

                atFrameWork2.BaseFramework.LogTools.Log.Info($"Setting event start date to: {eventStartDateTime:dd.MM.yyyy}");
                eventFormPage.SetStartDate(eventStartDateTime);

                atFrameWork2.BaseFramework.LogTools.Log.Info($"Setting event start time to: {eventStartDateTime:HH:mm}");
                eventFormPage.SetStartTime(eventStartDateTime);

                atFrameWork2.BaseFramework.LogTools.Log.Info($"Adding participant: {newUser.FirstName} {newUser.LastName}");
                eventFormPage.AddParticipant(newUser);

                atFrameWork2.BaseFramework.LogTools.Log.Info("Saving event...");
                calendarPage = eventFormPage.SaveEvent(); // SaveEvent returns CalendarPage
                atFrameWork2.BaseFramework.LogTools.Log.Info("Event saved successfully from admin context.");
            }
            catch (Exception e)
            {
                atFrameWork2.BaseFramework.LogTools.Log.Error($"Error during event creation (Admin Context): {e}");
                throw; // Re-throw to ensure test fails
            }

            // Step 6: Verify Event (Admin Context)
            try
            {
                atFrameWork2.BaseFramework.LogTools.Log.Info($"Verifying event '{uniqueEventName}' is visible on the calendar for admin...");
                // For now, IsEventVisible in CalendarPage only checks name.
                // We might enhance it later to check participants if the view allows.
                bool isAdminEventVisible = calendarPage.IsEventVisible(uniqueEventName, new List<string> { newUser.FirstName + " " + newUser.LastName });

                if (isAdminEventVisible)
                {
                    atFrameWork2.BaseFramework.LogTools.Log.Info($"Event '{uniqueEventName}' is visible for admin.");
                }
                else
                {
                    atFrameWork2.BaseFramework.LogTools.Log.Error($"Event '{uniqueEventName}' NOT visible for admin.");
                    throw new Exception($"Admin verification failed: Event '{uniqueEventName}' not found on calendar.");
                }
            }
            catch (Exception e)
            {
                atFrameWork2.BaseFramework.LogTools.Log.Error($"Error during event verification (Admin Context): {e}");
                throw; // Re-throw to ensure test fails
            }

            // Step 7: Logout Admin and Login as New User
            PortalLoginPage loginPage = null;
            PortalHomePage newUserHomePage = null;
            try
            {
                atFrameWork2.BaseFramework.LogTools.Log.Info("Logging out admin user...");
                // homePage is the PortalHomePage instance for the admin user
                loginPage = homePage.Logout();
                atFrameWork2.BaseFramework.LogTools.Log.Info("Admin user logged out successfully.");

                atFrameWork2.BaseFramework.LogTools.Log.Info($"Logging in as new user: {newUser.LoginAkaEmail}...");
                newUserHomePage = loginPage.Login(newUser); // Use the created newUser object
                atFrameWork2.BaseFramework.LogTools.Log.Info($"Successfully logged in as new user: {newUser.LoginAkaEmail}.");
            }
            catch (Exception e)
            {
                atFrameWork2.BaseFramework.LogTools.Log.Error($"Error during logout/login as new user: {e}");
                throw; // Re-throw to ensure test fails
            }

            // Step 8: Verify Event (New User Context)
            CalendarPage newUserCalendarPage = null;
            try
            {
                atFrameWork2.BaseFramework.LogTools.Log.Info($"Navigating to Calendar page for new user: {newUser.LoginAkaEmail}...");
                newUserCalendarPage = newUserHomePage.LeftMenu.OpenCalendar();

                atFrameWork2.BaseFramework.LogTools.Log.Info($"Verifying event '{uniqueEventName}' is visible on the calendar for new user...");
                bool isNewUserEventVisible = newUserCalendarPage.IsEventVisible(uniqueEventName); // Not passing participants here, as it's the user's own calendar view

                if (isNewUserEventVisible)
                {
                    atFrameWork2.BaseFramework.LogTools.Log.Info($"Event '{uniqueEventName}' is visible for new user: {newUser.LoginAkaEmail}.");
                }
                else
                {
                    atFrameWork2.BaseFramework.LogTools.Log.Error($"Event '{uniqueEventName}' NOT visible for new user: {newUser.LoginAkaEmail}.");
                    throw new Exception($"New user verification failed: Event '{uniqueEventName}' not found on calendar for user {newUser.LoginAkaEmail}.");
                }
            }
            catch (Exception e)
            {
                atFrameWork2.BaseFramework.LogTools.Log.Error($"Error during event verification (New User Context): {e}");
                throw; // Re-throw to ensure test fails
            }

            atFrameWork2.BaseFramework.LogTools.Log.Info("Calendar event creation and verification with new user completed successfully.");
        }
    }
}
