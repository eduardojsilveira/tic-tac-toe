using System;

namespace Tic.Tac.Toe.Web.Site.Models
{
    
    /// <summary> 
    /// Classe que contem informação de cada partida realizada no jogo da velha 
    /// </summary>
    public class HistoricoPartidas
    {   /// <summary> 
        /// Código de conexão do jogador
        /// </summary>
        public string IdJogador { get; set; }
        /// <summary> 
        /// Nome do jogador
        /// </summary>
        public string NomeJogador { get; set; }
        /// <summary> 
        /// Status se Jogador venceu a partida sim ou não
        /// </summary>
        public bool Vitoria { get; set; }
        /// <summary> 
        /// Status se Jogador empatou a partida sim ou não
        /// </summary>
        public bool Empate { get; set; }
        /// <summary> 
        /// Data de término da partida
        /// </summary>
        public DateTime DataTerminoPartida { get; set; }

    }
}