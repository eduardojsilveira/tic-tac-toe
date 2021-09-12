using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tic.Tac.Toe.Web.Site.Models;
using FluentAssertions;
namespace Tic.Tac.Toe.Web.Site.Tests
{
    [TestClass]
    public class JogoDaVelhaTestes
    {

        [DataTestMethod]
        [DataRow(0, 4, 8)]
        [DataRow(2, 4, 6)]
        [DataRow(0, 1, 2)]
        [DataRow(3, 4, 5)]
        [DataRow(6, 7, 8)]
        [DataRow(0, 3, 6)]
        [DataRow(1, 4, 7)]
        [DataRow(2, 5, 8)]
        public void JogoDaVelhaComVitoriaJogadorTest(int pos1, int pos2, int pos3)
        {
            var jogo = new JogoDaVelha();

            jogo.Jogar(1, pos1);
            jogo.Jogar(1, pos2);
            jogo.Jogar(1, pos3);

            jogo.JogoFinalizado.Should().BeTrue();
            jogo.DeuEmpate.Should().BeFalse();
        }


        [TestMethod]
        public void JogoDaVelhaComEmpateJogadorTest()
        {
            var jogo = new JogoDaVelha();

            jogo.Jogar(1, 0);
            jogo.Jogar(0, 1);
            jogo.Jogar(1, 2);
            jogo.Jogar(0, 3);
            jogo.Jogar(0, 4);
            jogo.Jogar(1, 5);
            jogo.Jogar(0, 8);
            jogo.Jogar(1, 7);
            jogo.Jogar(0, 6);

            jogo.JogoFinalizado.Should().BeTrue();
            jogo.DeuEmpate.Should().BeTrue();
        }

        [TestMethod]
        public void JogoDaVelhaComJogadaAposOJogoFinalizado()
        {
            var jogo = new JogoDaVelha();

            jogo.Jogar(1, 0);
            jogo.Jogar(0, 1);
            jogo.Jogar(1, 2);
            jogo.Jogar(0, 3);
            jogo.Jogar(0, 4);
            jogo.Jogar(1, 5);
            jogo.Jogar(0, 8);
            jogo.Jogar(1, 7);
            jogo.Jogar(0, 6);
            var result = jogo.Jogar(1, 6);
            result.Should().BeFalse();
            jogo.JogoFinalizado.Should().BeTrue();
            jogo.DeuEmpate.Should().BeTrue();
        }


        [TestMethod]
        public void JogoDaVelhaComJogadaEmPosicaoJaMarcarda()
        {
            var jogo = new JogoDaVelha();
            jogo.Jogar(1, 0);
            var result = jogo.Jogar(0, 0);      
            result.Should().BeFalse();
            jogo.JogoFinalizado.Should().BeFalse();
            jogo.DeuEmpate.Should().BeFalse();
        }

        [TestMethod]
        public void JogoDaVelhaComJogadaEmPosicaoInvalida()
        {
            var jogo = new JogoDaVelha();
            var result = jogo.Jogar(0, 10);
            result.Should().BeFalse();
            jogo.JogoFinalizado.Should().BeFalse();
            jogo.DeuEmpate.Should().BeFalse();
        }
    }
}
