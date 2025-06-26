using atFrameWork2.SeleniumFramework;
using OpenQA.Selenium;

namespace ATframework3demo.PageObjects
{
    public class NewsPage
    {
        public NewsPage(IWebDriver driver = default)
        {
            Driver = driver;
        }

        public IWebDriver Driver { get; }

        public NewsPostForm AddPost()
        {
            //Клик в Написать сообщение
            var btnPostCreate = new WebItem("//span[@id='feed-add-post-form-tab-message']//span[text()='Сообщение']", "Кнопка 'Сообщение' для создания поста");
            // Иногда элемент может быть таким: "//div[@id='microoPostFormLHE_blogPostForm_inner']" или похожим, если форма уже частично видима
            // Выбираем более явный элемент для начала создания простого сообщения
            btnPostCreate.Click(Driver);
            // Может потребоваться ожидание появления формы, если она загружается динамически
            return new NewsPostForm(Driver);
        }

        /// <summary>
        /// Проверяет, виден ли пост с указанным текстом в ленте новостей.
        /// </summary>
        /// <param name="postText">Текст поста для поиска.</param>
        /// <param name="timeoutSeconds">Максимальное время ожидания поста.</param>
        /// <returns>True, если пост найден, иначе false.</returns>
        public bool IsPostVisible(string postText, int timeoutSeconds = 10)
        {
            // Локатор для поиска поста по тексту. Bitrix24 часто оборачивает текст поста в div с классом feed-post-text-content
            // Важно: этот локатор может потребовать адаптации под конкретную структуру HTML вашего портала Bitrix24
            var postTextContent = new WebItem($"//div[contains(@class, 'feed-post-text-content') and contains(., \"{postText}\")] | //div[contains(@class, 'feed-post-text') and contains(., \"{postText}\")]", // Added another common class
                $"Пост в ленте с текстом '{postText}'");

            return postTextContent.WaitElementDisplayed(timeoutSeconds, driver: Driver);
        }
    }
}
