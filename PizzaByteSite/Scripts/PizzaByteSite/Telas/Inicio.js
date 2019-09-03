$(document).ready(function () {
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
        data: [
            { y: 'Dom', Pizza: 26, Bebida: 12 }, 
            { y: 'Seg', Pizza: 12, Bebida: 2 },
            { y: 'Ter', Pizza: 15, Bebida: 3 },
            { y: 'Qua', Pizza: 13, Bebida: 1 },
            { y: 'Qui', Pizza: 16, Bebida: 4 },
            { y: 'Sex', Pizza: 37, Bebida: 18 },
            { y: 'Sab', Pizza: 49, Bebida: 20 }
        ],
        xkey: 'y',
        ykeys: ['Pizza', 'Bebida'],
        labels: ['Pizza', 'Bebida'],
        xlabels: 'day',
        lineColors: ['#a0d0e0', '#3c8dbc'],
        hideHover: 'auto',
        parseTime: false
    });
    var line = new Morris.Line({
        element: 'line-chart',
        resize: true,
        data: [
            { y: 'Jan', item1: 405 },
            { y: 'Fev', item1: 305 },
            { y: 'Mar', item1: 312 },
            { y: 'Abr', item1: 376 },
            { y: 'Mai', item1: 381 },
            { y: 'Jun', item1: 470 }
        ],
        xkey: 'y',
        ykeys: ['item1'],
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
        gridTextSize: 10,
        parseTime: false
    });

    // Donut Chart
    var donut = new Morris.Donut({
        element: 'sales-chart',
        resize: true,
        colors: ['#3c8dbc', '#f56954', '#00a65a'],
        data: [
            { label: 'Balcão', value: 12 },
            { label: 'Entrega', value: 30 },
            { label: 'Retirada', value: 20 }
        ],
        hideHover: 'auto'
    });

    // Fix for charts under tabs
    $('.box ul.nav a').on('shown.bs.tab', function () {
        area.redraw();
        donut.redraw();
        line.redraw();
    });
});
