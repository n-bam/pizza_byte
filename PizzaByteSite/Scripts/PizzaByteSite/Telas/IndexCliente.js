function BuscarClientes(nPagina) {

    ExibirCarregando("divCarregando");
    PaginarPesquisa(0, nPagina, "BuscarClientes");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Cliente/ObterListaFiltrada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            Nome: $("#Nome").val(),
            Telefone: $("#Telefone").val(),
            Cpf: $("#Cpf").val(),
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
                    text: "Ocorreu um problema ao fazer a pesquisa de clientes. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de clientes. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (dados.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontrados clientes com os filtros preenchidos", "Pesquisa de clientes",
                        {
                            "preventDuplicates": true
                        });
                } else {
                    for (var i = 0; i < dados.ListaEntidades.length; i++) {

                        $("#tblResultados tbody").append("<tr>"
                            + "<td>" + dados.ListaEntidades[i].Nome + "</td>"
                            + "<td>" + FormatarTelefone(dados.ListaEntidades[i].Telefone) + "</td>"
                            + "<td>" + FormatarCpf(dados.ListaEntidades[i].Cpf) + "</td>"
                            + "<td>" + ((dados.ListaEntidades[i].Inativo) ? "Sim" : "Não") + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../Cliente/Visualizar/"
                            + dados.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + " <a class='btn btn-sm btn-info' href='../Cliente/Editar/"
                            + dados.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>"
                            + " <a class='btn btn-sm btn-danger' href='../Cliente/Excluir/"
                            + dados.ListaEntidades[i].Id + "?Descricao="
                            + dados.ListaEntidades[i].Nome + "'><i class='fa fa-trash'></i></a>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(dados.NumeroPaginas, nPagina, "BuscarClientes");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de clientes. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}