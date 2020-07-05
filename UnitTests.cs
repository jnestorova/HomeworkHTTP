using System;
using System.Collections.Generic;
using HomeworkHTTP.Model;
using NUnit.Framework;
using RestSharp;

namespace HomeworkHTTP
{
    [TestFixture]
    public class Tests
    {
        private RestClient _restClient;
        private Household household;
        private User _createdUser1;
        private User _createdUser2;
        private Book book;
        private List<Book> books;


        [SetUp]
        public void Setup()
        {
            _restClient = new RestClient();
            books = new List<Book>();

            _restClient.BaseUrl = new Uri("http://localhost:3000");
            _restClient.AddDefaultHeader("Content-Type", "application/json");
            _restClient.AddDefaultHeader("G-Token", "ROM831ESV");
        }

        [Test]
        [Order(1)]
        public void CreateHousehold()
        {
            household = new Household() { Name = "Smith family 10" };
            var request = new RestRequest("/households", Method.POST);
            request.AddParameter("application/json", household.ToJson(), ParameterType.RequestBody);

            string content = _restClient.Post(request).Content;
            household = Household.FromJson(content);

            IRestResponse response = _restClient.Post(request);

            Assert.IsTrue(response.IsSuccessful);
            Assert.AreEqual("Smith family 10", household.Name);
        }

        [Test]
        [Order(2)]
        public void CreateUser1()
        {
            string user1Random = GenerateRandomTestString();
            int householdId = household.Id;

            var user1 = new User()
            {
                Email = $"user1-{user1Random}@gmail.com",
                FirstName = $"user1FN-{user1Random}",
                LastName = $"user1LN{user1Random}",
                HouseholdId = householdId
            };

            string createdUser1Response = CreateUserRequest("/users", user1).Content;
            _createdUser1 = User.FromJson(createdUser1Response);

        }

        [Test]
        [Order(3)]
        public void CreateUser2()
        {
            string user2Random = GenerateRandomTestString();
            int householdId = household.Id;

            var user2 = new User()
            {
                Email = $"user2-{user2Random}@hotmail.com",
                FirstName = $"user2FN-{user2Random}",
                LastName = $"user1LN{user2Random}",
                HouseholdId = householdId
            };

            var createdUser2Response = CreateUserRequest("/users", user2).Content;
            _createdUser2 = User.FromJson(createdUser2Response);

        }

        [Test]
        [Order(4)]
        public void CreateBooks()
        {
            string bookRandom = GenerateRandomTestString();

            for (int i = 0; i < 4; i++)
            {
                book = new Book() { Title = $"book_{bookRandom}" };
                var request = new RestRequest("/books", Method.POST);
                request.AddParameter("application/json", book.ToJson(), ParameterType.RequestBody);

                IRestResponse response = _restClient.Post(request);

                Assert.IsTrue(response.IsSuccessful);
                Assert.IsNotNull(book.Id);
                books.Add(book);
            }
        }

        [Test]
        [Order(5)]
        public void AddBooksToUsers()
        {
            int user1WishlistId = _createdUser1.WishlistId;
            int user2WishlistId = _createdUser2.WishlistId;

            AddBooksToUser(new int[] { 1, 2 }, user1WishlistId);
            AddBooksToUser(new int[] { 3, 4 }, user2WishlistId);
        }

        [Test]
        [Order(6)]
        public void GetHouseholdWishlist()
        {
            int householdId = household.Id;

            RestRequest restRequest = new RestRequest($"/households/{householdId}/wishlistBooks", Method.GET);
            IRestResponse restResponse = _restClient.Get(restRequest);

            Assert.IsTrue(restResponse.IsSuccessful);
        }


        ///HelpMethods 

        private RestRequest Post(string endPoint)
        {
            RestRequest request = new RestRequest(endPoint, Method.POST);
            return request;
        }

        private IRestResponse CreateUserRequest(string endPoint, User userCreation)
        {
            var restRequest = Post(endPoint).AddParameter("application/json", userCreation.ToJson(), ParameterType.RequestBody);
            return _restClient.Post(restRequest);
        }

        private string GenerateRandomTestString()
        {
            int randomNumber = new Random().Next(0, int.MaxValue);
            return $"{randomNumber}";
        }

        private void AddBooksToUser(int[] books, int userWishlistId)
        {
            for (int i = 0; i < books.Length; i++)
            {
                _ = _restClient.Post(Post($"wishlists/{userWishlistId}/books/{books[i]}"));
            }
        }
        ///end HelpMethods
    }
}
