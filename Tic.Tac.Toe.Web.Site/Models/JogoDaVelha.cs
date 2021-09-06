using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tic.Tac.Toe.Web.Site.Models
{   /// <summary> 
    /// Classe modelo que contém a lógica do jogo da velha
    /// </summary>
    public class JogoDaVelha
    {   /// <summary> 
        /// Status se o jogo foi finalizado ou não
        /// </summary>
        public bool JogoFinalizado { get; private set; }
        /// <summary> 
        /// Status se a partida deu empate ou não
        /// </summary>
        public bool DeuEmpate { get; private set; }
        /// <summary> 
        /// Dados do Jogador1
        /// </summary>
        public Jogador Jogador1 { get; set; }
        /// <summary> 
        /// Dados do Jogador2
        /// </summary>
        public Jogador Jogador2 { get; set; }
        /// <summary> 
        /// Array de inteiros que contém as movimentos marcados pelos jogadores
        /// </summary>
        private readonly int[] array = new int[9];
        /// <summary> 
        /// Contador de movimentos inicia-se com 9 
        /// </summary>
        private int qtdMovimentos = 9;

        /// <summary> 
        /// Construtor que inicia o jogo atribuindo o  array com valor de -1
        /// </summary>
        public JogoDaVelha()
        {
            // Inicializa tabuleiro
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = -1;
            }
        }
        /// <summary> 
        /// Método que realiza o movimento do jogador e
        /// </summary>
        /// <param name="jogador">Qual é o jogador que está fazendo o movimento. Valores possivéis são 0 e 1</param>
        /// <param name="posicao">Qual é a posição no tabuleiro que o jogador está marcando</param>
        /// <returns>Retorna se houve vencedor ou não ápos o movimento</returns>
        public bool Jogar(int jogador, int posicao)
        {    
            // Caso a partida esteja finalizada retora falso
            if (JogoFinalizado)
                return false;

            MarcarJogada(jogador, posicao);
           
            return VerificarGanhador();
        }
        /// <summary> 
        /// Método que verifica se existe vencedor na partida
        /// </summary>>
        /// <returns>Retorna se houve ou não vencedor da partida </returns>
        private bool VerificarGanhador()
        {
            for (int i = 0; i < 3; i++)
            {   // Condição para verficar se o mesmo jogador marcou no tabuleiro
                // nas posições (0,1,2) ou (3,4,5) ou (6,7,8)
                // (0,3,6) ou  (1,4,7) ou (2,5,8)
                if (
                    ((array[i * 3] != -1 && array[(i * 3)] == array[(i * 3) + 1] && array[(i * 3)] == array[(i * 3) + 2]) ||
                     (array[i] != -1 && array[i] == array[i + 3] && array[i] == array[i + 6])))
                {
                    JogoFinalizado = true;
                    return true;
                }
            }
            // Condição para verficar se o mesmo jogador marcou no tabuleiro nas posições (0,4,8) ou (2,4,6)
            if ((array[0] != -1 && array[0] == array[4] && array[0] == array[8]) || (array[2] != -1 && array[2] == array[4] && array[2] == array[6]))
            {
                JogoFinalizado = true;
                return true;
            }

            return false;
        }
        /// <summary> 
        /// Método que verifica se existe vencedor na partida
        /// </summary>>
        /// <returns>Retorna se houve ou não vencedor da partida </returns>
        private bool MarcarJogada(int jogador, int posicao)
        {   // Diminui no contador a quantidade de movimentos
            qtdMovimentos -= 1;

            if (posicao > array.Length)
                return false;
            // Caso o jogador tente fazer jogada em uma posição que já foi marcada
            if (array[posicao] != -1)
                return false;
            // atribui a posição que o jogador marcou
            array[posicao] = jogador;
            // Caso termine todos os movimentos e não houve vencedor retorna como empate.
            if (qtdMovimentos <= 0 && !VerificarGanhador())
            {
                JogoFinalizado = true;
                DeuEmpate = true;
                return false;
            }

            return true;
        }

    }
}