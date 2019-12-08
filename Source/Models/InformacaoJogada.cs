using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tic.Tac.Toe.Web
{   /// <summary> 
    /// Classe modelo que contem os dados de cada movimento que o jogador faz
    /// </summary>
    public class InformacaoJogada
    {    /// <summary> 
        /// Nome do Adversário
        /// </summary>
        public string NomeAdversario { get; set; }
        /// <summary> 
        /// Qual é a posição no tabuleiro que o usuário marcou
        /// </summary>
        public int PosicaoMarcacao { get; set; }
        /// <summary> 
        /// Qual o simbolo que o usuário está fazendo a jogada
        /// </summary>
        public string Simbolo { get; set; }
    }
}