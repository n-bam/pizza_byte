// -------> Busa uma lista de clientes
function BuscarClientesSelecao(nPagina) {

    ExibirCarregando("divCarregandoCliente");
    PaginarPesquisa(0, nPagina, "BuscarClientesSelecao", "divPaginasCliente", "paginacaoCliente");
    $("#tblPesquisaClientes tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Cliente/ObterListaFiltrada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            Nome: $("#txtNomeCliente").val(),
            Telefone: $("#txtTelefoneCliente").val(),
            Cpf: $("#txtCpfCliente").val(),
            ObterInativos: false,
            Pagina: nPagina,
            NaoPaginaPesquisa: false,
            NumeroItensPagina: 5
        }),
        traditional: true,
        success: function (dados) {
            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de clientes. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                EsconderCarregando("divCarregandoCliente");
                return;
            } else if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao fazer a pesquisa de clientes. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregandoCliente");
            } else {

                if (dados.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontrados clientes com os filtros preenchidos", "Pesquisa de clientes");

                } else {
                    for (var i = 0; i < dados.ListaEntidades.length; i++) {

                        $("#tblPesquisaClientes tbody").append("<tr>"
                            + "<td>" + dados.ListaEntidades[i].Nome + "</td>"
                            + "<td>" + FormatarTelefone(dados.ListaEntidades[i].Telefone) + "</td>"
                            + "<td>" + FormatarCpf(dados.ListaEntidades[i].Cpf) + "</td>"
                            + "<td style='text-align: center;'><button type='button' " +
                            "class='btn btn-sm btn-primary addCliente' onclick='SelecionarCliente(\""
                            + dados.ListaEntidades[i].Id + "\", \""
                            + dados.ListaEntidades[i].Nome + "\", \""
                            + dados.ListaEntidades[i].Cpf + "\", \""
                            + dados.ListaEntidades[i].Telefone + "\")' "
                            + "tittle='Adicionar ao pedido'>"
                            + "<i class='fa fa-check-circle'></i></button> "
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregandoCliente");
                PaginarPesquisa(dados.NumeroPaginas, nPagina, "BuscarClientesSelecao", "divPaginasCliente", "paginacaoCliente");
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

            EsconderCarregando("divCarregandoCliente");
        }
    });
}

// -------> Adiciona um cliente ao pedido
var listaEnderecos = [];
function SelecionarCliente(idCliente, nome, cpf, telefone) {
    $("#ClienteSelecionadoTab").html("Cliente selecionado");

    LimparCliente();

    $("#Cliente_Id").val(idCliente);
    $("#Cliente_Nome").val(nome);
    $("#Cliente_Cpf").val(FormatarCpf(cpf));
    $("#Cliente_Telefone").val(FormatarTelefone(telefone));

    BuscarEnderecosClientePedido(idCliente, true);
}

// -------> Busca os enderços de um cliente
function BuscarEnderecosClientePedido(idCliente, preencherEndereco) {
    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/ClienteEndereco/ObterListaEnderecosCliente",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            idCliente: idCliente
        }),
        traditional: true,
        success: function (dados) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de endereços do cliente. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                EsconderCarregando("divCarregandoCliente");
                return;
            } else if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao fazer a pesquisa de endereços do cliente. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregandoCliente");
            } else {

                if (dados.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontrados endereços para este cliente", "Pesquisa de endereços");

                    $("#optEnderecoCliente option").remove();
                    $("#optEnderecoCliente").append("<option>Adicionar novo endereço</option>");
                    $("#optEnderecoCliente").attr("disabled", "disabled");
                    $("#Entrega_ClienteEndereco_Endereco_Bairro").change();

                } else {

                    $("#optEnderecoCliente option").remove();
                    $("#optEnderecoCliente").removeAttr("disabled");
                    listaEnderecos = dados.ListaEntidades;

                    $("#optEnderecoCliente").append("<option "
                        + "value='00000000-0000-0000-0000-000000000000'>Adicionar novo endereço</option>");

                    if (dados.ListaEntidades.length == 1) {

                        var optionEndereco = "<option value='" + dados.ListaEntidades[0].Id + "' ";
                        if (preencherEndereco || dados.ListaEntidades[0].Id == $("#Entrega_ClienteEndereco_Id").val()) {
                            optionEndereco = optionEndereco + "selected";
                        }

                        optionEndereco = optionEndereco + ">" + dados.ListaEntidades[0].Endereco.Logradouro + "</option>";
                        $("#optEnderecoCliente").append(optionEndereco);

                        if (preencherEndereco || dados.ListaEntidades[0].Id == $("#Entrega_ClienteEndereco_Id").val()) {
                            PreencherEndereco(dados.ListaEntidades[0]);
                        }
                    } else {

                        for (var i = 0; i < dados.ListaEntidades.length; i++) {

                            var enderecoAdicionado = "<option value='" + dados.ListaEntidades[i].Id + "'";
                            if (!preencherEndereco && dados.ListaEntidades[i].Id == $("#Entrega_ClienteEndereco_Id").val()) {
                                enderecoAdicionado = enderecoAdicionado + " selected ";
                                PreencherEndereco(dados.ListaEntidades[i]);
                            }

                            enderecoAdicionado = enderecoAdicionado + ">" + dados.ListaEntidades[i].Endereco.Logradouro + "</option>";
                            $("#optEnderecoCliente").append(enderecoAdicionado);
                        }
                    }
                }

                $("#ClienteSelecionadoTab").click();
                EsconderCarregando("divCarregandoCliente");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de endereços. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregandoCliente");
        }
    });
}

