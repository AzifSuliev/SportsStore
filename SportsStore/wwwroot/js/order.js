var dataTable;
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        }
        else {
            if (url.includes("approved")) {
                loadDataTable("approved");
            }
            else {
                loadDataTable("all");
            }
        }
    }  
    loadDataTable()
});

function loadDataTable(status) {
    // код из вкладки Ajax сайта DataTables
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/order/getall?status=' + status }, // путь к методу getall + аргумент status 
        // код из вкладки Data сайта DataTables
        "columns": [
            { data: 'id', "width": "10%" },
            { data: 'name', "width": "40%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'appUser.email', "width": "10%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                // Оба упоминания свойства id используются для разных целей:
                // одно для отображения значения id в таблице,
                // а другое для формирования ссылки на страницу с деталями заказа.
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                         <a href="/admin/order/details?orderId=${data}"  class="btn btn-primary" style="height: 40px; width: 160px; border-radius: 10px;"> 
                         <i class="bi bi-pencil-square"></i></a>
                    </div>`
                },
                "width": "10%"
            },
        ]
    });
}