// A simple templating method for replacing placeholders enclosed in curly braces.
if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}

$(function () {
    var ticker = $.connection.chatHub, //声明集线器代理
        up = '▲',
        down = '▼',
        $stockTable = $('#stockTable'),
        $stockTableBody = $stockTable.find('tbody'),
        rowTemplate = '<tr data-symbol="{Symbol}"><td>{ParkCode}</td><td>{ParkName}</td><td>{ParkCode}</td><td>{ParkCode}</td><td>{ParkCode}</td></tr>';

    function formatStock(stock) {
        return $.extend(stock, {
            ParkCode: stock.ParkCode,
            ParkName: stock.ParkName,
            ParkName: stock.ParkName
        });
    }

    function init() {
        //调用服务端方法获取股票价格
        ticker.server.getDate().done(function (stocks) {
            alert(stocks);
            $stockTableBody.empty();
            $.each(stocks, function () {
                var stock = formatStock(this);
                $stockTableBody.append(rowTemplate.supplant(stock));
            });
        });
    }


    //供服务端调用的方法，更新股票价格
    ticker.client.broadcastMessage = function (stock) {
        var displayStock = formatStock(stock),
            $row = $(rowTemplate.supplant(displayStock));

        $stockTableBody.find('tr[data-symbol=' + stock.Symbol + ']')
            .replaceWith($row);
    }
 

    //创建连接
    //$.connection.hub.logging = true;//启用日志
    $.connection.hub.start().done(init);
});