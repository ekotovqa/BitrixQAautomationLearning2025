using atFrameWork2.BaseFramework;
using atFrameWork2.SeleniumFramework;
using OpenQA.Selenium;

namespace ATframework3demo.PageObjects
{
    /// <summary>
    /// Форма добавления нового сообщения в новости
    /// </summary>
    public class NewsPostForm
    {
        // Обычно для таких форм используется iframe, в который нужно переключиться
        private const string PostEditorFrameId = "bx-editor-iframe"; // Пример ID, может отличаться
        private WebItem PostEditorFrame() => new WebItem($"//iframe[@class='{PostEditorFrameId}']", "Фрейм редактора поста");
        private WebItem PostBodyInput() => new WebItem("//body[@contenteditable='true']", "Поле ввода текста поста"); // Типичный локатор для contenteditable body в iframe
        private WebItem SendButton() => new WebItem("//span[@id='blog-submit-button-save']", "Кнопка 'Отправить'");

        public NewsPostForm(IWebDriver driver = default)
        {
            Driver = driver;
        }

        public IWebDriver Driver { get; }

        public NewsPostForm SetPostText(string text)
        {
            // Переключаемся во фрейм редактора, если он есть
            // Некоторые редакторы Bitrix24 используют iframe
            // Если фрейма нет для данного типа поста, эту часть нужно будет убрать или сделать условной
            var editorFrame = PostEditorFrame();
            bool switchedToFrame = false;
            if (editorFrame.WaitElementDisplayed(2, driver: Driver)) // Проверяем наличие фрейма с небольшим таймаутом
            {
                editorFrame.SwitchToFrame(Driver);
                switchedToFrame = true;
            }

            PostBodyInput().SendKeys(text, Driver);

            // Переключаемся обратно на основной контент страницы, если были во фрейме
            if (switchedToFrame)
            {
                 WebDriverActions.SwitchToDefaultContent(Driver);
            }
            return this;
        }

        public NewsPage SendPost()
        {
            SendButton().Click(Driver);
            // После отправки поста мы обычно возвращаемся на страницу новостей
            return new NewsPage(Driver);
        }

        public bool IsRecipientPresent(string recipientName)
        {
            //проверить наличие шильдика
            var recipientsArea = new WebItem("//div[@id='entity-selector-oPostFormLHE_blogPostForm']//div[@class='ui-tag-selector-items']",
                "Область получателей поста");
            bool isRecipientPresent = Waiters.WaitForCondition(() => recipientsArea.AssertTextContains(recipientName, default, Driver), 2, 6,
                $"Ожидание появления строки '{recipientName}' в '{recipientsArea.Description}'");
            return isRecipientPresent;
        }
    }
}
