$(document).ready(function () {
    $('#SendOrder').on('click', function () {
        

        let id = $(this).data('id');
        let name = "";
        let email = "";
        console.log("Adding favorite ID:", id);
        Swal.fire({
            title: "Multiple inputs",
             html: `<input id="Name" class="swal2-input" placeholder="Enter your name">
                    <input id="Email" class="swal2-input" placeholder="Enter your email">`,
            focusConfirm: false,
            preConfirm: () => {
                name = document.getElementById("Name").value;
                 email = document.getElementById("Email").value;

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
                    

                }
                catch (error) {
                    console.error('Error:', error);
                    $('#result').html = 'An error occurred';
                }
                return { name: name, email: email };
            }
        }).then((result) => {
            if (result.isConfirmed) {
                // Display the collected values
                Swal.fire({
                    title: "Collected Values",
                    text: `Name: ${result.value.name}, Email: ${result.value.email}`
                });
            }
        });

        console.log(name,email)
    });

});