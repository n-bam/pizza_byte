function BuscarContaReceber(nPagina) {

    ExibirCarregando("divCarregando");
    PaginarPesquisa(0, nPagina, "BuscarContaReceber");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/ContaReceber/ObterListaFiltrada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            Descricao: $("#Descricao").val(),
            DataInicio: $("#DataInicio").val(),
            DataFim: $("#DataFim").val(),
            Valor: $("#Valor").val(),
            PesquisarPor: $("#PesquisarPor").val(),
            PrecoInicial: $("#PrecoInicial").val(),
            PrecoFinal: $("#PrecoFinal").val(),
            Status: $("#Status").val(),
            Pagina: nPagina,
            NaoPaginaPesquisa: false
        }),
        traditional: true,
        success: function (dados, status, request) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de conta. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de conta. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (dados.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontradas conta com os filtros preenchidos", "Pesquisa de conta");
                } else {
                    for (var i = 0; i < dados.ListaEntidades.length; i++) {

                        var textoTipo = "";
                        switch (dados.ListaEntidades[i].Status) {
                            case 1:
                                textoTipo = "Aberta";
                                break;

                            case 2:
                                textoTipo = "Paga";
                                break;

                            case 3:
                                textoTipo = "Estornada";
                                break;

                            case 4:
                                textoTipo = "Perdida";
                                break;

                            default:
                                textoTipo = "Não identificado";
                        }

                        var classeEstorno = "";
                        if (dados.ListaEntidades[i].Status == 3) {
                            classeEstorno = "dashed";

                        }

                        $("#tblResultados tbody").append("<tr class='" + classeEstorno + "'>"
                            + "<td>" + ConverterDataJson(dados.ListaEntidades[i].DataVencimento) + "</td>"
                            + "<td>" + dados.ListaEntidades[i].Descricao + "</td>"
                            + "<td>" + textoTipo + "</td>"
                            + "<td>" + dados.ListaEntidades[i].Valor.toFixed(2).replace(".", ",") + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../ContaReceber/Visualizar/"
                            + dados.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + " <a class='btn btn-sm btn-info' href='../ContaReceber/Editar/"
                            + dados.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>"
                            + " <a class='btn btn-sm btn-danger' href='../ContaReceber/Excluir/"
                            + dados.ListaEntidades[i].Id + "?Descricao="
                            + dados.ListaEntidades[i].Descricao + "'><i class='fa fa-trash'></i></a>"
                            + "</td></tr>");
                    }

                    PaginarPesquisa(dados.NumeroPaginas, nPagina, "BuscarContaReceber");
                }

                EsconderCarregando("divCarregando");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de conta. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}