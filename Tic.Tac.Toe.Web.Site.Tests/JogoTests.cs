using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Tic.Tac.Toe.Web.Site.Hubs;

namespace Tic.Tac.Toe.Web.Site.Tests
{
    [TestClass]
    public class JogoTests
    {
        [TestMethod]
        public void CadastrarJogador()
        {
            var guid = Guid.NewGuid().ToString();
            var hub = new Jogo();
            var mockClients = Mock.Create<IHubCallerConnectionContext<dynamic>>();
            var mockContext = Mock.Create<HubCallerContext>();
            var all = Mock.Create<IJogadorClient>();
            hub.Clients = mockClients;
            hub.Context = mockContext;
            Mock.Arrange(() => mockClients.All).Returns(all);
            Mock.Arrange(() => mockClients.Client(Arg.AnyString)).Returns(all);
            Mock.Arrange(() => mockContext.ConnectionId).Returns(guid);

            hub.CadastrarJogador("Teste1");

            Mock.Assert(() => all.atualizaInformacoesJogos(0, 1), Occurs.Exactly(1));
            Mock.Assert(() => all.confirmarCadastro(), Occurs.Exactly(1));
        }

        [TestMethod]
        public async Task OnConnectedApplicationTestAsync()
        {
            var hub = new Jogo();
            var mockClients = Mock.Create<IHubCallerConnectionContext<dynamic>>();
            var all = Mock.Create<IJogadorClient>();
            hub.Clients = mockClients;
            Mock.Arrange(() => mockClients.All).Returns(all);
            await hub.OnConnected();
            Mock.Assert(() => all.atualizaInformacoesJogos(Arg.AnyInt, Arg.AnyInt), Occurs.Exactly(1));
        }

        public interface IJogadorClient
        {
            Task atualizaInformacoesJogos(int qtdPartidas, int qtdJogadores);
            Task confirmarCadastro();

        }
    }
}
