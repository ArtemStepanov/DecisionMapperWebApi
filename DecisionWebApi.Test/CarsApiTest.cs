using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using DecisionWebApi.Models;
using DecisionWebApi.Test.Servers;
using Newtonsoft.Json;
using Xunit;

namespace DecisionWebApi.Test
{
    public class ApiTest
    {
        private readonly List<Car> _cars = new List<Car>
        {
            new Car
            {
                Id = 1,
                Name = "KIA",
                Description = null
            },
            new Car
            {
                Id = 2,
                Name = "HYUNDAI",
                Description = "Korean car"
            },
            new Car
            {
                Name = "Test",
                Description = "Korean car"
            }
        };

        private const string CONTENT_TYPE = "application/json";

        [Fact(DisplayName = "Create cars test [OK]")]
        public async void CreateTest()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                foreach (var car in _cars)
                {
                    var result = await client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(car), Encoding.UTF8, CONTENT_TYPE));
                    Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                }
            }
        }

        [Fact(DisplayName = "Update car's description test [OK]")]
        public async void UpdateDescriptionTest()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var result = await client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(_cars[0]), Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                string newDescription = "Korean car";
                result = await client.PatchAsync("/api/cars", new StringContent("{ \"Id\": 1, \"Description\": \"" + newDescription + "\" }", Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
                result = await client.GetAsync("api/cars/1");
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                var car = await result.Content.ReadAsAsync<Car>();
                Assert.Equal(newDescription, car.Description);
            }
        }

        [Fact(DisplayName = "Delete car test [OK]")]
        public async void DeleteTest()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var car = _cars[0];
                car.Id = 10;
                var result = await client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(car), Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                result = await client.DeleteAsync($"/api/cars/{car.Id}");
                Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            }
        }

        [Fact(DisplayName = "Delete car test [ERROR]")]
        public async void DeleteTestError()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var result = await client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(_cars[0]), Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                result = await client.DeleteAsync($"/api/cars/10");
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            }
        }

        [Fact(DisplayName = "Get all cars test [OK]")]
        public async void ReadAllTest()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var result = await client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(_cars[0]), Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                result = await client.GetAsync("/api/cars");
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                var cars = await result.Content.ReadAsAsync<Car[]>();
                Assert.True(cars.Length > 0);
            }
        }

        [Fact(DisplayName = "Get car by id [OK]")]
        public async void ReadByIdTest()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var result = await client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(_cars[0]), Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                result = await client.GetAsync("/api/cars/1");
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                var car = await result.Content.ReadAsAsync<Car>();
                Assert.Equal(_cars[0].Name, car.Name);
            }
        }

        [Fact(DisplayName = "Get car test [ERROR]")]
        public async void ReadByIdTestError()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var result = await client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(_cars[0]), Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                result = await client.GetAsync("/api/cars/5");
                Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            }
        }

        [Fact(DisplayName = "Update car's description to null test [OK]")]
        public async void UpdateDescriptionToNullTest()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var result = await client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(_cars[0]), Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                result = await client.PatchAsync("/api/cars", new StringContent("{ \"Id\": 1, \"Description\": null }", Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
                result = await client.GetAsync("/api/cars/1");
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                var car = await result.Content.ReadAsAsync<Car>();
                Assert.Null(car.Description);
            }
        }

        [Fact(DisplayName = "Update car's name to null test [ERROR]")]
        public async void UpdateNameToNullTest()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var result = await client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(_cars[0]), Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                result = await client.PatchAsync("/api/cars", new StringContent("{ \"Id\": 1, \"Name\": null }", Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
                result = await client.GetAsync("/api/cars/1");
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                var car = await result.Content.ReadAsAsync<Car>();
                Assert.Equal(_cars[0].Name, car.Name);
            }
        }

        [Fact(DisplayName = "Create similar car entity twice [ERROR]")]
        public async void ExistTest()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var result = await client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(_cars[0]), Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                result = await client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(_cars[0]), Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            }
        }

        [Fact(DisplayName = "Create car with additional field [OK]")]
        public async void CreateWithAdditionalFieldTest()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var car = new
                {
                    Id = 1,
                    Name = "KIA",
                    Date = DateTime.Now
                };
                var result = await client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(car), Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                result = await client.GetAsync($"/api/cars/{car.Id}");
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                var createdCar = await result.Content.ReadAsAsync<Car>();
                Assert.Equal(car.Id, createdCar.Id);
            }
        }
        
        [Fact(DisplayName = "Update car with additional field [OK]")]
        public async void UpdateWithAdditionalFieldTest()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var car = new
                {
                    Id = 1,
                    Name = "KIA",
                    Date = DateTime.Now
                };
                var result = await client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(_cars[0]), Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                result = await client.PatchAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(car), Encoding.UTF8, CONTENT_TYPE));
                // Will not use Date field, because controller waits for Car object
                Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            }
        }
        
        [Fact(DisplayName = "Update car without id test [OK]")]
        public async void UpdateWithoutIdTest()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var car = new
                {
                    Name = "Car"
                };
                var result = await client.PatchAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(car), Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
                result = await client.GetAsync($"/api/cars/1");
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                var createdCar = await result.Content.ReadAsAsync<Car>();
                Assert.Equal(car.Name, createdCar.Name);
            }
        }
        
        [Fact(DisplayName = "Update car's description to null test without existing car [ERROR]")]
        public async void UpdateWithNullDescription()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var result = await client.PatchAsync("/api/cars", new StringContent("{ \"Description\": null }", Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            }
        }
        
        [Fact(DisplayName = "Update car's name to null test without existing car [ERROR]")]
        public async void UpdateWithNullName()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var result = await client.PatchAsync("/api/cars", new StringContent("{ \"Name\": null }", Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            }
        }
        
        [Fact(DisplayName = "Update with only id assigned in request [ERROR]")]
        public async void UpdateWithOnlyIdAssigned()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var result = await client.PatchAsync("/api/cars", new StringContent("{ \"Id\" : 1 }", Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            }
        }
        
        [Fact(DisplayName = "Update with id and description assigned in request [ERROR]")]
        public async void UpdateWithIdAndDescriptionAssigned()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var result = await client.PatchAsync("/api/cars", new StringContent("{ \"Id\" : 1, \"Description\" : null }", Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            }
        }
        
        [Fact(DisplayName = "Update with id and null name assigned in request [ERROR]")]
        public async void UpdateWithIdAndNullNameAssigned()
        {
            var server = CarTestServer.CreateNew();
            using (var client = server.CreateClient())
            {
                var result = await client.PatchAsync("/api/cars", new StringContent("{ \"Id\" : 1, \"Name\" : null }", Encoding.UTF8, CONTENT_TYPE));
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            }
        }
    }
}