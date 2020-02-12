﻿function BuscarProdutos(nPagina) {

    ExibirCarregando("divCarregando");
    PaginarPesquisa(0, nPagina, "BuscarProdutos");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Produto/ObterListaFiltrada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            Descricao: $("#Descricao").val(),
            PrecoInicial: $("#PrecoInicial").val(),
            PrecoFinal: $("#PrecoFinal").val(),
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
                    text: "Ocorreu um problema ao fazer a pesquisa de produtos. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de produtos. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (dados.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontrados produtos com os filtros preenchidos", "Pesquisa de produtos");
                } else {
                    for (var i = 0; i < dados.ListaEntidades.length; i++) {

                        var textoTipo = "";
                        switch (dados.ListaEntidades[i].Tipo) {
                            case 1:
                                textoTipo = "Pizza";
                                break;

                            case 2:
                                textoTipo = "Bebida";
                                break;

                            default:
                                textoTipo = "Não identificado";
                        }

                        $("#tblResultados tbody").append("<tr>"
                            + "<td><p title='" + dados.ListaEntidades[i].Detalhes + "'>" + dados.ListaEntidades[i].Descricao + "</p></td>"
                            + "<td>" + textoTipo + "</td>"
                            + "<td>" + dados.ListaEntidades[i].Preco.toFixed(2).replace(".", ",") + "</td>"
                            + "<td>" + ((dados.ListaEntidades[i].Inativo) ? "Sim" : "Não") + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../Produto/Visualizar/"
                            + dados.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + " <a class='btn btn-sm btn-info' href='../Produto/Editar/"
                            + dados.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>"
                            + " <a class='btn btn-sm btn-danger' href='../Produto/Excluir/"
                            + dados.ListaEntidades[i].Id + "?Descricao="
                            + dados.ListaEntidades[i].Descricao + "'><i class='fa fa-trash'></i></a>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(dados.NumeroPaginas, nPagina, "BuscarProdutos");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de produtos. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}