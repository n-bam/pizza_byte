function BuscarTaxasEntrega(nPagina) {

    ExibirCarregando();
    PaginarPesquisa(0, nPagina, "BuscarTaxaEntregas");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/TaxaEntrega/ObterListaFiltrada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            Bairro: $("#Bairro").val(),
            TaxaInicial: $("#TaxaInicial").val(),
            TaxaFinal: $("#TaxaFinal").val(),
            Pagina: nPagina,
            NaoPaginaPesquisa: false
        }),
        traditional: true,
        success: function (dados, status, request) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de taxas de entrega. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                EsconderCarregando();
                return;
            }

            if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao fazer a pesquisa de taxas de entrega. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando();
            } else {

                if (dados.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontrados taxas de entrega com os filtros preenchidos", "Pesquisa de taxas de entrega",
                        {
                            "preventDuplicates": true
                        });
                } else {
                    for (var i = 0; i < dados.ListaEntidades.length; i++) {

                        $("#tblResultados tbody").append("<tr>"
                            + "<td>" + dados.ListaEntidades[i].BairroCidade.split("_")[0] + "</td>"
                            + "<td>" + dados.ListaEntidades[i].BairroCidade.split("_")[1] + "</td>"
                            + "<td>" + dados.ListaEntidades[i].ValorTaxa.toFixed(2).replace(".", ",") + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../TaxaEntrega/Visualizar/"
                            + dados.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + " <a class='btn btn-sm btn-info' href='../TaxaEntrega/Editar/"
                            + dados.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>"
                            + " <a class='btn btn-sm btn-danger' href='../TaxaEntrega/Excluir/"
                            + dados.ListaEntidades[i].Id + "?Descricao="
                            + dados.ListaEntidades[i].BairroCidade.split("_")[0] + " (cidade "
                            + dados.ListaEntidades[i].BairroCidade.split("_")[1] + ")"
                            + "'><i class='fa fa-trash'></i></a>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando();
                PaginarPesquisa(dados.NumeroPaginas, nPagina, "BuscarTaxaEntregas");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de taxas de entrega. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error + " " + request.abort + " " + status,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando();
        }
    });
}