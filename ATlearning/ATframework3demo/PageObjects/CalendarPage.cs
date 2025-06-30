using atFrameWork2.SeleniumFramework;
using OpenQA.Selenium;
using System.Collections.Generic; // For participant lists in future methods

namespace ATframework3demo.PageObjects
{
    public class CalendarPage
    {
        private WebItem AddEventButton() => new WebItem("//button[contains(@class, 'ui-btn-main')]/span[contains(text(),'Add') or contains(text(),'Создать')]", "Кнопка добавления события в календаре");
        // More specific locators might be needed depending on the Bitrix24 version/customization

        public CalendarPage(IWebDriver driver = default)
        {
            Driver = driver ?? BaseItem.DefaultDriver;
        }

        public IWebDriver Driver { get; }

        public EventFormPage ClickAddEventButton()
        {
            AddEventButton().Click(Driver);
            // Wait for the event form to potentially appear/load if it's a slider or dynamic
            // For now, directly returning, assuming form appears promptly
            return new EventFormPage(Driver);
        }

        /// <summary>
        /// Placeholder for verifying event visibility.
        /// Implementation will depend on how events are rendered in the calendar view.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="participants"></param>
        /// <returns></returns>
        public bool IsEventVisible(string eventName, List<string> participants = null, int timeoutSeconds = 10)
        {
            // Example: "//div[contains(@class, 'calendar-event-title') and contains(text(), '{eventName}')]"
            // This will need to be more robust, potentially checking date/time and participant details if visible directly on the event block
            var eventItem = new WebItem($"//span[contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), \"{eventName.ToLower()}\")]",
                $"Событие в календаре с именем '{eventName}'");

            if (!eventItem.WaitElementDisplayed(timeoutSeconds, Driver))
                return false;

            // Further verification for participants might be needed here if they are displayed directly on the calendar view
            // For now, just checking title visibility
            return true;
        }
    }
}
