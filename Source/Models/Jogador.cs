using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tic.Tac.Toe.Web
{   /// <summary> 
    /// Classe modelo que contém os dados do nome do jogador
    /// </summary>
    public class Jogador
    {
        /// <summary> 
        /// Nome do Jogador
        /// </summary>
        public string Nome { get; set; }
        /// <summary> 
        ///  Adversário do jogador no nomento do jogo
        /// </summary>
        public Jogador Adversario { get; set; }
        /// <summary> 
        ///  Status se o jogador está jogando
        /// </summary>
        public bool EstaJogando { get; set; }
        /// <summary> 
        ///  Status se o jogador está fazendo algum movimento
        /// </summary>
        public bool EsperandoMovimento { get; set; }
        /// <summary> 
        ///  Status se o jogador está esperando movimento do Adversário
        /// </summary>
        public bool EsperandoAdversario { get; set; }
        /// <summary> 
        ///  Data de cadastro do jogador
        /// </summary>
        public DateTime DataInicio { get; set; }
        /// <summary> 
        ///  Valor Simbolo do jogador quando ele está jogando (Se é X ou O)
        /// </summary>
        public string Simbolo { get; set; }
        /// <summary> 
        ///  Id de conexão do jogador quando ele se cadastra no site.
        /// </summary>
        public string IdConexao { get; set; }
    }
}