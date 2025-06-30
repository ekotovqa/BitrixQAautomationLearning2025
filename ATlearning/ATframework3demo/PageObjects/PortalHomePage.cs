using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace atFrameWork2.PageObjects
{
    public class PortalHomePage
    {
        public IWebDriver Driver { get; }

        public PortalHomePage(IWebDriver driver = default)
        {
            Driver = driver;
        }

        public PortalLeftMenu LeftMenu => new PortalLeftMenu(Driver);

        /// <summary>
        /// Logs out the current user and returns to the login page.
        /// </summary>
        /// <returns>New instance of PortalLoginPage</returns>
        public PortalLoginPage Logout()
        {
            // Locator for the user profile block/name in the header
            var userProfileBlock = new SeleniumFramework.WebItem("//div[@id='user-block']", "User profile block in header");
            userProfileBlock.Click(Driver);

            // Locator for the "Выход" (Logout) link in the dropdown menu
            var logoutButton = new SeleniumFramework.WebItem("//a[@class='system-auth-form__item-link-all']", "Logout button");
            logoutButton.WaitElementDisplayed(5, Driver); // Wait for dropdown to appear
            logoutButton.Click(Driver);

            // Wait for the login page to be indicative, e.g., by checking for a login form element
            var loginFormElement = new SeleniumFramework.WebItem("//form[@class='login-form']", "Login form indicator");
            loginFormElement.WaitElementDisplayed(10, Driver);

            // Access PortalInfo from the currently running test case
            var portalInfo = atFrameWork2.BaseFramework.TestCase.RunningTestCase?.TestPortal;
            if (portalInfo == null)
            {
                // This case should ideally not happen if Logout is called during a valid test run
                // Or, throw an exception if PortalInfo is absolutely critical for PortalLoginPage initialization for subsequent actions.
                // For now, let's log an error and proceed, though PortalLoginPage might not be fully functional.
                atFrameWork2.BaseFramework.LogTools.Log.Error("PortalInfo not available in TestCase.RunningTestCase.TestPortal during Logout. PortalLoginPage might not be correctly initialized.");
                // Depending on PortalLoginPage's constructor, this might need a more robust fallback or an exception.
                // Assuming PortalLoginPage can handle a null PortalInfo or has another constructor.
                // Given its current constructor `public PortalLoginPage(PortalInfo portalInfo, IWebDriver driver = null)`
                // we MUST provide it. If not available, it's a critical issue.
                throw new InvalidOperationException("Cannot initialize PortalLoginPage after logout: PortalInfo is missing from TestCase.RunningTestCase.");
            }

            return new PortalLoginPage(portalInfo, Driver);
        }
    }
}
