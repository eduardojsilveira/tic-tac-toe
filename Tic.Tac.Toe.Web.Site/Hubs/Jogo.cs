using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Tic.Tac.Toe.Web.Site.Models;

namespace Tic.Tac.Toe.Web.Site.Hubs
{
    [HubName("jogo")]
    public class Jogo : Hub
    {
        /// <summary> 
        /// Objeto utilizado para o evitar que várias threads utilizem o mesma seção
        /// </summary>
        private static object sincronizacao = new object();
        /// <summary> 
        /// Quantidade total de partidas entre dois jogadores
        /// </summary>
        private int qtdPartidasJogadas = 0;
        /// <summary> 
        /// Lista de todos os jogadores cadastrados e ativos no momento
        /// </summary>
        private static readonly List<Jogador> listaJogadores = new List<Jogador>();
        /// <summary> 
        /// Lista de todas as partidas que ocorrem no momento 
        /// </summary>
        private static readonly List<JogoDaVelha> listaPartidas = new List<JogoDaVelha>();
        /// <summary> 
        /// Lista de histórico de partidas entre dois jogadores 
        /// </summary>
        private static readonly List<HistoricoPartidas> historico = new List<HistoricoPartidas>();
        /// <summary> 
        /// Objeto de números aleatórios para decidir quem começa a jogar 
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary> 
        /// Método sobrescrito da classe Hub. É executado quando o usuário entra no site.  
        /// </summary> 
        /// <returns>Retorna um objeto de tipo Thread (tarefa)</returns>
        public override Task OnConnected()
        {
            return AtualizarStatusSincronizacao();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            // faz uma busca na lista de partidas com parametro o id da conexão do jogador que está disconectando do site
            var jogo = listaPartidas.FirstOrDefault(x => x.Jogador1.IdConexao == Context.ConnectionId || x.Jogador2.IdConexao == Context.ConnectionId);
            // caso o jogador não esteja jogando remove-lo da lista de jogadores e retornar para o AtualizaStatusSincronizacao
            if (jogo == null)
            {
                var jogadorSemAdversario = listaJogadores.FirstOrDefault(x => x.IdConexao == Context.ConnectionId);
                if (jogadorSemAdversario != null)
                {
                    listaJogadores.Remove(jogadorSemAdversario);

                    AtualizarStatusSincronizacao();
                }
                return null;
            }
            // caso o jogador estava jogando remove a partida que estava jogando
            if (jogo != null)
            {
                listaPartidas.Remove(jogo);
            }
            // descobre quem é o jogador de acordo com o id da conexão dele
            var jogador = jogo.Jogador1.IdConexao == Context.ConnectionId ? jogo.Jogador1 : jogo.Jogador2;

            if (jogador == null) return null;

            listaJogadores.Remove(jogador);
            // caso tenha adversario remove todo o historico de partidas entre os dois e avisa ao adversario que o jogador desistiu da partida
            if (jogador.Adversario != null)
            {
                string[] array = { jogador.IdConexao, jogador.Adversario.IdConexao };
                historico.RemoveAll(x => array.Contains(x.IdJogador));

                AtualizarStatusSincronizacao();
                return Clients.Client(jogador.Adversario.IdConexao).adversarioDesistiu();
            }
            return null;
        }

