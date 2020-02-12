function ObterInformacoesDashboard() {
    ExibirCarregando("divCarregando");

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Usuario/ObterInformacoesDashboard",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: "",
        traditional: true,
        success: function (dados) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao obter as informações. \n"
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
                    text: "Ocorreu um problema ao obter as informações. \n"
                        + "Se o problema continuar, entre em contato com o suporte. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {
                $("#labPedidos").html(dados.QuantidadePedidos);
                $("#labContasQuitadas").html(dados.PercentualContasQuitadas.toFixed(2).replace(".", ",") + '<sup style="font-size: 20px">%</sup>');
                $("#labNovosClientes").html(dados.QuantidadeNovosClientes);
                $("#labPedidosCancelados").html(dados.QuantidadePedidosCancelados);

                $('.connectedSortable').sortable({
                    placeholder: 'sort-highlight',
                    connectWith: '.connectedSortable',
                    handle: '.box-header, .nav-tabs',
                    forcePlaceholderSize: true,
                    zIndex: 999999
                });

                $('.connectedSortable .box-header, .connectedSortable .nav-tabs-custom').css('cursor', 'move');

                $('#chat-box').slimScroll({
                    height: '250px'
                });
                
                var area = new Morris.Area({
                    element: 'revenue-chart',
                    resize: true,
                    data: dados.ListaProdutosVendidosPorDiaSemana,
                    xkey: 'DiaSemana',
                    ykeys: ['Pizza', 'Bebida'],
                    labels: ['Pizza', 'Bebida'],
                    xlabels: 'DiaSemana',
                    lineColors: ['#a0d0e0', '#3c8dbc'],
                    hideHover: 'auto',
                    parseTime: false
                });

                var line = new Morris.Line({
                    element: 'line-chart',
                    resize: true,
                    data: dados.ListaPedidosPorMes,
                    xkey: 'Mes',
                    ykeys: ['Pedidos'],
                    labels: ['Pedidos'],
                    lineColors: ['#efefef'],
                    lineWidth: 2,
                    hideHover: 'auto',
                    gridTextColor: '#fff',
                    gridStrokeWidth: 0.4,
                    pointSize: 4,
                    pointStrokeColors: ['#efefef'],
                    gridLineColor: '#efefef',
                    gridTextFamily: 'Open Sans',
                    gridTextSize: 15,
                    parseTime: false
                });

                // Donut Chart
                var donut = new Morris.Donut({
                    element: 'sales-chart',
                    resize: true,
                    colors: ['#3c8dbc', '#f56954', '#00a65a'],
                    data: [
                        { label: 'Balcão', value: dados.PercentualPedidosBalcaoSemana },
                        { label: 'Entrega', value: dados.PercentualPedidosEntregaSemana },
                        { label: 'Retirada', value: dados.PercentualPedidosRetiradaSemana }
                    ],
                    hideHover: 'auto'
                });

                // Fix for charts under tabs
                $('.box ul.nav a').on('shown.bs.tab', function () {
                    area.redraw();
                    donut.redraw();
                    line.redraw();
                });
            }

            EsconderCarregando("divCarregando");
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao obter as informações. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}