// -------> Preenche os dados de endereço do cliente
function PreencherEndereco(endereco) {
    $("#Entrega_ClienteEndereco_Id").val(endereco.Id);
    $("#Entrega_ClienteEndereco_Endereco_Id").val(endereco.IdCep);
    $("#Entrega_ClienteEndereco_NumeroEndereco").val(endereco.NumeroEndereco);
    $("#Entrega_ClienteEndereco_ComplementoEndereco").val(endereco.ComplementoEndereco);
    $("#Entrega_ClienteEndereco_Endereco_Cep").val(FormatarCep(endereco.Endereco.Cep));
    $("#Entrega_ClienteEndereco_Endereco_Logradouro").val(endereco.Endereco.Logradouro);
    $("#Entrega_ClienteEndereco_Endereco_Bairro").val(endereco.Endereco.Bairro);
    $("#Entrega_ClienteEndereco_Endereco_Cidade").val(endereco.Endereco.Cidade);
    $("#Entrega_ClienteEndereco_Endereco_Bairro").change();
}

// -------> Limpa os dados do cliente
function LimparCliente() {
    $("#Cliente_Nome").val("");
    $("#Cliente_Telefone").val("");
    $("#Cliente_Cpf").val("");
    $("#Cliente_Id").val("00000000-0000-0000-0000-000000000000");
    $("#Pedido_IdCliente").val("00000000-0000-0000-0000-000000000000");

    $("#optEnderecoCliente option").remove();
    $("#optEnderecoCliente").append("<option>Nenhum endereço encontrado</option>");
    $("#optEnderecoCliente").attr("disabled", "disabled");

    LimparEndereco();
}

// -------> Limpa os dados do endereço
function LimparEndereco() {
    $("#Entrega_ClienteEndereco_Id").val("00000000-0000-0000-0000-000000000000");
    $("#Entrega_ClienteEndereco_Endereco_Id").val("00000000-0000-0000-0000-000000000000");
    $("#Entrega_ClienteEndereco_NumeroEndereco").val("");
    $("#Entrega_ClienteEndereco_ComplementoEndereco").val("");
    $("#Entrega_ClienteEndereco_Endereco_Logradouro").val("");
    $("#Entrega_ClienteEndereco_Endereco_Cidade").val("Americana");
    $("#Entrega_ClienteEndereco_Endereco_Bairro").val("");
    $("#Entrega_ClienteEndereco_Endereco_Cep").val("");
}

// -------> Preenche os campos de endereço ao selecionar um 
function SelecionarEndereco(idEndereco) {

    LimparEndereco();
    for (var i = 0; i < listaEnderecos.length; i++) {
        if (listaEnderecos[i].Id == idEndereco) {
            PreencherEndereco(listaEnderecos[i]);

            i = listaEnderecos.length;
        }
    }
}

// -------> Obtem a taxa de entrega de acordo com o bairro
function ObterTaxaEntregaBairro() {
    ExibirCarregando("divCarregandoTaxa");

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/TaxaEntrega/ObterTaxaPorBairro",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            bairroCidade: $("#Entrega_ClienteEndereco_Endereco_Bairro").val() + "_" + $("#Entrega_ClienteEndereco_Endereco_Cidade").val(),
        }),
        traditional: true,
        success: function (dados) {
            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa da taxa de entrega. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                EsconderCarregando("divCarregandoTaxa");
                return;

            } else if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao fazer a pesquisa da taxa de entrega. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregandoTaxa");
            } else {

                if (dados.Entidade == null) {

                    $("#TaxaEntrega").val("0,00");
                    $("#TaxaEntrega").change();
                    toastr.options.preventDuplicates = true;
                    toastr.info("A taxa de entrega não foi encontrada para este bairro", "Pesquisa de taxa de entrega");

                } else {
                    $("#TaxaEntrega").val(dados.Entidade.ValorTaxa.toFixed(2).replace(".", ","));
                    $("#TaxaEntrega").change();
                }

                EsconderCarregando("divCarregandoTaxa");
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

            EsconderCarregando("divCarregandoTaxa");
        }
    });
}