        public void CadastrarJogador(string nome)
        {   // bloqueia o processo quando cadastra o jogador
            lock (sincronizacao)
            {   // verifica se o jogador existe e caso não exista adiciona-o na lista de jogadores
                var jogador = listaJogadores.FirstOrDefault(x => x.IdConexao == Context.ConnectionId);
                if (jogador == null)
                {
                    jogador = new Jogador { IdConexao = Context.ConnectionId, Nome = nome };
                    listaJogadores.Add(jogador);
                }

                jogador.EstaJogando = false;
            }

            AtualizarStatusSincronizacao();
            // chama metodo no cliente que o cadastro foi confirmado

            var client = Clients.Client(Context.ConnectionId);
            client.ConfirmarCadastro();
        }
        /// <summary> 
        /// Método que irá buscar adversários para o jogador cadastrado
        /// </summary> 
        /// <param name="IdJogador2">Recebe um identificador de conexão do jogador adversário. Parametro Obrigatório caso enquanto ocorre as 3 partidas</param>
        public void BuscarAdversario(string IdJogador2)
        {
            Jogador jogador, adversario;

            qtdPartidasJogadas = (qtdPartidasJogadas == 3) ? 0 : qtdPartidasJogadas;
            // caso o parametro idJogador2 esteja nulo significa que ele está procurando novo adversário
            if (String.IsNullOrEmpty(IdJogador2))
            {
                jogador = listaJogadores.FirstOrDefault(x => x.IdConexao == Context.ConnectionId);
                if (jogador == null) return;
                jogador.EsperandoAdversario = true;
                /* encontro o adversário da seguinte forma : 
                 * faço uma busca na lista de jogadores que tenham id diferente do jogador 
                 * e que não estejam jogando e ordeno pelo id e pego o primeiro resultado */
                adversario = listaJogadores.Where(x => x.IdConexao != Context.ConnectionId && x.EsperandoAdversario && !x.EstaJogando).OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                if (adversario == null)
                {   // caso não encontro o adversário chamo o metodo no cliente que não existe nenhum adversário
                    Clients.Client(Context.ConnectionId).adversarioNaoEncontrado();
                    return;
                }
            }
            // se não significa que está jogando com o mesmo adversário e atribuimos novamente.
            else
            {
                jogador = listaJogadores.FirstOrDefault(x => x.IdConexao == Context.ConnectionId);
                adversario = listaJogadores.FirstOrDefault(x => x.IdConexao.Equals(IdJogador2));
            }
            #region Atribuimos os dados dos jogadores
            jogador.EstaJogando = true;
            jogador.EsperandoAdversario = false;
            jogador.Simbolo = "X";

            adversario.EstaJogando = true;
            adversario.EsperandoAdversario = false;
            adversario.Simbolo = "O";

            jogador.Adversario = adversario;
            adversario.Adversario = jogador;
            #endregion
            // chama remotamente o metodo iniciarPartida para o adversário e o jogador
            Clients.Client(Context.ConnectionId).iniciarPartida(adversario.Nome, "/Content/Images/tic-tac-toe-x-icon.png", Context.ConnectionId);
            Clients.Client(adversario.IdConexao).iniciarPartida(jogador.Nome, "/Content/Images/tic-tac-toe-o-icon.png");
            #region Decide quem aletoriamente quem começa a partida
            if (random.Next(0, 5000) % 2 == 0)
            {
                jogador.EsperandoMovimento = false;
                adversario.EsperandoMovimento = true;

                Clients.Client(jogador.IdConexao).esperandoMovimentoJogador();
                Clients.Client(adversario.IdConexao).esperandoMovimentoAdversario();
            }
            else
            {
                jogador.EsperandoMovimento = true;
                adversario.EsperandoMovimento = false;

                Clients.Client(adversario.IdConexao).esperandoMovimentoJogador();
                Clients.Client(jogador.IdConexao).esperandoMovimentoAdversario();
            }
            #endregion
            #region Cadastra a partida
            lock (sincronizacao)
            {
                listaPartidas.Add(new JogoDaVelha { Jogador1 = jogador, Jogador2 = adversario });
            }
            #endregion

            AtualizarStatusSincronizacao();

        }
        /// <summary> 
        /// Método que irá fazer o movimento (jogada) do jogador no tabuleiro /// 
        /// </summary> 
        /// <param name="posicao">Recebe qual a posição do jogador que marcou no tabuleiro </param>
        public void Jogar(int posicao)
        {
            // verifica se o jogo existe na lista de partidas
            JogoDaVelha jogo = listaPartidas.FirstOrDefault(x => x.Jogador1.IdConexao == Context.ConnectionId || x.Jogador2.IdConexao == Context.ConnectionId);
            // caso o jogo esteja finalizado sai do metodo
            if (jogo == null || jogo.JogoFinalizado) return;

            int marcador = 0;

            // Verificar se quem está executando é o jogador1 ou jogador2
            if (jogo.Jogador2.IdConexao == Context.ConnectionId)
            {
                marcador = 1;
            }
            var jogador = marcador == 0 ? jogo.Jogador1 : jogo.Jogador2;

            if (jogador.EsperandoMovimento) return;
            // chama remotamente o metodo no cliente que houve jogada 
            Clients.Client(jogo.Jogador1.IdConexao).adicionarJogada(new InformacaoJogada { NomeAdversario = jogador.Nome, PosicaoMarcacao = posicao, Simbolo = jogador.Simbolo });
            Clients.Client(jogo.Jogador2.IdConexao).adicionarJogada(new InformacaoJogada { NomeAdversario = jogador.Nome, PosicaoMarcacao = posicao, Simbolo = jogador.Simbolo });
            // obtem o resultado a cada movimento realizado do jogador
            bool resultado = jogo.Jogar(marcador, posicao);
            // caso a partida terminou
            if (jogo.JogoFinalizado)
            {  // remove o jogo na lista de partidas
                listaPartidas.Remove(jogo);
                // adiciona na lista de historico de partidas dados do jogador e do adversário quem ganhou, perdeu ou empatou a partida
                historico.Add(new HistoricoPartidas
                {
                    DataTerminoPartida = DateTime.Now,
                    IdJogador = jogador.IdConexao,
                    NomeJogador = jogador.Nome,
                    Vitoria = (jogo.DeuEmpate == true) ? false : resultado,
                    Empate = (jogo.JogoFinalizado && jogo.DeuEmpate)
                });

                historico.Add(new HistoricoPartidas
                {
                    DataTerminoPartida = DateTime.Now,
                    IdJogador = jogador.Adversario.IdConexao,
                    NomeJogador = jogador.Adversario.Nome,
                    Vitoria = (jogo.DeuEmpate == true) ? false : !resultado,
                    Empate = (jogo.JogoFinalizado && jogo.DeuEmpate)
                });
                string[] jogadores = { jogador.IdConexao, jogador.Adversario.IdConexao };

                // determina a quantidade de partidas jogadas
                qtdPartidasJogadas = historico.Where(x => jogadores.Contains(x.IdJogador)).Count() / 2;
                // chama remotamente para o jogador e o adversário que a partida terminou
                Clients.Client(jogo.Jogador1.IdConexao).fimDeJogo(qtdPartidasJogadas, AtualizarPlacar(historico.Where(x => jogadores.Contains(x.IdJogador)).ToList(), qtdPartidasJogadas));
                Clients.Client(jogo.Jogador2.IdConexao).fimDeJogo(qtdPartidasJogadas, AtualizarPlacar(historico.Where(x => jogadores.Contains(x.IdJogador)).ToList(), qtdPartidasJogadas));
                // caso a quantidade de partidas seja menor que 3 chama o mesmo adversário
                if (qtdPartidasJogadas < 3)
                {
                    BuscarAdversario(jogador.Adversario.IdConexao);
                }
                else
                {
                    historico.RemoveAll(x => jogadores.Contains(x.IdJogador));
                }

            }
            // caso o jogo não esteja finalizado troca a vez de quem irá jogar
            if (!jogo.JogoFinalizado)
            {
                jogador.EsperandoMovimento = !jogador.EsperandoMovimento;
                jogador.Adversario.EsperandoMovimento = !jogador.Adversario.EsperandoMovimento;

                Clients.Client(jogador.Adversario.IdConexao).esperandoMovimentoJogador();
            }

            AtualizarStatusSincronizacao();
        }
        /// <summary> 
        /// Método que retornar a quantidade de jogadores e partidas ativas no momento em que o jogador estiver conectado ao site ///        
        /// </summary> 
        /// <returns>Retorna um objeto de tipo Thread (tarefa)</returns>
        public Task AtualizarStatusSincronizacao()
        {
            // Chama remotamente o metodo no client o metodo atualizaInformacoesJogos que contem  quantidade de partidas e jogadores.
            return Clients.All.atualizaInformacoesJogos(listaPartidas.Count, listaJogadores.Count);
        }
        /// <summary> 
        /// Método que atualiza os placar das partidas.
        /// </summary> 
        /// <param name="lista">Recebe a lista de partidas entre os dois jogadores</param>
        /// <param name="qtdPartidaJogada">Total de partidas jogadas entre os dois jogadores</param>
        /// <returns>Retorna um objeto que contem dados da partida e o vencedor final após 3 partidas.</returns>
        private object[] AtualizarPlacar(List<HistoricoPartidas> lista, int qtdPartidaJogada)
        {

