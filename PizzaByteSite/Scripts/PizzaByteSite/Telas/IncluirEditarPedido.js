// Calcular o valor do troco de acordo com as formas de pagamento
function CalcularTroco() {
    var valorDebito = $("#RecebidoDebito").val();
    var valorDinheiro = $("#RecebidoDinheiro").val();
    var valorCredito = $("#RecebidoCredito").val();
    var taxaEntrega = $("#TaxaEntrega").val();
    var valorTotal = $("#Total").val();

    valorDebito = parseFloat(valorDebito.replace(",", "."));
    valorDinheiro = parseFloat(valorDinheiro.replace(",", "."));
    valorCredito = parseFloat(valorCredito.replace(",", "."));
    taxaEntrega = parseFloat(taxaEntrega.replace(",", "."));
    valorTotal = parseFloat(valorTotal.replace(",", "."));

    var valorTroco = (valorDebito + valorDinheiro + valorCredito) - valorTotal;

    if (valorTroco > 0) {
        $("#Troco").val(valorTroco.toFixed(2).replace(".", ","));
    } else {
        $("#Troco").val("0,00");
    }
}

// Recebe todo o valor da compra em uma forma de pagamento única
function ReceberTudo(campo) {
    $("#RecebidoDebito").val("0,00");
    $("#RecebidoDinheiro").val("0,00");
    $("#RecebidoCredito").val("0,00");
    $("#Troco").val("0,00");
    $("#" + campo).val($("#Total").val());
    $("#" + campo).focus();
}