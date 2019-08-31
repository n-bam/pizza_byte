function BuscarLogs(nPagina) {

    ExibirCarregando();
    PaginarPesquisa(0, nPagina, "BuscarLogs");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Log/ObterListaFiltrada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            Mensagem: $("#Mensagem").val(),
            IdUsuario: $("#IdUsuario").val(),
            DataInicial: $("#DataInicial").val(),
            DataFinal: $("#DataFinal").val(),
            Recurso: $("#Recurso").val(),
            Pagina: nPagina,
            NaoPaginaPesquisa: false
        }),
        traditional: true,
        success: function (dados, status, request) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de LOGs. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de LOGs. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando();
            } else {

                if (dados.ListaEntidades.length == 0) {
                    toastr.info("Não foram encontrados LOGs com os filtros preenchidos", "Pesquisa de LOGs");
                } else {
                    for (var i = 0; i < dados.ListaEntidades.length; i++) {

                        var nomeRecurso = $("#Recurso [value=" + dados.ListaEntidades[i].Recurso + "]").text();
                        var nomeUsuario = $("#IdUsuario [value=" + dados.ListaEntidades[i].IdUsuario + "]").text();

                        $("#tblResultados tbody").append("<tr>"
                            + "<td>" + ConverterDataHoraJson(dados.ListaEntidades[i].DataInclusao) + "</td>"
                            + "<td>" + dados.ListaEntidades[i].Mensagem + "</td>"
                            + "<td>" + nomeUsuario + "</td>"
                            + "<td>" + nomeRecurso + "</td>"
                            + "<td>" + dados.ListaEntidades[i].IdEntidade + "</td>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando();
                PaginarPesquisa(dados.NumeroPaginas, nPagina, "BuscarLogs");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de LOGs. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error + " " + request.abort + " " + status,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando();
        }
    });
}