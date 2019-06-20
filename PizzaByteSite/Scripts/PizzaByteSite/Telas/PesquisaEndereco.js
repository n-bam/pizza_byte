// -------> Obtém um endereço pelo CEP
// Obtém um endereço por CEP completo
function ObterEnderecoPorCep(cepPesquisado) {
    $("#divCarregandoEndereco").show();
    $.ajax({
        type: "GET",
        url: RetornarEndereco() + "/Cep/ObterPorCep",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: {
            cep: cepPesquisado,
        },
        traditional: true,
        success: function (dados, status, request) {
            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de CEP. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                $("#divCarregandoEndereco").hide();
            } else if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao fazer a pesquisa de CEP. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                $("#divCarregandoEndereco").hide();
            } else {

                if (dados.Entidade != null) {
                    $(".Endereco_Logradouro").val(dados.Entidade.Logradouro);
                    $(".Endereco_Cidade").val(dados.Entidade.Cidade);
                    $(".Endereco_Bairro").val(dados.Entidade.Bairro);
                    $(".Endereco_Id").val(dados.Entidade.Id);

                    $("#divCarregandoEndereco").hide();
                } else {
                    toastr.success("CEP não encontrado. Para adicionar, preencha os dados do endereço.", "Pesquisa de CEP",
                        {
                            "preventDuplicates": true
                        });

                    $(".Endereco_Cep").focus();
                    $(".Endereco_Id").val("00000000-0000-0000-0000-000000000000");
                    $(".Endereco_Logradouro").val("");
                    $(".Endereco_Cidade").val("Americana");
                    $(".Endereco_Bairro").val("");
                    $("#divCarregandoEndereco").hide();
                }
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de CEP. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: " + error,
                icon: "warning",
                button: "Ok",
            });
        }
    });
}

var colunas = [{ name: 'Logradouro', minWidth: '400px' },
{ name: 'Bairro', minWidth: '300px' },
{ name: 'Cidade', minWidth: '200px' }];

// Obtém uma lista de possível endereços referentes ao logradouro parcial digitado
function ObterListaEnderecoPorLogradouro(pesquisa, cidade) {
    $("#divCarregandoEndereco").show();
    LimparAutocomplete();

    if (pesquisa.length > 3) {
        $.ajax({
            type: "GET",
            url: RetornarEndereco() + "/Cep/ObterListaCepPorLogradouro",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: {
                logradouro: pesquisa,
                cidade: cidade
            },
            traditional: true,
            success: function (dados, status, request) {
                if (dados.Error != undefined) {
                    swal({
                        title: "Ops...",
                        html: true,
                        text: "Ocorreu um problema ao fazer a pesquisa de endereço. \n"
                            + "\n Mensagem de retorno: " + dados.Error,
                        icon: "warning",
                        button: "Ok"
                    });

                } else if (!dados.Retorno) {
                    swal({
                        title: "Ops...",
                        text: "Ocorreu um problema ao fazer a pesquisa de endereço. \n"
                            + "Se o problema continuar, entre em contato com o suporte. \n"
                            + "Mensagem de retorno: " + dados.Mensagem,
                        icon: "warning",
                        button: "Ok",
                    });

                } else {

                    if (dados.ListaEntidades.length > 0) {

                        $("#btnBuscarEndereco").focus();
                        var maxi = (dados.ListaEntidades.length > 5) ? 5 : dados.ListaEntidades.length;

                        var enderecos = [];
                        for (var i = 0; i < maxi; i++) {
                            enderecos.push([
                                dados.ListaEntidades[i].Logradouro,
                                dados.ListaEntidades[i].Bairro,
                                dados.ListaEntidades[i].Cidade,
                                dados.ListaEntidades[i].Id,
                                dados.ListaEntidades[i].Cep,
                            ]);
                        }

                        $(".Endereco_Logradouro").mcautocomplete({
                            showHeader: true,
                            columns: colunas,
                            source: enderecos,
                            select: function (event, ui) {
                                event.preventDefault();

                                //setar valores e desabilitar campo descrição
                                $('.Endereco_Logradouro').val(ui.item[0]);
                                $('.Endereco_Bairro').val(ui.item[1]);
                                $('.Endereco_Cidade').val(ui.item[2]);
                                $(".Endereco_Id").val(ui.item[3]);
                                $(".Endereco_Cep").val(ui.item[4]);
                            },
                            focus: function (event, ui) {
                                event.preventDefault();
                            },
                            minLength: 0
                        }).on("focus", function () {
                            if ($(".Endereco_Logradouro").val().length > 0) {
                                $(this).mcautocomplete("search", "");
                            }
                        });

                        $(".Endereco_Logradouro").focus();
                        $("#divCarregandoEndereco").hide();
                    } else {
                        $("#divCarregandoEndereco").hide();
                        toastr.success("Endereço não encontrado. Para adicionar, preencha os dados do endereço.", "Pesquisa de endereço",
                            {
                                "preventDuplicates": true
                            });

                        $(".Endereco_Logradouro").focus();
                        $(".Endereco_Id").val("00000000-0000-0000-0000-000000000000");
                        $(".Endereco_Cep").val("");
                    }
                }
            },
            error: function (request, status, error) {

                $("#divCarregandoEndereco").hide();

                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao fazer a pesquisa de CEP. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + error,
                    icon: "warning",
                    button: "Ok",
                });
            }
        });

    } else {
        $("#divCarregandoEndereco").hide();

        // Se tiver manos que 3 caracteres, pedir para digitar novamente
        swal({
            title: "Pesquisa inválida",
            text: "Por favor, digite ao menos 3 letrar para pesquisar o endereço.",
            icon: "warning",
            button: "Ok",
        });
    }
}

// Limpa a lista do autocomplete
function LimparAutocomplete() {

    // Limpar autocomplete
    $(".Endereco_Logradouro").mcautocomplete({
        showHeader: true,
        columns: colunas,
        source: []
    });
}