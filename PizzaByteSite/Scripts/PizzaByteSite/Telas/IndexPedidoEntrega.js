var optionsFuncionarios = "";
var numPag = 1;

// Busca as entregas
function BuscarEntregas(nPagina) {

    ExibirCarregando("divCarregando");
    PaginarPesquisa(0, nPagina, "BuscarEntregas");
    $("#tblResultados tbody").empty();
    numPag = nPagina;

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/PedidoEntrega/ObterListaFiltrada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            ValorRetorno: $("#ValorRetorno").val(),
            IdFuncionario: $("#IdFuncionario").val(),
            Conferido: $("#Conferido").val(),
            DataInicio: $("#DataInicio").val(),
            DataFim: $("#DataFim").val(),
            ObterInativos: $("#ObterInativos").val(),
            Pagina: nPagina,
            NaoPaginaPesquisa: false
        }),
        traditional: true,
        success: function (dados) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de entregas. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de entregas. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (dados.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontradas entregas com os filtros preenchidos", "Pesquisa de entregas");
                } else {
                    for (var i = 0; i < dados.ListaEntidades.length; i++) {

                        var botoes = "", classeEstornado = "";
                        var comboFunc = "<select class='form-control cbFuncionarios' onchange='AlterarFuncionario(\""
                            + dados.ListaEntidades[i].Id + "\")' id='cbFuncionario" + dados.ListaEntidades[i].Id + "'";

                        //Se estiver cancelado
                        if (!dados.ListaEntidades[i].Inativo) {
                            botoes = " <a class='btn btn-sm btn-default' href='../PedidoEntrega/Visualizar/"
                                + dados.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                                + " <a class='btn btn-sm btn-info' href='../PedidoEntrega/Editar/"
                                + dados.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>";

                        } else {
                            botoes = " <a class='btn btn-sm btn-default' href='../PedidoEntrega/Visualizar/"
                                + dados.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                                + " <a class='btn btn-sm btn-info' disabled><i class='fa fa-pencil'></i></a>";
                            classeEstornado = "dashed";
                            comboFunc += " disabled";
                        }

                        if (dados.ListaEntidades[i].Conferido || dados.ListaEntidades[i].Inativo) {
                            botoes += " <button class='btn btn-sm btn-warning' disabled "
                                + "title='Conferir retorno'><i class='fa fa-calculator'></i></button>";
                        } else {
                            botoes += " <button class='btn btn-sm btn-warning' onclick='ConferirRetorno(\""
                                + dados.ListaEntidades[i].Id + "\", \"" + dados.ListaEntidades[i].IdPedido + "\")'"
                                + "title='Conferir retorno'><i class='fa fa-calculator'></i></button>";
                        }

                        comboFunc += ">" + optionsFuncionarios + "</select>";
                        $("#tblResultados tbody").append("<tr class='" + classeEstornado + "'>"
                            + "<td>" + ConverterDataHoraJson(dados.ListaEntidades[i].DataInclusao) + "</td>"
                            + "<td>" + dados.ListaEntidades[i].ClienteEndereco.Endereco.Logradouro + ", "
                            + dados.ListaEntidades[i].ClienteEndereco.NumeroEndereco + " - "
                            + dados.ListaEntidades[i].ClienteEndereco.Endereco.Bairro + "/"
                            + dados.ListaEntidades[i].ClienteEndereco.Endereco.Cidade + "</td>"
                            + "<td>" + comboFunc + "</td>"
                            + "<td>" + dados.ListaEntidades[i].ValorRetorno.toFixed(2).replace(".", ",") + "</td>"
                            + "<td>" + botoes + "</td></tr>");

                        $("#cbFuncionario" + dados.ListaEntidades[i].Id).val(dados.ListaEntidades[i].IdFuncionario);
                        if (dados.ListaEntidades[i].IdFuncionario != "" &&
                            dados.ListaEntidades[i].IdFuncionario != null &&
                            dados.ListaEntidades[i].IdFuncionario != "00000000-0000-0000-0000-000000000000") {

                            $("#cbFuncionario" + dados.ListaEntidades[i].Id).attr("disabled", "disabled");
                        }

                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(dados.NumeroPaginas, nPagina, "BuscarEntregas");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de entregas. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}

// Preenche a combo de funcionários disponíveis
function PreencherComboFuncionarios(listaFuncionarios) {
    for (var i = 0; i < listaFuncionarios.length; i++) {
        optionsFuncionarios += "<option value='" + listaFuncionarios[i].Value.replace("null", "") + "'>";
        optionsFuncionarios += listaFuncionarios[i].Text + "</option>";
    }
}

// Conferir o valor retornado da entrega
function ConferirRetorno(idEntrega, idPedido) {
    $("#txtIdEntrega").val(idEntrega);
    $("#txtValorRetornado").val("0,00");

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Pedido/ObterPedidoResumido",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            idPedido: idPedido,
        }),
        traditional: true,
        success: function (dados) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao obter os dados do pedido. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                return;
            }

            if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao obter os dados do pedido. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });
            } else {

                $("#cbPedidoIfood").prop("checked", dados.Entidade.PedidoIfood);
                $("#txtValorTotal").val(dados.Entidade.Total.toFixed(2).replace(".", ","));
                $("#txtTaxaEntrega").val(dados.Entidade.TaxaEntrega.toFixed(2).replace(".", ","));
                $("#txtDinheiro").val(dados.Entidade.RecebidoDinheiro.toFixed(2).replace(".", ","));
                $("#txtDebito").val(dados.Entidade.RecebidoDebito.toFixed(2).replace(".", ","));
                $("#txtCredito").val(dados.Entidade.RecebidoCredito.toFixed(2).replace(".", ","));
                $("#txtTroco").val(dados.Entidade.Troco.toFixed(2).replace(".", ","));

                $("#txtValorRetornado").val(dados.Entidade.RecebidoDinheiro.toFixed(2).replace(".", ","));
                $("#modalRetornoEntrega").modal("show");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao obter os dados do pedido. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });
        }
    });
}

// Consome o serviço para conferir o retorno
function ConfirmarRetorno() {
    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/PedidoEntrega/ConferirEntrega",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            idEntrega: $("#txtIdEntrega").val(),
            valorRetorno: $("#txtValorRetornado").val()
        }),
        traditional: true,
        success: function (dados) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao conferir a entrega. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                return;
            }

            if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao conferir a entrega. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });
            } else {
                toastr.options.preventDuplicates = true;
                toastr.info("Entrega conferida com sucesso.", "Tudo certo!");
                BuscarEntregas(numPag);
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao obter os dados do pedido. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });
        }
    });
}

// Altera o funcionário da entrega
function AlterarFuncionario(idEntrega) {
    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/PedidoEntrega/AlterarFuncionarioEntrega",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            idEntrega: idEntrega,
            idFuncionario: $("#cbFuncionario" + idEntrega).val()
        }),
        traditional: true,
        success: function (dados) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao alterar o profissional. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                return;
            }

            if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao alterar o profissional. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });
            } else {
                $("#cbFuncionario" + idEntrega).attr("disabled", "disabled");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao alterar o profissional. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });
        }
    });
}