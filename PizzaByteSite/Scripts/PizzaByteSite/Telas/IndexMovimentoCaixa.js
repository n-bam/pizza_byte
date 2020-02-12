// Obtem os totais de cada forma de pagamento
function BuscarFormasPagamento() {

    ExibirCarregando("divCarregando");

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/MovimentoCaixa/ObterFormasPagamento",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            dataCaixa: $("#txtDataCaixa").val()
        }),
        traditional: true,
        success: function (dados, status, request) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao obter os movimentos de caixa. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                EsconderCarregando("divCarregando");
                return;
            }

            if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao obter os movimentos de caixa. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                $("#txtDinheiro").val(dados.RecebidoDinheiro.toFixed(2).replace(".", ","));
                $("#txtDebito").val(dados.RecebidoDebito.toFixed(2).replace(".", ","));
                $("#txtCredito").val(dados.RecebidoCredito.toFixed(2).replace(".", ","));
                $("#txtTaxaEntrega").val(dados.TaxaEntrega.toFixed(2).replace(".", ","));
                $("#txtTroco").val(dados.Troco.toFixed(2).replace(".", ","));
                $("#txtTotalVendas").val(dados.TotalVendas.toFixed(2).replace(".", ","));
                CalcularTotalCaixa();
            }

            EsconderCarregando("divCarregando");
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao obter os movimentos de caixa. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}

// Busca a lista de movimentações feitas
function BuscarMovimentacoes() {

    ExibirCarregando("divCarregandoMovimentacoes");
    $("#tblMovimentos tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/MovimentoCaixa/ObterMovimentosDia",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            dataCaixa: $("#txtDataCaixa").val()
        }),
        traditional: true,
        success: function (dados, status, request) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao obter os movimentos de caixa. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                EsconderCarregando("divCarregandoMovimentacoes");
                return;
            }

            if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao obter os movimentos de caixa. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregandoMovimentacoes");
            } else {

                var totalSaida = 0;
                var totalEntrada = 0;

                for (var i = 0; i < dados.ListaEntidades.length; i++) {
                    var corLinha = "";
                    if (dados.ListaEntidades[i].Valor > 0) {
                        corLinha = "#333";
                        totalEntrada += dados.ListaEntidades[i].Valor;
                    } else {
                        corLinha = "#8B0000";
                        totalSaida += dados.ListaEntidades[i].Valor;
                    }

                    $("#tblMovimentos tbody").append("<tr style='color:" + corLinha + "'>"
                        + "<td>" + dados.ListaEntidades[i].Justificativa + "</td>"
                        + "<td>" + dados.ListaEntidades[i].Valor.toFixed(2).replace(".", ",") + "</td>"
                        + "<td>" + ConverterDataHoraJson(dados.ListaEntidades[i].DataInclusao) + "</td>"
                        + "</td></tr>");
                }

                $("#txtEntradas").val(totalEntrada.toFixed(2).replace(".", ","));
                $("#txtSaidas").val(totalSaida.toFixed(2).replace(".", ","));
                CalcularTotalCaixa();
            }

            EsconderCarregando("divCarregandoMovimentacoes");
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao obter os movimentos de caixa. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregandoMovimentacoes");
        }
    });
}

// Busca a lista de entregas feitas
function BuscarTotaisProfissionais() {

    ExibirCarregando("divCarregando");
    $("#tblProfissionais tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/MovimentoCaixa/ObterTotalProfissionais",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            dataCaixa: $("#txtDataCaixa").val()
        }),
        traditional: true,
        success: function (dados, status, request) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao obter as entregas. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                EsconderCarregando("divCarregando");
                return;
            }

            if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao obter as entregas. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {
                
                for (var i = 0; i < dados.ListaTotais.length; i++) {

                    $("#tblProfissionais tbody").append("<tr>"
                        + "<td>" + dados.ListaTotais[i].NomeProfissional + "</td>"
                        + "<td>" + dados.ListaTotais[i].TotalEntregas.toFixed(2).replace(".", ",") + "</td>"
                        + "</td></tr>");
                }
            }

            EsconderCarregando("divCarregando");
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao obter as entregas. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}

// inclui uma movimentação de caixa
function IncluirMovimento() {
    ExibirCarregando("divCarregandoIncluir");

    var movimentoDto = new Object();
    movimentoDto.Justificativa = $("#txtJustificativa").val()
    movimentoDto.Valor = $("#txtValor").val()

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/MovimentoCaixa/IncluirMovimento",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            movimentoDto: movimentoDto,
            indicadorSaida: $("#txtIndicadorSaida").val()
        }),
        traditional: true,
        success: function (dados, status, request) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao incluir o movimento de caixa. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                EsconderCarregando("divCarregandoIncluir");
                return;
            }

            if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao incluir o movimento de caixa. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregandoIncluir");
            } else {
                BuscarMovimentacoes();
                $("#modalIncluir").modal("hide");
                LimparModalIncluir()
            }

            EsconderCarregando("divCarregandoIncluir");
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao incluir o movimento de caixa. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregandoIncluir");
        }
    });
}

// Limpa os campos da modal de incluir uma movimentação
function LimparModalIncluir() {
    $("#txtJustificativa").val("");
    $("#txtValor").val("0,00");
    $("#txtIndicadorSaida").val("");
}

// Calcula o valor total em caixa
function CalcularTotalCaixa() {
    var dinheiro = ($("#txtDinheiro").val() == "" || $("#txtDinheiro").val() == null) ? "0" : $("#txtDinheiro").val();
    dinheiro = parseFloat(dinheiro.replace(",", "."));

    var troco = ($("#txtTroco").val() == "" || $("#txtTroco").val() == null) ? "0" : $("#txtTroco").val();
    troco = parseFloat(troco.replace(",", "."));

    var saidas = ($("#txtSaidas").val() == "" || $("#txtSaidas").val() == null) ? "0" : $("#txtSaidas").val();
    saidas = parseFloat(saidas.replace(",", "."));

    var entrada = ($("#txtEntradas").val() == "" || $("#txtEntradas").val() == null) ? "0" : $("#txtEntradas").val();
    entrada = parseFloat(entrada.replace(",", "."));

    var totalCaixa = dinheiro - troco + entrada + saidas;

    $("#txtTotalEmCaixa").val(totalCaixa.toFixed(2).replace(".", ","))
}