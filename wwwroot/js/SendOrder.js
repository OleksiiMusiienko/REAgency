$(document).ready(function () {
    $('#Send').on('click', function () {
        

        let id = $(this).data('id');
        let name = "";
        let email = "";
        let url = "/Home/OrderViewing";
       
        Swal.fire({
            title: "Заповните заявку",
             html: `<input id="Name" class="swal2-input" placeholder="Ім'я">
                    <input id="Email" class="swal2-input" placeholder="Email">
                    <input id="Phone" class="swal2-input" placeholder="Номер телефону">`,
            focusConfirm: false,
            showCancelButton: true,
            confirmButtonColor: "#052a5e",
            preConfirm: async () => {
                name = document.getElementById("Name").value;
                email = document.getElementById("Email").value;
                phone = document.getElementById("Phone").value;

                const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                const phonePattern = /^\d{10}$/;

                if (!emailPattern.test(email)) {
                    Swal.showValidationMessage("Введіть дійсну електронну адресу.");
                    return false;
                }

                if (!phonePattern.test(phone)) {
                    Swal.showValidationMessage("Введіть дійсний 10-значний номер телефону.");
                    return false;
                }

                try {
                    const response = await fetch(url, {
                        method: "POST",
                        headers: {
                            'Content-Type': 'application/json;charset=utf-8'
                        },
                        body: JSON.stringify({
                            name: name,
                            email: email,
                            phone: phone,
                            id: id
                        })

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
                return { name: name, email: email };
            }
        })
       
    });

});