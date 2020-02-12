var primeiraMeia = null;

// -------> Busa uma lista de produtos
function BuscarProdutosSelecao(nPagina) {

    ExibirCarregando("divCarregandoProduto");
    PaginarPesquisa(0, nPagina, "BuscarProdutosSelecao", "divPaginasProduto", "paginacaoProduto");
    $("#tblPesquisaProdutos tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Produto/ObterListaFiltrada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            Descricao: $("#txtDescricaoProduto").val(),
            PrecoInicial: $("#txtPrecoInicial").val(),
            PrecoFinal: $("#txtPrecoFinal").val(),
            Tipo: $("#optTipo").val(),
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
                    text: "Ocorreu um problema ao fazer a pesquisa de produtos. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                EsconderCarregando("divCarregandoProduto");
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

                EsconderCarregando("divCarregandoProduto");
            } else {

                if (dados.ListaEntidades.length == 0) {
                    EsconderCarregando("divCarregandoProduto");

                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontrados produtos com os filtros preenchidos", "Pesquisa de produtos");
                } else {
                    for (var i = 0; i < dados.ListaEntidades.length; i++) {

                        var botaoMeia = "";

                        // Se for pizza, opção de meia
                        if (dados.ListaEntidades[i].Tipo == 1) {
                            botaoMeia = "<button type='button' class='btn btn-sm btn-default' onclick='SelecionarMeia(\""
                                + dados.ListaEntidades[i].Id + "\", \"" + dados.ListaEntidades[i].Descricao
                                + "\", " + + dados.ListaEntidades[i].Preco + ")' tittle='Selecionar meia'><i class='fa fa-adjust'></i></button>";
                        }

                        var meiaSelecionada = "";
                        if (primeiraMeia != null) {
                            meiaSelecionada = "disabled";
                        }

                        $("#tblPesquisaProdutos tbody").append("<tr>"
                            + "<td><p title='" + dados.ListaEntidades[i].Detalhes + "'>" + dados.ListaEntidades[i].Descricao + "</p></td>"
                            + "<td>" + dados.ListaEntidades[i].Preco.toFixed(2).replace(".", ",") + "</td>"
                            + "<td hidden>" + dados.ListaEntidades[i].Tipo + "</td>"
                            + "<td><button type='button' class='btn btn-sm btn-primary addProduto' onclick='AdicionarProduto(\""
                            + dados.ListaEntidades[i].Id + "\", \""
                            + dados.ListaEntidades[i].Descricao + "\", "
                            + dados.ListaEntidades[i].Preco + ", \""
                            + dados.ListaEntidades[i].Tipo + "\", \"00000000-0000-0000-0000-000000000000\")' "
                            + "tittle='Adicionar ao pedido' " + meiaSelecionada + ">"
                            + "<i class='fa fa-cart-plus'></i></button> " + botaoMeia
                            + "</td></tr>");
                    }

                    EsconderCarregando("divCarregandoProduto");
                    PaginarPesquisa(dados.NumeroPaginas, nPagina, "BuscarProdutosSelecao", "divPaginasProduto", "paginacaoProduto");
                }
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

            EsconderCarregando("divCarregandoProduto");
        }
    });
}

// -------> Adiciona um produto ao pedido
function AdicionarProduto(idProduto, descricao, preco, tipo, idItemMeia) {

    var itemExistente = null;
    var numItem = document.getElementById("tblCorpoItens").getElementsByTagName("tr").length;
    for (var i = 0; i < numItem; i++) {
        var id = $("#ListaItens_" + i + "__IdProduto").val();
        var idComposto = $("#ListaItens_" + i + "__IdProdutoComposto").val();
        if (id === idProduto && (idComposto === idItemMeia || (idComposto === "" && idItemMeia === "00000000-0000-0000-0000-000000000000"))) {
            itemExistente = i;
        }
    }

    // Se não existir, incluir
    if (itemExistente == null) {
        $("#tblItensPedido tbody").append("<tr id='Linha" + numItem + "'>" +
            "<td><input style='display:none' id='ListaItens_" + numItem +
            "__DescricaoProduto' name='ListaItens[" +
            numItem + "].DescricaoProduto' type='text' class='form-control' value='" +
            descricao + "' />" + descricao + "</td>" +

            "<td><input id='ListaItens_" + numItem + "__Quantidade' name='ListaItens[" +
            numItem + "].Quantidade' type='text' class='form-control quantidade' value='1' " +
            "onfocus='$(this).maskMoney({ " + "allowNegative: false, precision: 0, " +
            "thousands: \"\", allowZero: true })' onchange='AtualizarTotal()' /></td>" +

            "<td><input type='text' id='ListaItens_" + numItem + "__PrecoProduto' name='ListaItens[" +
            numItem + "].PrecoProduto' class='form-control preco' onchange='AtualizarTotal()' value='" +
            preco.toFixed(2).replace(".", ",") + "' onfocus='$(this).maskMoney({ decimal: \",\", " +
            "allowNegative: false, precision: 2, thousands: \"\", allowZero: true })' /></td>" +

            "<td style='display:none'><input type='text' id='ListaItens_" + numItem +
            "__TipoProduto' name='ListaItens[" + numItem + "].TipoProduto' class='form-control' " +
            "value='" + tipo + "' /></td>" +

            "<td style='display:none'><input type='text' id='ListaItens_" + numItem +
            "__Id' name='ListaItens[" + numItem + "].Id' class='form-control' " +
            "value='00000000-0000-0000-0000-000000000000' /></td>" +

            "<td style='display:none'><input type='text' id='ListaItens_" + numItem +
            "__IdProduto' name='ListaItens[" + numItem + "].IdProduto' class='form-control' " +
            "value='" + idProduto + "' /></td>" +

            "<td style='display:none'><input type='text' id='ListaItens_" + numItem +
            "__IdProdutoComposto' name='ListaItens[" + numItem + "].IdProdutoComposto' class='form-control' " +
            "value='" + idItemMeia + "' /></td>" +

            "<td><button type='button' class='btn btn-danger' tittle='Remover do pedido' " +
            "onclick='RemoverItem(\"" + numItem + "\")'><i class='fa fa-remove'></i></button></td>" +
            "</tr>");
    } else {
        var quantidade = $("#ListaItens_" + itemExistente + "__Quantidade").val();
        quantidade = parseFloat(quantidade);

        quantidade++;
        $("#ListaItens_" + itemExistente + "__Quantidade").val(quantidade.toString());
        $("#Linha" + itemExistente).removeAttr("style");
    }

    toastr.options = {
        "showDuration": "400",
        "hideDuration": "600",
        "timeOut": "3000",
        "positionClass": "toast-top-right",
        "preventDuplicates": "true"
    }

    toastr.success("Produto adicionado ao pedido com sucesso", "Tudo certo!");
    AtualizarTotal();
}

