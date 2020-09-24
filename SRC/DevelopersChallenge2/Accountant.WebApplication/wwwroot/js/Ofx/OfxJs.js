class OfxJs {
    loadAll() {
        this.loadTotals();
        this.loadTable();
        this.getTimeChart();
    }

    loadTotals() {
        $(".loader-totals").show()
        $.ajax({
            type: "POST",
            url: '?handler=Totals',
            headers: {
                RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
            }
        })
            .done(function (response) {
                var currency = "R$ ";

                $("#Total").html(currency + response.total);
                $("#TotalCredits").html(currency + response.totalCredits);
                $("#TotalDebits").html(currency + response.totalDebits);
                $("#TotalMovements").html(response.totalMovements);

                $(".loader-totals").hide()
            })
            .fail(function () {
                alert("Error to load total sales");
            });
    }

    getTimeChart() {
        $("#loader-line-chart").show()

        $.ajax({
            type: "POST",
            url: '?handler=TimeChart',
            headers: {
                RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
            }
        })
            .done(function (response) {
                if (response.values.length < 1) {
                    $("#loader-line-chart").hide();
                    return;
                }

                var chartTitle = response.chartTitle;
                var dates = response.labels;
                var values = response.values;
                var blue = 'rgb(54, 162, 235)';

                var config = {
                    type: 'line',
                    data: {
                        labels: dates,
                        datasets: [{
                            label: chartTitle,
                            data: values,
                            backgroundColor: blue,
                            borderColor: blue,
                            fill: false,
                        }],
                        
                    },

                    options: {
                        title: {
                            text: chartTitle
                        },
                        responsive: true,
                        scales: {
                            xAxes: [{
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Date'
                                }
                            }],
                            yAxes: [{
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Value'
                                }
                            }]
                        },
                    }
                };

                if (myChartInTime != null) {
                    myChartInTime.destroy();
                }

                var canvas = document.getElementById('myChartInTime');
                var ctx = canvas.getContext("2d");
                myChartInTime = new Chart(ctx, config);
                $("#loader-line-chart").hide()
            })
            .fail(function () {
                alert("Error to load line chart");
            });
    }

    loadTable() {
        $("#loader-table").show();
        $.ajax({
            type: "POST",
            url: '?handler=LoadTable',
            headers: {
                RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
            }
        })
            .done(function (response) {
                if (response.header.length < 1) {
                    $("#loader-table").hide();
                    return;
                }

                var table = $("#myTable");
                table.html("");

                var trHead = $("<thead><tr></tr></thead>");
                var trHeadHtml = "";

                $.each(response.header, function (index, item) {

                    trHeadHtml += `<th scope=\"col\" id=${item.replace("/", "")}> ${item}</th>`;
                });

                trHead.html(`<tr>${trHeadHtml}</tr>`);
                table.append(trHead);


                var trBody = $("<tbody></tbody>");
                var trBodyHtml = "";

                $.each(response.items, function (index, item) {

                    var line = "<tr>";
                    var firstColumn = true;

                    $.each(item, function (index2, itemValue) {

                        if (firstColumn) {
                            firstColumn = false;
                            line += "<th scope=\"row\">" + itemValue + "</th>";
                        }
                        else {
                            line += "<td>" + itemValue + "</td>";
                        }
                    });

                    line += "</tr>";
                    trBodyHtml += line;
                });

                trBody.html(trBodyHtml);
                table.append(trBody);

                var table = $('#myTable').DataTable();

                $("#loader-table").hide();
            })
            .fail(function () {
                alert("Error to ofx table");
            });
    }
}

var ofxJs = new OfxJs();
var myChartInTime = null;
