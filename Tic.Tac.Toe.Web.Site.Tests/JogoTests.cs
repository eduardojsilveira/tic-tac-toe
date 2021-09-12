using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Tic.Tac.Toe.Web.Site.Hubs;
using Tic.Tac.Toe.Web.Site.Models;

namespace Tic.Tac.Toe.Web.Site.Tests
{
    [TestClass]
    public class JogoTests
    {
        private readonly Jogo _hub;

        public JogoTests()
        {
            _hub = new Jogo();
        }

        [TestMethod]
        public void CadastrarJogador()
        {
            var guid = Guid.NewGuid().ToString();
            
            var mockClients = Mock.Create<IHubCallerConnectionContext<dynamic>>();
            var mockContext = Mock.Create<HubCallerContext>();
            var all = Mock.Create<IJogadorClient>();
            _hub.Clients = mockClients;
            _hub.Context = mockContext;
            Mock.Arrange(() => mockClients.All).Returns(all);
            Mock.Arrange(() => mockClients.Client(Arg.AnyString)).Returns(all);
            Mock.Arrange(() => mockContext.ConnectionId).Returns(guid);

            _hub.CadastrarJogador("Teste1");

            Mock.Assert(() => all.atualizaInformacoesJogos(Arg.AnyInt, Arg.AnyInt), Occurs.Once());
            Mock.Assert(() => all.confirmarCadastro(), Occurs.Once());
        }

        [TestMethod]
        public void BuscarAdversarioNaoEncontradoTest()
        {
            var guid = Guid.NewGuid().ToString();
            
            var mockClients = Mock.Create<IHubCallerConnectionContext<dynamic>>();
            var mockContext = Mock.Create<HubCallerContext>();
            var all = Mock.Create<IJogadorClient>();
            _hub.Clients = mockClients;
            _hub.Context = mockContext;
            Mock.Arrange(() => mockClients.All).Returns(all);
            Mock.Arrange(() => mockClients.Client(Arg.AnyString)).Returns(all);
            Mock.Arrange(() => mockContext.ConnectionId).Returns(guid);

            _hub.CadastrarJogador("Teste1");
            _hub.BuscarAdversario(null);

            Mock.Assert(() => all.adversarioNaoEncontrado(), Occurs.Once());
        }

        [TestMethod]
        public void BuscarAdversarioExistenteTest()
        {
            var guidJogador1 = Guid.NewGuid().ToString();
            var mockClients = Mock.Create<IHubCallerConnectionContext<dynamic>>();
            var mockContext = Mock.Create<HubCallerContext>();
            var all = Mock.Create<IJogadorClient>();
            _hub.Clients = mockClients;
            _hub.Context = mockContext;
            Mock.Arrange(() => mockClients.All).Returns(all);
            Mock.Arrange(() => mockClients.Client(Arg.AnyString)).Returns(all);
            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador1);

            var jogadorOne = "Jogador1";
            _hub.CadastrarJogador(jogadorOne);

            var guidJogador2 = Guid.NewGuid().ToString();
            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador2);