            object[] resultado = new object[3];
            #region Determina Vencedor Final em 3 partidas 
            if (qtdPartidaJogada == 3)
            {

                string[] idsJogadores = lista.GroupBy(x => x.IdJogador).Select(x => x.First().IdJogador).ToArray();

                List<HistoricoPartidas> historicoJogador1 = lista.Where(x => x.IdJogador.Equals(idsJogadores[0])).ToList();
                List<HistoricoPartidas> historicoJogador2 = lista.Where(x => x.IdJogador.Equals(idsJogadores[1])).ToList();
                string vencedorFinal = null;
                // caso haja 3 empates ou numero de vitorias entre o dois jogadores seja igual : é empate !!!
                if (lista.Where(x => x.Empate == true).Count() / 2 == 3 || historicoJogador1.Where(x => x.Vitoria == true).Count() == historicoJogador2.Where(x => x.Vitoria == true).Count())
                {
                    vencedorFinal = string.Empty;
                }
                // caso o jogador 1 tenha um numero de vitorias maior que o jogador 2 : Vitória do jogador 1
                else if (historicoJogador1.Where(x => x.Vitoria == true).Count() > historicoJogador2.Where(x => x.Vitoria == true).Count())
                {
                    vencedorFinal = historicoJogador1.Select(x => x.NomeJogador).FirstOrDefault();
                }
                // caso o jogador 2 tenha um numero de vitorias maior que o jogador 1: Vitória do jogador 2
                else if (historicoJogador2.Where(x => x.Vitoria == true).Count() > historicoJogador1.Where(x => x.Vitoria == true).Count())
                {
                    vencedorFinal = historicoJogador2.Select(x => x.NomeJogador).FirstOrDefault();
                }

                resultado[2] = new { vencedor = vencedorFinal };
            }
            #endregion
            #region Atualiza cada partida o placar no cliente
            if (qtdPartidaJogada != 1)
            {

                lista = lista.GetRange((qtdPartidaJogada == 3) ? qtdPartidaJogada + 1 : qtdPartidaJogada, 2);
            }
            if (lista.Where(x => x.Vitoria == true).Count() == 1)
            {
                resultado[0] = lista.Where(x => x.IdJogador.Equals(Context.ConnectionId)).Select(x => new { nomeJogador = x.NomeJogador, status = (x.Vitoria) ? "V" : "D", id = x.IdJogador }).FirstOrDefault();
                resultado[1] = lista.Where(x => !x.IdJogador.Equals(Context.ConnectionId)).Select(x => new { nomeJogador = x.NomeJogador, status = (x.Vitoria) ? "V" : "D", id = x.IdJogador }).FirstOrDefault();
            }
            else
            {
                resultado[0] = lista.Where(x => x.IdJogador.Equals(Context.ConnectionId)).Select(x => new { nomeJogador = x.NomeJogador, status = "E", id = x.IdJogador }).FirstOrDefault();
                resultado[1] = lista.Where(x => !x.IdJogador.Equals(Context.ConnectionId)).Select(x => new { nomeJogador = x.NomeJogador, status = "E", id = x.IdJogador }).FirstOrDefault();
            }
            #endregion
            return resultado;
        }
    }
}