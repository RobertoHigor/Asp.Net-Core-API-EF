using AutoMapper;
using CoreCodeCamp.Controllers;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace Controllers.Test
{
    public class CampsControllerTest
    {
        private readonly CampsController _controller;

        public CampsControllerTest(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _controller = new CampsController(repository, mapper, linkGenerator);
        }

        [Fact]
        public void Test1()
        {
            // Código de teste, é preciso utilizar NSubstitute

        }
    }
}
