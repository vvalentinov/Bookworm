// Function to filter the table
export function filterTableRows(filterValue) {
    let tableBodyElement = document.getElementsByClassName('table-body')[0];
    var rows = tableBodyElement.getElementsByTagName("tr");
    for (var i = rows.length - 1; i >= 0; i--) {
        if (rows[i].style.display == 'none') {
            rows[i].style.display = '';
        }
    }

    for (var i = rows.length - 1; i >= 0; i--) {
        var cellValue = rows[i].getElementsByTagName("td")[0].textContent;
        if (filterValue.includes(cellValue) == false) {
            rows[i].style.display = 'none';
        }
    }
}