// -------> Remove um item da lista
function RemoverItem(indice) {
    $("#Linha" + indice).attr("style", "display:none");
    $("#ListaItens_" + indice + "__Quantidade").attr("value", "0");
    $("#ListaItens_" + indice + "__Quantidade").val("0");

    AtualizarTotal();
}

// -------> Atualiza o valor total do pedido
function AtualizarTotal() {
    var totalAtual = 0;
    var quantidadePizzas = 0;

    // Somar todos os itens
    for (var i = 0; i < document.getElementById("tblCorpoItens").getElementsByTagName("tr").length; i++) {
        var valor = $("#ListaItens_" + i + "__PrecoProduto").val();
        valor = parseFloat(valor.replace(",", "."));

        var quantidade = $("#ListaItens_" + i + "__Quantidade").val();
        quantidade = parseInt(quantidade);

        totalAtual += (valor * quantidade);

        if ($("#ListaItens_" + i + "__TipoProduto").val() == "1" ||
            $("#ListaItens_" + i + "__TipoProduto").val() == 1) {
            var quantidadePizza = $("#ListaItens_" + i + "__Quantidade").val();
            quantidadePizza = parseFloat(quantidadePizza);

            quantidadePizzas += quantidadePizza;
        }
    }

    var entrega = $("#TaxaEntrega").val();
    entrega = parseFloat(entrega.replace(",", "."));
    totalAtual += entrega;

    $("#Total").val(totalAtual.toFixed(2).replace(".", ","));
    VerificarProdutoPromocao(quantidadePizzas);
    CalcularTroco();
}

// -------> Adicionar pizza meio a meio
function SelecionarMeia(id, descricao, preco) {
    if (primeiraMeia == null) {
        primeiraMeia = new Object();
        primeiraMeia.Id = id;
        primeiraMeia.Descricao = descricao;
        primeiraMeia.Preco = preco;
        $(".addProduto").attr("disabled", "disabled");

    } else {

        if (id == primeiraMeia.Id) {
            primeiraMeia = null;
            $(".addProduto").removeAttr("disabled");

        } else {
            var precoItem = preco;
            if (primeiraMeia.Preco > preco) {
                precoItem = primeiraMeia.Preco;
            }

            AdicionarProduto(id, primeiraMeia.Descricao + "/" + descricao, precoItem, 1, primeiraMeia.Id);
            primeiraMeia = null;
            $(".addProduto").removeAttr("disabled");
        }
    }
}

// -------> Ao apareder a modal, focar no campo de pesquisa
$('#modalProduto').on('shown.bs.modal', function (e) {
    $("#txtDescricaoProduto").val("");
    $("#txtPrecoInicial").val("0,00");
    $("#txtPrecoFinal").val("0,00");
    $("#txtDescricaoProduto").focus();
});

// -------> Verifica se o pedido está entrando na promoção
function VerificarProdutoPromocao(quantidadePizzas) {
    if ($("#DiaPromocao").val() == "True" || $("#DiaPromocao").val() == true) {
        // Se tiver duas, incluir o refri
        if (quantidadePizzas >= 2) {
            var quantidade = $("#ListaItens_0__Quantidade").val();
            quantidade = parseFloat(quantidade);

            if (quantidade <= 0) {
                quantidade++;
            }

            $("#ListaItens_0__Quantidade").val(quantidade.toString());
            $("#Linha0").removeAttr("style");
        } else {
            $("#Linha0").attr("style", "display:none");
            $("#ListaItens_0__Quantidade").attr("value", "0");
            $("#ListaItens_0__Quantidade").val("0");
        }
    }
}