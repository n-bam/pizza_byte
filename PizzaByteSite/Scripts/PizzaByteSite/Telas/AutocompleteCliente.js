var colunas = [{ name: 'Nome', minWidth: '400px' },
{ name: 'Telefone', minWidth: '300px' }];

// Obtém uma lista de clientes
function ObterListaClientes(pesquisa) {
    $("#divCarregandoCliente").show();
    LimparAutocompleteCliente();

    if (pesquisa.length >= 3) {
        $.ajax({
            type: "GET",
            url: RetornarEndereco() + "/Cliente/ObterListaNomeTelefone",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: {
                nomeTelefone: $("#NomeCliente").val(),
            },
            traditional: true,
            success: function (dados, status, request) {
                if (dados.Error != undefined) {
                    swal({
                        title: "Ops...",
                        html: true,
                        text: "Ocorreu um problema ao fazer a pesquisa de cliente. \n"
                            + "\n Mensagem de retorno: " + dados.Error,
                        icon: "warning",
                        button: "Ok"
                    });

                } else if (!dados.Retorno) {
                    swal({
                        title: "Ops...",
                        text: "Ocorreu um problema ao fazer a pesquisa de cliente. \n"
                            + "Se o problema continuar, entre em contato com o suporte. \n"
                            + "Mensagem de retorno: " + dados.Mensagem,
                        icon: "warning",
                        button: "Ok",
                    });

                } else {

                    if (dados.ListaEntidades.length > 0) {

                        $("#btnBuscarCliente").focus();

                        var clientes = [];
                        for (var i = 0; i < dados.ListaEntidades.length; i++) {
                            clientes.push([
                                dados.ListaEntidades[i].Nome,
                                FormatarTelefone(dados.ListaEntidades[i].Telefone),
                                dados.ListaEntidades[i].Id
                            ]);
                        }

                        $("#NomeCliente").mcautocomplete({
                            showHeader: true,
                            columns: colunas,
                            source: clientes,
                            select: function (event, ui) {
                                //setar valores e desabilitar campo descrição
                                $('#IdCliente').val(ui.item[2]);
                                $('#NomeCliente').val(ui.item[0]);
                                $("#NomeCliente").attr("readonly", "readonly");
                                $("#btnBuscarCliente").html("<i class='fa fa-eraser'></i> Limpar");

                                event.preventDefault();
                            },
                            focus: function (event, ui) {
                                event.preventDefault();
                            },
                            minLength: 0
                        }).on("focus", function () {
                            if ($("#NomeCliente").val().length > 0) {
                                $(this).mcautocomplete("search", "");
                            }
                        });

                        $("#NomeCliente").focus();
                        $("#divCarregandoCliente").hide();
                    } else {
                        $("#divCarregandoCliente").hide();
                        toastr.success("Cliente não encontrado. Para adicionar, preencha os dados do cliente.", "Pesquisa de cliente",
                            {
                                "preventDuplicates": true
                            });

                        $("#NomeCliente").focus();
                        $("#IdCliente").val("null");
                    }
                }
            },
            error: function (request, status, error) {

                $("#divCarregandoCliente").hide();

                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao fazer a pesquisa de cliente. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + error,
                    icon: "warning",
                    button: "Ok",
                });
            }
        });

    } else {
        $("#divCarregandoCliente").hide();

        // Se tiver manos que 3 caracteres, pedir para digitar novamente
        swal({
            title: "Pesquisa inválida",
            text: "Por favor, digite ao menos 3 letrar para pesquisar o cliente.",
            icon: "warning",
            button: "Ok",
        });
    }
}

// Limpa a lista do autocomplete
function LimparAutocompleteCliente() {
    // Limpar autocomplete
    $("#NomeCliente").mcautocomplete({
        showHeader: true,
        columns: colunas,
        source: []
    });
}
