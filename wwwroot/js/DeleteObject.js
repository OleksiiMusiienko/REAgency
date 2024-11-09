
document.getElementById("myButton").addEventListener("click", function () {
    
    id = $(this).data('id');

    fetch('/Office/Delete', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(id)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                Swal.fire({
                    position: "center",
                    icon: "success",
                    title: data.message,
                    showConfirmButton: false,
                    timer: 2000
                });
                window.location.assign('/Office/Index');
            }
        })
        .catch(error => console.error('Ошибка:', error));
});