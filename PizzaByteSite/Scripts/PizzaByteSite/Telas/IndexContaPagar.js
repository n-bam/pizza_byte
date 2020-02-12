function BuscarContaPagar(nPagina) {

    ExibirCarregando("divCarregando");
    PaginarPesquisa(0, nPagina, "BuscarContaPagar");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/ContaPagar/ObterListaFiltrada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            Descricao: $("#Descricao").val(),
            PesquisarPor: $("#PesquisarPor").val(),
            DataInicio: $("#DataInicio").val(),
            DataFim: $("#DataFim").val(),
            Valor: $("#Valor").val(),
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
                    text: "Ocorreu um problema ao fazer a pesquisa de contas. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de contas. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (dados.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontrados contas com os filtros preenchidos", "Pesquisa de contas");
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

                        $("#tblResultados tbody").append("<tr>"
                            + "<td>" + ConverterDataJson(dados.ListaEntidades[i].DataVencimento) + "</td>"
                            + "<td>" + dados.ListaEntidades[i].Descricao + "</td>"
                            + "<td>" + textoTipo + "</td>"
                            + "<td>" + dados.ListaEntidades[i].Valor.toFixed(2).replace(".", ",") + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../ContaPagar/Visualizar/"
                            + dados.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + " <a class='btn btn-sm btn-info' href='../ContaPagar/Editar/"
                            + dados.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>"
                            + " <a class='btn btn-sm btn-danger' href='../ContaPagar/Excluir/"
                            + dados.ListaEntidades[i].Id + "?Descricao="
                            + dados.ListaEntidades[i].Descricao + "'><i class='fa fa-trash'></i></a>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(dados.NumeroPaginas, nPagina, "BuscarContaPagar");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de contas. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}

var colunas = [{ name: 'NomeFantasia', minWidth: '400px' },
{ name: 'Cnpj', minWidth: '300px' }];

// Obtém uma lista de fornecedores
function ObterListaFornecedores(pesquisa) {
    $("#divCarregandoFornecedor").show();
    LimparAutocompleteFornecedor();

    if (pesquisa.length >= 3) {
        $.ajax({
            type: "GET",
            url: RetornarEndereco() + "/Fornecedor/ObterListaFiltrada",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: {
                NomeFantasiaCnpj: $("#NomeFantasia").val(),
                ObterInativos: false
            },
            traditional: true,
            success: function (dados, status, request) {
                if (dados.Error != undefined) {
                    swal({
                        title: "Ops...",
                        html: true,
                        text: "Ocorreu um problema ao fazer a pesquisa de fornecedores. \n"
                            + "\n Mensagem de retorno: " + dados.Error,
                        icon: "warning",
                        button: "Ok"
                    });

                } else if (!dados.Retorno) {
                    swal({
                        title: "Ops...",
                        text: "Ocorreu um problema ao fazer a pesquisa de fornecedores. \n"
                            + "Se o problema continuar, entre em contato com o suporte. \n"
                            + "Mensagem de retorno: " + dados.Mensagem,
                        icon: "warning",
                        button: "Ok",
                    });

                } else {

                    if (dados.ListaEntidades.length > 0) {

                        $("#btnBuscarFornecedor").focus();

                        var fornecedor = [];
                        for (var i = 0; i < dados.ListaEntidades.length; i++) {
                            fornecedor.push([
                                dados.ListaEntidades[i].NomeFantasia,
                                FormatarCnpj(dados.ListaEntidades[i].Cnpj),
                                dados.ListaEntidades[i].Id
                            ]);
                        }

                        $("#NomeFantasia").mcautocomplete({
                            showHeader: true,
                            columns: colunas,
                            source: fornecedor,
                            select: function (event, ui) {
                                //setar valores e desabilitar campo descrição
                                $('#IdFornecedor').val(ui.item[2]);
                                $('#NomeFantasia').val(ui.item[0] + " (CNPJ: " + ui.item[1] + ")");
                                $("#NomeFantasia").attr("readonly", "readonly");
                                $("#btnBuscarFornecedor").html("<i class='fa fa-eraser'></i> Limpar");

                                event.preventDefault();
                            },
                            focus: function (event, ui) {
                                event.preventDefault();
                            },
                            minLength: 0
                        }).on("focus", function () {
                            if ($("#NomeFantasia").val().length > 0) {
                                $(this).mcautocomplete("search", "");
                            }
                        });

                        $("#NomeFantasia").focus();
                        $("#divCarregandoFornecedor").hide();
                    } else {
                        $("#divCarregandoFornecedor").hide();
                        toastr.success("Fornecedor não encontrado. Para adicionar, preencha os dados do fornecedor.", "Pesquisa de fornecedor",
                            {
                                "preventDuplicates": true
                            });

                        $("#NomeFantasia").focus();
                        $("#IdFornecedor").val("null");
                    }
                }
            },
            error: function (request, status, error) {

                $("#divCarregandoFornecedor").hide();

                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao fazer a pesquisa de fornecedores. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + error,
                    icon: "warning",
                    button: "Ok",
                });
            }
        });

    } else {
        $("#divCarregandoFornecedor").hide();

        // Se tiver manos que 3 caracteres, pedir para digitar novamente
        swal({
            title: "Pesquisa inválida",
            text: "Por favor, digite ao menos 3 letrar para pesquisar o fornecedor.",
            icon: "warning",
            button: "Ok",
        });
    }
}

// Limpa a lista do autocomplete
function LimparAutocompleteFornecedor() {
    // Limpar autocomplete
    $("#NomeFantasia").mcautocomplete({
        showHeader: true,
        columns: colunas,
        source: []
    });
}

function IniciarAutoComplete() {
    if ($("#IdFornecedor").val() != null && $("#IdFornecedor").val() != "") {
        $("#NomeFantasia").attr("readonly", "readonly");
        $("#btnBuscarFornecedor").html("<i class='fa fa-eraser'></i> Limpar");
    }
}