$(document).ready(function () {
    //$('#selectLocality').on('change', function (event) {

    //    let localitySelect = document.getElementById('selectLocality');
    //    localitySelect.value = this.value;
    //});
    $('#selectDistrict').on('change', function (event) {
        let district = this.value;
        const url = '/Office/GetLocality/';
        var response = GetItems(url, district);
    }); 
    async function GetItems(url, id) {
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
            let localitySelect = document.getElementById('selectLocality');
            if (responseData.length == 0) {
                localitySelect.innerHTML = "";
            }
            else {
                for (let i = 0; i < responseData.length; i++) {
                    var newOption = document.createElement("option");
                    newOption.value = i+1;
                    newOption.text = responseData[i].name;
                    localitySelect.add(newOption);
                }
            }

        }
        catch (error) {
            console.error('Error:', error);
            $('#result').html = 'An error occurred';
        }
    }
});
