$(document).ready(function () {
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
            for (let i = 0; i < responseData.length; i++) {
                var newOption = document.createElement("option");
                newOption.value = i;
                newOption.text = responseData[i].name;
                localitySelect.add(newOption);
            }

        }
        catch (error) {
            console.error('Error:', error);
            $('#result').html = 'An error occurred';
        }
    }
});