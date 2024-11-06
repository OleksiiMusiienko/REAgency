
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
                window.location.assign('/Office/Index');
                alert("Об'єкт успішно вилучено!");
            }
        })
        .catch(error => console.error('Ошибка:', error));
});