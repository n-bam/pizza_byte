function BuscarPedidos(nPagina) {

    ExibirCarregando("divCarregando");
    PaginarPesquisa(0, nPagina, "BuscarPedidos");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Pedido/ObterListaFiltrada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            IdCliente: $("#IdCliente").val(),
            Tipo: $("#Tipo").val(),
            Total: $("#Total").val(),
            DataInicio: $("#DataInicio").val(),
            DataFim: $("#DataFim").val(),
            PedidoIfood: $("#PedidoIfood").val(),
            Tipo: $("#Tipo").val(),
            ObterInativos: $("#ObterInativos").val(),
            Pagina: nPagina,
            NaoPaginaPesquisa: false
        }),
        traditional: true,
        success: function (dados, status, request) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de pedidos. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de pedidos. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (dados.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontrados pedidos com os filtros preenchidos", "Pesquisa de pedidos");
                } else {
                    for (var i = 0; i < dados.ListaEntidades.length; i++) {

                        var textoTipo = "";
                        switch (dados.ListaEntidades[i].Tipo) {
                            case 1:
                                textoTipo = "Balcão";
                                break;

                            case 2:
                                textoTipo = "Retirada";
                                break;

                            case 3:
                                textoTipo = "Entrega";
                                break;

                            default:
                                textoTipo = "Não identificado";
                        }

                        var botoes = "", classeEstorno = "";
                        if (!dados.ListaEntidades[i].Inativo) {
                            botoes = " <a class='btn btn-sm btn-info' href='../Pedido/Editar/"
                                + dados.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>"
                                + " <a class='btn btn-sm btn-danger' title='Estornar' href='../Pedido/Cancelar/"
                                + dados.ListaEntidades[i].Id + "?Descricao="
                                + dados.ListaEntidades[i].NomeCliente + " feito no dia"
                                + ConverterDataHoraJson(dados.ListaEntidades[i].DataInclusao) + "'><i class='fa fa-ban'></i></a>";
                        } else {
                            classeEstorno = "dashed";
                            botoes = " <a class='btn btn-sm btn-info' disabled><i class='fa fa-pencil'></i></a>"
                                + " <a class='btn btn-sm btn-danger' title='Estornar' disabled><i class='fa fa-ban'></i></a>";
                        }

                        $("#tblResultados tbody").append("<tr class='" + classeEstorno + "'>"
                            + "<td>" + ConverterDataHoraJson(dados.ListaEntidades[i].DataInclusao) + "</td>"
                            + "<td>" + dados.ListaEntidades[i].NomeCliente + "</td>"
                            + "<td>" + textoTipo + "</td>"
                            + "<td>" + dados.ListaEntidades[i].Total.toFixed(2).replace(".", ",") + "</td>"
                            + "<td>" + ((dados.ListaEntidades[i].PedidoIfood) ? "Sim" : "Não") + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../Pedido/Visualizar/"
                            + dados.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + botoes + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(dados.NumeroPaginas, nPagina, "BuscarPedidos");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de pedidos. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}

// Monta a tela para impressão do cupom
function ImprimirPedido(impr) {
    if (impr != "" && impr != null) {
        var winparams = 'dependent=yes,locationbar=no,scrollbars=yes,menubar=yes,' +
            'resizable,screenX=50,screenY=50,width=400,height=1000';

        var htmlPop = "<html><body>" + impr + "</body></html>";

        var printWindow = window.open("", "PNG", winparams);
        printWindow.document.write(htmlPop);
        printWindow.document.close();
        printWindow.focus();

        printWindow.onload = new function () {
            setTimeout(function () {
                printWindow.print();
                printWindow.close();
            }, 700);
        };
    }
}