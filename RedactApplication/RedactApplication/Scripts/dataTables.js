$(document).ready(function () {
    $('.usr-srch--input').on('search', function (e) {
        if ('' == this.value) {
            jQuery(this).trigger('keyup');
        }
    });

    $('.usr-srch').submit(function (e) {
        e.preventDefault();
    })

    $(".cancelText").click(function () {
        $('input[type=search]').val("");
        $('input[type=search]').trigger('keyup');
    })

    $('.usr-srch--input').keyup(function () {
        var _val = $(this).val();
        $('input[type=search]').val(_val);
        $('.dataTables_filter input[type=search]').keyup();

        if (_val.length > 0)
            $(".cancelText").css('visibility', 'visible')
        else
            $(".cancelText").css('visibility', 'hidden')
    });

    if (jQuery('#myDataTableListCampaign').length > 0) {
        $('#myDataTableListCampaign').dataTable({
            "dom": '<"top"f>rt<"bottom"ipl><"clear">',
            "language": {
                "search": '<i class="fa fa-search"></i>',
                "searchPlaceholder": "search",
                "emptyTable": "No records found.",
                "zeroRecords": "No records found."
            },
            "stripeClasses": ['odd-row', 'even-row'],
        });
    }
    
    if (jQuery('#myDataTable').length > 0) {
        $('#myDataTable').dataTable({
            "dom": '<"top"f>rt<"bottom"ipl><"clear">',
            "language": {
                "search": '<i class="fa fa-search"></i>',
                "searchPlaceholder": "search",
                "emptyTable": "No records found.",
                "zeroRecords": "No records found."
            },
            "stripeClasses": ['odd-row', 'even-row'],

            initComplete: function () {
                this.api().columns().every(function () {
                    var column = this;

                    if (column.selector.cols == 4) {
                        var select = $('<select><option value=""></option></select>')
                            .appendTo($(column.footer()).empty())
                            .on('change', function () {
                                var val = $.fn.dataTable.util.escapeRegex(
                                    $(this).val()
                                );

                                column
                                    .search(val ? '^' + val + '$' : '', true, false)
                                    .draw();
                            });

                        column.data().unique().sort().each(function (d, j) {
                            select.append('<option value="' + d + '">' + d + '</option>')
                        });


                    }

                });
            }
        });
    }
})