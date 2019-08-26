// Variável que determina o tipo de usuário
var tipoUsuario = 1;
function AlterarTipoUsuario() {
    tipoUsuario = 2;
} 

// Busca as mensagens de suporte enviadas
function BuscarMensagens(nPagina) {   

    ExibirCarregando();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Suporte/ObterLista",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            Pagina: nPagina,
            NaoPaginaPesquisa: false
        }),
        traditional: true,
        success: function (dados, status, request) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de mensagens de suporte. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de mensagens de suporte. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando();
            } else {

                if (dados.ListaEntidades.length == 0) {
                    toastr.info("Não foram encontrados mensagens de suporte", "Pesquisa de suporte");
                    $("#bntMaisMensagens").hide();
                } else {
                    for (var i = 0; i < dados.ListaEntidades.length; i++) {

                        var suporte = dados.ListaEntidades[i];
                        $("#divAddMensagens").prepend(RetornarMensagem(suporte.Tipo, suporte.Mensagem, suporte.Id, ConverterDataHoraJson(suporte.DataInclusao)));

                        if (nPagina === dados.NumeroPaginas) {
                            $("#bntMaisMensagens").hide();
                        } else {
                            $("#bntMaisMensagens").attr("onclick", "BuscarMensagens(" + (nPagina + 1) + ")");
                            $("#bntMaisMensagens").show();
                        }
                    }
                }

                // EsconderCarregando();
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de mensagens de suporte. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error + " " + request.abort + " " + status,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando();
        }
    });
}

// Formata uma mensagem enviada/recebida
function RetornarMensagem(tipoMensagem, mensagem, id, dataInclusao) {

    var mensagem = '<div class="item" id="' + id + '">' +
        ((tipoMensagem === 1) ? '<img src="/Content/PizzaByteSite/img/user2-160x160.jpg" class="online">' : '<img src="/Content/PizzaByteSite/img/Suporte.png" class="online">') +
        '<p class="message">' +
        '<a href="#" class="name" disabled>' +
        '<small class="text-muted pull-right"><i class="fa fa-clock-o"></i> ' + dataInclusao + '</small>' +
        ((tipoMensagem === 1) ? 'Usuário' : "Atendente") +
        '</a>' +
        ((tipoMensagem === tipoUsuario) ? '<button onclick="ApagarMensagem(\'' + id + '\')" class="btn btn-xs btn-danger pull-right"><i class="fa fa-trash"></i></button>' : '') +
        mensagem +
        '</p>' +
        '</div>';

    return mensagem;
}

// Envia uma mensagem de suporte
function EnviarMensagem() {

    ExibirCarregando();
    $("#btnEnviar").attr("disabled", "disabled");
    var suporteDto = new Object();

    suporteDto.Mensagem = $("#txtMensagem").val();
    suporteDto.Tipo = tipoUsuario;

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Suporte/IncluirMensagemSuporte",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            suporteDto: suporteDto,
        }),
        traditional: true,
        success: function (dados, status, request) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao incluir a mensagens de suporte. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                EsconderCarregando();
                $("#btnEnviar").removeAttr("disabled");
                return;
            }

            if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao incluir a mensagens de suporte. \n"
                        + "Se o problema continuar, entre em contato com o suporte por outra via de comunicação. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando();
                $("#btnEnviar").removeAttr("disabled");
            } else {

                var dataHoje = new Date();
                var data = Padl(dataHoje.getDate().toString(), 2, '0') + "/" + Padl((dataHoje.getMonth() + 1).toString(), 2, '0') + "/" + dataHoje.getFullYear().toString();
                data += " " + dataHoje.toString().substring(16, 24);

                $("#divAddMensagens").append(RetornarMensagem(tipoUsuario, $("#txtMensagem").val(), dados.Id, data));
                $("#txtMensagem").val("");

                var objDiv = document.getElementById("divAddMensagens");
                objDiv.scrollTop = objDiv.scrollHeight;

                swal({
                    title: "Mensagem enviada com sucesso!",
                    text: "Sua mensagem foi enviada, aguarde a resposta da sua solicitação.",
                    icon: "success",
                    button: "Ok",
                });

                EsconderCarregando();
                $("#btnEnviar").removeAttr("disabled");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de mensagens de suporte. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error + " " + request.abort + " " + status,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando();
            $("#btnEnviar").removeAttr("disabled");
        }
    });
}

// Apaga uma mensagem 
function ApagarMensagem(idSuporte) {
    if (idSuporte === null || idSuporte.length <= 0 || idSuporte === "") {
        swal({
            title: "Ops...",
            html: true,
            text: "Para apagar uma mensagem informe o ID, " +
                "caso o erro persista atualize a página e tente novamenteo.",
            icon: "warning",
            button: "Ok"
        });
    } else {

        $.ajax({
            type: "POST",
            url: RetornarEndereco() + "/Suporte/ExcluirMensagem",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({
                idMensagem: idSuporte
            }),
            traditional: true,
            success: function (dados) {

                if (dados.Error != undefined) {
                    swal({
                        title: "Ops...",
                        html: true,
                        text: "Ocorreu um problema ao excluir a mensagens de suporte. \n"
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
                        text: "Ocorreu um problema ao excluir a mensagens de suporte. \n"
                            + "Se o problema continuar, entre em contato com o suporte. \n"
                            + "Mensagem de retorno: " + dados.Mensagem,
                        icon: "warning",
                        button: "Ok",
                    });

                    EsconderCarregando();
                } else {

                    toastr.success("A mensagem foi excluída com sucesso!", "Mensagem excluída");
                    EsconderCarregando();
                    $("#" + idSuporte).hide();

                }
            },
            error: function (request, status, error) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao excluir a mensagens de suporte. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: \n" + error + " " + request.abort + " " + status,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando();
            }
        });
    }
}
