using atFrameWork2.BaseFramework; // For Waiters
using atFrameWork2.SeleniumFramework;
using atFrameWork2.TestEntities; // For User
using OpenQA.Selenium;
using System; // For DateTime
using System.Collections.Generic; // For List
using System.Globalization; // For CultureInfo

namespace ATframework3demo.PageObjects
{
    public class EventFormPage
    {
        // Locators - these are examples and will likely need adjustment
        private WebItem EventNameInput() => new WebItem("//input[@name='name']", "Поле ввода имени события");
        private WebItem EventDescriptionInput() => new WebItem("//textarea[@name='description']", "Поле ввода описания события"); // This might be inside an iframe like other editors
        private WebItem EventDatePickerStart() => new WebItem("//input[@name='date_from']", "Поле выбора даты начала события");
        private WebItem EventTimePickerStart() => new WebItem("//input[@name='time_from']", "Поле выбора времени начала события");
        private WebItem AddParticipantLink() => new WebItem("//span[contains(@class, 'ui-tag-selector-add-button')]", "Ссылка 'Добавить участников'");
        private WebItem ParticipantSearchInput() => new WebItem("//input[contains(@class,'ui-tag-selector-text-box')]", "Поле поиска участника"); // Common Bitrix24 finder input
        private WebItem SaveEventButton() => new WebItem("//button[contains(@class, 'ui-btn-success') and (contains(text(), 'Сохранить') or contains(text(), 'Save'))]", "Кнопка сохранения события");
        private WebItem EventEditorFrame() => new WebItem("//iframe[contains(@class, 'bx-editor-iframe')]", "Фрейм редактора описания события"); // Common editor iframe

        public EventFormPage(IWebDriver driver = default)
        {
            Driver = driver ?? BaseItem.DefaultDriver;
            // Wait for the main form/slider to be visible
            new WebItem("//div[contains(@class, 'calendar-slider-container') or contains(@class, 'side-panel-content-container')]", "Контейнер формы события").WaitElementDisplayed(5, Driver);
        }

        public IWebDriver Driver { get; }

        public EventFormPage SetEventName(string name)
        {
            EventNameInput().SendKeys(name, Driver);
            return this;
        }

        public EventFormPage SetEventDescription(string description)
        {
            // Bitrix editors are often in iframes
            var editorFrame = EventEditorFrame();
            bool switchedToFrame = false;
            if (editorFrame.WaitElementDisplayed(2, Driver))
            {
                editorFrame.SwitchToFrame(Driver);
                switchedToFrame = true;
            }

            // Assuming description is a simple textarea if no iframe, or body if in iframe
            var bodyInput = new WebItem(switchedToFrame ? "//body[@contenteditable='true']" : "//textarea[@name='description']", "Поле ввода описания события");
            bodyInput.SendKeys(description, Driver);

            if (switchedToFrame)
            {
                WebDriverActions.SwitchToDefaultContent(Driver);
            }
            return this;
        }

        public EventFormPage SetStartDate(DateTime date)
        {
            // Date format might vary based on portal settings. Assuming DD.MM.YYYY
            //EventDatePickerStart().Clear(Driver); // Clear existing date
            EventDatePickerStart().SendKeys(date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture), Driver, clearBefore: true);
            // Clicking somewhere to close datepicker if it's open
            new WebItem("//body", "Body element").Click(Driver);
            return this;
        }

        public EventFormPage SetStartTime(DateTime time)
        {
            // Time format might vary. Assuming HH:mm
            //EventTimePickerStart().Clear(Driver); // Clear existing time
            EventTimePickerStart().SendKeys(time.ToString("HH:mm"), Driver, clearBefore: true);
            // Clicking somewhere to close timepicker if it's open
            new WebItem("//body", "Body element").Click(Driver);
            return this;
        }

        public EventFormPage AddParticipant(User user)
        {
            AddParticipantLink().Click(Driver);
            // Wait for participant selection dialog/area to appear
            ParticipantSearchInput().WaitElementDisplayed(5, Driver);
            ParticipantSearchInput().SendKeys(user.NameLastName, Driver); // Corrected to use NameLastName

            // Wait for search results and click the correct user
            // This locator needs to be specific to how users are listed in search results
            var userInSearchResults = new WebItem($"//div[@class='ui-selector-item-title']//span[contains(text(), '{user.LastName}')]",
                $"Пользователь '{user.NameLastName}' в результатах поиска"); // Corrected to use NameLastName
            userInSearchResults.WaitElementDisplayed(10, Driver);
            userInSearchResults.Click(Driver);

            // Close the participant selection dialog if necessary
            // Sometimes there's an explicit "Select" button or clicking outside closes it
            // For now, assuming clicking the user is enough or the dialog closes automatically.
            // If not, add: new WebItem("//button[contains(text(),'Выбрать') or contains(@class,'ui-selector-select-button')]", "Кнопка Выбрать участника").Click(Driver);

            return this;
        }

        public CalendarPage SaveEvent()
        {
            SaveEventButton().Click(Driver);
            // Wait for slider/form to close, might need a more robust wait here
            Waiters.WaitForCondition(() => !SaveEventButton().WaitElementDisplayed(driver:Driver), timeout_s: 10, retryInterval_s:1, waitDescription:"Ожидание закрытия формы события");
            return new CalendarPage(Driver);
        }
    }
}
