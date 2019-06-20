function MostrarMensagemRetorno(retornoMensagem, inicioMensagem) {
    toastr.options = {
        "showDuration": "400",
        "hideDuration": "600",
        "timeOut": "3000",
        "positionClass": "toast-top-right"
    }

    switch (retornoMensagem) {
        case "INCLUIDO":
            toastr.success(inicioMensagem + " foi incluído com sucesso!", "Produto incluído");
            break;

        case "ALTERADO":
            toastr.success(inicioMensagem + " foi alterado com sucesso!");
            break;

        case "EXCLUIDO":
            toastr.success(inicioMensagem + " foi excluído com sucesso!");
            break;

        default:
            break;
    }
}

function RetornarEndereco() {
    var enderecoServico = "http://localhost:55751/";
    return enderecoServico;
}

function PaginarPesquisa() {

}

function ExibirCarregando() {
    $("#divCarregando").attr("class", "overlay");
    $("#divCarregando").hide();

}

function EsconderCarregando() {
    $("#divCarregando").removeAttr("class");
    $("#divCarregando").hide();

}

function ObterEndereco(cepPesquisado) {
    $("#Endereco_Logradouro").attr("readonly", "readonly");
    $("#Endereco_Cidade").attr("readonly", "readonly");
    $("#Endereco_Bairro").attr("readonly", "readonly");

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Cep/ObterPorCep",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            cep: cepPesquisado,
        }),
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

                return;
            }

            if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao fazer a pesquisa de CEP. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

            } else {

                if (dados.Entidade != null) {
                    $("#Endereco_Logradouro").val(dados.Entidade.Logradouro);
                    $("#Endereco_Cidade").val(dados.Entidade.Cidade);
                    $("#Endereco_Bairro").val(dados.Entidade.Bairro);
                    $("#Endereco_Id").val(dados.Entidade.Id);

                } else {
                    toastr.success("CEP não encontrado. Para adicionar, preencha os dados do endereço.", "Pesquisa de CEP");

                    $("#Endereco_Id").val("00000000-0000-0000-0000-000000000000");
                    $("#Endereco_Logradouro").removeAttr("readonly");
                    $("#Endereco_Cidade").removeAttr("readonly");
                    $("#Endereco_Bairro").removeAttr("readonly");
                    $("#Endereco_Logradouro").focus();
                }
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de CEP. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: " + dados.Mensagem,
                icon: "warning",
                button: "Ok",
            });
        }
    });
}

$(document).keypress(function (e) {
    if (e.keyCode === 13) {
        e.preventDefault();
        return false;
    }
});