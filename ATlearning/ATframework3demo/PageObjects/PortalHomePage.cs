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
            Driver = driver ?? SeleniumFramework.BaseItem.GetDefaultDriver();
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
            var logoutButton = new SeleniumFramework.WebItem("//a[@class='menu-popup-item-text' and contains(text(),'Выход') or contains(text(),'Log out')]", "Logout button");
            logoutButton.WaitElementDisplayed(5, Driver); // Wait for dropdown to appear
            logoutButton.Click(Driver);

            // Wait for the login page to be indicative, e.g., by checking for a login form element
            var loginFormElement = new SeleniumFramework.WebItem("//form[@class='login-form']", "Login form indicator");
            loginFormElement.WaitElementDisplayed(10, Driver);

            return new PortalLoginPage(Driver);
        }
    }
}