            var jogadorTwo = "Jogador2";
            _hub.CadastrarJogador(jogadorTwo);

            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador1);
            _hub.BuscarAdversario(null);

            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador2);
            _hub.BuscarAdversario(null);

            Mock.Assert(() => all.iniciarPartida(Arg.AnyString, Arg.AnyString), Occurs.AtLeastOnce());
            Mock.Assert(() => all.iniciarPartida(Arg.AnyString, Arg.AnyString, Arg.AnyString), Occurs.AtLeastOnce());

            Mock.Assert(() => all.esperandoMovimentoAdversario(), Occurs.AtLeastOnce());
            Mock.Assert(() => all.esperandoMovimentoJogador(), Occurs.AtLeastOnce());

            Mock.Assert(() => all.atualizaInformacoesJogos(Arg.AnyInt, Arg.AnyInt), Occurs.AtLeast(2));
        }

        [TestMethod]
        public void BuscarAdversarioExistenteComJogadoresPreAtribuidosTest()
        {
            var guidJogador1 = Guid.NewGuid().ToString();
            var mockClients = Mock.Create<IHubCallerConnectionContext<dynamic>>();
            var mockContext = Mock.Create<HubCallerContext>();
            var all = Mock.Create<IJogadorClient>();
            _hub.Clients = mockClients;
            _hub.Context = mockContext;
            Mock.Arrange(() => mockClients.All).Returns(all);
            Mock.Arrange(() => mockClients.Client(Arg.AnyString)).Returns(all);
            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador1);

            var jogadorOne = "Jogador1";
            _hub.CadastrarJogador(jogadorOne);

            var guidJogador2 = Guid.NewGuid().ToString();
            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador2);

            var jogadorTwo = "Jogador2";
            _hub.CadastrarJogador(jogadorTwo);

            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador1);
            _hub.BuscarAdversario(guidJogador2);

            Mock.Assert(() => all.iniciarPartida(Arg.AnyString, Arg.AnyString), Occurs.AtLeastOnce());
            Mock.Assert(() => all.iniciarPartida(Arg.AnyString, Arg.AnyString, Arg.AnyString), Occurs.AtLeastOnce());
            Mock.Assert(() => all.esperandoMovimentoAdversario(), Occurs.AtLeastOnce());
            Mock.Assert(() => all.esperandoMovimentoJogador(), Occurs.AtLeastOnce());
            Mock.Assert(() => all.atualizaInformacoesJogos(Arg.AnyInt, Arg.AnyInt), Occurs.AtLeast(2));
        }

        [TestMethod]
        public void BuscarAdversarioExistenteComValorRandomicoDivisivelPorDoisTest()
        {
            var guidJogador1 = Guid.NewGuid().ToString();
            var mockClients = Mock.Create<IHubCallerConnectionContext<dynamic>>();
            var mockContext = Mock.Create<HubCallerContext>();
            var all = Mock.Create<IJogadorClient>();

            var random = Mock.Create<Random>();
            Mock.Arrange(() => random.Next(Arg.AnyInt, Arg.AnyInt)).Returns(3000);
            var hub = new Jogo(random);

            hub.Clients = mockClients;
            hub.Context = mockContext;
            Mock.Arrange(() => mockClients.All).Returns(all);
            Mock.Arrange(() => mockClients.Client(Arg.AnyString)).Returns(all);
            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador1);

            var jogadorOne = "Jogador1";
            hub.CadastrarJogador(jogadorOne);

            var guidJogador2 = Guid.NewGuid().ToString();
            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador2);

            var jogadorTwo = "Jogador2";
            hub.CadastrarJogador(jogadorTwo);

            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador1);
            hub.BuscarAdversario(null);

            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador2);
            hub.BuscarAdversario(null);

            Mock.Assert(() => all.iniciarPartida(Arg.AnyString, Arg.AnyString), Occurs.AtLeastOnce());
            Mock.Assert(() => all.iniciarPartida(Arg.AnyString, Arg.AnyString, Arg.AnyString), Occurs.AtLeastOnce());

            Mock.Assert(() => all.esperandoMovimentoAdversario(), Occurs.AtLeastOnce());
            Mock.Assert(() => all.esperandoMovimentoJogador(), Occurs.AtLeastOnce());

            Mock.Assert(() => all.atualizaInformacoesJogos(Arg.AnyInt, Arg.AnyInt), Occurs.AtLeast(2));
        }

        [TestMethod]
        public void BuscarAdversarioExistenteComValorRandomicoNaoDivisivelPorDoisTest()
        {
            var guidJogador1 = Guid.NewGuid().ToString();
            var mockClients = Mock.Create<IHubCallerConnectionContext<dynamic>>();
            var mockContext = Mock.Create<HubCallerContext>();
            var all = Mock.Create<IJogadorClient>();

            var random = Mock.Create<Random>();
            Mock.Arrange(() => random.Next(Arg.AnyInt, Arg.AnyInt)).Returns(3001);
            var hub = new Jogo(random);

            hub.Clients = mockClients;
            hub.Context = mockContext;
            Mock.Arrange(() => mockClients.All).Returns(all);
            Mock.Arrange(() => mockClients.Client(Arg.AnyString)).Returns(all);
            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador1);

            var jogadorOne = "Jogador1";
            hub.CadastrarJogador(jogadorOne);

            var guidJogador2 = Guid.NewGuid().ToString();
            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador2);

            var jogadorTwo = "Jogador2";
            hub.CadastrarJogador(jogadorTwo);

            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador1);
            hub.BuscarAdversario(null);

            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador2);
            hub.BuscarAdversario(null);

            Mock.Assert(() => all.iniciarPartida(Arg.AnyString, Arg.AnyString), Occurs.AtLeastOnce());
            Mock.Assert(() => all.iniciarPartida(Arg.AnyString, Arg.AnyString, Arg.AnyString), Occurs.AtLeastOnce());

            Mock.Assert(() => all.esperandoMovimentoAdversario(), Occurs.AtLeastOnce());
            Mock.Assert(() => all.esperandoMovimentoJogador(), Occurs.AtLeastOnce());

            Mock.Assert(() => all.atualizaInformacoesJogos(Arg.AnyInt, Arg.AnyInt), Occurs.AtLeast(2));
        }

        [TestMethod]
        public async Task OnConnectedApplicationTestAsync()
        {
            var mockClients = Mock.Create<IHubCallerConnectionContext<dynamic>>();
            var all = Mock.Create<IJogadorClient>();
            _hub.Clients = mockClients;
            Mock.Arrange(() => mockClients.All).Returns(all);
            await _hub.OnConnected();
            Mock.Assert(() => all.atualizaInformacoesJogos(Arg.AnyInt, Arg.AnyInt), Occurs.Exactly(1));
        }

        [TestMethod]
        public async Task OnDisconnectedApplicationPlayerTestAsync()
        {
            var guidJogador1 = Guid.NewGuid().ToString();
            var mockClients = Mock.Create<IHubCallerConnectionContext<dynamic>>();
            var mockContext = Mock.Create<HubCallerContext>();
            var all = Mock.Create<IJogadorClient>();
            _hub.Clients = mockClients;
            _hub.Context = mockContext;
            Mock.Arrange(() => mockClients.All).Returns(all);
            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador1);
            await _hub.OnDisconnected(false);
            Mock.Assert(() => all.atualizaInformacoesJogos(Arg.AnyInt, Arg.AnyInt), Occurs.Never());
        }

        [TestMethod]
        public async Task OnDisconnectedJogadorCadastradoTestAsync()
        {
            var guidJogador1 = Guid.NewGuid().ToString();
            var mockClients = Mock.Create<IHubCallerConnectionContext<dynamic>>();
            var mockContext = Mock.Create<HubCallerContext>();
            var all = Mock.Create<IJogadorClient>();
            _hub.Clients = mockClients;
            _hub.Context = mockContext;
            Mock.Arrange(() => mockClients.All).Returns(all);
            Mock.Arrange(() => mockClients.Client(Arg.AnyString)).Returns(all);
            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador1);
           
            _hub.CadastrarJogador("Jogador1");
            await _hub.OnDisconnected(false);

            Mock.Assert(() => all.atualizaInformacoesJogos(Arg.AnyInt, Arg.AnyInt), Occurs.Exactly(2));

        }

        [TestMethod]
        public async Task OnDisconnectedJogadorWhenWasPlayingAsyncTest()
        {
            var guidJogador1 = Guid.NewGuid().ToString();
            var guidJogador2 = Guid.NewGuid().ToString();

            var jogadores = new List<Jogador>();
            var jogador2 = new Jogador
            {
                Nome = "Player2",
                EstaJogando = true,
                IdConexao = guidJogador2,
                DataInicio = DateTime.Now,
                EsperandoAdversario = true,
                Simbolo = "O",
            };

            var jogador1 = new Jogador
            {
                Nome = "Player1",
                EstaJogando = true,
                IdConexao = guidJogador1,
                DataInicio = DateTime.Now,
                EsperandoAdversario = false,
                Simbolo = "X",
                Adversario = jogador2
            };

            jogadores.Add(jogador1);
            jogador2.Adversario = jogador1;
            jogadores.Add(jogador2);

            var partida = new List<JogoDaVelha>();

            partida.Add(new JogoDaVelha
            {
                Jogador1 = jogador1,
                Jogador2 = jogador2,
            });

            var historico = new List<HistoricoPartidas>();
            historico.Add(new HistoricoPartidas
            {
                IdJogador = guidJogador1,
                DataTerminoPartida = DateTime.Now,
                Empate = true,
                NomeJogador = "Player1",
                Vitoria = false
            });

            Jogo hub = new Jogo(partida, jogadores, historico);

            var mockClients = Mock.Create<IHubCallerConnectionContext<dynamic>>();
            var mockContext = Mock.Create<HubCallerContext>();
            var all = Mock.Create<IJogadorClient>();

            hub.Context = mockContext;
            hub.Clients = mockClients;

            Mock.Arrange(() => mockClients.All).Returns(all);
            Mock.Arrange(() => mockClients.Client(Arg.AnyString)).Returns(all);
            Mock.Arrange(() => mockContext.ConnectionId).Returns(guidJogador1);

            await hub.OnDisconnected(true);
            Mock.Assert(() => all.atualizaInformacoesJogos(Arg.AnyInt, Arg.AnyInt), Occurs.Once());
            Mock.Assert(() => all.adversarioDesistiu(), Occurs.Once());
        }

        public interface IJogadorClient
        {
            Task atualizaInformacoesJogos(int qtdPartidas, int qtdJogadores);
            Task confirmarCadastro();
            Task adversarioNaoEncontrado();
            Task iniciarPartida(string nomeJogador, string simbolo, string connectionId);
            Task iniciarPartida(string nomeJogador, string simbolo);
            Task esperandoMovimentoJogador();
            Task esperandoMovimentoAdversario();
            Task adversarioDesistiu();
        }
    }
}
