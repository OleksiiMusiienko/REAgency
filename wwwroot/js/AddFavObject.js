$(document).ready(function () {
    $('.favorite').on('click', function (event) {
        event.preventDefault();

        let id = $(this).data('id');
        console.log("Adding favorite ID:", id);

        const url = '/Home/AddFavObject';

        AddFavObject(url, id);
    });

    async function AddFavObject(url, id) {
        try {
            const response = await fetch(url, {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify(id)

            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const responseData = await response.json();
            // console.log(responseData);
            if (responseData.success === false) {
                Swal.fire({
                    position: "center",
                    icon: "error",
                    title: responseData.message,
                    showConfirmButton: false,
                    timer: 1500
                });
            }
            else {
                Swal.fire({
                    position: "center",
                    icon: "success",
                    title: responseData.message,
                    showConfirmButton: false,
                    timer: 1500
                });
            }

        }
        catch (error) {
            console.error('Error:', error);
            $('#result').html = 'An error occurred';
        }
    }
});
