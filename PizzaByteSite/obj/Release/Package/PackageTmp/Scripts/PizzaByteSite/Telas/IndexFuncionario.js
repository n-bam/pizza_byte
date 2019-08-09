﻿function BuscarFuncionarios(nPagina) {

    ExibirCarregando();
    PaginarPesquisa(0, nPagina, "BuscarFuncionarios");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarNome() + "/Funcionario/ObterListaFiltrada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            Nome: $("#Nome").val(),
            ObterInativos: $("#ObterInativos").val(),
            Telefone: $("#Telefone").val(),
            Pagina: nPagina,
            NaoPaginaPesquisa: false
        }),
        traditional: true,
        success: function (dados, status, request) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de funcionarios. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de funcionarios. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando();
            } else {

                if (dados.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontrados funcionarios com os filtros preenchidos", "Pesquisa de funcionarios");
                } else {
                    for (var i = 0; i < dados.ListaEntidades.length; i++) {

                        $("#tblResultados tbody").append("<tr>"
                            + "<td>" + dados.ListaEntidades[i].Nome + "</td>"
                            + "<td>" + dados.ListaEntidades[i].Telefone + "</td>"
                            + "<td>" + ((dados.ListaEntidades[i].Inativo) ? "Sim" : "Não") + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../Funcionarios/Visualizar/"
                            + dados.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + " <a class='btn btn-sm btn-info' href='../Funcionario/Editar/"
                            + dados.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>"
                            + " <a class='btn btn-sm btn-danger' href='../Funcionario/Excluir/"
                            + dados.ListaEntidades[i].Id + "?Descricao="
                            + dados.ListaEntidades[i].Nome + "'><i class='fa fa-trash'></i></a>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando();
                PaginarPesquisa(dados.NumeroPaginas, nPagina, "BuscarFuncionarios");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de funcionarios. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error + " " + request.abort + " " + status,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando();
        }
    });
}