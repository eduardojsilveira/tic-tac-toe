var jogo;
var IdConexao;
$(function () {
    //objeto de conexão entre o cliente e o servidor hub
    jogo = $.connection.jogo;
    
    //Metodo que exibe informções dos jogos para todos que visitarem a aplicação
    jogo.client.atualizaInformacoesJogos = (function (qtdPartidas, qtdJogadores) {

        $("#qtdJogadores").text(qtdJogadores);
        $("#qtdPartidas").text(qtdPartidas);

    });
    //Método exibe uma mensagem indicando que não houve adversário encontrado
    jogo.client.adversarioNaoEncontrado = (function () {

        $("#msgAdversarioNaoEncontrado").show();
        $("#msgAdversarioNaoEncontrado").delay(3000).hide(0);
    });
    // Método exibe uma mensagem indicando que o jogador foi cadastrado com sucesso
    jogo.client.confirmarCadastro = (function () {

        $("#msgConfirmarCadastro").show();
        $("#msgConfirmarCadastro").delay(3000).hide(0);
    });
    //Método exibe uma mensagem indicando que o jogador deve fazer sua jogada
    jogo.client.esperandoMovimentoJogador = (function () {

        $("#msgInfoStatusJogo > p").html("<strong>Atenção, é a sua vez de jogar. Faça sua jogada !!! </strong>");
        $("#msgInfoStatusJogo").show();
    });
    // Método exibe uma mensagem indicando que o jogador deve esperar o adversário fazer sua jogada
    jogo.client.esperandoMovimentoAdversario = (function () {

        $("#msgInfoStatusJogo > p").html("<strong>Por favor,espere o seu adversário fazer a jogada !!! </strong>");
        $("#msgInfoStatusJogo").show();
    });
    // Método exibe uma mensagem indicando que o adversário desistiu da partida
    jogo.client.adversarioDesistiu = (function () {
        $("#msgInfoStatusJogo > p").html("<p><strong>Atenção, adversário desistiu da partida, Você é o vencedor !!! </strong></p>");
        $("#msgInfoStatusJogo").removeClass("alert alert-info");
        $("#msgInfoStatusJogo").addClass("alert alert-warning");
        $("#msgInfoStatusJogo").show();
        $("#buscarOutroAdversario").show();

    });
    // Método exibe o painel do jogo da velha
    jogo.client.iniciarPartida = (function (adversario, simbolo) {
        $("#painelAguardeAdversario").hide();
        $("#buscarOutroAdversario").hide();
        $("#adversario").text(adversario);
        $("#simbolo-small").html("<img src='" + simbolo + "' width='20' height='20'/>");
        $("#msgInfoJogo").show();
        $("#painelJogo").show();
    });
    // Método marca cada movimento do jogador no painel do jogo da velha
    jogo.client.adicionarJogada = (function (resultado) {
        if (resultado.Simbolo == "O") {
            $("#" + resultado.PosicaoMarcacao).addClass("mark2");
            $("#" + resultado.PosicaoMarcacao).addClass("marked");
        }
        else {
            $("#" + resultado.PosicaoMarcacao).addClass("mark1");
            $("#" + resultado.PosicaoMarcacao).addClass("marked");
        }
        $("#msgInfoStatusJogo > p").html("<strong>Por favor,espere o seu adversário fazer a jogada</strong>");
    });
    // Método que exibe o fim da partida quando há 
    jogo.client.fimDeJogo = (function (qtdPartidas, historico) {

        var mensagem = (qtdPartidas == 3) ? ((historico[2].vencedor != '') ? "<strong>Fim de jogo, o vencedor é: " + historico[2].vencedor + "</strong>" : "<strong>Fim de jogo, Empate !!! </strong>") : "";
        atualizaPlacar(historico, qtdPartidas);

        if (qtdPartidas == 3) {
            $("#buscarOutroAdversario").show();
        }
        else {
            limparTabuleiro();
        }

        $("#qtdTotalPartidas").text(qtdPartidas);
        $("#msgInfoStatusJogo > p").html(mensagem);

    });
    // preeche a tabela partida a partida contendo nomes dos jogadores e o status ( V - Vitória , E - Empate, D- Derrota)
    function atualizaPlacar(historico, qtdPartidas) {
        
        $("#statusPartida").show();
        $("#statusPartida").next().show();
        if (qtdPartidas == 1) {
            $("#jogador1").text(historico[0].nomeJogador);
            $("#jogador2").text(historico[1].nomeJogador);
            $("#idjogador1").val(historico[0].id);
            $("#idjogador2").val(historico[1].id);
            $("#statusPartida > tbody > tr:nth-child(" + qtdPartidas + ") > td:nth-child(2)").html(historico[0].status);
            $("#statusPartida > tbody > tr:nth-child(" + qtdPartidas + ") > td:nth-child(3)").html(historico[1].status);
        }
        else {
            if ($("#idjogador1").val() == historico[0].id) {
                $("#statusPartida > tbody > tr:nth-child(" + qtdPartidas + ") > td:nth-child(2)").html(historico[0].status);
                $("#statusPartida > tbody > tr:nth-child(" + qtdPartidas + ") > td:nth-child(3)").html(historico[1].status);
            }
            else {
                $("#statusPartida > tbody > tr:nth-child(" + qtdPartidas + ") > td:nth-child(2)").html(historico[1].status);
                $("#statusPartida > tbody > tr:nth-child(" + qtdPartidas + ") > td:nth-child(3)").html(historico[0].status);
            }
        }

    }
    // Ação que irá cadastrar o jogador
    $("#cadastrarJogador").click(function () {
        jogo.server.cadastrarJogador($('#nomeJogador').val());
        $("#cadastroJogador").hide();
        $("#painelBuscarAdversario").show();
    });
    // Ação que irá buscar adversário para o jogador
    $("#buscarAdversario").click(function () {
        $("#painelBuscarAdversario").hide();
        $("#msgInfoStatusJogo").hide();
        $("#painelAguardeAdversario").show();
        jogo.server.buscarAdversario(null);
    });
    // Ação que irá buscar outro adversário após 3 partidas
    $('#buscarOutroAdversario').click(function () {
        limparTabuleiro();
        limparPlacar();
        $("#statusPartida").hide();
        $("#painelJogo").hide();
        jogo.server.buscarAdversario(null);
        $("#painelAguardeAdversario").show();
    });
    // Ação que irá marcar a jogada no painel do jogo da velha
    $('body .box').on('click', function (event) {
        if ($(this).hasClass("marked")) return;
        jogo.server.jogar(event.target.id);
    });
    // Limpa o tabuleiro a cada partida. É executado no metodo fimDeJogo
    function limparTabuleiro() {
        $(".box").removeClass("mark1");
        $(".box").removeClass("mark2");
        $(".box").removeClass("marked");
    }
    // limpa a tabela de placar É executado no metodo fimDeJogo.
    function limparPlacar() {
        $("#statusPartida > tbody > tr > td:contains('D'),td:contains('E'),td:contains('V')").html('');
        $("#jogador1,#jogador2").html('');
        $("#idjogador1").val('');
        $("#idjogador2").val('');
    }
    // inicia a conexão com o servidor
    $.connection.hub.start().done();
});

