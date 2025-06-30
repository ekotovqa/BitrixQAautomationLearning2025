using atFrameWork2.BaseFramework;
using atFrameWork2.PageObjects; // Assuming PortalHomePage might be needed
using ATframework3demo.PageObjects;

namespace ATframework3demo.TestCases // Changed namespace to reflect project
{
    public class Case_Bitrix24_NewsFeed : CaseCollectionBuilder
    {
        protected override List<TestCase> GetCases()
        {
            var cases = new List<TestCase>
            {
                new TestCase("Проверка добавления нового поста в ленте", homePage => AddAndVerifyNewsFeedPost(homePage))
            };
            return cases;
        }

        public void AddAndVerifyNewsFeedPost(PortalHomePage homePage)
        {
            // 1. Navigate to News Feed
            NewsPage newsPage = homePage.LeftMenu.OpenNews();

            // 2. Initiate new post creation
            NewsPostForm newsPostForm = newsPage.AddPost();

            // 3. Generate unique post content
            string uniquePostText = $"Test post content {DateTime.Now.Ticks}";

            // 4. Set post text and send
            // The NewsPostForm handles switching to/from its iframe if necessary
            newsPostForm.SetPostText(uniquePostText);
            newsPage = newsPostForm.SendPost(); // SendPost returns a NewsPage

            // 5. Verify the post is visible in the feed
            bool isVisible = newsPage.IsPostVisible(uniquePostText);

            if (isVisible)
            {
                atFrameWork2.BaseFramework.LogTools.Log.Info($"Successfully created and found news feed post: '{uniquePostText}'");
            }
            else
            {
                atFrameWork2.BaseFramework.LogTools.Log.Error($"Failed to find news feed post: '{uniquePostText}'");
                // Optionally, fail the test explicitly here if your framework doesn't auto-fail on Log.Error or if an assertion is preferred
                // Assert.IsTrue(isVisible, $"Failed to find news feed post: '{uniquePostText}'");
                // For now, relying on Log.Error to mark test as failed as per existing TestCase behavior
            }

            // It's good practice to ensure the test fails if the post isn't visible.
            // The TestCase base class seems to mark a test as failed if any Log.Error messages are present.
            // If a more explicit assertion is needed, one could be added here, e.g., using NUnit's Assert.IsTrue(isVisible, "message");
            // For now, we'll rely on the Log.Error to indicate failure.
            if (!isVisible)
            {
                // This will make the test fail if not already failed by Log.Error
                throw new Exception($"Verification failed: News feed post '{uniquePostText}' was not found after creation.");
            }
        }
    }
}
