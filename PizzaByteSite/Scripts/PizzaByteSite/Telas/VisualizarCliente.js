function BuscarEnderecos(idCliente, nPagina) {

    ExibirCarregando("divCarregando");
    $("#tblEnderecos tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/ClienteEndereco/ObterListaEnderecosCliente",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            idCliente: idCliente,
            Pagina: nPagina
        }),
        traditional: true,
        success: function (dados, status, request) {
            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao obter a lista de endereços. \n"
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
                    text: "Ocorreu um problema ao obter a lista de endereços. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (dados.ListaEntidades.length == 0) {
                    toastr.info("Não foram encontrados endereços para este cliente", "Pesquisa de endereços",
                        {
                            "preventDuplicates": true
                        });

                    $("#tblEnderecos tbody").append("<tr><td>Não existem endereços cadastrados</td></tr>");
                } else {
                    for (var i = 0; i < dados.ListaEntidades.length; i++) {

                        $("#tblEnderecos tbody").append("<tr>"
                            + "<td>" + dados.ListaEntidades[i].Endereco.Logradouro + "</td>"
                            + "<td>" + dados.ListaEntidades[i].NumeroEndereco + "</td>"
                            + "<td>" + dados.ListaEntidades[i].Endereco.Bairro + "</td>"
                            + "<td>" + dados.ListaEntidades[i].Endereco.Cidade + "</td>"
                            + "<td>" + ((dados.ListaEntidades[i].Inativo) ? "Sim" : "Não") + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='/ClienteEndereco/Visualizar/"
                            + dados.ListaEntidades[i].Id + "?nomeCliente=" + $("#Nome").val() + "'><i class='fa fa-eye'></i></a>"
                            + " <a class='btn btn-sm btn-info' href='/ClienteEndereco/Editar/"
                            + dados.ListaEntidades[i].Id + "?nomeCliente=" + $("#Nome").val() + "'><i class='fa fa-pencil'></i></a>"
                            + " <a class='btn btn-sm btn-danger' href='/ClienteEndereco/Excluir/"
                            + dados.ListaEntidades[i].Id + "?Descricao="
                            + dados.ListaEntidades[i].Endereco.Logradouro + " do cliente " + $("#Nome").val()
                            + "&idPai=" + $("#Id").val() + "'><i class='fa fa-trash'></i></a>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(dados.NumeroPaginas, nPagina, "BuscarEnderecos");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de CEPs. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}